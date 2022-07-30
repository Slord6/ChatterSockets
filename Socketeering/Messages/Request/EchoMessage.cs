using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class EchoMessage : Message
    {
        public EchoMessage(string rawMessage) : base(rawMessage)
        {
        }

        public EchoMessage(string source, string destination, string value) : base(source, destination, NodeControl.CLOSE,
            new Dictionary<string, string>() { { "VALUE", value } })
        {
        }
    }
}
