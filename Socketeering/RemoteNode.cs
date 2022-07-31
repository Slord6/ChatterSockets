using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering
{
    internal class RemoteNode
    {
        public static int CONNECTED_TIMEOUT_S = 75;

        public string Name { get; private set; }
        private List<Messages.Message> messages;

        private DateTime lastSeen;

        public DateTime LastSeen { get => lastSeen; }
        public TimeSpan TimeSinceLastSeen
        {
            get
            {
                return DateTime.Now - LastSeen;
            }
        }
        public bool IsAvailable
        {
            get
            {
                return TimeSinceLastSeen.TotalSeconds < CONNECTED_TIMEOUT_S;
            }
        }

        public RemoteNode(string name)
        {
            this.Name = name;
            this.messages = new List<Messages.Message>();
            lastSeen = DateTime.Now;
        }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public void AddMessage(Messages.Message message)
        {
            lastSeen = DateTime.Now;
            this.messages.Add(message);
        }

        public override string ToString()
        {
            return $"Node ({Name}), {messages.Count} messages recieved. Last seen {TimeSinceLastSeen} ago";
        }
    }
}
