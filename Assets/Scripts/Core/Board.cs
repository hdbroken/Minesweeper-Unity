using System;

public class Board
{
    private int _columns;
    private int _rows;
    private int _mineCount;
    private int _proximityRange;
    private Cell[,] _grid;
    private int _revealedCellsCount = 0;
    private int _totalCells = 0;
    private int _flagsCount = 0;

    public int Columns => _columns;
    public int Rows => _rows;
    public int MineCount => _mineCount;
    public int RevealedCellsCount => _revealedCellsCount;
    public int MarkedCellsCount => _flagsCount;

    public int TotalCells => _totalCells;

    public event Action<int, int> OnCellRevealed;
    public event Action<int, int> OnFlagToggled;

    public Board(int columns, int rows, int mineCount, int proximityRange = 1)
    {
        _columns = columns;
        _rows = rows;
        _mineCount = mineCount;
        _proximityRange = proximityRange;
        _grid = new Cell[_columns, _rows];
        _revealedCellsCount = 0;
        _totalCells = _columns * _rows;

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
                for (int offsetX = -_proximityRange; offsetX <= _proximityRange; offsetX++)
                {
                    for (int offsetY = -_proximityRange; offsetY <= _proximityRange; offsetY++)
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

    public void RevealCell(int x, int y)
    {
        Cell cell = GetCell(x, y);

        if (cell.IsRevealed) return;

        cell.Reveal();
        _revealedCellsCount++;
        OnCellRevealed?.Invoke(x, y);
    }

    public void AutoRevealCells(Cell cell, int x, int y)
    {
        if (cell.ProximityCount == 0 && !cell.IsMine)
        {
            for (int offsetX = -_proximityRange; offsetX <= _proximityRange; offsetX++)
            {
                for (int offsetY = -_proximityRange; offsetY <= _proximityRange; offsetY++)
                {
                    int neighborX = x + offsetX;
                    int neighborY = y + offsetY;
                    if (neighborX >= 0 && neighborX < _columns && neighborY >= 0 && neighborY < _rows)
                    {
                        Cell NeighborCell = GetCell(neighborX, neighborY);
                        if (!NeighborCell.IsRevealed)
                        {
                            RevealCell(neighborX, neighborY);
                            AutoRevealCells(NeighborCell, neighborX, neighborY);
                        }
                    }
                }
            }
        }
    }

    public void ToggleCellMark(int x, int y)
    {
        Cell cell = GetCell(x, y);
        // 'previousState' stores the cell's state before toggling. 
        // This ensures the marked cells counter is only updated 
        // if the state actually changed. 
        // Prevents double increments/decrements 
        // if ToggleFlag is called rapidly
        CellMarkState previousState = cell.MarkState;

        cell.ToggleMark();

        if (cell.MarkState == CellMarkState.Flag && previousState != CellMarkState.Flag)
            _flagsCount++; 
        else if (previousState == CellMarkState.Flag && cell.MarkState != CellMarkState.Flag)
            _flagsCount--;

        OnFlagToggled?.Invoke(x, y);
    }

    public bool AreFlagsCorrect()
    {
        if (_flagsCount != _mineCount)
            return false;

        foreach (Cell cell in _grid)
        {
            if (cell.MarkState == CellMarkState.Flag && !cell.IsMine)
                // Wrong flag placement, lost the game.
                return false;
        }

        // All flags match mines
        return true;
    }

    public void RevealAllCells()
    {
        // Reveal every cell in the grid.
        // Called when the game ends (loss or win) to show all mines and safe cells.

        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                Cell cell = _grid[x, y];

                if (!cell.IsRevealed)
                {
                    cell.Reveal();

                    // Trigger the board event so the view updates
                    OnCellRevealed?.Invoke(x, y);
                }
            }
        }
    }
}