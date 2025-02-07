using TransactionNftScanner.Models;
using TransactionNftScanner.Shared;

namespace TransactionNftScanner.Logic
{
    public class TransactionProcessor(IHttpClientHelper httpClientHelper)
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;

        public async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = new(transactionUTXOs.Inputs.Concat(transactionUTXOs.Outputs));
            HashSet<string> processedAssets = [];
            List<Task> downloadTasks = [];

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.Amount.Where(x => x.Unit != "lovelace" && x.Quantity == "1"))
                {
                    if (processedAssets.Contains(asset.Unit))
                    {
                        continue;
                    }

                    var assetApiPath = $"assets/{asset.Unit}";
                    var specificAssetTask = _httpClientHelper.GetData<SpecificAsset>(HttpClientName.Blackfrost, assetApiPath);
                    processedAssets.Add(asset.Unit);

                    downloadTasks.Add(ProcessAsset(specificAssetTask, dirPath));
                }
            }

            await Task.WhenAll(downloadTasks).ConfigureAwait(false);
        }

        private async Task ProcessAsset(Task<SpecificAsset?> specificAssetTask, string dirPath)
        {
            var specificAsset = await specificAssetTask.ConfigureAwait(false);
            if (specificAsset == null) { return; }

            var ipfsHash = specificAsset.OnchainMetadata.Image.Remove(0, 7);
            var name = specificAsset.OnchainMetadata.Name;
            var path = $"{dirPath}\\{name}.png";

            await _httpClientHelper.DownloadImage(ipfsHash, path).ConfigureAwait(false);
            Console.WriteLine($"Successfully downloaded image for asset: {name} to path: {path}\n");
        }
    }
}
