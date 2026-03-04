public enum CellMarkState
{
    Empty,
    Flag,
    Question
}

/// <summary>
/// Represents an individual board cell.
/// - Can be a mine or safe.
/// - Maintains its state: revealed, interactable, and mark (Empty, Flag, Question). 
/// - Stores the number of nearby mines (proximity).
/// - Provides simple methods to change its state (reveal, mark, set values).
/// </summary>
public class Cell
{
    private bool _isMine;
    private bool _isRevealed;
    private bool _isInteractable;
    private CellMarkState _markState = CellMarkState.Empty;
    private int _proximityCount;

    public bool IsMine => _isMine;
    public bool IsRevealed => _isRevealed;
    public bool IsInteractable => _isInteractable;
    public CellMarkState MarkState => _markState;
    public int ProximityCount => _proximityCount;

    public void SetMine(bool value) => _isMine = value;
    public void SetProximityCount(int count) => _proximityCount = count;
    public void Reveal() => _isRevealed = true;
    public void Interactable(bool isInteractable) => _isInteractable = isInteractable;

    public void ToggleMark() 
    { 
        switch (_markState) 
        { 
            case CellMarkState.Empty: _markState = CellMarkState.Flag;
                break; 
            case CellMarkState.Flag: _markState = CellMarkState.Question;
                break; 
            case CellMarkState.Question: _markState = CellMarkState.Empty;
                break;
        } 
    }
}