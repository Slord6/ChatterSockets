using Socketeering.Messages;
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

        public Node(Gateway gateway)
        {
            this.gateway = gateway;
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

        public Task CreateInteractiveSession()
        {
            Task sessionTask = new Task(() =>
            {
                Task sendTask = CreateInteractiveSend();
                sendTask.Start();

                gateway.RecvMessageContinuous((string message) =>
                {
                    Console.WriteLine("Incoming>>");
                    Console.WriteLine(message);
                    Console.WriteLine("<<End");
                });
                sendTask.Wait();
            });
            return sessionTask;
        }

        public void Start()
        {
            Task autoRespond = CreateAutoRespond();
            autoRespond.Start();
            Task periodicPing = new Task(async () =>
            {
                while (true)
                {
                    AlivePing();
                    await Task.Delay(new TimeSpan(0, 0, 30));
                }
            });
            periodicPing.Start();
        }

        private void AlivePing()
        {
            Send(new Messages.Info.AliveMessage(Name, null));
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

            string? @ref = incoming.ControlArgs.ContainsKey("ID") ? incoming.ControlArgs["ID"] : null;
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
                    bool supported;
                    string method = incoming.ControlArgs["METHOD"];
                    string remote = incoming.ControlArgs["REMOTE"];
                    bool reachable = ConnectivityCheck(method, remote, out supported);
                    if(!supported)
                    {
                        Send(new Messages.Error.NotImplementedMessage(Name, incoming, method));
                    } else
                    {
                        Send(new Messages.Info.ConnectivitySyncMessage(Name, incoming.Source, remote, method, reachable, @ref));
                    }
                    break;
                case NodeControl.CLOSE:
                    // We deny these
                    Send(new Messages.Info.DenialMessage(Name, incoming, "Unauthorised"));
                    break;
                case NodeControl.RELAY:
                    Message innerMessage = new Message(incoming.ControlArgs["MESSAGE"]);
                    if(innerMessage.MessageType == NodeControl.INVALID_CONTROL)
                    {
                        Send(new Messages.Info.DenialMessage(Name, incoming, "Malformed forwarding message"));
                        return;
                    }
                    Send(innerMessage);
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
                    Console.WriteLine($"INFO: {incoming.Source}: {String.Join(",", incoming.ControlArgs.ToArray())}");
                    break;
                case NodeControl.TIME_SYNC:
                    Console.WriteLine($"Time update: {incoming.Source}: {incoming.ControlArgs["TIME"]}");
                    break;
                case NodeControl.DISCONNECTING:
                    string? waitTime;
                    if (!incoming.ControlArgs.TryGetValue("WILLWAIT", out waitTime)) waitTime = "soon";
                    Console.WriteLine($"{incoming.Source} is disconnecting {waitTime}");
                    break;
                case NodeControl.ALIVE:
                    Console.WriteLine($"{incoming.Source} - PING!");
                    break;
                default:
                    Console.WriteLine("Not implemented info " + incoming.BuildMessage());
                    break;
            }
        }

        public void HandleErrorNotification(Message incoming)
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
                        Message incoming = new Message(messageText);

                        if ((incoming.Destination != Name && incoming.Destination != "*") || incoming.Source == Name)
                        {
                            Console.WriteLine($"Discarded message {incoming.Source}-->{incoming.Destination}, {incoming.MessageType}, {String.Join(",", incoming.ControlArgs.ToArray())}");
                            return;
                        }

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
                            case 9:
                                HandleErrorNotification(incoming);
                                break;
                            default:
                                Console.WriteLine($"Node does not handle messages of type: {type}**");
                                Console.WriteLine("Notifying requester as not implemented");
                                Send(new Messages.Error.NotImplementedMessage(Name, incoming));
                                break;
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
