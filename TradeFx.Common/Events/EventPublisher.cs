//  ===================================================================================
//  <copyright file="EventPublisher.cs" company="TechieNotes">
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
//  <date>11-03-2013</date>
//  <summary>
//     The EventPublisher.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using log4net;

using TradeFx.Common.Culture;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Provides methods for publishing events.
    /// </summary>
    public static class EventPublisher
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods and Operators

        /// <summary>Raises the event with thread-safety making sure all subscribers receive the notification.</summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="sender">The event originator.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public static void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> method, object sender, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            RaiseEventInternal(method, sender, eventArgs);
        }

        /// <summary>Raises the event with thread-safety making sure all subscribers receive the notification.  Raises the event with EventArgs.Empty.</summary>
        /// <param name="method">The method.</param>
        /// <param name="sender">The event originator.</param>
        public static void RaiseEvent(EventHandler method, object sender)
        {
            RaiseEventInternal(method, sender, EventArgs.Empty);
        }

        /// <summary>Raises the event asynchronously and with thread-safety making sure all subscribers receive the
        ///     notification.</summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="sender">The event originator.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public static void RaiseEventAsync<TEventArgs>(
            EventHandler<TEventArgs> method, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        try
                        {
                            AppCulture.SetThreadCulture();

                            RaiseEventInternal(method, sender, eventArgs);
                        }
                        catch (Exception e)
                        {
                            Debug.Assert(false, CreateAssertionMessage(e));
                            var ex = CreateLogFriendlyException(e, eventArgs);
                            Log.Error(ex.Message, ex);
                        }
                    });
        }

        /// <summary>Raises the property changed event. This event is exposed via the <see cref="INotifyPropertyChanged"/>
        ///     interface and is used to facilitate data binding of customer data sources.</summary>
        /// <param name="method">The handlers.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public static void RaisePropertyChangedEvent(
            PropertyChangedEventHandler method, object sender, PropertyChangedEventArgs eventArgs)
        {
            RaiseEventInternal(method, sender, eventArgs);
        }

        #endregion

        #region Methods

        /// <summary>The create assertion message.</summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The System.String.</returns>
        private static string CreateAssertionMessage(Exception exception)
        {
            return string.Format(
                "Exception thrown by event handler: {0}\nFrom {1}\n{2}\nAsserted", 
                exception.Message, 
                exception.StackTrace, 
                (exception.InnerException != null) ? "\nInner " + exception.InnerException : string.Empty);
        }

        /// <summary>The create log friendly exception.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventArgs">The event args.</param>
        /// <returns>The System.Exception.</returns>
        private static Exception CreateLogFriendlyException(Exception exception, EventArgs eventArgs)
        {
            var logFriendlyException = new Exception("Exception caught in EventPublisher", exception);
            logFriendlyException.Source = eventArgs.ToString();

            return logFriendlyException;
        }

        /// <summary>This method has been optimised for the two use cases above where method is either of type
        ///     EventHandler&lt;TEventArgs&gt; or PropertyChangedEventHandler. This is to avoid the use of
        ///     DynamicInvoke which is much slower than direct invocation:
        ///     http://stackoverflow.com/questions/932699/what-is-the-difference-between-calling-a-delegate-directly-using-dynamicinvoke</summary>
        /// <param name="method">The method.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event Args.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RaiseEventInternal<TEventArgs>(Delegate method, object sender, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            Debug.Assert(eventArgs != null);

            var dlg = method;
            if (dlg == null)
            {
                return;
            }

            foreach (var oneCall in dlg.GetInvocationList())
            {
                try
                {
                    var genericHandler = oneCall as EventHandler<TEventArgs>;
                    if (genericHandler != null)
                    {
                        genericHandler(sender, eventArgs);
                        continue;
                    }

                    var propertyChangedEventHandler = oneCall as PropertyChangedEventHandler;
                    if (propertyChangedEventHandler != null)
                    {
                        propertyChangedEventHandler(sender, eventArgs as PropertyChangedEventArgs);
                        continue;
                    }

                    // Should never reach here
                    oneCall.DynamicInvoke(sender, eventArgs);
                }
                catch (Exception exception)
                {
                    Debug.Assert(false, CreateAssertionMessage(exception));
                    var ex = CreateLogFriendlyException(exception, eventArgs);
                    Log.Error(ex.Message, ex);
                }
            }
        }

        #endregion
    }
}