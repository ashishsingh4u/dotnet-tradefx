//  ===================================================================================
//  <copyright file="NetworkConnection.cs" company="TechieNotes">
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
//     The NetworkConnection.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using TradeFx.Common.Culture;
using TradeFx.Common.Events;
using TradeFx.Common.Transport.DirectConnector;
using TradeFx.Common.Transport.Packet;

using log4net;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection.</summary>
    public class NetworkConnection
    {
        #region Constants

        /// <summary>
        ///     Default port value
        /// </summary>
        private const int DefaultPort = 5000;

        /// <summary>
        ///     Default receive buffer size
        /// </summary>
        private const int DefaultReceiveBufferSize = 1000 * 1024;

        #endregion

        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        /// <summary>The _pad lock.</summary>
        private readonly object _padLock = new object();

        /// <summary>
        ///     The socket connector.
        /// </summary>
        private readonly ISocketConnector _socketConnector;

        /// <summary>The _status lock.</summary>
        private readonly object _statusLock = new object();

        /// <summary>
        ///     The connection.
        /// </summary>
        private NetworkConnectionImpl _connection;

        /// <summary>The _on status changed.</summary>
        private volatile bool _onStatusChanged;

        /// <summary>
        ///     The port at the remote end
        /// </summary>
        private int _port = DefaultPort;

        /// <summary>
        ///     Size of buffer used to extract data from socket
        /// </summary>
        private int _receiveBufferSize = DefaultReceiveBufferSize;

        /// <summary>The _remote certificate validation callback.</summary>
        private RemoteCertificateValidationCallback _remoteCertificateValidationCallback;

        /// <summary>The _secure.</summary>
        private bool _secure;

        /// <summary>The _server certificate name.</summary>
        private string _serverCertificateName;

        /// <summary>
        ///     the Status of the NetworkConnection
        /// </summary>
        private volatile ConnectionStatus _status = ConnectionStatus.Closed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkConnection" /> class.
        /// </summary>
        public NetworkConnection()
            : this((ISocketConnector)new DirectSocketConnector())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkConnection"/> class.</summary>
        /// <param name="socketConnector">The socket connector.</param>
        public NetworkConnection(ISocketConnector socketConnector)
        {
            this._socketConnector = socketConnector;
            this._status = ConnectionStatus.Closed;
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkConnection"/> class. Internal constructor</summary>
        /// <remarks>This constructor exists only for the NetworkConnectionListener class</remarks>
        /// <param name="connection">an already initialized network stream</param>
        internal NetworkConnection(NetworkConnectionImpl connection)
        {
            this._secure = connection.IsSecure;
            this._connection = connection;
            this._status = ConnectionStatus.Open;
            this.HookupConnectionEvents();
        }

        #endregion

        #region Public Events

        /// <summary>The receive.</summary>
        public event EventHandler<ReceiveEventArgs> Receive
        {
            // Wrap this event so that it causes Reads to start from
            // the underlying connection in case they are being held off
            add
            {
                this._receive += value;
                lock (this._statusLock)
                {
                    if (this._connection != null)
                    {
                        this._connection.BeginReceive();
                    }
                }
            }

            remove
            {
                this._receive -= value;
            }
        }

        /// <summary>The send completed.</summary>
        public event EventHandler<SendCompletedEventArgs> SendCompleted;

        /// <summary>The status changed.</summary>
        public event EventHandler<EventArgs> StatusChanged;

        #endregion

        #region Events

        /// <summary>The _receive.</summary>
        private event EventHandler<ReceiveEventArgs> _receive;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the address.</summary>
        [Category("Behavior")]
        [Description("The Address of the remote connection")]
        [DefaultValue("")]
        public string Address { get; set; }

        /// <summary>Gets or sets the port.</summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Category("Behavior")]
        [Description("The port of the remote connection")]
        [DefaultValue(DefaultPort)]
        public int Port
        {
            get
            {
                return this._port;
            }

            set
            {
                // Actually its really an unsigned short; CLS compliance and Socket
                // API mean int makes sense.
                // 0 - is not allowed
                if (value < 1 || value > IPEndPoint.MaxPort)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this._port = value;
            }
        }

        /// <summary>Gets or sets the receive buffer size.</summary>
        [Category("Behavior")]
        [Description("Specifies the size of the buffer used to extract bytes from the underlying socket")]
        [DefaultValue(DefaultReceiveBufferSize)]
        public int ReceiveBufferSize
        {
            get
            {
                return this._receiveBufferSize;
            }

            set
            {
                this._receiveBufferSize = value;
            }
        }

        /// <summary>Gets or sets the remote certificate validation callback.</summary>
        [Browsable(false)]
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

        /// <summary>Gets or sets a value indicating whether secure.</summary>
        [Category("Behavior")]
        [Description("Specifies whether to use SSL authentication and encryption")]
        [DefaultValue(false)]
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

        /// <summary>Gets or sets the server certificate name.</summary>
        [Category("Behavior")]
        [Description("The name of the required server certificate")]
        [DefaultValue("")]
        public string ServerCertificateName
        {
            get
            {
                return this._serverCertificateName;
            }

            set
            {
                this._serverCertificateName = value;
            }
        }

        /// <summary>Gets the status.</summary>
        [Browsable(false)]
        public ConnectionStatus Status
        {
            get
            {
                lock (this._statusLock) return this._status;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        public void Close()
        {
            if (this._status != ConnectionStatus.Closed)
            {
                this.UpdateStatus(ConnectionStatus.Closed);

                try
                {
                    if (this._connection != null)
                    {
                        this._connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
                finally
                {
                    this._connection = null;
                }
            }
        }

        /// <summary>
        ///     Function using the default LengthPreFixedPackAssembler.
        /// </summary>
        public void Open()
        {
            var packetAssembler = new LengthPreFixedPackAssembler();
            this.Open(packetAssembler);
        }

        /// <summary>The open.</summary>
        /// <param name="packetAssembler">The packet assembler.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open(PacketAssembler packetAssembler)
        {
            if (this.Status != ConnectionStatus.Closed)
            {
                throw new InvalidOperationException("Attempt to open a connection that is already open or opening");
            }

            if (string.IsNullOrEmpty(this.Address))
            {
                throw new InvalidOperationException("Invalid host Address");
            }

            if (this._secure && this._remoteCertificateValidationCallback == null)
            {
                throw new InvalidOperationException("RemoteCertificateValidationCallback needed for secure mode");
            }

            if (this._socketConnector == null)
            {
                throw new InvalidOperationException(
                    "Cannot open new connection with NetworkConnection with null ISocketConnector");
            }

            this.UpdateStatus(ConnectionStatus.Opening);

            Socket socket = null;

            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        try
                        {
                            AppCulture.SetThreadCulture();

                            socket = this._socketConnector.GetConnectedSocket(this.Address, this.Port);

                            if (this.Secure)
                            {
                                this._connection = new NetworkConnectionImplSecure(
                                    socket, 
                                    this._remoteCertificateValidationCallback, 
                                    this._receiveBufferSize, 
                                    packetAssembler);
                                ((NetworkConnectionImplSecure)this._connection).AuthenticateAsClient(
                                    this._serverCertificateName);
                            }
                            else
                            {
                                this._connection = new NetworkConnectionImplUnsecure(
                                    socket, this._receiveBufferSize, packetAssembler);
                            }

                            this.HookupConnectionEvents();
                            this.UpdateStatus(ConnectionStatus.Open);
                        }
                        catch (Exception ex)
                        {
                            // Special anti-pattern handling to prevent exceptions noise in the logs
                            string message = null;
                            if (ex is SocketException)
                            {
                                message = ex.Message;
                            }
                            else if (ex.InnerException != null && ex.InnerException is SocketException)
                            {
                                message = ex.InnerException.Message;
                            }

                            if (!string.IsNullOrEmpty(message))
                            {
                                Log.ErrorFormat(
                                    "{0} failed for {1}:{2} {3}", 
                                    MethodBase.GetCurrentMethod().Name, 
                                    this.Address, 
                                    this.Port, 
                                    message);
                            }
                            else
                            {
                                Log.Error(ex.Message, ex);
                            }

                            try
                            {
                                if (socket != null)
                                {
                                    socket.Close();
                                }

                                this.UpdateStatus(ConnectionStatus.Closed);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message, e);
                            }
                        }
                    }, 
                null);
        }

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Boolean.</returns>
        /// <exception cref="ApplicationException"></exception>
        public bool Send(byte[] data, PacketType packetType)
        {
            try
            {
                if (this._connection == null)
                {
                    throw new ApplicationException("Cannot send - connection is closed");
                }

                this._connection.Send(data, packetType);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                this.Close();
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>Internal Port setting for use by NetworkConnectionListener</summary>
        /// <param name="port"></param>
        /// <remarks>This method is used because Netscaler polls for connections but closes them so quickly the socket created is invalid</remarks>
        internal void SetPort(int port)
        {
            this._port = port;
        }

        /// <summary>The on receive.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnReceive(ReceiveEventArgs e)
        {
            EventPublisher.RaiseEvent(this._receive, this, e);
        }

        /// <summary>The on send completed.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnSendCompleted(SendCompletedEventArgs e)
        {
            EventPublisher.RaiseEvent(this.SendCompleted, this, e);
        }

        /// <summary>The on status changed.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            lock (this._padLock)
            {
                if (this._onStatusChanged)
                {
                    return;
                }

                try
                {
                    this._onStatusChanged = true;
                    EventPublisher.RaiseEvent(this.StatusChanged, this, e);
                }
                finally
                {
                    this._onStatusChanged = false;
                }
            }
        }

        /// <summary>The update status.</summary>
        /// <param name="newStatus">The new status.</param>
        protected void UpdateStatus(ConnectionStatus newStatus)
        {
            ConnectionStatus oldStatus;
            lock (this._statusLock)
            {
                oldStatus = this._status;
                this._status = newStatus;
            }

            if (newStatus != oldStatus)
            {
                this.OnStatusChanged(EventArgs.Empty);
            }
        }

        /// <summary>The hookup connection events.</summary>
        private void HookupConnectionEvents()
        {
            try
            {
                this._connection.Receive += delegate(object source, ReceiveEventArgs args) { this.OnReceive(args); };

                this._connection.Closed += delegate { this.UpdateStatus(ConnectionStatus.Closed); };

                lock (this._statusLock)
                {
                    if (this._receive != null)
                    {
                        this._connection.BeginReceive();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }

    #region Event argument classes

    /// <summary>The receive event args.</summary>
    public class ReceiveEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ReceiveEventArgs"/> class.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        public ReceiveEventArgs(byte[] data, PacketType packetType)
        {
            this.Data = data;
            this.PacketType = packetType;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the data.</summary>
        public byte[] Data { get; private set; }

        /// <summary>Gets or sets the in process time stamp.</summary>
        public long InProcessTimeStamp { get; set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType { get; private set; }

        #endregion
    }

    /// <summary>The send completed event args.</summary>
    public class SendCompletedEventArgs : EventArgs
    {
    }

    #endregion

    #region Enumerations

    /// <summary>The connection status.</summary>
    public enum ConnectionStatus
    {
        /// <summary>
        ///     The Network Connection is connected to a remote application
        /// </summary>
        Open, 

        /// <summary>
        ///     The Network connection is attempting to connect to a remote application
        /// </summary>
        Opening, 

        /// <summary>
        ///     The Network Connection is not connected to a remote application
        /// </summary>
        Closed
    }

    /// <summary>The string encoding.</summary>
    public enum StringEncoding
    {
        /// <summary>
        ///     Encodes strings as ASCII with a EOT (4) terminating character
        /// </summary>
        /// <remarks>This mode is supplied for communicating with legacy TechieNotes applications and should not be used</remarks>
        Ascii, 

        /// <summary>
        ///     Encodes strings as UTF-8
        /// </summary>
        UTF8
    }

    #endregion
}