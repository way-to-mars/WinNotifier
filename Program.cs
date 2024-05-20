using System.Diagnostics;
using Windows.UI.Notifications;

namespace WinNotifier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowToast("Требуется файл настроек в качестве параметра (формат json)");
                Process.Start(new ProcessStartInfo("faq.pdf") { UseShellExecute = true });
                return;
            }

            var json = new JsonLoader(args[0].Trim('"'));

            var client = new WebClient(
                apiKey: json.ApiKey,
                timeOut: TimeSpan.FromSeconds(json.TimeOutSeconds)
                );

            string message = $"[{DateTime.Now}] {json.PushText}";

            var result = client.Post(title: json.PushTitle, message: message);

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
    }
}
