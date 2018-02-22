using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Message
{
	public class NothingReceivedException : Exception {
		public NothingReceivedException(Exception inExcept) : base("Didnt receive any message", inExcept) {}
	}
	
	public class WrongMsgReceivedException : Exception
	{
		public int spectedType;
		public Message.BaseMessage receivedMessage;
		
		public WrongMsgReceivedException(int spectedType, Message.BaseMessage gotType)
            : base(string.Format("Received wrong message: spected {0} but received {1} ({2})", spectedType, gotType, gotType.Type))
		{
			this.spectedType = spectedType;
			this.receivedMessage = gotType;
		}
    }

    public static class Util
    {

        public static bool Send(NetworkStream ns, BaseMessage message)
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
                Console.WriteLine("SendMessage {0} SerializationError: {1}/{2}", message, sendError.Message, sendError.InnerException);
                return false;
            }
            catch (System.IO.IOException socketShutDown)
            {
                Console.WriteLine("SendMessage {0} IOError (socket shut down?): {1}/{2}", message, socketShutDown.Message, socketShutDown.InnerException);
                return false;
            }
        }

        public static BaseMessage Receive(NetworkStream networkStream, int spectedMessageType)
        {
            Console.WriteLine("ReceiveMessage()");

            IFormatter formatter = new BinaryFormatter();

            try
            {
                Message.BaseMessage message = (Message.BaseMessage)formatter.Deserialize(networkStream);

                if (message.Type != spectedMessageType)
                    throw new WrongMsgReceivedException(spectedMessageType, message);

                return message;
            }
            catch (SerializationException receiveError)
            {
                throw new NothingReceivedException(receiveError.InnerException);
            }
        }


        public static void Receive<MessageType>(
            NetworkStream networkStream,
            Action<MessageType> onReceiveSuccess,
            Action<BaseMessage> onReceiveWrongMessage,
            Action<Exception> onReceiveError) where MessageType : BaseMessage
        {
            Console.WriteLine("ReceiveMessage()");
			IFormatter formatter = new BinaryFormatter();

            try
            {
                BaseMessage message = (BaseMessage)formatter.Deserialize(networkStream);

                if (message is MessageType)
                    onReceiveSuccess((MessageType) message);
                else
                    onReceiveWrongMessage(message);
            }
            catch (SerializationException receiveError)
            {
                onReceiveError(new NothingReceivedException(receiveError.InnerException));
            }
        }
    }
}
