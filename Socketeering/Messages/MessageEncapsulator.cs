using Socketeering.Messages.Error;
using Socketeering.Messages.Info;
using Socketeering.Messages.Request;
using Socketeering.Messages.Service;

namespace Socketeering.Messages
{
    static internal class MessageEncapsulator
    {
        public static Message Encapsulate(this Message message)
        {
            return MessageEncapsulator.MakeSpecific(message);
        }

        public static Message MakeSpecific(Message message)
        {
            switch (message.MessageType)
            {
                case NodeControl.INFO:
                    return new InfoMessage(message.BuildMessage());
                case NodeControl.DENIAL:
                    return new DenialMessage(message.BuildMessage());
                case NodeControl.ALIVE:
                    return new AliveMessage(message.BuildMessage());
                case NodeControl.TIME_SYNC:
                    return new TimeSyncMessage(message.BuildMessage());
                case NodeControl.NAME_CHANGE:
                    return new NameChangeMessage(message.BuildMessage());
                case NodeControl.CONNECTIVITY_SYNC:
                    return new ConnectivitySyncMessage(message.BuildMessage());
                case NodeControl.PEER_SYNC:
                    return new PeerSyncMessage(message.BuildMessage());

                case NodeControl.DISCONNECTING:
                    return new DisconnectingMessage(message.BuildMessage());
                case NodeControl.NAME:
                    return new NameMessage(message.BuildMessage());
                case NodeControl.VERSION:
                    return new VersionMessage(message.BuildMessage());
                case NodeControl.TIME:
                    return new TimeMessage(message.BuildMessage());
                case NodeControl.CONNECTIVITY:
                    return new ConnectivityMessage(message.BuildMessage());
                case NodeControl.RELAY:
                    return new RelayMessage(message.BuildMessage());
                case NodeControl.ECHO:
                    return new EchoMessage(message.BuildMessage());
                case NodeControl.ALERT:
                    return new AlertMessage(message.BuildMessage());
                case NodeControl.PEERS:
                    return new PeersMessage(message.BuildMessage());

                case NodeControl.SERVICE:
                    return new StandardServiceMessage(message.BuildMessage());
                case NodeControl.STORE:
                    if (message.ControlArgs["OP"] == "GET") return new StoreGetMessage(message.BuildMessage());
                    return new StoreGetMessage(message.BuildMessage());
                case NodeControl.CLOSE:
                    return new CloseMessage(message.BuildMessage());
                case NodeControl.NAME_CLASH:
                    return new NameClashMessage(message.BuildMessage());
                case NodeControl.UNPARSABLE:
                    return new UnparsableMessage(message.BuildMessage());
                case NodeControl.INVALID_CONTROL:
                    return message;
                default:
                    throw new ArgumentException("MessageEncapsulator cannot handle " + message.MessageType);
            }
        }
    }
}
