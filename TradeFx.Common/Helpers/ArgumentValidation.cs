//  ===================================================================================
//  <copyright file="ArgumentValidation.cs" company="TechieNotes">
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
//     The ArgumentValidation.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Helpers
{
    /// <summary>The argument validation.</summary>
    public static class ArgumentValidation
    {
        #region Public Methods and Operators

        /// <summary>The check array length.</summary>
        /// <param name="array">The array.</param>
        /// <param name="expectedLength">The expected length.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentException">Throws if array length is not equal to expected length.</exception>
        public static void CheckArrayLength(Array array, int expectedLength, string variableName)
        {
            CheckForNullReference(array, "array");
            CheckForNullReference(variableName, "variableName");
            CheckZeroOrPositive(expectedLength, "expectedLength");

            if (array.Length != expectedLength)
            {
                throw new ArgumentException(
                    string.Format(
                        "Expected array length <{0}>, <{1}>'s length is <{2}>.", 
                        expectedLength, 
                        variableName, 
                        array.Length), 
                    variableName);
            }
        }

        /// <summary>The check buffer index.</summary>
        /// <param name="bufferLength">The buffer length.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if bufferlength and index difference is less than count</exception>
        public static void CheckBufferIndex(int bufferLength, int index, int count)
        {
            if ((bufferLength - index) < count)
            {
                var exceptionMessage =
                    string.Format(
                        "Index <{0}> and count <{1}> must refer to a location within the buffer, BufferLength <{2}>", 
                        index, 
                        count, 
                        bufferLength);
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>The check enumeration.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <exception cref="ArgumentException">Throws if enum is not defined.</exception>
        public static void CheckEnumeration<T>(object variable, string variableName) where T : struct
        {
            CheckForNullReference(variableName, "variableName");
            CheckForNullReference(variable, variableName);

            var enumType = typeof(T);
            if (!Enum.IsDefined(enumType, variable))
            {
                var exceptionMessage = string.Format(
                    "Enumeration <{0}> not defined for variable <{1}>, value <{2}>", 
                    enumType.FullName, 
                    variableName, 
                    variable);
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>The check expected type.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <exception cref="ArgumentException">Throws if enum is not defined.</exception>
        public static void CheckExpectedType<T>(object variable, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            CheckForNullReference(variable, variableName);

            var expectedType = typeof(T);
            var actualType = variable.GetType();

            if (!expectedType.IsAssignableFrom(actualType))
            {
                var exceptionMessage = string.Format(
                    "Expected type <{0}> not <{1}>", expectedType.FullName, actualType.FullName);
                throw new ArgumentException(exceptionMessage, variableName);
            }
        }

        /// <summary>The check for empty array.</summary>
        /// <param name="array">The array.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentException">Throws if Array length is zero.</exception>
        public static void CheckForEmptyArray(Array array, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            CheckForNullReference(array, variableName);

            if (array.Length == 0)
            {
                throw new ArgumentException("Expected non-empty array.", variableName);
            }
        }

        /// <summary>The check for empty string.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentException">Throws if string length is zero</exception>
        public static void CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);

            if (variable.Length == 0)
            {
                throw new ArgumentException("Expected non-empty string.", variableName);
            }
        }

        /// <summary>Check for null reference types and uninitialized value types.</summary>
        /// <typeparam name="T">Type Name</typeparam>
        /// <param name="variable">Type parameter.</param>
        /// <param name="variableName">String variable name.</param>
        /// <remarks>Removed class constraints due to .net 2.0 code having problems.</remarks>
        /// <remarks>See http://support.microsoft.com/kb/940164</remarks>
        public static void CheckForNullReference<T>(T variable, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            var x = default(T);
            if (Equals(x, variable))
            {
                throw new ArgumentNullException(variableName);
            }
        }

        /// <summary>The check for trimmed empty string.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentException">Throws if trimmed string length is zero.</exception>
        public static void CheckForTrimmedEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);

            if (variable.Trim().Length == 0)
            {
                throw new ArgumentException("Expected non-empty trimmed string.", variableName);
            }
        }

        /// <summary>The check positive.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if variable value is less than zero.</exception>
        public static void CheckPositive(int variable, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            if (variable <= 0)
            {
                throw new ArgumentOutOfRangeException(variableName, variable, "Must be positive");
            }
        }

        /// <summary>The check positive.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if variable value is less than zero.</exception>
        public static void CheckPositive(double variable, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            if (variable <= 0d)
            {
                throw new ArgumentOutOfRangeException(variableName, variable, "Must be positive");
            }
        }

        /// <summary>The check range exclusive.</summary>
        /// <param name="value">The value.</param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if variable value is not in the range.</exception>
        public static void CheckRangeExclusive(int value, int minimum, int maximum, string variableName)
        {
            if ((value < minimum) || (value > maximum))
            {
                var exceptionMessage = string.Format(
                    "Variable <{0}> value <{1}> not within range <{2}> to <{3}>", variableName, value, minimum, maximum);
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>The check zero or positive.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if value is less than zero.</exception>
        public static void CheckZeroOrPositive(int variable, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            if (variable < 0)
            {
                throw new ArgumentOutOfRangeException(variableName, variable, "Must be zero or positive");
            }
        }

        /// <summary>The check zero or positive.</summary>
        /// <param name="variable">The variable.</param>
        /// <param name="variableName">The variable name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if variable is less than zero.</exception>
        public static void CheckZeroOrPositive(double variable, string variableName)
        {
            CheckForNullReference(variableName, "variableName");
            if (variable < 0d)
            {
                throw new ArgumentOutOfRangeException(variableName, variable, "Must be zero or positive");
            }
        }

        #endregion
    }
}