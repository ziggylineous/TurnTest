using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : Turn {

    public bool notifyRemote = false;

    public override void StartTurn()
    {
        board.EnableSlots(position => game.Mark(position, symbol));
    }

    public override bool NotifyRemote { get { return notifyRemote; } }
}
