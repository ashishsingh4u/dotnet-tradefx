// ===================================================================================
// <copyright file="Consumers.cs" company="TechieNotes">
// ===================================================================================
//  TechieNotes Utilities & Best Practices
//  Samples and Guidelines for Winform & ASP.net development
// ===================================================================================
//  Copyright (c) TechieNotes.  All rights reserved.
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//  FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
// ===================================================================================
// </copyright>
// <author>Ashish Singh</author>
// <email>mailto:ashishsingh4u@gmail.com</email>
// <date>08-09-2012</date>
// <summary>
//    The Consumers.cs file.
// </summary>
// ===================================================================================

using System;
using System.Collections.Generic;

namespace TradeFx.Common.Transport
{
    /// <summary>The consumers.</summary>
    public class Consumers
    {
        #region Fields

        /// <summary>The _all types consumers.</summary>
        private readonly List<TechieQueue> _allTypesConsumers = new List<TechieQueue>();

        /// <summary>The _consumers.</summary>
        private readonly Dictionary<Type, List<TechieQueue>> _consumers = new Dictionary<Type, List<TechieQueue>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>The check consumers.</summary>
        /// <param name="objIn">The obj in.</param>
        public void CheckConsumers(object objIn)
        {
            Type objType = objIn.GetType();

            lock (this._consumers)
            {
                List<TechieQueue> queueList;
                if (this._consumers.TryGetValue(objType, out queueList))
                {
                    foreach (TechieQueue queue in queueList)
                    {
                        queue.Enqueue(objIn);
                    }
                }
            }

            lock (this._allTypesConsumers)
            {
                foreach (var allTypesConsumer in this._allTypesConsumers)
                {
                    allTypesConsumer.Enqueue(objIn);
                }
            }
        }

        /// <summary>The register all types consumer.</summary>
        /// <param name="queue">The queue.</param>
        /// <returns>The System.Boolean.</returns>
        public bool RegisterAllTypesConsumer(TechieQueue queue)
        {
            lock (this._allTypesConsumers)
            {
                if (!this._allTypesConsumers.Contains(queue))
                {
                    this._allTypesConsumers.Add(queue);
                }
            }

            return true;
        }

        /// <summary>The register consumer.</summary>
        /// <param name="objType">The obj type.</param>
        /// <param name="queue">The queue.</param>
        public void RegisterConsumer(Type objType, TechieQueue queue)
        {
            lock (this._consumers)
            {
                List<TechieQueue> queueList;
                if (!this._consumers.TryGetValue(objType, out queueList))
                {
                    queueList = new List<TechieQueue>();
                    this._consumers.Add(objType, queueList);
                }

                if (!queueList.Contains(queue))
                {
                    queueList.Add(queue);
                }
            }
        }

        /// <summary>The un register consumer.</summary>
        /// <param name="objType">The obj type.</param>
        /// <param name="queue">The queue.</param>
        public void UnRegisterConsumer(Type objType, TechieQueue queue)
        {
            lock (this._consumers)
            {
                List<TechieQueue> queueList;
                if (this._consumers.TryGetValue(objType, out queueList))
                {
                    queueList.Remove(queue);
                }
            }
        }

        #endregion
    }
}