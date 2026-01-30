public class Board
{
    private int _columns;
    private int _rows;
    private int _mineCount;
    private Cell[,] _grid;

    public int Columns => _columns;
    public int Rows => _rows;
    public int MineCount => _mineCount;

    private const int MAX_NEIGHBORS = 8;

    public Board(int columns, int rows, int mineCount)
    {
        _columns = columns;
        _rows = rows;
        _mineCount = mineCount;
        _grid = new Cell[_columns, _rows];

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                _grid[x, y] = new Cell();
            }
        }

        PlaceMines();
        CalculateProximity();
    }

    private void PlaceMines()
    {
        int placed = 0;
        System.Random rand = new System.Random();

        while (placed < _mineCount)
        {
            int x = rand.Next(_columns);
            int y = rand.Next(_rows);

            if (!_grid[x, y].IsMine)
            {
                _grid[x, y].SetMine(true);
                placed++;
            }
        }
    }

    private void CalculateProximity()
    {
        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                if (_grid[x, y].IsMine) continue;

                int count = 0;
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        int neighborX = x + offsetX;
                        int neighborY = y + offsetY;

                        if (neighborX >= 0 && neighborX < _columns && neighborY >= 0 && neighborY < _rows)
                        {
                            if (_grid[neighborX, neighborY].IsMine) count++;
                        }
                    }
                }

                _grid[x, y].SetProximityCount(count);
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        return _grid[x, y];
    }
}