using Socketeering.Messages;
using Socketeering.Messages.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Services
{
    internal class StoreServiceHandler
    {
        private enum Op
        {
            GET,
            SET
        }

        private class StoreRequest
        {
            public Op op;
            public string path;
            public string? value;
            public string? until;
        }


        private IStoreService storeService;

        public StoreServiceHandler(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        public void HandleStoreMessage(Message storeMessage, Node node)
        {
            StoreRequest? req;
            if(!MessageToStoreRequest(storeMessage, out req)) {
                Console.WriteLine($"Invalid store message from {storeMessage.Source} - {storeMessage}");
                return;
            } else
            {
                if (req == null) return; // This will never happen
                switch(req.op)
                {
                    case Op.GET:
                        Get(req, node, storeMessage);
                        return;
                    case Op.SET:
                        Set(req, node, storeMessage);
                        return;
                }
            }
        }

        private void Get(StoreRequest req, Node node, Message incoming)
        {
            string value;
            if (storeService.TryGet(req.path, out value))
            {
                Console.WriteLine($"{incoming.Source} retrieving {value} from {req.path}");
                InfoMessage message = new InfoMessage(node.Name, incoming.Source, incoming.ID, new Dictionary<string, string> { { "VALUE", value } });
                node.Send(message);
            }
            else
            {
                Console.WriteLine($"{incoming.Source} requested non-existant value at {req.path}");
                DenialMessage deny = new DenialMessage(node.Name, incoming, "NO-DATA");
                deny.ControlArgs.Add("REASONDESCRIPTION", "No data at that location");
                node.Send(deny);
            }
        }

        private void Set(StoreRequest req, Node node, Message incoming)
        {
            Console.WriteLine($"{incoming.Source} is storing {req.value} at {req.path}");
            storeService.Set(req.path, req.value);
            InfoMessage confMessage = new InfoMessage(node.Name, incoming.Source, incoming.ID, new Dictionary<string, string> { { "STORED", req.path } });
            node.Send(confMessage);
        }

        private bool MessageToStoreRequest(Message storeMessage, out StoreRequest? storeRequest)
        {
            try
            {
                Op op = Enum.Parse<Op>(storeMessage.ControlArgs["OP"]);
                string path = storeMessage.ControlArgs["PATH"];
                string? value = null;
                if (op == Op.SET)
                {
                    value = storeMessage.ControlArgs["VALUE"];
                }
                storeRequest = new StoreRequest() { op = op, path = path, value = value };
                return true;
            }
            catch(Exception ex)
            {
                storeRequest = null;
                return false;
            }
        }
    }
}
