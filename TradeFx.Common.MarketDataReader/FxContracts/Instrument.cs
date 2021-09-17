//  ===================================================================================
//  <copyright file="Instrument.cs" company="TechieNotes">
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
//  <date>12-01-2013</date>
//  <summary>
//     The Instrument.cs file.
//  </summary>
//  ===================================================================================

using System.Diagnostics.CodeAnalysis;

namespace TradeFx.Common.MarketDataReader.FxContracts
{
    /// <summary>The instrument.</summary>
    internal class Instrument
    {
        #region Public Properties

        /// <summary>Gets or sets the display name.</summary>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the extra precision.</summary>
        public int ExtraPrecision { get; set; }

        /// <summary>Gets or sets the pip.</summary>
        public string Pip { get; set; }

        /// <summary>Gets or sets the pip location.</summary>
        public int PipLocation { get; set; }

        /// <summary>Gets or sets the instrument.</summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", 
            Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable InconsistentNaming Jason entity please don't rename.
        public string instrument { get; set; }

        #endregion

        // ReSharper restore InconsistentNaming
    }
}