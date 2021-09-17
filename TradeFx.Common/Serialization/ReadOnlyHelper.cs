// ===================================================================================
// <copyright file="ReadOnlyHelper.cs" company="TechieNotes">
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
//    The ReadOnlyHelper.cs file.
// </summary>
// ===================================================================================

using System;

namespace TechieNotes.Common
{
    /// <summary>
    /// This is a temporary interface, will eliminate
    /// </summary>
    public interface IReadOnly
    {
        #region Public Methods and Operators

        /// <summary>The check read only.</summary>
        void CheckReadOnly();

        /// <summary>The set read only.</summary>
        void SetReadOnly();

        #endregion
    }

    /// <summary>The read only helper.</summary>
    [Serializable]
    public sealed class ReadOnlyHelper
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether read only.</summary>
        public bool ReadOnly { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The check read only.</summary>
        /// <exception cref="ReadOnlyException"></exception>
        public void CheckReadOnly()
        {
            if (this.ReadOnly)
            {
                throw new ReadOnlyException();
            }
        }

        /// <summary>The set read only.</summary>
        /// <returns>The System.Boolean.</returns>
        public bool SetReadOnly()
        {
            bool oldValue = this.ReadOnly;
            this.ReadOnly = true;

            if (oldValue == false)
            {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Read-only specific exception
        /// </summary>
        public class ReadOnlyException : ApplicationException
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="ReadOnlyException"/> class.</summary>
            public ReadOnlyException()
                : base("Trying to set ReadOnly Property")
            {
            }

            #endregion
        }
    }
}