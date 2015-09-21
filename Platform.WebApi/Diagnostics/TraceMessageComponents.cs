using System;

namespace DDCloud.Platform.WebApi.Diagnostics
{
	/// <summary>
	///		Components of request / response messages that can be traced.
	/// </summary>
	[Flags]
	public enum TraceMessageComponents
	{
		/// <summary>
		///		No tracing.
		/// </summary>
		None			= 0,

		/// <summary>
		///		Request headers.
		/// </summary>
		RequestHeaders	= 1,

		/// <summary>
		///		Request body.
		/// </summary>
		RequestBody		= 2,

		/// <summary>
		///		Response headers.
		/// </summary>
		ResponseHeaders	= 4,

		/// <summary>
		///		Response body.
		/// </summary>
		ResponseBody	= 8,

		/// <summary>
		///		All request message components.
		/// </summary>
		Request = RequestBody | RequestHeaders,

		/// <summary>
		///		All response message components.
		/// </summary>
		Response = ResponseBody | ResponseHeaders,

		/// <summary>
		///		All message components.
		/// </summary>
		All = Request | Response
	}
}
