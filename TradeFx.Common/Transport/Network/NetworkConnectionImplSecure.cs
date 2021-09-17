//  ===================================================================================
//  <copyright file="NetworkConnectionImplSecure.cs" company="TechieNotes">
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
//     The NetworkConnectionImplSecure.cs file.
//  </summary>
//  ===================================================================================

using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection impl secure.</summary>
    internal class NetworkConnectionImplSecure : NetworkConnectionImpl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionImplSecure"/> class.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="certificateValidationCallback">The certificate validation callback.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        /// <param name="packetAssembler">The packet assembler.</param>
        public NetworkConnectionImplSecure(
            Socket socket, 
            RemoteCertificateValidationCallback certificateValidationCallback, 
            int readBufferSize, 
            PacketAssembler packetAssembler)
            : base(CreateSecureSocket(socket, certificateValidationCallback), readBufferSize, packetAssembler)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is secure.</summary>
        public override bool IsSecure
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The authenticate as client.</summary>
        /// <param name="serverCertificateName">The server certificate name.</param>
        public void AuthenticateAsClient(string serverCertificateName)
        {
            ((SslStream)this._connection).AuthenticateAsClient(serverCertificateName);
        }

        /// <summary>The authenticate as server.</summary>
        /// <param name="certificate">The certificate.</param>
        public void AuthenticateAsServer(X509Certificate certificate)
        {
            ((SslStream)this._connection).AuthenticateAsServer(certificate);
        }

        #endregion

        #region Methods

        /// <summary>The create secure socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="certificateValidationCallback">The certificate validation callback.</param>
        /// <returns>The System.IO.Stream.</returns>
        private static Stream CreateSecureSocket(
            Socket socket, RemoteCertificateValidationCallback certificateValidationCallback)
        {
            var stream = new NetworkStream(socket, true);
            return new SslStream(stream, false, certificateValidationCallback);
        }

        #endregion
    }
}