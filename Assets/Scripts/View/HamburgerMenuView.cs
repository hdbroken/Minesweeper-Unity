using System;
using UnityEngine;
using UnityEngine.UI;

public class HamburgerMenuView : MonoBehaviour
{
    public event Action OnRestartSelected;
    public event Action OnMainMenuSelected;
    public event Action OnSettingsSelected;

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _settingsButton;

    private void Awake()
    {
        if (_restartButton != null) _restartButton.onClick.AddListener(HandleRestart);
        if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(HandleMainMenu);
        if (_settingsButton != null) _settingsButton.onClick.AddListener(HandleSettings);
    }

    private void OnDisable()
    {
        if (_restartButton != null) _restartButton.onClick.RemoveListener(HandleRestart);
        if (_mainMenuButton != null) _mainMenuButton.onClick.RemoveListener(HandleMainMenu);
        if (_settingsButton != null) _settingsButton.onClick.RemoveListener(HandleSettings);
    }

    public void Show() => _panel.SetActive(true);
    public void Hide() => _panel.SetActive(false);

    private void HandleRestart() => OnRestartSelected?.Invoke();
    private void HandleMainMenu() => OnMainMenuSelected?.Invoke();
    private void HandleSettings() => OnSettingsSelected?.Invoke();
}
