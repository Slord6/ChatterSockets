using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages
{
    public class Message
    {
        private static readonly string SOURCE_SEP = ";";
        private static readonly string DEST_SEP = "|";
        private static readonly string TYPE_SEP = "!";
        private static readonly string ARG_SEP = ",";

        private readonly string source;
        private readonly string destination;
        private readonly string rawMessage;
        private readonly NodeControl messageType;
        private readonly string[] controlArgs;

        public string Source { get { return source; } }
        public string Destination { get { return destination; } }
        public NodeControl MessageType { get { return messageType; } }
        public string[] ControlArgs { get { return controlArgs; } }

        public Message(string source, string destination, NodeControl messageType, string[] controlArgs)
        {
            this.source = source;
            this.destination = destination;
            this.messageType = messageType;
            this.controlArgs = controlArgs;
            rawMessage = BuildMessage();
        }

        public Message(string rawMessage)
        {
            try
            {
                string[] messageParts = rawMessage.Split(SOURCE_SEP);
                source = messageParts[0];

                messageParts = messageParts[1].Split(DEST_SEP);
                destination = messageParts[0];

                messageParts = messageParts[1].Split(TYPE_SEP);
                messageType = messageParts[0].ToNodeControl();

                controlArgs = messageParts[1].Split(ARG_SEP);

                this.rawMessage = rawMessage;
            }
            catch
            {
                if (destination == null) destination = "";
                if (controlArgs == null) controlArgs = new string[0];
                if (source == null) source = "";
                messageType = NodeControl.INVALID_CONTROL;
                this.rawMessage = rawMessage;
            }
        }

        public string BuildMessage()
        {
            return Source + SOURCE_SEP + Destination + DEST_SEP + MessageType.ToString() + TYPE_SEP + string.Join(ARG_SEP, ControlArgs);
        }


        public override string ToString()
        {
            return rawMessage;
        }
    }
}
