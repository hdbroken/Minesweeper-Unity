using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtCellInfo;
    [SerializeField] private Button _button;

    private Cell _cell;

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

    public void Init(Cell cell)
    {
        _cell = cell ?? throw new System.ArgumentNullException(nameof(cell));

        UpdateVisual();
    }

    private void OnClick()
    {
        if (_cell == null) return;

        //Pasar a un controlador
        _cell.Reveal();

        UpdateVisual();
    }

    // Updates the visual state based on the Cell data
    private void UpdateVisual()
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
                _txtCellInfo.text = "💣";
            }
            else
            {
                _txtCellInfo.text = _cell.ProximityCount > 0 ? _cell.ProximityCount.ToString() : string.Empty;
            }
        }
        else
        {
            _txtCellInfo.text = string.Empty;
        }

        if (_button != null)
            _button.interactable = !_cell.IsRevealed;
    }
}