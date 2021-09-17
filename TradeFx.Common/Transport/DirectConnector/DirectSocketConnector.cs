// ===================================================================================
// <copyright file="DirectSocketConnector.cs" company="TechieNotes">
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
//    The DirectSocketConnector.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net.Sockets;

namespace TradeFx.Common.Transport.DirectConnector
{
    /// <summary>The direct socket connector.</summary>
    public class DirectSocketConnector : ISocketConnector
    {
        #region Fields

        /// <summary>The _address family.</summary>
        private AddressFamily _addressFamily;

        /// <summary>The _protocol type.</summary>
        private ProtocolType _protocolType;

        /// <summary>The _socket type.</summary>
        private SocketType _socketType;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DirectSocketConnector"/> class. 
        /// Initializes an instance of the DirectSocketConnector class.</summary>
        public DirectSocketConnector()
        {
            // Set default values for new socket.
            this.SetSocketDetails(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="host">IP address or host name of remote server.</param>
        /// <param name="port">Port of remote server.</param>
        /// <returns>Connected socket.</returns>
        public Socket GetConnectedSocket(string host, int port)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(this._addressFamily, this._socketType, this._protocolType);
                socket.Connect(host, port);
            }
            catch
            {
                try
                {
                    if (socket != null)
                    {
                        socket.Close(); // try not to leave open handles hanging around for some eventual finalization
                    }
                }
// ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
                {
                }

                throw; // rethrow original exception to maintain behaviour when we fail to connect
            }

            return socket;
        }

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="remoteUri">The remote Uri.</param>
        /// <returns>Connected socket.</returns>
        public Socket GetConnectedSocket(Uri remoteUri)
        {
            return this.GetConnectedSocket(remoteUri.Host, remoteUri.Port);
        }

        /// <summary>Allows overriding of the default socket values for new socket.</summary>
        /// <param name="addressFamily">One of the AddressFamily values.</param>
        /// <param name="socketType">One of the SocketType values.</param>
        /// <param name="protocolType">One of the ProtocolType values.</param>
        public void SetSocketDetails(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            this._addressFamily = addressFamily;
            this._socketType = socketType;
            this._protocolType = protocolType;
        }

        #endregion
    }
}