//  ===================================================================================
//  <copyright file="ConnectionClient.cs" company="TechieNotes">
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
//     The ConnectionClient.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

using TradeFx.Common.Transport;
using TradeFx.Connection.Data;

using log4net.Config;

namespace TradeFx.Connection.Client
{
    /// <summary>The connection client.</summary>
    public partial class ConnectionClient : Form
    {
        #region Fields

        /// <summary>The _manager.</summary>
        private TestClientManager _manager;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ConnectionClient"/> class.</summary>
        public ConnectionClient()
        {
            InitializeComponent();
            XmlConfigurator.Configure();

            this.InitManager();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initialize manager.
        /// </summary>
        public void InitManager()
        {
            this._manager = new TestClientManager();
            this._manager.IncommingObject += this.ManagerIncomingObject;
            this._manager.LatencyChanged += this.ManagerLatencyChanged;
            this._manager.Initialize("127.0.0.1:6060", new TechieSocket(), true, true);
            this._manager.FireIncommingObjectsAsEvents = true;
        }

        #endregion

        #region Methods

        /// <summary>The manager incoming object.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ManagerIncomingObject(object sender, Manager.IncommingObjectEventArgs e)
        {
            if (e.Object is TestData)
            {
                var testData = e.Object as TestData;
                MessageBox.Show(testData.Name);
            }
        }

        /// <summary>The manager latency changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ManagerLatencyChanged(object sender, Manager.LatencyEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    (Action)(() =>
                        {
                            this._latencyLabel.Text = e.Latency.ToString(CultureInfo.InvariantCulture);
                            this.Text = e.UpdateTime.ToString(CultureInfo.InvariantCulture);
                        }));
            }
        }

        #endregion
    }

    /// <summary>The test client manager.</summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Reviewed. Suppression is OK here.")]
    public class TestClientManager : Manager
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestClientManager" /> class.
        /// </summary>
        public TestClientManager()
        {
            this.LatencyInterval = 5000;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The process incoming.</summary>
        /// <param name="objectin">The object in.</param>
        /// <returns>The System.Boolean.</returns>
        public override bool ProcessIncoming(object objectin)
        {
            if (objectin is TestData)
            {
                return true;
            }

            return base.ProcessIncoming(objectin);
        }

        #endregion
    }
}