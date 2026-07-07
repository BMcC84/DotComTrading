namespace DotComTrading.Models
{
    //Represents a transaction made by a user for history
    public class Trade
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; } = null!;
        public int WebsiteId { get; set; }
        public Website Website { get; set; } = null!;
        public decimal NoShares { get; set; }
        public decimal PricePerShare { get; set; }
        public DateTime TradeTime { get; set; }
        public string TradeType { get; set; } = null!;
    }
}
