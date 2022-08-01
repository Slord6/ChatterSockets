using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Service
{
    internal class CustomServiceMessage : StandardServiceMessage
    {
        public CustomServiceMessage(string rawMessage) : base(rawMessage)
        {
        }

        public CustomServiceMessage(string source, string destination, string messageArg, string argsArg, string descriptionArg, string verisonArg)
            : base(source, destination, messageArg, argsArg, descriptionArg, verisonArg)
        {
        }
    }
}
