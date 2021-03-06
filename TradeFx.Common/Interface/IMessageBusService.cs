//  ===================================================================================
//  <copyright file="IMessageBusService.cs" company="TechieNotes">
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
//     The IMessageBusService.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Interface
{
    /// <summary>The MessageBusService interface.</summary>
    public interface IMessageBusService : IService
    {
        #region Public Methods and Operators

        /// <summary>The publish.</summary>
        /// <param name="data">The data.</param>
        /// <typeparam name="T"></typeparam>
        void Publish<T>(T data);

        /// <summary>The subscribe.</summary>
        /// <param name="subscribe">The subscribe.</param>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(Action<T> subscribe);

        #endregion
    }
}