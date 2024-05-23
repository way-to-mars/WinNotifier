using System.Diagnostics;
using System.Text.Json;

namespace WinNotifier
{
    internal class JsonLoader
    {
        public string ApiKey { get; }
        public string PushTitle { get; }
        public string PushText { get; }
        public int TimeOutSeconds { get; }

        public JsonLoader(string fileFullName)
        {
            ApiKey = "";
            PushTitle = " ";
            PushText = " ";
            TimeOutSeconds = 10 * 60;

            try
            {
                using FileStream openStream = File.OpenRead(fileFullName);
                JsonDTO? data = JsonSerializer.DeserializeAsync<JsonDTO>(openStream).Result;

                if (data != null)
                {
                    ApiKey = data.ApiKey ?? ApiKey;
                    PushTitle = data.PushTitle ?? PushTitle;
                    PushText = data.PushText ?? PushText;
                    TimeOutSeconds = data.TimeOutMinutes * 60;  // convert to seconds
                }
            }
            catch { /* use default values */ }
        }

        private string ReadFile(string fileFullName)
        {
            try
            {
                return File.ReadAllText(fileFullName);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Data class representing deserialized json data received from external file
        /// </summary>
        public class JsonDTO
        {
            public string? ApiKey { get; set; }
            public string? PushTitle { get; set; }
            public string? PushText { get; set; }
            public int TimeOutMinutes { get; set; }
        }
    }

}
