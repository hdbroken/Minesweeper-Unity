using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private CellView _cellPrefab;

    private CellViewPool _pool;
    private List<CellView> _activeCells = new List<CellView>();

    public void GenerateBoard(Board board)
    {
        if (board == null) throw new System.ArgumentNullException(nameof(board));

        ClearBoard();

        int columns = board.Columns;
        int rows = board.Rows;
        int needed = columns * rows;

        // Create or ensure pool capacity with the correct size
        if (_pool == null)
        {
            _pool = new CellViewPool(_cellPrefab, needed, _gridParent);
        }
        else
        {
            _pool.EnsureCapacity(needed);
        }

        // Adjust layout so the Grid generates the correct number of columns
        if (_gridLayout != null)
        {
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = columns;
        }

        // Instantiate visual cells from the pool
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Cell cell = board.GetCell(x, y);

                CellView cellView = _pool.Get();
                cellView.transform.SetParent(_gridParent, false);

                cellView.Init(cell);
                _activeCells.Add(cellView);
            }
        }
    }

    public void ClearBoard()
    {
        foreach (var cellView in _activeCells)
        {
            _pool?.Return(cellView);
        }
        _activeCells.Clear();
    }
}
