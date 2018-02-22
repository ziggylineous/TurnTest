using UnityEngine;
using UnityEngine.UI;

public class CurrentTurnDisplay : MonoBehaviour {

    public SymbolVar currentSymbol;

    public void Start()
    {
        currentSymbol.Changed += RefreshCurrentSymbol;

        RefreshCurrentSymbol(currentSymbol.Value);
    }

    private void RefreshCurrentSymbol(SymbolDescriptor newCurrentSymbol)
    {
        SymbolImage.sprite = currentSymbol.Value.shape.GetComponent<SpriteRenderer>().sprite;
        SymbolText.text = currentSymbol.Value.symbol.ToString();
    }

    public void OnDestroy()
    {
        Debug.Log("CurrentTurnDisplay::OnDestroy()");
        currentSymbol.Changed -= RefreshCurrentSymbol;
    }

    private Image SymbolImage {
        get { return GetComponentInChildren<Image>(); }
    }

    private Text SymbolText
    {
        get { return GetComponentInChildren<Text>(); }
    }
}
