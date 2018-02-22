using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {

    public NetworkHelper networkHelper;
    public Button requestMatchButton;
    public SymbolDescriptor circle;
    public SymbolDescriptor cross;
    public GameMode gameMode;
    public GameObject CantConnectPopup;
	
    // Use this for initialization
	void Start ()
    {
        Connect();
	}

    private void Connect()
    {
        ShowLoading();

        networkHelper.Connect(
            ReadyToRequestMatch,
            error => ShowFatalError("couldn't connect to server. Try again later", error)
        );
    }

    private void ShowLoading()
    {
        // show loading icon
        requestMatchButton.enabled = false;
    }

    private void ShowFatalError(string message, Exception error)
    {
        networkHelper.CantConnectPopup(message, error.Message);
        requestMatchButton.enabled = false;
    }

    private void ReadyToRequestMatch()
    {
        Debug.Log("can request match");
        // hide loading icon
        requestMatchButton.enabled = true;
    }

    public void RequestMatch()
    {
        Debug.Log("requesting match");
        ShowLoading();
        networkHelper.RequestMatch(
            StartMatch,
            RequestMatchWrongMessageHandler,
            NothingReceivedErrorHandler
        );
    }

    private void RequestMatchWrongMessageHandler(Message.BaseMessage wrongMessage)
    {
        Debug.LogFormat("Got wrong message while awaiting match message. Got {0} instead. Requesting match again", wrongMessage);
        RequestMatch();
    }

    private void NothingReceivedErrorHandler(Exception error)
    {
        if (!networkHelper.IsConnected)
        {
            Debug.Log("noting received when asking for match");
            Connect();
        }
        else
        {
            ShowFatalError("Didnt receive any response", error);
        }
    }

    public void Rename(string newName)
    {
        networkHelper.name = newName;
    }

    public void StartMatch(Message.StartMatch startMatchMsg)
    {
        Debug.Log("StartMatch");
        gameMode.mode = GameMode.Mode.PvPRemote;
        gameMode.playersTurnIndex = startMatchMsg.playersTurnIndex;
        gameMode.playerSymbol = ((Game.Symbol) startMatchMsg.symbol == Game.Symbol.Circle) ? circle : cross;

        SceneManager.LoadScene("Game");
    }
}
