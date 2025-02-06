using TransactionNftScanner.Models;

namespace TransactionNftScanner
{
    public static class TransactionProcessor
    {
        public static async Task IterateDataAndDownloadImages(TransactionUTXOs transactionUTXOs, string dirPath)
        {
            List<TransactionUTXOs.InputOutput> IOs = new(transactionUTXOs.inputs.Concat(transactionUTXOs.outputs));

            foreach (var IO in IOs)
            {
                foreach (var asset in IO.amount.Where(x => x.unit != "lovelace" && x.quantity == "1"))
                {
                    var assetApiPath = $"assets/{asset.unit}";
                    var specificAsset = await HttpClientHelper.GetData<SpecificAsset>(assetApiPath).ConfigureAwait(false);
                    if (specificAsset == null) { continue; }
                    var ipfsHash = specificAsset.onchain_metadata.image.Remove(0, 7);
                    var name = specificAsset.onchain_metadata.name;
                    var path = $"{dirPath}\\{name}.png";

                    await HttpClientHelper.DownloadImage(ipfsHash, path).ConfigureAwait(false);
                }
            }
        }
    }
}
