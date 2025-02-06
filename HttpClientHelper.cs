using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TransactionNftScanner
{
    public static class HttpClientHelper
    {
        static readonly HttpClient client = new();
        const string API_BASE_URL = "https://cardano-mainnet.blockfrost.io/api/v0/";
        const string IPFS_BASE_URL = "https://cloudflare-ipfs.com/ipfs/";

        public static void SetClientDefaults()
        {
            client.BaseAddress = new Uri(API_BASE_URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("project_id", "mainnetRUrPjKhpsagz4aKOCbvfTPHsF0SmwhLc");
        }

        public static async Task<T?> GetData<T>(string path)
        {
            try
            {
                var response = await client.GetAsync(path).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get data from: {path}. Error: {ex.Message}");
                return default;
            }
        }

        public static async Task DownloadImage(string ipfsHash, string path)
        {
            try
            {
                var url = new Uri(IPFS_BASE_URL + ipfsHash);
                var response = await client.GetAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                await File.WriteAllBytesAsync(path, imageBytes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image download failed! Error: {ex.Message}");
            }
        }
    }
}
