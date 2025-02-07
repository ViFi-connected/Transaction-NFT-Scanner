using Moq;
using TransactionNftScanner.Models;

namespace TransactionNftScanner.Tests
{
    [TestFixture]
    public class TransactionProcessorTests
    {
        private Mock<IHttpClientHelper> httpClientHelperMock;
        private TransactionUTXOs transactionUTXOs;
        private SpecificAsset specificAsset;
        private string dirPath;
        private TransactionProcessor transactionProcessor;

        [SetUp]
        public void Setup()
        {
            transactionUTXOs = new TransactionUTXOs
            {
                Hash = "testHash",
                Inputs =
                    [
                        new() {
                            Amount =
                            [
                                new TransactionUTXOs.Amount { Unit = "testUnit", Quantity = "1" }
                            ]
                        }
                    ],
                Outputs = []
            };

            specificAsset = new SpecificAsset
            {
                OnchainMetadata = new SpecificAsset.Metadata
                {
                    Image = "ipfs://testHash",
                    Name = "testName"
                }
            };

            httpClientHelperMock = new Mock<IHttpClientHelper>();
            httpClientHelperMock
                .Setup(x => x.GetData<SpecificAsset>(It.IsAny<HttpClientName>(), It.IsAny<string>()))
                .ReturnsAsync(specificAsset);
            httpClientHelperMock
                .Setup(x => x.DownloadImage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            dirPath = "testDir";
            transactionProcessor = new TransactionProcessor(httpClientHelperMock.Object);
        }

        [Test]
        public async Task IterateDataAndDownloadImages_DownloadsImagesSuccessfully()
        {
            // Act
            await transactionProcessor.IterateDataAndDownloadImages(transactionUTXOs, dirPath);

            // Assert
            httpClientHelperMock.Verify(x => x.GetData<SpecificAsset>(It.IsAny<HttpClientName>(), "assets/testUnit"), Times.Once);
            httpClientHelperMock.Verify(x => x.DownloadImage("testHash", $"{dirPath}\\testName.png"), Times.Once);
        }
    }
}
