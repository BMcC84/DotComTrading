namespace DotComTrading.Models
{
    //Represents the response returned from fetching a portfolio
    public class PortfolioResponse
    {
        public int PortfolioId { get; set; }
        public decimal Balance { get; set; }
        public List<PortfolioHoldings> Holdings { get; set; } = new();
    }
}
