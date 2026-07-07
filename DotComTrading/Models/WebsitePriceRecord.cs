namespace DotComTrading.Models
{
    //Represents a historical record of a websites price at a snapshot in time
    public class WebsitePriceRecord
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public Website Website { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime TimeOfRecording { get; set; }
    }
}
