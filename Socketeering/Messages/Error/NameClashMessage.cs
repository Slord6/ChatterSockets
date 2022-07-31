using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Error
{
    internal class NameClashMessage : Message
    {
        public NameClashMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NameClashMessage(string source, string clashingName) : base(source, "*", NodeControl.NAME_CLASH,
            new Dictionary<string, string>() { { "NAME", clashingName } })
        {
        }
    }
}
