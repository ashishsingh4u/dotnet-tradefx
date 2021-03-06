//  ===================================================================================
//  <copyright file="MessageParserFactory.cs" company="TechieNotes">
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
//  <date>26-01-2013</date>
//  <summary>
//     The MessageParserFactory.cs file.
//  </summary>
//  ===================================================================================

using System.Net.Sockets;

using Emcaster.Sockets;

namespace Emcaster.Topics
{
    /// <summary>The message parser factory.</summary>
    public class MessageParserFactory : IByteParserFactory, IMessageListener, IMessageEvent
    {
        #region Public Events

        /// <summary>The message event.</summary>
        public event OnTopicMessage MessageEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns>The <see cref="IByteParser"/>.</returns>
        public IByteParser Create(Socket socket)
        {
            return new MessageParser(this);
        }

        /// <summary>The create.</summary>
        /// <returns>The <see cref="MessageParser"/>.</returns>
        public MessageParser Create()
        {
            return new MessageParser(this);
        }

        /// <summary>The on message.</summary>
        /// <param name="parser">The parser.</param>
        public void OnMessage(IMessageParser parser)
        {
            var msg = MessageEvent;
            if (msg != null)
            {
                msg(parser);
            }
        }

        #endregion
    }
}