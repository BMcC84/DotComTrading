using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotComTrading.Models;
using DotComTrading.Data;
using System.Security.Claims;

namespace DotComTrading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradingController : ControllerBase
    {
        private readonly WebsiteRepository _websiteRepository;
        private readonly PortfolioRepository _portfolioRepository;

        public TradingController(WebsiteRepository websiteRepository, PortfolioRepository portfolioRepository)
        {
            _websiteRepository = websiteRepository;
            _portfolioRepository = portfolioRepository;
        }

        //Gets user's portfolio with balance and holdings
        [HttpGet("portfolio/{portfolioId}")]
        public async Task<ActionResult> GetPortfolio(int portfolioId)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
            if(portfolio == null)
            {
                return NotFound();
            }

            var displayHoldings = new List<PortfolioHoldings>();

                foreach(var holding in portfolio.Holdings)
                {
                    var website = await _websiteRepository.GetByIdAsync(holding.WebsiteId);
                    if(website == null)
                    {
                        return BadRequest();
                    }

                    displayHoldings.Add(new PortfolioHoldings
                    {
                        WebsiteId = holding.WebsiteId,
                        Website = website,
                        NoShares = holding.NoShares,
                    });
                }
                //Returns structured portfolio for frontend
                return Ok(new
                {
                    portfolioId = portfolio.Id,
                    balance = portfolio.Balance,
                    holdings = displayHoldings
                });
        }

        //Buy operation for website shares
        [HttpPost("{portfolioId}/purchase")]
        public async Task<ActionResult> Purchase(int portfolioId, [FromBody] TradeRequest request)
        {
            if(request == null || request.NoShares <= 0)
            {
                return BadRequest();
            }

            var website = await _websiteRepository.GetByIdAsync(request.WebsiteId);
            if(website == null)
            {
                return NotFound();
            }

            var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
            if(portfolio == null)
            {
                return NotFound();
            }

            decimal cost = (website.Price * request.NoShares); //Calculates cost of shares the user is trying to purchase

            //Checks that user can afford their order
            if(portfolio.Balance < cost)
            {
                return BadRequest();
            }

            portfolio.Balance -= cost;

            //Updates or creates holding
            var holding = portfolio.Holdings.FirstOrDefault(h  => h.WebsiteId == website.Id);
            if(holding == null)
            {
                portfolio.Holdings.Add(new Holding
                {
                    PortfolioId = portfolio.Id,
                    WebsiteId = website.Id,
                    NoShares = request.NoShares
                });
            }
            else
            {
                holding.NoShares += request.NoShares;
            }

            //Records trade for transaction history
            portfolio.Trades.Add(new Trade
            {
                PortfolioId = portfolio.Id,
                WebsiteId = website.Id,
                NoShares = request.NoShares,
                PricePerShare = website.Price,
                TradeTime = DateTime.UtcNow,
                TradeType = "BUY"
            });

            await _portfolioRepository.SaveChangesAsync();

            //Returns transaction summary for frontend
            return Ok(new
            {
                message = "Purchased " + request.NoShares + " of " + website.Name,
                portfolioId = portfolio.Id,
                websiteId = website.Id,
                numberofShares = request.NoShares,
                priceofShare = website.Price,
                newBalance = portfolio.Balance
            });
        }

        //Sell operation for website shares
        [HttpPost("{portfolioId}/sell")]
        public async Task<ActionResult> Sell(int portfolioId, [FromBody] TradeRequest request)
        {
            if(request == null || request.NoShares <= 0)
            {
                return BadRequest();
            }

            var website = await _websiteRepository.GetByIdAsync(request.WebsiteId);
            if(website == null)
            {
                return NotFound();
            }

            var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
            if(portfolio == null)
            {
                return NotFound();
            }

            //Ensures user actually owns what they are trying to sell
            var holding = portfolio.Holdings.FirstOrDefault(h => h.WebsiteId == website.Id);
            if(holding == null || holding.NoShares < request.NoShares)
            {
                return BadRequest();
            }

            decimal price = website.Price * request.NoShares;

            portfolio.Balance += price;
            holding.NoShares -= request.NoShares;

            //Removes Holding from User's Portfolio if they now own 0 Shares
            if(holding.NoShares == 0)
            {
                portfolio.Holdings.Remove(holding);
            }

			//Records trade for transaction history
			portfolio.Trades.Add(new Trade
			{
				PortfolioId = portfolio.Id,
				WebsiteId = website.Id,
				NoShares = request.NoShares,
				PricePerShare = website.Price,
				TradeTime = DateTime.UtcNow,
				TradeType = "SELL"
			});

			await _portfolioRepository.SaveChangesAsync();

			//Returns transaction summary for frontend
			return Ok(new
            {
                message = "Sold " + request.NoShares + " of " + website.Name,
                portfolioId = portfolio.Id,
                websiteId = website.Id,
                numberofSharesSold = request.NoShares,
                priceofShare = website.Price,
                newBalance = portfolio.Balance
            });
        }

        //Returns portfolio's full trade history, starting with most recent
        [HttpGet("portfolio/{portfolioId}/trades")]
        public async Task<ActionResult> GetTradeHistory(int portfolioId)
        {
			var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);

			if (portfolio == null)
			{
				return NotFound();
			}

            var trades = portfolio.Trades.OrderByDescending(trade => trade.TradeTime).Select(trade => new
            {
                trade.Id,
                trade.PortfolioId,
                trade.WebsiteId,
                WebsiteName = trade.Website.Name,
                Domain = trade.Website.Domain,
                trade.NoShares,
                trade.PricePerShare,
                TotalValue = (trade.NoShares * trade.PricePerShare),
                trade.TradeTime,
                trade.TradeType,
			}).ToList();

			return Ok(trades);
		}

        //Returns leaderboard ranking of portfolios based on balance
		[HttpGet("leaderboard")]
		public async Task<ActionResult> GetLeaderboard()
		{
			var portfolios = await _portfolioRepository.GetAllAsync();

			var leaderboard = portfolios.OrderByDescending(p => p.Balance).Select((portfolios, index) => new
			{
				Rank = index + 1,
				Username = portfolios.User.UserName,
				Balance = portfolios.Balance
			}).ToList();

			return Ok(leaderboard);
		}
	}
}
