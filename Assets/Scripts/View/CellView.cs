using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CellView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtCellInfo;
    [SerializeField] private Button _button;

    private Cell _cell;
    private Action<int, int> _onCellClicked;
    private int _column;
    private int _row;
    public int Column => _column;
    public int Row => _row;

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnClick);
    }

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

    private void OnClick()
    {
        if (_cell == null) return;

        // Trigger the callback assigned during initialization. 
        // This sends the cell's coordinates to the CellViewController, 
        // which then notifies to the GameController to update the game state.
        _onCellClicked?.Invoke(_column, _row);
    }

    // Updates the visual state based on the Cell data:
    // - Revealed mine : "M"
    // - Revealed safe cell : proximity number or empty
    // - Not revealed but marked : "F" (flag) or "?" (Question)
    // - Not revealed and not marked : empty
    // Button interactability is disabled once the cell is revealed.
    public void UpdateVisual()
    {
        if (_txtCellInfo == null) return;

        if (_cell == null)
        {
            _txtCellInfo.text = string.Empty;
            if (_button != null) _button.interactable = false;
            return;
        }

        if (_cell.IsRevealed)
        {
            if (_cell.IsMine)
            {
                _txtCellInfo.text = "M";
            }
            else
            {
                _txtCellInfo.text = _cell.ProximityCount > 0 ? _cell.ProximityCount.ToString() : string.Empty;
            }
        }
        else
        {
            if (_cell.MarkState == CellMarkState.Flag)
                _txtCellInfo.text = "F";
            else if (_cell.MarkState == CellMarkState.Question)
                _txtCellInfo.text = "?";
            else
                _txtCellInfo.text = string.Empty;
        }

        if (_button != null)
            _button.interactable = !_cell.IsRevealed;
    }
}