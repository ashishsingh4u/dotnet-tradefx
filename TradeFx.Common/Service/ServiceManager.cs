//  ===================================================================================
//  <copyright file="ServiceManager.cs" company="TechieNotes">
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
//  <date>27-01-2013</date>
//  <summary>
//     The ServiceManager.cs file.
//  </summary>
//  ===================================================================================

using System.Collections.Generic;

using StructureMap;

using TradeFx.Common.Interface;

namespace TradeFx.Common.Service
{
    /// <summary>The ServiceManager.</summary>
    public sealed class ServiceManager
    {
        #region Static Fields

        /// <summary>The _service instance.</summary>
        private static readonly ServiceManager _serviceInstance = new ServiceManager();

        #endregion

        #region Public Properties

        /// <summary>Gets the instance.</summary>
        public static ServiceManager Instance
        {
            get
            {
                return _serviceInstance;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose container.</summary>
        public void DisposeContainer()
        {
            ObjectFactory.Container.Dispose();
        }

        /// <summary>The initialize.</summary>
        public void Initialize()
        {
            foreach (IService plugintype in ObjectFactory.GetAllInstances(typeof(IService)))
            {
                plugintype.Initialize();
            }
        }

        /// <summary>The register non service type.</summary>
        /// <typeparam name="TFrom">Interface</typeparam>
        /// <typeparam name="TTo">Concrete member</typeparam>
        public void RegisterNonServiceType<TFrom, TTo>() where TTo : TFrom
        {
            ObjectFactory.Configure(x => x.For<TFrom>().Singleton().Use<TTo>());
        }

        /// <summary>The register type.</summary>
        /// <typeparam name="TFrom">From type</typeparam>
        /// <typeparam name="TTo">to type</typeparam>
        public void RegisterType<TFrom, TTo>() where TTo : TFrom where TFrom : class, IService
        {
            ObjectFactory.Configure(
                x =>
                    {
                        x.For<TFrom>().Singleton().Use<TTo>();
                        x.Forward<TFrom, IService>();
                    });
        }

        /// <summary>The RegisterTypes.</summary>
        public void RegisterTypes()
        {
            ObjectFactory.Configure(
                x => x.Scan(
                    scanner =>
                        {
                            scanner.LookForRegistries();
                            scanner.AssemblyContainingType<IService>();
                        }));
        }

        /// <summary>The resolve.</summary>
        /// <typeparam name="T">Resolved instance</typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T Resolve<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }

        /// <summary>The resolve all.</summary>
        /// <typeparam name="T">IService Instance</typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IList</cref>
        ///     </see>
        ///     Type list.
        /// </returns>
        public IList<T> ResolveAll<T>()
        {
            return ObjectFactory.GetAllInstances<T>();
        }

        #endregion
    }
}