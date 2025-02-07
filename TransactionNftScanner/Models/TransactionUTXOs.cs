namespace TransactionNftScanner.Models
{
    public class TransactionUTXOs
    {
        public required string Hash { get; set; }
        public required List<InputOutput> Inputs { get; set; }
        public required List<InputOutput> Outputs { get; set; }

        public class InputOutput
        {
            public required List<Amount> Amount { get; set; }
        }

        public class Amount
        {
            public required string Unit { get; set; }
            public required string Quantity { get; set; }
        }
    }
}
