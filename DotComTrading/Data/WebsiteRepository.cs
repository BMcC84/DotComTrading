using DotComTrading.Models;
using DotComTrading.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace DotComTrading.Data
{
    //Repository responsible for managing website data and external data sources used to gain it
    public class WebsiteRepository
    {
        private readonly DotComTradingDBContext _context;
        private readonly MozSeoService _mozSeoService;
        private readonly RDAPDomainAgeService _rdapDomainAgeService;
        private readonly HttpClient _httpClient;

        public WebsiteRepository(DotComTradingDBContext context, MozSeoService mozSeoService, RDAPDomainAgeService rdapDomainAgeService, HttpClient httpClient)
        {
            _context = context;
            _mozSeoService = mozSeoService;
            _rdapDomainAgeService = rdapDomainAgeService;
            _httpClient = httpClient;
        }

        //Estimates dailyvisits using domainRank provided by Cloudflare
        private long EstimateDailyVisits(int domainRank)
        {
            double dailyVists = (1000000000 / (Math.Pow(domainRank, 0.5)));

            return (long)dailyVists;
        }

        //Estimates lifetimevisits using dailyvists and domainage
        private long EstimateLifetimeVisits(long dailyVisits, int domainAge)
        {
            double lifetimeVisits = (dailyVisits * (365 * domainAge) * 0.35);

            return (long)lifetimeVisits;
        }

        //Checks if website has favicon as all market stocks should
        private async Task<bool> CheckFaviconAsync(string domain)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://{domain}/favicon.ico");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (contentType == null || !contentType.Contains("image"))
                {
                    return false;
                }

                var googleResponse = await _httpClient.GetAsync($"https://www.google.com/s2/favicons?domain={domain}&sz=64");

                return googleResponse.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        //Used for seeding database with real domain list from Cloudflare
        public async Task AddWebsitesFromRadarAsync(CloudflareRadarClient radar, int limit)
        {
            var domains = await radar.GetTopDomainsAsync(limit);

            int currentWebsiteCount = await _context.Websites.CountAsync();
            int websiteCount = domains.Count + currentWebsiteCount;

            for(int i = 0; i < domains.Count; i++)
            {
                string domain = domains[i];
                int domainRank = i + 1;

                bool hasFavicon = await CheckFaviconAsync(domain);

                if (!hasFavicon)
                {
                    continue;
                }

                var domainExists = await _context.Websites.AnyAsync(w => w.Domain == domain);
                if(!domainExists)
                {
                    long dailyVisits = EstimateDailyVisits(domainRank);
					int domainAge = await _rdapDomainAgeService.GetDomainAgeAsync(domain);
					long lifetimeVisits = EstimateLifetimeVisits(dailyVisits, domainAge);

                    var website = new Website
                    {
                        Name = domain.Replace("www.", "").Split('.')[0],
                        Domain = domain,
                        Url = "https://" + domain,

                        BacklinkCount = 0,
                        FriendlyBacklinkCount = 0,
                        DomainAuthority = 0,
                        LastMozUpdate = null,
                        LastMetricUpdate = null,
                        MozUsed = false,
		                DomainAge = domainAge,
                        LifetimeVisits = lifetimeVisits,
                        DailyVisits = dailyVisits,
                        Price = 0,
                    };

                    await _mozSeoService.PopulateSeoMetricsAsync(website, websiteCount);
                    website.Price = ValuateWebsite.CalculatePrice(website);

                    _context.Websites.Add(website);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Website>> GetAllAsync()
        {
            return await _context.Websites.AsNoTracking().OrderBy(w => w.Id).ToListAsync();
        }

        public async Task<List<Website>> GetAllTrackedAsync()
        {
            return await _context.Websites.OrderBy(w => w.Id).ToListAsync();
        }

        public async Task<Website?> GetByIdAsync(int id)
        {
            return await _context.Websites.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Website> CreateAsync(Website website)
        {
            _context.Websites.Add(website);
            await _context.SaveChangesAsync();
            return website;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
