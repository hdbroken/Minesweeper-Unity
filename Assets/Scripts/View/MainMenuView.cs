using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _creditsButton;

    private void Awake()
    {
        if (_gameManager == null) throw new System.ArgumentNullException(nameof(_gameManager));
        if (_startButton == null) throw new System.ArgumentNullException(nameof(_startButton));
        if (_optionButton == null) throw new System.ArgumentNullException(nameof(_optionButton));
        if (_creditsButton == null) throw new System.ArgumentNullException(nameof(_creditsButton));
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        _gameManager.StartGame();
    }
}
