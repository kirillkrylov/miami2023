namespace DemoAngularBackEnd.Dto
{
	using System;
	using System.Runtime.Serialization;
	using Terrasoft.Core.ServiceModelContract;

	[DataContract]
	[Serializable]
	public class MyBaseResponse
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terrasoft.Core.ServiceModelContract.BaseResponse" /> class.
		/// </summary>
		public MyBaseResponse() => this.Success = true;

		/// <summary>The service response status.</summary>
		[DataMember(Name = "success", IsRequired = true)]
		public bool Success { get; set; }

		/// <summary>Error object.</summary>
		[DataMember(Name = "errorInfo", IsRequired = false, EmitDefaultValue = false)]
		public ErrorInfo ErrorInfo { get; set; }
	}
}