using System.Net.Http.Json;
using Azure.Core.GeoJson;
using DotComTrading.Models;

namespace DotComTrading.UI.Services
{
    public class APIClient
    {
        private readonly HttpClient _httpClient;

        public APIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DotComTrading");
        }

        //GetAll()
        public async Task<List<Website>> GetWebsitesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<Website>>("api/websites");

            if (result == null)
            {
                return new List<Website>();
            }
            return result;
        }

        //GetPortfolio()
        public async Task<PortfolioResponse> GetPortfolioAsync(int portfolioId)
        {
            var response = await _httpClient.GetAsync("api/trading/portfolio/" + portfolioId);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("Failed to fetch portfolio, Code: " + response.StatusCode + " " + body);
            }

            var portfolio = await response.Content.ReadFromJsonAsync<PortfolioResponse>();

            if (portfolio == null)
            {
                throw new HttpRequestException("Portfolio response was empty.");
            }

            return portfolio;
        }

        //Purchase(int portfolioId, FromBody TradeRequest request)
        public async Task<string> PurchaseAsync(int portfolioId, TradeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(("api/trading/" + portfolioId + "/purchase"), request);

            if(!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("Failed to Process Transaction, Code: " + response.StatusCode.ToString() + " " + response.ReasonPhrase + " " + body);
            }

            return await response.Content.ReadAsStringAsync();
        }

        //Sell(int portfolioId, FromBody TradeRequest request)
        public async Task<string> SellAsync(int portfolioId, TradeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(("api/trading/" + portfolioId + "/sell"), request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("Failed to Process Transaction, Code: " + response.StatusCode.ToString() + " " + response.ReasonPhrase + " " + body);
            }

            return await response.Content.ReadAsStringAsync();
        }

        //GetPriceChanges()
        public async Task<List<WebsitePriceChange>> GetPriceChangesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WebsitePriceChange>>("api/websites/price-changes");
        }

        //GetById(int id)
        public async Task<Website> GetWebsiteByIdAsync(int websiteId)
        {
            return await _httpClient.GetFromJsonAsync<Website>($"api/websites/{websiteId}");
        }

        //GetPriceHistory(int id, [FromQuery] int count = 40)
        public async Task<List<WebsitePriceRecord>> GetPriceHistoryAsync(int websiteId, int count)
        {
            var result = await _httpClient.GetFromJsonAsync<List<WebsitePriceRecord>>($"api/websites/{websiteId}/price-history?count={count}");

            if(result == null)
            {
                return new List<WebsitePriceRecord>();
            }

            return result;
        }

        //CreateWebsite([FromBody] CreateWebsiteRequest request)
        public async Task<HttpResponseMessage> CreateWebsiteAsync(string url)
        {
            var request = new
            {
                Url = url
            };

            return await _httpClient.PostAsJsonAsync("api/websites", request);
        }

        //GetTradeHistory(int portfolioId)
        public async Task<List<TradeHistoryResponse>> GetTradeHistoryAsync(int portfolioId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<TradeHistoryResponse>>("api/trading/portfolio/" + portfolioId + "/trades");

            if(result == null)
            {
                return new List<TradeHistoryResponse>();
            }

            return result;
        }

        //GetLeaderboard()
        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>("api/trading/leaderboard");

            if(result == null)
            {
                return new List<LeaderboardEntry>();
            }

            return result;
        }
    }
}
