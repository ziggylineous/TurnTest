using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public class Lobby
    {
        private List<Client> awaitingMatchClients;
        public delegate void MatchFoundDelegate(Client a, Client b);
        public event MatchFoundDelegate MatchFound;

        public Lobby()
        {
            awaitingMatchClients = new List<Client>();
        }

        public void Enter(Client client) {
            Console.WriteLine("Lobby: {0} entered", client.name);

            Message.Util.Receive<Message.StartMatchRequest>(
                client.client.GetStream(),
                startMatchRequest => MakeMatch(client),
                wrongMessage => {
                    Console.WriteLine("ReceptionArea: received {0} message instead of Enter\nClosing that client", wrongMessage.ToString());
                    client.client.Close();
                },
                exception => Console.WriteLine("ReceptionArea: nothing received from client {0}", client.ToString())
            );
        }

        private void MakeMatch(Client client)
        {
            Console.WriteLine("MakeMatch() [a message was received]");
            RemoveDisconnectedClients();

            Client opponentClient = null;

            lock (awaitingMatchClients)
            {
                if (awaitingMatchClients.Count > 0)
                {
                    int lastIndex = awaitingMatchClients.Count - 1;
                    opponentClient = awaitingMatchClients[lastIndex];
                    awaitingMatchClients.RemoveAt(lastIndex);
                }
                else
                {
                    awaitingMatchClients.Add(client);
                }
            }

            if (opponentClient != null)
                MatchFound(client, opponentClient);
        }

        private void RemoveDisconnectedClients() {
            lock (awaitingMatchClients) {
				for (int i = awaitingMatchClients.Count - 1; i > -1; --i) {
					Client client = awaitingMatchClients[i];
					
					if (!client.client.Client.IsConnected()) {
                        Console.WriteLine("Lobby: removing client {0}", client.name);
						awaitingMatchClients.RemoveAt(i);
					}
				}
            }
        }
    }
}
