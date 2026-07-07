using Microsoft.Extensions.Hosting;

namespace DotComTrading.Services
{
	//Background service responsible for automatically updating metrics and prices
	public class MarketMovementService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<MarketMovementService> _logger;

		public MarketMovementService(IServiceProvider serviceProvider, ILogger<MarketMovementService> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("MarketMovementService started.");

			while(!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
				try
				{
					using IServiceScope scope = _serviceProvider.CreateScope();

					UpdateService updateService = scope.ServiceProvider.GetRequiredService<UpdateService>();

					await updateService.UpdateMarketAsync();

					_logger.LogInformation("Market Updated at {Time}", DateTime.UtcNow);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Market Update Error");
				}
			}
		}
	}
}
