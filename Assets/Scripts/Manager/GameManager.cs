using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MainMenuView _mainMenu;
    [SerializeField] private GameInitializer _gameInitializer;
    [SerializeField] private BoardView _boardView;
    [SerializeField] private HudView _hudView;

    private GameController _gameController;

    private void OnEnable()
    {
        if (_mainMenu == null) throw new System.ArgumentNullException(nameof(_mainMenu));
        if (_gameInitializer == null) throw new System.ArgumentNullException(nameof(_gameInitializer));
        if (_boardView == null) throw new System.ArgumentNullException(nameof(_boardView));
        if (_hudView == null) throw new System.ArgumentNullException(nameof(_hudView));
    }

    public void StartGame()
    {
        _gameInitializer.InitializeGame(_boardView, _hudView);
        _gameController = _gameInitializer.GameController;
        _gameController.OnGameStateChanged += HandleGameStateChanged;
        _hudView.OnRestartRequested += OnHudRestartRequested;
        _hudView.OnReturnToMainMenuRequested += OnHudReturnToMainMenuRequested;
        _hudView.OnSettingsRequested += OnHudSettingsRequested;

        _mainMenu.gameObject.SetActive(false);
        _boardView.gameObject.SetActive(true);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Won || state == GameState.Lost)
        {
            _gameController.OnGameStateChanged -= HandleGameStateChanged;
            _boardView.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);
        }
    }

    private void OnHudRestartRequested()
    {
        CleanupAfterGame();
        StartGame();
    }

    private void OnHudReturnToMainMenuRequested()
    {
        CleanupAfterGame();
    }

    private void OnHudSettingsRequested()
    {
        // abrir panel de settings o delegar a UIManager
    }

    private void CleanupAfterGame()
    {
        if (_gameController != null)
            _gameController.OnGameStateChanged -= HandleGameStateChanged;

        if (_hudView != null)
        {
            _hudView.OnRestartRequested -= OnHudRestartRequested;
            _hudView.OnReturnToMainMenuRequested -= OnHudReturnToMainMenuRequested;
            _hudView.OnSettingsRequested -= OnHudSettingsRequested;
        }
    }
}