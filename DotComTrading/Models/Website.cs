namespace DotComTrading.Models
{
    //Represents a Tradable Website
    public class Website
    {
        //Basic Information about Website Stock
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Domain { get; set; } = "";
        public string Url { get; set; } = "";

        //Data for Valuating Website Stock
        public long BacklinkCount {  get; set; }
        public double DomainAuthority { get; set; }
        public DateTime? LastMozUpdate { get; set; }
		public DateTime? LastMetricUpdate { get; set; }
		public bool MozUsed { get; set; }
        public int FriendlyBacklinkCount { get; set; } 
        public int DomainAge { get; set; } = 0;
        public long LifetimeVisits { get; set; }
        public long DailyVisits {  get; set; }

        public decimal Price {  get; set; }

        public List<Trade> Trades { get; set; } = new();
        public List<WebsitePriceRecord> PriceRecords { get; set; } = new();
    }
}
