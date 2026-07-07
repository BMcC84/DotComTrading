namespace DotComTrading.Services
{
	//Stores current user session data and updates ui when balance changes
	public class UserSessionService
	{
		public string Username { get; set; } = "";
		public int PortfolioId { get; set; }
		public event Action? BalanceChanged;
		
		public void NotifyBalanceChanged()
		{
			BalanceChanged?.Invoke();
		}
	}
}
