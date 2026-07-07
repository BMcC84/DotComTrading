namespace DotComTrading.Models
{
    //Represents a user's portfolio
    public class Portfolio
    {
        public int Id { get; set; }
        public decimal Balance { get; set; } = 10000m;
        public List<Holding> Holdings { get; set; } = new();
        public List<Trade> Trades { get; set; } = new();

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
