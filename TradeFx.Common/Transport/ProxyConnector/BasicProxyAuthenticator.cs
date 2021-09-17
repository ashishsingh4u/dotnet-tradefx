// ===================================================================================
// <copyright file="BasicProxyAuthenticator.cs" company="TechieNotes">
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
//    The BasicProxyAuthenticator.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    /// Provides authentication according to the Basic HTTP protocol.
    /// </summary>
    internal class BasicProxyAuthenticator : IProxyAuthenticator
    {
        #region Constants

        /// <summary>The return prefix.</summary>
        private const string ReturnPrefix = "Basic ";

        #endregion

        #region Fields

        /// <summary>The _credential.</summary>
        private readonly NetworkCredential _credential;

        /// <summary>The _is first call.</summary>
        private bool _isFirstCall;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BasicProxyAuthenticator"/> class. Initializes an instance of the BasicProxyAuthenticator class.</summary>
        /// <param name="credential">Credential to use for authentication.</param>
        public BasicProxyAuthenticator(NetworkCredential credential)
        {
            // As credentials are required for basic authentication, ensure that they have been supplied
            if (credential == null)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.AuthenticationCredentialRequired);
            }

            this._credential = credential;
            this._isFirstCall = true;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Disposes the BasicProxyAuthenticator instance.
        /// </summary>
        public void Dispose()
        {
            // No action to take here
        }

        /// <summary>Gets an authentication token to return to the proxy server.</summary>
        /// <param name="proxyResponse">Response from the previous proxy request.</param>
        /// <returns>Authenticator token.</returns>
        public byte[] GetAuthenticatorToken(ProxyResponse proxyResponse)
        {
            // We should only need to call this method once to complete Basic proxy authentication.
            // Anything more can be considered a failure.
            if (!this._isFirstCall)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.AuthenticationFailed);
            }

            this._isFirstCall = false;

            // Generate the token string.
            string tokenString = this._credential.UserName + ':' + this._credential.Password;

            // Encode the token as Base64 string.
            string base64TokenString = Convert.ToBase64String(ProxyEncoding.Default.GetBytes(tokenString));

            // Copy the return prefix and token bytes into a byte array and return.
            // Regardless of encoding, base64 string will result in one byte per char (as will return prefix).
            byte[] returnToken = new byte[ReturnPrefix.Length + base64TokenString.Length];
            ProxyEncoding.Default.GetBytes(ReturnPrefix, 0, ReturnPrefix.Length, returnToken, 0);
            ProxyEncoding.Default.GetBytes(
                base64TokenString, 0, base64TokenString.Length, returnToken, ReturnPrefix.Length);

            return returnToken;
        }

        #endregion
    }
}