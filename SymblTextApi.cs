using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SymblAISharp.Async.TextApi;

namespace Symbl.Insights.Audio
{
    public class SymblTextApi
    {
        public static SummaryRoot GetSummary(string url,
            string token)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = $"Bearer {token}";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<SummaryRoot>(result);
                }
            }

            return null;
        }

        public static async Task<TextResponse> Post(string url,
            string token,
            TextPostRequest textRequest)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Headers["accept"] = "application/json";
            httpRequest.Headers["Authorization"] = $"Bearer {token}";
            httpRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(
                    JsonConvert.SerializeObject(textRequest)
                    .Replace("\"duration\":null,", "")
                    .Replace(",\"trackers\":[]", "")
                    .Replace("\"webhookUrl\":\"\",", "")
                    .Replace("\"confidenceThreshold\":0.0,", "")
                    .Replace("True", "true")
                    .Replace("False", "false"));
            }

            var httpResponse = await httpRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                if (((HttpWebResponse)httpResponse).StatusCode == HttpStatusCode.Created)
                    return JsonConvert.DeserializeObject<TextResponse>(
                        streamReader.ReadToEnd());
            }

            return null;
        }
    }
}
