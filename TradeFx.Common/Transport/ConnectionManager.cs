//  ===================================================================================
//  <copyright file="ConnectionManager.cs" company="TechieNotes">
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
//  <date>13-03-2013</date>
//  <summary>
//     The ConnectionManager.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

using TradeFx.Common.Logging;

using log4net;

using TradeFx.Common.Helpers;
using TradeFx.Common.Transport.Network;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport
{
    /// <summary>The connection manager.</summary>
    public class ConnectionManager
    {
        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        /// <summary>The _managers.</summary>
        private readonly List<Manager> _managers = new List<Manager>();

        /// <summary>The _managers uninitialising lock.</summary>
        private readonly object _managersUninitialisingLock = new object();

        /// <summary>The _network connection listener.</summary>
        private readonly NetworkConnectionListener _networkConnectionListener = new NetworkConnectionListener();

        /// <summary>The _not uninitialising.</summary>
        private readonly ManualResetEvent _notUninitialising = new ManualResetEvent(true);

        /// <summary>The _factory.</summary>
        private IConnectionManagerFactory _factory;

        /// <summary>The _managers uninitialising.</summary>
        private int _managersUninitialising;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectionManager" /> class.
        /// </summary>
        public ConnectionManager()
        {
            this._networkConnectionListener.Connect += this.NetworkConnectionListenerConnect;
            this._networkConnectionListener.Error += this.NetworkConnectionListener_Error;
        }

        #endregion

        #region Interfaces

        /// <summary>The ConnectionManagerFactory interface.</summary>
        public interface IConnectionManagerFactory
        {
            #region Public Methods and Operators

            /// <summary>The create manager.</summary>
            /// <returns>The TechieNotes.Common.Manager.</returns>
            Manager CreateManager();

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the outgoing heart beat period.</summary>
        public int OutgoingHeartBeatPeriod { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The initialize.</summary>
        /// <param name="listeningEndPoint">The listening end point.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>The System.Boolean.</returns>
        public bool Initialize(IPEndPoint listeningEndPoint, IConnectionManagerFactory factory)
        {
            try
            {
                ArgumentValidation.CheckForNullReference(listeningEndPoint, "listeningEndPoint");
                ArgumentValidation.CheckForNullReference(factory, "factory");

                this._factory = factory;

                this._networkConnectionListener.Address = listeningEndPoint.Address.ToString();
                this._networkConnectionListener.Port = listeningEndPoint.Port;
                this.OpenNetworkConnectionListener(this._networkConnectionListener);

                Log.WarnFormat(
                    "Opened connection listener. Address={0}, Port={1}", 
                    this._networkConnectionListener.Address, 
                    this._networkConnectionListener.Port);

                // PERFORMANCE COUNTER REMOVED
                // Performance.Counters.AddCounterInstance("CONNECTIONS", ManagerInstance, "Number of connections");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        /// <summary>The send in.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SendIn(object obj)
        {
            try
            {
                var managersList = new List<Manager>();

                lock (this._managers)
                {
                    managersList.AddRange(this._managers);
                }

                foreach (var manager in managersList)
                {
                    Monitor.Enter(manager);

                    try
                    {
                        manager.SendIn(obj);
                    }
                    finally
                    {
                        Monitor.Exit(manager);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return false;
        }

        /// <summary>The log error is used when the caller of this send is the Logger function
        ///     which would lead to the failure to send a Log entry causing another error which would need
        ///     to be logged which would (potentially) also fail, leading to a downward spiral.</summary>
        /// <param name="obj">transmitting data</param>
        /// <returns>The System.Boolean.</returns>
        public bool SendOut(ISendable obj)
        {
            try
            {
                var managersList = new List<Manager>();

                lock (this._managers)
                {
                    managersList.AddRange(this._managers);
                }

                foreach (var manager in managersList)
                {
                    Monitor.Enter(manager);

                    try
                    {
                        manager.SendOut(obj);
                    }
                    finally
                    {
                        Monitor.Exit(manager);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                bool isDataLogEntry = obj is LogData;

                // check the type of object which was not sent.
                // if it is a log entry then do not log a further error (may churn)
                if (!isDataLogEntry)
                {
                    Log.Error(ex.Message, ex);
                }
            }

            return false;
        }

        /// <summary>The un initialize.</summary>
        public void UnInitialize()
        {
            try
            {
                Log.WarnFormat(
                    "Closing connection listener. Address={0}, Port={1}", 
                    this._networkConnectionListener.Address, 
                    this._networkConnectionListener.Port);

                this._networkConnectionListener.Close();

                var managersList = new List<Manager>();

                lock (this._managers)
                {
                    managersList.AddRange(this._managers);
                    this._managers.Clear();

                    // PERFORMANCE COUNTER REMOVED
                    // Performance.Counters.Clear("CONNECTIONS", ManagerInstance);
                }

                foreach (var manager in managersList)
                {
                    try
                    {
                        Log.InfoFormat(
                            "{0}: Closing connection. Manager Name={1}", 
                            MethodBase.GetCurrentMethod().Name, 
                            manager.Name);

                        manager.ConnectionClosed -= this.ManagerConnectionClosedEvent;
                        manager.UnInitialize();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                    }
                }

                // Wait until any uninitialisation being down by ManagerConnectionClosed_Event has completed
                this._notUninitialising.WaitOne();

                Log.WarnFormat(
                    "Closed connection listener. Address={0}, Port={1}", 
                    this._networkConnectionListener.Address, 
                    this._networkConnectionListener.Port);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>The get socket.</summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The TechieNotes.Common.TechieSocket.</returns>
        protected virtual TechieSocket GetSocket(NetworkConnection connection)
        {
            return new TechieSocket(connection);
        }

        /// <summary>The open network connection listener.</summary>
        /// <param name="connectionListener">The connection listener.</param>
        protected virtual void OpenNetworkConnectionListener(NetworkConnectionListener connectionListener)
        {
            connectionListener.Open();
        }

        /// <summary>The manager connection closed_ event.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ManagerConnectionClosedEvent(object sender, EventArgs e)
        {
            var manager = (Manager)sender;

            Log.InfoFormat("{0}: Connection lost. Manager Name={1}", MethodBase.GetCurrentMethod().Name, manager.Name);

            lock (this._managersUninitialisingLock)
            {
                if (++this._managersUninitialising == 1)
                {
                    this._notUninitialising.Reset();
                }
            }

            try
            {
                bool uninitialise;
                lock (this._managers)
                {
                    uninitialise = this._managers.Remove(manager);
                }

                if (uninitialise)
                {
                    manager.ConnectionClosed -= this.ManagerConnectionClosedEvent;
                    manager.UnInitialize();
                }
            }
            finally
            {
                lock (this._managersUninitialisingLock)
                {
                    if (--this._managersUninitialising == 0)
                    {
                        this._notUninitialising.Set();
                    }
                }
            }
        }

        /// <summary>The network connection listener_ connect.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="ApplicationException">Throws if connection initialization failed.</exception>
        private void NetworkConnectionListenerConnect(object sender, ConnectEventArgs e)
        {
            IConnection socket = null;

            try
            {
                // No proxy required as this is accepting an incoming connection
                socket = this.GetSocket(e.Connection);

                Log.InfoFormat(
                    "{0}: Connection request. ConnectionAddress={1} Local Address={2}, Port={3}", 
                    MethodBase.GetCurrentMethod().Name, 
                    socket.ConnectionAddress, 
                    this._networkConnectionListener.Address, 
                    this._networkConnectionListener.Port);

                var manager = this._factory.CreateManager();

                manager.Name = "CONNECTION: " + socket.ConnectionAddress;
                manager.ConnectionClosed += this.ManagerConnectionClosedEvent;

                try
                {
                    if (this.OutgoingHeartBeatPeriod > 0)
                    {
                        manager.EnableOutgoingHeartBeat = true;
                        manager.OutgoingHeartbeatPeriod = this.OutgoingHeartBeatPeriod;
                    }

                    if (manager.Initialize(socket, true, true))
                    {
                        lock (this._managers)
                        {
                            this._managers.Add(manager);

                            // PERFORMANCE COUNTER REMOVED
                            // Performance.Counters.Increment("CONNECTIONS", ManagerInstance);
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Unable to initialize new connection: " + manager.Name);
                    }
                }
                catch (Exception)
                {
                    manager.ConnectionClosed -= this.ManagerConnectionClosedEvent;
                    manager.UnInitialize();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                try
                {
                    if (socket != null)
                    {
                        socket.Disconnect();
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                }
            }
        }

        /// <summary>The network connection listener_ error.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void NetworkConnectionListener_Error(object sender, ErrorEventArgs e)
        {
            var ex = e.GetException();
            Log.Error(ex.Message, ex);
        }

        #endregion
    }
}