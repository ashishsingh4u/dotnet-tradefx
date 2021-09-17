// ===================================================================================
// <copyright file="WDigestProxyAuthenticator.cs" company="TechieNotes">
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
//    The WDigestProxyAuthenticator.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>The w digest proxy authenticator.</summary>
    internal class WDigestProxyAuthenticator : IProxyAuthenticator
    {
        #region Constants

        /// <summary>The package name.</summary>
        private const string PackageName = "WDIGEST";

        /// <summary>The request method.</summary>
        private const string RequestMethod = "CONNECT";

        /// <summary>The request uri.</summary>
        private const string RequestUri = "/";

        /// <summary>The return prefix.</summary>
        private const string ReturnPrefix = "Digest ";

        #endregion

        #region Fields

        /// <summary>The _credential.</summary>
        private readonly NetworkCredential _credential;

        /// <summary>The _sspi helper.</summary>
        private readonly SspiClientHelper _sspiHelper;

        /// <summary>The _is first call.</summary>
        private bool _isFirstCall;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="WDigestProxyAuthenticator"/> class. Initializes an instance of the BasicProxyAuthenticator class.</summary>
        /// <param name="credential">Credential to use for authentication.</param>
        public WDigestProxyAuthenticator(NetworkCredential credential)
        {
            this._credential = credential;
            this._sspiHelper = new SspiClientHelper(PackageName);
            this._isFirstCall = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is supported.</summary>
        public static bool IsSupported
        {
            get
            {
                return SspiClientHelper.IsSupported(PackageName);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Disposes the BasicProxyAuthenticator instance.
        /// </summary>
        public void Dispose()
        {
            this._sspiHelper.Dispose();
        }

        /// <summary>Gets an authentication token to return to the proxy server.</summary>
        /// <param name="proxyResponse">Response from the previous proxy request.</param>
        /// <returns>Authenticator token.</returns>
        public byte[] GetAuthenticatorToken(ProxyResponse proxyResponse)
        {
            // We should only need to call this method once to complete Digest proxy authentication.
            // Stale flag is not considered as we are never re-using an old nonce value.
            // Anything more can be considered a failure.
            if (!this._isFirstCall)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.AuthenticationFailed);
            }

            this._isFirstCall = false;

            // Parse header data into digest challenge object.
            DigestChallenge challenge = new DigestChallenge(proxyResponse);

            // Initialize the sspiHelper if not already initialized. Must wait until 
            // GetAuthenticatorToken to initialize as realm value is required.
            if (!this._sspiHelper.IsInitialized)
            {
                // This is slightly unfortunate, but the Domain value must equal the proxy realm
                // or authentication will fail. 
                if (this._credential != null)
                {
                    this._credential.Domain = challenge.Realm;
                }

                // Initial call to GetResponseToken generates partial context handle (essential)
                this._sspiHelper.Initialize(this._credential);
                this._sspiHelper.GetResponseToken(null);
            }

            // Get the in blob from the proxy challenge
            string inBlob = challenge.GetInBlob();

            // Get response token from SSPI API.
            byte[] token = this._sspiHelper.GetDigestResponseToken(inBlob, RequestMethod, RequestUri);

            // Copy the return prefix and token bytes into a byte array and return.
            // Token is generally in default proxy encoding, but username attribute may be in utf-8.
            // We therefore copy the bytes returned from SSPI directly to the return token.
            byte[] returnToken = new byte[ReturnPrefix.Length + token.Length];
            ProxyEncoding.Default.GetBytes(ReturnPrefix, 0, ReturnPrefix.Length, returnToken, 0);
            Buffer.BlockCopy(token, 0, returnToken, ReturnPrefix.Length, token.Length);

            return returnToken;
        }

        #endregion

        /// <summary>The digest challenge.</summary>
        private class DigestChallenge
        {
            #region Static Fields

            /// <summary>The attribute pattern.</summary>
            private static readonly Regex AttributePattern = new Regex(
                @"(\w*?)=""?([^""]*)""?", RegexOptions.Compiled | RegexOptions.Singleline);

            #endregion

            #region Fields

            /// <summary>The _algorithm.</summary>
            private readonly string _algorithm;

            /// <summary>The _charset.</summary>
            private readonly string _charset;

            /// <summary>The _nonce.</summary>
            private readonly string _nonce;

            /// <summary>The _opaque.</summary>
            private readonly string _opaque;

            /// <summary>The _qop.</summary>
            private readonly string _qop;

            /// <summary>The _realm.</summary>
            private readonly string _realm;

            /// <summary>The _stale.</summary>
            private readonly string _stale;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="DigestChallenge"/> class.</summary>
            /// <param name="proxyResponse">The proxy response.</param>
            public DigestChallenge(ProxyResponse proxyResponse)
            {
                foreach (string authHeader in proxyResponse.Headers.GetValues("Proxy-Authenticate"))
                {
                    Match attributeMatch = AttributePattern.Match(authHeader);
                    if (attributeMatch.Success)
                    {
                        string name = attributeMatch.Groups[1].Value.ToLowerInvariant();
                        string value = attributeMatch.Groups[2].Value;

                        // These are the only attributes that we are interested in.
                        switch (name)
                        {
                            case "realm":
                                this._realm = value;
                                break;

                            case "qop":
                                this._qop = value;
                                break;

                            case "nonce":
                                this._nonce = value;
                                break;

                            case "opaque":
                                this._opaque = value;
                                break;

                            case "algorithm":
                                this._algorithm = value;
                                break;

                            case "charset":
                                this._charset = value;
                                break;

                            case "stale":
                                this._stale = value;
                                break;
                        }
                    }
                }
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the realm.</summary>
            public string Realm
            {
                get
                {
                    return this._realm;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The get in blob.</summary>
            /// <returns>The System.String.</returns>
            public string GetInBlob()
            {
                StringBuilder stringBuilder = new StringBuilder();
                AddBlobAttribute(stringBuilder, "realm", this._realm, true);
                AddBlobAttribute(stringBuilder, "qop", this._qop, true);
                AddBlobAttribute(stringBuilder, "nonce", this._nonce, true);
                AddBlobAttribute(stringBuilder, "opaque", this._opaque, true);
                AddBlobAttribute(stringBuilder, "algorithm", this._algorithm, true);
                AddBlobAttribute(stringBuilder, "charset", this._charset, false);
                AddBlobAttribute(stringBuilder, "stale", this._stale, false);
                return stringBuilder.ToString();
            }

            #endregion

            #region Methods

            /// <summary>The add blob attribute.</summary>
            /// <param name="stringBuilder">The string builder.</param>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            /// <param name="isQuoted">The is quoted.</param>
            private static void AddBlobAttribute(StringBuilder stringBuilder, string name, string value, bool isQuoted)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(',');
                    }

                    stringBuilder.AppendFormat(isQuoted ? "{0}=\"{1}\"" : "{0}={1}", name, value);
                }
            }

            #endregion
        }
    }
}