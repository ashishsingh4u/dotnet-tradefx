// ===================================================================================
// <copyright file="IStreamWriter.cs" company="TechieNotes">
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
//    The IStreamWriter.cs file.
// </summary>
// ===================================================================================

using System;

namespace TradeFx.Common.Serialization.Interfaces
{
    /// <summary>The StreamWriter interface.</summary>
    public interface IStreamWriter : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>The get bytes.</summary>
        /// <returns>The System.Byte[].</returns>
        byte[] GetBytes();

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(int data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(bool data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(string data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(byte data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(double data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(long data);

        /// <summary>The write.</summary>
        /// <param name="data">The data.</param>
        void Write(DateTime data);

        #endregion
    }
}