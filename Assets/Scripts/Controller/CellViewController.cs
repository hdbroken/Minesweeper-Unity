/// <summary>
/// Cell interaction controller.
/// - Receives clicks from the view (CellView).
/// - Forwards them to GameController to apply the corresponding logic
/// (reveal or flag depending on current mode).
/// </summary>
public class CellViewController
{
    private GameController _gameController;

    public CellViewController(GameController gameController)
    {
        _gameController = gameController;
    }

    /// <summary>
    /// Handles a cell click:
    /// - Receives coordinates from CellView.
    /// - Calls GameController.HandleCellClick to apply game logic. 
    /// </summary>
    public void HandleCellClick(int column, int row)
    {
        _gameController.HandleCellClick(column, row);
    }
}