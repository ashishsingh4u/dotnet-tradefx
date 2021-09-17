//  ===================================================================================
//  <copyright file="TestData.cs" company="TechieNotes">
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
//  <date>17-03-2013</date>
//  <summary>
//     The TestData.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Transport.Packet;

namespace TradeFx.Connection.Data
{
    /// <summary>
    ///     Data send to test
    /// </summary>
    [Serializable]
    public class TestData : ISendable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Gets a value indicating whether Conflate.
        /// </summary>
        public bool Conflate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets ConflateKey.
        /// </summary>
        /// <exception cref="NotImplementedException">
        ///     Currently this is not implemented.
        /// </exception>
        public Key ConflateKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets PacketType.
        /// </summary>
        public PacketType PacketType
        {
            get
            {
                return PacketType.BinaryFormatter;
            }
        }

        /// <summary>
        ///     Gets or sets Roll.
        /// </summary>
        public int Roll { get; set; }

        #endregion
    }
}