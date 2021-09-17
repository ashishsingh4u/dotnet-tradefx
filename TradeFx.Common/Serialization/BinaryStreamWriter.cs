//  ===================================================================================
//  <copyright file="BinaryStreamWriter.cs" company="TechieNotes">
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
//     The BinaryStreamWriter.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.IO;

using TradeFx.Common.Serialization.Interfaces;

namespace TradeFx.Common.Serialization
{
    /// <summary>The binary stream writer.</summary>
    public class BinaryStreamWriter : IStreamWriter, IDisposable
    {
        #region Static Fields

        /// <summary>The _java base date.</summary>
        private static readonly DateTime _javaBaseDate = new DateTime(621355968000000000L);

        #endregion

        #region Fields

        /// <summary>The _memory stream.</summary>
        private readonly MemoryStream _memoryStream;

        /// <summary>The _writer.</summary>
        private readonly BinaryWriter _writer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryStreamWriter" /> class.
        /// </summary>
        public BinaryStreamWriter()
        {
            this._memoryStream = new MemoryStream();
            this._writer = new BinaryWriter(this._memoryStream);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this._writer.Close();
            this._memoryStream.Close();
        }

        /// <summary>The get bytes.</summary>
        /// <returns>The System.Byte[].</returns>
        public byte[] GetBytes()
        {
            return this._memoryStream.ToArray();
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(int data)
        {
            this._writer.Write(data);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(bool data)
        {
            this._writer.Write(data);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(string data)
        {
            if (data == null)
            {
                this._writer.Write(string.Empty);
            }
            else
            {
                this._writer.Write(data);
            }
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(byte data)
        {
            this._writer.Write(data);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(double data)
        {
            this._writer.Write(data);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(decimal data)
        {
            this._writer.Write(data);
        }

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(long data)
        {
            this._writer.Write(data);
        }

        // To simplify things a bit we serialise/deserialise date/time as an offset in milliseconds 
        // from the java base date (1/1/1970). Use UTC to serialise/deserialise dates
        // This makes java code very simple; we lose a bit iof precision but it's acceptable tradeoff

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        public void Write(DateTime data)
        {
            if (data == DateTime.MinValue)
            {
                this.Write(0L);
            }
            else
            {
                this.Write(
                    (long)TimeZone.CurrentTimeZone.ToUniversalTime(data).Subtract(_javaBaseDate).TotalMilliseconds);
            }
        }

        #endregion
    }
}