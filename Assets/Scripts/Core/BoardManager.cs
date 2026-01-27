public class BoardManager
{
    private int _width;
    private int _height;
    private int _mineCount;
    private Cell[,] _grid;

    public int Width => _width;
    public int Height => _height;
    public int MineCount => _mineCount;

    private const int MAX_NEIGHBORS = 8;

    public BoardManager(int width, int height, int mineCount)
    {
        _width = width;
        _height = height;
        _mineCount = mineCount;
        _grid = new Cell[_width, _height];

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
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
            int x = rand.Next(_width);
            int y = rand.Next(_height);

            if (!_grid[x, y].IsMine)
            {
                _grid[x, y].SetMine(true);
                placed++;
            }
        }
    }

    private void CalculateProximity()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y].IsMine) continue;

                int count = 0;
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        int neighborX = x + offsetX;
                        int neighborY = y + offsetY;

                        if (neighborX >= 0 && neighborX < _width && neighborY >= 0 && neighborY < _height)
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