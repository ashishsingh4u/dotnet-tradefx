//  ===================================================================================
//  <copyright file="Key.cs" company="TechieNotes">
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
//     The Key.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Logging;

namespace TradeFx.Common.Transport.Packet
{
    /// <summary>The DispatchableKey interface.</summary>
    /// <summary>The key.</summary>
    [Serializable]
    public abstract class Key : ILoggable
    {
        #region Public Methods and Operators

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public abstract override bool Equals(object obj);

        /// <summary>The get hash code.</summary>
        /// <returns>The System.Int32.</returns>
        public abstract override int GetHashCode();

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public abstract string ToLog();

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override sealed string ToString()
        {
            return this.ToLog();
        }

        #endregion
    }
}