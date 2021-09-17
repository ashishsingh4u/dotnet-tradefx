//  ===================================================================================
//  <copyright file="ServerSideHbtResult.cs" company="TechieNotes">
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
//  <date>13-03-2013</date>
//  <summary>
//     The ServerSideHbtResult.cs file.
//  </summary>
//  ===================================================================================

using System;

using TechieNotes.Common.Creation;
using TechieNotes.Common.Serialization;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;
using TradeFx.Common.Serialization;
using TradeFx.Common.Transport.Packet;

namespace TechieNotes.Common.HBT
{
    /// <summary>The server side hbt result.</summary>
    public sealed class ServerSideHbtResult : ITechieNotesDelimitedSerialize, ILoggable, IGetShortName, ISendable, IHbt
    {
        #region Fields

        /// <summary>The _packet type.</summary>
        private PacketType _packetType = PacketType.TechieNotesDelimited;

        #endregion

        // Key of original ServerSideHbtRequest
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResult"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        public ServerSideHbtResult(ServerSideHbtResult hbt)
        {
            this.CopyFrom(hbt);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSideHbtResult" /> class.
        /// </summary>
        public ServerSideHbtResult()
        {
            this.RequestKey = string.Empty;
            this.Latency = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResult"/> class.</summary>
        /// <param name="requestKey">The request key.</param>
        /// <param name="latency">The latency.</param>
        public ServerSideHbtResult(string requestKey, long latency)
        {
            this.RequestKey = requestKey;
            this.Latency = latency;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResult"/> class.</summary>
        /// <param name="requestKey">The request key.</param>
        /// <param name="latency">The latency.</param>
        /// <param name="packetType">The packet type.</param>
        public ServerSideHbtResult(string requestKey, long latency, PacketType packetType)
            : this(requestKey, latency)
        {
            this._packetType = packetType;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether conflate.</summary>
        public bool Conflate
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets the conflate key.</summary>
        /// <exception cref="NotImplementedException">This is not required.</exception>
        public Key ConflateKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the latency.</summary>
        public long Latency { get; private set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType
        {
            get
            {
                return this._packetType;
            }
        }

        /// <summary>Gets the request key.</summary>
        public string RequestKey { get; private set; }

        /// <summary>Gets the short name.</summary>
        public string ShortName
        {
            get
            {
                return "SSHbtRs";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new ServerSideHbtResult();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.RequestKey = parser.GetString();
                this.Latency = parser.GetLong();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.ServerSideHbtResult;
        }

        /// <summary>The get short name.</summary>
        /// <returns>The System.String.</returns>
        public string GetShortName()
        {
            return this.ShortName;
        }

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        public void Serialize(TechieNotesDelimitedBuilder builder)
        {
            using (new TechieNotesDelimitedBuilder.Terminator(builder))
            {
                builder.Append(this.RequestKey);
                builder.Append(this.Latency);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            return string.Format("ServerSideHbtResult : {0} {1}", this.RequestKey, this.Latency);
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="hbt">The hbt.</param>
        private void CopyFrom(ServerSideHbtResult hbt)
        {
            this.RequestKey = hbt.RequestKey;
            this.Latency = hbt.Latency;
            this._packetType = hbt.PacketType;
        }

        #endregion
    }
}