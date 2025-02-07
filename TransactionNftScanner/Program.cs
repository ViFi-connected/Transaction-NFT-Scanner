using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using TransactionNftScanner.Logic;
using TransactionNftScanner.Models;
using TransactionNftScanner.Services;
using TransactionNftScanner.Shared;

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
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var httpClientSettings = context.Configuration.GetSection("HttpClientSettings").Get<HttpClientSettings>();

                    if (httpClientSettings?.Blackfrost != null)
                    {
                        services.AddHttpClient(HttpClientName.Blackfrost.ToString(), client =>
                        {
                            client.BaseAddress = new Uri(httpClientSettings.Blackfrost.BaseAddress);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("project_id", httpClientSettings.Blackfrost.ProjectId);
                            client.Timeout = TimeSpan.FromSeconds(httpClientSettings.Blackfrost.Timeout);
                        });
                    }

                    if (httpClientSettings?.Ipfs != null)
                    {
                        services.AddHttpClient(HttpClientName.Ipfs.ToString(), client =>
                        {
                            client.BaseAddress = new Uri(httpClientSettings.Ipfs.BaseAddress);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.Timeout = TimeSpan.FromSeconds(httpClientSettings.Ipfs.Timeout);
                        });
                    }

                    services.AddTransient<IHttpClientHelper, HttpClientHelper>();
                    services.AddTransient<TransactionProcessor>();
                    services.AddTransient<App>();
                    services.AddLogging();
                });
    }
}