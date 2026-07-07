using Microsoft.VisualBasic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DotComTrading.Services
{
    public class CloudflareRadarClient
    {
        private readonly HttpClient _httpClient;

        public CloudflareRadarClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Fetches a list of Cloudflares top ranked domains
        public async Task<List<string>> GetTopDomainsAsync(int limit)
        {
            var url = $"radar/ranking/top?rankingType=POPULAR&limit={limit}&format=JSON";

            using var response = await _httpClient.GetAsync(url);

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            var top = document.RootElement.GetProperty("result").GetProperty("top_0");

            var domainList = new List<string>(top.GetArrayLength());

            foreach (var item in top.EnumerateArray())
            {
                if (item.TryGetProperty("domain", out var domain))
                {
                    var domainString = domain.GetString();
                    if (!string.IsNullOrWhiteSpace(domainString))
                    {
                        domainList.Add(domainString.Trim().ToLowerInvariant());
                    }
                }
            }

            return domainList;
        }


    }
}
