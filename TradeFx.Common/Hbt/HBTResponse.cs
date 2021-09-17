//  ===================================================================================
//  <copyright file="HBTResponse.cs" company="TechieNotes">
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
//     The HBTResponse.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Text;

using TechieNotes.Common.Creation;
using TechieNotes.Common.HBT;
using TechieNotes.Common.Serialization;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;
using TradeFx.Common.Serialization;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Hbt
{
    /// <summary>The hbt response.</summary>
    [Serializable]
    public sealed class HbtResponse : ITechieNotesDelimitedSerialize, ILoggable, IGetShortName, ISendable, IHbt
    {
        #region Constants

        /// <summary>The short name.</summary>
        public const string SHORT_NAME = "HBTR";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HbtResponse" /> class.
        /// </summary>
        public HbtResponse()
        {
            this.DateTime = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="HbtResponse"/> class.</summary>
        /// <param name="hbt">The hbt.</param>
        /// <param name="dateTime">The date time.</param>
        public HbtResponse(Hbt hbt, string dateTime)
        {
            this.Hbt = hbt;
            this.DateTime = dateTime;
        }

        /// <summary>Initializes a new instance of the <see cref="HbtResponse"/> class.</summary>
        /// <param name="hbtResponse">The hbt response.</param>
        public HbtResponse(HbtResponse hbtResponse)
        {
            this.CopyFrom(hbtResponse);
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

        /// <summary>Gets the hbt.</summary>
        public Hbt Hbt { get; private set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType
        {
            get
            {
                return PacketType.TechieNotesDelimited;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The copy from.</summary>
        /// <param name="hbtResponse">The hbt response.</param>
        public void CopyFrom(HbtResponse hbtResponse)
        {
            this.Hbt = new Hbt(hbtResponse.Hbt);
            this.DateTime = hbtResponse.DateTime;
        }

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new HbtResponse();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.Hbt = (Hbt)parser.GetObject();
                this.DateTime = parser.GetString();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.HbtResponse;
        }

        /// <summary>The get short name.</summary>
        /// <returns>The System.String.</returns>
        public string GetShortName()
        {
            return SHORT_NAME;
        }

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        public void Serialize(TechieNotesDelimitedBuilder builder)
        {
            using (new TechieNotesDelimitedBuilder.Terminator(builder))
            {
                builder.Append(this.Hbt);
                builder.Append(this.DateTime);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            var sb = new StringBuilder();

            LogUtil.Append(sb, "ShortName", SHORT_NAME);
            LogUtil.Append(sb, "HBTResponseDateTime", this.DateTime);

            if (this.Hbt != null)
            {
                sb.Append(this.Hbt.ToLog());
            }
            else
            {
                LogUtil.Append(sb, "HBT", (string)null);
            }

            return sb.ToString();
        }

        #endregion
    }
}