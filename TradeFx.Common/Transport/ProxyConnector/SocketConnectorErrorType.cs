//  ===================================================================================
//  <copyright file="SocketConnectorErrorType.cs" company="TechieNotes">
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
//     The SocketConnectorErrorType.cs file.
//  </summary>
//  ===================================================================================
namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    ///     Enumeration of proxy error types.
    /// </summary>
    public enum SocketConnectorErrorType
    {
        /// <summary>The unexpected error.</summary>
        UnexpectedError, 

        /// <summary>The socket connect error.</summary>
        SocketConnectError, 

        /// <summary>The socket close error.</summary>
        SocketCloseError, 

        /// <summary>The socket send error.</summary>
        SocketSendError, 

        /// <summary>The socket receive error.</summary>
        SocketReceiveError, 

        /// <summary>The protocol negotiation failed.</summary>
        ProtocolNegotiationFailed, 

        /// <summary>The unsupported authentication type.</summary>
        UnsupportedAuthenticationType, 

        /// <summary>The authentication credential required.</summary>
        AuthenticationCredentialRequired, 

        /// <summary>The authentication failed.</summary>
        AuthenticationFailed, 

        /// <summary>The sspi error.</summary>
        SspiError, 

        /// <summary>The remote server not available.</summary>
        RemoteServerNotAvailable
    }
}