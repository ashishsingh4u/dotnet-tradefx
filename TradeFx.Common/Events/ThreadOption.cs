//  ===================================================================================
//  <copyright file="ThreadOption.cs" company="TechieNotes">
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
//  <date>31-12-2012</date>
//  <summary>
//     The ThreadOption.cs file.
//  </summary>
//  ===================================================================================
namespace TradeFx.Common.Events
{
    /// <summary>
    ///     Specifies on which thread a <see cref="CompositePresentationEvent{TPayload}" /> subscriber will be called.
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        ///     The call is done on the same thread on which the <see cref="CompositePresentationEvent{TPayload}" /> was published.
        /// </summary>
        PublisherThread, 

        /// <summary>
        ///     The call is done on the UI thread.
        /// </summary>
        UIThread, 

        /// <summary>
        ///     The call is done asynchronously on a background thread.
        /// </summary>
        BackgroundThread
    }
}