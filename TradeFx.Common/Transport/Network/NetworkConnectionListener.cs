//  ===================================================================================
//  <copyright file="NetworkConnectionListener.cs" company="TechieNotes">
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
//     The NetworkConnectionListener.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using TradeFx.Common.Events;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>
    ///     Listens for incoming connections from the network
    /// </summary>
    public class NetworkConnectionListener
    {
        #region Constants

        /// <summary>
        ///     Default maximum length of the pending connections queue
        /// </summary>
        private const int DefaultListenBacklog = 50;

        /// <summary>
        ///     Default port value
        /// </summary>
        private const int DefaultPort = 5000;

        /// <summary>
        ///     Read buffer size
        /// </summary>
        private const int ReadBufferSize = 5000;

        #endregion

        #region Fields

        /// <summary>
        ///     Private sync object for locking
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        ///     An optional address to listen on
        /// </summary>
        private string _address;

        /// <summary>The _certificate.</summary>
        private X509Certificate _certificate;

        /// <summary>
        ///     The maximum length of the pending connections queue
        /// </summary>
        private int _listenBacklog = DefaultListenBacklog;

        /// <summary>The _listener.</summary>
        private NetworkConnectionListenerImpl _listener;

        /// <summary>
        ///     Listening port
        /// </summary>
        private int _port = DefaultPort;

        /// <summary>The _secure.</summary>
        private bool _secure;

        #endregion

        #region Public Events

        /// <summary>The connect.</summary>
        public event EventHandler<ConnectEventArgs> Connect;

        /// <summary>The error.</summary>
        public event EventHandler<ErrorEventArgs> Error;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the address.</summary>
        [Category("Behavior")]
        [Description("The local network address to listen on. If null all local addresses will be bound")]
        [DefaultValue(null)]
        public string Address
        {
            get
            {
                return this._address;
            }

            set
            {
                this._address = value;
            }
        }

        /// <summary>Gets or sets the certificate.</summary>
        public X509Certificate Certificate
        {
            get
            {
                return this._certificate;
            }

            set
            {
                this._certificate = value;
            }
        }

        /// <summary>Gets or sets the listen backlog.</summary>
        [Category("Behavior")]
        [Description("The maximum length of the pending connections queue")]
        [DefaultValue(DefaultListenBacklog)]
        public int ListenBacklog
        {
            get
            {
                return this._listenBacklog;
            }

            set
            {
                this._listenBacklog = value;
            }
        }

        /// <summary>Gets or sets the port.</summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Category("Behavior")]
        [Description("The port to be used to connect to the server")]
        [DefaultValue(DefaultPort)]
        public int Port
        {
            get
            {
                return this._port;
            }

            set
            {
                // Actually its really an unsigned short CLS compliance and Socket
                // API mean int makes sense. 0 is not allowed either
                if (value < 1 || value > IPEndPoint.MaxPort)
                {
                    throw new ArgumentOutOfRangeException("value", "Not a valid Port value");
                }

                this._port = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether secure.</summary>
        public bool Secure
        {
            get
            {
                return this._secure;
            }

            set
            {
                this._secure = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        public void Close()
        {
            lock (this._sync)
            {
                if (this._listener != null)
                {
                    this._listener.Close();
                    this._listener = null;
                }
            }
        }

        /// <summary>
        ///     Function using the default LengthPreFixedPackAssembler.
        /// </summary>
        public void Open()
        {
            this.Open<LengthPreFixedPackAssembler>();
        }

        /// <summary>The open.</summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open<T>() where T : PacketAssembler, new()
        {
            if (this.Secure && this._certificate == null)
            {
                throw new InvalidOperationException("Missing Certificate");
            }

            lock (this._sync)
            {
                if (string.IsNullOrEmpty(this._address))
                {
                    throw new InvalidOperationException("Address cannot be empty string");
                }

                if (this._secure)
                {
                    this._listener = new NetworkConnectionListenerImplSecure<T>(
                        this._address, this._port, this._listenBacklog, ReadBufferSize, this._certificate, null);
                }
                else
                {
                    this._listener = new NetworkConnectionListenerImplUnsecure<T>(
                        this._address, this._port, this._listenBacklog, ReadBufferSize);
                }

                this._listener.Connect += (source, e) => this.OnConnect(e);
                this._listener.Error += (source, e) => this.OnError(e);
                this._listener.Start();
            }
        }

        #endregion

        #region Methods

        /// <summary>The on connect.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnConnect(ConnectEventArgs e)
        {
            EventPublisher.RaiseEvent(this.Connect, this, e);
        }

        /// <summary>The on error.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnError(ErrorEventArgs e)
        {
            EventPublisher.RaiseEvent(this.Error, this, e);
        }

        #endregion
    }

    /// <summary>
    ///     Connect event data
    /// </summary>
    public class ConnectEventArgs : EventArgs
    {
        #region Fields

        /// <summary>The _connection.</summary>
        private readonly NetworkConnection _connection;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ConnectEventArgs"/> class.</summary>
        /// <param name="connection">The connection.</param>
        public ConnectEventArgs(NetworkConnection connection)
        {
            this._connection = connection;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the connection.</summary>
        public NetworkConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        #endregion
    }
}