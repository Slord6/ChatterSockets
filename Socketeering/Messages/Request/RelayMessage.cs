using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class RelayMessage : Message
    {
        public Message Message
        {
            get => new Message(GetControlArgSafe("MESSAGE"));
            set => SetControlArg("MESSAGE", value.BuildMessage());
        }

        public RelayMessage(string rawMessage) : base(rawMessage)
        {
        }

        public RelayMessage(string source, string destination, Message fwdMessage, string toNode) : base(source, destination, NodeControl.RELAY,
            new Dictionary<string, string>() { { "TO", toNode}, { "MESSAGE", fwdMessage.BuildMessage() } })
        {
        }
    }
}
