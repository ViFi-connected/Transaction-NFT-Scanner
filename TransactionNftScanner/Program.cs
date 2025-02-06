using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace TransactionNftScanner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var app = host.Services.GetRequiredService<App>();
            await app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddHttpClient(HttpClientName.Blackfrost.ToString(), client =>
                    {
                        client.BaseAddress = new Uri("https://cardano-mainnet.blockfrost.io/api/v0/");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("project_id", "mainnetRUrPjKhpsagz4aKOCbvfTPHsF0SmwhLc");
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });

                    services.AddHttpClient(HttpClientName.Ipfs.ToString(), client =>
                    {
                        client.BaseAddress = new Uri("https://ipfs.io/");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });

                    services.AddTransient<IHttpClientHelper, HttpClientHelper>();
                    services.AddTransient<TransactionProcessor>();
                    services.AddTransient<App>();
                    services.AddLogging();
                });
    }
}