//  ===================================================================================
//  <copyright file="MarketDataBootstrapper.cs" company="TechieNotes">
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
//     The MarketDataBootstrapper.cs file.
//  </summary>
//  ===================================================================================

using TradeFx.Common.Events;
using TradeFx.Common.MarketDataReader.Interface;
using TradeFx.Common.MarketDataReader.Service;
using TradeFx.Common.Service;

namespace TradeFx.Common.MarketDataReader
{
    /// <summary>The market data bootstrapper.</summary>
    public class MarketDataBootstrapper
    {
        #region Public Methods and Operators

        /// <summary>The register market data services.</summary>
        public static void RegisterMarketDataServices()
        {
            CommonServiceBootstrapper.RegisterCommonServices();
            ServiceManager.Instance.RegisterType<ITrueFxMarketDataService, TrueFxMarketDataService>();

            // ServiceManager.Instance.RegisterType<IOnadaMarketDataService, OnadaMarketDataService>();
            ServiceManager.Instance.Initialize();
        }

        #endregion
    }
}