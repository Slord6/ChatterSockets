using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Socketeering
{
    public class Gateway
    {
        private readonly int PORT;
        // Multicast range is 224.0.0.0-239.255.255.255 
        private readonly IPAddress MULTICAST_ADDRESS;

        private readonly Socket sendSocket;
        private readonly Socket recvSocket;
        private readonly int ttl;

        public Gateway(int port, IPAddress multicastAddress, int ttl)
        {
            PORT = port;
            MULTICAST_ADDRESS = multicastAddress;
            this.ttl = ttl;

            sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Connect();
        }

        public void Connect()
        {
            ConnectSend();
            ConnectRecv();
        }

        private void ConnectRecv()
        {
            if (recvSocket.Connected) return;
            // Bind socket to multicast endpoint
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, PORT);
            recvSocket.Bind(ipep);
            
            // Join multicast group
            recvSocket.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership, new MulticastOption(MULTICAST_ADDRESS, IPAddress.Any));
        }

        private void ConnectSend()
        {
            if (sendSocket.Connected) return;

            // join multicast group
            sendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MULTICAST_ADDRESS));

            // TTL, 1 = within local network (1 hop)
            sendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl);
            
            IPEndPoint multicastEndpoint = new IPEndPoint(MULTICAST_ADDRESS, PORT);

            // Connect to the multicast endpoint
            sendSocket.Connect(multicastEndpoint);
        }

        public void CloseAll()
        {
            sendSocket.Close();
            recvSocket.Close();
        }

        public void SendSync(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            sendSocket.Send(messageBytes, messageBytes.Length, SocketFlags.None);
        }

        public async void Send(string message)
        {
            Task sendTask = new Task(() =>
            {
                SendSync(message);
            });
            sendTask.Start();
            await sendTask;
        }

        public Task RecvMessageContinuous(Action<string> onMessage, CancellationToken? token = null)
        {
            CancellationToken cancellationToken = token ?? CancellationToken.None;

            Task recvTask = new Task(() =>
            {
                while (true)
                {
                    string msg = RecvMessageSync();
                    if (cancellationToken.IsCancellationRequested) return;
                    onMessage(msg);
                }
            }, cancellationToken);
            recvTask.Start();
            return recvTask;
        }

        public string RecvMessageSync()
        {
            string? str;

            do
            {
                byte[] incomingBuffer = new byte[2048];
                recvSocket.Receive(incomingBuffer);
                str = Encoding.ASCII.GetString(incomingBuffer, 0, incomingBuffer.Length).Trim();
            } while (str.Length == 0);

            return str;
        }

        public async Task<string> RecvMessage()
        {
            Task<string> recvTask = new Task<string>(() =>
            {
                string str = RecvMessageSync();
                return str;
            });
            await recvTask;
            return recvTask.Result;
        }
    }
}
