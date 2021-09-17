// ===================================================================================
// <copyright file="SocketConnectorException.cs" company="TechieNotes">
// ===================================================================================
//  TechieNotes Utilities & Best Practices
//  Samples and Guidelines for Winform & ASP.net development
// ===================================================================================
//  Copyright (c) TechieNotes.  All rights reserved.
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//  FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
// ===================================================================================
// </copyright>
// <author>Ashish Singh</author>
// <email>mailto:ashishsingh4u@gmail.com</email>
// <date>08-09-2012</date>
// <summary>
//    The SocketConnectorException.cs file.
// </summary>
// ===================================================================================

using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    /// Exception for proxy connector errors.
    /// </summary>
    [Serializable]
    public class SocketConnectorException : Exception
    {
        #region Fields

        /// <summary>The _error number.</summary>
        private readonly int _errorNumber;

        /// <summary>The _error type.</summary>
        private readonly SocketConnectorErrorType _errorType;

        /// <summary>The _http status.</summary>
        private readonly HttpStatusCode _httpStatus;

        /// <summary>The _remote uri.</summary>
        private readonly Uri _remoteUri;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. 
        /// Initializes an instance of the SocketConnectorException class.</summary>
        public SocketConnectorException()
        {
            this._errorType = SocketConnectorErrorType.UnexpectedError;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="message">Message describing the exception.</param>
        public SocketConnectorException(string message)
            : base(message)
        {
            this._errorType = SocketConnectorErrorType.UnexpectedError;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="inner">Exception that caused the current exception.</param>
        public SocketConnectorException(string message, Exception inner)
            : base(message, inner)
        {
            this._errorType = SocketConnectorErrorType.UnexpectedError;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="errorType">The type of the error.</param>
        public SocketConnectorException(SocketConnectorErrorType errorType)
        {
            this._errorType = errorType;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="errorType">The type of the error.</param>
        /// <param name="httpStatus">The status of the HTTP response.</param>
        /// <param name="remoteUri">Uri of the remote socket server.</param>
        public SocketConnectorException(SocketConnectorErrorType errorType, HttpStatusCode httpStatus, Uri remoteUri)
        {
            this._errorType = errorType;
            this._httpStatus = httpStatus;
            this._remoteUri = remoteUri;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="errorType">The type of the error.</param>
        /// <param name="errorNumber">The underlying Win32 error number.</param>
        public SocketConnectorException(SocketConnectorErrorType errorType, int errorNumber)
        {
            this._errorType = errorType;
            this._errorNumber = errorNumber;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="errorType">The type of the error.</param>
        /// <param name="inner">Exception that caused the current exception.</param>
        public SocketConnectorException(SocketConnectorErrorType errorType, Exception inner)
            : base(null, inner)
        {
            this._errorType = errorType;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="errorType">The type of the error.</param>
        public SocketConnectorException(string message, SocketConnectorErrorType errorType)
            : base(message)
        {
            this._errorType = errorType;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class. Initializes an instance of the SocketConnectorException class.</summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="inner">Exception that caused the current exception.</param>
        /// <param name="errorType">The type of the error.</param>
        public SocketConnectorException(string message, Exception inner, SocketConnectorErrorType errorType)
            : base(message, inner)
        {
            this._errorType = errorType;
        }

        /// <summary>Initializes a new instance of the <see cref="SocketConnectorException"/> class.</summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected SocketConnectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the error number. Only set when ErrorType is SspiError.
        /// </summary>
        public int ErrorNumber
        {
            get
            {
                return this._errorNumber;
            }
        }

        /// <summary>
        /// Gets the type of the error.
        /// </summary>
        public SocketConnectorErrorType ErrorType
        {
            get
            {
                return this._errorType;
            }
        }

        /// <summary>
        /// Gets the HttpStatus of the http response.
        /// </summary>
        public HttpStatusCode HttpStatus
        {
            get
            {
                return this._httpStatus;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                // I am not all that happy with this, but short of changing the logging approach, 
                // this is the easiest option to ensure that details are included in the log.
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine(base.Message);
                messageBuilder.Append("Error Type: ").AppendLine(this._errorType.ToString());
                if (this._httpStatus != 0)
                {
                    messageBuilder.Append("Http Status: ").AppendLine(this._httpStatus.ToString());
                }

                if (this._remoteUri != null)
                {
                    messageBuilder.Append("Remote Uri: ").AppendLine(this._remoteUri.OriginalString);
                }

                if (this._errorNumber != 0)
                {
                    messageBuilder.Append("Error Number: ").AppendLine(this._errorNumber.ToString());
                }

                return messageBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the Uri of the socket server to which connection is attempted.
        /// </summary>
        public Uri RemoteUri
        {
            get
            {
                return this._remoteUri;
            }
        }

        #endregion
    }
}