using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    public class ReceptionArea
    {
        private List<Client> enteringClients;
        private TcpListener listener;
        bool done;
        public event Client.Delegate ClientEntered;

        public ReceptionArea() {
            enteringClients = new List<Client>();

            Config config = Server.Config.Load("server_config.json");
            listener = new TcpListener(IPAddress.Parse(config.ip), config.port);
        }

        public void Start() {
            Console.WriteLine("Reception clients");
            listener.Start();
            done = false;

            while (!done)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();

                    Thread clientThread = new Thread(ClientThread);
                    clientThread.Start(client);
                }
                catch (SocketException sockException)
                {
                    Console.WriteLine("AcceptTcpClient error {0}/{1}", sockException, sockException.ErrorCode);
                }
            }

            listener.Stop();
        }

        private void ClientThread(object tcpClientObject)
        {
            TcpClient client = tcpClientObject as TcpClient;
            Client playerClient = new Client(client, "Player");
            Console.WriteLine("Client Thread {0}...", client.Client);

            lock (enteringClients)
            {
                enteringClients.Add(playerClient);
            }

            Message.Util.Receive<Message.EnterLobby>(
                client.GetStream(),
                enterLobby => {
                    playerClient.name = enterLobby.name;
                    ClientEntered(playerClient);
                },
                wrongMessage => {
                    Console.WriteLine("ReceptionArea: received {0} message instead of Enter\nClosing that client", wrongMessage.ToString());
                    playerClient.client.Close();
                },
                exception => Console.WriteLine("ReceptionArea: nothing received from client {0}", playerClient.client.ToString())
            );

            lock (enteringClients)
            {
                enteringClients.Remove(playerClient);
            }
            // thread end
        }
    }
}
