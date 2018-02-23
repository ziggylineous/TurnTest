using System;
using Game;
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

    public void EnableSlots()
    {
        Array.ForEach(slots, slot => slot.EnableMarkInput());
    }

    public void DisableSlots()
    {
        Array.ForEach(slots, slot => slot.DisableMarkInput());
    }

    public void Mark(Game.Position position, Game.Symbol symbol)
    {
        board.Mark(position, symbol);
    }

    public bool HasWon(Game.Position position)
    {
        return board.HasWon(position);
    }

    public Slot SlotAt(Position position)
    {
        return Array.Find(
            slots,
            slot =>
            (slot.position.y == position.Row) &&
            (slot.position.x == position.Col)
        );
    }

    public bool HasAnyEmpty
    {
        get { return board.HasAnyEmpty; }
    }

    public Slot[] Slots { get { return slots; } }
}
