using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolSMS
{
    class HttpProcessor
    {
        private static readonly object LockObj = new object();
        private static HttpClient client = null;
        public HttpProcessor()
        {
            GetInstance();
        }

        public static void GetInstance()
        {
            if (client == null)
            {
                lock (LockObj)
                {
                    if (client == null)
                    {
                        client = new HttpClient();
                    }
                }
            }
        }

        public async Task<string> SendAsync(string url, Dictionary<string, string> header, Dictionary<string, string> postData)
        {
            if (header != null)
                foreach (KeyValuePair<string, string> kvp in header)
                {
                    client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            HttpContent content = postData != null ? new FormUrlEncodedContent(postData) : null;
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
