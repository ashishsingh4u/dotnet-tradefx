//  ===================================================================================
//  <copyright file="EventBase.cs" company="TechieNotes">
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
//     The EventBase.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Defines a base class to publish and subscribe to events.
    /// </summary>
    public abstract class EventBase
    {
        #region Fields

        /// <summary>The _subscriptions.</summary>
        private readonly List<IEventSubscription> _subscriptions = new List<IEventSubscription>();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the list of current subscriptions.
        /// </summary>
        /// <value>The current subscribers.</value>
        protected ICollection<IEventSubscription> Subscriptions
        {
            get
            {
                return _subscriptions;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Returns <see langword="true"/> if there is a subscriber matching <see cref="SubscriptionToken"/>.</summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is a <see cref="SubscriptionToken"/> that matches; otherwise<see langword="false"/>
        ///     .</returns>
        public virtual bool Contains(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                var subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                return subscription != null;
            }
        }

        /// <summary>Removes the subscriber matching the <seealso cref="SubscriptionToken"/>.</summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                var subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                if (subscription != null)
                {
                    Subscriptions.Remove(subscription);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>Calls all the execution strategies exposed by the list of <see cref="IEventSubscription"/>.</summary>
        /// <param name="arguments">The arguments that will be passed to the listeners.</param>
        /// <remarks>Before executing the strategies, this class will prune all the subscribers from the
        ///     list that return a <see langword="null"/> <see cref="Action{T}"/> when calling the<see cref="IEventSubscription.GetExecutionStrategy"/>
        ///     method.</remarks>
        protected virtual void InternalPublish(params object[] arguments)
        {
            var executionStrategies = PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        /// <summary>Adds the specified <see cref="IEventSubscription"/> to the subscribers' collection.</summary>
        /// <param name="eventSubscription">The subscriber.</param>
        /// <returns>The <see cref="SubscriptionToken"/> that uniquely identifies every subscriber.</returns>
        /// <remarks>Adds the subscription to the internal list and assigns it a new <see cref="SubscriptionToken"/>.</remarks>
        protected virtual SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
        {
            if (eventSubscription == null)
            {
                throw new ArgumentNullException("eventSubscription");
            }

            eventSubscription.SubscriptionToken = new SubscriptionToken(Unsubscribe);

            lock (Subscriptions)
            {
                Subscriptions.Add(eventSubscription);
            }

            return eventSubscription.SubscriptionToken;
        }

        /// <summary>The prune and return strategies.</summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            var returnList = new List<Action<object[]>>();

            lock (Subscriptions)
            {
                for (var i = Subscriptions.Count - 1; i >= 0; i--)
                {
                    var listItem = _subscriptions[i].GetExecutionStrategy();

                    if (listItem == null)
                    {
                        // Prune from main list. Log?
                        _subscriptions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }

        #endregion
    }
}