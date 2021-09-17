//  ===================================================================================
//  <copyright file="WindowService.cs" company="TechieNotes">
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
//  <date>12-01-2013</date>
//  <summary>
//     The WindowService.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Windows.Forms;

using TradeFx.Common;
using TradeFx.Common.MarketDataReader;

namespace TradeFx.Server.MarketDataReader
{
    /// <summary>The form 1.</summary>
    public partial class WindowService : Form
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowService" /> class.
        /// </summary>
        public WindowService()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The register services.</summary>
        public static void RegisterServices()
        {
            CommonServiceBootstrapper.RegisterMessageBusService();
            MarketDataBootstrapper.RegisterMarketDataServices();
        }

        #endregion

        #region Methods

        /// <summary>The button 1 click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Button1Click(object sender, EventArgs e)
        {
            RegisterServices();
        }

        #endregion
    }
}