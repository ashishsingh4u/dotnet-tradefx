//  ===================================================================================
//  <copyright file="LogUtil.cs" company="TechieNotes">
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
//     The LogUtil.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Globalization;
using System.Text;

using log4net;
using log4net.Core;

using TradeFx.Common.Helpers;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Logging
{
    /// <summary>
    ///     Logging helper utility
    /// </summary>
    /// <remarks>
    ///     In order to get logging entries on a line by line basis do: LogUtil.FieldSeparator = System.Environment.NewLine
    ///     Append methods are returning the string builder to allow for chaining of calls
    /// </remarks>
    public static class LogUtil
    {
        #region Constants

        /// <summary>The password mask.</summary>
        public const char PASSWORD_MASK = '*';

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="LogUtil" /> class.
        /// </summary>
        static LogUtil()
        {
            FieldSeparator = ",";
            KeyValueSeparator = "=";
            NullText = null;
            DoubleFormat = "F6";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the double format.</summary>
        public static string DoubleFormat { get; set; }

        /// <summary>Gets or sets the field separator.</summary>
        public static string FieldSeparator { get; set; }

        /// <summary>Gets or sets the key value separator.</summary>
        public static string KeyValueSeparator { get; set; }

        /// <summary>Gets or sets the null text.</summary>
        public static string NullText { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldValue)
        {
            if (fieldValue == null && NullText == null)
            {
                return sb;
            }

            sb.Append(fieldValue ?? NullText);
            sb.Append(FieldSeparator);

            return sb;
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, string fieldValue)
        {
            if (fieldValue == null && NullText == null)
            {
                return sb;
            }

            sb.Append(fieldName);
            sb.Append(KeyValueSeparator);
            sb.Append(fieldValue ?? NullText);
            sb.Append(FieldSeparator);

            return sb;
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, double fieldValue)
        {
            return Append(sb, fieldName, fieldValue.ToString(DoubleFormat));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, double? fieldValue)
        {
            if (fieldValue.HasValue)
            {
                return Append(sb, fieldName, ((double)fieldValue).ToString(DoubleFormat));
            }

            return Append(sb, fieldName, (string)null);
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, int fieldValue)
        {
            return Append(sb, fieldName, fieldValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, int? fieldValue)
        {
            if (fieldValue.HasValue)
            {
                return Append(sb, fieldName, (int)fieldValue);
            }

            return Append(sb, fieldName, (string)null);
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, bool fieldValue)
        {
            return Append(sb, fieldName, fieldValue.ToString());
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, bool? fieldValue)
        {
            if (fieldValue.HasValue)
            {
                return Append(sb, fieldName, fieldValue.ToString());
            }

            return Append(sb, fieldName, (string)null);
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, DateTime fieldValue)
        {
            return Append(sb, Tools.ConvertDateTimeToString(fieldValue, true));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, DateTime fieldValue)
        {
            return Append(sb, fieldName, Tools.ConvertDateTimeToString(fieldValue, true));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, DateTime? fieldValue)
        {
            if (fieldValue.HasValue)
            {
                return Append(sb, fieldName, (DateTime)fieldValue);
            }

            return Append(sb, fieldName, (string)null);
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, char fieldValue)
        {
            return Append(sb, fieldName, fieldValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, char? fieldValue)
        {
            if (fieldValue.HasValue)
            {
                return Append(sb, fieldName, fieldValue.ToString());
            }

            return Append(sb, fieldName, (string)null);
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, Enum fieldValue)
        {
            if (ReferenceEquals(fieldValue, null))
            {
                return Append(sb, fieldName, NullText);
            }

            return Append(sb, fieldName, fieldValue.ToString());
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, ILoggable fieldValue)
        {
            if (ReferenceEquals(fieldValue, null))
            {
                return Append(sb, fieldName, NullText);
            }

            return Append(sb, fieldName, fieldValue.ToLog());
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <typeparam name="T">Type to log.</typeparam>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append<T>(StringBuilder sb, string fieldName, T[] fieldValue)
        {
            if (ReferenceEquals(fieldValue, null))
            {
                return Append(sb, fieldName, NullText);
            }

            return Append(sb, fieldName, string.Join(",", Array.ConvertAll(fieldValue, ToLog)));
        }

        /// <summary>The append.</summary>
        /// <param name="sb">The sb.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>The System.Text.StringBuilder.</returns>
        public static StringBuilder Append(StringBuilder sb, string fieldName, object fieldValue)
        {
            if (ReferenceEquals(fieldValue, null))
            {
                return Append(sb, fieldName, NullText);
            }

            return Append(sb, fieldName, fieldValue.ToString());
        }

        /// <summary>Mask the password</summary>
        /// <param name="password">password to be masked</param>
        /// <returns>Masked password string</returns>
        public static string MaskPassword(string password)
        {
            return new string(PASSWORD_MASK, 8);
        }

        #endregion

        #region Methods

        /// <summary>The to log.</summary>
        /// <param name="obj">The obj.</param>
        /// <typeparam name="T">Type to log.</typeparam>
        /// <returns>The System.String.</returns>
        private static string ToLog<T>(T obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return NullText;
            }

            var loggable = obj as ILoggable;
            if (!ReferenceEquals(loggable, null))
            {
                return loggable.ToLog();
            }

            return obj.ToString();
        }

        #endregion

        /// <summary>
        ///     log4net helper methods
        /// </summary>
        public static class Log4Net
        {
            #region Public Methods and Operators

            /// <summary>log4net wrapper to provide single point logging for different parameterized levels</summary>
            /// <param name="log">log4net logger</param>
            /// <param name="level">Logging level</param>
            /// <param name="message">Message to log</param>
            public static void Log(ILog log, Level level, object message)
            {
                if (level == Level.Debug)
                {
                    log.Debug(message);
                }
                else if (level == Level.Info)
                {
                    log.Info(message);
                }
                else if (level == Level.Warn)
                {
                    log.Warn(message);
                }
                else if (level == Level.Fatal)
                {
                    log.Fatal(message);
                }
                else
                {
                    log.Error(message);
                }
            }

            /// <summary>Log message to the logger and to the console</summary>
            /// <param name="log">log4net logger</param>
            /// <param name="level">Logging level</param>
            /// <param name="message">Message to be logged</param>
            /// <remarks>We do not use console appender because if memory and performance issues so explicit console write is needed</remarks>
            /// <remarks>This method must not be abused and only used when really required!</remarks>
            public static void LogConsoleMessage(ILog log, Level level, object message)
            {
                Log(log, level, message);
                Console.WriteLine(message.ToString());
            }

            /// <summary>Log message to the logger and to the console</summary>
            /// <param name="log">log4net logger</param>
            /// <param name="level">Logging level</param>
            /// <param name="format">Log format</param>
            /// <param name="args">Log arguments</param>
            /// <remarks>We do not use console appender because if memory and performance issues so explicit console write is needed</remarks>
            /// <remarks>This method must not be abused and only used when really required!</remarks>
            public static void LogConsoleMessageFormat(ILog log, Level level, string format, params object[] args)
            {
                LogFormat(log, level, format, args);
                Console.WriteLine(format, args);
            }

            /// <summary>log4net wrapper to provide single point logging for different parameterized levels</summary>
            /// <param name="log">log4net logger</param>
            /// <param name="level">Logging level</param>
            /// <param name="format">Log format</param>
            /// <param name="args">Log arguments</param>
            public static void LogFormat(ILog log, Level level, string format, params object[] args)
            {
                if (level == Level.Debug)
                {
                    log.DebugFormat(format, args);
                }
                else if (level == Level.Info)
                {
                    log.InfoFormat(format, args);
                }
                else if (level == Level.Warn)
                {
                    log.WarnFormat(format, args);
                }
                else if (level == Level.Fatal)
                {
                    log.FatalFormat(format, args);
                }
                else
                {
                    log.ErrorFormat(format, args);
                }
            }

            #endregion
        }
    }
}