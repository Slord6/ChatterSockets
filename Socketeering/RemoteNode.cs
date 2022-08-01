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
        private List<Messages.MessageArrival> messages;

        private DateTime lastSeen;
        public DateTime LastSeen { get => lastSeen; }
        public int Uptime_s
        {
            get
            {
                // Most recent message
                Messages.MessageArrival? messageArrival = messages
                    .Where(m => m.Message.MessageType == NodeControl.ALIVE)
                    .LastOrDefault();
                if (messageArrival == null)
                {
                    Messages.MessageArrival? firstMessage = messages.FirstOrDefault();
                    if (firstMessage == null) return -1;
                    return (int)(DateTime.Now - firstMessage.ArrivedAt).TotalSeconds;
                }
                string? uptime = ((Messages.Info.AliveMessage)messageArrival.Message).Uptime;
                return int.Parse(uptime ?? "-1");
            }
        }
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
        public DateTime TimeAtNode
        {
            get
            {
                // Most recent message
                Messages.MessageArrival? messageArrival = messages
                    .Where(m => m.Message.MessageType == NodeControl.TIME_SYNC)
                    .LastOrDefault();
                // If we don't know for sure, we just assume same as us
                if(messageArrival == null)
                {
                    return DateTime.Now;
                }
                TimeSpan timeSinceTimeSync = DateTime.Now - messageArrival.ArrivedAt;

                // Check that the TIME_SYNC is valid
                if(!messageArrival.Message.ControlArgs.ContainsKey("TIME"))
                {
                    // If it is invalid, remove it and try again
                    messages.Remove(messageArrival);
                    return TimeAtNode;
                }

                return DateTime.Parse(messageArrival.Message.ControlArgs["TIME"]) + timeSinceTimeSync;
            }
        }
        /// <summary>
        /// We assume that the time offset of a node is how long a TIME_SYNC took to arrive
        /// </summary>
        public double EstimatedMessageFlightTimeMs
        {
            get
            {
                return (DateTime.Now - TimeAtNode).TotalMilliseconds;
            }
        }
        public string LastAlertMessage
        {
            get
            {
                // Most recent message
                Messages.MessageArrival? messageArrival = messages
                    .Where(m => m.Message.MessageType == NodeControl.ALERT)
                    .LastOrDefault();
                if (messageArrival == null)
                {
                    return "";
                }
                return ((Messages.Request.AlertMessage)messageArrival.Message).Message ?? "";
            }
        }

        public RemoteNode(string name)
        {
            this.Name = name;
            this.messages = new List<Messages.MessageArrival>();
            lastSeen = DateTime.Now;
        }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public void AddMessage(Messages.Message message)
        {
            lastSeen = DateTime.Now;
            this.messages.Add(new Messages.MessageArrival(message));
        }

        public override string ToString()
        {
            return $"Node ({Name})\n" +
                $"{messages.Count} messages recieved.\n" +
                $"Last seen {TimeSinceLastSeen} ago.\n" +
                $"Flight time {EstimatedMessageFlightTimeMs}ms.\n" +
                $"Time at node {TimeAtNode}.\n" +
                $"Uptime: {Uptime_s}s\n" +
                $"Last alert message: {LastAlertMessage}";
        }
    }
}
