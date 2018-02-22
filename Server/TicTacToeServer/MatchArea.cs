using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Game;

namespace Server
{
    public class MatchArea
    {
        private List<Game.Game> games;
		private Random random;
        public delegate void GameOverDelegate(Game.Result result, Client a, Client b);
        public event GameOverDelegate GameOver;

        public MatchArea()
        {
            games = new List<Game.Game>();
            random = new Random();
        }

        public void StartMatch(Client a, Client b)
        {
            Console.WriteLine("StartMatch({0}, {1})", a.name, b.name);
            a.symbol = Symbol.Circle;
            b.symbol = Symbol.Cross;

			Client[] players = new Client[2];

            if (random.NextDouble() < 0.5) {
                players[0] = a;
                players[1] = b;
            } else {
                players[0] = b;
                players[1] = a;
            }

            Console.WriteLine("First player is: ({0})", players[0].name);

            Console.WriteLine("Sending startMatch to player {0}", a.name);
            bool sentOk = Message.Util.Send(a.client.GetStream(), new Message.StartMatch((int) a.symbol, Array.IndexOf(players, a)));
            Console.WriteLine("send result? {0}", sentOk);

            Console.WriteLine("Sending startMatch to player {0}", b.name);
            sentOk = Message.Util.Send(b.client.GetStream(), new Message.StartMatch((int) b.symbol, Array.IndexOf(players, b)));
            Console.WriteLine("send result? {0}", sentOk);


			Game.Game newMatch = new Game.Game(players);

            Game.Result matchResult = newMatch.Play();
            GameOver(matchResult, a, b);
        }
    }
}
