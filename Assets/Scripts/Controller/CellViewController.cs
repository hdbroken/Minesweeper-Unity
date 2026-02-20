public class CellViewController
{
    private GameController _gameController;

    public CellViewController(GameController gameController)
    {
        _gameController = gameController;
    }

    public void HandleCellClick(int column, int row)
    {
        _gameController.HandleCellClick(column, row);
    }
}