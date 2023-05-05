namespace DemoAngularBackEnd.Dto
{
	using Newtonsoft.Json;
	using System.Runtime.Serialization;

	[DataContract]
	public class MyValueResponse<TValue> : MyBaseResponse
	{
		/// <summary>Value.</summary>
		[JsonProperty("value", Order = 0)]
		[DataMember(Name = "value", IsRequired = false, EmitDefaultValue = false)]
		public TValue Value { get; set; }
	}
}