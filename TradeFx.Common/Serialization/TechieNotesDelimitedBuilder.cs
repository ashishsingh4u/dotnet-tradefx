//  ===================================================================================
//  <copyright file="TechieNotesDelimitedBuilder.cs" company="TechieNotes">
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
//     The TechieNotesDelimitedBuilder.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Diagnostics;
using System.Text;

using TradeFx.Common.Logging;
using TradeFx.Common.Serialization;

namespace TechieNotes.Common.Serialization
{
    /// <summary>
    ///     Enum to control the DateTime option
    /// </summary>
    public enum DateTimeOption
    {
        /// <summary>The date only.</summary>
        DateOnly, 

        /// <summary>The date and time.</summary>
        DateAndTime
    }

    /// <summary>The techie notes delimited builder.</summary>
    [DebuggerDisplay("{ToLog()}")]
    public class TechieNotesDelimitedBuilder : ILoggable, IDisposable
    {
        #region Constants

        /// <summary>The empty fields marker.</summary>
        private const char EmptyFieldsMarker = (char)4;

        /// <summary>The field delimiter.</summary>
        private const char FieldDelimiter = (char)2;

        /// <summary>The null marker.</summary>
        private const char NullMarker = (char)1;

        /// <summary>The object delimiter.</summary>
        private const char ObjectDelimiter = (char)3;

        #endregion

        #region Fields

        /// <summary>The _sb.</summary>
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>The _artifacts removed.</summary>
        private bool _artifactsRemoved;

        /// <summary>The _disposed.</summary>
        private volatile bool _disposed;

        /// <summary>The _empty field counter.</summary>
        private int _emptyFieldCounter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TechieNotesDelimitedBuilder" /> class.
        ///     Public constructor
        /// </summary>
        public TechieNotesDelimitedBuilder()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TechieNotesDelimitedBuilder"/> class. Special private constructor to make copy for inspection and logging</summary>
        /// <param name="builder"></param>
        private TechieNotesDelimitedBuilder(TechieNotesDelimitedBuilder builder)
        {
            this._sb = new StringBuilder(builder._sb.ToString());
            this._emptyFieldCounter = builder._emptyFieldCounter;
            this._artifactsRemoved = builder._artifactsRemoved;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is empty.</summary>
        public bool IsEmpty
        {
            get
            {
                return (this._sb.Length == 0 || this._sb.Length == 1 && this._sb[0] == NullMarker)
                       && this._emptyFieldCounter == 0;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Append(string value)
        {
            if (value == FieldDelimiter.ToString())
            {
                throw new ArgumentException("FieldDelimiter");
            }

            if (value == ObjectDelimiter.ToString())
            {
                throw new ArgumentException("ObjectDelimiter");
            }

            if (string.IsNullOrEmpty(value))
            {
                this._emptyFieldCounter++;
                return;
            }

            this.AppendEmptyFields();

            this._sb.Append(FieldDelimiter);
            this._sb.Append(value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Append(char value)
        {
            if (value == FieldDelimiter)
            {
                throw new ArgumentException("FieldDelimiter");
            }

            if (value == ObjectDelimiter)
            {
                throw new ArgumentException("ObjectDelimiter");
            }

            this._sb.Append(FieldDelimiter);
            this._sb.Append(value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(char? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(double value)
        {
            if (value == 0)
            {
                this.Append(string.Empty);
                return;
            }

            var strValue = GetStrValue(value);

            Append(strValue);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(double? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(decimal value)
        {
            if (value == 0)
            {
                this.Append(string.Empty);
                return;
            }

            var strValue = GetStrValue(value);

            Append(strValue);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(decimal? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTimeOption">The date time option.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Append(DateTime value, DateTimeOption dateTimeOption)
        {
            switch (dateTimeOption)
            {
                case DateTimeOption.DateOnly:
                    if (value == DateTime.MinValue)
                    {
                        this.Append(string.Empty);
                    }
                    else
                    {
                        var intValue = (int)(value.Date.ToOADate() - TechieNotesDelimitedFormatInfo.DateOnly.Rebase);
                        var strValue = intValue.ToString(
                            TechieNotesDelimitedFormatInfo.DateOnly.Format, 
                            TechieNotesDelimitedFormatInfo.DateOnly.Culture);
                        Append(strValue);
                    }

                    break;
                case DateTimeOption.DateAndTime:
                    if (value == DateTime.MinValue)
                    {
                        this.Append(string.Empty);
                    }
                    else
                    {
                        Append(
                            value.ToString(
                                TechieNotesDelimitedFormatInfo.DateAndTime.Format, 
                                TechieNotesDelimitedFormatInfo.DateAndTime.Culture));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("dateTimeOption");
            }
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTimeOption">The date time option.</param>
        public void Append(DateTime? value, DateTimeOption dateTimeOption)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value, dateTimeOption);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(int value)
        {
            if (value == 0)
            {
                this.Append(string.Empty);
                return;
            }

            var strValue = GetStrValue(value);

            Append(strValue);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(int? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(long value)
        {
            if (value == 0)
            {
                this.Append(string.Empty);
                return;
            }

            var strValue = GetStrValue(value);

            Append(strValue);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(long? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(bool value)
        {
            this.Append(value ? 1 : 0);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(bool? value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(value.Value);
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(Enum value)
        {
            if (AppendNullable(value))
            {
                return;
            }

            Append(Convert.ToInt32(value));
        }

        /// <summary>The append.</summary>
        /// <param name="value">The value.</param>
        public void Append(ITechieNotesDelimitedSerialize value)
        {
            Append(value.GetId());

            value.Serialize(this);
        }

        /// <summary>Serialize ITechieNotesDelimitedSerialize[]</summary>
        /// <param name="array"></param>
        public void Append<T>(T[] array) where T : ITechieNotesDelimitedSerialize
        {
            Append(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Append(array[i]);
            }
        }

        /// <summary>Serialize decimal[]</summary>
        /// <param name="array"></param>
        public void Append(decimal[] array)
        {
            Append(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Append(array[i]);
            }
        }

        /// <summary>Serialize double[]</summary>
        /// <param name="array"></param>
        public void Append(double[] array)
        {
            Append(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Append(array[i]);
            }
        }

        /// <summary>Serialize string[]</summary>
        /// <param name="array"></param>
        public void Append(string[] array)
        {
            Append(array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Append(array[i]);
            }
        }

        /// <summary>Serialize type ITechieNotesDelimitedSerialize[,]</summary>
        /// <param name="grid"></param>
        public void Append<T>(T[,] grid) where T : ITechieNotesDelimitedSerialize
        {
            Append(grid.GetLength(0));
            Append(grid.GetLength(1));

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Append(grid[i, j]);
                }
            }
        }

        /// <summary>Serialize type decimal[,]</summary>
        /// <param name="grid"></param>
        public void Append(decimal[,] grid)
        {
            Append(grid.GetLength(0));
            Append(grid.GetLength(1));

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Append(grid[i, j]);
                }
            }
        }

        /// <summary>Serialize type double[,]</summary>
        /// <param name="grid"></param>
        public void Append(double[,] grid)
        {
            Append(grid.GetLength(0));
            Append(grid.GetLength(1));

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Append(grid[i, j]);
                }
            }
        }

        /// <summary>Serialize type string[,]</summary>
        /// <param name="grid"></param>
        public void Append(string[,] grid)
        {
            Append(grid.GetLength(0));
            Append(grid.GetLength(1));

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Append(grid[i, j]);
                }
            }
        }

        /// <summary>The append object or null.</summary>
        /// <param name="value">The value.</param>
        public void AppendObjectOrNull(ITechieNotesDelimitedSerialize value)
        {
            var isNotNull = !ReferenceEquals(value, null);
            Append(isNotNull);
            if (isNotNull)
            {
                Append(value);
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>The to array.</summary>
        /// <returns>The System.Byte[].</returns>
        public byte[] ToArray()
        {
            return Encoding.UTF8.GetBytes(this.ToString());
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            // Make a copy to avoid interfering with internal state
            var builder = new TechieNotesDelimitedBuilder(this);

            return PrettyPrint(builder);
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString()
        {
            this.AppendEmptyFields();

            if (!this._artifactsRemoved)
            {
                this._artifactsRemoved = true;

                if (this._sb.Length > 0)
                {
                    // Remove leading field delimiter
                    this._sb.Remove(0, 1);

                    // Remove trailing object delimiter
                    if (this._sb.Length > 0 && this._sb[this._sb.Length - 1] == ObjectDelimiter)
                    {
                        this._sb.Remove(this._sb.Length - 1, 1);
                    }
                }
                else
                {
                    this._sb.Append(NullMarker);
                }
            }

            return this._sb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Add the object end to the fields list
        /// </summary>
        protected void AddObjectEnd()
        {
            this.AppendEmptyFields();

            // We don't have to add field delimiter here - we will restore the sequence in the parser as needed
            this._sb.Append(ObjectDelimiter);
        }

        /// <summary>The get str value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        private static string GetStrValue(long value)
        {
            var strValue = value.ToString(
                TechieNotesDelimitedFormatInfo.Integer.Format, TechieNotesDelimitedFormatInfo.Integer.Culture);

            var trailingZeroCount = GetTrailingZeroCount(strValue);

            if (trailingZeroCount > 1)
            {
                var replaceChar =
                    Convert.ToChar(TechieNotesDelimitedFormatInfo.Integer.ReplaceCharBase + trailingZeroCount);
                strValue = strValue.Substring(0, strValue.Length - trailingZeroCount) + replaceChar;
            }

            return strValue;
        }

        /// <summary>The get str value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        private static string GetStrValue(double value)
        {
            var strValue = value.ToString(
                TechieNotesDelimitedFormatInfo.Double.Format, TechieNotesDelimitedFormatInfo.Double.Culture);

            if (value != double.MinValue && value != double.MaxValue && !double.IsNaN(value)
                && !double.IsInfinity(value) && value > 1)
            {
                strValue = OptimizeStrValue((long)value, strValue);
            }

            return strValue;
        }

        /// <summary>The get str value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.String.</returns>
        private static string GetStrValue(decimal value)
        {
            var strValue = value.ToString(
                TechieNotesDelimitedFormatInfo.Decimal.Format, TechieNotesDelimitedFormatInfo.Decimal.Culture);

            if (value != decimal.MinValue && value != decimal.MaxValue && value > 1)
            {
                strValue = OptimizeStrValue((long)value, strValue);
            }

            return strValue;
        }

        /// <summary>The get trailing zero count.</summary>
        /// <param name="strValue">The str value.</param>
        /// <returns>The System.Int32.</returns>
        private static int GetTrailingZeroCount(string strValue)
        {
            var trailingZeroCount = 0;
            for (var i = strValue.Length - 1; i >= 0; i--)
            {
                if (strValue[i] == TechieNotesDelimitedFormatInfo.Integer.ZeroChar)
                {
                    trailingZeroCount++;
                }
                else
                {
                    break;
                }
            }

            return trailingZeroCount;
        }

        /// <summary>The optimize str value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="strValue">The str value.</param>
        /// <returns>The System.String.</returns>
        private static string OptimizeStrValue(long value, string strValue)
        {
            var decimaPointIndex =
                strValue.IndexOf(TechieNotesDelimitedFormatInfo.Double.Culture.NumberFormat.NumberDecimalSeparator);
            if (decimaPointIndex < 0)
            {
                strValue = GetStrValue(value);
            }
            else if (decimaPointIndex > 1)
            {
                strValue = GetStrValue(value) + strValue.Substring(decimaPointIndex);
            }

            return strValue;
        }

        /// <summary>The pretty print.</summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The System.String.</returns>
        private static string PrettyPrint(TechieNotesDelimitedBuilder builder)
        {
            if (builder.IsEmpty)
            {
                return "*Empty*";
            }

            var sb = new StringBuilder(builder.ToString());

            // Replace unreadable characters with something nice
            sb.Replace(FieldDelimiter, '|').Replace(ObjectDelimiter, '#').Replace(EmptyFieldsMarker, '~');

            return string.Format("Data=[{0}], Length=[{1}]", sb, builder.ToArray().Length);
        }

        /// <summary>The append empty fields.</summary>
        private void AppendEmptyFields()
        {
            if (this._emptyFieldCounter <= 0)
            {
                return;
            }

            if (this._emptyFieldCounter < 2)
            {
                this._sb.Append(FieldDelimiter);
                this._emptyFieldCounter = 0;
                return;
            }

            this._sb.Append(FieldDelimiter);
            this._sb.Append(EmptyFieldsMarker);
            this._sb.Append(
                this._emptyFieldCounter.ToString(
                    TechieNotesDelimitedFormatInfo.Integer.Format, TechieNotesDelimitedFormatInfo.Integer.Culture));

            this._emptyFieldCounter = 0;
        }

        /// <summary>The append nullable.</summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The System.Boolean.</returns>
        private bool AppendNullable<T>(T? value) where T : struct
        {
            if (!value.HasValue)
            {
                this.Append(TechieNotesDelimitedFormatInfo.NullSymbol);
                return true;
            }

            return false;
        }

        /// <summary>The append nullable.</summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The System.Boolean.</returns>
        private bool AppendNullable<T>(T value) where T : class
        {
            if (ReferenceEquals(value, null))
            {
                this.Append(TechieNotesDelimitedFormatInfo.NullSymbol);
                return true;
            }

            return false;
        }

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this._sb.Length = 0;
                    this._emptyFieldCounter = 0;
                    this._artifactsRemoved = false;
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            this._disposed = true;
        }

        #endregion

        /// <summary>
        ///     Class to assure the objects are ended with ObjectDelimiter
        /// </summary>
        public class Terminator : IDisposable
        {
            #region Fields

            /// <summary>The _builder.</summary>
            private readonly TechieNotesDelimitedBuilder _builder;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="Terminator"/> class.</summary>
            /// <param name="builder">The builder.</param>
            public Terminator(TechieNotesDelimitedBuilder builder)
            {
                this._builder = builder;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                // We will add the object end every time Dispose is called
                this._builder.AddObjectEnd();
            }

            #endregion
        }
    }
}