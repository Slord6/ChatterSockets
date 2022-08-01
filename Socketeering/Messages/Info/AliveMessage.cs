using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class AliveMessage : Message
    {
        public string? Uptime
        {
            get => GetControlArg("UPTIME") ?? "-1";
            set => SetControlArgSafe("UPTIME", value);
        }

        public AliveMessage(string rawMessage) : base(rawMessage)
        {
        }

        public AliveMessage(string source, int? uptime) : base(source, "*", NodeControl.ALIVE,
            uptime != null ? new Dictionary<string, string> { { "UPTIME", uptime.ToString()} } : new Dictionary<string, string>())
        {
        }
    }
}
