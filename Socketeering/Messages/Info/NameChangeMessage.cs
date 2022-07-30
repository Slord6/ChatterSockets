using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class NameChangeMessage : Message
    {
        public NameChangeMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NameChangeMessage(string source, string destination, string oldName) : base(source, destination, NodeControl.INFO,
            new Dictionary<string, string>() { { "OLDNAME", oldName} })
        {
        }
    }
}
