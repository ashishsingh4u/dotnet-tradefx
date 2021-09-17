//  ===================================================================================
//  <copyright file="NetworkConnectionImplUnsecure.cs" company="TechieNotes">
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
//     The NetworkConnectionImplUnsecure.cs file.
//  </summary>
//  ===================================================================================

using System.Net.Sockets;

using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Transport.Network
{
    /// <summary>The network connection impl unsecure.</summary>
    internal class NetworkConnectionImplUnsecure : NetworkConnectionImpl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionImplUnsecure"/> class.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="readBufferSize">The read buffer size.</param>
        /// <param name="packetAssembler">The packet assembler.</param>
        public NetworkConnectionImplUnsecure(Socket socket, int readBufferSize, PacketAssembler packetAssembler)
            : base(new NetworkStream(socket, true), readBufferSize, packetAssembler)
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
    }
}