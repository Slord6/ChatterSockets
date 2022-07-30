using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    public class InfoMessage : RefableMessage
    {
        public InfoMessage(string rawMessage) : base(rawMessage)
        {
        }

        public InfoMessage(string source, string destination, string? @ref, Dictionary<string, string> controlArgs) : base(source, destination, NodeControl.INFO, @ref, controlArgs)
        {
        }
    }
}
