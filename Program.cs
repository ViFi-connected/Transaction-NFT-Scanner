using TransactionNftScanner.Models;

namespace TransactionNftScanner
{
    public class Program
    {
        private static async Task Main()
        {
            HttpClientHelper.SetClientDefaults();

            while (true)
            {
                var transactionHash = GetUserInput();
                var transactionDirPath = $"Transactions\\{transactionHash}";
                if (!Directory.Exists(transactionDirPath))
                {
                    Directory.CreateDirectory(transactionDirPath);
                }

                var transactionApiPath = $"txs/{transactionHash}/utxos";
                var transactionUTXOs = await HttpClientHelper.GetData<TransactionUTXOs>(transactionApiPath).ConfigureAwait(false);
                if (transactionUTXOs == null) { continue; }

                await TransactionProcessor.IterateDataAndDownloadImages(transactionUTXOs, transactionDirPath).ConfigureAwait(false);
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