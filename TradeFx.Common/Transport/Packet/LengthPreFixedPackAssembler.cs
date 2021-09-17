//  ===================================================================================
//  <copyright file="LengthPreFixedPackAssembler.cs" company="TechieNotes">
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
//  <date>11-03-2013</date>
//  <summary>
//     The LengthPreFixedPackAssembler.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using TradeFx.Common.Transport.Network;

namespace TradeFx.Common.Transport.Packet
{
    /// <summary>
    ///     Encode data using the supplied encoding and frames by prefixing the data length
    /// </summary>
    public class LengthPreFixedPackAssembler : PacketAssembler
    {
        #region Constants

        /// <summary>
        ///     The number of bytes to encode the length
        /// </summary>
        private const int LENGTH_BYTE_COUNT = 4;

        #endregion

        #region Fields

        /// <summary>
        ///     The accumulated length
        /// </summary>
        private readonly HeaderBuffer _headerBuffer = new HeaderBuffer();

        /// <summary>The _sync lock.</summary>
        private readonly object _syncLock = new object();

        /// <summary>
        ///     The accumulated data
        /// </summary>
        private InputBuffer _buffer;

        #endregion

        // Utility class accumulating bytes from a stream 
        #region Public Methods and Operators

        /// <summary>The add.</summary>
        /// <param name="data">The data.</param>
        public void Add(byte[] data)
        {
            this.Add(data, 0, data.Length);
        }

        /// <summary>The add.</summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="ArgumentNullException">Throws if data is null.</exception>
        /// <exception cref="InvalidOperationException">Throws if data length is less than index.</exception>
        public override void Add(byte[] data, int index, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (index + length > data.Length)
            {
                throw new InvalidOperationException("array not big enough for index/length combination");
            }

            var packetEvents = new List<ReceiveEventArgs>();

            lock (this._syncLock)
            {
                while (length > 0)
                {
                    int bytesUsed;
                    if (!this._headerBuffer.Complete)
                    {
                        bytesUsed = this._headerBuffer.Write(data, index, length);
                    }
                    else
                    {
                        if (this._buffer == null)
                        {
                            this._buffer = new InputBuffer(this._headerBuffer.Length);
                        }

                        bytesUsed = this._buffer.Write(data, index, length);
                        if (this._buffer.Complete)
                        {
                            var packet = this._headerBuffer.Compressed
                                             ? Decompress(this._buffer.Data)
                                             : this._buffer.Data;
                            packetEvents.Add(new ReceiveEventArgs(packet, this._headerBuffer.PacketType));
                            this._buffer = null;
                            this._headerBuffer.Clear();
                        }
                    }

                    index += bytesUsed;
                    length -= bytesUsed;
                }
            }

            // raise events out of lock to prevent deadlock
            foreach (var packetEventArgs in packetEvents)
            {
                this.OnPacket(packetEventArgs);
            }
        }

        /// <summary>The make packet.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The System.Byte[].</returns>
        public byte[] MakePacket(byte[] data, PacketType packetType)
        {
            return this.MakePacket(data, packetType, false);
        }

        /// <summary>The make packet.</summary>
        /// <param name="data">The data.</param>
        /// <param name="packetType">The packet type.</param>
        /// <param name="compress">The compress.</param>
        /// <returns>The System.Byte[].</returns>
        public override byte[] MakePacket(byte[] data, PacketType packetType, bool compress)
        {
            if (compress)
            {
                data = Compress(data);
            }

            var packet = new byte[HeaderBuffer.HEADER_LENGTH + data.Length];
            HeaderBuffer.WriteHeader(packet, 0, data.Length, packetType, 1, compress);
            Buffer.BlockCopy(data, 0, packet, HeaderBuffer.HEADER_LENGTH, data.Length);
            return packet;
        }

        /// <summary>The reset.</summary>
        public override void Reset()
        {
            lock (this._syncLock)
            {
                this._headerBuffer.Clear();
                this._buffer = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>The compress.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The System.Byte[].</returns>
        private static byte[] Compress(byte[] data)
        {
            var memoryStream = new MemoryStream();
            var zipper = new GZipStream(memoryStream, CompressionMode.Compress, true);
            zipper.Write(data, 0, data.Length);
            zipper.Close(); // Very important
            var ret = memoryStream.ToArray();
            return ret;
        }

        /// <summary>The decompress.</summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <returns>The System.Byte[].</returns>
        private static byte[] Decompress(byte[] compressedData)
        {
            var compressedStream = new MemoryStream(compressedData);
            var zipper = new GZipStream(compressedStream, CompressionMode.Decompress);

            int size;
            var buffer = new byte[1024];
            var memoryStream = new MemoryStream();
            do
            {
                size = zipper.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, size);
            }
            while (size > 0);

            return memoryStream.ToArray();
        }

        #endregion

        /// <summary>The header buffer.</summary>
        private class HeaderBuffer : InputBuffer
        {
            #region Constants

            /// <summary>The header length.</summary>
            public const int HEADER_LENGTH = FLAGS_OFFSET + 1;

            /// <summary>The flags offset.</summary>
            private const int FLAGS_OFFSET = VERSION_OFFSET + 1;

            /// <summary>The packet type offset.</summary>
            private const int PACKET_TYPE_OFFSET = LENGTH_BYTE_COUNT;

            /// <summary>The version offset.</summary>
            private const int VERSION_OFFSET = PACKET_TYPE_OFFSET + 1;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="HeaderBuffer" /> class.
            /// </summary>
            public HeaderBuffer()
                : base(HEADER_LENGTH)
            {
            }

            #endregion

            #region Enums

            /// <summary>The flag byte values.</summary>
            [Flags]
            private enum FlagByteValues : byte
            {
                /// <summary>The none.</summary>
                None = 0, 

                /// <summary>The compressed.</summary>
                Compressed = 1
            }

            #endregion

            #region Public Properties

            /// <summary>Gets a value indicating whether compressed.</summary>
            public bool Compressed
            {
                get
                {
                    var flags = (FlagByteValues)this.Data[FLAGS_OFFSET];
                    return (flags & FlagByteValues.Compressed) == FlagByteValues.Compressed;
                }
            }

            /// <summary>Gets the length.</summary>
            public int Length
            {
                get
                {
                    var retval = 0;
                    var shift = 0;
                    for (var i = 0; i < LENGTH_BYTE_COUNT; i++)
                    {
                        retval += this.Data[i] << shift;
                        shift += 8;
                    }

                    return retval;
                }
            }

            /// <summary>Gets the packet type.</summary>
            public PacketType PacketType
            {
                get
                {
                    return (PacketType)this.Data[PACKET_TYPE_OFFSET];
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The write header.</summary>
            /// <param name="array">The array.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="length">The length.</param>
            /// <param name="packetType">The packet type.</param>
            /// <param name="version">The version.</param>
            /// <param name="compressed">The compressed.</param>
            public static void WriteHeader(
                byte[] array, int offset, int length, PacketType packetType, byte version, bool compressed)
            {
                for (var idx = offset; idx < offset + LENGTH_BYTE_COUNT; idx++)
                {
                    array[idx] = (byte)((length >> (idx * 8)) & 0xff);
                }

                array[offset + PACKET_TYPE_OFFSET] = (byte)packetType;
                array[offset + VERSION_OFFSET] = version;
                var flags = FlagByteValues.None;
                flags |= compressed ? FlagByteValues.Compressed : FlagByteValues.None;
                array[offset + FLAGS_OFFSET] = (byte)flags;
            }

            #endregion
        }

        /// <summary>The input buffer.</summary>
        private class InputBuffer
        {
            #region Fields

            /// <summary>The _buffer data.</summary>
            private readonly byte[] _bufferData;

            /// <summary>The _buffer index.</summary>
            private int _bufferIndex;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="InputBuffer"/> class.</summary>
            /// <param name="size">The size.</param>
            public InputBuffer(int size)
            {
                this._bufferData = new byte[size];
            }

            #endregion

            #region Public Properties

            /// <summary>Gets a value indicating whether complete.</summary>
            public bool Complete
            {
                get
                {
                    return this._bufferIndex == this._bufferData.Length;
                }
            }

            /// <summary>Gets the data.</summary>
            public byte[] Data
            {
                get
                {
                    return this._bufferData;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The clear.</summary>
            public void Clear()
            {
                this._bufferIndex = 0;
            }

            // Writes data to the buffer

            /// <summary>The write.</summary>
            /// <param name="data">The data.</param>
            /// <param name="index">The index.</param>
            /// <param name="length">The length.</param>
            /// <returns>The System.Int32.</returns>
            public int Write(byte[] data, int index, int length)
            {
                var thisRead = Math.Min(this._bufferData.Length - this._bufferIndex, length);
                Buffer.BlockCopy(data, index, this._bufferData, this._bufferIndex, thisRead);
                this._bufferIndex += thisRead;
                return thisRead;
            }

            #endregion
        }
    }
}