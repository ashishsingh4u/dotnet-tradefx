//  ===================================================================================
//  <copyright file="CommonServiceBootstrapper.cs" company="TechieNotes">
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
//     The CommonServiceBootstrapper.cs file.
//  </summary>
//  ===================================================================================

using TradeFx.Common.Events;
using TradeFx.Common.Interface;
using TradeFx.Common.Service;

namespace TradeFx.Common
{
    /// <summary>The common service bootstrapper.</summary>
    public class CommonServiceBootstrapper
    {
        #region Public Methods and Operators

        /// <summary>The register market data services.</summary>
        public static void RegisterCommonServices()
        {
            ServiceManager.Instance.RegisterNonServiceType<IEventAggregator, EventAggregator>();
        }

        /// <summary>The register message bus service.</summary>
        public static void RegisterMessageBusService()
        {
            ServiceManager.Instance.RegisterType<IMessageBusService, MessageBusService>();
        }

        #endregion
    }
}