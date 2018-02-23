using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour {

    public GameObject winDisplay;
    public GameObject tieDisplay;
    private PlayerGame game;

    public void PlayAgain()
    {
        Destroy(gameObject);
        game.Restart();
    }

    private ResultPopup MakePopup(PlayerGame game_)
    {

        GameObject resultPopupGameObj = Instantiate(gameObject);
        ResultPopup resultPopup = resultPopupGameObj.GetComponent<ResultPopup>();
        resultPopup.game = game_;

        CanvasExtensions.Add(resultPopupGameObj);
		//resultPopupGameObj.GetComponent<RectTransform>().SetParent();

        return resultPopup;
    }

    public void WinPopup(PlayerGame game_, SymbolDescriptor winner)
    {
        ResultPopup resultPopup = MakePopup(game_);
        resultPopup.WinSetup(winner);
    }
	
	private void WinSetup(SymbolDescriptor winner)
	{
		tieDisplay.SetActive(false);
		winDisplay.SetActive(true);
		SetResultText(winDisplay, "Won");
		winDisplay.GetComponentInChildren<Image>().sprite = winner.icon;
	}
	
    public void TiePopup(PlayerGame game_)
    {
        ResultPopup resultPopup = MakePopup(game_);
        resultPopup.TieSetup();
    }

    private void TieSetup()
    {
        winDisplay.SetActive(false);
        tieDisplay.SetActive(true);

        SetResultText(tieDisplay, "Tie");
    }

    private void SetResultText(GameObject resultDisplay, string resultText)
    {
        Transform textGameObj = resultDisplay.transform.Find("Text");
        Text text = textGameObj.GetComponent<Text>();
        text.text = resultText;
    }
}
