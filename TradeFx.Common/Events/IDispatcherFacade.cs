//  ===================================================================================
//  <copyright file="IDispatcherFacade.cs" company="TechieNotes">
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
//  <date>31-12-2012</date>
//  <summary>
//     The IDispatcherFacade.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Defines the interface for invoking methods through a Dispatcher Facade
    /// </summary>
    public interface IDispatcherFacade
    {
        #region Public Methods and Operators

        /// <summary>Dispatches an invocation to the method received as parameter.</summary>
        /// <typeparam name="TPayload">Event payload</typeparam>
        /// <param name="method">Method to be invoked.</param>
        /// <param name="arg">Arguments to pass to the invoked method.</param>
        void BeginInvoke<TPayload>(Action<TPayload> method, TPayload arg);

        #endregion
    }
}