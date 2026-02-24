using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Button _toggleModeButton;
    [SerializeField] private TextMeshProUGUI _modeButtonLabel;
    [SerializeField] private Transform _cellsParent;
    [SerializeField] private CellView _cellPrefab;
    
    private float _spacing;
    private CellViewPool _cellPool;
    private List<CellView> _activeCells = new List<CellView>();
    private Board _board;
    private GameController _gameController;

    private void OnEnable()
    {
        if (_toggleModeButton != null)
            _toggleModeButton.onClick.AddListener(OnToggleModeButtonClick);
    }

    private void OnDisable()
    {
        if (_toggleModeButton != null)
            _toggleModeButton.onClick.RemoveListener(OnToggleModeButtonClick);
    }

    private void OnToggleModeButtonClick()
    {
        bool newFlagMode = !_gameController.IsFlagMode;
        _gameController.SetMarkMode(newFlagMode);

        _modeButtonLabel.text = newFlagMode ? "Mark" : "Reveal";
    }

    public void Init(Board board, GameController gameController, CameraController cameraController)
    {
        if (board == null) throw new System.ArgumentNullException(nameof(board));
        if (gameController == null) throw new System.ArgumentNullException(nameof(gameController));
        if (cameraController == null) throw new System.ArgumentNullException(nameof(cameraController));

        // Subscribe directly to Board events.
        // - OnCellRevealed: triggered when a cell is revealed in the model.
        // - OnFlagToggled: triggered when a cell is marked/unmarked.
        // BoardView listens to these events to update the visual state.
        board.OnCellRevealed += UpdateCell;
        board.OnFlagToggled += UpdateCell;

        _spacing = _cellPrefab.CellSize * 0.1f; // 10% cell's size
        GenerateBoard(board, gameController);
        
        _gameController = gameController;
        
        cameraController.FitCameraToBoard(board.Rows, board.Columns, _cellPrefab.CellSize, _spacing);
    }

    // Generate the visual board based on the model data. 
    // For each cell in the board, request a CellView from the pool, 
    // initialize it with the cell data and the click callback, 
    // and add it to the grid layout.
    public void GenerateBoard(Board board, GameController gameController)
    {
        _board = board;

        ClearBoard();
        
        CellViewController cellViewController = new CellViewController(gameController);

        int cellsNeeded = board.Columns * board.Rows;

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
        float offsetX = -(board.Columns - 1) * step / 2f;
        float offsetY = (board.Rows - 1) * step / 2f;

        // Instantiate visual cells from the pool
        for (int row = 0; row < board.Rows; row++)
        {
            for (int column = 0; column < board.Columns; column++)
            {
                Cell cell = board.GetCell(column, row);

                CellView cellView = _cellPool.Get();
                cellView.transform.SetParent(_cellsParent, false);

                // Cell's position in world space (centered)
                cellView.transform.localPosition = new Vector3(column * step + offsetX, -(row * step - offsetY), 0);

                // Initializes the visual cell:
                // Cell model data, his coordinates on the grid.
                // and the click callback (HandleCellClick) provided by CellViewController.
                // When the player clicks
                // the event is forwared to the controller for handling.
                cellView.Init(cell, column, row, cellViewController.HandleCellClick);

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

    // UpdateCell is called when Board triggers OnCellRevealed or OnFlagToggled.
    // It finds the matching CellView by coordinates and refreshes its visual state.
    public void UpdateCell(int column, int row)
    {
        Cell cell = _board.GetCell(column, row);

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