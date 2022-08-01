using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class AlertMessage : RefableMessage
    {
        public int? Priority
        {
            get => int.Parse(GetControlArg("PRIORITY") ?? "0");
            set => SetControlArg("PRIORITY", value?.ToString() ?? "0");
        }

        public string? Message
        {
            get => GetControlArg("MESSAGE");
            set => SetControlArgSafe("MESSAGE", value);
        }

        public AlertMessage(string rawMessage) : base(rawMessage)
        {
        }

        public AlertMessage(string source, string destination, int? priority, string? message, string? @ref) : base(source, destination, NodeControl.ECHO, @ref,
            BuildArgs(priority, message))
        {
        }

        private static Dictionary<string, string> BuildArgs(int? priority, string? message)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (priority.HasValue) args.Add("PRIORITY", priority.ToString());
            if (message != null) args.Add("MESSAGE", message);
            return args;
        }
    }
}
