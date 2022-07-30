using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    public class TimeSyncMessage : Message
    {
        public TimeSyncMessage(string rawMessage) : base(rawMessage)
        {
        }

        public TimeSyncMessage(string source, string destination) : base(source, destination, NodeControl.TIME_SYNC, new string[] { DateTime.Now.ToString() })
        {
        }
    }
}
