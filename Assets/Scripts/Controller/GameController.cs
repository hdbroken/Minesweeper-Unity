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

    public Action<GameState> OnGameStateChanged;

    // Properties exposed to view/controllers
    public bool IsMarkMode => _isMarkMode;
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
    /// - If safe and proximity = 0: auto-reveals neighbors. 
    /// - Then checks victory conditions.
    /// </summary>
    private void RevealCell(int column, int row)
    {
        if (_gameState != GameState.Playing) return;

        _board.RevealCell(column, row);

        Cell cell = _board.GetCell(column, row);
        if (cell.IsMine)
        {
            _mineRevealed = true;            
        }
        else if (cell.ProximityCount == 0)
        {
            _board.AutoRevealCells(cell, column, row);
        }

        CheckVictory();
    }

    private void ToggleCellMark(int column, int row)
    {
        if (_gameState != GameState.Playing) return;

        _board.ToggleCellMark(column, row);
    }

    /// <summary> 
    /// Evaluates win/lose conditions:
    /// - Mine revealed: defeat.
    /// - All safe cells revealed: victory.
    /// In both cases, reveals the full board calling FinishGame.
    /// </summary>
    private void CheckVictory()
    {
        if (_gameState != GameState.Playing)
            return;

        if (_mineRevealed)
        {
            FinishGame(GameState.Lost, "Game Over! Mine revealed.");
            return;
        }

        // Victory by revealing all safe cells
        if (_board.RevealedCellsCount == _board.TotalCells - _board.MineCount)
        {
            FinishGame(GameState.Won, "Victory: All safe cells revealed!");
            return;
        }
    }

    private void FinishGame(GameState result, string logMessage)
    {
        _gameState = result;

        _board.RevealAllCells();

        EndGame();

        Debug.Log(logMessage);

        switch (result)
        {
            case GameState.Won:
                // TODO: trigger victory screen
                break;
            case GameState.Lost:
                // TODO: trigger lost screen
                break;
        }

        OnGameStateChanged?.Invoke(result);
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
        else if (GetCell(column, row).MarkState == CellMarkState.Empty)// Ignore flagged or questioned cells
            RevealCell(column, row);
    }

    public void SetMarkMode(bool mark)
    {
        _isMarkMode = mark;
    }

    public (int Rows, int Columns) GetBoardDimensions()
    {
        return (_board.Rows, _board.Columns);
    }
}