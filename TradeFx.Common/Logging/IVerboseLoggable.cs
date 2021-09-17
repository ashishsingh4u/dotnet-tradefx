// ===================================================================================
// <copyright file="IVerboseLoggable.cs" company="TechieNotes">
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
//    The IVerboseLoggable.cs file.
// </summary>
// ===================================================================================
namespace TradeFx.Common.Logging
{
    /// <summary>The VerboseLoggable interface.</summary>
    public interface IVerboseLoggable : ILoggable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Interface class that provides a "verbose" log string.
        /// A verbose log is intended to be called infrequently to reduce the effect of logging on system performance
        /// </summary>
        /// <returns>Log as text</returns>
        string ToVerboseLog();

        #endregion
    }
}