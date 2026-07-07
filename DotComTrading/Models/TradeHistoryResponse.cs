namespace DotComTrading.Models
{
	//Represents a formatted trade record returned for displaying history
    public class TradeHistoryResponse
    {
		public int Id { get; set; }
		public int PortfolioId { get; set; }
		public int WebsiteId { get; set; }
		public string WebsiteName { get; set; } = "";
		public string Domain { get; set; } = "";
		public decimal NoShares { get; set; }
		public decimal PricePerShare { get; set; }
		public decimal TotalValue { get; set; }
		public DateTime TradeTime { get; set; }
		public string TradeType { get; set; } = "";
	}
}
