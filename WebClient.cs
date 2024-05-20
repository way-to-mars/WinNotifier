using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net;


namespace WinNotifier
{
    internal class WebClient(string apiKey, TimeSpan timeOut)
    {
        private readonly string apiKey = apiKey;
        private readonly long timeOutMilliseconds = (long)timeOut.TotalMilliseconds;
        private readonly string postURL = "https://www.notifymydevice.com/push";

        public enum Status {
            OK,
            NO_CONNECTION,
            WRONG_API_KEY,
        }

        private static HttpClient? ProvideHttpClient()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            return httpClientFactory?.CreateClient();
        }

        /// <summary>
        /// Отправляет уведомление через https://www.notifymydevice.com/push с некоторым query
        /// В случае сбоя отправки выполняет её повтор каждые 0.5 секунд в течении timeOut времени
        /// </summary>
        public Status Post(string title, string message)
        {
            const int delay = 500;  // milliseconds
            long totalTime = 0;

            HttpClient? httpClient = ProvideHttpClient();

            do
            {
                httpClient ??= ProvideHttpClient();
                if (TryPost(httpClient, title, message, out var statusCode)) return Status.OK;

                if (statusCode == HttpStatusCode.InternalServerError) return Status.WRONG_API_KEY;

                Task.Delay(delay).Wait();
                totalTime += delay;
            }
            while (totalTime <= timeOutMilliseconds);

            return TryPost(httpClient, title, message, out _) ? Status.OK : Status.NO_CONNECTION;
        }

        private bool TryPost(HttpClient? httpClient, string title, string message, out HttpStatusCode? code)
        {
            code = null;
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

                var response = httpClient.PostAsync(postURL, content).Result;

                var responseString = response.Content.ReadAsStringAsync().Result;

                code = response?.StatusCode;

                return response?.IsSuccessStatusCode ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}
