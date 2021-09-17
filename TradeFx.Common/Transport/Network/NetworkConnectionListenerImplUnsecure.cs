//  ===================================================================================
//  <copyright file="NetworkConnectionListenerImplUnsecure.cs" company="TechieNotes">
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
//     The NetworkConnectionListenerImplUnsecure.cs file.
//  </summary>
//  ===================================================================================

using System.Net;
using System.Net.Sockets;

using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection listener impl unsecure.</summary>
    /// <typeparam name="T"></typeparam>
    internal class NetworkConnectionListenerImplUnsecure<T> : NetworkConnectionListenerImpl
        where T : PacketAssembler, new()
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionListenerImplUnsecure{T}"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="listenBacklog">The listen backlog.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        public NetworkConnectionListenerImplUnsecure(string address, int port, int listenBacklog, int readBufferSize)
            : base(address, port, listenBacklog, readBufferSize)
        {
        }

        #endregion

        #region Methods

        /// <summary>The create connection.</summary>
        /// <param name="newSocket">The new socket.</param>
        protected override void CreateConnection(Socket newSocket)
        {
            SetNetConnectionSocketOptions(newSocket);
            var connectionImpl = new NetworkConnectionImplUnsecure(newSocket, this._readBufferSize, new T());
            var connection = new NetworkConnection(connectionImpl);
            var endPoint = (IPEndPoint)newSocket.RemoteEndPoint;
            connection.Address = endPoint.Address.ToString();
            connection.SetPort(endPoint.Port);
            this.OnConnect(new ConnectEventArgs(connection));
        }

        /// <summary>The get thread name.</summary>
        /// <returns>The System.String.</returns>
        protected override string GetThreadName()
        {
            return "Listener_Unsecure_" + this._address + "_" + this._port;
        }

        #endregion
    }
}