using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Error
{
    internal class NameClashMessage : Message
    {
        public string Name
        {
            get
            {
                return GetControlArgSafe("NAME");
            }
            set
            {
                SetControlArg("NAME", value);
            }
        }

        public NameClashMessage(string rawMessage) : base(rawMessage)
        {
        }

        public NameClashMessage(string source, string destination, string clashingName) : base(source, destination, NodeControl.NAME_CLASH,
            new Dictionary<string, string>() { { "NAME", clashingName } })
        {
        }
    }
}
