using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class CloseMessage : Message
    {
        public CloseMessage(string rawMessage) : base(rawMessage)
        {
        }

        public CloseMessage(string source, string destination) : base(source, destination, NodeControl.CLOSE, null)
        {
        }
    }
}
