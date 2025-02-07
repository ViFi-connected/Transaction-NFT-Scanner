namespace TransactionNftScanner.Models
{
    public record HttpClientSettings(HttpClientConfig Blackfrost, HttpClientConfig Ipfs);

    public record HttpClientConfig(string BaseAddress, int Timeout);
}