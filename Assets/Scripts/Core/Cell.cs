public class Cell
{
    private bool _isMine;
    private bool _isRevealed;
    private bool _isMarked;
    private int _proximityCount;

    public bool IsMine => _isMine;
    public bool IsRevealed => _isRevealed;
    public bool IsMarked => _isMarked;
    public int ProximityCount => _proximityCount;

    private const int MAX_NEIGHBORS = 8;

    public void SetMine(bool value) => _isMine = value;
    public void SetProximityCount(int count) => _proximityCount = count;
    public void Reveal() => _isRevealed = true;
    public void ToggleMark() => _isMarked = !_isMarked;    
}