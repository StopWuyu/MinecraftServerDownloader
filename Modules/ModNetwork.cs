using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerDownloader.Modules
{
    internal static class ModNetwork
    {
        public static async Task<string> SendGetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpResponse = await client.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode(); // 确保响应成功

                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                return responseContent;
            }
        }
    }
}
