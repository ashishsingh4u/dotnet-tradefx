//  ===================================================================================
//  <copyright file="NetworkConnectionListenerImplSecure.cs" company="TechieNotes">
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
//     The NetworkConnectionListenerImplSecure.cs file.
//  </summary>
//  ===================================================================================

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection listener impl secure.</summary>
    /// <typeparam name="T"></typeparam>
    internal class NetworkConnectionListenerImplSecure<T> : NetworkConnectionListenerImpl
        where T : PacketAssembler, new()
    {
        #region Fields

        /// <summary>The _certificate.</summary>
        private readonly X509Certificate _certificate;

        /// <summary>The _remote callback.</summary>
        private readonly RemoteCertificateValidationCallback _remoteCallback;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionListenerImplSecure{T}"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="listenBacklog">The listen backlog.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="remoteCallback">The remote callback.</param>
        public NetworkConnectionListenerImplSecure(
            string address, 
            int port, 
            int listenBacklog, 
            int readBufferSize, 
            X509Certificate certificate, 
            RemoteCertificateValidationCallback remoteCallback)
            : base(address, port, listenBacklog, readBufferSize)
        {
            this._certificate = certificate;
            this._remoteCallback = remoteCallback;
        }

        #endregion

        #region Methods

        /// <summary>The create connection.</summary>
        /// <param name="newSocket">The new socket.</param>
        protected override void CreateConnection(Socket newSocket)
        {
            SetNetConnectionSocketOptions(newSocket);
            var connectionImpl = new NetworkConnectionImplSecure(
                newSocket, this._remoteCallback, this._readBufferSize, new T());
            connectionImpl.AuthenticateAsServer(this._certificate);
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
            return "Listener_Secure_" + this._address + "_" + this._port;
        }

        #endregion
    }
}