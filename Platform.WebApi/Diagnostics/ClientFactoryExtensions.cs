using System;

namespace DDCloud.Platform.WebApi.Diagnostics
{
	/// <summary>
	///		Diagnostics extensions for <see cref="HttpClientBuilder"/>.
	/// </summary>
	/// <remarks>
	///		Be aware that, if you return singleton instances of message handlers from factory delegates, those handlers will be disposed if the factory encounters any exception while creating a client.
	/// </remarks>
	public static class ClientFactoryExtensions
	{
		/// <summary>
		///		Enable activity-correlation support for clients created by the factory.
		/// </summary>
		/// <param name="clientBuilder">
		///		The HTTP client builder.
		/// </param>
		/// <returns>
		///		The client builder (enables inline use / method chaining).
		/// </returns>
		public static HttpClientBuilder WithActivityCorrelation(this HttpClientBuilder clientBuilder)
		{
			if (clientBuilder == null)
				throw new ArgumentNullException(nameof(clientBuilder));

			return clientBuilder.WithMessageHandler<ActivityCorrelationClientMessageHandler>();
		}

		/// <summary>
		///		Trace requests / responses to the platform WebAPI event-source.
		/// </summary>
		/// <param name="clientBuilder">
		///		The HTTP client builder.
		/// </param>
		/// <param name="messageComponents">
		///		An optional combination of <see cref="TraceMessageComponents"/> flags indicating the message components to trace.
		/// 
		///		Defaults to <see cref="TraceMessageComponents.All"/>.
		/// </param>
		/// <returns>
		///		The client builder (enables inline use / method chaining).
		/// </returns>
		public static HttpClientBuilder WithTracingToEtw(this HttpClientBuilder clientBuilder, TraceMessageComponents messageComponents = TraceMessageComponents.All)
		{
			if (clientBuilder == null)
				throw new ArgumentNullException(nameof(clientBuilder));

			return clientBuilder.AddHandlerAfter<EtwTraceMessageHandler, ActivityCorrelationClientMessageHandler>(
				() => new EtwTraceMessageHandler(messageComponents)
			);
		}
	}
}
