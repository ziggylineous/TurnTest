using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGame : MonoBehaviour {

    public NetworkHelper networkHelper;

    private Turn[] turns = new Turn[2];
    private int currentTurnIndex;
    private Board board;
    public GameObject resultPopupPrefab;
    public GameMode gameMode;
    public MarkCommand mark;

    public void Start()
    {
		board = transform.GetComponentInChildren<Board>();
        mark = GetComponent<MarkCommand>();

        Dictionary<GameMode.Mode, System.Func<Turn>> otherPlayerTurnFactory = new Dictionary<GameMode.Mode, System.Func<Turn>>
        {
            {GameMode.Mode.PvPLocal, CreateAnotherLocalPlayer},
            {GameMode.Mode.PvPRemote, CreateRemotePlayer}
        };

        PlayerTurn playerTurn = gameObject.AddComponent<PlayerTurn>();
        playerTurn.notifyRemote = gameMode.mode == GameMode.Mode.PvPRemote;

        Turn otherPlayerTurn = otherPlayerTurnFactory[gameMode.mode]();

        Restart(playerTurn, otherPlayerTurn, gameMode);
    }

    private void Restart(Turn player, Turn otherPlayer, GameMode mode)
    {
        turns[gameMode.playersTurnIndex] = player;
        player.symbol = mode.playerSymbol;

        int otherPlayersTurnIndex = (mode.playersTurnIndex + 1) % 2;
        turns[otherPlayersTurnIndex] = otherPlayer;
        otherPlayer.symbol = mode.playerSymbol.Opposite;

        board.Reset();

        currentTurnIndex = -1;
        NextTurn();
    }

    private Turn CreateRemotePlayer()
    {
        RemoteTurn remoteTurn = gameObject.AddComponent<RemoteTurn>();
        remoteTurn.networkHelper = networkHelper;

        return remoteTurn;
    }

    private Turn CreateAnotherLocalPlayer()
    {
        PlayerTurn otherPlayerTurn = gameObject.AddComponent<PlayerTurn>();

        return otherPlayerTurn;
    }




    // Game posta methods
    private void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turns.Length;
		mark.currentSymbol.Set(CurrentTurn.symbol);
        CurrentTurn.StartTurn();
    }

    public void EndTurn(Game.Position position)
    {
        if (board.HasWon(position))
        {
            Win(CurrentTurn);
        }
        else if (!board.HasAnyEmpty)
        {
            Tie();
        }
        else
        {
            NextTurn();
        }
    }

    private void Win(Turn turn)
    {
        EndGame();
        resultPopupPrefab.GetComponent<ResultPopup>().WinPopup(this, turn.symbol);
    }

    private void Tie()
    {
        EndGame();
        resultPopupPrefab.GetComponent<ResultPopup>().TiePopup(this);
    }

    private void EndGame()
    {
        board.DisableSlots();
    }




    // Restart 
    public void Restart()
    {
        Dictionary<GameMode.Mode, System.Action> restarts = new Dictionary<GameMode.Mode, System.Action>
        {
            {GameMode.Mode.PvPLocal, RestartPvPLocal},
            {GameMode.Mode.PvPRemote, RestartPvPRemote}
        };

        restarts[gameMode.mode]();
    }

    private void RestartPvPLocal()
    {
        Turn playerTurn = turns[gameMode.playersTurnIndex];

        int otherPlayersTurnIndex = (gameMode.playersTurnIndex + 1) % turns.Length;
        Turn otherPlayerTurn = turns[otherPlayersTurnIndex];

		gameMode.playersTurnIndex = Random.Range(0, turns.Length);
        Restart(playerTurn, otherPlayerTurn, gameMode);
    }

    private void ShowLoadingMatch()
    {
		// showLoading gameobject
		board.DisableSlots();
    }

    private void RestartPvPRemote()
    {
        networkHelper.RequestMatch(
            matchRequest =>
            {
                if ((Game.Symbol)matchRequest.symbol == Game.Symbol.Circle)
                    gameMode.playerSymbol = SymbolDescriptor.Circle;
                else
                    gameMode.playerSymbol = SymbolDescriptor.Cross;

                gameMode.playersTurnIndex = matchRequest.playersTurnIndex;
                Restart(GetComponent<PlayerTurn>(), GetComponent<RemoteTurn>(), gameMode);
            },
            wrongMessage => ReceiveUnknownMessage(wrongMessage, "StartMatch"),
            error => SceneManager.LoadScene("Lobby")
        );
    }

    public void OpponentDisconnected()
    {
        Debug.Log("OpponentDisconnected handler, winning");
        Win(GetComponent<PlayerTurn>());
    }

    public void ReceiveUnknownMessage(Message.BaseMessage wrongMessage, string awaitedMessage)
    {
        Debug.LogFormat("Received wrong message {0} [awaiting {1}]", wrongMessage, awaitedMessage);
    }

    public Turn CurrentTurn { get { return turns[currentTurnIndex]; } }
}
