// ===================================================================================
// <copyright file="ProxyHelper.cs" company="TechieNotes">
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
//    The ProxyHelper.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net;
using System.Reflection;

using Microsoft.Win32;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>The proxy helper.</summary>
    public static class ProxyHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the location of the Pac file configured in IE.
        /// </summary>
        /// <returns>Pac file Uri.</returns>
        public static Uri GetDefaultPacUri()
        {
            Uri pacUri = null;

            RegistryKey settingsKey =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
            if (settingsKey != null)
            {
                Uri.TryCreate((string)settingsKey.GetValue("AutoConfigURL"), UriKind.RelativeOrAbsolute, out pacUri);
                settingsKey.Close();
            }

            return pacUri;
        }

        /// <summary>
        /// Gets the default proxy using a combination of WPAD and IE settings.
        /// </summary>
        /// <returns>Default proxy object.</returns>
        public static IWebProxy GetDefaultProxy()
        {
            return WebRequest.DefaultWebProxy;
        }

        /// <summary>Gets a web proxy from address / port settings</summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <returns>Web proxy object.</returns>
        public static IWebProxy GetProxy(string host, int port)
        {
            return new WebProxy(host, port);
        }

        /// <summary>Gets a web proxy that returns proxy uri's based on supplied pac Uri.</summary>
        /// <param name="pacUri">Uri of the pac file.</param>
        /// <returns>Web proxy object.</returns>
        public static IWebProxy GetProxyFromPac(Uri pacUri)
        {
            WebProxy webProxy = new WebProxy();

            Type proxyType = webProxy.GetType();
            PropertyInfo scriptLocationInfo = proxyType.GetProperty(
                "ScriptLocation", BindingFlags.Instance | BindingFlags.NonPublic);
            scriptLocationInfo.SetValue(webProxy, pacUri, null);

            return webProxy;
        }

        #endregion
    }
}