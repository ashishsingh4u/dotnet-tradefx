// ===================================================================================
// <copyright file="ProxyEncoding.cs" company="TechieNotes">
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
//    The ProxyEncoding.cs file.
// </summary>
// ===================================================================================

using System.Text;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    /// Simple static class exposing default proxy encoding.
    /// </summary>
    /// <remarks>
    /// ISO-8859-1 is defined as the standard HTTP encoding, and all proxy
    /// negotiations should comply with this.  
    /// </remarks>
    internal static class ProxyEncoding
    {
        #region Static Fields

        /// <summary>The _default encoding.</summary>
        private static readonly Encoding _defaultEncoding = Encoding.GetEncoding("ISO-8859-1");

        #endregion

        #region Public Properties

        /// <summary>Gets the default.</summary>
        public static Encoding Default
        {
            get
            {
                return _defaultEncoding;
            }
        }

        #endregion
    }
}