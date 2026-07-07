using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotComTrading.Models;
using DotComTrading.Services;
using Xunit.Abstractions;

namespace DotComTrading.Tests
{
    public class ValuateWebsiteTests
    {
        //Allows Prices to be Displayed
        private readonly ITestOutputHelper output;

        public ValuateWebsiteTests(ITestOutputHelper output) 
        {
            this.output = output;
        }

        //Testing that given known inputs, the correct price is calculated
        [Fact]
        public void CalculatePriceReturnsExpectedPrice()
        {
            const decimal basePrice = 10m;

            var website = new Website //Website Where All Factors for Importance Score Will be 1
            {
                BacklinkCount = 1000,
                FriendlyBacklinkCount = 10,
                LifetimeVisits = 1000000,
                DailyVisits = 1000,
                DomainAge = 4
            };

            decimal importanceScore = 20 + 40 + 5 + 50; //All Weights for Importance Score * 1
            decimal dampingFactor = (0.85m + (0.15m * (website.DomainAge / (website.DomainAge + 4m))));

            decimal expectedPrice = Math.Round(((basePrice + importanceScore) * dampingFactor), 2, MidpointRounding.AwayFromZero);

            var actualPrice = ValuateWebsite.CalculatePrice(website);

            Assert.Equal(expectedPrice, actualPrice);
        }

        //Testing that as website age increases, if the website price increases and by how much
        [Fact]
        public void CalculatePriceDomainAgeImportance()
        {
            var age5Website = new Website //Website Where All Factors for Importance Score Will be 1 and Domain Age is 5
            {
                BacklinkCount = 1000,
                FriendlyBacklinkCount = 10,
                LifetimeVisits = 1000000,
                DailyVisits = 1000,
                DomainAge = 5
            };

            var age10Website = new Website //Website Where All Factors for Importance Score Will be 1 and Domain Age is 10
            {
                BacklinkCount = 1000,
                FriendlyBacklinkCount = 10,
                LifetimeVisits = 1000000,
                DailyVisits = 1000,
                DomainAge = 10
            };

            var age5Price = ValuateWebsite.CalculatePrice(age5Website);
            var age10Price = ValuateWebsite.CalculatePrice(age10Website);

            output.WriteLine("5 Year Website Price: " + age5Price);
            output.WriteLine("10 Year Website Price: " + age10Price);

            Assert.True(age10Price > age5Price);
        }
    }
}
