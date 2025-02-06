namespace TransactionNftScanner.Models
{
    public class TransactionUTXOs
    {
        public required string hash { get; set; }
        public required List<InputOutput> inputs { get; set; }
        public required List<InputOutput> outputs { get; set; }

        public class InputOutput
        {
            public required List<Amount> amount { get; set; }
        }

        public class Amount
        {
            public required string unit { get; set; }
            public required string quantity { get; set; }
        }
    }
}
