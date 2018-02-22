using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using System.Runtime;

[CreateAssetMenu(fileName = "networkHelper", menuName = "Network/Network Helper", order = 1)]
public class NetworkHelper : ScriptableObject {

	public string ip;
    public int port;
	public int connectRetries = 2;
	private int currentConnectRetries;
    private TcpClient client;
    private string playerName;
	private bool connecting = false;
    public GameObject cantConnectPopup;
    
    public void Awake () {
        Debug.Log("NetworkHelper::Start()");
        client = new TcpClient();

        currentConnectRetries = connectRetries;

        connecting = false;

        DontDestroyOnLoad(this);
    }

    public void OnEnable()
    {
        Debug.Log("NetworkHelper::OnEnable()");

        if (client == null)
            client = new TcpClient();
    }


    public void Connect(Action onConnect, Action<Exception> onError)
    {
        Debug.Assert(!connecting);

        if (!connecting)
        {
			Debug.LogFormat("Connecting ({0})...", currentConnectRetries);
			
			ThreadStart threadFun = () =>
			{
				try
				{
					client.Connect(
						IPAddress.Parse(ip),
						port
					);

                    bool enterLobbySent = Send(client.GetStream(), new Message.EnterLobby("ignacio"));

                    if (enterLobbySent)
                        MainThreadCommandQueue.Instance.ExecuteInUpdate(onConnect);
                    else
                        onError(new Exception("Couldnt send enterLobby message"));
				}
				catch (Exception connectException)
				{
					--currentConnectRetries;
					
					if (currentConnectRetries > 0)
					{
						Connect(onConnect, onError);
					}
					else
					{
                        MainThreadCommandQueue.Instance.ExecuteInUpdate(()=>onError(connectException)) ;
					}
				}

                connecting = false;
            };
            // thread fun end

			Thread connectThread = new Thread(threadFun);
			connectThread.Start();
        }
        // if !connecting end
    }

    public void RequestMatch(Action<Message.StartMatch> onReceiveSuccess, Action<Message.BaseMessage> onReceiveWrongMessage, Action<Exception> onReceiveError)
    {
        //ThreadStart threadFun = () =>
        //{
		    bool sentRequest = Send(client.GetStream(), new Message.StartMatchRequest());

            if (sentRequest)
            {
                Debug.Log("sent match request, waiting for match...");

                Message.Util.Receive<Message.StartMatch>(
                    client.GetStream(),
                    startMatch => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveSuccess(startMatch)),
                    wrongMessage => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveWrongMessage(wrongMessage)),
                    receiveError => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveError(receiveError))
                );
            }
            else
            {
                MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveError(new Exception("couldnt send nada")));
            }
        //};
        // thread func end

        //Thread receiveThread = new Thread(threadFun);
        //receiveThread.Start();
    }

    public void RequestPosition(
        Action<Message.Position> onReceiveSuccess,
        Action<Message.BaseMessage> onReceiveWrongMessage,
        Action<Exception> onReceiveError)
    {
        ReceiveThread<Message.Position>(onReceiveSuccess, onReceiveWrongMessage, onReceiveError);
    }

    public void SendPosition(Game.Position position)
    {
        Send(client.GetStream(), new Message.Position(position.Row, position.Col));
    }

    public GameObject CantConnectPopup(string message, string error)
    {
        GameObject cantConnectPopupInstance = Instantiate(cantConnectPopup, Vector3.zero, Quaternion.identity);
        CanvasExtensions.Add(cantConnectPopupInstance);
        cantConnectPopupInstance.GetComponent<CantConnectPopup>().Show(message, error);

        return cantConnectPopup;
    }

    public void OnDisable()
    {
        Debug.Log("NetworkHelper::OnDisable()");
    }

    public void OnDestroy() {
        Debug.Log("NetworkHelper::OnDestroy()");
    }
	
	private bool Send(NetworkStream ns, Message.BaseMessage message)
	{
		Console.WriteLine("SendMessage: " + message.ToString());
		IFormatter formatter = new BinaryFormatter();
		
		try
		{
			formatter.Serialize(ns, message);
			return true;
		}
		catch (SerializationException sendError)
		{
			Console.WriteLine("SendMessage {0} error: {1}/{2}", message, sendError.Message, sendError.InnerException);
			return false;
		}
	}

    private void ReceiveThread<MessageType>(
        Action<MessageType> onReceiveSuccess,
        Action<Message.BaseMessage> onReceiveWrongMessage,
        Action<Exception> onReceiveError
        ) where MessageType : Message.BaseMessage
    {
        ThreadStart threadFun = () =>
        {
            Message.Util.Receive<MessageType>(
                client.GetStream(),
                startMatch => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveSuccess(startMatch)),
                wrongMessage => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveWrongMessage(wrongMessage)),
                receiveError => MainThreadCommandQueue.Instance.ExecuteInUpdate(() => onReceiveError(receiveError))
            );
        };
        // thread func end

        Thread receiveThread = new Thread(threadFun);
        receiveThread.Start();
    }

    public bool IsConnected {
        get { return client.Client.IsConnected(); }
    }
}


/*
namespace Client
{
    public class Client
    {
        private static TcpClient client;
        private static string playerName;

        public static int Main(String[] args)
        {
            if (args.Length > 0)
                playerName = args[0];
            else
                playerName = "Player";

            Connect();

            return 0;
        }

        

        private static void Lobby()
        {
            Console.WriteLine("In Lobby...");
            bool inLobby = Message.Util.Send(client.GetStream(), new Message.EnterLobby(playerName));

            if (inLobby)
            {
                WaitStartMatch();
            }
            else
            {
                Console.WriteLine("Couldn't enter lobby; exit program");
            }
        }

        private static void WaitStartMatch()
        {
            Console.WriteLine("Waiting start match...");

            try
            {
                Message.BaseMessage message = Message.Util.Receive(client.GetStream(), Message.StartMatch.TypeId);
                Message.StartMatch startMatchMsg = (Message.StartMatch)message;
                StartMatch(startMatchMsg.symbol, startMatchMsg.begin);
            }
            catch (Message.NothingReceivedException)
            {
                Console.WriteLine("didnt receive start match message...");
            }
            catch (Message.WrongMsgReceivedException wrongMessage)
            {
                Console.WriteLine("Me llego un mensaje distinto que start match, {0}", wrongMessage.receivedMessage);
            }
        }

        private static void StartMatch(int symbolInt, bool begin)
        {
            Console.WriteLine("starting match...");
            Symbol symbol = (Symbol)symbolInt;

            Game.LocalPlayer selfPlayer = new Game.LocalPlayer(symbol);
            RemotePlayer otherPlayer = new RemotePlayer(symbol.Opposite(), client);

            Player first, second;

            if (begin)
            {
                first = selfPlayer;
                second = otherPlayer;
            }
            else
            {
                first = otherPlayer;
                second = selfPlayer;
            }

            var remoteTurnObservers = new Dictionary<Player, TcpClient>();
            remoteTurnObservers.Add(selfPlayer, client);

            Game.PlayerGame game = new Game.PlayerGame(first, second, remoteTurnObservers);

            Player winner = game.Play();

            if (winner == selfPlayer)
            {
                Console.WriteLine("Ganaste, capo");
            }
            else
            {
                Console.WriteLine("Perdiste, sos un gil");
            }

            WaitStartMatch();
        }
    }
}*/