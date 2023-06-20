using System.Net.Http;
using System.Threading.Tasks;

namespace MinecraftServerDownloader.Modules
{
    internal static class ModNetwork
    {
        public static async Task<string> SendGetRequest(string url)
        {
            using (HttpResponseMessage httpResponse = await HttpClientSingleton.Instance.GetAsync(url))
            {
                httpResponse.EnsureSuccessStatusCode(); // 确保响应成功

                return await httpResponse.Content.ReadAsStringAsync();
            }
        }
    }
}
