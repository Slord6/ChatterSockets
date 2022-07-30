using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class DenialMessage : Message
    {
        public DenialMessage(string rawMessage) : base(rawMessage)
        {
        }

        public DenialMessage(string source, string destination, Message incoming, string reason)
            : base(source, destination, NodeControl.DENIAL, new string[] { $"REQUEST:{incoming.MessageType}", $"REASON:{reason}" })
        {
        }
    }
}
