using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Error
{
    internal class NotImplementedMessage : RefableMessage
    {
        public string Request
        {
            get => GetControlArgSafe("REQUEST");
            set => SetControlArg("REQUEST", value);
        }

        public NotImplementedMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NotImplementedMessage(string source, Message incoming, string? subset = null) : base(source, incoming.Source, NodeControl.NOT_IMPLEMENTED, incoming.ID,
            new Dictionary<string, string> {
                { "REQUEST", incoming.MessageType.ToString() + (subset != null ? $";{subset}" : "" )
                } })
        {
        }
    }
}
