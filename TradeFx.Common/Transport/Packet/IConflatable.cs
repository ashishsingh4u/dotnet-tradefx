//  ===================================================================================
//  <copyright file="IConflatable.cs" company="TechieNotes">
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
//     The IConflatable.cs file.
//  </summary>
//  ===================================================================================
namespace TradeFx.Common.Transport.Packet
{
    /// <summary>
    ///     Conflation support
    /// </summary>
    public interface IConflatable
    {
        #region Public Properties

        /// <summary>
        ///     Return true to indicate ability to conflate. ConflateKey property should be used to compare objects for conflation purposes
        /// </summary>
        /// <remarks>See </remarks>
        /// <seealso cref="ConflatedQueue" />
        bool Conflate { get; }

        /// <summary>
        ///     Conflate key used to compare objects for conflation
        /// </summary>
        Key ConflateKey { get; }

        #endregion
    }
}