using System;
using System.Net;

namespace DDCloud.Platform.WebApi.Diagnostics
{
	using Core.ErrorHandling;

	/// <summary>
	///		HTTP message handler that traces messages using the <see cref="PlatformWebApiEvents">Aperture Web API event-source</see>.
	/// </summary>
	public sealed class EtwTraceMessageHandler
		: TraceMessageHandler
	{
		/// <summary>
		///		Create a new <see cref="TraceMessageHandler"/>.
		/// </summary>
		/// <param name="enabledComponents">
		///		Components of messages to be traced.
		/// </param>
		public EtwTraceMessageHandler(TraceMessageComponents enabledComponents)
			: base(enabledComponents)
		{
		}

		/// <summary>
		///		Trace the start of a request.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="requestUri">
		///		The full URI of the request.
		/// </param>
		/// <param name="method">
		///		The request's HTTP method (e.g. GET, POST, etc).
		/// </param>
		/// <param name="hasBody">
		///		Does the request have a body?
		/// </param>
		protected override void TraceBeginRequest(Guid requestId, Uri requestUri, string method, bool hasBody)
		{
			if (requestUri == null)
				throw new ArgumentNullException(nameof(requestUri));

			PlatformWebApiEvents.Raise.SendingHttpRequest(
				requestUri.AbsoluteUri,
				method,
				hasBody
			);
		}

		/// <summary>
		///		Trace the end of a request.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="requestUri">
		///		The full URI of the request.
		/// </param>
		/// <param name="method">
		///		The request's HTTP method (e.g. GET, POST, etc).
		/// </param>
		/// <param name="responseStatusCode">
		///		The response status code.
		/// </param>
		protected override void TraceEndRequest(Guid requestId, Uri requestUri, string method, HttpStatusCode responseStatusCode)
		{
			if (requestUri == null)
				throw new ArgumentNullException(nameof(requestUri));

			PlatformWebApiEvents.Raise.SentHttpRequest(
				requestUri.AbsoluteUri,
				method
			);
		}

		/// <summary>
		///		Trace an error encountered while processing a request.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="requestUri">
		///		The full URI of the request.
		/// </param>
		/// <param name="method">
		///		The request's HTTP method (e.g. GET, POST, etc).
		/// </param>
		/// <param name="exception">
		///		The exception encountered while processing the request.
		/// </param>
		protected override void TraceRequestError(Guid requestId, Uri requestUri, string method, Exception exception)
		{
			if (requestUri == null)
				throw new ArgumentNullException(nameof(requestUri));

			if (exception == null)
				throw new ArgumentNullException(nameof(exception));

			PlatformWebApiEvents.Raise.SendHttpRequestFailed(
				requestUri.AbsoluteUri,
				method,
				exceptionType: exception.GetType().FullName,
				exceptionMessage: exception.Message,
				exceptionDetail: exception.SafeToString()
			);
		}
	}
}
