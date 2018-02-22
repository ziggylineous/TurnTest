using System.Net;
using System.Net.Sockets;

public static class SocketExtensions
{
    public static bool IsConnected(this Socket socket)
    {
        try
        {
            return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        catch (SocketException) { return false; }
    }

    public static bool IsConnected222(this Socket socket)
    {
        if (socket.Poll(0, SelectMode.SelectRead))
        {
            byte[] buff = new byte[1];
            if (socket.Receive(buff, SocketFlags.Peek) == 0)
            {
                return false;
            }
        }

        return true;
    }
}