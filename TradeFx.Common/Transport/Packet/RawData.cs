//  ===================================================================================
//  <copyright file="RawData.cs" company="TechieNotes">
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
//     The RawData.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Text;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;

namespace TradeFx.Common.Transport.Packet
{
    /// <summary>The raw data.</summary>
    [Serializable]
    public sealed class RawData : IGetShortName, ILoggable, ISendable
    {
        #region Constants

        /// <summary>The short name.</summary>
        public const string ShortName = "RWD";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RawData" /> class.
        /// </summary>
        public RawData()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="RawData"/> class.</summary>
        /// <param name="rawData">The raw data.</param>
        public RawData(RawData rawData)
        {
            this.CopyFrom(rawData);
        }

        /// <summary>Initializes a new instance of the <see cref="RawData"/> class.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        public RawData(byte[] data, PacketType packetType)
        {
            this.Data = data;
            this.PacketType = packetType;
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
        /// <exception cref="NotImplementedException"></exception>
        public Key ConflateKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the data.</summary>
        public byte[] Data { get; private set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get short name.</summary>
        /// <returns>The System.String.</returns>
        public string GetShortName()
        {
            return ShortName;
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            var stringBuilder = new StringBuilder();

            LogUtil.Append(stringBuilder, ShortName);
            LogUtil.Append(stringBuilder, "RawDataLen", this.Data != null ? this.Data.Length : 0);
            LogUtil.Append(
                stringBuilder, "RawData", this.Data != null ? Encoding.UTF8.GetString(this.Data) : string.Empty);

            return stringBuilder.ToString();
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="rawData">The raw data.</param>
        private void CopyFrom(RawData rawData)
        {
            this.Data = rawData.Data;
            this.PacketType = rawData.PacketType;
        }

        #endregion
    }
}