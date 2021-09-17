//  ===================================================================================
//  <copyright file="ISendable.cs" company="TechieNotes">
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
//     The ISendable.cs file.
//  </summary>
//  ===================================================================================
namespace TradeFx.Common.Transport.Packet
{
    /// <summary>
    ///     Sendable objects can be "send out" by managers
    /// </summary>
    /// <remarks>ISendable implies IConflatable to enforce the decision about object or class ability to use conflation</remarks>
    public interface ISendable : IConflatable
    {
        #region Public Properties

        /// <summary>
        ///     Gets Packatetype.
        ///     Defines the type of packet that should be used for serialization of this object
        /// </summary>
        PacketType PacketType { get; }

        #endregion
    }
}