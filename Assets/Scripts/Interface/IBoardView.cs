public interface IBoardView
{
    void Init(BoardViewController boardViewControllerm, CameraController cameraController);
    void GenerateBoard(int columns, int rows);
    void ClearBoard();
    void UpdateCell(int column, int row);
}