//  ===================================================================================
//  <copyright file="ServerSideHbtResponse.cs" company="TechieNotes">
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
//     The ServerSideHbtResponse.cs file.
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
    /// <summary>The server side hbt response.</summary>
    public sealed class ServerSideHbtResponse : ITechieNotesDelimitedSerialize, 
                                                ILoggable, 
                                                IGetShortName, 
                                                ISendable, 
                                                IHbt
    {
        #region Fields

        /// <summary>The _packet type.</summary>
        private PacketType _packetType = PacketType.TechieNotesDelimited;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResponse"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        public ServerSideHbtResponse(ServerSideHbtResponse hbt)
        {
            this.CopyFrom(hbt);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSideHbtResponse" /> class.
        /// </summary>
        public ServerSideHbtResponse()
        {
            this.DateTime = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResponse"/> class.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="hbt">The hbt.</param>
        public ServerSideHbtResponse(string dateTime, ServerSideHbt hbt)
        {
            this.DateTime = dateTime;
            this.Hbt = hbt;
        }

        /// <summary>Initializes a new instance of the <see cref="ServerSideHbtResponse"/> class.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="hbt">The hbt.</param>
        /// <param name="packetType">The packet type.</param>
        public ServerSideHbtResponse(string dateTime, ServerSideHbt hbt, PacketType packetType)
            : this(dateTime, hbt)
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

        /// <summary>Gets the hbt.</summary>
        public ServerSideHbt Hbt { get; private set; }

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
                return "SSHbtR";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new ServerSideHbtResponse();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.DateTime = parser.GetString();
                this.Hbt = parser.GetObject<ServerSideHbt>();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.ServerSideHbtResponse;
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
                builder.Append(this.DateTime);
                builder.Append(this.Hbt);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            return string.Format("ServerSideHbtResponse : {0} {1}", this.DateTime, this.Hbt.ToLog());
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="hbt">The hbt.</param>
        private void CopyFrom(ServerSideHbtResponse hbt)
        {
            this.DateTime = hbt.DateTime;
            this.Hbt = hbt.Hbt;
            this._packetType = hbt.PacketType;
        }

        #endregion
    }
}