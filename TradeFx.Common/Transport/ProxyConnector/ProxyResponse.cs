// ===================================================================================
// <copyright file="ProxyResponse.cs" company="TechieNotes">
// ===================================================================================
//  TechieNotes Utilities & Best Practices
//  Samples and Guidelines for Winform & ASP.net development
// ===================================================================================
//  Copyright (c) TechieNotes.  All rights reserved.
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//  FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
// ===================================================================================
// </copyright>
// <author>Ashish Singh</author>
// <email>mailto:ashishsingh4u@gmail.com</email>
// <date>08-09-2012</date>
// <summary>
//    The ProxyResponse.cs file.
// </summary>
// ===================================================================================

using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    /// Parsed HTTP response from the proxy server.
    /// </summary>
    internal class ProxyResponse
    {
        #region Static Fields

        /// <summary>The http header pattern.</summary>
        private static readonly Regex HttpHeaderPattern = new Regex(
            @"^(.*?):\s(.*?)\r\n", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>The response split pattern.</summary>
        private static readonly Regex ResponseSplitPattern = new Regex(
            @"(^.*?\r\n\r\n)(.*$)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>The response status pattern.</summary>
        private static readonly Regex ResponseStatusPattern = new Regex(
            @"^HTTP/1.\d\s(\d{3})\s.*?\r\n", RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion

        #region Fields

        /// <summary>The _body.</summary>
        private readonly string _body;

        /// <summary>The _headers.</summary>
        private readonly WebHeaderCollection _headers;

        /// <summary>The _status.</summary>
        private readonly HttpStatusCode _status;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ProxyResponse"/> class.</summary>
        /// <param name="status">The status.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        private ProxyResponse(HttpStatusCode status, WebHeaderCollection headers, string body)
        {
            this._status = status;
            this._headers = headers;
            this._body = body;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the body of the proxy response.
        /// </summary>
        public string Body
        {
            get
            {
                return this._body ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the header collection received in the proxy response.
        /// </summary>
        /// <remarks>
        /// If no headers were in the response but this property is accessed, return 
        /// an empty collection to remove the need to do null checks elsewhere. 
        /// </remarks>
        public WebHeaderCollection Headers
        {
            get
            {
                return this._headers ?? new WebHeaderCollection();
            }
        }

        /// <summary>
        /// Gets the status of the proxy response.
        /// </summary>
        public HttpStatusCode Status
        {
            get
            {
                return this._status;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Parses the response from an HTTP request.</summary>
        /// <param name="responseString">String response from the HTTP request.</param>
        /// <returns>ProxyResponse object.</returns>
        public static ProxyResponse Parse(string responseString)
        {
            // Set default values
            HttpStatusCode status = 0;
            WebHeaderCollection headers = null;

            // Split the response string into metadata and body
            Match responseSplitMatch = ResponseSplitPattern.Match(responseString);
            string httpMetadata = responseSplitMatch.Groups[1].Value;
            string body = responseSplitMatch.Groups[2].Value;

            // First parse the status
            Match statusMatch = ResponseStatusPattern.Match(httpMetadata);
            if (statusMatch.Success)
            {
                int statusInt = int.Parse(statusMatch.Groups[1].Value, NumberFormatInfo.InvariantInfo);

                status = (HttpStatusCode)statusInt;
            }

            // Now parse any http headers
            MatchCollection headerMatches = HttpHeaderPattern.Matches(httpMetadata);
            if (headerMatches.Count != 0)
            {
                headers = new WebHeaderCollection();
                foreach (Match match in headerMatches)
                {
                    string name = match.Groups[1].Value;
                    string value = match.Groups[2].Value;

                    headers.Add(name, value);
                }
            }

            return new ProxyResponse(status, headers, body);
        }

        #endregion
    }
}