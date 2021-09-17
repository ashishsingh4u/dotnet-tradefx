//  ===================================================================================
//  <copyright file="Encrypter.cs" company="TechieNotes">
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
//     The Encrypter.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Security.Cryptography;
using System.Text;

namespace TradeFx.Common.Crypto
{
    /// <summary>
    ///     Encrypts/decrypts string and byte data.
    /// </summary>
    public class Encrypter
    {
        #region Fields

        /// <summary>The _decryptor.</summary>
        private readonly ICryptoTransform _decryptor;

        /// <summary>The _des crypto.</summary>
        private readonly TripleDESCryptoServiceProvider _desCrypto = new TripleDESCryptoServiceProvider();

        /// <summary>The _encryptor.</summary>
        private readonly ICryptoTransform _encryptor;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Encrypter"/> class using the specified literal to generate
        ///     the hash.</summary>
        /// <param name="hashLiteral">The hash literal.</param>
        public Encrypter(string hashLiteral)
        {
            if (string.IsNullOrEmpty(hashLiteral))
            {
                throw new ArgumentException("The hash literal is null or empty", "hashLiteral");
            }

            var hashMd5 = new MD5CryptoServiceProvider();
            var byteHash = hashMd5.ComputeHash(Encoding.ASCII.GetBytes(hashLiteral));
            this._desCrypto.Key = byteHash;
            this._desCrypto.Mode = CipherMode.ECB;

            this._encryptor = this._desCrypto.CreateEncryptor();
            this._decryptor = this._desCrypto.CreateDecryptor();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Decrypts the specified text.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The decrypted text string.</returns>
        public string Decrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("The data is null or empty", "data");
            }

            var byteBuff = Convert.FromBase64String(data);
            return Encoding.ASCII.GetString(this.Decrypt(byteBuff));
        }

        /// <summary>Decrypts the specified byte array.</summary>
        /// <param name="data">The data.</param>
        /// <returns>Decrypted array of bytes.</returns>
        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return this._decryptor.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>Encrypts the specified text.</summary>
        /// <param name="data">The data.</param>
        /// <returns>Encrypted text string.</returns>
        public string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("The data is null or empty", "data");
            }

            var byteBuff = Encoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(this.Encrypt(byteBuff));
        }

        /// <summary>Encrypts the specified byte array.</summary>
        /// <param name="data">The data.</param>
        /// <returns>Encrypted array of bytes.</returns>
        public byte[] Encrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return this._encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        #endregion
    }
}