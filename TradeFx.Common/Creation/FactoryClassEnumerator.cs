//  ===================================================================================
//  <copyright file="FactoryClassEnumerator.cs" company="TechieNotes">
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
//     The FactoryClassEnumerator.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using log4net;

using TradeFx.Common.Creation;
using TradeFx.Common.Crypto;
using TradeFx.Common.Culture;

namespace TechieNotes.Common.Creation
{
    /// <summary>
    ///     Identifies classes in a set of Assemblies that have a certain
    ///     interface.  Derives from MarshalByRef obj so we can invoke a
    ///     method across an AppDomain boundary.
    /// </summary>
    public class FactoryClassEnumerator : MarshalByRefObject
    {
        #region Constants

        /// <summary>The assemblies search pattern.</summary>
        private const string ASSEMBLIES_SEARCH_PATTERN = "TechieNotes*.dll";

        /// <summary>The _encrypt literal.</summary>
        private const string ENCRYPT_LITERAL = "JETSTREAMPROD";

        #endregion

        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods and Operators

        /// <summary>The get object factory.</summary>
        /// <param name="useCachedFile">The use cached file.</param>
        /// <returns>The TechieNotes.Common.Creation.FactoryInfo.</returns>
        public FactoryInfo GetObjectFactory(bool useCachedFile)
        {
            if (!useCachedFile)
            {
                return this.FindFactoryClasses();
            }

            FactoryInfo factoryInfo;
            try
            {
                var filePath = Environment.CurrentDirectory;
                filePath = Path.Combine(filePath, "ObjectFactory.txt");
                if (File.Exists(filePath))
                {
                    factoryInfo = DeSerializeData(filePath);
                }
                else
                {
                    factoryInfo = this.FindFactoryClasses();
                    SerializeData(factoryInfo, filePath);
                }
            }
            catch (Exception)
            {
                factoryInfo = this.FindFactoryClasses();
            }

            return factoryInfo;
        }

        #endregion

        #region Methods

        /// <summary>The async write to local storage.</summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        private static void AsyncWriteToLocalStorage(string path, byte[] data)
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        try
                        {
                            AppCulture.SetThreadCulture();

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                AppCulture.SetThreadCulture();
                                fileStream.Write(data, 0, data.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message, ex);
                        }
                    }, 
                null);
        }

        /// <summary>The de serialize data.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The TechieNotes.Common.Creation.FactoryInfo.</returns>
        /// <exception cref="ArgumentException">Throws when file is path is wrong.</exception>
        private static FactoryInfo DeSerializeData(string filePath)
        {
            Log.Warn("Reading factory Info to file " + filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The path is null or empty", filePath);
            }

            byte[] data;
            using (var fileStream = File.OpenRead(filePath))
            {
                // the first HeaderLength bytes contain the last update time so we need to skip those
                data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
            }

            // the data is an encrypted DataSet so we first need to decrypt it
            var encrypter = new Encrypter(ENCRYPT_LITERAL);
            var serializedData = encrypter.Decrypt(data);
            object factoryInfo;

            var formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(serializedData))
            {
                factoryInfo = formatter.Deserialize(memoryStream);
            }

            return factoryInfo as FactoryInfo;
        }

        /// <summary>The has creatable interface.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The System.Boolean.</returns>
        private static bool HasCreatableInterface(Type type)
        {
            return ((IList<Type>)type.GetInterfaces()).Contains(typeof(ICreatable));
        }

        /// <summary>The serialize data.</summary>
        /// <param name="factoryInfo">The factory info.</param>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="ArgumentNullException">Throws when factoryinfo is null.</exception>
        /// <exception cref="ArgumentException">Throws when filepath is null or empty</exception>
        private static void SerializeData(FactoryInfo factoryInfo, string filePath)
        {
            Log.Warn("Writing factory Info to file " + filePath);
            if (factoryInfo == null)
            {
                throw new ArgumentNullException("factoryInfo");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The path is null or empty", "filePath");
            }

            try
            {
                var formatter = new BinaryFormatter();
                byte[] encryptedData;
                using (var memoryStream = new MemoryStream())
                {
                    formatter.Serialize(memoryStream, factoryInfo);
                    var encrypter = new Encrypter(ENCRYPT_LITERAL);
                    encryptedData = encrypter.Encrypt(memoryStream.ToArray());

                    // first write the last update time
                }

                AsyncWriteToLocalStorage(filePath, encryptedData);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Find all matching assemblies in the assembly folder.
        /// </summary>
        /// <returns>FactoryInfo with all information</returns>
        private FactoryInfo FindFactoryClasses()
        {
            var factoryInfo = new FactoryInfo();

            var baseDir = AppDomain.CurrentDomain.ShadowCopyFiles
                              ? AppDomain.CurrentDomain.SetupInformation.CachePath
                              : AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(
                baseDir, 
                ASSEMBLIES_SEARCH_PATTERN, 
                AppDomain.CurrentDomain.ShadowCopyFiles ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                Type[] types;

                try
                {
                    var asm = Assembly.LoadFrom(file);
                    types = asm.GetTypes();
                }
                catch (Exception ex)
                {
                    factoryInfo.Exceptions.Add(new Exception("File Load Exception: " + file, ex));
                    continue;
                }

                foreach (var type in types)
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    try
                    {
                        if (HasCreatableInterface(type))
                        {
                            var factory = Activator.CreateInstance(type, true) as ICreatable;
                            if (factory != null)
                            {
                                factoryInfo.Creatable.Add(
                                    new FactoryInfo.CreatableInfo(type.AssemblyQualifiedName, factory.GetId()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        factoryInfo.Exceptions.Add(
                            new Exception(string.Format("Exception occurred loading type {0}", type.FullName), ex));
                    }
                }
            }

            return factoryInfo;
        }

        #endregion
    }
}