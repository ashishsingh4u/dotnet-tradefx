//  ===================================================================================
//  <copyright file="DataEventArgs.cs" company="TechieNotes">
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
//     The DataEventArgs.cs file.
//  </summary>
//  ===================================================================================

using System;

namespace TradeFx.Common.Events
{
    /// <summary>Generic arguments class to pass to event handlers that need to receive data.</summary>
    /// <typeparam name="TData">The type of data to pass.</typeparam>
    public class DataEventArgs<TData> : EventArgs
    {
        #region Fields

        /// <summary>The _value.</summary>
        private readonly TData _value;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DataEventArgs{TData}"/> class. Initializes the DataEventArgs class.</summary>
        /// <param name="value">Information related to the event.</param>
        public DataEventArgs(TData value)
        {
            _value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the information related to the event.
        /// </summary>
        /// <value>Information related to the event.</value>
        public TData Value
        {
            get
            {
                return _value;
            }
        }

        #endregion
    }
}