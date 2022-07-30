using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class VersionMessage : Message
    {
        public VersionMessage(string rawMessage) : base(rawMessage)
        {
        }

        public VersionMessage(string source, string destination) : base(source, destination, NodeControl.VERSION, new string[] { })
        {
        }
    }
}
