//  ===================================================================================
//  <copyright file="LogData.cs" company="TechieNotes">
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
//     The LogData.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics;
using System.Text;

using TechieNotes.Common.Serialization;

using TradeFx.Common.Creation;
using TradeFx.Common.Serialization;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Logging
{
    /// <summary>
    ///     Represents a Log entry
    /// </summary>
    [Serializable]
    public sealed class LogData : ITechieNotesDelimitedSerialize, ILoggable, IGetShortName, ISendable
    {
        #region Constants

        /// <summary>The short name.</summary>
        public const string SHORT_NAME = "LOGDATA";

        /// <summary>The key seperator.</summary>
        private const char KEY_SEPERATOR = '\t';

        /// <summary>The log time format.</summary>
        private const string LOG_TIME_FORMAT = "HH:mm:ss:fff";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogData" /> class.
        /// </summary>
        public LogData()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LogData"/> class.</summary>
        /// <param name="logData">The log data.</param>
        public LogData(LogData logData)
        {
            this.CopyFrom(logData);
        }

        /// <summary>Initializes a new instance of the <see cref="LogData"/> class.</summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logTime">The log time.</param>
        /// <param name="log">The log.</param>
        public LogData(Logger.LogLevel logLevel, DateTime logTime, string log)
        {
            this.LogLevel = logLevel;

            this.LogTime = logTime;
            this.Log = log;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether conflate.</summary>
        public bool Conflate
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets the conflate key.</summary>
        /// <exception cref="NotImplementedException">This is not required.</exception>
        public Key ConflateKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the log.</summary>
        public string Log { get; private set; }

        /// <summary>Gets the log level.</summary>
        public Logger.LogLevel LogLevel { get; private set; }

        /// <summary>Gets the log time.</summary>
        public DateTime LogTime { get; private set; }

        /// <summary>Gets the packet type.</summary>
        public PacketType PacketType
        {
            get
            {
                return PacketType.TechieNotesDelimited;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The translate log level.</summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The System.String.</returns>
        public static string TranslateLogLevel(Logger.LogLevel logLevel)
        {
            var translatedLogLevel = "INF";

            switch (logLevel)
            {
                case Logger.LogLevel.Err:
                    translatedLogLevel = "ERR";
                    break;
                case Logger.LogLevel.Inf:
                    translatedLogLevel = "INF";
                    break;
                default:
                    Debug.Assert(false, "Unknown log level: " + logLevel);
                    break;
            }

            return translatedLogLevel;
        }

        /// <summary>The create.</summary>
        /// <returns>The System.Object.</returns>
        public object Create()
        {
            return new LogData();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        public void Deserialize(TechieNotesDelimitedParser parser)
        {
            using (new TechieNotesDelimitedParser.Terminator(parser))
            {
                this.LogLevel = parser.GetEnum<Logger.LogLevel>();
                this.LogTime = parser.GetDateTime();
                this.Log = parser.GetString();
            }
        }

        /// <summary>The get id.</summary>
        /// <returns>The TechieNotes.Common.Creation.ObjectId.</returns>
        public ObjectId GetId()
        {
            return ObjectId.LogData;
        }

        /// <summary>The get short name.</summary>
        /// <returns>The System.String.</returns>
        public string GetShortName()
        {
            return SHORT_NAME;
        }

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        public void Serialize(TechieNotesDelimitedBuilder builder)
        {
            using (new TechieNotesDelimitedBuilder.Terminator(builder))
            {
                builder.Append(this.LogLevel);
                builder.Append(this.LogTime, DateTimeOption.DateAndTime);
                builder.Append(this.Log);
            }
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            var sb = new StringBuilder();

            LogUtil.Append(sb, "ShortName", this.GetShortName());
            LogUtil.Append(sb, "LogLevel", this.LogLevel);
            LogUtil.Append(sb, "Log", this.Log);

            return sb.ToString();
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString()
        {
            var logLevel = TranslateLogLevel(this.LogLevel);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(this.LogTime.ToString(LOG_TIME_FORMAT));
            stringBuilder.Append(KEY_SEPERATOR);
            stringBuilder.Append(logLevel);

            if (!string.IsNullOrEmpty(this.Log))
            {
                stringBuilder.Append(KEY_SEPERATOR);
                stringBuilder.Append(this.Log);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #region Methods

        /// <summary>The copy from.</summary>
        /// <param name="otherLogData">The other log data.</param>
        private void CopyFrom(LogData otherLogData)
        {
            this.LogLevel = otherLogData.LogLevel;
            this.LogTime = otherLogData.LogTime;
            this.Log = otherLogData.Log;
        }

        #endregion
    }
}