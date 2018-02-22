using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        public TcpClient client;
        public string name;
        public delegate void Delegate(Client client);
        public Game.Symbol symbol;

        public Client(TcpClient client, string name)
        {
            this.client = client;
            this.name = name;
        }
    }
}
