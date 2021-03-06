﻿using System;
using UnityEngine;

public class RemoteTurn : Turn {

    public NetworkHelper networkHelper;
    private MarkCommand mark;

    protected void Start()
    {
        mark = GetComponent<MarkCommand>();
    }

	public override void StartTurn ()
    {
        board.DisableSlots();

        networkHelper.RequestPosition(
            (Message.Position posMsg) => mark.Mark(posMsg.GamePosition),
            ReceiveWrongMessage,
            (Exception exception) => Debug.LogFormat("exception while receiving position {0}", exception)
        );
	}

    private void ReceiveWrongMessage(Message.BaseMessage wrongMessage)
    {
        Debug.Log("Remote turn wrong message");

        if (wrongMessage.Type == Message.OpponentDisconnected.TypeId)
            game.OpponentDisconnected();
        else
            game.ReceiveUnknownMessage(wrongMessage, "Position");
    }


}
