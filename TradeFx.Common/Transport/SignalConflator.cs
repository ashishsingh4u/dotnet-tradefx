//  ===================================================================================
//  <copyright file="SignalConflator.cs" company="TechieNotes">
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
//     The SignalConflator.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Reflection;
using System.Threading;

using log4net;

using TradeFx.Common.Culture;
using TradeFx.Common.Helpers;

namespace TradeFx.Common.Transport
{
    /// <summary>The signal callback.</summary>
    public delegate void SignalCallback();

    /// <summary>The signal conflator.</summary>
    public abstract class SignalConflator : IDisposable
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        /// <summary>The _closed.</summary>
        protected volatile bool _closed;

        /// <summary>The _callback.</summary>
        private readonly SignalCallback _callback;

        /// <summary>The _signal count.</summary>
        private long _signalCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SignalConflator"/> class.</summary>
        /// <param name="callback">The callback.</param>
        protected SignalConflator(SignalCallback callback)
        {
            ArgumentValidation.CheckForNullReference(callback, "callback");

            this._closed = false;
            this._callback = callback;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this._closed = true;
            this.Dispose(true);
        }

        /// <summary>The signal.</summary>
        public void Signal()
        {
            if (this._closed || Interlocked.Increment(ref this._signalCount) != 1)
            {
                return;
            }

            this.InvokeConflationCallback();
        }

        #endregion

        #region Methods

        /// <summary>The conflation callback.</summary>
        protected void ConflationCallback()
        {
            AppCulture.SetThreadCulture();

            long currentSignalCount;
            do
            {
                if (this._closed)
                {
                    Interlocked.Exchange(ref this._signalCount, 0);
                    return;
                }

                currentSignalCount = Interlocked.Read(ref this._signalCount);

                try
                {
                    this._callback();
                }
                catch (Exception ex)
                {
                    Log.Error("Callback failed", ex);
                }
            }
            while (Interlocked.CompareExchange(ref this._signalCount, 0, currentSignalCount) != currentSignalCount);
        }

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>The invoke conflation callback.</summary>
        protected abstract void InvokeConflationCallback();

        #endregion
    }
}