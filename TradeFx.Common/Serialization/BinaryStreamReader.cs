//  ===================================================================================
//  <copyright file="BinaryStreamReader.cs" company="TechieNotes">
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
//     The BinaryStreamReader.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;

using TradeFx.Common.Serialization.Interfaces;

namespace TradeFx.Common.Serialization
{
    /// <summary>The binary stream reader.</summary>
    public class BinaryStreamReader : IStreamReader
    {
        #region Static Fields

        /// <summary>The _java base date.</summary>
        private static DateTime _javaBaseDate = new DateTime(621355968000000000L);

        #endregion

        #region Fields

        /// <summary>The _memory stream.</summary>
        private readonly MemoryStream _memoryStream;

        /// <summary>The _reader.</summary>
        private readonly BinaryReader _reader;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BinaryStreamReader"/> class.</summary>
        /// <param name="bytes">The bytes.</param>
        public BinaryStreamReader(byte[] bytes)
        {
            this._memoryStream = new MemoryStream();
            this._memoryStream.Write(bytes, 0, bytes.Length);
            this._reader = new BinaryReader(this._memoryStream);
            this._memoryStream.Position = 0;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this._memoryStream.Close();
            this._reader.Close();
        }

        /// <summary>The read bool.</summary>
        /// <returns>The System.Boolean.</returns>
        public bool ReadBool()
        {
            return this._reader.ReadBoolean();
        }

        /// <summary>The read byte.</summary>
        /// <returns>The System.Byte.</returns>
        public byte ReadByte()
        {
            return this._reader.ReadByte();
        }

        /// <summary>The read date time.</summary>
        /// <returns>The System.DateTime.</returns>
        public DateTime ReadDateTime()
        {
            var data = this.ReadLong();
            if (data == 0)
            {
                return DateTime.MinValue;
            }

            return TimeZone.CurrentTimeZone.ToLocalTime(_javaBaseDate.AddMilliseconds(data));
        }

        /// <summary>The read decimal.</summary>
        /// <returns>The System.Decimal.</returns>
        public decimal ReadDecimal()
        {
            return this._reader.ReadDecimal();
        }

        /// <summary>The read double.</summary>
        /// <returns>The System.Double.</returns>
        public double ReadDouble()
        {
            return this._reader.ReadDouble();
        }

        /// <summary>The read int.</summary>
        /// <returns>The System.Int32.</returns>
        public int ReadInt()
        {
            return this._reader.ReadInt32();
        }

        /// <summary>The read long.</summary>
        /// <returns>The System.Int64.</returns>
        public long ReadLong()
        {
            return this._reader.ReadInt64();
        }

        /// <summary>The read string.</summary>
        /// <returns>The System.String.</returns>
        public string ReadString()
        {
            return this._reader.ReadString();
        }

        #endregion

        // To simplify things a bit we serialise/deserialise date/time as an offset in milliseconds 
        // from the java base date (1/1/1970). The date in the stream is an UTC date - convert into server local time
    }
}