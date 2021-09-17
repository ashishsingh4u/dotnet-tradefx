//  ===================================================================================
//  <copyright file="ObjectId.cs" company="TechieNotes">
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
//     The ObjectId.cs file.
//  </summary>
//  ===================================================================================
namespace TradeFx.Common.Creation
{
    /// <summary>The object id.</summary>
    public enum ObjectId
    {
        /// <summary>The diagnostic state update.</summary>
        DiagnosticStateUpdate, // +

        /// <summary>The hbt.</summary>
        Hbt, // +

        /// <summary>The hbt response.</summary>
        HbtResponse, // +

        /// <summary>The server side hbt request.</summary>
        ServerSideHbtRequest, 

        /// <summary>The server side hbt.</summary>
        ServerSideHbt, 

        /// <summary>The server side hbt response.</summary>
        ServerSideHbtResponse, 

        /// <summary>The server side hbt result.</summary>
        ServerSideHbtResult, 

        /// <summary>The raw data.</summary>
        RawData, // -

        /// <summary>The identity.</summary>
        Identity, // -

        /// <summary>The log data.</summary>
        LogData // +
    }
}