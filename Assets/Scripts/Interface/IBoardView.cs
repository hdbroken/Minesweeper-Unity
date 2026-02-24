public interface IBoardView
{
    void Init(Board board, GameController gameController, CameraController cameraController);
    void GenerateBoard(Board board, GameController gameController);
    void ClearBoard();
    void UpdateCell(int x, int y);
}