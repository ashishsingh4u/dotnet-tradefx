//  ===================================================================================
//  <copyright file="TechieNotesDelimitedFormatter.cs" company="TechieNotes">
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
//     The TechieNotesDelimitedFormatter.cs file.
//  </summary>
//  ===================================================================================

using System;

using TechieNotes.Common.Creation;
using TechieNotes.Common.Serialization;

namespace TradeFx.Common.Serialization
{
    /// <summary>The TechieNotesDelimitedSerialize interface.</summary>
    public interface ITechieNotesDelimitedSerialize : ICreatable
    {
        #region Public Methods and Operators

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        void Deserialize(TechieNotesDelimitedParser parser);

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        void Serialize(TechieNotesDelimitedBuilder builder);

        #endregion
    }

    /// <summary>The TechieNotesDelimitedFormatter interface.</summary>
    public interface ITechieNotesDelimitedFormatter
    {
        #region Public Methods and Operators

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        /// <returns>The System.Object.</returns>
        object Deserialize(TechieNotesDelimitedParser parser);

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="graph">The graph.</param>
        void Serialize(TechieNotesDelimitedBuilder builder, ITechieNotesDelimitedSerialize graph);

        #endregion
    }

    /// <summary>The techie notes delimited formatter.</summary>
    public class TechieNotesDelimitedFormatter : ITechieNotesDelimitedFormatter
    {
        #region Public Methods and Operators

        /// <summary>The deserialize.</summary>
        /// <param name="parser">The parser.</param>
        /// <returns>The System.Object.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public object Deserialize(TechieNotesDelimitedParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            return parser.GetObject();
        }

        /// <summary>The serialize.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="graph">The graph.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Serialize(TechieNotesDelimitedBuilder builder, ITechieNotesDelimitedSerialize graph)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            if (graph is ICreatable)
            {
                builder.Append(((ICreatable)graph).GetId());
            }

            graph.Serialize(builder);
        }

        #endregion
    }
}