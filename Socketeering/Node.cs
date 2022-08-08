using Socketeering.Messages;
using Socketeering.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering
{
    public class Node
    {
        private readonly static int version = 1;
        private readonly Gateway gateway;

        public string Name { get => NameGenerator.GetName(); }
        public NetworkState State { get => networkState; }
        public bool OutputDiscards { get; set; }
        public List<Action<Node, Message>> onMessageArrived { get; private set; }
        public List<Action<Node, string>> onPreMessageArrived { get; private set; }
        public static int KEEP_ALIVE_S = 20;

        // Services
        StoreServiceHandler storeHandler = new StoreServiceHandler(new KeyValueStoreService());

        private DateTime startedAt;
        private NetworkState networkState;

        public Node(Gateway gateway)
        {
            this.gateway = gateway;
            this.OutputDiscards = true;
            this.onMessageArrived = new List<Action<Node, Message>>();
            this.onPreMessageArrived = new List<Action<Node, string>>();
            this.startedAt = DateTime.Now;
            this.networkState = new NetworkState();
            networkState.Monitor(this);
        }

        public void SendSync(Message outgoing)
        {
            gateway.SendSync(outgoing.BuildMessage());
        }

        public async void Send(Message outgoing)
        {
            Task sendTask = new Task(() => {
                gateway.Send(outgoing.BuildMessage());
            });
            sendTask.Start();
            await sendTask;
        }

        public void RecvMessage(Action<Message> onMessageArrive)
        {
            Task recvTask = new Task(() =>
            {
                Message incoming = RecvMessageSync();
                onMessageArrive(incoming);
            });
            recvTask.Start();
        }

        public Message RecvMessageSync()
        {
            string messageText = gateway.RecvMessageSync();
            return new Message(messageText);
        }

        public void Start()
        {
            startedAt = DateTime.Now;
            Task autoRespond = CreateAutoRespond();
            autoRespond.Start();
            Task periodicPing = new Task(async () =>
            {
                while (true)
                {
                    Send(new Messages.Info.AliveMessage(Name, (int)(DateTime.Now - startedAt).TotalSeconds));
                    await Task.Delay(new TimeSpan(0, 0, KEEP_ALIVE_S));
                }
            });
            periodicPing.Start();
        }

        private bool ConnectivityCheck(string method, string remote, out bool supported)
        {
            try
            {
                switch (method)
                {
                    case "HTTP":
                    case "HTTPS":
                        supported = true;
                        HttpClient client = new HttpClient();
                        Task<HttpResponseMessage> getTask = client.GetAsync(remote);
                        getTask.Wait();
                        return getTask.Result.IsSuccessStatusCode;
                    case "PING":
                        supported = true;
                        Ping pingSender = new Ping();
                        PingReply reply = pingSender.Send(remote);
                        return reply.Status == IPStatus.Success;
                    default:
                        supported = false;
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR: Connectivity check failed" + ex.Message);
                supported = true;
                return false;
            }
        }

        private void HandleRequest(Message incoming)
        {
            Console.WriteLine("Incoming>>");
            Console.WriteLine(incoming);
            Console.WriteLine("<<End");

            string? @ref = incoming.ControlArgs.ContainsKey("ID") ? incoming.ID : null;
            switch (incoming.MessageType)
            {
                case NodeControl.INVALID_CONTROL:
                    Console.WriteLine("Ignoring, invalid");
                    break;
                case NodeControl.NAME:
                    Console.WriteLine("Responding with name");
                    Send(new Messages.Info.InfoMessage(Name, incoming.Source, @ref, new Dictionary<string, string>() { { "REQUEST", incoming.MessageType.ToString() } }));
                    break;
                case NodeControl.TIME:
                    Console.WriteLine("Responding with current time");
                    Send(new Messages.Info.TimeSyncMessage(Name, incoming.Source, @ref));
                    break;
                case NodeControl.VERSION:
                    Console.WriteLine("Responing with version");
                    Send(new Messages.Info.InfoMessage(Name, incoming.Source, @ref, new Dictionary<string, string>() { { "VERSION", version.ToString() } }));
                    break;
                case NodeControl.ECHO:
                    Console.WriteLine("Responding to echo");
                    Send(new Messages.Info.InfoMessage(Name, incoming.Source, @ref, incoming.ControlArgs));
                    break;
                case NodeControl.CONNECTIVITY:
                    Messages.Request.ConnectivityMessage connMessage = (Messages.Request.ConnectivityMessage)incoming;
                    bool supported;
                    bool reachable = ConnectivityCheck(connMessage.Method, connMessage.Remote, out supported);
                    if(!supported)
                    {
                        Send(new Messages.Error.NotImplementedMessage(Name, incoming, connMessage.Method));
                    } else
                    {
                        Send(new Messages.Info.ConnectivitySyncMessage(Name, incoming.Source, connMessage.Remote, connMessage.Method, reachable, @ref));
                    }
                    break;
                case NodeControl.CLOSE:
                    // We deny these
                    Send(new Messages.Info.DenialMessage(Name, incoming, "Unauthorised"));
                    break;
                case NodeControl.RELAY:
                    Messages.Request.RelayMessage relayMessage = (Messages.Request.RelayMessage)incoming;
                    Message innerMessage = relayMessage.Message;
                    if(innerMessage.MessageType == NodeControl.INVALID_CONTROL)
                    {
                        Send(new Messages.Info.DenialMessage(Name, incoming, "Malformed forwarding message"));
                        return;
                    }
                    Send(innerMessage);
                    break;
                case NodeControl.ALERT:
                    Messages.Request.AlertMessage alertMessage = (Messages.Request.AlertMessage)incoming;
                    Console.WriteLine($"Alert from {alertMessage.Source}: {alertMessage.Message ?? "[No text]"}");
                    Console.Beep();
                    // Extra beeps for higher priority
                    for(int i = 0; i < alertMessage.Priority; i+= 250)
                    {
                        Console.Beep();
                    }
                    break;
                case NodeControl.PEERS:
                    Messages.Request.PeersMessage peersMessage = (Messages.Request.PeersMessage)incoming;
                    Console.WriteLine($"Peer request from {peersMessage.Source}");
                    Messages.Info.PeerSyncMessage response = new Messages.Info.PeerSyncMessage(Name, peersMessage.Source, State.AvailableNodes(this).Select(x => x.Name).ToList(), peersMessage.ID);
                    break;
                default:
                    Console.WriteLine("Node does not handle messages of type: " + incoming.MessageType);
                    Console.WriteLine("Notifying requester");
                    Send(new Messages.Error.NotImplementedMessage(Name, incoming));
                    break;
            }
        }

        private void HandleInfo(Message incoming)
        {
            switch(incoming.MessageType)
            {
                case NodeControl.INFO:
                    Messages.Info.InfoMessage infoMessage = (Messages.Info.InfoMessage)incoming;
                    Console.WriteLine($"INFO: {infoMessage.Source}: {String.Join(",", infoMessage.ControlArgs.ToArray())}");
                    break;
                case NodeControl.TIME_SYNC:
                    Messages.Info.TimeSyncMessage timeSyncMessage = (Messages.Info.TimeSyncMessage)incoming;
                    Console.WriteLine($"Time update: {timeSyncMessage.Source}: {timeSyncMessage.Time}");
                    break;
                case NodeControl.DISCONNECTING:
                    Messages.Info.DisconnectingMessage disconnectingMessage = (Messages.Info.DisconnectingMessage)incoming;
                    Console.WriteLine($"{disconnectingMessage.Source} is disconnecting {disconnectingMessage.WillWait ?? "soon"}");
                    break;
                case NodeControl.ALIVE:
                    Messages.Info.AliveMessage aliveMessage = (Messages.Info.AliveMessage)incoming;
                    Console.WriteLine($"{aliveMessage.Source} is ALIVE, Uptime: {aliveMessage.Uptime ?? "unknown"}");
                    break;
                case NodeControl.CONNECTIVITY_SYNC:
                    Messages.Info.ConnectivitySyncMessage connSyncMessage = (Messages.Info.ConnectivitySyncMessage)incoming;
                    Console.WriteLine($"{connSyncMessage.Source} {(connSyncMessage.Reachable ? "can" : "cannot")} connect to {connSyncMessage.Remote}");
                    break;
                case NodeControl.PEER_SYNC:
                    Messages.Info.PeerSyncMessage peerSyncMessage = (Messages.Info.PeerSyncMessage)incoming;
                    Console.WriteLine($"{peerSyncMessage.Source} peers = {string.Join(", ", peerSyncMessage.Peers)}");
                    break;
                default:
                    Console.WriteLine("Not implemented info " + incoming.BuildMessage());
                    break;
            }
        }

        private void HandleService(Message incoming)
        {
            switch(incoming.MessageType)
            {
                case NodeControl.STORE:
                    storeHandler.HandleStoreMessage(incoming, this);
                    break;
                default:
                    Console.WriteLine("Node does not handle messages of type: " + incoming.MessageType);
                    Console.WriteLine("Notifying requester");
                    Send(new Messages.Error.NotImplementedMessage(Name, incoming));
                    break;

            }
        }

        private void HandleErrorNotification(Message incoming)
        {
            Console.WriteLine("Notifcation of error>>");
            Console.WriteLine(incoming.ToString());
            Console.WriteLine("<<end");
        }

        public Task CreateAutoRespond() {
            {
                Task sessionTask = new Task(() =>
                {
                    CancellationTokenSource recvCancellationSource = new CancellationTokenSource();
                    gateway.RecvMessageContinuous((string messageText) =>
                    {
                        onPreMessageArrived.ForEach(a => a(this, messageText));
                        Message incoming = new Message(messageText).Encapsulate();

                        if ((incoming.Destination != Name && incoming.Destination != "*") || incoming.Source == Name)
                        {
                            if(OutputDiscards) Console.WriteLine($"Discarded message {incoming.Source}-->{incoming.Destination}, {incoming.MessageType}, {String.Join(",", incoming.ControlArgs.ToArray())}");
                            return;
                        }


                        try
                        {
                            onMessageArrived.ForEach(a => a(this, incoming));

                            int code = (int)incoming.MessageType;
                            // Get first digit
                            int type = Int32.Parse(code.ToString()[0].ToString());
                            switch (type)
                            {
                                case 1:
                                    HandleInfo(incoming);
                                    break;
                                case 2:
                                    HandleRequest(incoming);
                                    break;
                                case 3:
                                    HandleService(incoming);
                                    break;
                                case 9:
                                    HandleErrorNotification(incoming);
                                    break;
                                default:
                                    Console.WriteLine($"Node does not handle messages of type: {type}**");
                                    Console.WriteLine("Notifying requester as not implemented");
                                    Send(new Messages.Error.NotImplementedMessage(Name, incoming));
                                    break;
                            }
                        } catch (Exception ex)
                        {
                            Console.WriteLine("Unparsable message " + incoming.ToString());
                            Console.WriteLine(ex.Message);
                            Send(new Messages.Error.UnparsableMessage(Name, incoming));
                        }


                    }, recvCancellationSource.Token);
                });
                return sessionTask;
            }
        }

        public Task CreateInteractiveSend()
        {
            return new Task(() =>
            {
                while (true)
                {
                    Console.Write(">");
                    string? input = Console.ReadLine();
                    if (input == "_CLOSE") return;
                    if (input == null) throw new Exception("NULL input from console");

                    gateway.Send(input);
                }
            });
        }
    }
}
