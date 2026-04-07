public interface IBoardView
{
    void Init(BoardViewController boardViewController, CameraController cameraController);
    void GenerateBoard(int columns, int rows);
    void ClearBoard();
    void UpdateCell(int column, int row);
}