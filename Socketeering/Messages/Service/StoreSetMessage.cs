using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Service
{
    internal class StoreSetMessage : StoreGetMessage
    {
        public StoreSetMessage(string rawMessage) : base(rawMessage)
        {
        }

        public StoreSetMessage(string source, string destination, string path, string value, DateTime? until)
            : base(source, destination, path, BuildArgs(value, until))
        {
        }

        private static Dictionary<string, string> BuildArgs(string value, DateTime? until)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("OP", "SET");
            args.Add("VALUE", value);
            if (until != null) args.Add("UNTIL", until.ToString());
            return args;
        }
    }
}
