using TransactionNftScanner.Models;

namespace TransactionNftScanner
{
    public class App(IHttpClientHelper httpClientHelper, TransactionProcessor transactionProcessor)
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly TransactionProcessor _transactionProcessor = transactionProcessor;

        public async Task Run()
        {
            while (true)
            {
                var transactionHash = GetUserInput();
                var transactionDirPath = $"Transactions\\{transactionHash}";
                if (!Directory.Exists(transactionDirPath))
                {
                    Directory.CreateDirectory(transactionDirPath);
                }

                var transactionApiPath = $"txs/{transactionHash}/utxos";
                var transactionUTXOs = await _httpClientHelper.GetData<TransactionUTXOs>(HttpClientName.Blackfrost, transactionApiPath).ConfigureAwait(false);
                if (transactionUTXOs == null) { continue; }

                await _transactionProcessor.IterateDataAndDownloadImages(transactionUTXOs, transactionDirPath).ConfigureAwait(false);
            }
        }

        private static string GetUserInput()
        {
            string? input = null;

            while (input == null)
            {
                Console.WriteLine("Enter transaction hash:");
                input = Console.ReadLine();
                Console.WriteLine();
            }
            return input;
        }
    }
}
