using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Error
{
    internal class NotImplementedMessage : Message
    {
        public NotImplementedMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NotImplementedMessage(string source, string destination, Message incoming)
            : base(source, destination, NodeControl.DENIAL, new string[] { $"REQUEST:{incoming.MessageType}"})
        {
        }
    }
}
