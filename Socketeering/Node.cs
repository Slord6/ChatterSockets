using Socketeering.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering
{
    public class Node
    {
        private readonly static int version = 1;
        private readonly Gateway gateway;

        private bool isActive;
        public bool IsActive { get => isActive; }

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

        public Task CreateAutoRespond() {
            {
                Task sessionTask = new Task(() =>
                {
                    CancellationTokenSource recvCancellationSource = new CancellationTokenSource();
                    gateway.RecvMessageContinuous((string messageText) =>
                    {
                        Message incoming = new Message(messageText);
                        Console.WriteLine("Incoming>>");
                        Console.WriteLine(incoming);
                        Console.WriteLine("<<End");

                        if (incoming.Destination != Name && incoming.Destination != "*")
                        {
                            Console.WriteLine("Discarded");
                            return;
                        }

                        switch (incoming.MessageType)
                        {
                            case NodeControl.INVALID_CONTROL:
                                Console.WriteLine("Ignoring, invalid");
                                break;
                            case NodeControl.NAME:
                                Console.WriteLine("Responding with name");
                                Send(new Messages.Info.InfoMessage(Name, incoming.Source, new string[] { $"REQUEST:{incoming.MessageType}" }));
                                break;
                            case NodeControl.TIME:
                                Console.WriteLine("Responding with current time");
                                Send(new Messages.Info.TimeSyncMessage(Name, incoming.Source));
                                break;
                            case NodeControl.CLOSE:
                                Console.WriteLine("Ending autorespond");
                                recvCancellationSource.Cancel();
                                return;
                            case NodeControl.VERSION:
                                Console.WriteLine("Responing with version");
                                Send(new Messages.Info.InfoMessage(Name, incoming.Source, new string[] { $"VERISON:{version}" }));
                                break;
                            default:
                                Console.WriteLine("Node does not handle messages of type: " + incoming.MessageType);
                                Console.WriteLine("Notifying requester");
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
