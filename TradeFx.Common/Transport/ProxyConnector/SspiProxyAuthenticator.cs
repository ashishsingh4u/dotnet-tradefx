//  ===================================================================================
//  <copyright file="SspiProxyAuthenticator.cs" company="TechieNotes">
//  ===================================================================================
//   TechieNotes Utilities & Best Practices
//   Samples and Guidelines for Winform & ASP.net development
//  ===================================================================================
//   Copyright (c) TechieNotes.  All rights reserved.
//   THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//   OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//   LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//   FITNESS FOR A PARTICULAR PURPOSE.
//  ===================================================================================
//   The example companies, organizations, products, domain names,
//   e-mail addresses, logos, people, places, and events depicted
//   herein are fictitious.  No association with any real company,
//   organization, product, domain name, email address, logo, person,
//   places, or events is intended or should be inferred.
//  ===================================================================================
//  </copyright>
//  <author>ASHISHSINGH</author>
//  <email>mailto:ashishsingh4u@gmail.com</email>
//  <date>11-03-2013</date>
//  <summary>
//     The SspiProxyAuthenticator.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>The sspi proxy authenticator.</summary>
    internal class SspiProxyAuthenticator : IProxyAuthenticator
    {
        #region Constants

        /// <summary>The max iteration count.</summary>
        private const int MaxIterationCount = 5;

        #endregion

        #region Fields

        /// <summary>The _package name.</summary>
        private readonly string _packageName;

        /// <summary>The _sspi helper.</summary>
        private readonly SspiClientHelper _sspiHelper;

        /// <summary>The _iteration count.</summary>
        private int _iterationCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SspiProxyAuthenticator"/> class. Initializes an instance of the SspiProxyAuthenticator class.</summary>
        /// <param name="packageName">Supported packages are NTLM, Kerberos and Negotiate.</param>
        /// <param name="credential">Credential to use for authentication.</param>
        public SspiProxyAuthenticator(string packageName, NetworkCredential credential)
        {
            this._packageName = packageName;
            this._sspiHelper = new SspiClientHelper(packageName);
            this._sspiHelper.Initialize(credential);
            this._iterationCount = 0;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets a value indicating if the named package is supported.</summary>
        /// <param name="packageName">Name of the package to check.</param>
        /// <returns>True if supported, false if not supported.</returns>
        public static bool IsSupported(string packageName)
        {
            return SspiClientHelper.IsSupported(packageName);
        }

        /// <summary>
        ///     Disposes of the SspiProxyAuthenticator instance.
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
            // Maximum of MaxItertationCount (5) iterations before we can consider the handshake a failure
            if (++this._iterationCount > MaxIterationCount)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.AuthenticationFailed);
            }

            // Return prefix is packagename followed by space
            var returnPrefix = this._packageName + ' ';

            // Get the proxy challenge (if one has been set).
            var serverChallenge = this.GetServerChallenge(proxyResponse);

            // Get the response token from SSPI API as Base64 string.
            var base64TokenString = Convert.ToBase64String(this._sspiHelper.GetResponseToken(serverChallenge));

            // Copy the return prefix and token bytes into a byte array and return.
            // Regardless of encoding, base64 string will result in one byte per char (as will return prefix).
            var returnToken = new byte[returnPrefix.Length + base64TokenString.Length];
            ProxyEncoding.Default.GetBytes(returnPrefix, 0, returnPrefix.Length, returnToken, 0);
            ProxyEncoding.Default.GetBytes(
                base64TokenString, 0, base64TokenString.Length, returnToken, returnPrefix.Length);

            return returnToken;
        }

        #endregion

        #region Methods

        /// <summary>The get server challenge.</summary>
        /// <param name="proxyResponse">The proxy response.</param>
        /// <returns>The System.Byte[].</returns>
        private byte[] GetServerChallenge(ProxyResponse proxyResponse)
        {
            foreach (var authHeader in proxyResponse.Headers.GetValues("Proxy-Authenticate"))
            {
                if (authHeader.StartsWith(this._packageName, StringComparison.OrdinalIgnoreCase)
                    && authHeader.Length > this._packageName.Length)
                {
                    return Convert.FromBase64String(authHeader.Substring(this._packageName.Length + 1));
                }
            }

            return null;
        }

        #endregion
    }
}