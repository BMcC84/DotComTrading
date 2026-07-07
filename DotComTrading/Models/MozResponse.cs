using System.Text.Json.Serialization;

namespace DotComTrading.Models
{
	//Represents the structure for Moz API responses
	public class MozResponse
	{
		[JsonPropertyName("results")]
		public List<MozResult>? Results { get; set; }
	}

	//Represents the individual SEO metrics Moz returns
	public class MozResult 
	{
		[JsonPropertyName("external_pages_to_root_domain")]
		public long ExternalLinks { get; set; }

		[JsonPropertyName("domain_authority")]
		public double DomainAuthority { get; set; }
	}
}
