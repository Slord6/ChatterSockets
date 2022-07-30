using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages
{
    public class RefableMessage : Message
    {
        public RefableMessage(string rawMessage) : base(rawMessage)
        {
        }

        public RefableMessage(string source, string destination, NodeControl messageType, string? @ref, Dictionary<string, string>? controlArgs) : base(source, destination, messageType, controlArgs)
        {
            if (@ref != null)
            {
                if (this.controlArgs.ContainsKey("REF"))
                {
                    this.controlArgs["REF"] = @ref;
                }
                else
                {
                    this.controlArgs.Add("REF", @ref);
                }
            }
        }
    }
}
