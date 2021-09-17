//  ===================================================================================
//  <copyright file="IocpPoolSignalConflator.cs" company="TechieNotes">
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
//     The IocpPoolSignalConflator.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Threading;

namespace TradeFx.Common.Transport
{
    /// <summary>The iocp pool signal conflator.</summary>
    internal class IocpPoolSignalConflator : SignalConflator
    {
        #region Fields

        /// <summary>The _trigger completion thread.</summary>
        private readonly AutoResetEvent _triggerCompletionThread;

        /// <summary>The _wait handle.</summary>
        private readonly RegisteredWaitHandle _waitHandle;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="IocpPoolSignalConflator"/> class.</summary>
        /// <param name="callback">The callback.</param>
        public IocpPoolSignalConflator(SignalCallback callback)
            : base(callback)
        {
            this._triggerCompletionThread = new AutoResetEvent(false);
            this._waitHandle = ThreadPool.UnsafeRegisterWaitForSingleObject(
                this._triggerCompletionThread, 
                (state, timeout) => this.ConflationCallback(), 
                null, 
                Timeout.Infinite, 
                false);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="IocpPoolSignalConflator" /> class.
        /// </summary>
        ~IocpPoolSignalConflator()
        {
            this._closed = true;
            this.Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._waitHandle.Unregister(this._triggerCompletionThread);
            }
            else
            {
                this._waitHandle.Unregister(null);
            }

            this._triggerCompletionThread.Close();

            GC.SuppressFinalize(this);

            base.Dispose(disposing);
        }

        /// <summary>The invoke conflation callback.</summary>
        protected override void InvokeConflationCallback()
        {
            this._triggerCompletionThread.Set();
        }

        #endregion
    }
}