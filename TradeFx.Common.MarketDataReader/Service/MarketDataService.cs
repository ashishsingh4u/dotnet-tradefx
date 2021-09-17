//  ===================================================================================
//  <copyright file="MarketDataService.cs" company="TechieNotes">
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
//  <date>01-01-2013</date>
//  <summary>
//     The MarketDataService.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Events;
using TradeFx.Common.MarketDataReader.Interface;
using TradeFx.Common.Service;

namespace TradeFx.Common.MarketDataReader.Service
{
    /// <summary>The market data service.</summary>
    internal class MarketDataService : ServiceBase, IMarketDataService
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MarketDataService"/> class.</summary>
        /// <param name="aggregator">The aggregator.</param>
        public MarketDataService(IEventAggregator aggregator)
            : base(aggregator)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get market data.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string GetMarketData()
        {
            return string.Empty;
        }

        /// <summary>The register currency pair.</summary>
        public virtual void RegisterCurrencyPair()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}