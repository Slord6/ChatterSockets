using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    public class TimeSyncMessage : RefableMessage
    {
        public string Time
        {
            get => GetControlArgSafe("TIME");
            set => SetControlArg("TIME", value);
        }

        public string? Zone
        {
            get => GetControlArg("ZONE");
            set => SetControlArg("ZONE", value);
        }

        public TimeSyncMessage(string rawMessage) : base(rawMessage)
        {
        }

        public TimeSyncMessage(string source, string destination, string? @ref) : base(source, destination, NodeControl.TIME_SYNC, @ref,
            new Dictionary<string, string>() { { "TIME", DateTime.Now.ToString() }, { "ZONE", TimeZoneInfo.Local.ToString() } })
        {
        }
    }
}
