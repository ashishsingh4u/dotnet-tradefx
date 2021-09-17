//  ===================================================================================
//  <copyright file="PriceResponse.cs" company="TechieNotes">
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
//     The PriceResponse.cs file.
//  </summary>
//  ===================================================================================

using System.Collections.Generic;

namespace TradeFx.Common.MarketDataReader.FxContracts
{
    /// <summary>The price response.</summary>
    internal class PricesResponse
    {
        #region Public Properties

        /// <summary>Gets or sets the prices.</summary>
        public List<Price> Prices { get; set; }

        /// <summary>Gets or sets the time.</summary>
        public long Time { get; set; }

        #endregion
    }
}