using DotComTrading.Controllers;
using DotComTrading.Models;

namespace DotComTrading.Services
{
    //Service for Calculating a Value for a Website Stock
    public static class ValuateWebsite
    {
        public static decimal CalculatePrice(Website website)
        {
            const decimal basePrice = 100m; //Base Starting Value for Each Website

			//Values used for Calculating Value
			decimal backlinkFactor = (decimal)Math.Log10(website.BacklinkCount + 1) * 80m;
			decimal friendlyBacklinkFactor = website.FriendlyBacklinkCount * 40m;
			decimal lifetimeVisitsFactor = (decimal)Math.Log10(website.LifetimeVisits + 1) * 60m;
			decimal dailyVisitsFactor = (decimal)Math.Log10(website.DailyVisits + 1) * 90m;
			decimal domainAgeFactor = (website.DomainAge * 4m);

			decimal importanceScore = (backlinkFactor + friendlyBacklinkFactor + lifetimeVisitsFactor + dailyVisitsFactor + domainAgeFactor);

            decimal price = (basePrice + importanceScore);

            //Returns and formats result of calculation as an actual money value.
            return System.Math.Round(price, 2, MidpointRounding.AwayFromZero);
        }
    }
}
