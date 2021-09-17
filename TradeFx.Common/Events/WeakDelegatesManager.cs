//  ===================================================================================
//  <copyright file="WeakDelegatesManager.cs" company="TechieNotes">
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
//     The WeakDelegatesManager.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeFx.Common.Events
{
    /// <summary>The weak delegates manager.</summary>
    internal class WeakDelegatesManager
    {
        #region Fields

        /// <summary>The listeners.</summary>
        private readonly List<DelegateReference> listeners = new List<DelegateReference>();

        #endregion

        #region Public Methods and Operators

        /// <summary>The add listener.</summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(Delegate listener)
        {
            this.listeners.Add(new DelegateReference(listener, false));
        }

        /// <summary>The raise.</summary>
        /// <param name="args">The args.</param>
        public void Raise(params object[] args)
        {
            this.listeners.RemoveAll(listener => listener.Target == null);

            foreach (var handler in
                this.listeners.ToList().Select(listener => listener.Target).Where(listener => listener != null))
            {
                handler.DynamicInvoke(args);
            }
        }

        /// <summary>The remove listener.</summary>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(Delegate listener)
        {
            this.listeners.RemoveAll(
                reference =>
                    {
                        // Remove the listener, and prune collected listeners
                        var target = reference.Target;
                        return listener.Equals(target) || target == null;
                    });
        }

        #endregion
    }
}