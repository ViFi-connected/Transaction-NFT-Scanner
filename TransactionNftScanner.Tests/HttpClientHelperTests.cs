using Moq;
using Moq.Protected;
using System.Net;
using TransactionNftScanner.Models;

namespace TransactionNftScanner.Tests
{
    [TestFixture]
    public class HttpClientHelperTests
    {
        private Mock<HttpMessageHandler> handlerMock;
        private HttpClient httpClient;
        private HttpClientHelper httpClientHelper;
        private Mock<IHttpClientFactory> httpClientFactoryMock;

        [SetUp]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>();
            httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://example.com/")
            };
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            httpClientHelper = new HttpClientHelper(httpClientFactoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }

        [Test]
        public async Task GetData_ReturnsData_WhenResponseIsSuccessful()
        {
            // Arrange
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"hash\":\"test\", \"inputs\":[], \"outputs\":[]}")
                });

            // Act
            var result = await httpClientHelper.GetData<TransactionUTXOs>(HttpClientName.Blackfrost, "test/path");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.hash, Is.EqualTo("test"));
        }

        [Test]
        public async Task GetData_ReturnsNull_WhenResponseIsUnsuccessful()
        {
            // Arrange
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await httpClientHelper.GetData<TransactionUTXOs>(HttpClientName.Blackfrost, "test/path");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DownloadImage_SavesImage_WhenResponseIsSuccessful()
        {
            // Arrange
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent([1, 2, 3])
                });

            var path = "test.png";

            // Act
            await httpClientHelper.DownloadImage("testHash", path);

            // Assert
            Assert.That(File.Exists(path), Is.True);
            var fileContent = await File.ReadAllBytesAsync(path);
            Assert.That(new byte[] { 1, 2, 3 }, Is.EqualTo(fileContent));

            // Cleanup
            File.Delete(path);
        }
    }
}
