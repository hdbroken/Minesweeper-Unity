using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Main board view. 
/// - Initialized with a BoardViewController and a CameraController. 
/// - Subscribes to BoardViewController event: 
///   OnCellUpdated: refreshes the visual cell.
/// - Generates the cell grid using a pool (CellViewPool).
/// </summary>
public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Transform _cellsParent;
    [SerializeField] private CellView _cellPrefab;

    private CellViewPool _cellPool;
    private List<CellView> _activeCells = new List<CellView>();

    private BoardViewController _boardViewController;
    private float _spacing;

    /// <summary>
    /// Cleanup when the view is disabled:
    /// - Unsubscribes from BoardViewController events. 
    /// </summary>
    private void OnDisable()
    {
        _boardViewController.OnCellUpdated -= UpdateCell;
        _boardViewController.UnsubscribeEvents();
    }

    /// <summary>
    /// Initializes the view with the camera and board controllers.
    /// - Subscribes to OnCellUpdated. 
    /// - Initializes the mine counter.
    /// - Generates the visual grid. 
    /// - Adjusts the camera to board size.
    /// </summary>
    public void Init(BoardViewController boardViewController, CameraController cameraController)
    {
        if (cameraController == null) throw new ArgumentNullException(nameof(cameraController));
        _boardViewController = boardViewController ?? throw new ArgumentNullException(nameof(boardViewController));

        _boardViewController.OnCellUpdated += UpdateCell;

        _boardViewController.InitializeMineCounter();

        _spacing = _cellPrefab.CellSize * 0.1f; // 10% cell's size

        GenerateBoard(_boardViewController.Columns, _boardViewController.Rows);

        cameraController.FitCameraToBoard(_boardViewController.Rows, _boardViewController.Columns, _cellPrefab.CellSize, _spacing);
    }

    /// <summary> 
    /// Generates the visual grid: 
    /// - Requests cells from the pool. 
    /// - Positions them centered on the board.
    /// - Initializes each cell with its data and click callback. 
    /// </summary>
    public void GenerateBoard(int columns, int rows)
    {
        ClearBoard();

        int cellsNeeded = columns * rows;

        // Create or ensure pool capacity with the correct size
        if (_cellPool == null)
        {
            _cellPool = new CellViewPool(_cellPrefab, cellsNeeded, _cellsParent);
        }
        else
        {
            _cellPool.EnsureCapacity(cellsNeeded);
        }

        float step = _cellPrefab.CellSize + _spacing;

        // Calculate offset to center the board
        float offsetX = -(columns - 1) * step / 2f;
        float offsetY = (rows - 1) * step / 2f;

        // Instantiate visual cells from the pool
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Cell cell = _boardViewController.GetCell(column, row);
#if UNITY_EDITOR
                string content = cell.IsMine ? "💣" : cell.ProximityCount.ToString();
                Debug.Log($"Cell[column: {column},row: {row}] → {content}");
#endif
                CellView cellView = _cellPool.Get();
                cellView.transform.SetParent(_cellsParent, false);

                // Cell's position in world space (centered)
                cellView.transform.localPosition = new Vector3(column * step + offsetX, -(row * step - offsetY), 0);

                // Initializes the visual cell:
                // Cell model data, his coordinates on the grid.
                // and the click callback (HandleCellClick) provided by CellViewController.
                // When the player clicks
                // the event is forwared to the controller for handling.
                cellView.Init(cell, column, row, _boardViewController.CellViewController.HandleCellClick);

                _activeCells.Add(cellView);
            }
        }
    }

    public void ClearBoard()
    {
        if (_activeCells == null) return;

        foreach (CellView cellView in _activeCells)
        {
            _cellPool?.Return(cellView);
        }
        _activeCells.Clear();
    }

    /// <summary> 
    /// Refreshes the visual cell at the given coordinates. 
    /// Triggered when BoardViewController raises OnCellUpdated. 
    /// </summary>
    public void UpdateCell(int column, int row)
    {
        Cell cell = _boardViewController.GetCell(column, row);

        foreach (CellView cellView in _activeCells)
        {
            if (cellView.Column == column && cellView.Row == row)
            {
                cellView.UpdateVisual();
                break;
            }
        }
    }
}