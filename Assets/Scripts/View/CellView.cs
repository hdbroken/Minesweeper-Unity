using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    // Initializes the CellView. 
    // Called from BoardView when creating visual cells. 
    // The click callback is provided by CellViewController 
    // and invoked inside OnClick(). 
    // This links the visual cell to the cell data
    // and the game logic on GameController via CellViewController.
    public void Init(Cell cell, int column, int row, Action<int, int> _onCellClickCallBack)
    {
        _cell = cell ?? throw new System.ArgumentNullException(nameof(cell));

        _column = column;
        _row = row;
        _onCellClicked = _onCellClickCallBack ?? throw new System.ArgumentNullException(nameof(_onCellClickCallBack));

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (_cell == null) return;

        if (!_cell.IsInteractable) return;

        // Trigger the callback assigned during initialization. 
        // This sends the cell's coordinates to the CellViewController, 
        // which then notifies to the GameController to update the game state.
        _onCellClicked?.Invoke(_column, _row);
    }

    // Updates the visual state based on the Cell data:
    // - Revealed mine : set sprite
    // - Revealed safe cell : proximity number or empty
    // - Not revealed but marked : change sprite.
    // - Not revealed and not marked : empty sprite
    // Button interactability is disabled once the cell is revealed.
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