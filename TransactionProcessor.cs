using TransactionNftScanner.Models;

namespace TransactionNftScanner
{
    public static class TransactionProcessor
    {
        public static async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = new(transactionUTXOs.inputs.Concat(transactionUTXOs.outputs));
            HashSet<string> processedAssets = new();
            List<Task> downloadTasks = new();

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.amount.Where(x => x.unit != "lovelace" && x.quantity == "1"))
                {
                    if (processedAssets.Contains(asset.unit))
                    {
                        continue;
                    }

                    var assetApiPath = $"assets/{asset.unit}";
                    var specificAssetTask = HttpClientHelper.GetData<SpecificAsset>(assetApiPath);
                    processedAssets.Add(asset.unit);

                    downloadTasks.Add(ProcessAsset(specificAssetTask, dirPath));
                }
            }

            await Task.WhenAll(downloadTasks).ConfigureAwait(false);
        }

        private static async Task ProcessAsset(Task<SpecificAsset?> specificAssetTask, string dirPath)
        {
            var specificAsset = await specificAssetTask.ConfigureAwait(false);
            if (specificAsset == null) { return; }

            var ipfsHash = specificAsset.onchain_metadata.image.Remove(0, 7);
            var name = specificAsset.onchain_metadata.name;
            var path = $"{dirPath}\\{name}.png";

            await HttpClientHelper.DownloadImage(ipfsHash, path).ConfigureAwait(false);
            Console.WriteLine($"Successfully downloaded image for asset: {name} to path: {path}\n");
        }
    }
}
