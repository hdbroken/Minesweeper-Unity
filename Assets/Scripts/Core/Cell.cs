public enum CellMarkState
{
    Empty,
    Flag,
    Question
}

public class Cell
{
    private bool _isMine;
    private bool _isRevealed;
    private CellMarkState _markState = CellMarkState.Empty;
    private int _proximityCount;

    public bool IsMine => _isMine;
    public bool IsRevealed => _isRevealed;
    public CellMarkState MarkState => _markState;
    public int ProximityCount => _proximityCount;
    public void SetMine(bool value) => _isMine = value;
    public void SetProximityCount(int count) => _proximityCount = count;
    public void Reveal() => _isRevealed = true;

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