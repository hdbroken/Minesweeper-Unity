using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private BoardView _boardView;

    private GameController _gameController;
    private CameraController _cameraController;
    
    [SerializeField] private int _columns = 10;
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _mines = 5;
    [SerializeField] private int _proximityRange = 1;

    [SerializeField] private float paddingX;
    [SerializeField] private float paddingY;

    private void Start()
    {
        Board board = new Board(_columns, _rows, _mines, _proximityRange);
        
        _gameController = new GameController(board);
        _cameraController = new CameraController(Camera.main, paddingX, paddingY);

        _boardView.Init(board, _gameController, _cameraController);        
    }
}