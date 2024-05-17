using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace WinNotifier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new WebClient("...");
            bool flag = false;
            int counter = 10;

            do
            {
                flag = client.Post(title: "C#", message: "OK");
                counter--;
            } while (!flag && counter > 0);

            Console.WriteLine(flag);

        }
    }
}
