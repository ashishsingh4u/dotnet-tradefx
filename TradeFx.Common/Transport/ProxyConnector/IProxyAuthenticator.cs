// ===================================================================================
// <copyright file="IProxyAuthenticator.cs" company="TechieNotes">
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
//    The IProxyAuthenticator.cs file.
// </summary>
// ===================================================================================

using System;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    /// Interface for all proxy authenticators.
    /// </summary>
    internal interface IProxyAuthenticator : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>Gets an authentication token to return to the proxy server.</summary>
        /// <param name="proxyResponse">Response from the previous proxy request.</param>
        /// <returns>Authenticator token.</returns>
        byte[] GetAuthenticatorToken(ProxyResponse proxyResponse);

        #endregion
    }
}