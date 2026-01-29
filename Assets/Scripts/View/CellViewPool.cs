using UnityEngine;
using System.Collections.Generic;

public class CellViewPool
{
    private readonly CellView _prefab;
    private readonly Transform _parent;
    private readonly Queue<CellView> _pool = new Queue<CellView>();

    public CellViewPool(CellView prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CellView newCell = GameObject.Instantiate(_prefab, _parent);
            newCell.gameObject.SetActive(false);
            _pool.Enqueue(newCell);
        }
    }

    public CellView Get()
    {
        if (_pool.Count == 0)
        {
            CellView newCell = GameObject.Instantiate(_prefab, _parent);
            newCell.gameObject.SetActive(false);
            _pool.Enqueue(newCell);
        }

        CellView cell = _pool.Dequeue();
        cell.gameObject.SetActive(true);
        return cell;
    }

    public void Return(CellView cell)
    {
        cell.gameObject.SetActive(false);
        _pool.Enqueue(cell);
    }
}