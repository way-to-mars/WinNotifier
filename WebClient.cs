using Microsoft.Extensions.DependencyInjection;


namespace WinNotifier
{
    internal class WebClient(string apiKey, int timeOutSeconds = 10)
    {
        private string apiKey = apiKey;
        private int timeOutSeconds = timeOutSeconds;

        private HttpClient? ProvideHttpClient() {
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

        public bool Post(string title, string message) {

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

                Console.WriteLine(responseString);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
