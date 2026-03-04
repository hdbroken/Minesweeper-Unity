using System;

/// <summary> 
/// Intermediate controller between the game logic (GameController) and the view (BoardView). 
/// - Subscribes to GameController events (OnCellRevealed, OnFlagToggled). 
/// - Translates those events into its own events (OnCellUpdated, OnMineCounterChanged) that the view consumes. 
/// - Exposes minimal necessary properties (Rows, Columns, IsFlagMode, IsTimerRunning, GetTime). 
/// - Contains a CellViewController to handle cell clicks. 
/// </summary>
public class BoardViewController
{
    private readonly GameController _gameController;
    private readonly CellViewController _cellViewController;

    // Events exposed to the view: 
    // - OnCellUpdated: notifies that a cell changed (revealed or flagged). 
    // - OnMineCounterChanged: notifies that the mine counter must be updated.
    public event Action<int, int> OnCellUpdated; 
    public event Action<int> OnMineCounterChanged;

    // State properties that the view needs to display.
    public bool IsTimerRunning => _gameController.IsTimerRunning;
    public TimeSpan GetTime => _gameController.GetTime();
    public bool IsFlagMode => _gameController.IsFlagMode;
    // Access to board dimensions (without exposing the Board directly).
    public int Rows => _gameController.GetBoardDimensions().Rows; 
    public int Columns => _gameController.GetBoardDimensions().Columns;
    // Access to the cell controller (for clicks).
    public CellViewController CellViewController => _cellViewController;
    public Cell GetCell(int column, int row) => _gameController.GetCell(column, row);

    /// <summary>
    /// Constructor: receives the GameController and subscribes to its events.
    /// - OnCellRevealed -> HandleCellChanged -> triggers OnCellUpdated.
    /// - OnFlagToggled -> HandleCellChanged + HandleMineCounterChanged -> triggers OnCellUpdated and OnMineCounterChanged.
    /// </summary>
    public BoardViewController(GameController gameController)
    {
        if (gameController == null) throw new ArgumentNullException(nameof(gameController));

        _gameController = gameController;
        _cellViewController = new CellViewController(gameController);

        _gameController.OnCellRevealed += HandleCellChanged;
        _gameController.OnFlagToggled += HandleCellChanged;
        _gameController.OnFlagToggled += HandleMineCounterChanged;
    }

    /// <summary> 
    /// Cleanup: unsubscribes all events from the GameController.
    /// Called from OnDisable in BoardView. 
    /// </summary>
    public void UnsubscribeEvents()
    {
        _gameController.OnCellRevealed -= HandleCellChanged;
        _gameController.OnFlagToggled -= HandleCellChanged;
        _gameController.OnFlagToggled -= HandleMineCounterChanged;
    }

    public void ToggleMarkMode()
    {
        _gameController.SetMarkMode(!_gameController.IsFlagMode);
    }

    /// <summary> 
    /// Internal handler: when a cell changes, notifies the view.
    /// </summary>
    private void HandleCellChanged(int column, int row)
    {
        Cell cell = _gameController.GetCell(column, row);
        OnCellUpdated?.Invoke(column, row); 
    }

    /// <summary>
    /// Internal handler: when a cell is flagged/unflagged, recalculates remaining mines and notifies the view.
    /// </summary>
    private void HandleMineCounterChanged(int column, int row)
    {
        int remainingMines = _gameController.MineCount - _gameController.FlagCount;
        OnMineCounterChanged?.Invoke(remainingMines);
    }

    public void InitializeMineCounter()
    {
        HandleMineCounterChanged(0, 0);
    }
}