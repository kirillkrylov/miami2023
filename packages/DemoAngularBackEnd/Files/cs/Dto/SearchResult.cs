namespace DemoAngularBackEnd.Dto
{
	using Newtonsoft.Json;
	using System.Collections.Generic;


	[JsonObject]
	public class SearchResult
	{

		/// <summary>
		/// Total number of matching items.
		/// </summary>
		[JsonProperty("total_count")]
		public int TotalCount { get; set; }

		/// <summary>
		/// True if the query timed out and it's possible that the results are incomplete.
		/// </summary>
		[JsonProperty("incomplete_results")]
		public bool IncompleteResults { get; set; }

		/// <summary>
		/// The found items. Up to 100 per page.
		/// </summary>

		[JsonProperty("items")]
		public List<User> Items { get; set; }

	}
}