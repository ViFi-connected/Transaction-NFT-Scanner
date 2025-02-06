namespace TransactionNftScanner.Models
{
    public class SpecificAsset
    {
        public required OnchainMetadata onchain_metadata { get; set; }

        public class OnchainMetadata
        {
            public required string image { get; set; }
            public required string name { get; set; }
        }
    }
}
