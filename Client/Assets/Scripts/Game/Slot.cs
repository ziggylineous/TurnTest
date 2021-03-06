﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class PositionEvent : UnityEvent<Game.Position> { }

public class Slot : MonoBehaviour {

    public SymbolDescriptor empty;
    public SymbolDescriptor cross;
    public SymbolDescriptor circle;
    public Vector2Int position;
	
    public MarkCommand mark;

    private Collider2D thisCollider;
    private SymbolDescriptor symbol;


	void Start ()
    {
		thisCollider = GetComponent<Collider2D>();
        Empty();
	}




    // Marking
    private void BaseSetup(SymbolDescriptor newSymbol, bool colliderEnabled)
    {
        if (newSymbol != symbol)
        {         
			symbol = newSymbol;
			thisCollider.enabled = colliderEnabled;
			
			Transform previousSymbolDisplay = transform.Find("Symbol");
			
			if (previousSymbolDisplay != null)
				Destroy(previousSymbolDisplay.gameObject);
			
			GameObject symbolDisplay = Instantiate(symbol.shape, gameObject.transform.position, Quaternion.identity);
			symbolDisplay.name = "Symbol";
			symbolDisplay.transform.parent = gameObject.transform;
			//symbolDisplay.transform.position = Vector3.zero;
        }
    }

    public void Empty()
    {
        BaseSetup(empty, true);
    }

    public void Mark(SymbolDescriptor symbol)
    {
        BaseSetup(symbol, false);
    }




    // input
    public void EnableMarkInput()
    {
        thisCollider.enabled = symbol.IsEmpty;
    }
	
    public void DisableMarkInput()
    {
        thisCollider.enabled = false;
    }

    private void OnMouseUp()
    {
        mark.Mark(this);
    }

    public MarkCommand MarkCmd {
        set { mark = value; }
    }
}
