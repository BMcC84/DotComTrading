namespace DotComTrading.Models
{
    //Represents the structure for a holding for display
    public class PortfolioHoldings
    {
        public int WebsiteId {  get; set; }
        public Website? Website { get; set; } = null;
        public decimal NoShares {  get; set; }
    }
}
