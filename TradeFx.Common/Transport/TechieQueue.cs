//  ===================================================================================
//  <copyright file="TechieQueue.cs" company="TechieNotes">
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
//     The TechieQueue.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TradeFx.Common.Transport
{
    /// <summary>The techie queue.</summary>
    public class TechieQueue
    {
        #region Fields

        /// <summary>The _queue.</summary>
        private readonly LinkedList<object> _queue = new LinkedList<object>();

        /// <summary>The _sync object.</summary>
        private readonly object _syncObject = new object();

        /// <summary>The _closed.</summary>
        private volatile bool _closed;

        /// <summary>The _count.</summary>
        private int _count;

        #endregion

        #region Public Events

        /// <summary>The item enqueued.</summary>
        public event EventHandler ItemEnqueued;

        #endregion

        #region Public Properties

        /// <summary>Gets the count.</summary>
        public int Count
        {
            get
            {
                return this._count;
            }
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether closed.</summary>
        protected bool Closed
        {
            get
            {
                return this._closed;
            }

            set
            {
                this._closed = value;
            }
        }

        /// <summary>Gets the sync object.</summary>
        protected object SyncObject
        {
            get
            {
                return this._syncObject;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The clear.</summary>
        public virtual void Clear()
        {
            lock (this._syncObject)
            {
                this._queue.Clear();
                this._count = 0;
            }
        }

        /// <summary>The close.</summary>
        public void Close()
        {
            lock (this._syncObject)
            {
                this.Clear();
                this.Closed = true;
            }
        }

        /// <summary>Dequeue an object</summary>
        /// <returns>The System.Object.</returns>
        public virtual object Dequeue()
        {
            lock (this._syncObject)
            {
                if (this._queue.Count == 0)
                {
                    throw new InvalidOperationException("Queue empty");
                }

                var returnObject = this._queue.First.Value;
                this._queue.RemoveFirst();
                Interlocked.Decrement(ref this._count);
                return returnObject;
            }
        }

        /// <summary>The dequeue batch.</summary>
        /// <param name="batchSize">The batch size.</param>
        /// <returns>The System.Collections.Generic.List`1[T -&gt; System.Object].</returns>
        public List<object> DequeueBatch(int batchSize)
        {
            lock (this._syncObject)
            {
                var count = Math.Min(batchSize, this._count);
                var items = new List<object>(count);
                for (var i = 0; i < count; i++)
                {
                    items.Add(this.Dequeue());
                }

                return items;
            }
        }

        /// <summary>Enqueue an object</summary>
        /// <param name="obj">Object to enqueue</param>
        /// <remarks>If EventEnabled is set to true then the Event will be signaled</remarks>
        /// <remarks>null can not be enqueued and will raise the assertion in Debug build</remarks>
        public void Enqueue(object obj)
        {
            this.EnqueueInternal(obj, false, true);
        }

        /// <summary>Enqueues the specified object at the front of the queue, such that it will be the next object that is
        ///     dequeued.</summary>
        /// <param name="obj">The object to be placed at the head of the queue.</param>
        public void EnqueueAtHead(object obj)
        {
            this.EnqueueInternal(obj, true, true);
        }

        /// <summary>The try dequeue.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public bool TryDequeue(out object obj)
        {
            lock (this._syncObject)
            {
                if (this._queue.Count == 0)
                {
                    obj = null;
                    return false;
                }

                obj = this.Dequeue();
                return true;
            }
        }

        #endregion

        #region Methods

        /// <summary>The enqueue internal.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="enqueueAtHead">The enqueue at head.</param>
        /// <param name="incrementCount">The increment count.</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual void EnqueueInternal(object obj, bool enqueueAtHead, bool incrementCount)
        {
            if (obj == null)
            {
                Debug.Fail("Trying to Enqueue a null object");
                return;
            }

            lock (this._syncObject)
            {
                if (this.Closed)
                {
                    throw new InvalidOperationException("Queue is closed: " + this.Name);
                }

                if (enqueueAtHead)
                {
                    this._queue.AddFirst(obj);
                }
                else
                {
                    this._queue.AddLast(obj);
                }

                if (incrementCount)
                {
                    Interlocked.Increment(ref this._count);
                }
            }

            this.OnItemEnqueued();
        }

        /// <summary>The on item enqueued.</summary>
        protected virtual void OnItemEnqueued()
        {
            // chose not to use EventPublisher with approval from JK
            var handlers = this.ItemEnqueued;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}