//  ===================================================================================
//  <copyright file="EventWrapper.cs" company="TechieNotes">
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
//  <date>13-03-2013</date>
//  <summary>
//     The EventWrapper.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Events
{
    /// <summary>The event wrapper.</summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public class EventWrapper<TEventArgs>
        where TEventArgs : EventArgs
    {
        #region Public Events

        /// <summary>The event.</summary>
        public event EventHandler<TEventArgs> Event;

        #endregion

        #region Public Methods and Operators

        /// <summary>The +.</summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        /// <param name="event">The event.</param>
        /// <returns></returns>
        public static EventWrapper<TEventArgs> operator +(
            EventWrapper<TEventArgs> eventWrapper, EventHandler<TEventArgs> @event)
        {
            eventWrapper.Event = (EventHandler<TEventArgs>)Delegate.Combine(eventWrapper.Event, @event);
            return eventWrapper;
        }

        /// <summary>The +.</summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        /// <param name="eventWrapperToAdd">The event wrapper to add.</param>
        /// <returns></returns>
        public static EventWrapper<TEventArgs> operator +(
            EventWrapper<TEventArgs> eventWrapper, EventWrapper<TEventArgs> eventWrapperToAdd)
        {
            eventWrapper.Event = (EventHandler<TEventArgs>)Delegate.Combine(eventWrapper.Event, eventWrapperToAdd.Event);
            return eventWrapper;
        }

        /// <summary>The -.</summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        /// <param name="event">The event.</param>
        /// <returns></returns>
        public static EventWrapper<TEventArgs> operator -(
            EventWrapper<TEventArgs> eventWrapper, EventHandler<TEventArgs> @event)
        {
            eventWrapper.Event = (EventHandler<TEventArgs>)Delegate.Remove(eventWrapper.Event, @event);
            return eventWrapper;
        }

        /// <summary>The -.</summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        /// <param name="eventWrapperToRemove">The event wrapper to remove.</param>
        /// <returns></returns>
        public static EventWrapper<TEventArgs> operator -(
            EventWrapper<TEventArgs> eventWrapper, EventWrapper<TEventArgs> eventWrapperToRemove)
        {
            eventWrapper.Event =
                (EventHandler<TEventArgs>)Delegate.Remove(eventWrapper.Event, eventWrapperToRemove.Event);
            return eventWrapper;
        }

        /// <summary>Checks if the Wrapped event contains the passed Handler </summary>
        /// <param name="handler">Handler whose existence is to be checked</param>
        /// <returns>true if the Wrapped event contains the handler</returns>
        public bool ContainsHandler(EventHandler<TEventArgs> handler)
        {
            if (this.Event != null)
            {
                var invocationList = this.Event.GetInvocationList();
                return Array.Exists(
                    invocationList, existingHandler => (existingHandler != null && existingHandler.Equals(handler)));
            }

            return false;
        }

        /// <summary>The fire event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="objEventArgs">The obj event args.</param>
        public void FireEvent(object sender, TEventArgs objEventArgs)
        {
            EventPublisher.RaiseEvent(this.Event, sender, objEventArgs);
        }

        /// <summary>The fire event async.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="objEventArgs">The obj event args.</param>
        public void FireEventAsync(object sender, TEventArgs objEventArgs)
        {
            EventPublisher.RaiseEventAsync(this.Event, sender, objEventArgs);
        }

        #endregion
    }
}