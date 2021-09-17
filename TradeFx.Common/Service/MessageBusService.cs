//  ===================================================================================
//  <copyright file="MessageBusService.cs" company="TechieNotes">
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
//  <date>27-01-2013</date>
//  <summary>
//     The MessageBusService.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Configuration;

using Emcaster.Sockets;
using Emcaster.Topics;

using TradeFx.Common.Events;
using TradeFx.Common.Interface;

namespace TradeFx.Common.Service
{
    /// <summary>The message bus service.</summary>
    internal class MessageBusService : ServiceBase, IMessageBusService
    {
        #region Fields

        /// <summary>The _async writer.</summary>
        private BatchWriter _asyncWriter;

        /// <summary>The _reader.</summary>
        private PgmReader _reader;

        /// <summary>The _receiver socket.</summary>
        private PgmReceiver _receiverSocket;

        /// <summary>The _send socket.</summary>
        private PgmSource _sendSocket;

        /// <summary>The _topic publisher.</summary>
        private TopicPublisher _topicPublisher;

        /// <summary>The _topic subscriber.</summary>
        private TopicSubscriber _topicSubscriber;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MessageBusService"/> class.</summary>
        /// <param name="aggregator">The aggregator.</param>
        public MessageBusService(IEventAggregator aggregator)
            : base(aggregator)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public override void Dispose()
        {
            base.Dispose();
            _topicSubscriber.Dispose();
            _receiverSocket.Dispose();
            _reader = null;
            _sendSocket.Dispose();
            _asyncWriter.Dispose();
            _topicPublisher = null;
        }

        /// <summary>The initialize.</summary>
        public override void Initialize()
        {
            base.Initialize();

            var address = ConfigurationManager.AppSettings["address"];
            if (string.IsNullOrEmpty(address))
            {
                throw new ApplicationException("address is not present.");
            }

            var normalizedAddress = address.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var msgParser = new MessageParserFactory();
            _reader = new PgmReader(msgParser);
            _receiverSocket = new PgmReceiver(normalizedAddress[0], int.Parse(normalizedAddress[1]), _reader);

            _topicSubscriber = new TopicSubscriber(".*", msgParser);
            _topicSubscriber.Start();
            _receiverSocket.Start();

            _sendSocket = new PgmSource(normalizedAddress[0], int.Parse(normalizedAddress[1]));
            _sendSocket.Start();
            _asyncWriter = new BatchWriter(_sendSocket, 1024 * 64);
            _topicPublisher = new TopicPublisher(_asyncWriter);
            _topicPublisher.Start();
        }

        /// <summary>The publish.</summary>
        /// <param name="data">The data.</param>
        /// <typeparam name="T">Any type</typeparam>
        public void Publish<T>(T data)
        {
            _topicPublisher.PublishObject(string.Empty, data, 1000);
        }

        /// <summary>The subscribe.</summary>
        /// <param name="subscribe">The subscribe.</param>
        /// <typeparam name="T">Any type</typeparam>
        public void Subscribe<T>(Action<T> subscribe)
        {
            _topicSubscriber.TopicMessageEvent +=
                parser =>
                subscribe((T)new ByteMessageParser(parser.Topic, parser.ParseBytes(), parser.EndPoint).ParseObject());
        }

        #endregion
    }
}