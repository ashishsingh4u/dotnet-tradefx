// ===================================================================================
// <copyright file="IConnection.cs" company="TechieNotes">
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
// <date>09-09-2012</date>
// <summary>
//    The IConnection.cs file.
// </summary>
// ===================================================================================

using System;

using TradeFx.Common.Transport.Network;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport
{
    /// <summary>
    /// Connection interface
    /// </summary>
    public interface IConnection
    {
        #region Public Events

        /// <summary>The receive.</summary>
        event EventHandler<ReceiveEventArgs> Receive;

        /// <summary>The send completed.</summary>
        event EventHandler<SendCompletedEventArgs> SendCompleted;

        /// <summary>The status changed.</summary>
        event EventHandler<EventArgs> StatusChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the connection address.</summary>
        string ConnectionAddress { get; set; }

        /// <summary>Gets the connection status.</summary>
        ConnectionStatus ConnectionStatus { get; }

        /// <summary>Gets or sets a value indicating whether secure.</summary>
        bool Secure { get; set; }

        /// <summary>Gets or sets a value indicating whether synchronous send.</summary>
        bool SynchronousSend { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The connect.</summary>
        void Connect();

        /// <summary>The disconnect.</summary>
        void Disconnect();

        /// <summary>The send.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Boolean.</returns>
        bool Send(byte[] data, PacketType packetType);

        #endregion
    }
}