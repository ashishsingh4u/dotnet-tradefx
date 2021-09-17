//  ===================================================================================
//  <copyright file="PacketAssembler.cs" company="TechieNotes">
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
//     The PacketAssembler.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Events;
using TradeFx.Common.Transport.Network;

namespace TradeFx.Common.Transport.Packet
{
    /// <summary>
    ///     Base for classes that frame and encode data for transmission over a stream transport such as TCP
    /// </summary>
    public abstract class PacketAssembler
    {
        #region Public Events

        /// <summary>The packet.</summary>
        public event EventHandler<ReceiveEventArgs> Packet;

        #endregion

        #region Public Methods and Operators

        /// <summary>re-assembles the network packets from the byte stream</summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <remarks>Re-assembled packets are passed out via the Packet event</remarks>
        public abstract void Add(byte[] data, int offset, int count);

        /// <summary>Converts the string to a network packet</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet Type.</param>
        /// <param name="compress">The compress.</param>
        /// <returns>The System.Byte[].</returns>
        public abstract byte[] MakePacket(byte[] data, PacketType packetType, bool compress);

        /// <summary>
        ///     Reset the PacketAssembler
        /// </summary>
        public abstract void Reset();

        #endregion

        #region Methods

        /// <summary>The on packet.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnPacket(ReceiveEventArgs e)
        {
            EventPublisher.RaiseEvent(this.Packet, this, e);
        }

        #endregion
    }

    /// <summary>The packet event args.</summary>
    public class PacketEventArgs : EventArgs
    {
        #region Fields

        /// <summary>The _packet.</summary>
        private readonly byte[] _packet;

        /// <summary>The _packet type.</summary>
        private readonly PacketType _packetType;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PacketEventArgs"/> class.</summary>
        /// <param name="packet">The packet.</param>
        /// <param name="packetType">The packet type.</param>
        public PacketEventArgs(byte[] packet, PacketType packetType)
        {
            this._packet = packet;
            this._packetType = packetType;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the packet.</summary>
        public byte[] Packet
        {
            get
            {
                return this._packet;
            }
        }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType
        {
            get
            {
                return this._packetType;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Base for classes that frame and encode data for transmission over a stream transport such as TCP
    /// </summary>
    public abstract class StringPacketAssembler
    {
        #region Public Events

        /// <summary>The packet.</summary>
        public event EventHandler<StringPacketEventArgs> Packet;

        #endregion

        #region Public Methods and Operators

        /// <summary>re-assembles the network packets from the byte stream</summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <remarks>Re-assembled packets are passed out via the Packet event</remarks>
        public abstract void Add(byte[] data, int offset, int count);

        /// <summary>Converts the string to a network packet</summary>
        /// <param name="message"></param>
        /// <returns>The System.Byte[].</returns>
        public abstract byte[] MakePacket(string message);

        #endregion

        #region Methods

        /// <summary>The on packet.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnPacket(StringPacketEventArgs e)
        {
            EventPublisher.RaiseEvent(this.Packet, this, e);
        }

        #endregion
    }

    /// <summary>The string packet event args.</summary>
    public class StringPacketEventArgs : EventArgs
    {
        #region Fields

        /// <summary>The _packet.</summary>
        private readonly string _packet;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="StringPacketEventArgs"/> class.</summary>
        /// <param name="packet">The packet.</param>
        public StringPacketEventArgs(string packet)
        {
            this._packet = packet;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the packet.</summary>
        public string Packet
        {
            get
            {
                return this._packet;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Packet types - defines payload type in the packet
    /// </summary>
    /// <remarks>Values are specified to ensure compatibility with other implementations</remarks>
    public enum PacketType : byte
    {
        /// <summary>The binary formatter.</summary>
        BinaryFormatter = 0, 

        /// <summary>The soap formatter.</summary>
        SoapFormatter = 1, 

        /// <summary>The sql xml raw.</summary>
        SqlXmlRaw = 2, 

        /// <summary>The techie notes delimited.</summary>
        TechieNotesDelimited = 3, 

        /// <summary>The sql hbt.</summary>
        SqlHbt = 4, 

        /// <summary>The binary stream formatter.</summary>
        BinaryStreamFormatter = 5, 

        /// <summary>The binary formatter compressed.</summary>
        BinaryFormatterCompressed = 6, 
    }
}