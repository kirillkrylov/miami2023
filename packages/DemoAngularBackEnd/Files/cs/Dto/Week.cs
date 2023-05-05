namespace DemoAngularBackEnd.Dto
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;

	public class Weeks
	{

		[JsonProperty("weeks")]
		public List<Week> ContributionDays { get; set; }

	}


	public class Week
	{

		[JsonProperty("contributionDays")]
		public List<ContributionDay> ContributionDays { get; set; }
	}


	[JsonObject]
	public class ContributionDay
	{

		[JsonProperty("weekday")]
		public int WeekDay { get; set; }


		[JsonProperty("date")]
		public DateTime Date { get; set; }


		[JsonProperty("contributionCount")]
		public int Count { get; set; }


		[JsonProperty("color")]
		public string Color { get; set; }

	}

}