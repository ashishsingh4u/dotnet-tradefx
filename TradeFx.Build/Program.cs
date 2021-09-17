//  ===================================================================================
//  <copyright file="Program.cs" company="TechieNotes">
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
//  <date>19-01-2013</date>
//  <summary>
//     The Program.cs file.
//  </summary>
//  ===================================================================================

using System.IO;
using System.Xml.Linq;

using Microsoft.VisualStudio.Coverage.Analysis;

namespace TradeFx.Build
{
    /// <summary>The program.</summary>
    internal class Program
    {
        #region Methods

        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            var directoryInfo = new DirectoryInfo(args[0]);
            var coverageFiles = directoryInfo.GetFiles("*.coverage", SearchOption.AllDirectories);
            var unitTestReports = directoryInfo.GetFiles("*.trx", SearchOption.AllDirectories);
            using (var coverageInfo = CoverageInfo.CreateFromFile(coverageFiles[0].FullName))
            {
                using (var dataSet = coverageInfo.BuildDataSet(null))
                {
                    var document = XDocument.Parse(dataSet.GetXml());
                    document.Save(string.Concat(args[0], "\\coverage.trx"));
                    unitTestReports[0].CopyTo(string.Concat(args[0], "\\results.trx"));
                }
            }
        }

        #endregion
    }
}