using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the game board. 
/// - Contains the grid of cells (Cell).
/// - Generates mines and calculates proximity.
/// - Maintains counters for revealed cells and flags. 
/// - Exposes events:
///   OnCellRevealed: fired when a cell is revealed.
///   OnFlagToggled: fired when a cell is flagged/unflagged.
/// These events are consumed by GameController and then propagated to BoardViewController.
/// </summary>
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
    public int FlagCount => _flagsCount;
    public int TotalCells => _totalCells;

    /// <summary>
    /// Events:
    /// - OnCellRevealed: notifies coordinates of a revealed cell.
    /// - OnFlagToggled: notifies coordinates of a flagged/unflagged cell.
    /// </summary>
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

    /// <summary>
    /// Initializes grid, places mines, and calculates proximity.
    /// </summary>
    private void GenerateBoard()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int column = 0; column < _columns; column++)
            {
                _grid[column, row] = new Cell();
            }
        }

        PlaceMines();
        CalculateProximity();
    }

    private void PlaceMines()
    {
        int placed = 0;
        System.Random rand = new System.Random();

        if (_mineCount == 1)
            _grid[1, 0].SetMine(true); // For testing, place a single mine in a known position.
        else
            while (placed < _mineCount)
            {
                int column = rand.Next(_columns);
                int row = rand.Next(_rows);

                if (!_grid[column, row].IsMine)
                {
                    _grid[column, row].SetMine(true);
                    placed++;
                }
            }
    }

    private void CalculateProximity()
    {
        for (int column = 0; column < _columns; column++)
        {
            for (int row = 0; row < _rows; row++)
            {
                if (_grid[column, row].IsMine) continue;

                int count = 0;
                for (int offsetColumn = -_proximityRange; offsetColumn <= _proximityRange; offsetColumn++)
                {
                    for (int offsetRow = -_proximityRange; offsetRow <= _proximityRange; offsetRow++)
                    {
                        int neighborColumn = column + offsetColumn;
                        int neighborRow = row + offsetRow;

                        if (neighborColumn >= 0 && neighborColumn < _columns && neighborRow >= 0 && neighborRow < _rows)
                        {
                            if (_grid[neighborColumn, neighborRow].IsMine) count++;
                        }
                    }
                }

                _grid[column, row].SetProximityCount(count);
            }
        }
    }

    public Cell GetCell(int column, int row)
    {
        return _grid[column, row];
    }

    /// <summary>
    /// Reveals a cell:
    /// - Marks the cell as revealed.
    /// - Increments revealed counter.
    /// - Fires OnCellRevealed event.
    /// </summary>
    public void RevealCell(int column, int row)
    {
        Cell cell = GetCell(column, row);

        if (cell.IsRevealed || cell.MarkState == CellMarkState.Flag || cell.MarkState == CellMarkState.Question) return;

        cell.Reveal();
        _revealedCellsCount++;
        OnCellRevealed?.Invoke(column, row);
    }

    /// <summary> 
    /// Automatically reveals neighbors if the cell is safe and proximity = 0.
    /// </summary>
    public void AutoRevealCells(Cell startCell, int column, int row)
    {
        Queue<(Cell, int, int)> queue = new Queue<(Cell, int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();

        queue.Enqueue((startCell, column, row));
        visited.Add((column, row));

        while (queue.Count > 0)
        {
            (Cell cell, int currentColumn, int currentRow) = queue.Dequeue();

            if (cell.IsMine) continue;

            for (int offsetColumn = -_proximityRange; offsetColumn <= _proximityRange; offsetColumn++)
            {
                for (int offsetRow = -_proximityRange; offsetRow <= _proximityRange; offsetRow++)
                {
                    int neighborColumn = currentColumn + offsetColumn;
                    int neighborRow = currentRow + offsetRow;

                    if (neighborColumn >= 0 && neighborColumn < _columns &&
                        neighborRow >= 0 && neighborRow < _rows)
                    {
                        if (!visited.Contains((neighborColumn, neighborRow)))
                        {
                            Cell neighborCell = GetCell(neighborColumn, neighborRow);
                            if (!neighborCell.IsRevealed && neighborCell.MarkState == CellMarkState.Empty)
                            {
                                RevealCell(neighborColumn, neighborRow);
                                visited.Add((neighborColumn, neighborRow));

                                if (neighborCell.ProximityCount == 0)
                                {
                                    queue.Enqueue((neighborCell, neighborColumn, neighborRow));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Toggles the mark state of a cell (Empty - Flag - Question - Empty).
    /// - Updates flag counter.
    /// - Fires OnFlagToggled event.
    /// </summary>
    public void ToggleCellMark(int column, int row)
    {
        Cell cell = GetCell(column, row);
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

        OnFlagToggled?.Invoke(column, row);
    }

    /// <summary>
    /// Reveals all cells on the board. 
    /// Used when the game ends (win or loss).
    /// Fires OnCellRevealed for each cell.
    /// </summary>
    public void RevealAllCells()
    {
        for (int column = 0; column < _columns; column++)
        {
            for (int row = 0; row < _rows; row++)
            {
                Cell cell = _grid[column, row];

                if (!cell.IsRevealed)
                {
                    cell.Reveal();

                    OnCellRevealed?.Invoke(column, row);
                }
            }
        }
    }
}