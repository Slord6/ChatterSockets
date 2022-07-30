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
        private static readonly string ARG_KV_SEP = ":::";

        protected readonly string source;
        protected readonly string destination;
        protected readonly string rawMessage;
        protected readonly NodeControl messageType;
        protected readonly Dictionary<string,string> controlArgs;

        public string Source { get { return source; } }
        public string Destination { get { return destination; } }
        public NodeControl MessageType { get { return messageType; } }
        public Dictionary<string,string> ControlArgs { get { return controlArgs; } }

        // TODO: support optional REF arg
        public Message(string source, string destination, NodeControl messageType, Dictionary<string, string>? controlArgs)
        {
            this.source = source;
            this.destination = destination;
            this.messageType = messageType;
            this.controlArgs = controlArgs ?? new Dictionary<string, string>();
            this.controlArgs.Add("ID", Guid.NewGuid().ToString());
            rawMessage = BuildMessage();
        }

        public Message(string rawMessage)
        {
            try
            {
                string[] messageParts = rawMessage.Split(SOURCE_SEP);
                source = messageParts[0];

                messageParts = CollateAllButFirst(messageParts, SOURCE_SEP).Split(DEST_SEP);
                destination = messageParts[0];

                messageParts = CollateAllButFirst(messageParts, DEST_SEP).Split(TYPE_SEP);
                messageType = messageParts[0].ToNodeControl();


                // TODO: change this to regex match
                // Support key:::"value with ::: whatever \" in it"
                // Change ARG_KV_SEP to ":"
                controlArgs = new Dictionary<string, string>();
                // Collect all remaining as args
                string[] args = CollateAllButFirst(messageParts, TYPE_SEP).Split(ARG_SEP);
                foreach (string arg in args)
                {
                    string[] keyVal = arg.Split(ARG_KV_SEP);
                    if(keyVal.Length < 2) continue;
                    // Drop any null chars off the end of the value
                    controlArgs.Add(keyVal[0], keyVal[1].Split('\0')[0]);
                }

                this.rawMessage = rawMessage;
            }
            catch
            {
                if (destination == null) destination = "";
                if (controlArgs == null) controlArgs = new Dictionary<string, string>();
                if (source == null) source = "";
                messageType = NodeControl.INVALID_CONTROL;
                this.rawMessage = rawMessage;
            }
        }

        private string CollateAllButFirst(string[] orig, string joinWith)
        {
            string[] allButfirst = new string[orig.Length - 1];
            Array.Copy(orig, 1, allButfirst, 0, allButfirst.Length);
            return String.Join(joinWith, allButfirst);
        }

        public string BuildMessage()
        {
            string[] controlArgsStrings = new string[ControlArgs.Count];
            int i = 0;
            foreach(KeyValuePair<string, string> arg in ControlArgs)
            {
                controlArgsStrings[i] = arg.Key + ARG_KV_SEP + arg.Value;
                i++;
            }

            return Source + SOURCE_SEP + Destination + DEST_SEP + MessageType.ToString() + TYPE_SEP + string.Join(ARG_SEP, controlArgsStrings);
        }


        public override string ToString()
        {
            return rawMessage;
        }
    }
}
