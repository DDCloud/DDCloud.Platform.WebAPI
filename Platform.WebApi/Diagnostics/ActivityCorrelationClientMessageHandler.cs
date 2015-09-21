using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DDCloud.Platform.WebApi.Diagnostics
{
	using Core;

	/// <summary>
	///		Client-side delegating message handler to handle transfer of platform activity-correlation header with ASP.NET Web API requests.
	/// </summary>
	public sealed class ActivityCorrelationClientMessageHandler
		: DelegatingHandler
	{
		/// <summary>
		///		Create a new platform client-side activity correlation message handler.
		/// </summary>
		public ActivityCorrelationClientMessageHandler()
		{
		}

		/// <summary>
		///		Create a new platform client-side activity correlation message handler.
		/// </summary>
		/// <param name="nextHandler">
		///		The next message handler in the pipeline.
		/// </param>
		public ActivityCorrelationClientMessageHandler(HttpMessageHandler nextHandler)
			: base(nextHandler)
		{
		}

		/// <summary>
		///		Handle a request message before / after it is processed by the next handler in the pipeline.
		/// </summary>
		/// <param name="request">
		///		The request message.
		/// </param>
		/// <param name="cancellationToken">
		///		A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		///		A <see cref="Task{TResult}"/> representing the asynchronous operation, whose result is the response message.
		/// </returns>
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Guid? currentActivityId = ActivityCorrelationManager.GetCurrentActivityId();

			if (currentActivityId.HasValue)
			{
				if (request.Headers.Contains(ActivityCorrelationManager.ActivityIdHeaderName))
					request.Headers.Remove(ActivityCorrelationManager.ActivityIdHeaderName);

				request.Headers.Add(
					ActivityCorrelationManager.ActivityIdHeaderName,
					currentActivityId.Value.ToString()
				);
			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}
