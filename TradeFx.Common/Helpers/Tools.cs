//  ===================================================================================
//  <copyright file="Tools.cs" company="TechieNotes">
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
//  <date>11-03-2013</date>
//  <summary>
//     The Tools.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TradeFx.Common.Helpers
{
    /// <summary>
    ///     Various utility functions
    /// </summary>
    public abstract class Tools
    {
        #region Constants

        /// <summary>The c str date format.</summary>
        public const string CStrDateFormat = "dd MMM yyyy"; /*!< Standard date format */

        /// <summary>The c str date time format.</summary>
        public const string CStrDateTimeFormat = CStrDateFormat + " HH:mm:ss:fff";

        /// <summary>The c str hh time format.</summary>
        public const string CStrHhTimeFormat = "HH:mm:ss"; /*!< Standard time format, 24 hours, no milliseconds */

        /// <summary>The c str mm mdd date format.</summary>
        public const string CStrMmMddDateFormat = "MMM dd"; /*!< Standard date format without year */

        /// <summary>The c str parse date format.</summary>
        public const string CStrParseDateFormat = "d MMM yyyy";

        /// <summary>The c str parse date time format.</summary>
        public const string CStrParseDateTimeFormat = "d MMM yyyy H:mm:ss:fff";

        /// <summary>The c chr billion symbol.</summary>
        private const char CChrBillionSymbol = 'B';

        /// <summary>The c chr billion symbol 1.</summary>
        private const char CChrBillionSymbol1 = 'Y';

        /// <summary>The c chr kilo symbol.</summary>
        private const char CChrKiloSymbol = 'K';

        /// <summary>The c chr million symbol.</summary>
        private const char CChrMillionSymbol = 'M';

        /// <summary>The c chr trillion symbol.</summary>
        private const char CChrTrillionSymbol = 'T';

        /// <summary>The c dbl billion factor.</summary>
        private const double CDblBillionFactor = 1000000000.00;

        /// <summary>The c dbl kilo factor.</summary>
        private const double CDblKiloFactor = 1000.00;

        /// <summary>The c dbl million factor.</summary>
        private const double CDblMillionFactor = 1000000.00;

        /// <summary>The c dbl trillion factor.</summary>
        private const double CDblTrillionFactor = 1000.00;

        /// <summary>The crypto sequence.</summary>
        private const string CryptoSequence = "POIUYTREWQASDFGHJKLMNBVCXZ";

        /// <summary>The full digit sequence.</summary>
        private const string FullDigitSequence = "FDEBCA9786452310";

        #endregion

        #region Static Fields

        /// <summary>The _execution mode.</summary>
        private static ExecutionMode _executionMode = ExecutionMode.Unknown;

        #endregion

        #region Enums

        /// <summary>The execution mode.</summary>
        private enum ExecutionMode
        {
            /// <summary>The unknown.</summary>
            Unknown = 0, /*!< Not yet determined */

            /// <summary>The design.</summary>
            Design, /*!< Design mode */

            /// <summary>The runtime.</summary>
            Runtime, /*!< Runtime */
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The bit blt.</summary>
        /// <param name="hdcDest">The hdc dest.</param>
        /// <param name="nXDest">The n x dest.</param>
        /// <param name="nYDest">The n y dest.</param>
        /// <param name="nWidth">The n width.</param>
        /// <param name="nHeight">The n height.</param>
        /// <param name="hdcSrc">The hdc src.</param>
        /// <param name="nXSrc">The n x src.</param>
        /// <param name="nYSrc">The n y src.</param>
        /// <param name="dwRop">The dw rop.</param>
        /// <returns>The System.Int64.</returns>
        [DllImport("gdi32.dll")]
        public static extern long BitBlt(
            IntPtr hdcDest, 
            int nXDest, 
            int nYDest, 
            int nWidth, 
            int nHeight, 
            IntPtr hdcSrc, 
            int nXSrc, 
            int nYSrc, 
            int dwRop);

        /// <summary>The compare number strings.</summary>
        /// <param name="strNumber1">The str number 1.</param>
        /// <param name="strNumber2">The str number 2.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool CompareNumberStrings(string strNumber1, string strNumber2)
        {
            double dblNumber1;
            double dblNumber2;

            var strError = string.Empty;

            if (!ConvertStringToDouble(strNumber1, out dblNumber1, ref strError))
            {
                Debug.Assert(false, strError);
            }

            if (!ConvertStringToDouble(strNumber2, out dblNumber2, ref strError))
            {
                Debug.Assert(false, strError);
            }

            return dblNumber1 != dblNumber2;
        }

        /// <summary>The compare properties.</summary>
        /// <param name="objExpected">The obj expected.</param>
        /// <param name="objActual">The obj actual.</param>
        /// <param name="strError">The str error.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool CompareProperties(object objExpected, object objActual, ref string strError)
        {
            return CompareProperties(objExpected, objActual, null, ref strError);
        }

        /*!
			Compare the public (non-indexed) properties. The List is based on 
			typeof objExpected objects. For that reason type of objActual should be the same
			as objExpected or one of its derived classes
			\param objExpected Object with expected values of the properties
			\param objActual Object with actual values
			\param objIgnoreProperties   List of properties to ignore when comparing
			\param strError Return error message
			\return true if objects are equal, false otherwise
			\note If one object is null then difference is reported, if both objects are null then no difference is reported
		*/

        /// <summary>The compare properties.</summary>
        /// <param name="objExpected">The obj expected.</param>
        /// <param name="objActual">The obj actual.</param>
        /// <param name="objIgnoreProperties">The obj ignore properties.</param>
        /// <param name="strError">The str error.</param>
        /// <returns>The System.Boolean.</returns>
        /// <exception cref="Exception"></exception>
        public static bool CompareProperties(
            object objExpected, object objActual, List<string> objIgnoreProperties, ref string strError)
        {
            var blnRes = false;

            try
            {
                if (objExpected == null || objActual == null)
                {
                    if (objExpected != null && objActual == null)
                    {
                        throw new Exception("Actual object is null");
                    }

                    if (objExpected == null && objActual != null)
                    {
                        throw new Exception("Expected object is null");
                    }

                    // Both are null - considered same
                }
                else
                {
                    // Check the property list
                    if (objExpected.GetType() == typeof(Dictionary<string, string>)
                        && objActual.GetType() == typeof(Dictionary<string, string>))
                    {
                        var objExpectedList = objExpected as Dictionary<string, string>;
                        var objActualList = objActual as Dictionary<string, string>;

                        foreach (var strItem in objExpectedList.Keys)
                        {
                            string strActualValue;
                            if (!objActualList.TryGetValue(strItem, out strActualValue))
                            {
                                throw new Exception(
                                    "Expected item " + strItem + " not found in the actual property list");
                            }

                            if (objExpectedList[strItem] != strActualValue)
                            {
                                throw new Exception(strItem + " property list value is different");
                            }
                        }

                        foreach (var strItem in objActualList.Keys)
                        {
                            if (!objExpectedList.ContainsKey(strItem))
                            {
                                throw new Exception(
                                    "Actual item " + strItem + " not found in the expected property list");
                            }
                        }
                    }
                    else
                    {
                        // All public properties
                        var objProperties = objExpected.GetType().GetProperties();

                        foreach (var objProperty in objProperties)
                        {
                            if (objIgnoreProperties == null || !objIgnoreProperties.Contains(objProperty.Name))
                            {
                                try
                                {
                                    if (objProperty.GetValue(objExpected, null) != null
                                        || objProperty.GetValue(objActual, null) != null)
                                    {
                                        if (
                                            !objProperty.GetValue(objExpected, null)
                                                        .Equals(objProperty.GetValue(objActual, null)))
                                        {
                                            throw new Exception(objProperty.Name + " value is different");
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    // Nothing to do - sometimes properties are just not implemented									
                                }
                            }
                        }
                    }
                }

                blnRes = true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            return blnRes;
        }

        /// <summary>Compresses an input string using the GZipStream class</summary>
        /// <param name="decompressed">the string to be compressed</param>
        /// <returns>String is the compressed string</returns>
        public static string Compress(string decompressed)
        {
            // Convert unicode string to byte array for processing
            var bufferInput = Encoding.Unicode.GetBytes(decompressed);

            // Remember size of input string, this is required for decompression
            var length = bufferInput.Length;

            // Create memory stream
            var memoryStream = new MemoryStream();

            // Create compression stream for above memory stream
            var memoryStreamCompressed = new GZipStream(memoryStream, CompressionMode.Compress, true);

            // write bytes to compression stream
            memoryStreamCompressed.Write(bufferInput, 0, bufferInput.Length);

            // memoryStreamCompressed stream must be closed before buffer can be read
            memoryStreamCompressed.Close();

            // Create output buffer
            var bufferOutput = new byte[memoryStream.Length];

            // Read compressed data from memory stream
            bufferOutput = memoryStream.ToArray();

            // Store compressed buffer in a string, prefix string with original size and a # as a compression indicator
            // We must use Base64 encoding to prevent non-printable characters confusing the database
            var output = length.ToString() + "#" + Convert.ToBase64String(bufferOutput);

            // Close memory stream
            memoryStream.Close();

            return output;
        }

        /// <summary>Converts the date time to an ISO8601 literal that is always safe to pass into SQL Server stored proecure
        ///     datetime parameter types. This works regardless of SET DATEFORMAT or SET LANGUAGE settings.</summary>
        /// <param name="dateTime">The date time to convert.</param>
        /// <returns>ISO8601 datetime literal.</returns>
        public static string ConvertDateTimeToSafeLiteral(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        // Special strings to represent datetime

        /*!
			Convert DateTime to string representation
			\param objDate Date to convert
			\param blnAddTime true to add the time part 
			\return String representing the DateTime
		*/

        /// <summary>The convert date time to string.</summary>
        /// <param name="objDate">The obj date.</param>
        /// <param name="blnAddTime">The bln add time.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDateTimeToString(DateTime objDate, bool blnAddTime)
        {
            return objDate.ToString(blnAddTime ? CStrDateTimeFormat : CStrDateFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>Convert DateTime to string representation</summary>
        /// <param name="objDate">Date to convert</param>
        /// <param name="strFormat">Format</param>
        /// <returns>String representing the DateTime</returns>
        public static string ConvertDateTimeToString(DateTime objDate, string strFormat)
        {
            return objDate.ToString(strFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /*!
            Convert decimal value to string formatted for user display
            \param dNumber Number to convert
            \param intNumberDecimalDigits Number of decimal digits
            \param blnTrimTrailingZeros true to trim the trailing zeros
            \param blnShowGroupSeparator true to show the number group separator
            \return Return converted text
        */

        /// <summary>The convert decimal to string.</summary>
        /// <param name="dNumber">The d number.</param>
        /// <param name="intNumberDecimalDigits">The int number decimal digits.</param>
        /// <param name="blnTrimTrailingZeros">The bln trim trailing zeros.</param>
        /// <param name="blnShowGroupSeparator">The bln show group separator.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDecimalToString(
            decimal dNumber, int intNumberDecimalDigits, bool blnTrimTrailingZeros, bool blnShowGroupSeparator)
        {
            return ConvertDecimalToString(
                dNumber, intNumberDecimalDigits, blnTrimTrailingZeros, blnShowGroupSeparator, false);
        }

        /// <summary>Convert decimal value to string formatted for user display</summary>
        /// <param name="dNumber">Number to convert</param>
        /// <param name="intNumberDecimalDigits">Number of decimal digits</param>
        /// <param name="blnTrimTrailingZeros">true to trim the trailing zeros</param>
        /// <param name="blnShowGroupSeparator">true to show the number group separator</param>
        /// <param name="blnTrimWholeNumbers">true to trim whole numbers</param>
        /// <returns>Converted text</returns>
        public static string ConvertDecimalToString(
            decimal dNumber, 
            int intNumberDecimalDigits, 
            bool blnTrimTrailingZeros, 
            bool blnShowGroupSeparator, 
            bool blnTrimWholeNumbers)
        {
            var objNumberFormatInfo = new NumberFormatInfo { NumberDecimalDigits = intNumberDecimalDigits };

            if (!blnShowGroupSeparator)
            {
                objNumberFormatInfo.NumberGroupSeparator = string.Empty;
            }

            var strRes = dNumber.ToString("n", objNumberFormatInfo);

            var r = (double)(dNumber - Math.Floor(dNumber));
            if (blnTrimTrailingZeros || (blnTrimWholeNumbers && r < (1 / Math.Pow(10d, intNumberDecimalDigits))))
            {
                var intDecSepPos = strRes.IndexOf(objNumberFormatInfo.NumberDecimalSeparator);
                if (intDecSepPos > -1)
                {
                    strRes = strRes.TrimEnd('0');
                    strRes = strRes.TrimEnd(objNumberFormatInfo.NumberDecimalSeparator.ToCharArray());
                }
            }

            return strRes;
        }

        /// <summary>The convert decimal to string.</summary>
        /// <param name="dNumber">The d number.</param>
        /// <param name="intNumberDecimalDigits">The int number decimal digits.</param>
        /// <param name="blnTrimTrailingZeros">The bln trim trailing zeros.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDecimalToString(
            decimal dNumber, int intNumberDecimalDigits, bool blnTrimTrailingZeros)
        {
            return ConvertDecimalToString(dNumber, intNumberDecimalDigits, blnTrimTrailingZeros, true);
        }

        /// <summary>The convert double to string.</summary>
        /// <param name="dblNumber">The dbl number.</param>
        /// <param name="intNumberDecimalDigits">The int number decimal digits.</param>
        /// <param name="blnTrimTrailingZeros">The bln trim trailing zeros.</param>
        /// <param name="blnShowGroupSeparator">The bln show group separator.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDoubleToString(
            double dblNumber, int intNumberDecimalDigits, bool blnTrimTrailingZeros, bool blnShowGroupSeparator)
        {
            return ConvertDoubleToString(
                dblNumber, intNumberDecimalDigits, blnTrimTrailingZeros, blnShowGroupSeparator, false);
        }

        /// <summary>Convert double value to string formatted for user display</summary>
        /// <param name="dblNumber">Number to convert</param>
        /// <param name="intNumberDecimalDigits">Number of decimal digits</param>
        /// <param name="blnTrimTrailingZeros">true to trim the trailing zeros</param>
        /// <param name="blnShowGroupSeparator">true to show the number group separator</param>
        /// <param name="blnTrimWholeNumbers">true to trim whole numbers</param>
        /// <returns>Converted text</returns>
        public static string ConvertDoubleToString(
            double dblNumber, 
            int intNumberDecimalDigits, 
            bool blnTrimTrailingZeros, 
            bool blnShowGroupSeparator, 
            bool blnTrimWholeNumbers)
        {
            var objNumberFormatInfo = new NumberFormatInfo();
            objNumberFormatInfo.NumberDecimalDigits = intNumberDecimalDigits;

            if (!blnShowGroupSeparator)
            {
                objNumberFormatInfo.NumberGroupSeparator = string.Empty;
            }

            var strRes = dblNumber.ToString("n", objNumberFormatInfo);

            if (blnTrimTrailingZeros
                || (blnTrimWholeNumbers
                    && (dblNumber - Math.Floor(dblNumber)) < (1 / Math.Pow(10d, intNumberDecimalDigits))))
            {
                var intDecSepPos = strRes.IndexOf(objNumberFormatInfo.NumberDecimalSeparator);
                if (intDecSepPos > -1)
                {
                    strRes = strRes.TrimEnd('0');
                    strRes = strRes.TrimEnd(objNumberFormatInfo.NumberDecimalSeparator.ToCharArray());
                }
            }

            // This is not correct -  we loose the symmetry (the empty string doesn't convert back to Double.NaN)
            if (strRes == double.NaN.ToString())
            {
                strRes = string.Empty;
            }

            return strRes;
        }

        /// <summary>The convert double to string.</summary>
        /// <param name="dblNumber">The dbl number.</param>
        /// <param name="intNumberDecimalDigits">The int number decimal digits.</param>
        /// <param name="blnTrimTrailingZeros">The bln trim trailing zeros.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDoubleToString(
            double dblNumber, int intNumberDecimalDigits, bool blnTrimTrailingZeros)
        {
            return ConvertDoubleToString(dblNumber, intNumberDecimalDigits, blnTrimTrailingZeros, true);
        }

        /// <summary>The convert multiline text.</summary>
        /// <param name="strString">The str string.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertMultilineText(string strString)
        {
            return strString.Replace("\r\n", "\n");
        }

        /// <summary>Convert string representation of date and time to DateTime</summary>
        /// <param name="strDate">Date string to convert</param>
        /// <param name="blnAddTime">true to add the time part</param>
        /// <returns>Converted date</returns>
        public static DateTime ConvertStringToDateTime(string strDate, bool blnAddTime)
        {
            return DateTime.ParseExact(
                strDate, 
                blnAddTime ? CStrParseDateTimeFormat : CStrParseDateFormat, 
                DateTimeFormatInfo.InvariantInfo, 
                DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>The convert string to double.</summary>
        /// <param name="strNumber">The str number.</param>
        /// <param name="dblResult">The dbl result.</param>
        /// <param name="strError">The str error.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool ConvertStringToDouble(string strNumber, out double dblResult, ref string strError)
        {
            var blnRes = false;
            dblResult = 0;

            if (!string.IsNullOrEmpty(strNumber))
            {
                strNumber = strNumber.Trim();
            }

            if (string.IsNullOrEmpty(strNumber) || strNumber.Length == 0)
            {
                blnRes = true;
            }
            else
            {
                double dblFactor;
                if (DetectNumberShortcuts(strNumber, ref strNumber, out dblFactor))
                {
                    if (double.TryParse(strNumber, NumberStyles.Any, null, out dblResult))
                    {
                        if (dblFactor != 1)
                        {
                            dblResult *= dblFactor;
                        }

                        blnRes = true;
                    }
                    else
                    {
                        strError = "Unable to convert (" + strNumber + ") to number";
                    }
                }
                else
                {
                    strError = strNumber;
                }
            }

            return blnRes;
        }

        /// <summary>Convert string to int</summary>
        /// <param name="strNumber">String to convert</param>
        /// <param name="intResult">Return converted number</param>
        /// <param name="strError">Return error message</param>
        /// <returns>true on success, false otherwise</returns>
        public static bool ConvertStringToInt(string strNumber, out int intResult, ref string strError)
        {
            var blnRes = false;

            intResult = 0;
            double dblResult;
            if (ConvertStringToDouble(strNumber, out dblResult, ref strError))
            {
                intResult = (int)dblResult;
                blnRes = true;
            }

            return blnRes;
        }

        /// <summary>The convert string to time.</summary>
        /// <param name="strTime">The str time.</param>
        /// <returns>The System.DateTime.</returns>
        public static DateTime ConvertStringToTime(string strTime)
        {
            return DateTime.ParseExact(
                strTime, CStrHhTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>The convert time to string.</summary>
        /// <param name="objTime">The obj time.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertTimeToString(DateTime objTime)
        {
            return objTime.ToString(CStrHhTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>The copy from collection.</summary>
        /// <param name="objDestination">The obj destination.</param>
        /// <param name="objSource">The obj source.</param>
        /// <typeparam name="T"></typeparam>
        public static void CopyFromCollection<T>(List<T> objDestination, ICollection objSource)
        {
            foreach (T objItem in objSource)
            {
                objDestination.Add(objItem);
            }
        }

        /// <summary>Decompresses an input string created using Tools.Compress method</summary>
        /// <param name="compressed">Compressed string</param>
        /// <returns>String is the decompressed string</returns>
        /// <remarks>If the input string was not compressed using Tools.Compress then the input string is returned</remarks>
        public static string Decompress(string compressed)
        {
            // Check to see if the input string has an integer in the first 10 chars followed by a hash
            var outputBufferSize = 0;
            var compressedIndicatorPos = compressed.IndexOf('#');

            if (compressedIndicatorPos > 0 && compressedIndicatorPos < 10)
            {
                var originalSizeText = compressed.Substring(0, compressedIndicatorPos);

                if (!int.TryParse(originalSizeText, out outputBufferSize))
                {
                    compressedIndicatorPos = -1;
                }
            }
            else
            {
                compressedIndicatorPos = -1;
            }

            // If Tools.Compress indicator found then decompress string
            if (compressedIndicatorPos != -1)
            {
                // Decode the Base64 so we are working with the raw compressed data
                var compressedBuffer = Convert.FromBase64String(compressed.Substring(compressedIndicatorPos + 1));

                // Create memory stream
                var memoryStream = new MemoryStream(compressedBuffer);

                // Create decompressed stream
                var memoryStreamDecompressed = new GZipStream(memoryStream, CompressionMode.Decompress);

                // Create output buffer of the original size
                var outputBuffer = new byte[outputBufferSize];

                // Read decompressed data
                var bytesRead = memoryStreamDecompressed.Read(outputBuffer, 0, outputBufferSize);

                // Convert bytes back to a string
                var decompressed = Encoding.Unicode.GetString(outputBuffer);

                // Close the streams
                memoryStreamDecompressed.Close();
                memoryStream.Close();

                // Return decompressed string
                return decompressed;
            }

            // Compression identifier not found, just return input string
            return compressed;
        }

        /// <summary>The decrypt identifier.</summary>
        /// <param name="obfruscatedIdentifier">The obfruscated identifier.</param>
        /// <returns>The System.Int32.</returns>
        public static int DecryptIdentifier(string obfruscatedIdentifier)
        {
            if (string.IsNullOrEmpty(obfruscatedIdentifier))
            {
                return 0;
            }

            var identifierText = new StringBuilder(obfruscatedIdentifier.Length - 1);
            identifierText.Append('0', obfruscatedIdentifier.Length - 2);
            var location = 13;

            var digitSequence = StripSequence(FullDigitSequence, obfruscatedIdentifier.Length);

            var forwardDirection = true;

            for (var pos = 0; pos < obfruscatedIdentifier.Length; pos++)
            {
                var digitPos = CryptoSequence.IndexOf(obfruscatedIdentifier[pos]);

                if (digitPos < 0)
                {
                    return -1;
                }

                int result;

                if (forwardDirection)
                {
                    result = digitPos - location;

                    if (digitPos > CryptoSequence.Length - 10)
                    {
                        forwardDirection = false;
                    }
                }
                else
                {
                    result = location - digitPos;

                    if (digitPos < 10)
                    {
                        forwardDirection = true;
                    }
                }

                if (result < 0 || result > 9)
                {
                    return -1;
                }

                location = digitPos;

                var digitSeqPos = int.Parse(digitSequence[pos].ToString());
                if (digitSeqPos < obfruscatedIdentifier.Length - 2)
                {
                    identifierText[digitSeqPos] = result.ToString()[0];
                }
            }

            return int.Parse(identifierText.ToString());
        }

        /// <summary>The encrypt identifier.</summary>
        /// <param name="identity">The identity.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns>The System.String.</returns>
        public static string EncryptIdentifier(int identity, int seconds)
        {
            var numberLength = identity.ToString().Length;
            var codeLength = numberLength + 2;

            if (codeLength > 16 || identity < 0)
            {
                return "ERROR";
            }

            var digitSequence = StripSequence(FullDigitSequence, numberLength + 2);

            var format = "D" + numberLength;
            var inputText1 = identity.ToString(format);
            var inputText2 = seconds.ToString("D2");

            var inputText = inputText1 + inputText2;

            var obfruscatedIdentifier = new StringBuilder();

            var location = 13;

            var forwardDirection = true;

            for (var pos = 0; pos < digitSequence.Length; pos++)
            {
                var digitPos = int.Parse(digitSequence[pos].ToString(), NumberStyles.HexNumber);
                var digit = int.Parse(inputText[digitPos].ToString());

                if (forwardDirection)
                {
                    location += digit;
                }
                else
                {
                    location -= digit;
                }

                obfruscatedIdentifier.Append(CryptoSequence[location]);

                if (forwardDirection)
                {
                    if (location > CryptoSequence.Length - 10)
                    {
                        forwardDirection = false;
                    }
                }
                else
                {
                    if (location < 10)
                    {
                        forwardDirection = true;
                    }
                }
            }

            return obfruscatedIdentifier.ToString();
        }

        /*!
			Convert string collection to delimited string
			\param objTokenCollection Token collection
			\param strDelimiter Delimiter string
			\return Delimited string
		*/

        /// <summary>The format delimited string.</summary>
        /// <param name="objTokenCollection">The obj token collection.</param>
        /// <param name="strDelimiter">The str delimiter.</param>
        /// <returns>The System.String.</returns>
        public static string FormatDelimitedString(StringCollection objTokenCollection, string strDelimiter)
        {
            var objItemString = new StringBuilder();
            foreach (var strToken in objTokenCollection)
            {
                objItemString.Append(strToken);
                objItemString.Append(strDelimiter);
            }

            return objItemString.ToString();
        }

        /// <summary>Convert string collection to delimited string</summary>
        /// <param name="tokenCollection">Token collection</param>
        /// <param name="delimiter">Delimiter character</param>
        /// <returns>Delimited string</returns>
        public static string FormatDelimitedString(StringCollection tokenCollection, char delimiter)
        {
            var itemString = new StringBuilder();
            foreach (var token in tokenCollection)
            {
                itemString.Append(token);
                itemString.Append(delimiter);
            }

            return itemString.ToString();
        }

        /*!
			Helper method to format the reflection string for a given type
			\param objType Type to create the reflection string for
			\return Formatted reflection string
		*/

        /// <summary>The format reflection string.</summary>
        /// <param name="objType">The obj type.</param>
        /// <returns>The System.String.</returns>
        public static string FormatReflectionString(Type objType)
        {
            return objType.AssemblyQualifiedName;
        }

        /// <summary>Provides a utility for generating hash codes using the algorithm described at http://stackoverflow.com/q/263400</summary>
        /// <param name="fields">A list of fields for inclusion in the hashing function input</param>
        /// <returns>The hash of the fields input</returns>
        public static int GenerateHash(params object[] fields)
        {
            unchecked
            {
                var hash = 17;
                foreach (var field in fields)
                {
                    if (field == null)
                    {
                        continue;
                    }

                    hash = hash * 23 + field.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>The get ip address.</summary>
        /// <returns>The System.String.</returns>
        public static string GetIPAddress()
        {
            var hostName = Dns.GetHostName();
            var ipAddress = string.Empty;

            if (!string.IsNullOrEmpty(hostName))
            {
                var iPv4Address = Array.Find(
                    Dns.GetHostAddresses(hostName), x => (x.AddressFamily == AddressFamily.InterNetwork));
                if (iPv4Address != null)
                {
                    ipAddress = iPv4Address.ToString();
                }
            }

            return ipAddress;
        }

        /// <summary>The get icon info.</summary>
        /// <param name="hIcon">The h icon.</param>
        /// <param name="pIconInfo">The p icon info.</param>
        /// <returns>The System.Boolean.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        /// <summary>The is design mode.</summary>
        /// <returns>The System.Boolean.</returns>
        public static bool IsDesignMode()
        {
            if (_executionMode == ExecutionMode.Unknown)
            {
                _executionMode = (Process.GetCurrentProcess().ProcessName == "devenv")
                                     ? ExecutionMode.Design
                                     : ExecutionMode.Runtime;
            }

            return _executionMode == ExecutionMode.Design;
        }

        /// <summary>The is strike delta valid.</summary>
        /// <param name="strike">The strike.</param>
        /// <param name="error">The error.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool IsStrikeDeltaValid(string strike, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(strike))
            {
                return false;
            }

            var strikeUpper = strike.Trim().ToUpper();

            var result = true;
            double strikeInDouble;

            if (!double.TryParse(strikeUpper, out strikeInDouble))
            {
                result = false;
                error = "Strike not is valid format"; // required since input can be any character other than following

                // Check if its a standard format string
                if (strikeUpper == "ATM" || strikeUpper == "ATMS" || strikeUpper == "ATMF" || strikeUpper == "S"
                    || strikeUpper == "F" || strikeUpper == "DN")
                {
                    result = true;
                }
                else
                {
                    // Check if it is a numeric format
                    if (strikeUpper.EndsWith("D") || strikeUpper.EndsWith("F"))
                    {
                        if (double.TryParse(strikeUpper.Substring(0, strikeUpper.Length - 1), out strikeInDouble))
                        {
                            result = true;
                        }
                    }
                }
            }

            if (result)
            {
                error = string.Empty;
            }

            return result;
        }

        /// <summary>The prepare multiline string.</summary>
        /// <param name="strString">The str string.</param>
        /// <returns>The System.String.</returns>
        public static string PrepareMultilineString(string strString)
        {
            return strString.Replace("\n", "\r\n");
        }

        /// <summary>Send EMail Using the SMTP Server</summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="smtpServer"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmailNotification(
            string fromAddress, string toAddress, string smtpServer, string subject, string body)
        {
            var mailMessage = new MailMessage { From = new MailAddress(fromAddress) };
            mailMessage.To.Add(toAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient { Host = smtpServer, UseDefaultCredentials = true };

            smtpClient.Send(mailMessage);
        }

        /// <summary>Convert given object to string using specified format and object type</summary>
        /// <param name="strFormat">Format to be used</param>
        /// <param name="objVal">Object to convert</param>
        /// <returns>Formatted object string</returns>
        /// <remarks>Contains multiple exit points to speed up the call</remarks>
        public static string ToString(string strFormat, object objVal)
        {
            if (objVal.GetType() == typeof(Double))
            {
                return ((double)objVal).ToString(strFormat);
            }

            if (objVal.GetType() == typeof(decimal))
            {
                return ((decimal)objVal).ToString(strFormat);
            }

            if (objVal.GetType() == typeof(DateTime))
            {
                return ((DateTime)objVal).ToString(strFormat);
            }

            return objVal.ToString();
        }

        /// <summary>Convert given string to another string using specified data type format</summary>
        /// <param name="strFormat">Format to be used</param>
        /// <param name="strDataType">Original DataType stored in string format in strVal</param>
        /// <param name="objVal">value to be converted to provided format strFormat</param>
        /// <returns>Formatted object string</returns>
        public static string ToString(string strFormat, string strDataType, object objVal)
        {
            string strResult;
            var strError = string.Empty;

            try
            {
                switch (strDataType)
                {
                    case "DOUBLE":
                        double dblValue;
                        if (!ConvertStringToDouble(objVal.ToString(), out dblValue, ref strError))
                        {
                            throw new Exception(strError);
                        }

                        strResult = ToString(strFormat, dblValue);
                        break;
                    case "DATETIME":
                        var objDateTime = DateTime.Now;
                        try
                        {
                            objDateTime = ConvertStringToDateTime(objVal.ToString(), true);
                        }
                        catch
                        {
                            objDateTime = ConvertStringToDateTime(objVal.ToString(), false);
                        }

                        strResult = ToString(strFormat, objDateTime);
                        break;
                    default:
                        strResult = ToString(strFormat, objVal);
                        break;
                }
            }
            catch (Exception ex)
            {
                strResult = string.Format(
                    "Unable to format [{0}] as [{1}] using [{2}]: {3}", 
                    objVal.GetType(), 
                    strDataType, 
                    strFormat, 
                    ex.Message);
            }

            return strResult;
        }

        /// <summary>Translate special date or time string DateTime (in UTC)</summary>
        /// <param name="strDateString">Date string to convert: TODAY translated to today day, NOW translate to current date and time, YESTERDAY translated to today-1 day</param>
        /// <returns>Converted date</returns>
        public static DateTime TranslateStringToDateTime(string strDateString)
        {
            var objRes = DateTime.UtcNow;

            switch (strDateString.ToUpper())
            {
                case DateTimeString.Yesterday:
                    objRes = DateTime.UtcNow.AddDays(-1);
                    break;
                case DateTimeString.Today:
                    objRes = DateTime.UtcNow.Date;
                    break;
                case DateTimeString.Now:
                    objRes = DateTime.UtcNow;
                    break;
                default:
                    Debug.Assert(false, "Unknown datetime string");
                    break;
            }

            return objRes;
        }

        /// <summary>The trim.</summary>
        /// <param name="objVal">The obj val.</param>
        /// <returns>The System.String.</returns>
        public static string Trim(object objVal)
        {
            return objVal.ToString().Trim();
        }

        /// <summary>The trim zeros.</summary>
        /// <param name="strString">The str string.</param>
        /// <returns>The System.String.</returns>
        public static string TrimZeros(string strString)
        {
            var strReturn = strString.Trim('\0');
            var intZero = strReturn.IndexOf("\0");
            if (intZero > 0)
            {
                strReturn = strReturn.Substring(0, intZero);
            }

            return strReturn;
        }

        /// <summary>The validate server certificate.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The ssl policy errors.</param>
        /// <param name="SSLCertificates">The ssl certificates.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool ValidateServerCertificate(
            object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors, 
            Dictionary<string, X509Certificate2> SSLCertificates)
        {
            foreach (var cert in SSLCertificates.Values)
            {
                if (certificate.Equals(cert))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>Detect the shortcut symbols in the number string</summary>
        /// <param name="strNumber">Number string to expand shortcuts</param>
        /// <param name="strResult">Return number string with shortcuts characters removed</param>
        /// <param name="dblFactor">Factor</param>
        /// <returns>true on success, false otherwise</returns>
        internal static bool DetectNumberShortcuts(string strNumber, ref string strResult, out double dblFactor)
        {
            // Convert to upper case for easier search
            var strNumberString = strNumber.ToUpper();

            // Determine the shortcut(s) positions
            var intKiloPos = strNumberString.IndexOf(CChrKiloSymbol);
            var intMillionPos = strNumberString.IndexOf(CChrMillionSymbol);
            var intBillionPos = strNumberString.IndexOf(CChrBillionSymbol);
            var intBillionPos_1 = strNumberString.IndexOf(CChrBillionSymbol1);
            var intTrillionPos = strNumberString.IndexOf(CChrTrillionSymbol);

            // Assign the factor and symbol
            var intShortcutsCount = 0;
            var chrShortcutUsed = ' ';
            dblFactor = 1;
            var intShortcutPos = -1;

            if (intKiloPos > -1)
            {
                intShortcutsCount++;
                intShortcutPos = intKiloPos;
                chrShortcutUsed = CChrKiloSymbol;
                dblFactor = CDblKiloFactor;
            }

            if (intMillionPos > -1)
            {
                intShortcutsCount++;
                intShortcutPos = intMillionPos;
                chrShortcutUsed = CChrMillionSymbol;
                dblFactor = CDblMillionFactor;
            }

            if (intBillionPos > -1)
            {
                intShortcutsCount++;
                intShortcutPos = intBillionPos;
                chrShortcutUsed = CChrBillionSymbol;
                dblFactor = CDblBillionFactor;
            }

            if (intBillionPos_1 > -1)
            {
                intShortcutsCount++;
                intShortcutPos = intBillionPos_1;
                chrShortcutUsed = CChrBillionSymbol1;
                dblFactor = CDblBillionFactor;
            }

            if (intTrillionPos > -1)
            {
                intShortcutsCount++;
                intShortcutPos = intTrillionPos;
                chrShortcutUsed = CChrTrillionSymbol;
                dblFactor = CDblTrillionFactor;
            }

            // Verify expanded shortcut
            if (intShortcutsCount > 0)
            {
                if (intShortcutsCount == 1)
                {
                    // Make sure that shortcut symbol is the last one
                    if (intShortcutPos < strNumberString.Length - 1)
                    {
                        strResult = "Invalid use of '" + chrShortcutUsed.ToString()
                                    + "' symbol, it should be the last character";
                        return false;
                    }

                    strResult = strNumberString.Replace(chrShortcutUsed.ToString(), string.Empty);
                }
                else
                {
                    strResult =
                        string.Format(
                            "Invalid use of shortcut symbol, only one of '{0}', '{1}', '{2}', '{3}' , '{4}'should be used.", 
                            CChrKiloSymbol.ToString(), 
                            CChrMillionSymbol.ToString(), 
                            CChrBillionSymbol.ToString(), 
                            CChrBillionSymbol1.ToString(), 
                            CChrTrillionSymbol.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>The strip sequence.</summary>
        /// <param name="fullDigitSequence">The full digit sequence.</param>
        /// <param name="digits">The digits.</param>
        /// <returns>The System.String.</returns>
        private static string StripSequence(string fullDigitSequence, int digits)
        {
            var digitSequence = new StringBuilder();
            for (var pos = 0; pos < fullDigitSequence.Length; pos++)
            {
                var digitPos = int.Parse(fullDigitSequence[pos].ToString(), NumberStyles.HexNumber);
                if (digitPos < digits)
                {
                    digitSequence.Append(fullDigitSequence[pos]);
                }
            }

            return digitSequence.ToString();
        }

        #endregion

        /// <summary>The icon info.</summary>
        public struct IconInfo
        {
            #region Fields

            /// <summary>The f icon.</summary>
            public bool fIcon;

            /// <summary>The hbm color.</summary>
            public IntPtr hbmColor;

            /// <summary>The hbm mask.</summary>
            public IntPtr hbmMask;

            /// <summary>The x hotspot.</summary>
            public int xHotspot;

            /// <summary>The y hotspot.</summary>
            public int yHotspot;

            #endregion
        }

        /// <summary>The generic enum converter test.</summary>
        public static class GenericEnumConverterTest
        {
            #region Public Methods and Operators

            /// <summary>Check all converter mappings</summary>
            /// <param name="enumConverterType">Type of converter</param>
            /// <param name="errors"></param>
            /// <returns>true on success, false otherwise</returns>
            public static bool CheckAll(Type enumConverterType, out string errors)
            {
                var errorList = new List<string>();

                var fieldInfos = enumConverterType.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var fieldInfo in fieldInfos)
                {
                    var enumToDb = new Dictionary<object, object>();
                    var dbToEnum = new Dictionary<object, object>();

                    // Get the converter types information
                    var converter = fieldInfo.GetValue(null);

                    Type enumType = null;
                    Type dbType = null;

                    GetEnumConverterTypes(converter, ref enumType, ref dbType);

                    foreach (var enumValue in Enum.GetValues(enumType))
                    {
                        // Enum to db
                        var dbConverterValue = GetEnumConverterItem(converter, enumType, enumValue);

                        // Db to enum
                        var enumConverterValue = GetEnumConverterItem(converter, dbType, dbConverterValue);

                        // Detect any anomalies
                        if (enumToDb.ContainsKey(enumValue))
                        {
                            errorList.Add(
                                string.Format(
                                    "Duplicate enum value detected for {0} converter: {1}", fieldInfo.Name, enumValue));
                        }

                        enumToDb[enumValue] = dbConverterValue;

                        if (dbToEnum.ContainsKey(dbConverterValue))
                        {
                            errorList.Add(
                                string.Format(
                                    "Duplicate DbValue attribute detected for {0} converter: {1}", 
                                    fieldInfo.Name, 
                                    enumValue));
                        }

                        dbToEnum[dbConverterValue] = enumConverterValue;

                        if (!enumValue.Equals(enumConverterValue))
                        {
                            errorList.Add(
                                string.Format(
                                    "Round-trip conversion failed for {0}: Enum[{1}] --> DbByEnum[{2}] --> EnumByDb[{3}]", 
                                    fieldInfo.Name, 
                                    enumValue, 
                                    dbConverterValue, 
                                    enumConverterValue));
                        }
                    }
                }

                var stringBuilder = new StringBuilder();
                foreach (var error in errorList)
                {
                    stringBuilder.AppendLine(error);
                }

                errors = stringBuilder.ToString();

                // Using errors to detect the problems on purpose as we have to accumulate multiple issues
                return string.IsNullOrEmpty(errors);
            }

            #endregion

            #region Methods

            /// <summary>Get the item from enum converter</summary>
            /// <param name="converter">Converter to get the value from</param>
            /// <param name="indexType">Index type (enum type of db type)</param>
            /// <param name="indexValue">Index value for which the indexer value is required</param>
            /// <returns>Converted value for a given indexer</returns>
            private static object GetEnumConverterItem(object converter, Type indexType, object indexValue)
            {
                var getItemParams = new[] { indexType };
                var methodInfo = converter.GetType().GetMethod("get_Item", getItemParams);

                var methodParams = new[] { indexValue };
                var itemValue = methodInfo.Invoke(converter, methodParams);

                return itemValue;
            }

            /// <summary>Get the types used in the enum converter</summary>
            /// <param name="converter">Converter to analyze</param>
            /// <param name="enumType">Return the enum types</param>
            /// <param name="dbType">Return database type</param>
            private static void GetEnumConverterTypes(object converter, ref Type enumType, ref Type dbType)
            {
                var converterType = converter.GetType();
                var geneticTypes = converterType.GetGenericArguments();

                for (var i = 0; i < geneticTypes.Length; i++)
                {
                    if (geneticTypes[i].BaseType == typeof(Enum))
                    {
                        enumType = geneticTypes[i];
                    }
                    else
                    {
                        dbType = geneticTypes[i];
                    }
                }
            }

            #endregion
        }

        /// <summary>The date time string.</summary>
        public class DateTimeString
        {
            #region Constants

            /// <summary>The now.</summary>
            public const string Now = "NOW";

            /// <summary>The today.</summary>
            public const string Today = "TODAY";

            /// <summary>The yesterday.</summary>
            public const string Yesterday = "YESTERDAY";

            #endregion

            /*!
				Determine whether a given text(non-date, e.g. TODAY) can be translated into DateTime
				\param strText Text to check
				\return true if the string can be translated into DateTime, false otherwise
			*/

            /*!
				Check whether a special string should contain time part
				\param strText Text to check
				\return true if text contains time part, false otherwise
				\note Utility for conversion from DateTime to string
			*/
            #region Public Methods and Operators

            /// <summary>The has time.</summary>
            /// <param name="strText">The str text.</param>
            /// <returns>The System.Boolean.</returns>
            public static bool HasTime(string strText)
            {
                return strText.ToUpper() == Now;
            }

            /// <summary>The is date time string.</summary>
            /// <param name="strText">The str text.</param>
            /// <returns>The System.Boolean.</returns>
            public static bool IsDateTimeString(string strText)
            {
                strText = strText.ToUpper();
                return strText == Yesterday || strText == Today || strText == Now;
            }

            #endregion
        }
    }

    #region SybaseTools

    /// <summary>
    ///     Various utility functions only use for Sybase-specific operations
    /// </summary>
    public abstract class SybaseTools
    {
        #region Constants

        /// <summary>The c_str date format.</summary>
        public const string c_strDateFormat = "MMM dd yyyy"; /*!< Standard date format */

        /// <summary>The c_str date time format.</summary>
        public const string c_strDateTimeFormat = c_strDateFormat + "  hh:mm:ss:ffftt";

        /*!< Standard datetime format (convert 109) */

        /// <summary>The c_str hh time format.</summary>
        public const string c_strHHTimeFormat = "HH:mm:ss";

        /// <summary>The c_str mm mdd date format.</summary>
        public const string c_strMMMddDateFormat = "MMM dd"; /*!< Standard date format without year */

        /// <summary>The c_str parse date format.</summary>
        public const string c_strParseDateFormat = "MMM d yyyy";

        /// <summary>The c_str parse date time format.</summary>
        public const string c_strParseDateTimeFormat = "MMM d yyyy h:mm:ss:ffftt";

        #endregion

        /*!< Standard datetime format (convert 109) for parsing as database comes with single digits for days and hours */

        /*!
			Convert DateTime to string representation
			\param objDate Date to convert
			\param blnAddTime true to add the time part 
			\return String representing the DateTime
		*/
        #region Public Methods and Operators

        /// <summary>The convert date time to string.</summary>
        /// <param name="objDate">The obj date.</param>
        /// <param name="blnAddTime">The bln add time.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertDateTimeToString(DateTime objDate, bool blnAddTime)
        {
            return objDate.ToString(
                blnAddTime ? c_strDateTimeFormat : c_strDateFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>Convert DateTime to string representation</summary>
        /// <param name="objDate">Date to convert</param>
        /// <param name="strFormat">Format</param>
        /// <returns>String representing the DateTime</returns>
        public static string ConvertDateTimeToString(DateTime objDate, string strFormat)
        {
            return objDate.ToString(strFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>The convert string to date time.</summary>
        /// <param name="strDate">The str date.</param>
        /// <param name="strFormat">The str format.</param>
        /// <returns>The System.DateTime.</returns>
        public static DateTime ConvertStringToDateTime(string strDate, string strFormat)
        {
            return DateTime.ParseExact(
                strDate, strFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces);
        }

        /// <summary>Convert string representation of date and time to DateTime</summary>
        /// <param name="strDate">strDate Date string to convert</param>
        /// <param name="blnAddTime">true to add the time part</param>
        /// <returns>Converted date</returns>
        public static DateTime ConvertStringToDateTime(string strDate, bool blnAddTime)
        {
            return DateTime.ParseExact(
                strDate, 
                blnAddTime ? c_strParseDateTimeFormat : c_strParseDateFormat, 
                DateTimeFormatInfo.InvariantInfo, 
                DateTimeStyles.AllowWhiteSpaces);
        }

        #endregion
    }

    #endregion
}