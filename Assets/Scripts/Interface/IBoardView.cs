public interface IBoardView
{
    void Init(Board board, GameController gameController);
    void GenerateBoard(Board board, GameController gameController);
    void ClearBoard();
    void UpdateCell(int x, int y);
}