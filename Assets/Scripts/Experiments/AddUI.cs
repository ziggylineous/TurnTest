using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUI : MonoBehaviour {
    public GameObject uiPrefab;

	public void InstantiatePrefab()
    {
        Vector2 randPos = Random.insideUnitCircle * 4.0f;

        GameObject uiInstanxe = Instantiate(uiPrefab, new Vector3(randPos.x, randPos.y), Quaternion.identity);
        uiInstanxe.transform.SetParent(gameObject.transform.parent);
    }
}
