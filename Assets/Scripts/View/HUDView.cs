using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 
/// Hud view. 
/// - Initialized with a BoardViewController. 
/// - Subscribes to BoardViewController event: 
///   OnMineCounterChanged: updates the mine counter.
/// - Handles the mode toggle button (Reveal/Mark). 
/// - Updates the timer on screen while the game is active. 
/// </summary>
public class HudView : MonoBehaviour
{
    [SerializeField] private Button _hamburgerButton;
    [SerializeField] private HamburgerMenuView _hamburgerMenu;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _mineCounterText;
    [SerializeField] private Button _toggleModeButton;
    [SerializeField] private TextMeshProUGUI _modeButtonText;

    private BoardViewController _boardViewController;
    private Coroutine _updateTimer;

    public event Action OnRestartRequested;
    public event Action OnReturnToMainMenuRequested;
    public event Action OnSettingsRequested;

    /// <summary>
    /// Initializes the view with the board controller.
    /// - Subscribes to OnMineCounterChanged.
    /// - Starts the timer coroutine.
    /// - Sets the initial mode text to "Reveal".
    /// </summary>
    public void Init(BoardViewController controller)
    {
        if (_hamburgerButton == null) throw new ArgumentNullException(nameof(_hamburgerButton));
        if (_timerText == null) throw new ArgumentNullException(nameof(_timerText));
        if (_mineCounterText == null) throw new ArgumentNullException(nameof(_mineCounterText));
        if (_toggleModeButton == null) throw new ArgumentNullException(nameof(_toggleModeButton));
        if (_modeButtonText == null) throw new ArgumentNullException(nameof(_modeButtonText));

        UnsubscribeFromBoard();
        StopTimerCoroutine();

        _boardViewController = controller ?? throw new ArgumentNullException(nameof(controller));
        _boardViewController.OnMineCounterChanged += UpdateMineCounter;
        _updateTimer = StartCoroutine(UpdateTimer());
        UpdateModeText(false);
    }

    private void OnEnable()
    {
        if (_toggleModeButton != null)
            _toggleModeButton.onClick.AddListener(OnToggleModeClicked);

        if (_hamburgerButton != null)
            _hamburgerButton.onClick.AddListener(ToggleHamburgerMenu);

        if (_hamburgerMenu != null)
        {
            _hamburgerMenu.OnRestartSelected += HandleRestartSelected; 
            _hamburgerMenu.OnMainMenuSelected += HandleMainMenuSelected;
            _hamburgerMenu.OnSettingsSelected += HandleSettingsSelected;
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromBoard();
        StopTimerCoroutine();

        if (_toggleModeButton != null)
            _toggleModeButton.onClick.RemoveListener(OnToggleModeClicked);

        if (_hamburgerButton != null)
            _hamburgerButton.onClick.RemoveListener(ToggleHamburgerMenu);

        if (_hamburgerMenu != null)
        {
            _hamburgerMenu.OnRestartSelected -= HandleRestartSelected;
            _hamburgerMenu.OnMainMenuSelected -= HandleMainMenuSelected;
            _hamburgerMenu.OnSettingsSelected -= HandleSettingsSelected;
        }
    }

    private void OnDestroy()
    {
        // Failsafe for cases where the object is destroyed without a clean disable path.
        UnsubscribeFromBoard();
        StopTimerCoroutine();
    }

    private void HandleRestartSelected() => OnRestartRequested?.Invoke();
    private void HandleMainMenuSelected() => OnReturnToMainMenuRequested?.Invoke();
    private void HandleSettingsSelected() => OnSettingsRequested?.Invoke();

    private void ToggleHamburgerMenu()
    {
        if (_hamburgerMenu == null) return;
        bool isActive = _hamburgerMenu.gameObject.activeSelf;

        if (isActive) _hamburgerMenu.Hide();
        else _hamburgerMenu.Show();
    }

    private void OnToggleModeClicked()
    {
        bool isMarkMode =_boardViewController.ToggleMarkMode();
        UpdateModeText(isMarkMode);
    }    

    private IEnumerator UpdateTimer()
    {
        while (_boardViewController.IsTimerRunning)
        {
            _timerText.text = FormatTime(_boardViewController.GetTime);
            yield return new WaitForSeconds(.1f);
        }
        // Force a final refresh to display the precise stop time (not the last 0.1s update)
        _timerText.text = FormatTime(_boardViewController.GetTime);
    }

    private string FormatTime(TimeSpan time)
    {
        // Formats time as MM:SS.mmm (e.g. 02:15.347)
        return string.Format("{0:D2}:{1:D2}.{2:D3}",
            time.Minutes,
            time.Seconds,
            time.Milliseconds);
    }

    private void UpdateMineCounter(int remaining) => _mineCounterText.text = remaining.ToString();

    private void UpdateModeText(bool isMarkMode) => _modeButtonText.text = isMarkMode ? "Mark" : "Reveal";

    private void UnsubscribeFromBoard()
    {
        if (_boardViewController != null)
        {
            _boardViewController.OnMineCounterChanged -= UpdateMineCounter;
            _boardViewController = null;
        }

    }

    private void StopTimerCoroutine()
    {
        if (_updateTimer != null)
        {
            StopCoroutine(_updateTimer);
            _updateTimer = null;
        }
    }
}
