using DotComTrading.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DotComTrading.Services
{
    //Service used for retrieval or estimation of SEO metrics
    public class MozSeoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MozSeoService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        //Retrieves backlink and domainAuthority data from Moz and uses estimates as fallback values
        public async Task<(long backlinks, double authority, bool mozUsed)> GetSeoMetricsAsync(Website website)
        {
            try
            {
                var accessId = _config["Moz:AccessId"];
                var secretKey = _config["Moz:SecretKey"];

				var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accessId}:{secretKey}"));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var requestBody = new
                {
                    targets = new[] { website.Domain },
                    metrics = new[] { "external_pages_to_root_domain", "domain_authority" }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://lsapi.seomoz.com/v2/url_metrics", content);

				if (!response.IsSuccessStatusCode)
                {
                    return (0, 0, false);
                }

                var json = await response.Content.ReadAsStringAsync();

				var data = JsonSerializer.Deserialize<MozResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var result = data?.Results?.FirstOrDefault();

                //Uses Esimates as Fallback Values
                if(result == null || result.ExternalLinks == 0)
                {
                    long estimatedBacklinkCount = EstimateBacklinkCount(website);
                    website.BacklinkCount = estimatedBacklinkCount;
                    double estimatedAuthority = EstimateDomainAuthority(website);

					return(estimatedBacklinkCount, estimatedAuthority, false);
                }

                return (result.ExternalLinks, result.DomainAuthority, true);
            }
            catch
            {
				return (0, 0, false);
            }
        }

        private long EstimateBacklinkCount(Website website)
        {
            double ageFactor = ((website.DomainAge * 0.1) + 1);

            double visitsFactor = ((Math.Log10(website.DailyVisits + 1) * 4000) + (Math.Log10(website.LifetimeVisits + 1) * 1500));

            double estimatedBacklinkCount = visitsFactor * ageFactor;

            return (long)estimatedBacklinkCount;
        }

        private double EstimateDomainAuthority(Website website)
        {
            double backlinkFactor = (Math.Log10(website.BacklinkCount + 1) * 10);

            double ageFactor = website.DomainAge * 0.85;

            double estimatedDomainAuthority = backlinkFactor + ageFactor;

			return Math.Min(estimatedDomainAuthority, 100);
        }

        private int EstimateFriendlyBacklinkCount(Website website, int websiteCount)
        {
            double authorityFactor = (website.DomainAuthority / 100);

            int estimatedFriendlyBacklinkCount = (int)((websiteCount * 0.3) * authorityFactor);

            return estimatedFriendlyBacklinkCount;
        }

        public async Task PopulateSeoMetricsAsync(Website website, int websiteCount)
        {
            var (backlinkCount, domainAuthority, mozUsed) = await GetSeoMetricsAsync(website);

            website.BacklinkCount = backlinkCount;
            website.DomainAuthority = domainAuthority;
            website.MozUsed = mozUsed;
            website.FriendlyBacklinkCount = EstimateFriendlyBacklinkCount(website, websiteCount);
            website.LastMetricUpdate = DateTime.UtcNow;
            if (mozUsed)
            {
                website.LastMozUpdate = DateTime.UtcNow;
            }
        }

        public (long backlinkCount, long friendlyBacklinkCount) EstimateSeoMetrics(Website website, int websiteCount)
        {
            long estimatedBacklinkCount = EstimateBacklinkCount(website);
            long estimatedFriendlyBacklinkCount = EstimateFriendlyBacklinkCount(website, websiteCount);

            return(estimatedBacklinkCount, estimatedFriendlyBacklinkCount);
        }
    }
}
