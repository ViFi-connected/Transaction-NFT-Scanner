
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TransactionNftScanner
{
    public class Program
    {
        static readonly HttpClient client = new();
        const string API_BASE_URL = "https://cardano-mainnet.blockfrost.io/api/v0/";
        const string IPFS_BASE_URL = "https://cloudflare-ipfs.com/ipfs/";

        private static async Task Main()
        {
            SetClientDefaults();

            while (true)
            {
                var transactionHash = GetUserInput("Enter transaction hash:");
                var transactionDir = Directory.CreateDirectory($"Transactions\\{transactionHash}");

                var transactionApiPath = $"txs/{transactionHash}/utxos";
                var transactionUTXOs = await GetData<TransactionUTXOs>(transactionApiPath);
                if (transactionUTXOs == null) { continue; }

                await IterateDataAndDownloadImages(transactionUTXOs, transactionDir.FullName);
            }
        }

        private static async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = [.. transactionUTXOs.inputs, .. transactionUTXOs.outputs];

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.amount.Where(x => x.unit != "lovelace" && x.quantity == "1"))
                {
                    var assetApiPath = $"assets/{asset.unit}";
                    var specificAsset = await GetData<SpecificAsset>(assetApiPath);
                    if (specificAsset == null) { continue; }
                    var ipfsHash = specificAsset.onchain_metadata.image.Remove(0, 7);
                    var name = specificAsset.onchain_metadata.name;
                    var path = $"{dirPath}\\{name}.png";

                    await DownloadImage(ipfsHash, path);
                }
            }
        }

        private static void SetClientDefaults()
        {
            client.BaseAddress = new Uri(API_BASE_URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("project_id", "mainnetRUrPjKhpsagz4aKOCbvfTPHsF0SmwhLc");
        }

        private static string GetUserInput(string prompt)
        {
            string? input = null;

            while (input == null)
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();
                Console.WriteLine();
            }
            return input;
        }

        private static async Task<T> GetData<T>(string path)
        {
            var data = default(T);
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadFromJsonAsync<T>();
            }
            if (data == null)
            {
                Console.WriteLine("Failed to get data from: " + path);
            }
            return data;
        }

        private async static Task DownloadImage(string ipfsHash, string path)
        {
            var url = new Uri(IPFS_BASE_URL + ipfsHash);
            var response = await client.GetAsync(url);
            byte[]? imageBytes = null;

            if (response.IsSuccessStatusCode)
            {
                imageBytes = await response.Content.ReadAsByteArrayAsync();
            }
            if (imageBytes == null)
            {
                Console.WriteLine("Image download failed!");
                return;
            }
            await File.WriteAllBytesAsync(path, imageBytes);
        }
    }

    public class TransactionUTXOs
    {
        public required string hash { get; set; }
        public required List<InputOutput> inputs { get; set; }
        public required List<InputOutput> outputs { get; set; }

        public class InputOutput
        {
            public required List<Amount> amount { get; set; }
        }

        public class Amount
        {
            public required string unit { get; set; }
            public required string quantity { get; set; }
        }
    }

    public class SpecificAsset
    {
        public required OnchainMetadata onchain_metadata { get; set; }

        public class OnchainMetadata
        {
            public required string image { get; set; }
            public required string name { get; set; }
        }
    }
}