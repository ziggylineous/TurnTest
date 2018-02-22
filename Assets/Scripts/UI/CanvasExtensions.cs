using UnityEngine;
using UnityEngine.UI;

public static class CanvasExtensions
{
    public static GameObject MainGameObject
    {
        get { return GameObject.Find("Canvas"); }
    }

    public static void Add(GameObject uiGameObject)
    {
        uiGameObject.transform.SetParent(MainGameObject.transform, false);
    }
}
