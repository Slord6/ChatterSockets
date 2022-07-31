using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class DenialMessage : RefableMessage
    {
        public DenialMessage(string rawMessage) : base(rawMessage)
        {
        }

        public DenialMessage(string source, Message incoming, string reason)
            : base(source, incoming.Source, NodeControl.DENIAL, incoming.ControlArgs.ContainsKey("ID") ? incoming.ID : null,
                  new Dictionary<string, string>() { { "REQUEST", incoming.MessageType.ToString()}, { "REASON", reason } })
        {
        }
    }
}
