//  ===================================================================================
//  <copyright file="Connection.cs" company="TechieNotes">
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
//     The Connection.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Events;
using TradeFx.Common.Transport.Network;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport
{
    /// <summary>
    ///     Connection base class
    /// </summary>
    public abstract class Connection : IConnection
    {
        #region Fields

        /// <summary>The _synchronous send.</summary>
        private bool _synchronousSend = true;

        #endregion

        #region Public Events

        /// <summary>The receive.</summary>
        public abstract event EventHandler<ReceiveEventArgs> Receive;

        /// <summary>The send completed.</summary>
        public event EventHandler<SendCompletedEventArgs> SendCompleted
        {
            add
            {
                this.ConnectionSendCompleted += value;
                this.OnSendCompletedAdd(value);
            }

            remove
            {
                this.ConnectionSendCompleted -= value;
                this.OnSendCompletedRemove(value);
            }
        }

        /// <summary>The status changed.</summary>
        public event EventHandler<EventArgs> StatusChanged
        {
            add
            {
                this.ConnectionStatusChanged += value;
                this.OnStatusChangedAdd(value);
            }

            remove
            {
                this.ConnectionStatusChanged -= value;
                this.OnStatusChangedRemove(value);
            }
        }

        #endregion

        #region Events

        /// <summary>The _send completed.</summary>
        private event EventHandler<SendCompletedEventArgs> ConnectionSendCompleted;

        /// <summary>The _status changed.</summary>
        private event EventHandler<EventArgs> ConnectionStatusChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the connection address.</summary>
        public string ConnectionAddress { get; set; }

        /// <summary>Gets the connection status.</summary>
        public abstract ConnectionStatus ConnectionStatus { get; }

        /// <summary>Gets or sets a value indicating whether secure.</summary>
        public bool Secure { get; set; }

        /// <summary>Gets or sets a value indicating whether synchronous send.</summary>
        public bool SynchronousSend
        {
            get
            {
                return this._synchronousSend;
            }

            set
            {
                this._synchronousSend = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The connect.</summary>
        public abstract void Connect();

        /// <summary>The disconnect.</summary>
        public abstract void Disconnect();

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Boolean.</returns>
        public abstract bool Send(byte[] data, PacketType packetType);

        #endregion

        #region Methods

        /// <summary>The on send completed.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnSendCompleted(SendCompletedEventArgs e)
        {
            EventPublisher.RaiseEvent(this.ConnectionSendCompleted, this, e);
        }

        /// <summary>The on send completed add.</summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSendCompletedAdd(EventHandler<SendCompletedEventArgs> value)
        {
        }

        /// <summary>The on send completed remove.</summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSendCompletedRemove(EventHandler<SendCompletedEventArgs> value)
        {
        }

        /// <summary>The on status changed.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            EventPublisher.RaiseEvent(this.ConnectionStatusChanged, this, e);
        }

        /// <summary>The on status changed add.</summary>
        /// <param name="value">The value.</param>
        protected virtual void OnStatusChangedAdd(EventHandler<EventArgs> value)
        {
        }

        /// <summary>The on status changed remove.</summary>
        /// <param name="value">The value.</param>
        protected virtual void OnStatusChangedRemove(EventHandler<EventArgs> value)
        {
        }

        #endregion
    }
}