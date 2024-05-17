using System.Diagnostics;

namespace WinNotifier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new WebClient("...", TimeSpan.FromMinutes(10));

            string message;
            if (args.Length > 0) message = String.Join(separator: "; ", args);
            else
                 message = "<No message>";
            
            client.Post(title: "Honor", message: message);

            Debug.WriteLine("finish");
        }
    }
}
