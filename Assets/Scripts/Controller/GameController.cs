using System;
using UnityEngine;

public enum GameState
{
    Playing,
    Won,
    Lost
}

/// <summary> 
/// Main controller for game logic.
/// - Coordinates the board (Board) and the timer (GameTimer).
/// - Maintains game state (Playing, Won, Lost). 
/// - Exposes board events externally (OnCellRevealed, OnFlagToggled). 
/// - Handles cell clicks: reveal or flag depending on current mode.
/// - Evaluates win/lose conditions. 
/// </summary>
public class GameController
{
    private Board _board;
    private GameState _gameState;
    private bool _isMarkMode = false;
    private bool _mineRevealed = false;
    private GameTimer _timer;

    // Properties exposed to view/controllers
    public bool IsFlagMode => _isMarkMode;
    public bool IsTimerRunning => _timer.IsRunning;
    public Cell GetCell(int column, int row) => _board.GetCell(column, row);
    public int MineCount => _board.MineCount;
    public int FlagCount => _board.FlagCount;

    /// <summary>
    /// Constructor: receives Board and Timer and starts the game.
    /// </summary>
    public GameController(Board board, GameTimer timer)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (timer == null) throw new ArgumentNullException(nameof(timer));
        StartGame(board, timer);
    }

    /// <summary>
    /// Events exposed externally:
    /// - OnCellRevealed: fired when the board reveals a cell.
    /// - OnFlagToggled: fired when the board flags/unflags a cell.
    /// Consumed by BoardViewController.
    /// </summary>
    public event Action<int, int> OnCellRevealed 
    { 
        add { _board.OnCellRevealed += value; } 
        remove { _board.OnCellRevealed -= value; } 
    }

    public event Action<int, int> OnFlagToggled 
    { 
        add { _board.OnFlagToggled += value; }
        remove { _board.OnFlagToggled -= value; } 
    }

    /// <summary>
    /// - Initializes game state.
    /// - Subscribes HandleCellRevealed to board event.
    /// - Starts the timer.
    /// </summary>
    private void StartGame(Board board, GameTimer timer)
    {
        _board = board;
        _timer = timer;
        _gameState = GameState.Playing;
        _mineRevealed = false;
        _isMarkMode = false;

        _board.OnCellRevealed += HandleCellRevealed;

        StartTimer();
    }

    /// <summary>
    /// Ends the game: stops timer and unsubscribes events.
    /// </summary>
    private void EndGame()
    {
        StopTimer();
        _board.OnCellRevealed -= HandleCellRevealed;
    }

    private void StartTimer() 
    { 
        _timer.Start();
    } 

    private void StopTimer() 
    {
        _timer.Stop();
    }

    public TimeSpan GetTime()
    {
        return _timer.Elapsed;
    }

    /// <summary>
    /// Reveals a cell: 
    /// - If mine: defeat.
    /// - If safe: auto-reveals neighbors. 
    /// - Then checks victory conditions.
    /// </summary>
    private void RevealCell(int x, int y)
    {
        if (_gameState != GameState.Playing) return;

        _board.RevealCell(x, y);

        if (_board.GetCell(x, y).IsMine)
        {
            _mineRevealed = true;
            EndGame();
        }
        else
        {
            _board.AutoRevealCells(_board.GetCell(x, y), x, y);
        }

        CheckVictory();
    }

    private void ToggleCellMark(int x, int y)
    {
        if (_gameState != GameState.Playing) return;

        _board.ToggleCellMark(x, y);
        // Only check for victory if the game is in Reveal mode
        // and the cell ended up marked with a flag.
        if (!_isMarkMode && _board.GetCell(x, y).MarkState == CellMarkState.Flag)
        {
            CheckVictory();
        }
    }

    /// <summary> 
    /// Evaluates win/lose conditions:
    /// - Mine revealed: defeat.
    /// - All safe cells revealed: victory.
    /// - All mines correctly flagged": victory.
    /// - Incorrect flags: defeat.
    /// In all cases, reveals the full board at the end.
    /// </summary>
    private void CheckVictory()
    {
        if (_gameState != GameState.Playing)
            return;

        if (_mineRevealed)
        {
            _gameState = GameState.Lost;
            EndGame();
            Debug.Log("Game Over! Mine revealed.");
        }

        // Victory by revealing all safe cells
        if (_board.RevealedCellsCount == _board.TotalCells - _board.MineCount)
        {
            _gameState = GameState.Won;
            EndGame();
            Debug.Log("Victory: All safe cells revealed!");
        }

        // Player placed as many flags as mines
        if (_board.FlagCount == _board.MineCount)
        {
            if (_board.AreFlagsCorrect())
            {
                _gameState = GameState.Won;
                EndGame();
                Debug.Log("Victory: All mines flagged correctly!");
            }
            else
            {
                _gameState = GameState.Lost;
                EndGame();
                Debug.Log("Defeat: Wrong flag placement!");
            }
        }

        switch (_gameState)
        {
            case GameState.Won:
                {
                    _board.RevealAllCells();
                    // TODO: trigger victory screen
                }
                break;
            case GameState.Lost:
                {
                    _board.RevealAllCells();
                    // TODO: trigger lost screen
                }
                break;
        }
    }

    // Called whenever a cell is revealed.
    private void HandleCellRevealed(int column, int row)
    {
        /*  if (_gameState == GameState.Playing) 
          {
              CheckVictory();
          }*/
    }

    public void HandleCellClick(int column, int row)
    {
        if (_isMarkMode)
            ToggleCellMark(column, row);
        else
            RevealCell(column, row);
    }

    public void SetMarkMode(bool mark)
    {
        _isMarkMode = mark;

        // When switching from Mark mode to Reveal mode,
        // check victory conditions
        if (!_isMarkMode) 
        {
            CheckVictory();
        }
    }

    public (int Rows, int Columns) GetBoardDimensions()
    {
        return (_board.Rows, _board.Columns);
    }
}