//  ===================================================================================
//  <copyright file="ServerSideHbtRequest.cs" company="TechieNotes">
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
//     The ServerSideHbtRequest.cs file.
//  </summary>
//  ===================================================================================

using System;

using TechieNotes.Common.Creation;
using TechieNotes.Common.HBT;
using TechieNotes.Common.Serialization;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;
using TradeFx.Common.Serialization;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Hbt
{
    /// <summary>
    ///     Client to use this to request server side Hbt for Latency calculation purposes
    /// </summary>
    public sealed class ServerSideHbtRequest : ITechieNotesDelimitedSerialize, ILoggable, IGetShortName, ISendable, IHbt
    {
        #region Fields

        /// <summary>The _packet type.</summary>
        private PacketType _packetType = PacketType.TechieNotesDelimited;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtRequest"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        public ServerSideHbtRequest(ServerSideHbtRequest hbt)
        {
            this.CopyFrom(hbt);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSideHbtRequest" /> class.
        /// </summary>
        public ServerSideHbtRequest()
        {
            this.Key = string.Empty;
            this.DateTime = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtRequest"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        public ServerSideHbtRequest(string key, string dateTime)
        {
            this.Key = key;
            this.DateTime = dateTime;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtRequest"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="packetType">The packet type.</param>
        public ServerSideHbtRequest(string key, string dateTime, PacketType packetType)
            : this(key, dateTime)
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

        /// <summary>Gets the date time.</summary>
        public string DateTime { get; private set; }

        /// <summary>Gets the key.</summary>
        public string Key { get; private set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType
        {
            get
            {
                return this._packetType;
            }
        }

        /// <summary>Gets the short name.</summary>
        public string ShortName
        {
            get
            {
                return "SSHbtRq";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new ServerSideHbtRequest();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.Key = parser.GetString();
                this.DateTime = parser.GetString();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.ServerSideHbtRequest;
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
                builder.Append(this.Key);
                builder.Append(this.DateTime);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            return string.Format("ServerSideHbtRequest : {0} {1}", this.Key, this.DateTime);
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="hbt">The hbt.</param>
        private void CopyFrom(ServerSideHbtRequest hbt)
        {
            this.Key = hbt.Key;
            this.DateTime = hbt.DateTime;
            this._packetType = hbt.PacketType;
        }

        #endregion
    }
}