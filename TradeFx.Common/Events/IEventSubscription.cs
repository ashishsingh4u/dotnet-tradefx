//  ===================================================================================
//  <copyright file="IEventSubscription.cs" company="TechieNotes">
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
//     The IEventSubscription.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Defines a contract for an event subscription to be used by <see cref="EventBase" />.
    /// </summary>
    public interface IEventSubscription
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a <see cref="Events.SubscriptionToken" /> that identifies this <see cref="IEventSubscription" />.
        /// </summary>
        /// <value>
        ///     A token that identifies this <see cref="IEventSubscription" />.
        /// </value>
        SubscriptionToken SubscriptionToken { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>
        ///     An <see cref="Action{T}" /> with the execution strategy, or <see langword="null" /> if the
        ///     <see
        ///         cref="IEventSubscription" />
        ///     is no longer valid.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Action<object[]> GetExecutionStrategy();

        #endregion
    }
}