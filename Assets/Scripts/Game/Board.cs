using System;
using UnityEngine;

public class Board : MonoBehaviour {

    private Slot[] slots;

    [HideInInspector]
    public Game.Board board;
	
    void Start ()
    {
        slots = transform.GetComponentsInChildren<Slot>();
        board = new Game.Board(3, 3, 3);
	}

    public void Reset()
    {
        Array.ForEach(slots, slot => slot.Empty());
        board.Clear();
    }

    public void EnableSlots(Action<Game.Position> inputHandler)
    {
        Array.ForEach(slots, slot => slot.EnableMarkInput(inputHandler));
    }

    public void DisableSlots()
    {
        Array.ForEach(slots, slot => slot.DisableMarkInput());
    }

    public void Mark(Game.Position position, SymbolDescriptor symbol)
    {
        board.Mark(position, symbol.symbol);
        Slot slotWithPosition = Array.Find(slots,
                                           slot => 
           (slot.position.y == position.Row) &&
           (slot.position.x == position.Col)
        );
        slotWithPosition.Mark(symbol);
    }

    public bool HasWon(Game.Position position)
    {
        return board.HasWon(position);
    }

    public bool HasAnyEmpty
    {
        get { return board.HasAnyEmpty; }
    }

}
