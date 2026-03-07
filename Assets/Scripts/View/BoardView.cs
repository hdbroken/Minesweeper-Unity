using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 
/// Main board view. 
/// - Initialized with a BoardViewController and a CameraController. 
/// - Subscribes to BoardViewController events: 
///   OnCellUpdated: refreshes the visual cell. 
///   OnMineCounterChanged: updates the mine counter. 
/// - Generates the cell grid using a pool (CellViewPool). 
/// - Handles the mode toggle button (Reveal/Mark). 
/// - Updates the timer on screen while the game is active. 
/// </summary>
public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Button _toggleModeButton;
    [SerializeField] private TextMeshProUGUI _modeButtonText;
    [SerializeField] private TextMeshProUGUI _mineCounterText;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private Transform _cellsParent;
    [SerializeField] private CellView _cellPrefab;

    private CellViewPool _cellPool;
    private List<CellView> _activeCells = new List<CellView>();

    private BoardViewController _boardViewController;
    private Coroutine _updateTimer;
    private float _spacing;

    private void OnEnable()
    {
        if (_toggleModeButton != null)
            _toggleModeButton.onClick.AddListener(OnToggleModeButtonClick);
    }

    /// <summary>
    /// Cleanup when the view is disabled:
    /// - Removes button listener.
    /// - Stops the timer coroutine.
    /// - Unsubscribes from BoardViewController events. 
    /// </summary>
    private void OnDisable()
    {
        if (_toggleModeButton != null)
            _toggleModeButton.onClick.RemoveListener(OnToggleModeButtonClick);

        if (_updateTimer != null)
        {
            StopCoroutine(_updateTimer);
            _updateTimer = null;
        }

        _boardViewController.OnCellUpdated -= UpdateCell;
        _boardViewController.OnMineCounterChanged -= UpdateMineCounterText;
        _boardViewController.UnsubscribeEvents();
    }

    private void OnToggleModeButtonClick()
    {
        bool newFlagMode = !_boardViewController.IsFlagMode;
        _boardViewController.ToggleMarkMode();

        _modeButtonText.text = newFlagMode ? "Mark" : "Reveal";
    }

    /// <summary>
    /// Initializes the view with the controller and camera and board controller.
    /// - Subscribes to OnCellUpdated and OnMineCounterChanged. 
    /// - Initializes the mine counter. 
    /// - Generates the visual grid. 
    /// - Adjusts the camera to board size. 
    /// - Starts the timer coroutine. 
    /// </summary>
    public void Init(BoardViewController boardViewController, CameraController cameraController)
    {
        if (cameraController == null) throw new ArgumentNullException(nameof(cameraController));
        if (boardViewController == null) throw new ArgumentNullException(nameof(boardViewController));

        _boardViewController = boardViewController;
        // Subscribe directly to Board events.
        // - OnCellRevealed: triggered when a cell is revealed in the model.
        // - OnFlagToggled: triggered when a cell is marked/unmarked.
        // BoardView listens to these events to update the visual state.
        _boardViewController.OnCellUpdated += UpdateCell;
        _boardViewController.OnMineCounterChanged += UpdateMineCounterText;

        _boardViewController.InitializeMineCounter();

        _spacing = _cellPrefab.CellSize * 0.1f; // 10% cell's size

        GenerateBoard(_boardViewController.Columns, _boardViewController.Rows);

        cameraController.FitCameraToBoard(_boardViewController.Rows, _boardViewController.Columns, _cellPrefab.CellSize, _spacing);

        _updateTimer = StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (_boardViewController.IsTimerRunning)
        {
            _timerText.text = FormatTime(_boardViewController.GetTime);
            yield return new WaitForSeconds(.1f);
        }
        // Force a final refresh to display the precise stop time (not the last 0.1s update)
        _timerText.text = FormatTime(_boardViewController.GetTime);
    }

    private string FormatTime(TimeSpan time)
    {
        // Formats time as MM:SS.mmm (e.g. 02:15.347)
        return string.Format("{0:D2}:{1:D2}.{2:D3}",
            time.Minutes,
            time.Seconds,
            time.Milliseconds);
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

    private void UpdateMineCounterText(int remainingMines)
    {
        _mineCounterText.text = remainingMines.ToString();
    }
}