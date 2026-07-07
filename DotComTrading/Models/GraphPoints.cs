namespace DotComTrading.Models
{
	//Represents one point on the price movement line chart
	public class GraphPoints
	{
		public int Index { get; set; }
		public string TimeValue { get; set; } = "";
		public decimal PriceValue { get; set; }
	}
}
