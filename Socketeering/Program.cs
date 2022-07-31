using Socketeering;
using Socketeering.Messages;
using System.Net;

public class Program
{
    private static readonly int PORT = 31337;
    private static readonly IPAddress MULTICASTADDRESS = IPAddress.Parse("224.0.0.1");
    private static readonly int TTL = 1;

    public static void Main(string[] args)
    {
        Gateway gateway = new Gateway(PORT, MULTICASTADDRESS, TTL);
        Node node = new Node(gateway);
        node.OutputDiscards = false;
        NetworkState netState = new NetworkState();
        netState.Monitor(node);

        Task updateDisplay = new Task(async () =>
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine($"({node.Name}) Node timeout = {RemoteNode.CONNECTED_TIMEOUT_S}s");

                Console.WriteLine("\nActive nodes:");
                Console.WriteLine($"{String.Join("\n", netState.AvailableNodes(node))}");

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

}