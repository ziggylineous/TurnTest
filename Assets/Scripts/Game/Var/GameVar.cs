using System;
using System.Collections.Generic;
using UnityEngine;


public class GameVar<T> : ScriptableObject {
    [SerializeField]
    private T value;
    public event Action<T> Changed;

	public void Set(T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
			value = newValue;
            if (Changed != null) Changed(value);
        }
    }

    public T Value { get { return value; } }

    public void OnDisable()
    {
        Changed = null;
    }
}