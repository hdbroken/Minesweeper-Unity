public interface IBoardView
{
    void Init(BoardViewController boardViewControllerm, CameraController cameraController);
    void GenerateBoard(int Row, int Columns);
    void ClearBoard();
    void UpdateCell(int x, int y);
}