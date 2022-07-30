using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class NameMessage : Message
    {
        public NameMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NameMessage(string source, string destination) : base(source, destination, NodeControl.NAME, new Dictionary<string, string>())
        {
        }
    }
}
