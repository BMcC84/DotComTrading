using System.Text.Json;

namespace DotComTrading.Services
{
    //Service for retrieving domainAge regardless of TLD
    public class RDAPDomainAgeService
    {
        private readonly HttpClient _httpClient;
        private JsonDocument? _bootstrapDocument;
        private DateTime _bootstrapDocumentDownloadTime = DateTime.MinValue;
        private static readonly TimeSpan _bootstrapDownloadDuration = TimeSpan.FromHours(4);

        public RDAPDomainAgeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Extracts top-level domain from a given domain
        private string GetTLD(string domain) 
        {
            var domainParts = domain.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (domainParts.Length < 2)
            {
                return string.Empty;
            }
            return domainParts[^1];
        }

        //Retrieves RDAP bootstrap document from IANA
		private async Task<JsonDocument> GetBootstrapDocumentAsync()
		{
			if (_bootstrapDocument != null && (DateTime.UtcNow - _bootstrapDocumentDownloadTime) < _bootstrapDownloadDuration)
			{
				return _bootstrapDocument;
			}

			const string bootstrapUrl = "https://data.iana.org/rdap/dns.json";

			var response = await _httpClient.GetStringAsync(bootstrapUrl);

			_bootstrapDocument?.Dispose();
			_bootstrapDocument = JsonDocument.Parse(response);
			_bootstrapDocumentDownloadTime = DateTime.UtcNow;

			return _bootstrapDocument;
		}

        //Determines correct RDAP URL based on domains TLD
		private async Task<string?> GetRdapBaseUrlAsync(string domain)
        {
            var tldString = GetTLD(domain);

            if (tldString == null)
            {
                return null;
            }

            var document = await GetBootstrapDocumentAsync();

            if(document == null)
            {
                return null;
            }

            if(!document.RootElement.TryGetProperty("services", out JsonElement services))
            {
                return null;
            }

            foreach (var service in services.EnumerateArray())
            {
                if(service.GetArrayLength() < 2)
                {
                    continue;
                }

                var tlds = service[0];
                var urls = service[1];

                foreach(var tld in tlds.EnumerateArray())
                {
                    var currentTld = tld.GetString();

                    if (string.Equals(currentTld, tldString, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var url in urls.EnumerateArray())
                        {
                            var baseUrl = url.GetString();

                            if (!string.IsNullOrWhiteSpace(baseUrl))
                            {
                                return baseUrl.TrimEnd('/');
                            }
                        }
                    }
                }
            }
            return null;
        }

        //Gets domain age using RDAP registration data
        public async Task<int> GetDomainAgeAsync(string domain)
        {
            try
            {
                string? baseUrl = await GetRdapBaseUrlAsync(domain);

                if (string.IsNullOrEmpty(baseUrl))
                {
                    return 0;
                }

                string url = $"{baseUrl}/domain/{domain}";

                var response = await _httpClient.GetStringAsync(url);

                using JsonDocument document = JsonDocument.Parse(response);

                if (!document.RootElement.TryGetProperty("events", out JsonElement events))
                {
                    return 0;
                }

                foreach (var _event in events.EnumerateArray())
                {
                    if (!_event.TryGetProperty("eventAction", out JsonElement actionElement))
                    {
                        continue;
                    }
                    if (!_event.TryGetProperty("eventDate", out JsonElement dateElement))
                    {
                        continue;
                    }

                    var action = actionElement.GetString();

                    if (action == "registration")
                    {
                        DateTime createdDate = _event.GetProperty("eventDate").GetDateTime();

                        int age = DateTime.UtcNow.Year - createdDate.Year;

                        if (DateTime.UtcNow < createdDate.AddYears(age))
                        {
                            age--;
                        }

                        return age;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{domain} has thrown RDAP error: {ex.Message}");
            }
            return 0;
        }
    }
}
