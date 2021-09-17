//  ===================================================================================
//  <copyright file="AppCulture.cs" company="TechieNotes">
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
//  <date>11-03-2013</date>
//  <summary>
//     The AppCulture.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Globalization;
using System.Threading;

namespace TradeFx.Common.Culture
{
    /// <summary>The app culture.</summary>
    public static class AppCulture
    {
        #region Static Fields

        /// <summary>The _startup culture.</summary>
        private static readonly CultureInfo _startupCulture;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="AppCulture" /> class.
        /// </summary>
        static AppCulture()
        {
            // Capture the application startup culture before any call to SetThreadCulture can upset it.
            // This is used by server processes to verify that the server machine is configured
            // with the correct culture.
            _startupCulture = CultureInfo.CurrentCulture;

            // This app is currently standardized on en-GB culture.
            // Using readonly version as it is exposed as a property.
            Culture = CultureInfo.ReadOnly(new CultureInfo("en-US"));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the culture.</summary>
        public static CultureInfo Culture { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The set thread culture.</summary>
        public static void SetThreadCulture()
        {
            var thread = Thread.CurrentThread;
            if (!thread.CurrentCulture.Equals(Culture))
            {
                thread.CurrentCulture = Culture;
            }

            if (!thread.CurrentUICulture.Equals(Culture))
            {
                thread.CurrentUICulture = Culture;
            }
        }

        /// <summary>
        ///     Throws an ApplicationException if the application culture is not the required culture.
        /// </summary>
        public static void VerifyCurrentCulture()
        {
            // Compare the Parent culture, and make sure it is "en" - the specific culture is too unstable to compare here,
            // on DEV machines it keeps wanting to default to "en-US", regardless of regional settings.
            if (_startupCulture.Parent.Name != Culture.Parent.Name)
            {
                throw new ApplicationException(
                    "Server Regional Settings Failure: Culture " + _startupCulture.Name + " is invalid. Culture must be "
                    + Culture.Name);
            }
        }

        #endregion
    }
}