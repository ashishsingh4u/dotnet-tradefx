//  ===================================================================================
//  <copyright file="Manager.cs" company="TechieNotes">
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
//     The Manager.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

using TechieNotes.Common.HBT;

using TradeFx.Common.Hbt;
using TradeFx.Common.Serialization;

using log4net;

using TechieNotes.Common.Logging;

using TradeFx.Common.Enums;
using TradeFx.Common.Events;
using TradeFx.Common.Helpers;
using TradeFx.Common.Logging;
using TradeFx.Common.Transport.Network;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport
{
    /// <summary>
    ///     Introduced specifically for the etrader, which mostly forwards on messages without modifying them.
    ///     Essentially used as performance optimisation, so that a (read-only) incoming message is not serialised
    ///     repeatedly every-time the same message is written out by the Manager class.
    /// </summary>
    public interface IManagerBinaryStore
    {
        #region Public Properties

        /// <summary>
        ///     NB: The intention is that this property will be called ONLY by the Manager class
        /// </summary>
        byte[] ManagerBytes { get; set; }

        #endregion
    }

    /// <summary>The manager.</summary>
    public class Manager
    {
        #region Constants

        /// <summary>The connection a lternate address delimmiter.</summary>
        public const char ConnectionALternateAddressDelimmiter = ',';

        /// <summary>The connection address delimmiter.</summary>
        public const char ConnectionAddressDelimmiter = ';';

        /// <summary>The _max objects to process.</summary>
        protected const int _maxObjectsToProcess = 1000;

        #endregion

        #region Static Fields

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>The _conflation enabled.</summary>
        private static bool _conflationEnabled = true;

        /// <summary>The _server.</summary>
        private static bool _server;

        #endregion

        #region Fields

        /// <summary>The manager status event.</summary>
        public EventWrapper<ManagerStatusEventArgs> ManagerStatusEvent = new EventWrapper<ManagerStatusEventArgs>();

        /// <summary>The _incoming heartbeat period.</summary>
        protected int _incomingHeartbeatPeriod;

        /// <summary>The _manager status.</summary>
        protected volatile ManagerStatus _managerStatus = ManagerStatus.Err;

        /// <summary>The _connect delay matrix.</summary>
        private readonly int[] _connectDelayMatrix = new[] { 0, 1, 2, 5, 10 };

        /// <summary>The _connection addresses.</summary>
        private readonly List<string> _connectionAddresses = new List<string>();

        /// <summary>The _consumers.</summary>
        private readonly Consumers _consumers = new Consumers();

        /// <summary>The _latency requests.</summary>
        private readonly Dictionary<string, long> _latencyRequests = new Dictionary<string, long>();

        /// <summary>The _locker.</summary>
        private readonly object _locker = new object();

        /// <summary>The _connect delay index.</summary>
        private int _connectDelayIndex;

        /// <summary>The _connect fail count.</summary>
        private int _connectFailCount;

        /// <summary>The _connection.</summary>
        private IConnection _connection;

        /// <summary>The _currentconnection address index.</summary>
        private int _currentconnectionAddressIndex;

        /// <summary>The _enable incoming heartbeat.</summary>
        private bool _enableIncomingHeartbeat;

        /// <summary>The _enable outgoing heart beat.</summary>
        private bool _enableOutgoingHeartBeat;

        /// <summary>The _in queue.</summary>
        private TechieQueue _inQueue = new TechieQueue();

        /// <summary>The _in queue signal conflator.</summary>
        private volatile SignalConflator _inQueueSignalConflator;

        /// <summary>The _incoming heartbeat timeout.</summary>
        private int _incomingHeartbeatTimeout;

        /// <summary>The _incoming heartbeat timer.</summary>
        private Timer _incomingHeartbeatTimer;

        /// <summary>The _initialized.</summary>
        private volatile bool _initialized;

        /// <summary>The _last incoming tick count.</summary>
        private int _lastIncomingTickCount;

        /// <summary>The _latency.</summary>
        private long _latency = -1;

        /// <summary>The _latency interval.</summary>
        private int _latencyInterval = -1;

        /// <summary>The _latency timer.</summary>
        private Timer _latencyTimer;

        /// <summary>The _out queue.</summary>
        private TechieQueue _outQueue = new TechieQueue();

        /// <summary>The _out queue signal conflator.</summary>
        private volatile SignalConflator _outQueueSignalConflator;

        /// <summary>The _outgoing heart beat key.</summary>
        private int _outgoingHeartBeatKey;

        /// <summary>The _outgoing heartbeat period.</summary>
        private int _outgoingHeartbeatPeriod = 10000;

        /// <summary>The _outgoing heartbeat timer.</summary>
        private Timer _outgoingHeartbeatTimer;

        /// <summary>The _outhbt.</summary>
        private Hbt.Hbt _outhbt = new Hbt.Hbt();

        /// <summary>The _process in queue.</summary>
        private volatile bool _processInQueue;

        /// <summary>The _process out queue.</summary>
        private volatile bool _processOutQueue;

        /// <summary>The _sw.</summary>
        private Stopwatch _sw = new Stopwatch();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Manager" /> class.
        /// </summary>
        public Manager()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Manager"/> class.</summary>
        /// <param name="owner">The owner.</param>
        public Manager(string owner)
        {
            this.Name = owner ?? this.GetType().Name;
            this.InQueue.Name = this.Name + "_IN";
            this.OutQueue.Name = this.Name + "_OUT";

            this.IncomingHeartBeatTimeOut = 25000;
        }

        #endregion

        #region Public Events

        /// <summary>The connection closed.</summary>
        public event EventHandler<EventArgs> ConnectionClosed;

        /// <summary>The incomming object.</summary>
        public event EventHandler<IncommingObjectEventArgs> IncommingObject;

        /// <summary>The latency changed.</summary>
        public event EventHandler<LatencyEventArgs> LatencyChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether conflation enabled.</summary>
        public static bool ConflationEnabled
        {
            get
            {
                return _conflationEnabled;
            }

            set
            {
                _conflationEnabled = value;
            }
        }

        /// <summary>Gets a value indicating whether server.</summary>
        public static bool Server
        {
            get
            {
                return _server;
            }
        }

        /// <summary>Gets the current connection address.</summary>
        public string CurrentConnectionAddress
        {
            get
            {
                return this._connection != null ? this._connection.ConnectionAddress : string.Empty;
            }
        }

        /// <summary>Gets or sets a value indicating whether enable incoming heart beat.</summary>
        public bool EnableIncomingHeartBeat
        {
            get
            {
                return this._enableIncomingHeartbeat;
            }

            set
            {
                if (this._enableIncomingHeartbeat == value)
                {
                    return;
                }

                this._enableIncomingHeartbeat = value;
                this.ConfigureIncomingHeartbeatTimer();
            }
        }

        /// <summary>Gets or sets a value indicating whether enable outgoing heart beat.</summary>
        public bool EnableOutgoingHeartBeat
        {
            get
            {
                return this._enableOutgoingHeartBeat;
            }

            set
            {
                if (this._enableOutgoingHeartBeat == value)
                {
                    return;
                }

                this._enableOutgoingHeartBeat = value;
                if (this._outhbt.HeartbeatPeriod == 0)
                {
                    this._outhbt = this._outhbt.NewHeartbeatPeriod(this._outgoingHeartbeatPeriod);
                }

                this.ConfigureOutgoingHeartbeatTimer();
            }
        }

        /// <summary>Gets or sets a value indicating whether fire incomming objects as events.</summary>
        public bool FireIncommingObjectsAsEvents { get; set; }

        /// <summary>Gets or sets a value indicating whether heart beat ok.</summary>
        public bool HeartBeatOK { get; set; }

        /// <summary>Gets or sets the in queue.</summary>
        public TechieQueue InQueue
        {
            get
            {
                return this._inQueue;
            }

            set
            {
                if (this._inQueue != null)
                {
                    this._inQueue.ItemEnqueued -= this.InQueue_ItemEnqueued;
                    this._inQueue.Close();
                }

                // if some one is reseting the external queue, create the local queue
                this._inQueue = value ?? new TechieQueue();

                if (string.IsNullOrEmpty(this._inQueue.Name))
                {
                    this._inQueue.Name = this.Name + "_IN";
                }

                if (this._processInQueue)
                {
                    this._inQueue.ItemEnqueued += this.InQueue_ItemEnqueued;
                }

                this.ConfigureIncomingHeartbeatTimer();
            }
        }

        /// <summary>
        ///     IncomingHeartBeatTimeOut with default value of 15 ms. This value is used to check that server is alive
        /// </summary>
        public int IncomingHeartBeatTimeOut
        {
            get
            {
                return this._incomingHeartbeatTimeout;
            }

            set
            {
                if (this._incomingHeartbeatTimeout == value)
                {
                    return;
                }

                this._incomingHeartbeatTimeout = value;
                this.ConfigureIncomingHeartbeatTimer();
            }
        }

        /// <summary>Gets or sets the latency.</summary>
        public long Latency
        {
            get
            {
                return this._latency;
            }

            set
            {
                this._latency = value;
            }
        }

        /// <summary>Gets or sets the latency interval.</summary>
        public int LatencyInterval
        {
            get
            {
                return this._latencyInterval;
            }

            set
            {
                this._latencyInterval = value;
            }
        }

        /// <summary>Gets the manager status.</summary>
        public virtual ManagerStatus ManagerStatus
        {
            get
            {
                if (!this.IsInitialized)
                {
                    return ManagerStatus.Err;
                }

                return this._managerStatus;
            }
        }

        /// <summary>Gets the max objects to process.</summary>
        public int MaxObjectsToProcess
        {
            get
            {
                return _maxObjectsToProcess;
            }
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the out queue.</summary>
        public TechieQueue OutQueue
        {
            get
            {
                return this._outQueue;
            }

            set
            {
                if (this._outQueue != null)
                {
                    this._outQueue.ItemEnqueued -= this.OutQueue_ItemEnqueued;
                    this._outQueue.Close();
                }

                // if some one is reseting the external queue, create the local queue
                this._outQueue = value ?? new TechieQueue();

                if (string.IsNullOrEmpty(this._outQueue.Name))
                {
                    this._outQueue.Name = this.Name + "_OUT";
                }

                if (this._processOutQueue)
                {
                    this._outQueue.ItemEnqueued += this.OutQueue_ItemEnqueued;
                }

                this.ConfigureOutgoingHeartbeatTimer();
            }
        }

        /// <summary>
        ///     Heartbeat period in ms for outgoing HeartBeat to inform clients that server is alive
        /// </summary>
        public int OutgoingHeartbeatPeriod
        {
            get
            {
                return this._outgoingHeartbeatPeriod;
            }

            set
            {
                if (this._outgoingHeartbeatPeriod == value)
                {
                    return;
                }

                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat(
                        "{0}: New outgoing heartbeat. Name={1}, OutgoingHeartbeatPeriod={2}", 
                        MethodBase.GetCurrentMethod().Name, 
                        this.Name, 
                        this.OutgoingHeartbeatPeriod);
                }

                this._outgoingHeartbeatPeriod = value;
                this._outhbt = this._outhbt.NewHeartbeatPeriod(this._outgoingHeartbeatPeriod);

                this.ConfigureOutgoingHeartbeatTimer();
            }
        }

        /// <summary>Gets or sets a value indicating whether process raw data.</summary>
        public bool ProcessRawData { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether is initialized.</summary>
        protected bool IsInitialized
        {
            get
            {
                return this._initialized;
            }
        }

        /// <summary>Gets or sets the out hbt.</summary>
        protected Hbt.Hbt OutHbt
        {
            get
            {
                return this._outhbt;
            }

            set
            {
                this._outhbt = value;
            }
        }

        /// <summary>Gets a value indicating whether is connected.</summary>
        private bool IsConnected
        {
            get
            {
                return this._connection != null && this._connection.ConnectionStatus == ConnectionStatus.Open;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The set server mode.</summary>
        /// <param name="server">The server.</param>
        public static void SetServerMode(bool server)
        {
            _server = server;
        }

        /// <summary>The disconnect if heartbeat timedout.</summary>
        public void DisconnectIfHeartbeatTimedout()
        {
            if (this.EnableIncomingHeartBeat && this.HeartBeatOK)
            {
                var tickCount = Environment.TickCount;
                int tickDifferent;

                if (tickCount < this._lastIncomingTickCount)
                {
                    // TickCount rolled over
                    tickDifferent = int.MaxValue - this._lastIncomingTickCount + tickCount;
                }
                else
                {
                    tickDifferent = Environment.TickCount - this._lastIncomingTickCount;
                }

                if (tickDifferent > (this._incomingHeartbeatPeriod + this.IncomingHeartBeatTimeOut))
                {
                    if (this._connection.ConnectionStatus == ConnectionStatus.Open)
                    {
                        if (Log.IsWarnEnabled)
                        {
                            Log.WarnFormat(
                                "{0}: Stop connection. Name={1}, ConnectionAddress={2}", 
                                MethodBase.GetCurrentMethod().Name, 
                                this.Name, 
                                this.CurrentConnectionAddress);
                        }

                        this._connection.Disconnect();
                        this.HeartBeatOK = false;
                        this.SetManagerStatus(ManagerStatus.Err);
                    }
                }
            }
        }

        // only SocketAdapter needs this.  if SA is ever removed this can be made private and inlined
        /// <summary>The inbound object received.</summary>
        public void InboundObjectReceived()
        {
            if (this.EnableIncomingHeartBeat)
            {
                this.HeartBeatOK = true;
            }

            this.ResetIncomingTickCout();
        }

        /// <summary>The initialize.</summary>
        /// <param name="processInQueue">The process in queue.</param>
        /// <param name="processOutQueue">The process out queue.</param>
        /// <returns>The System.Boolean.</returns>
        public virtual bool Initialize(bool processInQueue, bool processOutQueue)
        {
            if (this._initialized)
            {
                return true;
            }

            lock (this._locker)
            {
                if (this._initialized)
                {
                    return true;
                }

                this._incomingHeartbeatTimer = new Timer(
                    this.IncomingHeartbeatTimer_Callback, null, Timeout.Infinite, Timeout.Infinite);
                this._outgoingHeartbeatTimer = new Timer(
                    this.OutgoingHeartbeatTimer_Callback, null, Timeout.Infinite, Timeout.Infinite);

                if (this._connection == null)
                {
                    this.EnableIncomingHeartBeat = false;
                }

                if (this.EnableIncomingHeartBeat == false)
                {
                    this.IncomingHeartBeatTimeOut = -1;
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: Init incoming HBT: Disabled. Name={1}, Queue Name={2}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this._inQueue.Name);
                    }
                }
                else
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: Init incoming HBT. Name={1}, Queue Name={2}, IncomingHeartBeatTimeOut={3}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this._inQueue.Name, 
                            this.IncomingHeartBeatTimeOut);
                    }
                }

                this._processInQueue = processInQueue;
                if (processInQueue)
                {
                    this._inQueueSignalConflator = new IocpPoolSignalConflator(this.InQueueSignalled_Callback);
                    this._inQueue.ItemEnqueued += this.InQueue_ItemEnqueued;

                    this.ConfigureIncomingHeartbeatTimer();
                }

                if (!this.EnableOutgoingHeartBeat)
                {
                    this._outgoingHeartbeatPeriod = -1;
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: Init outgoing HBT: Disabled. Name={1}, Queue Name={2}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this._outQueue.Name);
                    }
                }
                else
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: Init outgoing HBT. Name={1}, Queue Name={2}, OutgoingHeartbeatPeriod={3}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this._outQueue.Name, 
                            this.OutgoingHeartbeatPeriod);
                    }
                }

                this._processOutQueue = processOutQueue;
                if (processOutQueue)
                {
                    this._outQueueSignalConflator = new IocpPoolSignalConflator(this.OutQueueSignalled_Callback);
                    this._outQueue.ItemEnqueued += this.OutQueue_ItemEnqueued;

                    this.ConfigureOutgoingHeartbeatTimer();
                }

                this.ResetIncomingTickCout();

                this._initialized = true;
            }

            return this._initialized;
        }

        /// <summary>The initialize.</summary>
        /// <param name="connection">The connection.</param>
        /// <param name="processInQueue">The process in queue.</param>
        /// <param name="processOutQueue">The process out queue.</param>
        /// <returns>The System.Boolean.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual bool Initialize(IConnection connection, bool processInQueue, bool processOutQueue)
        {
            if (!this._initialized)
            {
                lock (this._locker)
                {
                    if (!this._initialized)
                    {
                        if (connection == null)
                        {
                            throw new ArgumentNullException("connection");
                        }

                        if (Log.IsWarnEnabled)
                        {
                            Log.WarnFormat(
                                "{0}: Connected socket. Name={1}, ConnectionAddress={2}, ConnectionStatus={3}", 
                                MethodBase.GetCurrentMethod().Name, 
                                this.Name, 
                                connection.ConnectionAddress, 
                                connection.ConnectionStatus);
                        }

                        this.EnableOutgoingHeartBeat = true;
                        this.EnableIncomingHeartBeat = true;

                        // SetManagerStatus will be called by SetConnection() methods
                        this.SetConnection(connection);

                        if (this.Initialize(processInQueue, processOutQueue))
                        {
                            if (connection.ConnectionStatus == ConnectionStatus.Open && this.EnableOutgoingHeartBeat)
                            {
                                // Send Init HeartBeat
                                this.SendHeartBeat(true);
                            }

                            this._initialized = true;
                        }
                    }
                }
            }

            return this._initialized;
        }

        /// <summary>The initialize.</summary>
        /// <param name="connectionAddresses">The connection addresses.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="processInQueue">The process in queue.</param>
        /// <param name="processOutQueue">The process out queue.</param>
        /// <returns>The System.Boolean.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual bool Initialize(
            string connectionAddresses, IConnection connection, bool processInQueue, bool processOutQueue)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (!this._initialized)
            {
                lock (this._locker)
                {
                    if (!this._initialized)
                    {
                        this._connectionAddresses.Clear();
                        connectionAddresses = connectionAddresses.Replace(
                            ConnectionALternateAddressDelimmiter, ConnectionAddressDelimmiter);
                        foreach (var address in connectionAddresses.Split(ConnectionAddressDelimmiter))
                        {
                            if (string.IsNullOrEmpty(address))
                            {
                                continue;
                            }

                            this._connectionAddresses.Add(address);
                        }

                        if (this._connectionAddresses.Count < 1)
                        {
                            throw new ArgumentException("Invalid connectionAddress: " + connectionAddresses);
                        }

                        connection.ConnectionAddress = this._connectionAddresses[this._currentconnectionAddressIndex];

                        this.EnableOutgoingHeartBeat = true;
                        this.EnableIncomingHeartBeat = true;

                        this.SetConnection(connection);

                        if (this.Initialize(processInQueue, processOutQueue))
                        {
                            this._connection.Connect();

                            this._initialized = true;
                        }
                    }
                }
            }

            return this._initialized;
        }

        /// <summary>The process incoming.</summary>
        /// <param name="objectIn">The object in.</param>
        /// <returns>The System.Boolean.</returns>
        public virtual bool ProcessIncoming(object objectIn)
        {
            return true;
        }

        /// <summary>The process incoming.</summary>
        /// <param name="objectsList">The objects list.</param>
        /// <returns>The System.Boolean.</returns>
        public virtual bool ProcessIncoming(List<object> objectsList)
        {
            foreach (var objectIn in objectsList)
            {
                try
                {
                    this.ProcessIncoming(objectIn);
                    if (this.FireIncommingObjectsAsEvents)
                    {
                        this.OnIncommingObject(objectIn);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
                }
            }

            return true;
        }

        /// <summary>NB : This method is only public because SocketAdapter uses it;
        ///     should be made protected after SocketAdapter is decomissioned</summary>
        /// <param name="objectIn"></param>
        public virtual void ProcessIncomingHeartBeat(object objectIn)
        {
            if (objectIn is Hbt.Hbt)
            {
                var hbt = (Hbt.Hbt)objectIn;

                // Send HeartBeatResponse for measuring Latency
                if (hbt.ResponseRequired)
                {
                    var dateTime = Tools.ConvertDateTimeToString(DateTime.UtcNow, true);
                    this._outhbt = this._outhbt.NewDateTime(dateTime);

                    var hbtResponse = new HbtResponse(hbt, dateTime);

                    this.SendOutAtHighPriority(hbtResponse);

                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: HBT response. Name={1}, ConnectionAddress={2}, {3}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this.CurrentConnectionAddress, 
                            hbtResponse.ToLog());
                    }
                }

                if (this._incomingHeartbeatPeriod != hbt.HeartbeatPeriod)
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat(
                            "{0}: New incoming HBT. Name={1}, ConnectionAddress={2}, {3}=>{4}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this.CurrentConnectionAddress, 
                            this.IncomingHeartBeatTimeOut + this._incomingHeartbeatPeriod, 
                            this.IncomingHeartBeatTimeOut + hbt.HeartbeatPeriod);
                    }

                    this._incomingHeartbeatPeriod = hbt.HeartbeatPeriod;
                    this.ConfigureIncomingHeartbeatTimer();
                }

                this.ResetIncomingTickCout();
            }
            else if (objectIn is HbtResponse)
            {
                // Manager status should be ok if heartbeat is received
                this.OnConnectionStatusChanged();

                // No need to process HeartBeat response as latency interval is not set
                if (this._latencyInterval <= 0)
                {
                    return;
                }

                var latency = this._sw.ElapsedMilliseconds;
                var hbtResponse = (HbtResponse)objectIn;
                long startTime;
                bool found;
                lock (this._latencyRequests)
                {
                    found = this._latencyRequests.TryGetValue(hbtResponse.Hbt.Key, out startTime);
                }

                if (found)
                {
                    var latencytime = (latency - startTime) / 2;
                    if (this._latency != latencytime)
                    {
                        this._latency = latencytime;
                        this.OnLatencyChanged(new LatencyEventArgs(string.Empty, this._latency));
                        if (Log.IsInfoEnabled)
                        {
                            Log.InfoFormat(
                                "{0}: Latency response. Name={1}, Time(ms)={2}, Time(eElapsed)={3}, {4}", 
                                MethodBase.GetCurrentMethod().Name, 
                                this.Name, 
                                this._latency, 
                                this._sw.Elapsed, 
                                hbtResponse.ToLog());
                        }
                    }

                    lock (this._latencyRequests)
                    {
                        this._latencyRequests.Remove(hbtResponse.Hbt.Key);
                    }
                }
                else
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.WarnFormat(
                            "{0}: Latency error - Key Not Found. Name={1}, Key={2}, {3}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            hbtResponse.Hbt.Key, 
                            hbtResponse.ToLog());
                    }
                }
            }
        }

        /// <summary>The process outgoing.</summary>
        public virtual void ProcessOutgoing()
        {
            object dequeued;
            while (this._outQueue.TryDequeue(out dequeued))
            {
                var writeObject = (ISendable)dequeued;
                if (!Write(writeObject))
                {
                    this._outQueue.Clear();
                    break;
                }
            }
        }

        /// <summary>The register all types consumer.</summary>
        /// <param name="queue">The queue.</param>
        /// <returns>The System.Boolean.</returns>
        public bool RegisterAllTypesConsumer(TechieQueue queue)
        {
            return this._consumers.RegisterAllTypesConsumer(queue);
        }

        /// <summary>The register consumer.</summary>
        /// <param name="type">The type.</param>
        /// <param name="queue">The queue.</param>
        public void RegisterConsumer(Type type, TechieQueue queue)
        {
            this._consumers.RegisterConsumer(type, queue);
        }

        /// <summary>The send in.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SendIn(object obj)
        {
            try
            {
                this._inQueue.Enqueue(obj);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }

            return false;
        }

        /// <summary>The send out.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SendOut(ISendable obj)
        {
            return this.SendOutInternal(obj, false);
        }

        /// <summary>The send out at high priority.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SendOutAtHighPriority(ISendable obj)
        {
            return this.SendOutInternal(obj, true);
        }

        /// <summary>The un initialize.</summary>
        public virtual void UnInitialize()
        {
            try
            {
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat(
                        "{0}: Name={1}, ConnectionAddress={2}, {3}", 
                        MethodBase.GetCurrentMethod().Name, 
                        this.Name, 
                        this.CurrentConnectionAddress, 
                        this.IsConnectedMessage());
                }

                this.UnHookConnectionEvent();
                this.Disconnect();
                this.SetManagerStatus(ManagerStatus.Err);
                this._initialized = false;

                this._inQueue.Close();
                this._outQueue.Close();

                this.InQueue.ItemEnqueued -= this.InQueue_ItemEnqueued;
                this.OutQueue.ItemEnqueued -= this.OutQueue_ItemEnqueued;

                lock (this._locker)
                {
                    if (this._incomingHeartbeatTimer != null)
                    {
                        this._incomingHeartbeatTimer.Dispose();
                        this._incomingHeartbeatTimer = null;
                    }

                    if (this._outgoingHeartbeatTimer != null)
                    {
                        this._outgoingHeartbeatTimer.Dispose();
                        this._outgoingHeartbeatTimer = null;
                    }

                    if (this._inQueueSignalConflator != null)
                    {
                        this._inQueueSignalConflator.Dispose();
                        this._inQueueSignalConflator = null;
                    }

                    if (this._outQueueSignalConflator != null)
                    {
                        this._outQueueSignalConflator.Dispose();
                        this._outQueueSignalConflator = null;
                    }
                }

                if (this._latencyTimer != null)
                {
                    this._latencyTimer.Dispose();
                    this._latencyTimer = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
            finally
            {
                this._initialized = false;
            }
        }

        /// <summary>The un register consumer.</summary>
        /// <param name="type">The type.</param>
        /// <param name="queue">The queue.</param>
        public void UnRegisterConsumer(Type type, TechieQueue queue)
        {
            this._consumers.UnRegisterConsumer(type, queue);
        }

        #endregion

        #region Methods

        /// <summary>The configure incoming heartbeat timer.</summary>
        protected void ConfigureIncomingHeartbeatTimer()
        {
            if (!this.EnableIncomingHeartBeat || this.IncomingHeartBeatTimeOut <= 0)
            {
                this.DisableIncomingHeartbeatTimer();
                return;
            }

            lock (this._locker)
            {
                if (this._incomingHeartbeatTimer != null)
                {
                    this._incomingHeartbeatTimer.Change(
                        this._incomingHeartbeatPeriod + this.IncomingHeartBeatTimeOut, Timeout.Infinite);
                }
            }
        }

        /// <summary>The connection_ receive.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected virtual void Connection_Receive(object sender, ReceiveEventArgs e)
        {
            try
            {
                object receivedObject;
                if (this.ProcessRawData)
                {
                    var rawData = new RawData(e.Data, e.PacketType);
                    receivedObject = rawData;
                }
                else
                {
                    receivedObject = TechieObjectFactory.Deserialize(e.Data, e.PacketType);

                    var readOnly = receivedObject as IReadOnly;
                    if (readOnly != null)
                    {
                        readOnly.SetReadOnly();
                    }
                }

                this.OnReceive(receivedObject, e);

                this._inQueue.Enqueue(receivedObject);
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
        }

        /// <summary>The create exception manager info message.</summary>
        /// <returns>The System.String.</returns>
        protected string CreateExceptionManagerInfoMessage()
        {
            const string NullText = "(null)";

            return string.Format(
                "ManagerInfo: Name:{0}, InQueue:{1}, OutQueue:{2}; Connection: {3}", 
                this.Name, 
                this._inQueue != null ? this._inQueue.Name : NullText, 
                this._outQueue != null ? this._outQueue.Name : NullText, 
                this.CurrentConnectionAddress);
        }

        /// <summary>The disconnect.</summary>
        protected void Disconnect()
        {
            try
            {
                if (this._connection != null)
                {
                    this._connection.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
        }

        /// <summary>The latency check.</summary>
        /// <param name="stateInfo">The state info.</param>
        protected virtual void LatencyCheck(object stateInfo)
        {
            try
            {
                if (!this._sw.IsRunning)
                {
                    this._sw.Start();
                }

                if (this.ManagerStatus == ManagerStatus.Ok)
                {
                    var key = this.NextOutgoingHeartBeatKey();
                    var hbt = new Hbt.Hbt(
                        key.ToString(), 
                        Tools.ConvertDateTimeToString(DateTime.UtcNow, true), 
                        this._outhbt.HeartbeatPeriod, 
                        true);
                    lock (this._latencyRequests)
                    {
                        this._latencyRequests[hbt.Key] = this._sw.ElapsedMilliseconds;
                    }

                    this.SendOutAtHighPriority(hbt);
                    if (Log.IsInfoEnabled)
                    {
                        Log.InfoFormat("{0}: Name={1}, {2}", MethodBase.GetCurrentMethod().Name, this.Name, hbt.ToLog());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
        }

        /// <summary>The next outgoing heart beat key.</summary>
        /// <returns>The System.Int32.</returns>
        protected int NextOutgoingHeartBeatKey()
        {
            return Interlocked.Increment(ref this._outgoingHeartBeatKey);
        }

        /// <summary>The on connection closed.</summary>
        protected virtual void OnConnectionClosed()
        {
            EventPublisher.RaiseEvent(this.ConnectionClosed, this, EventArgs.Empty);
        }

        /// <summary>The on connection status changed.</summary>
        protected void OnConnectionStatusChanged()
        {
            if (this._incomingHeartbeatPeriod > 0)
            {
                this.ResetIncomingTickCout();
            }

            if (this.IsConnected)
            {
                this.SetManagerStatus(ManagerStatus.Ok);
            }
            else
            {
                this.SetManagerStatus(ManagerStatus.Err);
            }
        }

        /// <summary>The on incomming object.</summary>
        /// <param name="obj">The obj.</param>
        protected virtual void OnIncommingObject(object obj)
        {
            Debug.Assert(this.FireIncommingObjectsAsEvents, "Firing when not enabled");
            EventPublisher.RaiseEvent(this.IncommingObject, this, new IncommingObjectEventArgs(obj));
        }

        /// <summary>The on latency changed.</summary>
        /// <param name="e">The e.</param>
        protected virtual void OnLatencyChanged(LatencyEventArgs e)
        {
            EventPublisher.RaiseEvent(this.LatencyChanged, this, e);
        }

        /// <summary>The on manager status.</summary>
        protected virtual void OnManagerStatus()
        {
            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat(
                    "{0}: Name={1}, ManagerStatus={2}", 
                    MethodBase.GetCurrentMethod().Name, 
                    this.Name, 
                    this._managerStatus);
            }

            this.ManagerStatusEvent.FireEvent(
                this, new ManagerStatusEventArgs(this.ManagerStatus, this.Name, 0, string.Empty));
        }

        /// <summary>The on receive.</summary>
        /// <param name="receivedObject">The received object.</param>
        /// <param name="e">The e.</param>
        protected virtual void OnReceive(object receivedObject, ReceiveEventArgs e)
        {
            if (Log.IsInfoEnabled)
            {
                var sampleString = string.Format(
                    "Manager:{0}| Size:{1}| Queue:{2}", this.Name, e.Data.Length, this._inQueue.Name);

                Log.InfoFormat(
                    "RECEIVED: Key={0}, LoggingKey={1}, Sampling=({2}), {3}", 
                    this.ToKey(2), 
                    receivedObject is IToKey ? ((IToKey)receivedObject).ToKey() : receivedObject.GetType().ToString(), 
                    sampleString, 
                    LoggableObject.ToVerboseLog(receivedObject));
            }
        }

        /// <summary>The on transmit.</summary>
        /// <param name="writeObject">The write object.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="isLoggingRequired">The is logging required.</param>
        protected virtual void OnTransmit(object writeObject, byte[] bytes, bool isLoggingRequired)
        {
            if (isLoggingRequired && Log.IsInfoEnabled)
            {
                var loggingKey = writeObject is IToKey
                                     ? ((IToKey)writeObject).ToKey()
                                     : writeObject.GetType().ToString();

                var sampleString = string.Format(
                    "Manager:{0}| Queue: {1} | Size:{2}", this.Name, this._outQueue.Name, bytes.Length);

                Log.InfoFormat(
                    "SENDING: Key={0}, LoggingKey={1}, Sampling=({2}), {3}", 
                    this.ToKey(2), 
                    loggingKey, 
                    sampleString, 
                    LoggableObject.ToLog(writeObject));
            }
        }

        /// <summary>The process incoming.</summary>
        protected virtual void ProcessIncoming()
        {
            List<object> items;
            while ((items = this._inQueue.DequeueBatch(_maxObjectsToProcess)).Count > 0)
            {
                foreach (var objectIn in items)
                {
                    if (this.EnableIncomingHeartBeat)
                    {
                        this.HeartBeatOK = true;

                        // do not call ProcessIncomingHeartBeat with messages that are not hbt-related
                        if (objectIn is IHbt)
                        {
                            this.ProcessIncomingHeartBeat(objectIn);
                        }
                    }

                    this._consumers.CheckConsumers(objectIn);
                }

                if (items.Count > 0)
                {
                    this.ProcessIncoming(items);
                }
            }
        }

        /// <summary>
        ///     Resets the last incoming tick count to Environment.TickCount.
        /// </summary>
        protected void ResetIncomingTickCout()
        {
            this._lastIncomingTickCount = Environment.TickCount;
        }

        /// <summary>Send the heartbeat.</summary>
        /// <param name="responseRequired">The response Required.</param>
        /// <remarks>Places the heartbeat at the top of the queue for send out to avoid race conditions.</remarks>
        protected virtual void SendHeartBeat(bool responseRequired)
        {
            if (this._outhbt.HeartbeatPeriod <= 0)
            {
                Log.ErrorFormat("Invalid HeartBeatPeriod. Manager {0}, _outhbt {1}", this.Name, this._outhbt.ToLog());
            }

            var key = this.NextOutgoingHeartBeatKey();

            this._outhbt = new Hbt.Hbt(
                key.ToString(), 
                Tools.ConvertDateTimeToString(DateTime.UtcNow, true), 
                this._outhbt.HeartbeatPeriod, 
                responseRequired, 
                this._outhbt.PacketType);

            if (responseRequired)
            {
                if (!this._sw.IsRunning)
                {
                    this._sw.Start();
                }

                lock (this._latencyRequests)
                {
                    this._latencyRequests[_outhbt.Key] = this._sw.ElapsedMilliseconds;
                }
            }

            this.SendOutAtHighPriority(this._outhbt);
        }

        /// <summary>The send identity.</summary>
        protected virtual void SendIdentity()
        {
        }

        /// <summary>The send out internal.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The System.Boolean.</returns>
        protected virtual bool SendOutInternal(ISendable obj, bool priority)
        {
            try
            {
                if (this._connection == null)
                {
                    Log.ErrorFormat(
                        "Object not enqueued because connection is not set. Manager {0}, Object {1}", 
                        this.Name, 
                        LoggableObject.ToLog(obj));

                    return false;
                }

                if (priority)
                {
                    this._outQueue.EnqueueAtHead(obj);
                }
                else
                {
                    this._outQueue.Enqueue(obj);
                }

                return true;
            }
            catch (InvalidOperationException ex)
            {
                // FXJ1-9871 If we have disconnected we expect this harmless error from the TechieQueue
                if (this._initialized)
                {
                    Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }

            return false;
        }

        /// <summary>The set heartbeat packet type.</summary>
        /// <param name="packetType">The packet type.</param>
        protected void SetHeartbeatPacketType(PacketType packetType)
        {
            this._outhbt = new Hbt.Hbt(
                this._outhbt.Key, 
                this._outhbt.DateTime, 
                this._outhbt.HeartbeatPeriod, 
                this._outhbt.ResponseRequired, 
                packetType);
        }

        /// <summary>The set manager status.</summary>
        /// <param name="managerStatus">The manager status.</param>
        protected void SetManagerStatus(ManagerStatus managerStatus)
        {
            if (this._managerStatus != managerStatus)
            {
                this._managerStatus = managerStatus;
                this.OnManagerStatus();
                if (this._managerStatus == ManagerStatus.Ok)
                {
                    this.InitializeLatencyCheck();
                }
            }
        }

        /// <summary>The to key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The System.String.</returns>
        protected virtual string ToKey(int key)
        {
            return this.CurrentConnectionAddress;
        }

        /// <summary>The write.</summary>
        /// <param name="writeObject">The write object.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool Write(ISendable writeObject)
        {
            return Write(writeObject, true);
        }

        /// <summary>The write.</summary>
        /// <param name="writeObject">The write object.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool Write(RawData writeObject)
        {
            return Write(writeObject, true);
        }

        /// <summary>The write.</summary>
        /// <param name="writeObject">The write object.</param>
        /// <param name="isLoggingRequired">The is logging required.</param>
        /// <returns>The System.Boolean.</returns>
        protected virtual bool Write(ISendable writeObject, bool isLoggingRequired)
        {
            byte[] bytes = TechieObjectFactory.Serialize(writeObject, writeObject.PacketType);

            return this.Write(bytes, writeObject.PacketType, writeObject, isLoggingRequired);
        }

        /// <summary>The write.</summary>
        /// <param name="writeObject">The write object.</param>
        /// <param name="isLoggingRequired">The is logging required.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool Write(RawData writeObject, bool isLoggingRequired)
        {
            return this.Write(writeObject.Data, writeObject.PacketType, writeObject, isLoggingRequired);
        }

        /// <summary>The write.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="packetType">The packet type.</param>
        /// <param name="writeObject">The write object.</param>
        /// <param name="isLoggingRequired">The is logging required.</param>
        /// <returns>The System.Boolean.</returns>
        protected bool Write(byte[] bytes, PacketType packetType, ISendable writeObject, bool isLoggingRequired)
        {
            var loggingKey = writeObject is IToKey ? ((IToKey)writeObject).ToKey() : writeObject.GetType().ToString();

            if (this._connection == null)
            {
                Log.ErrorFormat(
                    "Object not send because connection is not set. Manager {0}, loggingKey {1}", this.Name, loggingKey);

                return false;
            }

            if (!this.IsConnected)
            {
                if (isLoggingRequired)
                {
                    Log.ErrorFormat(
                        "Object not send because connection status is not connected. Manager {0}, Address {1}, loggingKey {2}", 
                        this.Name, 
                        this._connection.ConnectionAddress, 
                        loggingKey);
                }

                return false;
            }

            try
            {
                this.OnTransmit(writeObject, bytes, isLoggingRequired);

                this._connection.Send(bytes, packetType);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
                this.Disconnect();
            }

            return false;
        }

        /// <summary>The configure outgoing heartbeat timer.</summary>
        private void ConfigureOutgoingHeartbeatTimer()
        {
            if (!this.EnableOutgoingHeartBeat || this.OutgoingHeartbeatPeriod <= 0)
            {
                this.DisableOutgoingHeartbeatTimer();
                return;
            }

            lock (this._locker)
            {
                if (this._outgoingHeartbeatTimer != null)
                {
                    this._outgoingHeartbeatTimer.Change(this.OutgoingHeartbeatPeriod, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        ///     Attempt the connection to the next address if any
        /// </summary>
        /// <remarks>Connections will be slowed down by with more failed connect attempts</remarks>
        private void ConnectNextAddress()
        {
            if (this._connectionAddresses.Count > 0)
            {
                this._connectFailCount++;

                if (this._connectFailCount > this._connectionAddresses.Count)
                {
                    if (this._connectDelayIndex < this._connectDelayMatrix.Length - 1)
                    {
                        this._connectDelayIndex++;
                    }
                }

                if (this._connectDelayMatrix[this._connectDelayIndex] > 0)
                {
                    Thread.Sleep(this._connectDelayMatrix[this._connectDelayIndex] * 1000);
                }

                if (!this._initialized)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.WarnFormat(
                            "{0}: Manager not initialized, giving up. Name={1}, ConnectionAddress={2}", 
                            MethodBase.GetCurrentMethod().Name, 
                            this.Name, 
                            this._connection.ConnectionAddress);
                    }

                    return;
                }

                this._currentconnectionAddressIndex++;
                if (this._currentconnectionAddressIndex >= this._connectionAddresses.Count)
                {
                    this._currentconnectionAddressIndex = 0;
                }

                this._connection.ConnectionAddress = this._connectionAddresses[this._currentconnectionAddressIndex];

                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat(
                        "{0}: Connecting to next address. Name={1}, ConnectionAddress={2}, ConnectionStatus={3}", 
                        MethodBase.GetCurrentMethod().Name, 
                        this.Name, 
                        this._connection.ConnectionAddress, 
                        this._connection.ConnectionStatus);
                }

                this._connection.Disconnect();
                this._connection.Connect();
            }
        }

        /// <summary>The connection_ status changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Connection_StatusChanged(object sender, EventArgs e)
        {
            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat(
                    "{0}: Name={1}, ConnectionAddress={2}, ConnectionStatus={3}", 
                    MethodBase.GetCurrentMethod().Name, 
                    this.Name, 
                    this._connection.ConnectionAddress, 
                    this._connection.ConnectionStatus);
            }

            switch (this._connection.ConnectionStatus)
            {
                case ConnectionStatus.Open:
                    if (!this._initialized)
                    {
                        if (Log.IsWarnEnabled)
                        {
                            Log.WarnFormat(
                                "{0}: Manager not initialized, closing. Name={1}, ManagerStatus={2}, ConnectionAddress={3}, {4}", 
                                MethodBase.GetCurrentMethod().Name, 
                                this.Name, 
                                this._managerStatus, 
                                this._connection.ConnectionAddress, 
                                this.IsConnectedMessage());
                        }

                        this.Disconnect();
                        this.SetManagerStatus(ManagerStatus.Err);
                        break;
                    }

                    this._connectFailCount = 0;
                    this._connectDelayIndex = 0;

                    if (this.EnableOutgoingHeartBeat)
                    {
                        // Send Init HeartBeat
                        this.SendHeartBeat(true);
                    }
                    else
                    {
                        this.OnConnectionStatusChanged();
                    }

                    break;
                case ConnectionStatus.Opening:
                    break;
                case ConnectionStatus.Closed:
                    if (!this._initialized)
                    {
                        if (Log.IsWarnEnabled)
                        {
                            Log.WarnFormat(
                                "{0}: Manager not initialized, giving up. Name={1}, ConnectionAddress={2}", 
                                MethodBase.GetCurrentMethod().Name, 
                                this.Name, 
                                this._connection.ConnectionAddress);
                        }

                        break;
                    }

                    this.Disconnect();
                    this.OnConnectionStatusChanged();
                    this.OnConnectionClosed();
                    this.ConnectNextAddress();
                    break;
                default:
                    Debug.Assert(false, "Unknown ConnectionStatus: " + this._connection.ConnectionStatus);
                    this.OnConnectionStatusChanged();
                    break;
            }
        }

        /// <summary>The disable incoming heartbeat timer.</summary>
        private void DisableIncomingHeartbeatTimer()
        {
            lock (this._locker)
            {
                if (this._incomingHeartbeatTimer != null)
                {
                    this._incomingHeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>The disable outgoing heartbeat timer.</summary>
        private void DisableOutgoingHeartbeatTimer()
        {
            lock (this._locker)
            {
                if (this._outgoingHeartbeatTimer != null)
                {
                    this._outgoingHeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>The in queue signalled_ callback.</summary>
        private void InQueueSignalled_Callback()
        {
            try
            {
                this.DisableIncomingHeartbeatTimer();
                this.InboundObjectReceived();
                this.ProcessIncoming();
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
            finally
            {
                this.ConfigureIncomingHeartbeatTimer();
            }
        }

        /// <summary>The in queue_ item enqueued.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InQueue_ItemEnqueued(object sender, EventArgs e)
        {
            try
            {
                var conflator = this._inQueueSignalConflator;
                if (conflator == null)
                {
                    return;
                }

                conflator.Signal();
            }
            catch (ObjectDisposedException)
            {
                // prefer to live with this benign possibility than lock in this code path
            }
        }

        /// <summary>The incoming heartbeat timer_ callback.</summary>
        /// <param name="state">The state.</param>
        private void IncomingHeartbeatTimer_Callback(object state)
        {
            this.DisconnectIfHeartbeatTimedout();
            this.ConfigureIncomingHeartbeatTimer();
        }

        /// <summary>
        ///     Initializes Latency check.
        /// </summary>
        private void InitializeLatencyCheck()
        {
            if (this._latencyInterval > 0)
            {
                this._latencyTimer = new Timer(this.LatencyCheck, null, 0, this.LatencyInterval);
                this._sw = new Stopwatch();
            }
        }

        /// <summary>The is connected message.</summary>
        /// <returns>The System.String.</returns>
        private string IsConnectedMessage()
        {
            return this.IsConnected ? "Connected" : "Disconnected";
        }

        /// <summary>The out queue signalled_ callback.</summary>
        private void OutQueueSignalled_Callback()
        {
            try
            {
                this.DisableOutgoingHeartbeatTimer();
                this.ProcessOutgoing();
            }
            catch (Exception ex)
            {
                Log.Error(this.CreateExceptionManagerInfoMessage(), ex);
            }
            finally
            {
                this.ConfigureOutgoingHeartbeatTimer();
            }
        }

        /// <summary>The out queue_ item enqueued.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OutQueue_ItemEnqueued(object sender, EventArgs e)
        {
            try
            {
                var conflator = this._outQueueSignalConflator;
                if (conflator == null)
                {
                    return;
                }

                conflator.Signal();
            }
            catch (ObjectDisposedException)
            {
                // prefer to live with this benign possibility than lock in this code path
            }
        }

        /// <summary>The outgoing heartbeat timer_ callback.</summary>
        /// <param name="state">The state.</param>
        private void OutgoingHeartbeatTimer_Callback(object state)
        {
            if ((this._connection != null) && (this._connection.ConnectionStatus == ConnectionStatus.Open))
            {
                this.SendHeartBeat(false);
            }
        }

        /// <summary>The set connection.</summary>
        /// <param name="connection">The connection.</param>
        private void SetConnection(IConnection connection)
        {
            this.UnHookConnectionEvent();

            this._connection = connection;
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat(
                    "{0}: Manager={1}, Address={2}", 
                    MethodBase.GetCurrentMethod().Name, 
                    this.Name, 
                    this.CurrentConnectionAddress);
            }

            this._connection.StatusChanged += this.Connection_StatusChanged;
            this._connection.Receive += this.Connection_Receive;
        }

        /// <summary>The un hook connection event.</summary>
        private void UnHookConnectionEvent()
        {
            if (this._connection != null)
            {
                Log.DebugFormat(
                    "{0}: Manager={1}, Address={2}, ConnectionStatus={3}", 
                    MethodBase.GetCurrentMethod().Name, 
                    this.Name, 
                    this.CurrentConnectionAddress, 
                    this._connection.ConnectionStatus);

                // This should be in the reverse order of subscription
                this._connection.Receive -= this.Connection_Receive;
                this._connection.StatusChanged -= this.Connection_StatusChanged;
            }
        }

        #endregion

        /// <summary>The incomming object event args.</summary>
        public class IncommingObjectEventArgs : LoggableEventArgs
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="IncommingObjectEventArgs"/> class.</summary>
            /// <param name="obj">The obj.</param>
            public IncommingObjectEventArgs(object obj)
            {
                this.Object = obj;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the object.</summary>
            public object Object { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>The to log.</summary>
            /// <returns>The System.String.</returns>
            public override string ToLog()
            {
                var sb = new StringBuilder();
                sb.Append(base.ToLog());
                sb.AppendLine();

                LogUtil.Append(sb, LoggableObject.ToLog(this.Object));

                return sb.ToString();
            }

            #endregion
        }

        /// <summary>The latency event args.</summary>
        public class LatencyEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="LatencyEventArgs"/> class.</summary>
            /// <param name="user">The user.</param>
            /// <param name="latency">The latency.</param>
            public LatencyEventArgs(string user, long latency)
            {
                this.User = user;
                this.Latency = latency;
                this.UpdateTime = DateTime.UtcNow;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the latency.</summary>
            public long Latency { get; private set; }

            /// <summary>Gets the update time.</summary>
            public DateTime UpdateTime { get; private set; }

            /// <summary>Gets or sets the user.</summary>
            public string User { get; set; }

            #endregion
        }

        /// <summary>The manager status event args.</summary>
        public class ManagerStatusEventArgs : EventArgs
        {
            #region Fields

            /// <summary>The _error code.</summary>
            private readonly int _errorCode;

            /// <summary>The _error string.</summary>
            private readonly string _errorString = string.Empty;

            /// <summary>The _name.</summary>
            private readonly string _name;

            /// <summary>The _status.</summary>
            private readonly ManagerStatus _status = ManagerStatus.Err;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="ManagerStatusEventArgs"/> class.</summary>
            /// <param name="managerStatus">The manager status.</param>
            /// <param name="name">The name.</param>
            public ManagerStatusEventArgs(ManagerStatus managerStatus, string name)
            {
                this._status = managerStatus;
                this._name = name;
            }

            /// <summary>Initializes a new instance of the <see cref="ManagerStatusEventArgs"/> class.</summary>
            /// <param name="managerStatus">The manager status.</param>
            /// <param name="name">The name.</param>
            /// <param name="errorCode">The error code.</param>
            /// <param name="errorString">The error string.</param>
            public ManagerStatusEventArgs(ManagerStatus managerStatus, string name, int errorCode, string errorString)
            {
                this._status = managerStatus;
                this._errorCode = errorCode;
                this._name = name;
                this._errorString = errorString;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the error code.</summary>
            public int ErrorCode
            {
                get
                {
                    return this._errorCode;
                }
            }

            /// <summary>Gets the error string.</summary>
            public string ErrorString
            {
                get
                {
                    return this._errorString;
                }
            }

            /// <summary>Gets the manager status.</summary>
            public ManagerStatus ManagerStatus
            {
                get
                {
                    return this._status;
                }
            }

            /// <summary>Gets the name.</summary>
            public string Name
            {
                get
                {
                    return this._name;
                }
            }

            #endregion
        }
    }
}