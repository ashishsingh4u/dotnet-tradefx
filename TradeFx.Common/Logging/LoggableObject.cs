// ===================================================================================
// <copyright file="LoggableObject.cs" company="TechieNotes">
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
//    The LoggableObject.cs file.
// </summary>
// ===================================================================================

using TradeFx.Common.Logging;
using TradeFx.Common.Transport.Packet;

namespace TechieNotes.Common.Logging
{
    /// <summary>
    /// Wrapper object to allow delayed creation of logging string, e.g. StringBuilder or ILoggable objects
    /// </summary>
    public static class LoggableObject
    {
        #region Public Methods and Operators

        /// <summary>The to log.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.String.</returns>
        public static string ToLog(object obj)
        {
            // We will return empty string if object is null
            if (obj == null)
            {
                return string.Empty;
            }

            // If object implements ILoggable we shall
            var loggable = obj as ILoggable;
            return (loggable != null) ? loggable.ToLog() : obj.ToString();
        }

        /// <summary>NB CAREFUL: Only call this method is you want verbose logging 
        /// Excessive verbose logging can affect performance !!!</summary>
        /// <param name="obj">Object to log.</param>
        /// <returns>The System.String.</returns>
        public static string ToVerboseLog(object obj)
        {
            var verboseLoggable = obj as IVerboseLoggable;
            if (verboseLoggable != null)
            {
                return verboseLoggable.ToVerboseLog();
            }

            return ToLog(obj);
        }

        #endregion
    }
}