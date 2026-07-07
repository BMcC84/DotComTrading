namespace DotComTrading.Models
{
    //Represents data used to calculate website price changes
    public class WebsitePriceChange
    {
        public int WebsiteId { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PastPrice { get; set; }
    }
}
