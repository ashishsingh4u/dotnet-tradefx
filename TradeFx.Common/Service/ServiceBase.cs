//  ===================================================================================
//  <copyright file="ServiceBase.cs" company="TechieNotes">
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
//     The ServiceBase.cs file.
//  </summary>
//  ===================================================================================

using TradeFx.Common.Events;
using TradeFx.Common.Interface;

namespace TradeFx.Common.Service
{
    /// <summary>The service base.</summary>
    public class ServiceBase : IService
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServiceBase"/> class.</summary>
        /// <param name="aggregator">The aggregator.</param>
        public ServiceBase(IEventAggregator aggregator)
        {
            EventAggregator = aggregator;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the event aggregator.</summary>
        public IEventAggregator EventAggregator { get; private set; }

        /// <summary>Gets a value indicating whether initialized.</summary>
        public bool Initialized { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public virtual void Dispose()
        {
        }

        /// <summary>The initialize.</summary>
        public virtual void Initialize()
        {
            Initialized = true;
        }

        #endregion
    }
}