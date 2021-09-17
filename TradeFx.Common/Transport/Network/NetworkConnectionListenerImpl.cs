//  ===================================================================================
//  <copyright file="NetworkConnectionListenerImpl.cs" company="TechieNotes">
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
//     The NetworkConnectionListenerImpl.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using TradeFx.Common.Culture;
using TradeFx.Common.Events;

using log4net;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection listener impl.</summary>
    internal abstract class NetworkConnectionListenerImpl
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        /// <summary>
        ///     Bind Address
        /// </summary>
        protected readonly string _address;

        /// <summary>
        ///     Bind Port
        /// </summary>
        protected readonly int _port;

        /// <summary>
        ///     The size of the read buffer for accepted connections.
        /// </summary>
        protected readonly int _readBufferSize;

        /// <summary>
        ///     The backlog of sockets waiting to connect.
        /// </summary>
        private readonly int _listenBacklog;

        /// <summary>
        ///     Private sync object for locking
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        ///     The underlying socket doing the listening
        /// </summary>
        private Socket _listeningSocket;

        /// <summary>
        ///     The thread for the listening socket.
        /// </summary>
        private Thread _listeningSocketThread;

        /// <summary>The _new socket.</summary>
        private Socket _newSocket;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionListenerImpl"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="listenBacklog">The listen backlog.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        public NetworkConnectionListenerImpl(string address, int port, int listenBacklog, int readBufferSize)
        {
            this._address = address;
            this._port = port;
            this._listenBacklog = listenBacklog;
            this._readBufferSize = readBufferSize;
        }

        #endregion

        #region Public Events

        /// <summary>The connect.</summary>
        public event EventHandler<ConnectEventArgs> Connect;

        /// <summary>The error.</summary>
        public event EventHandler<ErrorEventArgs> Error;

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        public void Close()
        {
            Log.InfoFormat("Shutting down listener:{0}:{1}", this._address, this._port);
            lock (this._sync)
            {
                if (this._listeningSocketThread != null)
                {
                    if (this._listeningSocket != null)
                    {
                        this._listeningSocket.Close();
                    }

                    this._listeningSocketThread.Abort();
                    this._listeningSocketThread.Join();
                    this._listeningSocketThread = null;

                    if (this._newSocket != null)
                    {
                        this._newSocket.Close();
                        this._newSocket = null;
                    }
                }
            }
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            lock (this._sync)
            {
                if (this._listeningSocketThread != null)
                {
                    return;
                }

                this._listeningSocketThread = new Thread(this.SocketAcceptThreadProc);
                this._listeningSocketThread.IsBackground = true;
                this._listeningSocketThread.Name = this.GetThreadName();
                this._listeningSocketThread.Start();
            }
        }

        #endregion

        #region Methods

        /// <summary>The set net connection socket options.</summary>
        /// <param name="newSocket">The new socket.</param>
        protected static void SetNetConnectionSocketOptions(Socket newSocket)
        {
            // newSocket.UseOnlyOverlappedIO = true;
            newSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 128 * 1024);
            newSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
        }

        /// <summary>The create connection.</summary>
        /// <param name="newSocket">The new socket.</param>
        protected abstract void CreateConnection(Socket newSocket);

        /// <summary>The get thread name.</summary>
        /// <returns>The System.String.</returns>
        protected abstract string GetThreadName();

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

        /// <summary>The create listener.</summary>
        private void CreateListener()
        {
            this._listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            this._listeningSocket.LingerState = new LingerOption(true, 0);
            this._listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            var ep = new IPEndPoint(IPAddress.Any, this._port);
            if (this._address != null && this._address.ToUpper() != "LOCALHOST")
            {
                var iPv4Address = Array.Find(
                    Dns.GetHostEntry(this._address).AddressList, x => (x.AddressFamily == AddressFamily.InterNetwork));
                ep = new IPEndPoint(iPv4Address, this._port);
            }

            this._listeningSocket.Bind(ep);
            this._listeningSocket.Listen(this._listenBacklog);
        }

        /// <summary>The socket accept thread proc.</summary>
        private void SocketAcceptThreadProc()
        {
            try
            {
                AppCulture.SetThreadCulture();

                this.CreateListener();

                while (true)
                {
                    try
                    {
                        this._newSocket = this._listeningSocket.Accept();
                    }
                    catch (SocketException ex)
                    {
                        switch (ex.ErrorCode)
                        {
                            case 10022: // WSAEINVAL - Socket state invalid
                            case 10050: // WSAENETDOWN - Network Dead
                                throw;
                            case 10013: // WSAEACCES - Permission Denied
                            case 10054: // WSAECONNRESET - Connection was reset by peer
                            case 10061: // WSAECONNREFUSED - Connection Refused
                            case 11002: // WSATRY_AGAIN - Try Again
                                Log.Error(ex.Message, ex);
                                Thread.Sleep(50);
                                continue;
                            default:
                                Log.Error(
                                    string.Format(
                                        "Unknown SocketException error type: {0} ErrorCode: {1}", 
                                        ex.Message, 
                                        ex.ErrorCode), 
                                    ex);
                                throw;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    // Network monitoring often sends incomplete connects
                    // so ignore any socket with an invalid Port
                    var remoteEnd = (IPEndPoint)this._newSocket.RemoteEndPoint;
                    if (remoteEnd.Port < 1 || remoteEnd.Port >= IPEndPoint.MaxPort)
                    {
                        this._newSocket.Close();
                        continue;
                    }

                    ThreadPool.QueueUserWorkItem(
                        delegate(object state)
                            {
                                try
                                {
                                    AppCulture.SetThreadCulture();
                                    var newSock = (Socket)state;
                                    this.CreateConnection(newSock);
                                }
                                catch (ThreadAbortException)
                                {
                                    // Nothing to do
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message, ex);
                                }
                            }, 
                        this._newSocket);
                }
            }
            catch (ThreadAbortException)
            {
                // Nothing to do
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                this.OnError(new ErrorEventArgs(ex));
            }
            finally
            {
                if (this._listeningSocket != null)
                {
                    this._listeningSocket.Close();
                }
            }
        }

        #endregion
    }
}