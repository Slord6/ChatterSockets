using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Service
{
    internal class StandardServiceMessage : Message
    {
        public StandardServiceMessage(string rawMessage) : base(rawMessage)
        {
        }

        public StandardServiceMessage(string source, string destination, string messageArg, string? argsArg, string? descriptionArg, string? verisonArg)
            : base(source, destination, NodeControl.ALIVE, BuildArgs(messageArg, argsArg, descriptionArg, verisonArg))
        {
        }

        private static Dictionary<string, string> BuildArgs(string message, string? args, string? description, string? version)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("MESSAGE", message);
            if (args != null) result.Add("ARGS", args);
            if (description != null) result.Add("DESCRIPTION", description);
            if (version != null) result.Add("VERSION", version);
            return result;
        }
    }
}
