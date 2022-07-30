using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class TimeMessage : Message
    {
        public TimeMessage(string rawMessage) : base(rawMessage)
        {
        }

        public TimeMessage(string source, string destination) : base(source, destination, NodeControl.TIME, null)
        {
        }
    }
}
