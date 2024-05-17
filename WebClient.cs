using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;


namespace WinNotifier
{
    internal class WebClient(string apiKey, TimeSpan timeOut)
    {
        private string apiKey = apiKey;
        private int timeOutSeconds = timeOut.Seconds;

        private HttpClient? ProvideHttpClient()
        {
            // определяем коллекцию сервисов
            var services = new ServiceCollection();
            // добавляем сервисы, связанные с HttpClient, в том числе IHttpClientFactory
            services.AddHttpClient();
            // создаем провайдер сервисов
            var serviceProvider = services.BuildServiceProvider();
            // получаем сервис IHttpClientFactory
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            // создаем объект HttpClient
            return httpClientFactory?.CreateClient();
        }

        public bool Post(string title, string message)
        {
            const int delay = 500;
            int totalTime = 0;

            do
            {
                if (TryPost(title, message)) return true;

                Task.Delay(delay).Wait();
                totalTime += delay;
            }
            while (totalTime <= timeOutSeconds);

            return TryPost(title, message);
        }

        private bool TryPost(string title, string message)
        {
            HttpClient? httpClient = ProvideHttpClient();
            if (httpClient == null) return false;
            try
            {
                Dictionary<string, string> values = new()
                {
                    { "ApiKey", apiKey },
                    { "PushTitle", title },
                    { "PushText", message }
                };

                HttpContent content = new FormUrlEncodedContent(values);

                var response = httpClient.PostAsync("https://www.notifymydevice.com/push", content).Result;

                var responseString = response.Content.ReadAsStringAsync().Result;

                Debug.WriteLine(responseString);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
