using System.Diagnostics.Tracing;

namespace DDCloud.Platform.WebApi
{
	using Core;
	using Core.Utilities;
	
	/// <summary>
	///		The Aperture Web API event-source.
	/// </summary>
	[EventSource(Name = EventSourceName)]
	public sealed class PlatformWebApiEvents
		: EventSource
	{
		/// <summary>
		///		The Aperture Web API event-source name.
		/// </summary>
		const string EventSourceName					= "DDCloud-Platform-WebAPI";

		/// <summary>
		///		Placeholder inserted into event payloads when one was not supplied when raising the event.
		/// </summary>
		const string Unknown							= "Unknown";

		/// <summary>
		///		Message inserted into event payloads when one was not supplied when raising the event.
		/// </summary>
		const string NoFurtherInformationAvailable		= "No further information is available.";

		/// <summary>
		///		The maximum number of Unicode characters that will be inserted into the event payload when representing exception detail.
		/// </summary>
		public const int MaximumExceptionDetailLength	= 500;

		/// <summary>
		///		The maximum number of Unicode characters that will be inserted into the event payload when representing a request URI.
		/// </summary>
		public const int MaximumRequestUriLength		= 500;

		/// <summary>
		///		A singleton instance of the Aperture Web API event-source that can be used to raise events.
		/// </summary>
		static readonly PlatformWebApiEvents Instance = new PlatformWebApiEvents();

		/// <summary>
		///		A singleton instance of the Aperture Web API event-source that can be used to raise events.
		/// </summary>
		public static PlatformWebApiEvents Raise
		{
			get
			{
				// Ensure we have a valid activity Id.
				ActivityCorrelationManager.SynchronizeEventSourceActivityIds();

				return Instance;
			}
		}

		/// <summary>
		///		Create a new platform Web API event-source.
		/// </summary>
		PlatformWebApiEvents()
#if DEBUG
			: base(throwOnEventWriteErrors: true)
#else
			: base(throwOnEventWriteErrors: false)
#endif
		{
		}

		/// <summary>
		///		Raise an event representing the start of sending an HTTP request.
		/// </summary>
		/// <param name="requestUri">
		///		The request URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The request's HTTP method.
		/// </param>
		/// <param name="hasBody">
		///		Does the request have a body?
		/// </param>
		[
			Event(
				EventIds.SendingHttpRequest,
				Message = "Sending HTTP '{1}' request to '{0}'.",
				Level = EventLevel.Verbose,
				Task = Tasks.SendHttpRequest,
				Opcode = EventOpcode.Start,
				Version = 2
			)
		]
		public void SendingHttpRequest(string requestUri, string method, bool hasBody)
		{
			WriteEvent(
				EventIds.SendingHttpRequest,
				(requestUri ?? NoFurtherInformationAvailable).Truncate(MaximumRequestUriLength),
				method ?? Unknown,
				hasBody
			);
		}

		/// <summary>
		///		Raise an event representing the completion of sending an HTTP request.
		/// </summary>
		/// <param name="requestUri">
		///		The request URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The request's HTTP method.
		/// </param>
		[
			Event(
				EventIds.SentHttpRequest,
				Message = "Sent HTTP '{1}' request to '{0}'.",
				Level = EventLevel.Verbose,
				Task = Tasks.SendHttpRequest,
				Opcode = EventOpcode.Stop,
				Version = 2
			)
		]
		public void SentHttpRequest(string requestUri, string method)
		{
			WriteEvent(
				EventIds.SentHttpRequest,
				(requestUri ?? NoFurtherInformationAvailable).Truncate(MaximumRequestUriLength),
				method ?? Unknown
			);
		}

		/// <summary>
		///		Raise an event representing a failure to send an HTTP request.
		/// </summary>
		/// <param name="requestUri">
		///		The request URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The request's HTTP method.
		/// </param>
		/// <param name="exceptionType">
		///		The type of exception that was encountered.
		/// </param>
		/// <param name="exceptionMessage">
		///		The exception message.
		/// </param>
		/// <param name="exceptionDetail">
		///		The exception detail (truncated to <see cref="MaximumExceptionDetailLength"/> Unicode characters).
		/// </param>
		[
			Event(
				EventIds.SendHttpRequestFailed,
				Message = "Failed to send HTTP '{1}' request to '{0}' - {3}",
				Level = EventLevel.Error,
				Task = Tasks.SendHttpRequest,
				Opcode = Opcodes.Fail,
				Version = 2
			)
		]
		public void SendHttpRequestFailed(string requestUri, string method, string exceptionType, string exceptionMessage, string exceptionDetail)
		{
			WriteEvent(
				EventIds.SendHttpRequestFailed,
				(requestUri ?? NoFurtherInformationAvailable).Truncate(MaximumRequestUriLength),
				method ?? Unknown,
				exceptionType ?? Unknown,
				(exceptionMessage ?? NoFurtherInformationAvailable),
				(exceptionDetail ?? NoFurtherInformationAvailable).Truncate(MaximumExceptionDetailLength)
			);
		}

		/// <summary>
		///		Raise an event representing the start of receiving an HTTP response.
		/// </summary>
		/// <param name="responseUri">
		///		The response URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The response's HTTP method.
		/// </param>
		/// <param name="hasBody">
		///		Does the response have a body?
		/// </param>
		[
			Event(
				EventIds.ReceivingHttpResponse,
				Message = "Receiving response for HTTP '{1}' request to '{0}'.",
				Level = EventLevel.Verbose,
				Task = Tasks.ReceiveHttpResponse,
				Opcode = EventOpcode.Start,
				Version = 2
			)
		]
		public void ReceivingHttpResponse(string responseUri, string method, bool hasBody)
		{
			WriteEvent(
				EventIds.ReceivingHttpResponse,
				responseUri ?? NoFurtherInformationAvailable,
				method ?? Unknown,
				hasBody
			);
		}

		/// <summary>
		///		Raise an event representing the completion of receiving an HTTP response.
		/// </summary>
		/// <param name="responseUri">
		///		The response URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The response's HTTP method.
		/// </param>
		/// <param name="hasBody">
		///		Does the response have a body?
		/// </param>
		[
			Event(
				EventIds.ReceivedHttpResponse,
				Message = "Received response for HTTP '{1}' request to '{0}'.",
				Level = EventLevel.Verbose,
				Task = Tasks.ReceiveHttpResponse,
				Opcode = EventOpcode.Stop,
				Version = 2
			)
		]
		public void ReceivedHttpResponse(string responseUri, string method, bool hasBody)
		{
			WriteEvent(
				EventIds.ReceivedHttpResponse,
				responseUri ?? NoFurtherInformationAvailable,
				method ?? Unknown,
				hasBody
			);
		}

		/// <summary>
		///		Raise an event representing a failure to send an HTTP request.
		/// </summary>
		/// <param name="requestUri">
		///		The request URI (truncated to <see cref="MaximumRequestUriLength"/> Unicode characters).
		/// </param>
		/// <param name="method">
		///		The request's HTTP method.
		/// </param>
		/// <param name="exceptionType">
		///		The type of exception that was encountered.
		/// </param>
		/// <param name="exceptionMessage">
		///		The exception message.
		/// </param>
		/// <param name="exceptionDetail">
		///		The exception detail (truncated to <see cref="MaximumExceptionDetailLength"/> Unicode characters).
		/// </param>
		[
			Event(
				EventIds.ReceiveHttpResponseFailed,
				Message = "Failed to receive response for HTTP '{1}' request to '{0}' - {3}",
				Level = EventLevel.Error,
				Task = Tasks.ReceiveHttpResponse,
				Opcode = Opcodes.Fail,
				Version = 2
			)
		]
		public void ReceiveHttpResponseFailed(string requestUri, string method, string exceptionType, string exceptionMessage, string exceptionDetail)
		{
			WriteEvent(
				EventIds.ReceiveHttpResponseFailed,
				(requestUri ?? NoFurtherInformationAvailable).Truncate(MaximumRequestUriLength),
				method ?? Unknown,
				exceptionType ?? Unknown,
				(exceptionMessage ?? NoFurtherInformationAvailable),
				(exceptionDetail ?? NoFurtherInformationAvailable).Truncate(MaximumExceptionDetailLength)
			);
		}

		/// <summary>
		///		Event Ids for the Aperture Web API event-source.
		/// </summary>
		public static class EventIds
		{
			/// <summary>
			///		Sending an HTTP request.
			/// </summary>
			public const int SendingHttpRequest			= 100;

			/// <summary>
			///		Sent an HTTP request.
			/// </summary>
			public const int SentHttpRequest			= 101;

			/// <summary>
			///		Failed to send an HTTP request.
			/// </summary>
			public const int SendHttpRequestFailed		= 102;

			/// <summary>
			///		Receiving an HTTP request.
			/// </summary>
			public const int ReceivingHttpResponse		= 103;

			/// <summary>
			///		Received an HTTP request.
			/// </summary>
			public const int ReceivedHttpResponse		= 104;

			/// <summary>
			///		Failed to receive an HTTP request.
			/// </summary>
			public const int ReceiveHttpResponseFailed	= 105;
		}

		/// <summary>
		///		Event task constants.
		/// </summary>
		public static class Tasks
		{
			/// <summary>
			///		Process a request.
			/// </summary>
			public const EventTask SendHttpRequest		= (EventTask)100;

			/// <summary>
			///		Process a response.
			/// </summary>
			public const EventTask ReceiveHttpResponse	= (EventTask)101;
		}

		/// <summary>
		///		Event operation code constants.
		/// </summary>
		public static class Opcodes
		{
			/// <summary>
			///		The operation code representing the failure of a (logical, not TPL) task.
			/// </summary>
			public const EventOpcode Fail			= (EventOpcode)100;
		}
	}
}
