//  ===================================================================================
//  <copyright file="TechieObjectFactory.cs" company="TechieNotes">
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
//     The TechieObjectFactory.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using log4net;

using TechieNotes.Common.Serialization;
using TechieNotes.Common.Serialization.Interfaces;

using TradeFx.Common.Serialization.Interfaces;
using TradeFx.Common.Transport.Packet;

namespace TradeFx.Common.Serialization
{
    /// <summary>The techie object factory.</summary>
    public static class TechieObjectFactory
    {
        #region Constants

        /// <summary>The confirm fdc.</summary>
        public const string CONFIRM_FDC = "CONFIRM_FDC";

        /// <summary>The keep alive command.</summary>
        public const string KEEP_ALIVE_COMMAND = "KeepAlive";

        /// <summary>The subscription command.</summary>
        public const string SUBSCRIPTION_COMMAND = "Subscription";

        #endregion

        #region Static Fields

        /// <summary>The _factories.</summary>
        private static readonly List<IDeserializationFactory> Factories = new List<IDeserializationFactory>();

        /// <summary>The log.</summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods and Operators

        /// <summary>The add factory.</summary>
        /// <param name="factory">The factory.</param>
        public static void AddFactory(IDeserializationFactory factory)
        {
            lock (Factories) Factories.Add(factory);
        }

        /// <summary>The clone.</summary>
        /// <param name="source">The source.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The T.</returns>
        public static T Clone<T>(T source) where T : ISendable
        {
            var bytes = Serialize(source, source.PacketType);
            return (T)Deserialize(bytes, source.PacketType);
        }

        /// <summary>The convert bytes to string.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.String.</returns>
        public static string ConvertBytesToString(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append(Encoding.UTF8.GetString(data));
            return sb.ToString();
        }

        /// <summary>The deserialize.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static object Deserialize(byte[] data, PacketType packetType)
        {
            switch (packetType)
            {
                case PacketType.BinaryFormatter:
                    using (var stream = new MemoryStream(data))
                    {
                        var formatter = new BinaryFormatter();

                        return formatter.Deserialize(stream);
                    }

                case PacketType.BinaryFormatterCompressed:
                    using (var memoryStream = new MemoryStream(data))
                    {
                        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                        {
                            var formatter = new BinaryFormatter();

                            return formatter.Deserialize(gzipStream);
                        }
                    }

                case PacketType.SoapFormatter:
                    {
                        // Not implemented as it creates dependency on System.Runtime.Serialization.Formatters.Soap.dll assembly 
                        throw new NotImplementedException();
                    }

                case PacketType.TechieNotesDelimited:
                    using (var parser = new TechieNotesDelimitedParser(data))
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("TechieNotesDelimited Deserialize: {0}", parser.ToLog());
                        }

                        ITechieNotesDelimitedFormatter formatter = new TechieNotesDelimitedFormatter();
                        return formatter.Deserialize(parser);
                    }

                case PacketType.BinaryStreamFormatter:
                    using (var reader = new BinaryStreamReader(data))
                    {
                        var messageType = (MessageTypeIds)reader.ReadInt();
                        foreach (var factory in Factories)
                        {
                            var message = factory.Deserialize(messageType, reader);
                            if (message != null)
                            {
                                return message;
                            }
                        }
                    }

                    throw new InvalidDataException("Unknown object received; no factory could deserialize it");

                default:
                    Debug.Assert(false, "Unknown packet type: " + packetType);
                    break;
            }

            return null;
        }

        /// <summary>The deserialize.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The System.Object.</returns>
        public static object Deserialize<T>(byte[] data, PacketType packetType)
        {
            var defaultObj = default(T);
            switch (packetType)
            {
                case PacketType.BinaryFormatter:
                case PacketType.SoapFormatter:
                case PacketType.SqlXmlRaw:
                case PacketType.TechieNotesDelimited:
                case PacketType.BinaryStreamFormatter:
                    {
                        Deserialize(data, packetType);
                        break;
                    }

                default:
                    Debug.Assert(false, "Unknown packet type: " + packetType);
                    break;
            }

            return defaultObj;
        }

        /// <summary>The remove factory.</summary>
        /// <param name="factory">The factory.</param>
        public static void RemoveFactory(IDeserializationFactory factory)
        {
            lock (Factories) Factories.Remove(factory);
        }

        /// <summary>The serialize.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Byte[].</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] Serialize(object obj, PacketType packetType)
        {
            switch (packetType)
            {
                case PacketType.BinaryFormatter:
                    using (var stream = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, obj);

                        return stream.ToArray();
                    }

                case PacketType.BinaryFormatterCompressed:
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(gzipStream, obj);
                            gzipStream.Close();

                            return memoryStream.ToArray();
                        }
                    }

                case PacketType.SoapFormatter:
                    {
                        // Not implemented as it creates dependency on System.Runtime.Serialization.Formatters.Soap.dll assembly 
                        throw new NotImplementedException();
                    }

                case PacketType.SqlXmlRaw:
                    throw new InvalidOperationException(
                        string.Format("{0} formatter can't be used to serialize", packetType));

                case PacketType.TechieNotesDelimited:
                    using (var builder = new TechieNotesDelimitedBuilder())
                    {
                        ITechieNotesDelimitedFormatter formatter = new TechieNotesDelimitedFormatter();
                        formatter.Serialize(builder, (ITechieNotesDelimitedSerialize)obj);

                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("TechieNotesDelimited Serialize: {0}", builder.ToLog());
                        }

                        return builder.ToArray();
                    }

                case PacketType.BinaryStreamFormatter:

                    var sendable = obj as ISendable;
                    if (sendable == null)
                    {
                        throw new ArgumentException(
                            "Expected to receive object type ISendable. Please check type: " + obj.GetType().Name);
                    }

                    var message = obj as IStreamSerializable;
                    if (message == null)
                    {
                        throw new ArgumentException(
                            "Expected to receive object type IStreamSerializable. Please check type: "
                            + obj.GetType().Name);
                    }

                    var messageId = obj as IMessageTypeIdentifier;
                    if (messageId == null)
                    {
                        throw new ArgumentException(
                            "Expected to receive object type IMessageTypeIdentifier. Please check type: "
                            + obj.GetType().Name);
                    }

                    using (IStreamWriter writer = new BinaryStreamWriter())
                    {
                        writer.Write((int)messageId.MessageTypeId);
                        message.Serialize(writer);
                        return writer.GetBytes();
                    }

                default:
                    Debug.Assert(false, "Unknown packet type: " + packetType);
                    break;
            }

            return new byte[0];
        }

        /// <summary>Meant for xmlSerialization.</summary>
        /// <param name="obj"></param>
        /// <param name="packetType"></param>
        /// <param name="commandType"></param>
        /// <returns>The System.Byte[].</returns>
        public static byte[] Serialize<T>(T obj, PacketType packetType, string commandType)
        {
            switch (packetType)
            {
                case PacketType.BinaryFormatter:
                case PacketType.SoapFormatter:
                case PacketType.SqlXmlRaw:
                case PacketType.TechieNotesDelimited:
                case PacketType.BinaryStreamFormatter:
                    {
                        Serialize(obj, packetType);
                        break;
                    }

                default:
                    Debug.Assert(false, "Unknown packet type: " + packetType);
                    break;
            }

            return new byte[0];
        }

        #endregion
    }
}