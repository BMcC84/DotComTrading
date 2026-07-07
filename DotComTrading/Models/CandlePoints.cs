namespace DotComTrading.Models
{
	//Represents candlestick chart data for building price movement chart in frontend
	public class CandlePoints
	{
		public decimal Open { get; set; }
		public decimal Close { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }

		public int WickHeight { get; set; }
		public int WickTop { get; set; }
		public int BodyHeight { get; set; }
		public int BodyTop { get; set; }
		public bool IsUp => Close >= Open;
	}
}
