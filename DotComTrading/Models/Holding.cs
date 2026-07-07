namespace DotComTrading.Models
{
    //Represents one website holding record for a portfolio
    public class Holding
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; } = null!;
        public int WebsiteId { get; set; }
        public Website Website { get; set; } = null!;
        public decimal NoShares { get; set; }
    }
}
