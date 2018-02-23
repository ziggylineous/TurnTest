using UnityEngine;
using System;

public class MarkCommand : MonoBehaviour
{
    public PlayerGame game;
    public Board board;
	public SymbolVar currentSymbol;

    public NetworkHelper networkHelper;

    private Game.Position position;

    private void Start()
    {
		if (game == null)
            game = GetComponent<PlayerGame>();
        
        if (board == null)
            board = GetComponentInChildren<Board>();
    
        Array.ForEach(board.Slots, slot => slot.mark = this);
    }

    public void Mark(Slot slot)
    {
        position = new Game.Position(slot.position.y, slot.position.x);

        board.Mark(position, currentSymbol.Value.symbol);
        board.DisableSlots();

		slot.Mark(currentSymbol.Value);
		slot.GetComponentInChildren<SpriteMaskAnimation>().Completed += OnMarkDisplayCompleted;

        if (game.CurrentTurn.NotifyRemote)
            networkHelper.SendPosition(position);
    }

    public void Mark(Game.Position position)
    {
        Mark(board.SlotAt(position));
    }

    private void OnMarkDisplayCompleted()
    {
        game.EndTurn(position);
    }
}
