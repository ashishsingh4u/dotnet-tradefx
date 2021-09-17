// ===================================================================================
// <copyright file="FactoryInfo.cs" company="TechieNotes">
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
//    The FactoryInfo.cs file.
// </summary>
// ===================================================================================

using System;
using System.Collections.Generic;

namespace TradeFx.Common.Creation
{
    /// <summary>The factory info.</summary>
    [Serializable]
    public class FactoryInfo
    {
        #region Fields

        /// <summary>
        /// We store exceptions so we can log them from the calling domain because log4net doesn't work well with multiple domains
        /// </summary>
        private List<Exception> _exceptions;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FactoryInfo"/> class.</summary>
        public FactoryInfo()
        {
            this.Creatable = new List<CreatableInfo>();
            this._exceptions = new List<Exception>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the creatable.</summary>
        public List<CreatableInfo> Creatable { get; set; }

        /// <summary>
        /// Gets or sets exceptions.
        /// We store exceptions so we can log them from the calling domain because log4net doesn't work well with multiple domains
        /// </summary>
        public List<Exception> Exceptions
        {
            get
            {
                return this._exceptions;
            }

            set
            {
                this._exceptions = value;
            }
        }

        #endregion

        /// <summary>The creatable info.</summary>
        [Serializable]
        public class CreatableInfo
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="CreatableInfo"/> class.</summary>
            /// <param name="assemblyQualifiedName">The assembly qualified name.</param>
            /// <param name="objectId">The object id.</param>
            public CreatableInfo(string assemblyQualifiedName, ObjectId objectId)
            {
                this.AssemblyQualifiedTypeName = assemblyQualifiedName;
                this.ObjectId = objectId;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the assembly qualified type name.</summary>
            public string AssemblyQualifiedTypeName { get; set; }

            /// <summary>Gets or sets the object id.</summary>
            public ObjectId ObjectId { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>The to string.</summary>
            /// <returns>The System.String.</returns>
            public override string ToString()
            {
                return string.Format("[{0}] --> {1}", this.ObjectId, this.AssemblyQualifiedTypeName);
            }

            #endregion
        }
    }
}