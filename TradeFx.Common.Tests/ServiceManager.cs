//  ===================================================================================
//  <copyright file="ServiceManager.cs" company="TechieNotes">
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
//     The ServiceManager.cs file.
//  </summary>
//  ===================================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TradeFx.Common.Interface;

namespace TradeFx.Common.Tests
{
    /// <summary>The service manager.</summary>
    [TestClass]
    public class ServiceManager
    {
        #region Public Methods and Operators

        /// <summary>The dispose container.</summary>
        [TestCleanup]
        public void DisposeContainer()
        {
            Service.ServiceManager.Instance.DisposeContainer();
        }

        /// <summary>The initialize.</summary>
        [TestMethod]
        public void Initialize()
        {
            Service.ServiceManager.Instance.Initialize();
        }

        /// <summary>The is registered service count not zero.</summary>
        [TestMethod]
        public void IsRegisteredServiceCountZero()
        {
            Service.ServiceManager.Instance.RegisterTypes();
            Assert.AreEqual(0, Service.ServiceManager.Instance.ResolveAll<IService>().Count);
        }

        /// <summary>The is service manager instance not null.</summary>
        [TestMethod]
        public void IsServiceManagerInstanceNotNull()
        {
            Assert.AreNotEqual(null, Service.ServiceManager.Instance);
        }

        #endregion
    }
}