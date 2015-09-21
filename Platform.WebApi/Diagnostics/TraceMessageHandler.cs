using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DDCloud.Platform.WebApi.Diagnostics
{
	using Core.Collections;

	/// <summary>
	///		HTTP message handler that dumps requests and responses to a named trace source.
	/// </summary>
	public abstract class TraceMessageHandler
		: DelegatingHandler
	{
		/// <summary>
		///		Components of messages to be traced.
		/// </summary>
		TraceMessageComponents _enabledComponents;

		/// <summary>
		///		Create a new <see cref="TraceMessageHandler"/>.
		/// </summary>
		/// <param name="enabledComponents">
		///		Components of messages to be traced.
		/// </param>
		protected TraceMessageHandler(TraceMessageComponents enabledComponents)
		{
			_enabledComponents = enabledComponents;
		}

		/// <summary>
		///		Components of messages to be traced.
		/// </summary>
		public TraceMessageComponents EnabledComponents
		{
			get
			{
				return _enabledComponents;
			}
			set
			{
				_enabledComponents = value;
			}
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
		protected virtual void TraceBeginRequest(Guid requestId, Uri requestUri, string method, bool hasBody)
		{	
		}

		/// <summary>
		///		Trace request message headers.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="headers">
		///		A read-only list of key / value pairs representing the headers.
		/// </param>
		protected virtual void TraceRequestHeaders(Guid requestId, IReadOnlyList<KeyValuePair<string, IEnumerable<string>>> headers)
		{
		}

		/// <summary>
		///		Trace request message headers.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="body">
		///		The request body.
		/// </param>
		protected virtual void TraceRequestBody(Guid requestId, string body)
		{
		}

		/// <summary>
		///		Trace response message headers.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="headers">
		///		A read-only list of key / value pairs representing the headers.
		/// </param>
		protected virtual void TraceResponseHeaders(Guid requestId, IReadOnlyList<KeyValuePair<string, IEnumerable<string>>> headers)
		{
		}

		/// <summary>
		///		Trace response message headers.
		/// </summary>
		/// <param name="requestId">
		///		A unique Id for the request.
		/// </param>
		/// <param name="body">
		///		The response body.
		/// </param>
		protected virtual void TraceResponseBody(Guid requestId, string body)
		{
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
		protected virtual void TraceRequestError(Guid requestId, Uri requestUri, string method, Exception exception)
		{
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
		protected virtual void TraceEndRequest(Guid requestId, Uri requestUri, string method, HttpStatusCode responseStatusCode)
		{
		}

		/// <summary>
		///		Process an HTTP request.
		/// </summary>
		/// <param name="request">
		///		An <see cref="HttpRequestMessage"/> representing the request.
		/// </param>
		/// <param name="cancellationToken">
		///		A cancellation token that can be used to cancel message processing.
		/// </param>
		/// <returns>
		///		An <see cref="HttpResponseMessage"/> representing the response.
		/// </returns>
		protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Guid requestId;
			if (!request.Properties.TryCastValue("MS_RequestId", out requestId))
				requestId = Guid.NewGuid();

			TraceBeginRequest(
				requestId,
				request.RequestUri,
				request.Method.ToString(),
				hasBody: request.Content != null
			);

			if (ShouldTrace(TraceMessageComponents.Request))
			{
				if (ShouldTrace(TraceMessageComponents.RequestHeaders))
				{
					TraceRequestHeaders(
						requestId,
						request.Headers.ToArray()
					);
				}

				if (ShouldTrace(TraceMessageComponents.RequestBody))
				{
					if (request.Content != null)
					{
						string content = await request.Content.ReadAsStringAsync();
						TraceRequestBody(
							requestId,
							content
						);
					}
					else
						TraceRequestBody(requestId, String.Empty);
				}
			}

			HttpResponseMessage response = null;
			try
			{
				response = await base.SendAsync(request, cancellationToken);
				Debug.Assert(response != null, "response != null");

				if (ShouldTrace(TraceMessageComponents.Response))
				{
					if (ShouldTrace(TraceMessageComponents.ResponseHeaders))
					{
						TraceResponseHeaders(
							requestId,
							response.Headers.ToArray()
						);
					}

					if (ShouldTrace(TraceMessageComponents.ResponseBody))
					{
						if (response.Content != null)
						{
							string content = await response.Content.ReadAsStringAsync();
							TraceResponseBody(
								requestId,
								content
							);
						}
						else
							TraceResponseBody(requestId, String.Empty);
					}
				}
			}
			catch (Exception eRequest)
			{
				using (response)
				{
					AggregateException aggregateException = eRequest as AggregateException;
					if (aggregateException != null)
					{
						// Unpack inner exceptions.
						aggregateException.Flatten().Handle(
							innerException =>
							{
								TraceRequestError(
									requestId,
									request.RequestUri,
									request.Method.ToString(),
									innerException
								);

								return true; // We're going to re-throw anyway.
							}
						);
					}
					else
					{
						TraceRequestError(
							requestId,
							request.RequestUri,
							request.Method.ToString(),
							eRequest
						);
					}

					throw;
				}
			}
			finally
			{
				TraceEndRequest(
					requestId,
					request.RequestUri,
					request.Method.ToString(),
					response != null ? response.StatusCode : HttpStatusCode.Unused
				);
			}

			return response;
		}

		/// <summary>
		///		Determine whether the specified message component(s) be traced.
		/// </summary>
		/// <param name="messageComponent">
		///		One or more <see cref="TraceMessageComponents"/> flags indicating the message component(s).
		/// </param>
		/// <returns>
		///		<c>true</c>, if the message component(s) should be traced; otherwise, <c>false</c>.
		/// </returns>
		protected bool ShouldTrace(TraceMessageComponents messageComponent)
		{
			switch (_enabledComponents)
			{
				case TraceMessageComponents.None:
				{
					return false;
				}
				case TraceMessageComponents.All:
				{
					return true;
				}
				default:
				{
					return (_enabledComponents & messageComponent) != 0;
				}
			}
		}
	}
}
