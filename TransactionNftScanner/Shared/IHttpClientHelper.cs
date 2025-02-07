using TransactionNftScanner.Models;

namespace TransactionNftScanner.Shared
{
    public interface IHttpClientHelper
    {
        Task<T?> GetData<T>(HttpClientName clientName, string path);
        Task DownloadImage(string ipfsHash, string path);
    }
}
