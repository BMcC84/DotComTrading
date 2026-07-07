using DotComTrading.Data;
using DotComTrading.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DotComTrading.Services
{
	//Service responsible for creating new website entries and gathering data from external sources
	public class CreateWebsiteService
	{
		private readonly RDAPDomainAgeService _domainAgeService;
		private readonly MozSeoService _mozSeoService;
		private readonly DotComTradingDBContext _context;

		public CreateWebsiteService(RDAPDomainAgeService domainAgeService, MozSeoService mozSeoService, DotComTradingDBContext context)
		{
			_domainAgeService = domainAgeService;
			_mozSeoService = mozSeoService;
			_context = context;
		}

		//Validates input, gathers external data and creates new website object
		public async Task<Website?> CreateWebsite(string url)
		{
			try
			{
				if (string.IsNullOrEmpty(url))
				{
					return null;
				}

				string formattedUrl = FormatUrl(url);

				if (formattedUrl == "")
				{
					return null;
				}

				string domain = GetDomain(formattedUrl);

				if (domain == "")
				{
					return null;
				}

				bool existingWebsite = await _context.Websites.AnyAsync(w => w.Domain == domain);

				if (existingWebsite)
				{
					throw new Exception("This Website is Already on the Market.");
				}

				string name = GetName(domain);
				int domainAge = await _domainAgeService.GetDomainAgeAsync(domain);
				int websiteCount = await _context.Websites.CountAsync();

				Website website = new Website
				{
					Name = name,
					Url = formattedUrl,
					Domain = domain,
					DomainAge = domainAge,
					DailyVisits = 0,
					LifetimeVisits = 0,
					BacklinkCount = 0,
					FriendlyBacklinkCount = 0,
					DomainAuthority = 0,
					MozUsed = false,
				};

				await _mozSeoService.PopulateSeoMetricsAsync(website, websiteCount);

				website.Price = ValuateWebsite.CalculatePrice(website);

				return website;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		//Ensures provided URL can be used
		private static string FormatUrl(string givenUrl)
		{
			string url = givenUrl.Trim();

			if(!url.StartsWith("http://") && !url.StartsWith("https://"))
			{
				url = "https://" + url;
			}

			Uri? uri;
			bool isValid = Uri.TryCreate(url, UriKind.Absolute, out uri);

			if(!isValid ||  uri == null)
			{
				return "";
			}
			return uri.ToString();
		}

		private static string GetDomain(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				string domain = uri.Host.ToLowerInvariant();

				if (domain.StartsWith("www."))
				{
					domain = domain.Substring(4);
				}
				return domain;
			}
			catch
			{
				return "";
			}
		}

		//Generates display name for new website
		private static string GetName(string domain)
		{
			string name = domain.Split('.')[0];

			if (string.IsNullOrWhiteSpace(name))
			{
				return domain;
			}
			return char.ToUpper(name[0]) + name.Substring(1);
		}
	}
}
