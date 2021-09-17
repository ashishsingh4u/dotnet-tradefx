//  ===================================================================================
//  <copyright file="EventAggregator.cs" company="TechieNotes">
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
//     The EventAggregator.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Implements <see cref="IEventAggregator" />.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region Fields

        /// <summary>The events.</summary>
        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same
        ///     <typeparamref
        ///         name="TEventType" />
        ///     returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">
        ///     The type of event to get. This must inherit from <see cref="EventBase" />.
        /// </typeparam>
        /// <returns>
        ///     A singleton instance of an event object of type <typeparamref name="TEventType" />.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            EventBase existingEvent = null;

            if (!this.events.TryGetValue(typeof(TEventType), out existingEvent))
            {
                var newEvent = new TEventType();
                this.events[typeof(TEventType)] = newEvent;

                return newEvent;
            }
            else
            {
                return (TEventType)existingEvent;
            }
        }

        #endregion
    }
}