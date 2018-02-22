using System;
using System.Runtime.Serialization;

namespace Message
{
    [Serializable]
    public abstract class BaseMessage : ISerializable
    {
        abstract public int Type { get; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type);
        }
    }

    [Serializable]
    public class EnterLobby : BaseMessage {
        public string name;

        public EnterLobby(string name) {
            this.name = name;
        }

        public EnterLobby(SerializationInfo info, StreamingContext context) {
            name = (string)info.GetValue("name", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("name", name);
        }

        public static readonly int TypeId = "EnterLobby".GetHashCode();
        override public int Type {
            get { return TypeId; }
        }

        public override string ToString()
        {
            return string.Format("EnterLobby message: name={0}", name);
        }
    }

    [Serializable]
    public class StartMatchRequest : BaseMessage
    {
        public StartMatchRequest() { }
        public StartMatchRequest(SerializationInfo info, StreamingContext context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }

        public static readonly int TypeId = "StartMatchRequest".GetHashCode();
        override public int Type
        {
            get { return TypeId; }
        }
    }

    [Serializable]
    public class StartMatch : BaseMessage
    {
        public int symbol;
        public int playersTurnIndex;

        public StartMatch(int symbol, int playersTurnIndex)
        {
            this.symbol = symbol;
            this.playersTurnIndex = playersTurnIndex;
        }

        public StartMatch(SerializationInfo info, StreamingContext context)
        {
            symbol = info.GetInt32("symbol");
            playersTurnIndex = info.GetInt32("playersTurnIndex");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("symbol", symbol);
            info.AddValue("playersTurnIndex", playersTurnIndex);
        }

        public static readonly int TypeId = "StartMatch".GetHashCode();
        override public int Type
        {
            get { return TypeId; }
        }

        public override string ToString()
        {
            return string.Format("StartMatch message: symbol={0}, playersTurnIndex={1}", symbol, playersTurnIndex);
        }
    }

    [Serializable]
    public class Position : BaseMessage
    {
        public int i;
        public int j;

        public Position(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public Position(SerializationInfo info, StreamingContext context)
        {
            i = info.GetInt32("i");
            j = info.GetInt32("j");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("i", i);
            info.AddValue("j", j);
        }

        public static readonly int TypeId = "Play".GetHashCode();
        override public int Type
        {
            get { return TypeId; }
        }

        public override string ToString()
        {
            return string.Format("Position message: (i, j) = ({0}, {1})", i, j);
        }

        public Game.Position GamePosition { get { return new Game.Position(i, j); } }
    }

    [Serializable]
    public class OpponentDisconnected : BaseMessage
    {
        public OpponentDisconnected()
        {
        }

        public OpponentDisconnected(SerializationInfo info, StreamingContext context)
        {
        }

        public static readonly int TypeId = "OpponentDisconnected".GetHashCode();
        override public int Type
        {
            get { return TypeId; }
        }

        public override string ToString()
        {
            return "Opponent Disconnected";
        }
    }
}