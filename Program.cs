
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
                var transactionUTXOs = await GetData<TransactionUTXOs>(transactionApiPath).ConfigureAwait(false);
                if (transactionUTXOs == null) { continue; }

                await IterateDataAndDownloadImages(transactionUTXOs, transactionDir.FullName).ConfigureAwait(false);
            }
        }

        private static async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = new(transactionUTXOs.inputs.Concat(transactionUTXOs.outputs));

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.amount.Where(x => x.unit != "lovelace" && x.quantity == "1"))
                {
                    var assetApiPath = $"assets/{asset.unit}";
                    var specificAsset = await GetData<SpecificAsset>(assetApiPath).ConfigureAwait(false);
                    if (specificAsset == null) { continue; }
                    var ipfsHash = specificAsset.onchain_metadata.image.Remove(0, 7);
                    var name = specificAsset.onchain_metadata.name;
                    var path = $"{dirPath}\\{name}.png";

                    await DownloadImage(ipfsHash, path).ConfigureAwait(false);
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

        private static async Task<T?> GetData<T>(string path)
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

        private static async Task DownloadImage(string ipfsHash, string path)
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