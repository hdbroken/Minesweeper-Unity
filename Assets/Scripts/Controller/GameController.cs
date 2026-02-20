using UnityEngine;

public enum GameState
{
    Playing,
    Won,
    Lost
}

public class GameController
{
    private Board _board;
    private GameState _gameState;
    private bool _isMarkMode = false;
    private bool _mineRevealed = false;
    public bool IsFlagMode => _isMarkMode;

    public GameController(Board board)
    {
        if (board == null) throw new System.ArgumentNullException(nameof(board));
        StartGame(board);
    }

    public void StartGame(Board board)
    {
        _board = board;
        _gameState = GameState.Playing;
        _mineRevealed = false;
        _isMarkMode = false;

        _board.OnCellRevealed += HandleCellRevealed;
    }

    private void RevealCell(int x, int y)
    {
        if (_gameState != GameState.Playing) return;

        _board.RevealCell(x, y);

        if (_board.GetCell(x, y).IsMine)
        {
            _mineRevealed = true;
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

    private void CheckVictory()
    {
        if (_gameState != GameState.Playing)
            return;

        if (_mineRevealed)
        {
            _gameState = GameState.Lost;
            Debug.Log("Game Over! Mine revealed.");
        }

        // Victory by revealing all safe cells
        if (_board.RevealedCellsCount == _board.TotalCells - _board.MineCount)
        {
            _gameState = GameState.Won;
            Debug.Log("Victory: All safe cells revealed!");
        }

        // Player placed as many flags as mines
        if (_board.MarkedCellsCount == _board.MineCount)
        {
            if (_board.AreFlagsCorrect())
            {
                _gameState = GameState.Won;
                Debug.Log("Victory: All mines flagged correctly!");
            }
            else
            {
                _gameState = GameState.Lost;
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
}