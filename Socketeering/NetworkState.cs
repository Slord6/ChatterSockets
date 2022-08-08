using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering
{
    public class NetworkState
    {
        private Dictionary<Node, List<RemoteNode>> remoteNodes;

        public NetworkState()
        {
            remoteNodes = new Dictionary<Node, List<RemoteNode>>();
        }

        public void Monitor(Node node)
        {
            remoteNodes.Add(node, new List<RemoteNode>());
            node.onMessageArrived.Add(MessageArrived);
        }

        public void UnMonitor(Node node)
        {
            node.onMessageArrived.Remove(MessageArrived);
            remoteNodes.Remove(node);
        }

        public List<RemoteNode> AvailableNodes(Node node)
        {
            return remoteNodes[node].Where(n => n.IsAvailable).ToList();
        }

        public void DiscardInactiveNodes()
        {
            foreach (KeyValuePair<Node, List<RemoteNode>> remotes in remoteNodes)
            {
                List<string> inactives = remotes.Value.Where(n => !n.IsAvailable).Select(n => n.Name ).ToList();
                if(inactives.Count > 0) Console.WriteLine($"Removed {String.Join(",", inactives)} due to timeout");
                remoteNodes[remotes.Key] = remotes.Value.Where(n => n.IsAvailable).ToList();
            }
        }

        private void MessageArrived(Node node, Messages.Message incoming)
        {
            RemoteNode? sender;
            List<RemoteNode> knownNodes = remoteNodes[node];

            sender = knownNodes.Where(n => n.Name == incoming.Source).FirstOrDefault();
            if(sender == default(RemoteNode))
            {
                sender = new RemoteNode(incoming.Source);
                knownNodes.Add(sender);
            }

            sender.AddMessage(incoming);

            Console.WriteLine(sender.ToString());
        }
    }
}
