using System;
using System.IO;


public class Program
{
    public static int Main(String[] args)
    {
        Server.ReceptionArea reception = new Server.ReceptionArea();
		Server.MatchArea matchArea = new Server.MatchArea();
        Server.Lobby lobby = new Server.Lobby();

        reception.ClientEntered += lobby.Enter;
        lobby.MatchFound += matchArea.StartMatch;
        matchArea.GameOver += (result, a, b) => ProccesGameResult(result, a, b, lobby);

		reception.Start();

        return 0;
    }

    private static void ProccesGameResult(Game.Result result, Server.Client a, Server.Client b, Server.Lobby lobby)
    {
        switch (result.type)
        {
            case Game.Result.Type.Ok:
                if (result.winner != null)
                    Console.WriteLine("winner: {0}", result.winner.name);
                else
                    Console.WriteLine("tie {0} between {1}", a.name, b.name);
                break;

            case Game.Result.Type.Error:
                Console.WriteLine("some error while playing");

                if (a.client.Client.IsConnected() && !b.client.Client.IsConnected())
                    Message.Util.Send(a.client.GetStream(), new Message.OpponentDisconnected());

                if (!a.client.Client.IsConnected() && b.client.Client.IsConnected())
                    Message.Util.Send(b.client.GetStream(), new Message.OpponentDisconnected());
                
                break;
        }

        if (a.client.Client.IsConnected()) lobby.Enter(a);
        else Console.WriteLine("{0} disconnected while playing", a);

        if (b.client.Client.IsConnected()) lobby.Enter(b);
        else Console.WriteLine("{0} disconnected while playing", b);
    }
}