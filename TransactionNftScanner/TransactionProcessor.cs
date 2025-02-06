using TransactionNftScanner.Models;

namespace TransactionNftScanner
{
    public class TransactionProcessor(IHttpClientHelper httpClientHelper)
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;

        public async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = new(transactionUTXOs.inputs.Concat(transactionUTXOs.outputs));
            HashSet<string> processedAssets = [];
            List<Task> downloadTasks = [];

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.amount.Where(x => x.unit != "lovelace" && x.quantity == "1"))
                {
                    if (processedAssets.Contains(asset.unit))
                    {
                        continue;
                    }

                    var assetApiPath = $"assets/{asset.unit}";
                    var specificAssetTask = _httpClientHelper.GetData<SpecificAsset>(HttpClientName.Blackfrost, assetApiPath);
                    processedAssets.Add(asset.unit);

                    downloadTasks.Add(ProcessAsset(specificAssetTask, dirPath));
                }
            }

            await Task.WhenAll(downloadTasks).ConfigureAwait(false);
        }

        private async Task ProcessAsset(Task<SpecificAsset?> specificAssetTask, string dirPath)
        {
            var specificAsset = await specificAssetTask.ConfigureAwait(false);
            if (specificAsset == null) { return; }

            var ipfsHash = specificAsset.onchain_metadata.image.Remove(0, 7);
            var name = specificAsset.onchain_metadata.name;
            var path = $"{dirPath}\\{name}.png";

            await _httpClientHelper.DownloadImage(ipfsHash, path).ConfigureAwait(false);
            Console.WriteLine($"Successfully downloaded image for asset: {name} to path: {path}\n");
        }
    }
}
