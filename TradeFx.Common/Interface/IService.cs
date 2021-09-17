//  ===================================================================================
//  <copyright file="IService.cs" company="TechieNotes">
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
//     The IService.cs file.
//  </summary>
//  ===================================================================================

using System;

using TradeFx.Common.Events;

namespace TradeFx.Common.Interface
{
    /// <summary>The Service interface.</summary>
    public interface IService : IDisposable
    {
        #region Public Properties

        /// <summary>Gets the event aggregator.</summary>
        IEventAggregator EventAggregator { get; }

        /// <summary>Gets a value indicating whether initialized.</summary>
        bool Initialized { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The initialize.</summary>
        void Initialize();

        #endregion
    }
}