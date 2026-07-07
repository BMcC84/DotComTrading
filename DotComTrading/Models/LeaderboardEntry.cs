namespace DotComTrading.Models
{
	//Represents a user's position and balance for a leaderboard
	public class LeaderboardEntry
	{
		public int Rank { get; set; }
		public string Username { get; set; } = "";
		public decimal Balance { get; set; }
	}
}
