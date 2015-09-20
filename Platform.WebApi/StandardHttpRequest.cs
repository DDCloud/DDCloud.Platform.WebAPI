﻿using System;
using System.Net.Http.Formatting;

namespace DDCloud.Platform.WebApi
{
	/// <summary>
	///		Standard <see cref="HttpRequestBuilder"/>s.
	/// </summary>
	/// <remarks>
	///		The same request builder instance is always returned for a given base URI.
	/// </remarks>
	public static class StandardHttpRequest
	{
		/// <summary>
		///		Create a standard HTTP request builder that uses JSON.
		/// </summary>
		/// <param name="baseUri">
		///		The base URI to use.
		/// </param>
		/// <param name="jsonFormatter">
		///		A specific <see cref="JsonMediaTypeFormatter"/> to use (otherwise, a new <see cref="JsonMediaTypeFormatter"/> will be used).
		/// </param>
		/// <returns>
		///		The new <see cref="HttpRequestBuilder{TContext}"/>.
		/// </returns>
		public static HttpRequestBuilder<Unit> Json(Uri baseUri, JsonMediaTypeFormatter jsonFormatter = null)
		{
			if (baseUri == null)
				throw new ArgumentNullException("baseUri");

			return Json<Unit>(baseUri, jsonFormatter);
		}

		/// <summary>
		///		Create a standard HTTP request builder that uses JSON.
		/// </summary>
		/// <typeparam name="TContext">
		///		The type of object used by the request builder when resolving deferred template parameters.
		/// </typeparam>
		/// <param name="baseUri">
		///		The base URI to use.
		/// </param>
		/// <param name="jsonFormatter">
		///		A specific <see cref="JsonMediaTypeFormatter"/> to use (otherwise, a new <see cref="JsonMediaTypeFormatter"/> will be used).
		/// </param>
		/// <returns>
		///		The new <see cref="HttpRequestBuilder{TContext}"/>.
		/// </returns>
		public static HttpRequestBuilder<TContext> Json<TContext>(Uri baseUri, JsonMediaTypeFormatter jsonFormatter = null)
		{
			if (baseUri == null)
				throw new ArgumentNullException("baseUri");

			return HttpRequestBuilder.Create<TContext>(baseUri).UseJson(jsonFormatter);
		}

		/// <summary>
		///		Create a standard HTTP request builder that uses XML.
		/// </summary>
		/// <param name="baseUri">
		///		The base URI to use.
		/// </param>
		/// <param name="useXmlSerializer">
		///		Use the XML serialiser instead of the data-contract serialiser?
		/// </param>
		/// <returns>
		///		The new <see cref="HttpRequestBuilder{TContext}"/>.
		/// </returns>
		public static HttpRequestBuilder<Unit> Xml(Uri baseUri, bool useXmlSerializer = false)
		{
			return Xml<Unit>(baseUri, useXmlSerializer);
		}

		/// <summary>
		///		Create a standard HTTP request builder that uses XML.
		/// </summary>
		/// <typeparam name="TContext">
		///		The type of object used by the request builder when resolving deferred template parameters.
		/// </typeparam>
		/// <param name="baseUri">
		///		The base URI to use.
		/// </param>
		/// <param name="useXmlSerializer">
		///		Use the XML serialiser instead of the data-contract serialiser?
		/// </param>
		/// <returns>
		///		The new <see cref="HttpRequestBuilder{TContext}"/>.
		/// </returns>
		public static HttpRequestBuilder<TContext> Xml<TContext>(Uri baseUri, bool useXmlSerializer = false)
		{
			if (baseUri == null)
				throw new ArgumentNullException("baseUri");

			return HttpRequestBuilder.Create<TContext>(baseUri).UseXml(
				new XmlMediaTypeFormatter
				{
					UseXmlSerializer = useXmlSerializer
				}
			);
		}
	}
}
