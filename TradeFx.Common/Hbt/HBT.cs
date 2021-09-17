//  ===================================================================================
//  <copyright file="HBT.cs" company="TechieNotes">
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
//     The HBT.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Text;

using TechieNotes.Common.HBT;
using TechieNotes.Common.Serialization;
using TechieNotes.Common.Serialization.Interfaces;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;
using TradeFx.Common.Serialization;
using TradeFx.Common.Serialization.Interfaces;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Hbt
{
    /// <summary>The hbt.</summary>
    [Serializable]
    public sealed class Hbt : ITechieNotesDelimitedSerialize, 
                              IStreamSerializable, 
                              IMessageTypeIdentifier, 
                              ILoggable, 
                              IGetShortName, 
                              ISendable, 
                              IHbt
    {
        #region Constants

        /// <summary>The short name.</summary>
        public const string SHORT_NAME = "HBT";

        #endregion

        #region Fields

        /// <summary>The _packet type.</summary>
        private PacketType _packetType = PacketType.TechieNotesDelimited;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Hbt"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        public Hbt(Hbt hbt)
        {
            this.CopyFrom(hbt);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Hbt" /> class.
        /// </summary>
        public Hbt()
        {
            this.Key = string.Empty;
            this.DateTime = string.Empty;
            this.HeartbeatPeriod = 0;
            this.ResponseRequired = false;
        }

        /// <summary>Initializes a new instance of the <see cref="Hbt"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="heartbeatPeriod">The heartbeat period.</param>
        /// <param name="responseRequired">The response required.</param>
        public Hbt(string key, string dateTime, int heartbeatPeriod, bool responseRequired)
        {
            this.Key = key;
            this.DateTime = dateTime;
            this.HeartbeatPeriod = heartbeatPeriod;
            this.ResponseRequired = responseRequired;
        }

        /// <summary>Initializes a new instance of the <see cref="Hbt"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="heartbeatPeriod">The heartbeat period.</param>
        /// <param name="responseRequired">The response required.</param>
        /// <param name="packetType">The packet type.</param>
        public Hbt(string key, string dateTime, int heartbeatPeriod, bool responseRequired, PacketType packetType)
            : this(key, dateTime, heartbeatPeriod, responseRequired)
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
        /// <exception cref="NotImplementedException">The method or operation is not implemented.</exception>
        public Key ConflateKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the date time.</summary>
        public string DateTime { get; private set; }

        /// <summary>Gets the heartbeat period.</summary>
        public int HeartbeatPeriod { get; private set; }

        /// <summary>Gets the key.</summary>
        public string Key { get; private set; }

        /// <summary>Gets the message type id.</summary>
        public MessageTypeIds MessageTypeId
        {
            get
            {
                return MessageTypeIds.HeartbeatRequest;
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

        /// <summary>Gets a value indicating whether response required.</summary>
        public bool ResponseRequired { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new Hbt();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.Key = parser.GetString();
                this.DateTime = parser.GetString();
                this.HeartbeatPeriod = parser.GetInt();
                this.ResponseRequired = parser.GetBool();
            }
        }

        /// <summary>The deserialize.</summary>
        /// <param name="reader">The reader.</param>
        public void Deserialize(IStreamReader reader)
        {
            this.Key = reader.ReadString();
            this.DateTime = reader.ReadString();
            this.HeartbeatPeriod = reader.ReadInt();
            this.ResponseRequired = reader.ReadBool();
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.Hbt;
        }

        /// <summary>The get short name.</summary>
        /// <returns>The System.String.</returns>
        public string GetShortName()
        {
            return SHORT_NAME;
        }

        /// <summary>The new date time.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The TechieNotes.Common.Hbt.HBT.</returns>
        public Hbt NewDateTime(string dateTime)
        {
            var hbt = new Hbt(this) { DateTime = dateTime };

            return hbt;
        }

        /// <summary>The new heartbeat period.</summary>
        /// <param name="heartbeatPeriod">The heartbeat period.</param>
        /// <returns>The TechieNotes.Common.Hbt.HBT.</returns>
        public Hbt NewHeartbeatPeriod(int heartbeatPeriod)
        {
            var hbt = new Hbt(this) { HeartbeatPeriod = heartbeatPeriod };

            return hbt;
        }

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        public void Serialize(TechieNotesDelimitedBuilder builder)
        {
            using (new TechieNotesDelimitedBuilder.Terminator(builder))
            {
                builder.Append(this.Key);
                builder.Append(this.DateTime);
                builder.Append(this.HeartbeatPeriod);
                builder.Append(this.ResponseRequired);
            }
        }

        /// <summary>The serialize.</summary>
        /// <param name="writer">The writer.</param>
        public void Serialize(IStreamWriter writer)
        {
            writer.Write(this.Key);
            writer.Write(this.DateTime);
            writer.Write(this.HeartbeatPeriod);
            writer.Write(this.ResponseRequired);
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            var sb = new StringBuilder();

            LogUtil.Append(sb, "ShortName", SHORT_NAME);
            LogUtil.Append(sb, "Key", this.Key);
            LogUtil.Append(sb, "DateTime", this.DateTime);
            LogUtil.Append(sb, "HeartbeatPeriod", this.HeartbeatPeriod);
            LogUtil.Append(sb, "ResponseRequired", this.ResponseRequired);

            return sb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="hbt">The hbt.</param>
        private void CopyFrom(Hbt hbt)
        {
            this.Key = hbt.Key;
            this.DateTime = hbt.DateTime;
            this.HeartbeatPeriod = hbt.HeartbeatPeriod;
            this.ResponseRequired = hbt.ResponseRequired;
            this._packetType = hbt.PacketType;
        }

        #endregion
    }
}