using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Error
{
    internal class UnparsableMessage : RefableMessage
    {
        public UnparsableMessage(string rawMessage) : base(rawMessage)
        {
        }

        public UnparsableMessage(string source, Message incoming) : base(source, incoming.Source, NodeControl.UNPARSABLE, incoming.ID, null)
        {
        }
    }
}
