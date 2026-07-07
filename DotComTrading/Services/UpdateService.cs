using DotComTrading.Data;
using DotComTrading.Models;
using Microsoft.EntityFrameworkCore;

namespace DotComTrading.Services
{
	//Service responsible for updating metrics, creating market movement
	public class UpdateService
	{
		private readonly DotComTradingDBContext _context;
		private readonly MozSeoService _mozSeoService;

		public UpdateService(DotComTradingDBContext context, MozSeoService mozSeoService)
		{
			_context = context;
			_mozSeoService = mozSeoService;
		}

		//Generates a random change, positive or negative
		private static decimal RandomChange(decimal value, decimal changeFactor)
		{
			double randomValue = Random.Shared.NextDouble();
			decimal direction = (decimal)((randomValue * 2) - 1);

			decimal volatility = (decimal)(0.5 + Random.Shared.NextDouble());
			decimal flatNoise = ((decimal)(Random.Shared.NextDouble() - 0.5) * 50m);

			decimal totalChange = (direction * value * volatility * changeFactor) + flatNoise;

			return totalChange;
		}

		private static long ApplyMovement(long value, decimal changeFactor)
		{
			decimal updatedValue = (value + RandomChange(value, changeFactor));

			return Math.Max(Convert.ToInt64(Math.Round(updatedValue)), 1);
		}

		//Updates metrics and recalculates price
		public async Task<List<Website>> UpdateMarketAsync()
		{
			var websites = await _context.Websites.ToListAsync();
			int websiteCount = websites.Count;

			foreach(var website in websites )
			{
				var (estimatedBacklinkCount, estimatedFriendlyBacklinkCount) = _mozSeoService.EstimateSeoMetrics(website, websiteCount);

				website.BacklinkCount = Math.Max(ApplyMovement(estimatedBacklinkCount, 0.10m), 10);
				website.FriendlyBacklinkCount = (int)Math.Max(estimatedFriendlyBacklinkCount, 1);
				website.DailyVisits = Math.Max(ApplyMovement(website.DailyVisits, 0.40m), 10);
				website.LifetimeVisits = Math.Max((website.LifetimeVisits + website.DailyVisits), website.LifetimeVisits);

				website.Price = ValuateWebsite.CalculatePrice(website);
				website.LastMetricUpdate = DateTime.UtcNow;

				var priceRecord = new WebsitePriceRecord
				{
					WebsiteId = website.Id,
					Price = website.Price,
					TimeOfRecording = DateTime.UtcNow
				};

				_context.WebsitePriceRecords.Add(priceRecord);
			}

			await _context.SaveChangesAsync();
			return websites;
		}
	}
}
