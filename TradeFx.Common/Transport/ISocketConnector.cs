// ===================================================================================
// <copyright file="ISocketConnector.cs" company="TechieNotes">
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
//    The ISocketConnector.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net.Sockets;

namespace TradeFx.Common.Transport
{
    /// <summary>
    /// Interface for SocketConnector types.
    /// </summary>
    public interface ISocketConnector
    {
        #region Public Methods and Operators

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="host">IP address or host name of remote server.</param>
        /// <param name="port">Port of remote server.</param>
        /// <returns>Connected socket.</returns>
        Socket GetConnectedSocket(string host, int port);

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="remoteUri">The remote uri to connect to.</param>
        /// <returns>Connected socket.</returns>
        Socket GetConnectedSocket(Uri remoteUri);

        /// <summary>Allows overriding of the default socket values for new socket.</summary>
        /// <param name="addressFamily">One of the AddressFamily values.</param>
        /// <param name="socketType">One of the SocketType values.</param>
        /// <param name="protocolType">One of the ProtocolType values.</param>
        void SetSocketDetails(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType);

        #endregion
    }
}