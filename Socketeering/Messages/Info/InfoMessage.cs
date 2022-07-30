using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    public class InfoMessage : Message
    {
        public InfoMessage(string rawMessage) : base(rawMessage)
        {
        }

        public InfoMessage(string source, string destination, string[] controlArgs) : base(source, destination, NodeControl.INFO, controlArgs)
        {
        }
    }
}
