using Socketeering;
using System.Net;

public class Program
{
    private static readonly int PORT = 31337;
    private static readonly IPAddress MULTICASTADDRESS = IPAddress.Parse("224.0.0.1");
    private static readonly int TTL = 255;

    public static void Main(string[] args)
    {
        IPAddress sendAddress = QueryUserForSendAddress();
        IPAddress multicastAddress = QueryUserForMulticastAddress();

        Gateway gateway = new Gateway(PORT, multicastAddress, TTL, sendAddress);
        Node node = new Node(gateway);
        node.OutputDiscards = false;
        NetworkState netState = new NetworkState();
        netState.Monitor(node);

        Task updateDisplay = new Task(async () =>
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine($"This Node: ({node.Name})");
                Console.WriteLine($"Interface: {sendAddress}");
                Console.WriteLine($"Multicast address: {MULTICASTADDRESS}");
                Console.WriteLine($"Port: {PORT}");
                Console.WriteLine($"Node timeout: {RemoteNode.CONNECTED_TIMEOUT_S}s");
                Console.WriteLine("________________");

                Console.WriteLine("\nActive nodes:");
                Console.WriteLine($"{String.Join("\n====\n", netState.AvailableNodes(node))}");

                Console.WriteLine("\n");

                await Task.Delay(new TimeSpan(0, 0, 5));
            }
        });
        updateDisplay.Start();

        Task removeInactive = new Task(async () =>
        {
            while(true)
            {
                await Task.Delay(new TimeSpan(0, 0, RemoteNode.CONNECTED_TIMEOUT_S));
                netState.DiscardInactiveNodes();
            }
        });
        removeInactive.Start();

        node.Start();
        Console.WriteLine(node.Name + " started");
        
        Task interactiveSend = node.CreateInteractiveSend();
        interactiveSend.Start();
        interactiveSend.Wait();
    }

    private static IPAddress QueryUserForSendAddress()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress[] availableIps = hostEntry.AddressList;//.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToArray();

        IPAddress? sendAddress = null;
        while(sendAddress == null)
        {
            Console.WriteLine("Which endpoint for sending? Choose number:");
            for(int i = 0; i < availableIps.Length; i++)
            {
                IPAddress addressChoice = availableIps[i];
                Console.WriteLine($"{i} - {addressChoice.ToString()}");
            }
            string? input = Console.ReadLine();
            int indexChoice;
            if(int.TryParse(input, out indexChoice) && indexChoice >= 0 && indexChoice < availableIps.Length)
            {
                sendAddress = availableIps[indexChoice];
            } else
            {
                Console.WriteLine("Invalid selection");
                sendAddress = null;
                continue;
            }
        }

        return sendAddress;
    }

    private static IPAddress QueryUserForMulticastAddress()
    {
        IPAddress? multicastAddress = null;
        while (multicastAddress == null)
        {
            Console.WriteLine("Please enter multicast address (leave blank for default):");
            string multicastBase = "239.255.";
            Console.Write(multicastBase);
            string? input = Console.ReadLine();

            if (String.IsNullOrEmpty(input)) return IPAddress.Parse(multicastBase + "10.10");

            if (IPAddress.TryParse(multicastBase + input, out multicastAddress))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid IP");
                multicastAddress = null;
                continue;
            }
        }
        return multicastAddress;
    }

}