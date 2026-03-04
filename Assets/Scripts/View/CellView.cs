using UnityEngine;
using TMPro;
using System;

/// <summary> 
/// Visual representation of a single board cell.
/// - Displays sprite and text based on the model state (Cell).
/// - Receives coordinates and click callback from BoardView.
/// - Notifies CellViewController when the player interacts.
/// - Updates its visual appearance whenever the model changes.
/// </summary>
public class CellView : MonoBehaviour
{
    [SerializeField] private TextMeshPro _txtCellInfo;
    [SerializeField] private SpriteRenderer _cellSpriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite _flagSprite;
    [SerializeField] private Sprite _questionSprite;
    [SerializeField] private Sprite _mineSprite;
    [SerializeField] private Sprite _emptySprite;

    private Cell _cell;
    private Action<int, int> _onCellClicked;
    private int _column;
    private int _row;
    private bool _isInteractable = true;

    public int Column => _column;
    public int Row => _row;
    public float CellSize => _cellSpriteRenderer != null ? _cellSpriteRenderer.bounds.size.x : 1f;

    /// <summary> 
    /// Initializes the visual cell:
    /// - Associates the model (Cell).
    /// - Stores grid coordinates.
    /// - Registers the click callback (provided by CellViewController).
    /// - Refreshes initial appearance. 
    /// </summary>
    public void Init(Cell cell, int column, int row, Action<int, int> _onCellClickCallBack)
    {
        _cell = cell ?? throw new System.ArgumentNullException(nameof(cell));

        _column = column;
        _row = row;
        _onCellClicked = _onCellClickCallBack ?? throw new System.ArgumentNullException(nameof(_onCellClickCallBack));

        UpdateVisual();
    }

    /// <summary>
    /// Handles mouse click:
    /// - If interactable, triggers the callback(assigned during initialization) with its coordinates.
    /// - Callback flows to CellViewController -> GameController.
    /// </summary>
    private void OnMouseDown()
    {
        if (_cell == null) return;

        if (!_cell.IsInteractable) return;

        _onCellClicked?.Invoke(_column, _row);
    }

    /// <summary>
    /// Updates visual appearance based on model state:
    /// - Revealed mine: mine sprite.
    /// - Revealed safe: proximity number or empty.
    /// - Unrevealed but marked: flag or question sprite.
    /// - Unrevealed and unmarked: empty sprite.
    /// Also adjusts interactability.
    /// </summary>
    public void UpdateVisual()
    {
        if (!_isInteractable) return;

        if (_txtCellInfo == null) return;

        if (_cell == null)
        {
            _txtCellInfo.text = string.Empty;
            if (_isInteractable) _isInteractable = false;
            return;
        }

        if (_cell.IsRevealed)
        {
            if (_cell.IsMine)
            {
                _cellSpriteRenderer.sprite = _mineSprite;
            }
            else
            {
                _cellSpriteRenderer.color = Color.white; // Change color to indicate revealed state
                _txtCellInfo.text = _cell.ProximityCount > 0 ? _cell.ProximityCount.ToString() : string.Empty;
            }
        }
        else
        {
            if (_cell.MarkState == CellMarkState.Flag)
                _cellSpriteRenderer.sprite = _flagSprite;
            else if (_cell.MarkState == CellMarkState.Question)
                _cellSpriteRenderer.sprite = _questionSprite;
            else
                _cellSpriteRenderer.sprite = _emptySprite;
        }

        _isInteractable = !_cell.IsRevealed;
        _cell.Interactable(_isInteractable);
    }
}