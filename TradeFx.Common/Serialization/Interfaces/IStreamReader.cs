//  ===================================================================================
//  <copyright file="IStreamReader.cs" company="TechieNotes">
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
//     The IStreamReader.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Serialization.Interfaces
{
    /// <summary>The StreamReader interface.</summary>
    public interface IStreamReader : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>The read bool.</summary>
        /// <returns>The System.Boolean.</returns>
        bool ReadBool();

        /// <summary>The read byte.</summary>
        /// <returns>The System.Byte.</returns>
        byte ReadByte();

        /// <summary>The read date time.</summary>
        /// <returns>The System.DateTime.</returns>
        DateTime ReadDateTime();

        /// <summary>The read double.</summary>
        /// <returns>The System.Double.</returns>
        double ReadDouble();

        /// <summary>The read int.</summary>
        /// <returns>The System.Int32.</returns>
        int ReadInt();

        /// <summary>The read long.</summary>
        /// <returns>The System.Int64.</returns>
        long ReadLong();

        /// <summary>The read string.</summary>
        /// <returns>The System.String.</returns>
        string ReadString();

        #endregion
    }
}