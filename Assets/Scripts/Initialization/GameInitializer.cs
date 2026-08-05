using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private GameController _gameController;
    private CameraController _cameraController;
    private BoardViewController _boardViewController;
    private BoardView _boardView;
    private HudView _hudView;

    [SerializeField] private TimerDriver _timerDriver;
    [SerializeField] private int _columns = 10;
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _mines = 5;
    [SerializeField] private int _proximityRange = 1;

    [SerializeField] private float paddingX;
    [SerializeField] private float paddingY;

    public GameController GameController => _gameController;

    private void OnEnable()
    {
        if (_timerDriver == null) throw new System.ArgumentNullException(nameof(_timerDriver));
    }

    public void InitializeGame(BoardView boardview, HudView hudview)
    {
        _boardView = boardview ?? throw new System.ArgumentNullException(nameof(boardview));
        _hudView = hudview ?? throw new System.ArgumentNullException(nameof(hudview));

        Board board = new Board(_columns, _rows, _mines, _proximityRange);
        GameTimer gameTimer = new GameTimer();
        _gameController = new GameController(board, gameTimer);
        _boardViewController = new BoardViewController(_gameController);
        _timerDriver.Initialize(gameTimer);
        _cameraController = new CameraController(Camera.main, paddingX, paddingY);

        _boardView.Init(_boardViewController, _cameraController);
        _hudView.Init(_boardViewController);
    }
}