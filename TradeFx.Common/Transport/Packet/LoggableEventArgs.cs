//  ===================================================================================
//  <copyright file="LoggableEventArgs.cs" company="TechieNotes">
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
//     The LoggableEventArgs.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Logging;

namespace TradeFx.Common.Transport.Packet
{
    /// <summary>The loggable event args.</summary>
    public abstract class LoggableEventArgs : EventArgs, ILoggable
    {
        #region Public Methods and Operators

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public virtual string ToLog()
        {
            return this.ToString();
        }

        #endregion
    }
}