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
//  <date>27-01-2013</date>
//  <summary>
//     The MarketDataBootstrapper.cs file.
//  </summary>
//  ===================================================================================

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TradeFx.Common.Interface;
using TradeFx.Common.MarketDataReader.Interface;
using TradeFx.Common.Service;

namespace TradeFx.Common.MarketDataReader.Tests
{
    /// <summary>The market data bootstrapper.</summary>
    [TestClass]
    public class MarketDataBootstrapper
    {
        #region Public Methods and Operators

        /// <summary>The dispose container.</summary>
        [TestCleanup]
        public void DisposeContainer()
        {
            ServiceManager.Instance.DisposeContainer();
        }

        /// <summary>The is all services initialized.</summary>
        [TestMethod]
        public void IsAllServicesInitialized()
        {
            MarketDataReader.MarketDataBootstrapper.RegisterMarketDataServices();
            foreach (var service in ServiceManager.Instance.ResolveAll<IService>())
            {
                Assert.AreEqual(true, service.Initialized);
            }
        }

        /// <summary>The is onada market data service initialized.</summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        [Ignore]
        public void IsOnadaMarketDataServiceInitialized()
        {
            MarketDataReader.MarketDataBootstrapper.RegisterMarketDataServices();
            var service = ServiceManager.Instance.Resolve<IOnadaMarketDataService>();
            Assert.AreNotEqual(null, service.GetMarketData());
            service.RegisterCurrencyPair();
        }

        /// <summary>The is market data service initialized.</summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        [Ignore]
        public void IsRegisterCurrencyBaseThrowException()
        {
            MarketDataReader.MarketDataBootstrapper.RegisterMarketDataServices();
            var service = ServiceManager.Instance.Resolve<IOnadaMarketDataService>();
            service.RegisterCurrencyPair();
        }

        /// <summary>The is true fx market data initialized.</summary>
        [TestMethod]
        public void IsTrueFxMarketDataInitialized()
        {
            MarketDataReader.MarketDataBootstrapper.RegisterMarketDataServices();
            var service = ServiceManager.Instance.Resolve<ITrueFxMarketDataService>();
            Assert.AreNotEqual(null, service.GetMarketData());
        }

        #endregion
    }
}