//  ===================================================================================
//  <copyright file="DelegateReference.cs" company="TechieNotes">
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
//     The DelegateReference.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Reflection;

namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Represents a reference to a <see cref="Delegate" /> that may contain a
    ///     <see cref="WeakReference" /> to the target. This class is used
    ///     internally by the Composite Application Library.
    /// </summary>
    public class DelegateReference : IDelegateReference
    {
        #region Fields

        /// <summary>The _delegate.</summary>
        private readonly Delegate _delegate;

        /// <summary>The _delegate type.</summary>
        private readonly Type _delegateType;

        /// <summary>The _method.</summary>
        private readonly MethodInfo _method;

        /// <summary>The _weak reference.</summary>
        private readonly WeakReference _weakReference;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DelegateReference"/> class. Initializes a new instance of<see cref="DelegateReference"/>
        ///     .</summary>
        /// <param name="delegate">The original <see cref="Delegate"/> to create a reference for.</param>
        /// <param name="keepReferenceAlive">If <see langword="false"/> the class will create a weak reference to the delegate, allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.</param>
        /// <exception cref="ArgumentNullException">If the passed <paramref name="delegate"/> is not assignable to <see cref="Delegate"/>.</exception>
        public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException("delegate");
            }

            if (keepReferenceAlive)
            {
                this._delegate = @delegate;
            }
            else
            {
                _weakReference = new WeakReference(@delegate.Target);
                _method = @delegate.Method;
                _delegateType = @delegate.GetType();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="Delegate" /> (the target) referenced by the current <see cref="DelegateReference" /> object.
        /// </summary>
        /// <value>
        ///     <see langword="null" /> if the object referenced by the current <see cref="DelegateReference" /> object has been garbage collected; otherwise, a reference to the
        ///     <see
        ///         cref="Delegate" />
        ///     referenced by the current <see cref="DelegateReference" /> object.
        /// </value>
        public Delegate Target
        {
            get
            {
                if (_delegate != null)
                {
                    return _delegate;
                }
                else
                {
                    return TryGetDelegate();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>The try get delegate.</summary>
        /// <returns>
        ///     The <see cref="Delegate" />.
        /// </returns>
        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return Delegate.CreateDelegate(_delegateType, null, _method);
            }

            var target = _weakReference.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(_delegateType, target, _method);
            }

            return null;
        }

        #endregion
    }
}