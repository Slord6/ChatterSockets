using Socketeering;
using Socketeering.Messages;
using System.Net;

public class Program
{
    private static readonly int PORT = 4327;
    private static readonly IPAddress MULTICASTADDRESS = IPAddress.Parse("238.7.6.5");

    public static void Main(string[] args)
    {
        Gateway gateway = new Gateway(PORT, MULTICASTADDRESS);
        Node node = new Node(gateway);
        Console.WriteLine(node.Name + " active");

        Task autoRespond = node.CreateAutoRespond();
        autoRespond.Start();
        
        Task interactiveSend = node.CreateInteractiveSend();
        interactiveSend.Start();
        interactiveSend.Wait();
    }

}