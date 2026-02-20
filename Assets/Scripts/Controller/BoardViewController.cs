using UnityEngine;

public class BoardViewController
{
    private GameController _gameController;
    private Board _board;

    public BoardViewController(Board board, GameController gameController)
    {
        _gameController = gameController;
        _board = board;

        // Subscribe to Board events for global feedback.
        // BoardViewController does not update cells directly (BoardView handles that).
        // Instead, it triggers animations, sounds, or transitions when events occur.

        _board.OnCellRevealed += HandleCellRevealed;
        _board.OnFlagToggled += HandleFlagToggled;
    }

    private void HandleCellRevealed(int column, int row)
    {
        Debug.Log($"Cell revealed at ({column},{row}) - trigger global effects here.");
    }

    private void HandleFlagToggled(int column, int row)
    {
        Debug.Log($"Flag toggled at ({column},{row}) - trigger flag effects here.");
    }
}
