//  ===================================================================================
//  <copyright file="IDelegateReference.cs" company="TechieNotes">
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
//  <date>31-12-2012</date>
//  <summary>
//     The IDelegateReference.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Represents a reference to a <see cref="Delegate" />.
    /// </summary>
    public interface IDelegateReference
    {
        #region Public Properties

        /// <summary>
        ///     Gets the referenced <see cref="Delegate" /> object.
        /// </summary>
        /// <value>
        ///     A <see cref="Delegate" /> instance if the target is valid; otherwise <see langword="null" />.
        /// </value>
        Delegate Target { get; }

        #endregion
    }
}