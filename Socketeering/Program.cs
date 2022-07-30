using Socketeering;
using System.Net;

public class Program
{
    private static readonly int PORT = 4327;
    private static readonly IPAddress MULTICASTADDRESS = IPAddress.Parse("238.7.6.5");

    public static void Main(string[] args)
    {
        Console.WriteLine($"Hello, I am {NameGenerator.GetName()}");
        Gateway gateway = new Gateway(PORT, MULTICASTADDRESS);

        Task sendTask = InteractiveSend(gateway);
        sendTask.Start();

        gateway.RecvMessageContinuous((string message) =>
        {
            Console.WriteLine("Incoming>>");
            Console.WriteLine(message);
            Console.WriteLine("<<End");
        });
        sendTask.Wait();
    }

    public static Task InteractiveSend(Gateway gateway)
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