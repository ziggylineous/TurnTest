using System.Collections.Generic;
using System.Net.Sockets;
using System;

namespace Game
{
    public class Result
    {
        public enum Type {
            Error,
            Ok
        }

        public Type type;
        public Server.Client winner;
    }

    public class Game
    {
        protected Board board;
        protected Server.Client[] players;
        protected int playerIndex;

        public Game(Server.Client[] players)
        {
            board = new Board(3, 3, 3);

            this.players = players;
        }

        public Result Play()
        {
            
            board.Clear();
            playerIndex = -1;
            Server.Client winner = null;

            Console.WriteLine("Game::Play(), match starting");

            while (winner == null && board.HasAnyEmpty)
            {
                playerIndex = NextPlayerIndex;
                Server.Client currentPlayer = players[playerIndex];

                Console.WriteLine("Game::Play(), {0} turn", currentPlayer.symbol.ToString());

				Position position;
				Message.Position positionMsg;

                try {
                    Message.BaseMessage message = Message.Util.Receive(currentPlayer.client.GetStream(), Message.Position.TypeId);
                    positionMsg = (Message.Position)message;
                    position = new Position(positionMsg.i, positionMsg.j);
                }
                catch (Message.NothingReceivedException)
                {
                    Result errorResult = new Result();
                    errorResult.type = Result.Type.Error;
                    return errorResult;
                }

                Console.WriteLine("Game::Play(), received position {0}", position);

                board.Mark(position, currentPlayer.symbol);

                Server.Client nextPlayer = players[NextPlayerIndex];
                bool sentOk = Message.Util.Send(nextPlayer.client.GetStream(), positionMsg);
                if (!sentOk)
                {
                    Result errorResult = new Result();
                    errorResult.type = Result.Type.Error;
                    return errorResult;
                }


                if (board.HasWon(position))
                    winner = currentPlayer;
            }

            Result okResult = new Result();
            okResult.type = Result.Type.Ok;

            if (board.HasAnyEmpty)
                okResult.winner = winner;

            return okResult;
        }

        protected int NextPlayerIndex
        {
            get => (playerIndex + 1) % players.Length;
        }
    }
}
