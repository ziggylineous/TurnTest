using UnityEngine;
using UnityEngine.UI;

public class CantConnectPopup : MonoBehaviour
{
	public void Show(string message, string error)
    {
        SetText("Message", message);
        SetText("Error", error);
    }

	public void ExitGame()
    {
        Application.Quit();
    }

    private void SetText(string textName, string text)
    {
        GameObject gameObj = GameObject.Find(textName);
        Debug.Assert(gameObj != null);

        Text textCompo = gameObj.GetComponent<Text>();
        Debug.Assert(textCompo != null);

        textCompo.text = text;
    }
}
