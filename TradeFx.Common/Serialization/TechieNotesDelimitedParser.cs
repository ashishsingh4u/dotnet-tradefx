//  ===================================================================================
//  <copyright file="TechieNotesDelimitedParser.cs" company="TechieNotes">
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
//     The TechieNotesDelimitedParser.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Text;

using TechieNotes.Common.Creation;

using TradeFx.Common.Creation;
using TradeFx.Common.Logging;

namespace TradeFx.Common.Serialization
{
    /// <summary>
    ///     Parse TechieNotes delimited format
    /// </summary>
    public class TechieNotesDelimitedParser : ILoggable, IDisposable
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
        private readonly StringBuilder _sb;

        /// <summary>The _current.</summary>
        private int _current;

        /// <summary>The _data.</summary>
        private byte[] _data;

        /// <summary>The _disposed.</summary>
        private volatile bool _disposed;

        /// <summary>The _empty field counter.</summary>
        private int _emptyFieldCounter;

        /// <summary>The _fields.</summary>
        private string[] _fields;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TechieNotesDelimitedParser"/> class.</summary>
        /// <param name="data">The data.</param>
        public TechieNotesDelimitedParser(byte[] data)
        {
            this._data = data;
            this._sb = new StringBuilder(Encoding.UTF8.GetString(data));

            if (!this.IsNull())
            {
                // Restore the object delimiter field delimiter
                this._sb.Replace(ObjectDelimiter.ToString(), FieldDelimiter + ObjectDelimiter.ToString());

                // Add leading field delimiter
                this._sb.Insert(0, FieldDelimiter);

                // Add trailing object delimiter.
                this._sb.Append(FieldDelimiter);
                this._sb.Append(ObjectDelimiter);
            }

            this._fields = this._sb.ToString().Split(FieldDelimiter);
            this._current = 1;
        }

        /// <summary>Initializes a new instance of the <see cref="TechieNotesDelimitedParser"/> class. Special private constructor to make copy for inspection and logging</summary>
        /// <param name="parser"></param>
        private TechieNotesDelimitedParser(TechieNotesDelimitedParser parser)
        {
            this._data = new byte[parser._data.Length];
            Array.Copy(parser._data, this._data, parser._data.Length);

            this._fields = new string[parser._fields.Length];
            Array.Copy(parser._fields, this._fields, parser._fields.Length);

            this._emptyFieldCounter = parser._emptyFieldCounter;
            this._current = parser._current;
            this._sb = new StringBuilder(parser._sb.ToString());
        }

        #endregion

        #region Properties

        /// <summary>Gets the current index.</summary>
        private int CurrentIndex
        {
            get
            {
                return this._current + (this._emptyFieldCounter > 0 ? this._emptyFieldCounter : 0);
            }
        }

        /// <summary>Gets the current item.</summary>
        private string CurrentItem
        {
            get
            {
                if (this.IsEnd())
                {
                    return "<End of Data>";
                }

                if (this._emptyFieldCounter > 0)
                {
                    return string.Empty;
                }

                return this._fields[this._current];
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>The get bool.</summary>
        /// <returns>The System.Boolean.</returns>
        public bool GetBool()
        {
            return this.GetInt() != 0;
        }

        /// <summary>The get char.</summary>
        /// <returns>The System.Char.</returns>
        public char GetChar()
        {
            return Convert.ToChar(this.GetString());
        }

        /// <summary>The get date.</summary>
        /// <returns>The System.DateTime.</returns>
        public DateTime GetDate()
        {
            var data = this.GetString();

            return ConvertDataToDate(data);
        }

        /// <summary>The get date time.</summary>
        /// <returns>The System.DateTime.</returns>
        public DateTime GetDateTime()
        {
            var data = this.GetString();

            return ConvertDataToDateTime(data);
        }

        /// <summary>The get decimal.</summary>
        /// <returns>The System.Decimal.</returns>
        public decimal GetDecimal()
        {
            var data = this.GetString();

            return ConvertDataToDecimal(data);
        }

        /// <summary>The get decimal 2 d array.</summary>
        /// <returns>The System.Decimal[,].</returns>
        public decimal[,] GetDecimal2DArray()
        {
            var numRows = this.GetInt();
            var numCols = this.GetInt();
            var grid = new decimal[numRows, numCols];

            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numCols; j++)
                {
                    grid[i, j] = this.GetDecimal();
                }
            }

            return grid;
        }

        /// <summary>The get decimal array.</summary>
        /// <returns>The System.Decimal[].</returns>
        public decimal[] GetDecimalArray()
        {
            var numEntries = this.GetInt();
            var array = new decimal[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                array[i] = this.GetDecimal();
            }

            return array;
        }

        /// <summary>The get double.</summary>
        /// <returns>The System.Double.</returns>
        public double GetDouble()
        {
            var data = this.GetString();

            return ConvertDataToDouble(data);
        }

        /// <summary>The get double 2 d array.</summary>
        /// <returns>The System.Double[,].</returns>
        public double[,] GetDouble2DArray()
        {
            var numRows = this.GetInt();
            var numCols = this.GetInt();
            var grid = new double[numRows, numCols];

            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numCols; j++)
                {
                    grid[i, j] = this.GetDouble();
                }
            }

            return grid;
        }

        /// <summary>The get double array.</summary>
        /// <returns>The System.Double[].</returns>
        public double[] GetDoubleArray()
        {
            var numEntries = this.GetInt();
            var array = new double[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                array[i] = this.GetDouble();
            }

            return array;
        }

        /// <summary>The get enum.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T.</returns>
        public T GetEnum<T>()
        {
            return (T)(object)this.GetInt();
        }

        /// <summary>The get int.</summary>
        /// <returns>The System.Int32.</returns>
        public int GetInt()
        {
            var data = this.GetString();

            return ConvertDataToInt(data);
        }

        /// <summary>The get long.</summary>
        /// <returns>The System.Int64.</returns>
        public long GetLong()
        {
            var data = this.GetString();

            return ConvertDataToLong(data);
        }

        /// <summary>The get nullable bool.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Boolean].</returns>
        public bool? GetNullableBool()
        {
            var data = this.GetNullableInt();

            if (!data.HasValue)
            {
                return null;
            }

            return data.Value != 0;
        }

        /// <summary>The get nullable char.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Char].</returns>
        public char? GetNullableChar()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return Convert.ToChar(data);
        }

        /// <summary>The get nullable date.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.DateTime].</returns>
        public DateTime? GetNullableDate()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToDate(data);
        }

        /// <summary>The get nullable date time.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.DateTime].</returns>
        public DateTime? GetNullableDateTime()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToDateTime(data);
        }

        /// <summary>The get nullable decimal.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Decimal].</returns>
        public decimal? GetNullableDecimal()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToDecimal(data);
        }

        /// <summary>The get nullable double.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Double].</returns>
        public double? GetNullableDouble()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToDouble(data);
        }

        /// <summary>The get nullable enum.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The System.Nullable`1[T -&gt; T].</returns>
        public T? GetNullableEnum<T>() where T : struct
        {
            var data = this.GetNullableInt();

            if (!data.HasValue)
            {
                return null;
            }

            return (T)(object)data.Value;
        }

        /// <summary>The get nullable int.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Int32].</returns>
        public int? GetNullableInt()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToInt(data);
        }

        /// <summary>The get nullable long.</summary>
        /// <returns>The System.Nullable`1[T -&gt; System.Int64].</returns>
        public long? GetNullableLong()
        {
            var data = this.GetString();

            if (data == TechieNotesDelimitedFormatInfo.NullSymbol)
            {
                return null;
            }

            return ConvertDataToLong(data);
        }

        /// <summary>The get object.</summary>
        /// <returns>The System.Object.</returns>
        public object GetObject()
        {
            var objectId = this.GetEnum<ObjectId>();
            var obj = (ITechieNotesDelimitedSerialize)ObjectFactory.Instance.CreateObject(objectId);
            obj.Deserialize(this);

            return obj;
        }

        /// <summary>The get object.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T.</returns>
        public T GetObject<T>()
        {
            return (T)this.GetObject();
        }

        /// <summary>The get object 2 d array.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T[,].</returns>
        public T[,] GetObject2DArray<T>()
        {
            var numRows = this.GetInt();
            var numCols = this.GetInt();
            var grid = new T[numRows, numCols];

            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numCols; j++)
                {
                    grid[i, j] = this.GetObject<T>();
                }
            }

            return grid;
        }

        /// <summary>The get object array.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T[].</returns>
        public T[] GetObjectArray<T>()
        {
            var numEntries = this.GetInt();
            var array = new T[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                array[i] = this.GetObject<T>();
            }

            return array;
        }

        /// <summary>The get object or null.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T.</returns>
        public T GetObjectOrNull<T>()
        {
            var hasObject = this.GetBool();
            if (hasObject)
            {
                return this.GetObject<T>();
            }

            return default(T);
        }

        /// <summary>The get string.</summary>
        /// <returns>The System.String.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string GetString()
        {
            if (this.IsEnd())
            {
                throw new InvalidOperationException("End of data reached");
            }

            if (this._emptyFieldCounter > 0)
            {
                return this.GetEmptyField();
            }

            var data = this._fields[this._current++];

            if (!string.IsNullOrEmpty(data) && data[0] == EmptyFieldsMarker)
            {
                this._emptyFieldCounter = int.Parse(
                    data.Trim(EmptyFieldsMarker), 
                    TechieNotesDelimitedFormatInfo.Integer.NumStyles, 
                    TechieNotesDelimitedFormatInfo.Integer.Culture);

                if (this._emptyFieldCounter > 0)
                {
                    return this.GetEmptyField();
                }
            }

            return data;
        }

        /// <summary>The get string 2 d array.</summary>
        /// <returns>The System.String[,].</returns>
        public string[,] GetString2DArray()
        {
            var numRows = this.GetInt();
            var numCols = this.GetInt();
            var grid = new string[numRows, numCols];

            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numCols; j++)
                {
                    grid[i, j] = this.GetString();
                }
            }

            return grid;
        }

        /// <summary>The get string array.</summary>
        /// <returns>The System.String[].</returns>
        public string[] GetStringArray()
        {
            var numEntries = this.GetInt();
            var array = new string[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                array[i] = this.GetString();
            }

            return array;
        }

        /// <summary>
        ///     Check if there are any more items
        /// </summary>
        /// <returns>true if end reached, false otherwise</returns>
        public bool IsEnd()
        {
            return this._fields == null || (this._current >= this._fields.Length && this._emptyFieldCounter <= 0);
        }

        /// <summary>The to log.</summary>
        /// <returns>The System.String.</returns>
        public string ToLog()
        {
            // Make a copy to avoid interfering with internal state
            var parser = new TechieNotesDelimitedParser(this);

            return PrettyPrint(parser);
        }

        /// <summary>The to string.</summary>
        /// <returns>The System.String.</returns>
        public override string ToString()
        {
            // Something nice for the debugger
            return string.Format("Current={0}; Index={1}", this.CurrentItem, !this.IsEnd() ? this.CurrentIndex : -1);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Move the position to the end of object
        /// </summary>
        protected void SeekToObjectEnd()
        {
            while (!this.IsEnd()
                   && (this._fields[this._current].Length == 0 || this._fields[this._current++][0] != ObjectDelimiter))
            {
                if (this._fields[this._current].Length == 0)
                {
                    this._current++;
                }
            }
        }

        /// <summary>The convert data to date.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.DateTime.</returns>
        private static DateTime ConvertDataToDate(string data)
        {
            // We transmit zero as empty to save space
            if (string.IsNullOrEmpty(data))
            {
                return new DateTime();
            }

            return
                DateTime.FromOADate(
                    int.Parse(
                        data, 
                        TechieNotesDelimitedFormatInfo.DateOnly.NumStyles, 
                        TechieNotesDelimitedFormatInfo.DateOnly.Culture)
                    + TechieNotesDelimitedFormatInfo.DateOnly.Rebase);
        }

        /// <summary>The convert data to date time.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.DateTime.</returns>
        private static DateTime ConvertDataToDateTime(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return new DateTime();
            }

            return DateTime.ParseExact(
                data, 
                TechieNotesDelimitedFormatInfo.DateAndTime.Format, 
                TechieNotesDelimitedFormatInfo.DateAndTime.Culture);
        }

        /// <summary>The convert data to decimal.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.Decimal.</returns>
        private static decimal ConvertDataToDecimal(string data)
        {
            // We transmit zero as empty to save space
            if (string.IsNullOrEmpty(data))
            {
                return 0.0M;
            }

            var decimaPointIndex =
                data.IndexOf(TechieNotesDelimitedFormatInfo.Decimal.Culture.NumberFormat.NumberDecimalSeparator);
            if (decimaPointIndex < 0)
            {
                data = TranslateIntStrData(data);
            }
            else
            {
                data = TranslateIntStrData(data.Substring(0, decimaPointIndex)) + data.Substring(decimaPointIndex);
            }

            return decimal.Parse(data, TechieNotesDelimitedFormatInfo.Decimal.Culture);
        }

        /// <summary>The convert data to double.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.Double.</returns>
        private static double ConvertDataToDouble(string data)
        {
            // We transmit zero as empty to save space
            if (string.IsNullOrEmpty(data))
            {
                return 0.0;
            }

            if (data == TechieNotesDelimitedFormatInfo.Double.Culture.NumberFormat.NaNSymbol)
            {
                return double.NaN;
            }

            if (data == TechieNotesDelimitedFormatInfo.Double.Culture.NumberFormat.PositiveInfinitySymbol)
            {
                return double.PositiveInfinity;
            }

            if (data == TechieNotesDelimitedFormatInfo.Double.Culture.NumberFormat.NegativeInfinitySymbol)
            {
                return double.NegativeInfinity;
            }

            var decimaPointIndex =
                data.IndexOf(TechieNotesDelimitedFormatInfo.Double.Culture.NumberFormat.NumberDecimalSeparator);
            if (decimaPointIndex < 0)
            {
                data = TranslateIntStrData(data);
            }
            else
            {
                data = TranslateIntStrData(data.Substring(0, decimaPointIndex)) + data.Substring(decimaPointIndex);
            }

            return double.Parse(data, TechieNotesDelimitedFormatInfo.Double.Culture);
        }

        /// <summary>The convert data to int.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.Int32.</returns>
        private static int ConvertDataToInt(string data)
        {
            // We transmit zero as empty to save space
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }

            data = TranslateIntStrData(data);

            return int.Parse(
                data, TechieNotesDelimitedFormatInfo.Integer.NumStyles, TechieNotesDelimitedFormatInfo.Integer.Culture);
        }

        /// <summary>The convert data to long.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.Int64.</returns>
        private static long ConvertDataToLong(string data)
        {
            // We transmit zero as empty to save space
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }

            data = TranslateIntStrData(data);

            return long.Parse(
                data, TechieNotesDelimitedFormatInfo.Integer.NumStyles, TechieNotesDelimitedFormatInfo.Integer.Culture);
        }

        /// <summary>The pretty print.</summary>
        /// <param name="parser">The parser.</param>
        /// <returns>The System.String.</returns>
        private static string PrettyPrint(TechieNotesDelimitedParser parser)
        {
            if (parser.IsNull())
            {
                return "*Empty*";
            }

            var sb = new StringBuilder(Encoding.UTF8.GetString(parser._data));

            // Replace unreadable characters with something nice
            sb.Replace(FieldDelimiter, '|').Replace(ObjectDelimiter, '#').Replace(EmptyFieldsMarker, '~');

            return string.Format("Data=[{0}], Length=[{1}]", sb, sb.Length);
        }

        /// <summary>The translate int str data.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.String.</returns>
        private static string TranslateIntStrData(string data)
        {
            var lastChar = data[data.Length - 1];
            if (!char.IsDigit(lastChar))
            {
                data = data.Replace(
                    lastChar.ToString(), 
                    new string(
                        TechieNotesDelimitedFormatInfo.Integer.ZeroChar, 
                        lastChar - TechieNotesDelimitedFormatInfo.Integer.ReplaceCharBase));
            }

            return data;
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
                    this._fields = null;
                    this._current = 0;
                    this._emptyFieldCounter = 0;

                    this._data = null;
                    this._sb.Length = 0;
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
            }

            this._disposed = true;
        }

        /// <summary>The get empty field.</summary>
        /// <returns>The System.String.</returns>
        private string GetEmptyField()
        {
            this._emptyFieldCounter--;
            return string.Empty;
        }

        /// <summary>Test whether we are dealing with the null object</summary>
        /// <returns>The System.Boolean.</returns>
        private bool IsNull()
        {
            return this._sb.Length > 0 && this._sb[0] == NullMarker;
        }

        #endregion

        /// <summary>
        ///     Class to assure the objects are parsed until ObjectDelimiter to enable backward-compatibility
        /// </summary>
        public class Terminator : IDisposable
        {
            #region Fields

            /// <summary>The _parser.</summary>
            private readonly TechieNotesDelimitedParser _parser;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="Terminator"/> class.</summary>
            /// <param name="parser">The parser.</param>
            public Terminator(TechieNotesDelimitedParser parser)
            {
                this._parser = parser;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                // We will add the object end every time Dispose is called
                this._parser.SeekToObjectEnd();
            }

            #endregion
        }
    }
}