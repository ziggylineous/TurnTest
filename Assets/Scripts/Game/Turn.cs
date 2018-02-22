using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turn : MonoBehaviour {

    public SymbolDescriptor symbol;
    protected Board board;
    protected PlayerGame game;

    protected void Awake()
    {
        game = GetComponent<PlayerGame>();
        board = transform.GetComponentInChildren<Board>();
    }

    public abstract void StartTurn();

    public virtual bool NotifyRemote { get { return false; } }
}
