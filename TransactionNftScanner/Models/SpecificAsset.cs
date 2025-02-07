using System.Text.Json.Serialization;

namespace TransactionNftScanner.Models
{
    public class SpecificAsset
    {
        [JsonPropertyName("onchain_metadata")]
        public required Metadata OnchainMetadata { get; set; }

        public class Metadata
        {
            public required string Image { get; set; }
            public required string Name { get; set; }
        }
    }
}
