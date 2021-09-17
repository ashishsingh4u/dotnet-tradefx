//  ===================================================================================
//  <copyright file="TechieNotesDelimitedFormatInfo.cs" company="TechieNotes">
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
//     The TechieNotesDelimitedFormatInfo.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Globalization;

namespace TradeFx.Common.Serialization
{
    /// <summary>
    ///     Culture class to support TechieNotesDelimited Format
    /// </summary>
    public static class TechieNotesDelimitedFormatInfo
    {
        #region Constants

        /// <summary>The null symbol.</summary>
        public const string NullSymbol = "?";

        #endregion

        /// <summary>The date and time.</summary>
        public static class DateAndTime
        {
            #region Constants

            /// <summary>
            ///     DateAndTime option format
            /// </summary>
            public const string Format = "F";

            /// <summary>
            ///     Compact date and time format
            /// </summary>
            private const string FullDateTimePattern = "ddMMyyyyHHmmssfff";

            #endregion

            #region Static Fields

            /// <summary>
            ///     Invariant culture but change the full date time pattern to more compact
            /// </summary>
            public static readonly CultureInfo Culture = new CultureInfo(string.Empty);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes static members of the <see cref="DateAndTime" /> class.
            /// </summary>
            static DateAndTime()
            {
                // Set more compact full DateTime pattern
                Culture.DateTimeFormat.FullDateTimePattern = FullDateTimePattern;
            }

            #endregion
        }

        /// <summary>The date only.</summary>
        public static class DateOnly
        {
            #region Constants

            /// <summary>
            ///     Format of date
            /// </summary>
            public const string Format = "X";

            /// <summary>
            ///     Format of date
            /// </summary>
            public const NumberStyles NumStyles = NumberStyles.HexNumber;

            #endregion

            #region Static Fields

            /// <summary>The culture.</summary>
            public static readonly CultureInfo Culture = new CultureInfo(string.Empty);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes static members of the <see cref="DateOnly" /> class.
            /// </summary>
            static DateOnly()
            {
                // Set the date base to shorted the data
                Rebase = new DateTime(2009, 01, 01).Date.ToOADate(); // 39814.0
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     DateTimeOnly option re-base value
            /// </summary>
            public static double Rebase { get; private set; }

            #endregion
        }

        /// <summary>The decimal.</summary>
        public static class Decimal
        {
            #region Constants

            /// <summary>
            ///     Format of decimal
            /// </summary>
            public const string Format = "g";

            #endregion

            #region Static Fields

            /// <summary>The culture.</summary>
            public static readonly CultureInfo Culture = new CultureInfo(string.Empty);

            #endregion
        }

        /// <summary>The double.</summary>
        public static class Double
        {
            #region Constants

            /// <summary>
            ///     Format of double
            /// </summary>
            public const string Format = "r";

            #endregion

            #region Static Fields

            /// <summary>
            ///     We clone the invariant culture, but change some aspects to reduce size in ctor
            ///     http://msdn.microsoft.com/en-us/library/4c5zdc6a(VS.80).aspx
            /// </summary>
            public static readonly CultureInfo Culture = new CultureInfo(string.Empty);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes static members of the <see cref="Double" /> class.
            /// </summary>
            static Double()
            {
                // Make a double.NaN symbol smaller
                Culture.NumberFormat.NaNSymbol = "_";
                Culture.NumberFormat.PositiveInfinitySymbol = ">";
                Culture.NumberFormat.NegativeInfinitySymbol = "<";
            }

            #endregion
        }

        /// <summary>The integer.</summary>
        public static class Integer
        {
            #region Constants

            /// <summary>
            ///     Format of int
            /// </summary>
            public const string Format = "d";

            /// <summary>
            ///     Format of int
            /// </summary>
            public const NumberStyles NumStyles = NumberStyles.Integer;

            /// <summary>
            ///     Base character used to replace trailing zeros
            /// </summary>
            public const char ReplaceCharBase = 'A';

            /// <summary>
            ///     Base character used to replace trailing zeros
            /// </summary>
            public const char ZeroChar = '0';

            #endregion

            #region Static Fields

            /// <summary>The culture.</summary>
            public static readonly CultureInfo Culture = new CultureInfo(string.Empty);

            #endregion
        }
    }
}