using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class SpriteMaskAnimation : MonoBehaviour {

    public Sprite[] sprites;
    public float frameTime;

    private float timer;
    private int index;
    private SpriteMask mask;

    public event System.Action Completed;

	void Start ()
    {
        mask = GetComponent<SpriteMask>();
        index = 0;
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer > frameTime)
        {
			timer -= frameTime;
			++index;

            if (index < sprites.Length)
            {
				mask.sprite = sprites[index];            
            }
            else
            {
                enabled = false;
				if (Completed != null) Completed();
            }
        }
	}

    public void Animate()
    {
        timer = 0.0f;
        index = 0;
        mask.sprite = sprites[index];
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
    }
}
