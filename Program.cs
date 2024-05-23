using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Windows.UI.Notifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinNotifier
{
    internal class Program
    {

        static string logFile = "log.txt";

        static void Main(string[] args)
        {
            LogFile("start");
            if (args.Length == 0)
            {
                ShowToast("Требуется файл настроек в качестве параметра (формат json)");
                Process.Start(new ProcessStartInfo("faq.pdf") { UseShellExecute = true });
                LogFile("No args. Terminate.");
                return;
            }

            var json = new JsonLoader(args[0].Trim('"'));

            if (json.ApiKey.Length == 0) {
                LogFile("No API-key provided. Terminate.");
                return;
            }

            var client = new WebClient(
                apiKey: json.ApiKey,
                timeOut: TimeSpan.FromSeconds(json.TimeOutSeconds)
                );

            LogFile($"WebClient: {client}");

            string message = $"[{DateTime.Now}] {json.PushText}";

            LogFile($"message: {message}");

            var result = client.Post(title: json.PushTitle, message: message);

            LogFile($"Post result: {result}");

            if (result == WebClient.Status.WRONG_API_KEY)
                ShowToast("Ошибка API-ключа");
        }

        static void ShowToast(string message)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier("WinNotifier");
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes[0].AppendChild(toastXml.CreateTextNode(message));
            var toast = new ToastNotification(toastXml)
            {
                ExpirationTime = DateTime.Now.AddSeconds(5)
            };
            notifier.Show(toast);
            Task.Delay(1000).Wait();
        }

        static void LogFile(string message) {
            string time = DateTime.Now.ToString("hh:mm:ss.ffffff");
            string threadName = $"id:{Environment.CurrentManagedThreadId}-'{Thread.CurrentThread.Name ?? ""}'";

            string data = $"{time} [{threadName}] {message}\n";

            try
            {
                using StreamWriter writer = new(logFile, true, Encoding.UTF8);
                writer.Write(data);
            }
            catch
            {
                // just exit
            }

        }
    }
}
