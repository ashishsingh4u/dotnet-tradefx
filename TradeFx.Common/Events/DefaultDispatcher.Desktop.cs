//  ===================================================================================
//  <copyright file="DefaultDispatcher.Desktop.cs" company="TechieNotes">
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
//     The DefaultDispatcher.Desktop.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Threading;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Wraps the Application Dispatcher.
    /// </summary>
    public class DefaultDispatcher : IDispatcherFacade
    {
        #region Public Methods and Operators

        /// <summary>Forwards the BeginInvoke to the current application's.</summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="method">Method to be invoked.</param>
        /// <param name="args">The args.</param>
        public void BeginInvoke<TPayload>(Action<TPayload> method, TPayload args)
        {
            if (SynchronizationContext.Current != null)
            {
                SynchronizationContext.Current.Post((state) => method.Invoke(args), null);
            }
        }

        #endregion
    }
}