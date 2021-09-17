//  ===================================================================================
//  <copyright file="NetworkConnectionImpl.cs" company="TechieNotes">
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
//     The NetworkConnectionImpl.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;
using System.Reflection;

using TradeFx.Common.Culture;
using TradeFx.Common.Events;
using TradeFx.Common.Transport.Packet;

using log4net;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection impl.</summary>
    internal abstract class NetworkConnectionImpl
    {
        #region Constants

        /// <summary>The start read buffer size.</summary>
        private const int StartReadBufferSize = 10 * 1024;

        #endregion

        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>The _count.</summary>
        private static int _count;

        #endregion

        #region Fields

        /// <summary>
        ///     The connection
        /// </summary>
        protected readonly Stream _connection;

        /// <summary>
        ///     Packet assembler
        /// </summary>
        protected PacketAssembler _packetAssembler;

        /// <summary>The _max read buffer size.</summary>
        private readonly int _maxReadBufferSize;

        /// <summary>
        ///     Synchronization object to provide thread safety.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        ///     Prevents raising events after the connection is closed.
        /// </summary>
        private volatile bool _isClosed;

        /// <summary>
        ///     Receiving State
        /// </summary>
        private volatile bool _isReceiving;

        /// <summary>
        ///     The read buffer
        /// </summary>
        private byte[] _readBuffer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionImpl"/> class.</summary>
        /// <param name="connection">The connection.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        /// <param name="packetAssembler">The packet assembler.</param>
        protected NetworkConnectionImpl(Stream connection, int readBufferSize, PacketAssembler packetAssembler)
        {
            this._maxReadBufferSize = readBufferSize;
            this._readBuffer = new byte[StartReadBufferSize];
            this._connection = connection;
            this._packetAssembler = packetAssembler;
            this._packetAssembler.Reset();
        }

        #endregion

        #region Public Events

        /// <summary>The closed.</summary>
        public event EventHandler<EventArgs> Closed;

        /// <summary>The receive.</summary>
        public event EventHandler<ReceiveEventArgs> Receive
        {
            add
            {
                this._packetAssembler.Packet += value;
            }

            remove
            {
                this._packetAssembler.Packet -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is secure.</summary>
        public abstract bool IsSecure { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The begin receive.</summary>
        public void BeginReceive()
        {
            try
            {
                if (this._isReceiving || this._isClosed)
                {
                    return; // Already receiving
                }

                lock (this._syncObject)
                {
                    if (this._isReceiving)
                    {
                        return; // test again to make sure 
                    }

                    this._connection.BeginRead(this._readBuffer, 0, this._readBuffer.Length, this.ReadCallback, null);
                    this._isReceiving = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                this.Close();
            }
        }

        /// <summary>The close.</summary>
        public void Close()
        {
            try
            {
                if (this._isClosed)
                {
                    return;
                }

                this._isClosed = true;
                this._connection.Close();
                this.OnClosed();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        public void Send(byte[] data, PacketType packetType)
        {
            byte[] sendBuffer = this._packetAssembler.MakePacket(data, packetType, false);
            this._connection.Write(sendBuffer, 0, sendBuffer.Length);
        }

        #endregion

        #region Methods

        /// <summary>The on closed.</summary>
        protected void OnClosed()
        {
            EventPublisher.RaiseEvent(this.Closed, this, EventArgs.Empty);
        }

        /// <summary>The read callback.</summary>
        /// <param name="ar">The ar.</param>
        /// <exception cref="ApplicationException"></exception>
        private void ReadCallback(IAsyncResult ar)
        {
            AppCulture.SetThreadCulture();

            try
            {
                if (!this._isClosed)
                {
                    var count = this._connection.EndRead(ar);
                    ar.AsyncWaitHandle.Close(); // reduce 'leak' of handles waiting around for eventual finalization
                    if (count == 0)
                    {
                        throw new ApplicationException("Remote endpoint closed gracefully by remote end");
                    }

                    this._packetAssembler.Add(this._readBuffer, 0, count);

                    if ((count >= this._readBuffer.Length) && (this._readBuffer.Length < this._maxReadBufferSize))
                    {
                        // Readjust readbuffer
                        this._readBuffer =
                            new byte[
                                this._readBuffer.Length * 2 > this._maxReadBufferSize
                                    ? this._maxReadBufferSize
                                    : this._readBuffer.Length * 2];
                    }

                    this._connection.BeginRead(this._readBuffer, 0, this._readBuffer.Length, this.ReadCallback, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0} failed", MethodBase.GetCurrentMethod().Name), ex);
                this.Close();
            }
        }

        #endregion
    }
}