using System.Net.Http.Json;
using TransactionNftScanner.Models;
using TransactionNftScanner.Shared;

namespace TransactionNftScanner.Services
{
    public class HttpClientHelper(IHttpClientFactory httpClientFactory) : IHttpClientHelper
    {
        private readonly HttpClient _blackfrostClient = httpClientFactory.CreateClient(HttpClientName.Blackfrost.ToString());
        private readonly HttpClient _ipfsClient = httpClientFactory.CreateClient(HttpClientName.Ipfs.ToString());

        public async Task<T?> GetData<T>(HttpClientName clientName, string path)
        {
            HttpClient client = clientName switch
            {
                HttpClientName.Blackfrost => _blackfrostClient,
                HttpClientName.Ipfs => _ipfsClient,
                _ => throw new ArgumentException("Invalid client name", nameof(clientName))
            };

            var response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            return default;
        }

        public async Task DownloadImage(string ipfsHash, string path)
        {
            var imageUrl = $"ipfs/{ipfsHash}";
            var response = await _ipfsClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await using var fs = new FileStream(path, FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }
    }
}
