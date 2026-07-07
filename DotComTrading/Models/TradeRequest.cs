namespace DotComTrading.Models
{
    //Represents request data for buying or selling shares
    public class TradeRequest
    {
        public int WebsiteId { get; set; }
        public decimal NoShares {  get; set; }
    }
}
