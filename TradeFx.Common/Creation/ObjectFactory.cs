//  ===================================================================================
//  <copyright file="ObjectFactory.cs" company="TechieNotes">
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
//     The ObjectFactory.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using log4net;

using TradeFx.Common.Creation;
using TradeFx.Common.Hbt;
using TradeFx.Common.Logging;

namespace TechieNotes.Common.Creation
{
    /// <summary>The object factory.</summary>
    public class ObjectFactory
    {
        #region Static Fields

        /// <summary>The _creatable type name.</summary>
        private static readonly Dictionary<ObjectId, string> CreatableTypeName = new Dictionary<ObjectId, string>();

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        /// <summary>The _creators array.</summary>
        private readonly CreateMethod[] _creatorsArray = new CreateMethod[Enum.GetValues(typeof(ObjectId)).Length];

        /// <summary>The _creators lock.</summary>
        private readonly object _creatorsLock = new object();

        /// <summary>The _publishers lock.</summary>
        private readonly object _publishersLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="ObjectFactory" /> class.
        /// </summary>
        static ObjectFactory()
        {
            Instance = new ObjectFactory();
        }

        /// <summary>
        ///     Prevents a default instance of the <see cref="ObjectFactory" /> class from being created.
        /// </summary>
        private ObjectFactory()
        {
            // Explicit registration to make sure logging and other initial activities can function before the factory are initialized
            this._creatorsArray[(int)ObjectId.Hbt] = new Hbt().Create;
            this._creatorsArray[(int)ObjectId.HbtResponse] = new HbtResponse().Create;
            this._creatorsArray[(int)ObjectId.LogData] = new LogData().Create;
        }

        #endregion

        #region Delegates

        /// <summary>The create method.</summary>
        /// <returns>True if object creation is successful.</returns>
        private delegate object CreateMethod();

        /// <summary>The publish method.</summary>
        /// <param name="xml">The xml.</param>
        /// <returns>True object successfully published.</returns>
        private delegate object PublishMethod(string xml);

        #endregion

        #region Public Properties

        /// <summary>Gets the instance.</summary>
        public static ObjectFactory Instance { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The register object factories.</summary>
        public static void RegisterObjectFactories()
        {
            RegisterObjectFactories(false);
        }

        /// <summary>The register object factories.</summary>
        /// <param name="useCacheFile">The use cache file.</param>
        /// <exception cref="ApplicationException">Throws when ObjectId is common for two objects</exception>
        public static void RegisterObjectFactories(bool useCacheFile)
        {
            Log.Warn("Discovering object factories useCacheFile = " + useCacheFile);

            var factoryInfo = DiscoverFactories(useCacheFile);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Creatable object factories: {0}", factoryInfo.Creatable.Count);
                foreach (var info in factoryInfo.Creatable)
                {
                    Log.Debug(info);
                }
            }

            if (factoryInfo.Exceptions.Count > 0)
            {
                Log.Error("Exception(s) occurred during factory discovery");

                foreach (var exception in factoryInfo.Exceptions)
                {
                    Log.Error(MethodBase.GetCurrentMethod().Name, exception);
                }
            }

            CreatableTypeName.Clear();

            // Normalize the info and copy it for later use
            foreach (var info in factoryInfo.Creatable)
            {
                // we never expect this to be here already - its a sign that two different objects
                // are sharing the same object Id. SCREAM!
                if (CreatableTypeName.ContainsKey(info.ObjectId))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Two types share the same ObjectId : {0} {1}", 
                            CreatableTypeName[info.ObjectId], 
                            info.AssemblyQualifiedTypeName));
                }

                CreatableTypeName[info.ObjectId] = info.AssemblyQualifiedTypeName;
            }
        }

        /// <summary>The create object.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The System.Object.</returns>
        public object CreateObject(ObjectId id)
        {
            var createMethod = this._creatorsArray[(int)id];
            if (createMethod == null)
            {
                if (!CreatableTypeName.ContainsKey(id))
                {
                    return null;
                }

                // double-checked locking
                lock (this._creatorsLock)
                {
                    createMethod = this._creatorsArray[(int)id];
                    if (createMethod == null)
                    {
                        var type = Type.GetType(CreatableTypeName[id]);
                        if (type != null)
                        {
                            this._creatorsArray[(int)id] = ((ICreatable)Activator.CreateInstance(type, true)).Create;
                        }

                        createMethod = this._creatorsArray[(int)id];
                    }
                }
            }

            return createMethod();
        }

        #endregion

        #region Methods

        /// <summary>The discover factories.</summary>
        /// <param name="useCachFile">The use cach file.</param>
        /// <returns>The TechieNotes.Common.Creation.FactoryInfo.</returns>
        /// <exception cref="ApplicationException">Throws when not able to create app domain.</exception>
        /// <exception cref="InvalidCastException">Throws when cast fail to FactoryEnumeratorClass</exception>
        private static FactoryInfo DiscoverFactories(bool useCachFile)
        {
            const string DiscoveryDomainName = "Factory discovery domain";

            var domain = AppDomain.CreateDomain(
                DiscoveryDomainName, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

            if (domain == null)
            {
                throw new ApplicationException("Could not create AppDomain: " + DiscoveryDomainName);
            }

            // Load our enumerator into the other domain and Unwrap the type.
            // We need to tell the other appdomain where to load our FactoryEnumerator class from.
            var obj =
                domain.CreateInstance(
                    Path.GetFileNameWithoutExtension(typeof(FactoryClassEnumerator).Module.Name) ?? string.Empty, 
                    typeof(FactoryClassEnumerator).FullName ?? string.Empty);
            if (obj == null)
            {
                throw new InvalidCastException("Could not create instance of class in other AppDomain");
            }

            var enumerator = (FactoryClassEnumerator)obj.Unwrap();

            // Now call the method in the enumerator class get details of the factories.
            // This is marshalled across the appdomain as our enumerator class derives
            // from MarshalByRefObj.
            var factoryInfo = enumerator.GetObjectFactory(useCachFile);

            // Unload other appdomain.
            AppDomain.Unload(domain);

            return factoryInfo;
        }

        #endregion
    }
}