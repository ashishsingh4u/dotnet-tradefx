//  ===================================================================================
//  <copyright file="WindowService.cs" company="TechieNotes">
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
//     The WindowService.cs file.
//  </summary>
//  ===================================================================================

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Rhino.Mocks;

using TradeFx.Common.Interface;
using TradeFx.Common.MarketDataReader.Interface;
using TradeFx.Common.Service;

namespace TradeFx.Server.MarketDataReader.Tests
{
    /// <summary>The window service.</summary>
    [TestClass]
    public class WindowService
    {
        #region Public Methods and Operators

        /// <summary>The dispose container.</summary>
        [TestCleanup]
        public void DisposeContainer()
        {
            ServiceManager.Instance.DisposeContainer();
        }

        /// <summary>The is service started.</summary>
        [TestMethod]
        //[ExpectedException(typeof(NotImplementedException))]
        public void IsAllServiceRegistered()
        {
            MarketDataReader.WindowService.RegisterServices();
            var instances = ServiceManager.Instance.ResolveAll<IService>();
            Assert.AreNotEqual(0, instances.Count);

            // var service = MockRepository.GenerateMock<IOnadaMarketDataService>();
            var repository = new MockRepository();
            var service = repository.StrictMock<IOnadaMarketDataService>();

            // service.Stub(s => { s.GetMarketData(); }).Return("Hello");
            Expect.Call(service.GetMarketData()).Return("Hello");
            // Expect.Call(service.Initialize).Throw(new NotImplementedException());
            Expect.Call(service.Initialize);
            service.Replay();
            Assert.AreEqual("Hello", service.GetMarketData());
            service.Initialize();
        }

        /// <summary>The is service started.</summary>
        [TestMethod]
        public void InitializeServices()
        {
            MarketDataReader.WindowService.RegisterServices();
        }

        #endregion
    }
}