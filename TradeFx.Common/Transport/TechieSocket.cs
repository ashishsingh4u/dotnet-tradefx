//  ===================================================================================
//  <copyright file="TechieSocket.cs" company="TechieNotes">
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
//     The TechieSocket.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

using TradeFx.Common.Helpers;
using TradeFx.Common.Transport.Network;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport
{
    /// <summary>The techie socket.</summary>
    public class TechieSocket : Connection
    {
        #region Fields

        /// <summary>The _packet assembler.</summary>
        private readonly PacketAssembler _packetAssembler;

        /// <summary>The _network connection.</summary>
        private NetworkConnection _networkConnection;

        /// <summary>The _remote certificate validation callback.</summary>
        private RemoteCertificateValidationCallback _remoteCertificateValidationCallback;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TechieSocket" /> class.
        /// </summary>
        public TechieSocket()
        {
            this.SetNetworkConnection(new NetworkConnection());
        }

        /// <summary>Initializes a new instance of the <see cref="TechieSocket"/> class.</summary>
        /// <param name="packetAssembler">The packet assembler.</param>
        /// <param name="connection">The connection.</param>
        public TechieSocket(PacketAssembler packetAssembler, NetworkConnection connection)
        {
            this._packetAssembler = packetAssembler;
            this.SetNetworkConnection(connection);
        }

        /// <summary>Initializes a new instance of the <see cref="TechieSocket"/> class.</summary>
        /// <param name="packetAssembler">The packet assembler.</param>
        public TechieSocket(PacketAssembler packetAssembler)
        {
            this._packetAssembler = packetAssembler;
            this.SetNetworkConnection(new NetworkConnection());
        }

        /// <summary>Initializes a new instance of the <see cref="TechieSocket"/> class.</summary>
        /// <param name="socketConnector">The socket connector.</param>
        public TechieSocket(ISocketConnector socketConnector)
        {
            this.SetNetworkConnection(new NetworkConnection(socketConnector));
        }

        /// <summary>Initializes a new instance of the <see cref="TechieSocket"/> class.</summary>
        /// <param name="socketConnector">The socket connector.</param>
        /// <param name="secure">The secure.</param>
        /// <param name="remoteCertificateValidationCallback">The remote certificate validation callback.</param>
        public TechieSocket(
            ISocketConnector socketConnector, 
            bool secure, 
            RemoteCertificateValidationCallback remoteCertificateValidationCallback)
        {
            this.Secure = secure;
            this._remoteCertificateValidationCallback = remoteCertificateValidationCallback;
            this.SetNetworkConnection(new NetworkConnection(socketConnector));
        }

        /// <summary>Initializes a new instance of the <see cref="TechieSocket"/> class.</summary>
        /// <param name="networkConnection">The network connection.</param>
        public TechieSocket(NetworkConnection networkConnection)
        {
            this.ConnectionAddress = string.Format("{0}:{1}", networkConnection.Address, networkConnection.Port);
            this.SetNetworkConnection(networkConnection);
        }

        #endregion

        #region Public Events

        /// <summary>The receive.</summary>
        public override event EventHandler<ReceiveEventArgs> Receive
        {
            add
            {
                this._networkConnection.Receive += value;
            }

            remove
            {
                this._networkConnection.Receive -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the connection status.</summary>
        public override ConnectionStatus ConnectionStatus
        {
            get
            {
                return this._networkConnection.Status;
            }
        }

        /// <summary>Gets or sets the remote certificate validation callback.</summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
        {
            get
            {
                return this._remoteCertificateValidationCallback;
            }

            set
            {
                this._remoteCertificateValidationCallback = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Create the end point from given connectionString string</summary>
        /// <param name="connectionString">Connection string in the format: [host]:[port]</param>
        /// <param name="endPoint">Return end point</param>
        /// <param name="error">Return error message</param>
        /// <returns>true on success, false otherwise</returns>
        public static bool CreateEndPoint(string connectionString, ref IPEndPoint endPoint, ref string error)
        {
            var ressult = false;

            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Empty connection string");
                }

                var connection = connectionString.Split(':');

                if (connection.Length < 2)
                {
                    throw new Exception("Invalid connection string: " + connectionString);
                }

                var host = connection[0];
                var port = connection[1];

                host = TranslateHostName(host);

                int portNumber;
                if (!Tools.ConvertStringToInt(port, out portNumber, ref error))
                {
                    throw new Exception("Invalid port specification: " + error);
                }

                var iPv4Address = Array.Find(
                    Dns.GetHostAddresses(host), x => (x.AddressFamily == AddressFamily.InterNetwork));
                if (iPv4Address == null)
                {
                    throw new Exception("Can't get host addresses for connection string: " + connectionString);
                }

                endPoint = new IPEndPoint(iPv4Address, portNumber);

                ressult = true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return ressult;
        }

        /// <summary>Create the end point from given address string</summary>
        /// <param name="address">Address string in the format: [host]:[port]</param>
        /// <returns>on success return <see cref="IPEndPoint"/></returns>
        public static IPEndPoint GetEndPoint(string address)
        {
            IPEndPoint listeningEndPoint = null;
            var error = string.Empty;
            if (!CreateEndPoint(address, ref listeningEndPoint, ref error))
            {
                throw new Exception(error);
            }

            return listeningEndPoint;
        }

        /// <summary>The connect.</summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Connect()
        {
            var addressPort = this.ConnectionAddress.Split(':');
            if (addressPort.Length != 2)
            {
                throw new InvalidOperationException("Invalid address format");
            }

            this._networkConnection.Secure = this.Secure;
            this._networkConnection.RemoteCertificateValidationCallback = this._remoteCertificateValidationCallback;

            // FIXME: Why is this hardcoded
            this._networkConnection.ServerCertificateName = "TEST.TechieNotes.COM";
            this._networkConnection.Address = TranslateHostName(addressPort[0]);
            this._networkConnection.Port = int.Parse(addressPort[1]);

            if (this._packetAssembler == null)
            {
                this._networkConnection.Open();
            }
            else
            {
                this._networkConnection.Open(this._packetAssembler);
            }
        }

        /// <summary>The disconnect.</summary>
        public override void Disconnect()
        {
            this._networkConnection.Close();
        }

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Boolean.</returns>
        public override bool Send(byte[] data, PacketType packetType)
        {
            return this._networkConnection.Send(data, packetType);
        }

        #endregion

        #region Methods

        /// <summary>Translate given host name, e.g. localhost</summary>
        /// <param name="hostName">Hostname to translate</param>
        /// <returns>Translated host name</returns>
        private static string TranslateHostName(string hostName)
        {
            var result = hostName;
            try
            {
                var ipHostEntry = hostName.ToUpper() == "LOCALHOST"
                                      ? Dns.GetHostEntry(Dns.GetHostName())
                                      : Dns.GetHostEntry(hostName);
                var iPv4Address = Array.Find(
                    ipHostEntry.AddressList, x => (x.AddressFamily == AddressFamily.InterNetwork));
                if (iPv4Address != null)
                {
                    result = iPv4Address.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, string.Format("Error translating hostname[{0}]: {1}", hostName, ex.Message));
            }

            return result;
        }

        /// <summary>The network connection_ send completed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NetworkConnection_SendCompleted(object sender, SendCompletedEventArgs e)
        {
            this.OnSendCompleted(e);
        }

        /// <summary>The network connection_ status changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NetworkConnection_StatusChanged(object sender, EventArgs e)
        {
            this.OnStatusChanged(e);
        }

        /// <summary>Set the network connection and subscribe for events</summary>
        /// <param name="networkConnection">Network connection to be set</param>
        /// <remarks>Receive handler is not added here because we have to wait until someone subscribes to the Receive event to prevent any initial data loss</remarks>
        private void SetNetworkConnection(NetworkConnection networkConnection)
        {
            this._networkConnection = networkConnection;
            this._networkConnection.StatusChanged += this.NetworkConnection_StatusChanged;
            this._networkConnection.SendCompleted += this.NetworkConnection_SendCompleted;
        }

        #endregion
    }
}