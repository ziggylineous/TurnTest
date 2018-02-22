using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "GameMode", menuName = "Game Mode", order = 10)]
public class GameMode : ScriptableObject
{
    public enum Mode
    {
        PvPRemote,
        PvPLocal,
        PcComputer
    };

    public SymbolDescriptor playerSymbol;
    public int playersTurnIndex;
    public Mode mode;

    public void OnEnable()
    {
        DontDestroyOnLoad(this);
    }
}