﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DDCloud.Platform.WebApi
{
	/// <summary>
	///		Represents an HTTP request builder.
	/// </summary>
	public interface IHttpRequestBuilder
	{
		/// <summary>
		///		The request URI.
		/// </summary>
		Uri RequestUri
		{
			get;
		}

		/// <summary>
		///		Does the request builder have an absolute URI?
		/// </summary>
		bool HasAbsoluteUri
		{
			get;
		}

		/// <summary>
		///		Is the request URI a template?
		/// </summary>
		bool IsTemplate
		{
			get;
		}

		/// <summary>
		///		The media-type formatters to use.
		/// </summary>
		IReadOnlyCollection<MediaTypeFormatter> MediaTypeFormatters
		{
			get;
		}
		
		/// <summary>
		///		Ensure that the <see cref="HttpRequestBuilder{TContext}"/> has an <see cref="UriKind.Absolute">absolute</see> <see cref="Uri">URI</see>.
		/// </summary>
		/// <returns>
		///		The request builder's absolute URI.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The request builder has a <see cref="UriKind.Relative">relative</see> <see cref="Uri">URI</see>.
		/// </exception>
		Uri EnsureAbsoluteUri();

		/// <summary>
		///		Build and configure a new HTTP request message.
		/// </summary>
		/// <param name="httpMethod">
		///		The HTTP request method to use.
		/// </param>
		/// <param name="body">
		///		Optional <see cref="HttpContent"/> representing the request body.
		/// </param>
		/// <param name="baseUri">
		///		An optional base URI to use if the request builder does not already have an absolute request URI.
		/// </param>
		/// <returns>
		///		The configured <see cref="HttpRequestMessage"/>.
		/// </returns>
		HttpRequestMessage BuildRequestMessage(HttpMethod httpMethod, HttpContent body = null, Uri baseUri = null);
	}

	/// <summary>
	///		Represents an HTTP request builder.
	/// </summary>
	/// <typeparam name="TContext">
	///		The type of object used by the request builder when resolving deferred template parameters.
	/// </typeparam>
	public interface IHttpRequestBuilder<TContext>
		: IHttpRequestBuilder
	{
		/// <summary>
		///		The URI template parameters (if any).
		/// </summary>
		IReadOnlyDictionary<string, Func<TContext, string>> TemplateParameters
		{
			get;
		}
		
		/// <summary>
		///		Build and configure a new HTTP request message.
		/// </summary>
		/// <param name="httpMethod">
		///		The HTTP request method to use.
		/// </param>
		/// <param name="context">
		///		The <typeparamref name="TContext"/> to use as the context for resolving any deferred template or query parameters.
		/// </param>
		/// <param name="body">
		///		Optional <see cref="HttpContent"/> representing the request body.
		/// </param>
		/// <param name="baseUri">
		///		An optional base URI to use if the request builder does not already have an absolute request URI.
		/// </param>
		/// <returns>
		///		The configured <see cref="HttpRequestMessage"/>.
		/// </returns>
		HttpRequestMessage BuildRequestMessage(HttpMethod httpMethod, TContext context, HttpContent body = null, Uri baseUri = null);
	}
}
