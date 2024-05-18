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
                ShowToast();
                Process.Start(new ProcessStartInfo("faq.pdf") { UseShellExecute = true });
                return;
            }

            var json = new JsonLoader(args[0]);

            var client = new WebClient(
                apiKey: json.ApiKey,
                timeOut: TimeSpan.FromSeconds(json.TimeOutSeconds)
                );

            string message = $"[{DateTime.Now}] {json.PushText}";

            bool result = client.Post(title: json.PushTitle, message: message);
        }

        static void ShowToast()
        {
            var notifier = ToastNotificationManager.CreateToastNotifier("WinNotifier");
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes[0].AppendChild(toastXml.CreateTextNode("Требуется файл настроек в качестве параметра (формат json)"));
            var toast = new ToastNotification(toastXml)
            {
                ExpirationTime = DateTime.Now.AddSeconds(5)
            };
            notifier.Show(toast);
        }
    }
}
