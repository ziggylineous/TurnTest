using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "SymbolAsset", menuName = "Symbol", order = 2)]
public class SymbolDescriptor : ScriptableObject
{
    public Game.Symbol symbol;
    public GameObject shape;
    public Color color;
    public GameObject icon;

    public static SymbolDescriptor Circle;
    public static SymbolDescriptor Cross;

    public void Awake()
    {
        Debug.Log("SymbolDescriptor::Awake()");
    }

    public void OnEnable()
    {
        Debug.Log("SymbolDescriptor::OnEnable()");

        switch(symbol)
        {
            case Game.Symbol.Cross:
                Cross = this;
                break;
            
            case Game.Symbol.Circle:
                Circle = this;
                break;
            
            default:
                break;
        }
            
    }

    public bool IsEmpty { get { return symbol == Game.Symbol.Empty; } }
    public SymbolDescriptor Opposite {
        get {
            switch (symbol)
            {
                case Game.Symbol.Circle:
                    return Cross;
                case Game.Symbol.Cross:
                    return Circle;
                default:
                    return null;
            }
        }
    }
}