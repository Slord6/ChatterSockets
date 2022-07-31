using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages
{
    internal class MessageArrival
    {
        public Message Message { get; }
        public DateTime ArrivedAt { get; }

        public MessageArrival(Message arrival)
        {
            this.Message = arrival;
            ArrivedAt = DateTime.Now;
        }
    }
}
