// ===================================================================================
// <copyright file="ServerSideHbt.cs" company="TechieNotes">
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
// <date>08-09-2012</date>
// <summary>
//    The ServerSideHbt.cs file.
// </summary>
// ===================================================================================

using System;
using System.Globalization;

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
    /// Client to use this to request server side Hbt for Latency calculation purposes
    /// Want the client to initialise latency measurement to give it a chance 
    /// to notice the response is delayed and signal high latency earlier
    /// </summary>
    public sealed class ServerSideHbt : ITechieNotesDelimitedSerialize, ILoggable, IGetShortName, ISendable, IHbt
    {
        #region Fields

        /// <summary>The _packet type.</summary>
        private PacketType _packetType = PacketType.TechieNotesDelimited;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbt"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        public ServerSideHbt(ServerSideHbt hbt)
        {
            this.CopyFrom(hbt);
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbt"/> class.</summary>
        public ServerSideHbt()
        {
            this.Key = string.Empty;
            this.DateTime = string.Empty;
            this.Ts = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbt"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="ts">The ts.</param>
        /// <param name="requestKey">The request key.</param>
        public ServerSideHbt(string key, string dateTime, long ts, string requestKey)
        {
            this.Key = key;
            this.DateTime = dateTime;
            this.Ts = ts;
            this.RequestKey = requestKey;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbt"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="requestKey">The request key.</param>
        /// <param name="ts">The ts.</param>
        /// <param name="packetType">The packet type.</param>
        public ServerSideHbt(string key, string dateTime, string requestKey, long ts, PacketType packetType)
            : this(key, dateTime, ts, requestKey)
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

        /// <summary>Gets the request key.</summary>
        public string RequestKey { get; private set; }

        /// <summary>Gets the short name.</summary>
        public string ShortName
        {
            get
            {
                return "SSHbt";
            }
        }

        /// <summary>Gets the ts.</summary>
        public long Ts { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new ServerSideHbt();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.Key = parser.GetString();
                this.DateTime = parser.GetString();
                this.Ts = parser.GetLong();
                this.RequestKey = parser.GetString();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.ServerSideHbt;
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
                builder.Append(this.Ts);
                builder.Append(this.RequestKey);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            return string.Format(
                "ServerSideHbt : {0} {1} {2} {3}", this.Key, this.DateTime, this.Ts.ToString(CultureInfo.InvariantCulture), this.RequestKey);
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="hbt">The hbt.</param>
        private void CopyFrom(ServerSideHbt hbt)
        {
            this.Key = hbt.Key;
            this.DateTime = hbt.DateTime;
            this.Ts = hbt.Ts;
            this.RequestKey = hbt.RequestKey;
            this._packetType = hbt.PacketType;
        }

        #endregion
    }
}