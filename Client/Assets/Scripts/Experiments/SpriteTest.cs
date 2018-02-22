using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTest : MonoBehaviour {

    public Sprite sprite;
    public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		foreach (Sprite s in sprites)
        {
            Debug.LogFormat("name: {0}, uv = ({1}, {2})", s.name, s.uv[0], s.uv[1]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
