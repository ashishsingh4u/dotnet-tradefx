//  ===================================================================================
//  <copyright file="ConnectionServer.cs" company="TechieNotes">
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
//  <date>17-03-2013</date>
//  <summary>
//     The ConnectionServer.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using log4net.Config;

using TradeFx.Common.Transport;
using TradeFx.Connection.Data;

namespace TradeFx.Connection.Server
{
    /// <summary>The connection server.</summary>
    public partial class ConnectionServer : Form, ConnectionManager.IConnectionManagerFactory
    {
        #region Fields

        /// <summary>The _connection manager.</summary>
        private ConnectionManager _connectionManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectionServer" /> class.
        /// </summary>
        public ConnectionServer()
        {
            this.InitializeComponent();
            XmlConfigurator.Configure();
            this.InitManager("Localhost:6060");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create manager.</summary>
        /// <returns>The TechieNotes.Common.Manager.</returns>
        public Manager CreateManager()
        {
            return new Manager("MyManager");
        }

        #endregion

        #region Methods

        /// <summary>The button 1 click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Button1Click(object sender, EventArgs e)
        {
            this._connectionManager.SendOut(new TestData { Name = "Ashish", Roll = 1, City = "Delhi" });
        }

        /// <summary>The init manager.</summary>
        /// <param name="address">The address.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void InitManager(string address)
        {
            var endPoint = TechieSocket.GetEndPoint(address);

            this._connectionManager = new ConnectionManager();
            this._connectionManager.Initialize(endPoint, this);
            this._connectionManager.OutgoingHeartBeatPeriod = 5000;
        }

        #endregion
    }
}