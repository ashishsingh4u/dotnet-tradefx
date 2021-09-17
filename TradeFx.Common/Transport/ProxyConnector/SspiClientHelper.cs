//  ===================================================================================
//  <copyright file="SspiClientHelper.cs" company="TechieNotes">
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
//     The SspiClientHelper.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Net;
using System.Runtime.InteropServices;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>
    ///     Helper class providing access to the Win32 SSPI api.
    /// </summary>
    internal class SspiClientHelper : IDisposable
    {
        // SSPI direction values
        #region Constants

        /// <summary>The is c_ re q_ allocat e_ memory.</summary>
        private const uint ISC_REQ_ALLOCATE_MEMORY = 0x00000100;

        /// <summary>The is c_ re q_ cal l_ level.</summary>
        private const uint ISC_REQ_CALL_LEVEL = 0x00001000;

        /// <summary>The is c_ re q_ confidentiality.</summary>
        private const uint ISC_REQ_CONFIDENTIALITY = 0x00000010;

        /// <summary>The is c_ re q_ connection.</summary>
        private const uint ISC_REQ_CONNECTION = 0x00000800;

        /// <summary>The is c_ re q_ datagram.</summary>
        private const uint ISC_REQ_DATAGRAM = 0x00000400;

        /// <summary>The is c_ re q_ delegate.</summary>
        private const uint ISC_REQ_DELEGATE = 0x00000001;

        /// <summary>The is c_ re q_ extende d_ error.</summary>
        private const uint ISC_REQ_EXTENDED_ERROR = 0x00004000;

        /// <summary>The is c_ re q_ fragmen t_ supplied.</summary>
        private const uint ISC_REQ_FRAGMENT_SUPPLIED = 0x00002000;

        /// <summary>The is c_ re q_ fragmen t_ t o_ fit.</summary>
        private const uint ISC_REQ_FRAGMENT_TO_FIT = 0x00200000;

        /// <summary>The is c_ re q_ identify.</summary>
        private const uint ISC_REQ_IDENTIFY = 0x00020000;

        /// <summary>The is c_ re q_ ini t_ http.</summary>
        private const uint ISC_REQ_INIT_HTTP = 0x10000000;

        /// <summary>The is c_ re q_ integrity.</summary>
        private const uint ISC_REQ_INTEGRITY = 0x00010000;

        /// <summary>The is c_ re q_ manua l_ cre d_ validation.</summary>
        private const uint ISC_REQ_MANUAL_CRED_VALIDATION = 0x00080000;

        /// <summary>The is c_ re q_ mutua l_ auth.</summary>
        private const uint ISC_REQ_MUTUAL_AUTH = 0x00000002;

        /// <summary>The is c_ re q_ nul l_ session.</summary>
        private const uint ISC_REQ_NULL_SESSION = 0x00040000;

        /// <summary>The is c_ re q_ promp t_ fo r_ creds.</summary>
        private const uint ISC_REQ_PROMPT_FOR_CREDS = 0x00000040;

        /// <summary>The is c_ re q_ repla y_ detect.</summary>
        private const uint ISC_REQ_REPLAY_DETECT = 0x00000004;

        /// <summary>The is c_ re q_ reserve d 1.</summary>
        private const uint ISC_REQ_RESERVED1 = 0x00100000;

        /// <summary>The is c_ re q_ sequenc e_ detect.</summary>
        private const uint ISC_REQ_SEQUENCE_DETECT = 0x00000008;

        /// <summary>The is c_ re q_ stream.</summary>
        private const uint ISC_REQ_STREAM = 0x00008000;

        /// <summary>The is c_ re q_ us e_ dc e_ style.</summary>
        private const uint ISC_REQ_USE_DCE_STYLE = 0x00000200;

        /// <summary>The is c_ re q_ us e_ sessio n_ key.</summary>
        private const uint ISC_REQ_USE_SESSION_KEY = 0x00000020;

        /// <summary>The is c_ re q_ us e_ supplie d_ creds.</summary>
        private const uint ISC_REQ_USE_SUPPLIED_CREDS = 0x00000080;

        /// <summary>The secpk g_ cre d_ outbound.</summary>
        private const uint SECPKG_CRED_OUTBOUND = 2;

        /// <summary>The securit y_ nativ e_ drep.</summary>
        private const uint SECURITY_NATIVE_DREP = 0x00000010;

        // Return values
        /// <summary>The se c_ e_ ok.</summary>
        private const uint SEC_E_OK = 0;

        /// <summary>The se c_ i_ continu e_ needed.</summary>
        private const uint SEC_I_CONTINUE_NEEDED = 0x90312;

        /// <summary>The standar d_ contex t_ attributes.</summary>
        private const uint STANDARD_CONTEXT_ATTRIBUTES =
            ISC_REQ_CONFIDENTIALITY | ISC_REQ_REPLAY_DETECT | ISC_REQ_SEQUENCE_DETECT | ISC_REQ_CONNECTION;

        // Misc constants
        /// <summary>The toke n_ buffe r_ size.</summary>
        private const uint TOKEN_BUFFER_SIZE = 12288;

        #endregion

        #region Fields

        /// <summary>The _package name.</summary>
        private readonly string _packageName;

        /// <summary>The _context handle.</summary>
        private SecHandle _contextHandle;

        /// <summary>The _credential handle.</summary>
        private SecHandle _credentialHandle;

        /// <summary>The _is initialized.</summary>
        private bool _isInitialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SspiClientHelper"/> class. Initializes an instance of the SspiClientHelper class.</summary>
        /// <param name="packageName">Name of the security package.</param>
        public SspiClientHelper(string packageName)
        {
            this._packageName = packageName;
        }

        #endregion

        #region Enums

        /// <summary>The sec buffer type.</summary>
        private enum SecBufferType : uint
        {
            /// <summary>The version.</summary>
            Version = 0, 

            /// <summary>The data.</summary>
            Data = 1, 

            /// <summary>The token.</summary>
            Token = 2, 

            /// <summary>The parameters.</summary>
            Parameters = 3
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating if the Api has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return this._isInitialized;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets a value indicating if the named package is supported.</summary>
        /// <param name="packageName">Name of the package to check.</param>
        /// <returns>True if supported, false if not supported.</returns>
        public static bool IsSupported(string packageName)
        {
            uint packageCount;
            IntPtr packagePtr;

            var sspiResult = EnumerateSecurityPackages(out packageCount, out packagePtr);
            if (sspiResult != SEC_E_OK)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SspiError, sspiResult);
            }

            try
            {
                var offsetPtr = new OffsetIntPtr(packagePtr, SecPkgInfo.StructSize);
                for (var index = 0; index < packageCount; index++)
                {
                    var pkg = (SecPkgInfo)Marshal.PtrToStructure(offsetPtr.GetNextPtr(), typeof(SecPkgInfo));
                    if (packageName.Equals(pkg.NameString, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                FreeContextBuffer(packagePtr);
            }
        }

        /// <summary>
        ///     Disposes the SspiClientHelper instance.
        /// </summary>
        public void Dispose()
        {
            if (!this._credentialHandle.IsZero)
            {
                FreeCredentialsHandle(ref this._credentialHandle);
                this._credentialHandle.SetZero();
            }

            if (!this._contextHandle.IsZero)
            {
                DeleteSecurityContext(ref this._contextHandle);
                this._contextHandle.SetZero();
            }
        }

        /// <summary>Gets a response token to the inToken value. This method is specific to Digest authentication.</summary>
        /// <param name="inBlob">In blob is attribute pairs received as the challenge from the proxy.</param>
        /// <param name="requestMethod">Request method is the HTTP request method from the proxy.</param>
        /// <param name="uri">Uri requested from the proxy.</param>
        /// <returns>Out token.</returns>
        public byte[] GetDigestResponseToken(string inBlob, string requestMethod, string uri)
        {
            // Cant use 'using' to declare as outBuffer passed by ref to Win32 API
            var outBuffer = new SecBufferDesc(TOKEN_BUFFER_SIZE);
            try
            {
                var bufferArray = new[]
                                      {
                                          new SecBuffer(ProxyEncoding.Default.GetBytes(inBlob), SecBufferType.Token), 
                                          new SecBuffer(
                                              ProxyEncoding.Default.GetBytes(requestMethod), SecBufferType.Parameters), 
                                          new SecBuffer(new byte[] { }, SecBufferType.Parameters)
                                      };

                // Cant use 'using' to declare as inBuffer passed by ref to Win32 API
                var inBuffer = new SecBufferDesc(bufferArray);
                try
                {
                    var digestContextAttributes = (STANDARD_CONTEXT_ATTRIBUTES | ISC_REQ_INIT_HTTP)
                                                  & ~ISC_REQ_CONFIDENTIALITY;
                    uint contextAttributes;
                    TimeStamp expiry;

                    var sspiResult = InitializeSecurityContext(
                        ref this._credentialHandle, 
                        ref this._contextHandle, 
                        uri, 
                        digestContextAttributes, 
                        0, 
                        SECURITY_NATIVE_DREP, 
                        ref inBuffer, 
                        0, 
                        out this._contextHandle, 
                        ref outBuffer, 
                        out contextAttributes, 
                        out expiry);

                    if (sspiResult != SEC_E_OK && sspiResult != SEC_I_CONTINUE_NEEDED)
                    {
                        throw new SocketConnectorException(SocketConnectorErrorType.SspiError, sspiResult);
                    }

                    return outBuffer.GetSecBufferByteArray();
                }
                finally
                {
                    inBuffer.Dispose();
                }
            }
            finally
            {
                outBuffer.Dispose();
            }
        }

        /// <summary>Gets a response token to the inToken value.</summary>
        /// <param name="inToken">In token value received as challenge from the proxy server.  If null initial out token is generated.</param>
        /// <returns>Out token.</returns>
        public byte[] GetResponseToken(byte[] inToken)
        {
            // Cant use 'using' to declare as outBuffer passed by ref to Win32 API
            var outBuffer = new SecBufferDesc(TOKEN_BUFFER_SIZE);
            try
            {
                int sspiResult;
                uint contextAttributes;
                TimeStamp expiry;

                if (inToken != null)
                {
                    // Cant use 'using' to declare as inBuffer passed by ref to Win32 API
                    var inBuffer = new SecBufferDesc(inToken);
                    try
                    {
                        sspiResult = InitializeSecurityContext(
                            ref this._credentialHandle, 
                            ref this._contextHandle, 
                            null, 
                            STANDARD_CONTEXT_ATTRIBUTES, 
                            0, 
                            SECURITY_NATIVE_DREP, 
                            ref inBuffer, 
                            0, 
                            out this._contextHandle, 
                            ref outBuffer, 
                            out contextAttributes, 
                            out expiry);
                    }
                    finally
                    {
                        inBuffer.Dispose();
                    }
                }
                else
                {
                    sspiResult = InitializeSecurityContext(
                        ref this._credentialHandle, 
                        IntPtr.Zero, 
                        null, 
                        STANDARD_CONTEXT_ATTRIBUTES, 
                        0, 
                        SECURITY_NATIVE_DREP, 
                        IntPtr.Zero, 
                        0, 
                        out this._contextHandle, 
                        ref outBuffer, 
                        out contextAttributes, 
                        out expiry);
                }

                if (sspiResult != SEC_E_OK && sspiResult != SEC_I_CONTINUE_NEEDED)
                {
                    throw new SocketConnectorException(SocketConnectorErrorType.SspiError, sspiResult);
                }

                return outBuffer.GetSecBufferByteArray();
            }
            finally
            {
                outBuffer.Dispose();
            }
        }

        /// <summary>Initialized the SSPI api.</summary>
        /// <param name="networkCredential">Credential to use for authentication. If null credential of current user is used.</param>
        public void Initialize(NetworkCredential networkCredential)
        {
            int sspiResult;
            TimeStamp expiry;

            if (networkCredential != null)
            {
                var identity = new SecWinntAuthIdentity(networkCredential);

                sspiResult = AcquireCredentialsHandle(
                    null, 
                    this._packageName, 
                    SECPKG_CRED_OUTBOUND, 
                    IntPtr.Zero, 
                    ref identity, 
                    0, 
                    IntPtr.Zero, 
                    out this._credentialHandle, 
                    out expiry);
            }
            else
            {
                sspiResult = AcquireCredentialsHandle(
                    null, 
                    this._packageName, 
                    SECPKG_CRED_OUTBOUND, 
                    IntPtr.Zero, 
                    IntPtr.Zero, 
                    0, 
                    IntPtr.Zero, 
                    out this._credentialHandle, 
                    out expiry);
            }

            if (sspiResult != SEC_E_OK)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SspiError, sspiResult);
            }

            this._isInitialized = true;
        }

        #endregion

        #region Methods

        /// <summary>The acquire credentials handle.</summary>
        /// <param name="pszPrincipal">The psz principal.</param>
        /// <param name="pszPackage">The psz package.</param>
        /// <param name="fCredentialUse">The f credential use.</param>
        /// <param name="PAuthenticationID">The p authentication id.</param>
        /// <param name="pAuthData">The p auth data.</param>
        /// <param name="pGetKeyFn">The p get key fn.</param>
        /// <param name="pvGetKeyArgument">The pv get key argument.</param>
        /// <param name="phCredential">The ph credential.</param>
        /// <param name="ptsExpiry">The pts expiry.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int AcquireCredentialsHandle(
            string pszPrincipal, 
            string pszPackage, 
            uint fCredentialUse, 
            IntPtr PAuthenticationID, 
            IntPtr pAuthData, 
            int pGetKeyFn, 
            IntPtr pvGetKeyArgument, 
            out SecHandle phCredential, 
            out TimeStamp ptsExpiry);

        /// <summary>The acquire credentials handle.</summary>
        /// <param name="pszPrincipal">The psz principal.</param>
        /// <param name="pszPackage">The psz package.</param>
        /// <param name="fCredentialUse">The f credential use.</param>
        /// <param name="PAuthenticationID">The p authentication id.</param>
        /// <param name="pAuthData">The p auth data.</param>
        /// <param name="pGetKeyFn">The p get key fn.</param>
        /// <param name="pvGetKeyArgument">The pv get key argument.</param>
        /// <param name="phCredential">The ph credential.</param>
        /// <param name="ptsExpiry">The pts expiry.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int AcquireCredentialsHandle(
            string pszPrincipal, 
            string pszPackage, 
            uint fCredentialUse, 
            IntPtr PAuthenticationID, 
            ref SecWinntAuthIdentity pAuthData, 
            int pGetKeyFn, 
            IntPtr pvGetKeyArgument, 
            out SecHandle phCredential, 
            out TimeStamp ptsExpiry);

        /// <summary>The delete security context.</summary>
        /// <param name="phContext">The ph context.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int DeleteSecurityContext(ref SecHandle phContext);

        /// <summary>The enumerate security packages.</summary>
        /// <param name="pcPackages">The pc packages.</param>
        /// <param name="packagePtr">The package ptr.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int EnumerateSecurityPackages(out uint pcPackages, out IntPtr packagePtr);

        /// <summary>The free context buffer.</summary>
        /// <param name="pvContextBuffer">The pv context buffer.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int FreeContextBuffer(IntPtr pvContextBuffer);

        /// <summary>The free credentials handle.</summary>
        /// <param name="phCredential">The ph credential.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int FreeCredentialsHandle(ref SecHandle phCredential);

        /// <summary>The initialize security context.</summary>
        /// <param name="phCredential">The ph credential.</param>
        /// <param name="phContext">The ph context.</param>
        /// <param name="pszTargetName">The psz target name.</param>
        /// <param name="fContextReq">The f context req.</param>
        /// <param name="Reserved1">The reserved 1.</param>
        /// <param name="TargetDataRep">The target data rep.</param>
        /// <param name="pInput">The p input.</param>
        /// <param name="Reserved2">The reserved 2.</param>
        /// <param name="phNewContext">The ph new context.</param>
        /// <param name="pOutput">The p output.</param>
        /// <param name="pfContextAttr">The pf context attr.</param>
        /// <param name="ptsExpiry">The pts expiry.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int InitializeSecurityContext(
            ref SecHandle phCredential, 
            IntPtr phContext, 
            string pszTargetName, 
            uint fContextReq, 
            uint Reserved1, 
            uint TargetDataRep, 
            IntPtr pInput, 
            uint Reserved2, 
            out SecHandle phNewContext, 
            ref SecBufferDesc pOutput, 
            out uint pfContextAttr, 
            out TimeStamp ptsExpiry);

        /// <summary>The initialize security context.</summary>
        /// <param name="phCredential">The ph credential.</param>
        /// <param name="phContext">The ph context.</param>
        /// <param name="pszTargetName">The psz target name.</param>
        /// <param name="fContextReq">The f context req.</param>
        /// <param name="Reserved1">The reserved 1.</param>
        /// <param name="TargetDataRep">The target data rep.</param>
        /// <param name="pInput">The p input.</param>
        /// <param name="Reserved2">The reserved 2.</param>
        /// <param name="phNewContext">The ph new context.</param>
        /// <param name="pOutput">The p output.</param>
        /// <param name="pfContextAttr">The pf context attr.</param>
        /// <param name="ptsExpiry">The pts expiry.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int InitializeSecurityContext(
            ref SecHandle phCredential, 
            ref SecHandle phContext, 
            string pszTargetName, 
            uint fContextReq, 
            uint Reserved1, 
            uint TargetDataRep, 
            ref SecBufferDesc pInput, 
            uint Reserved2, 
            out SecHandle phNewContext, 
            ref SecBufferDesc pOutput, 
            out uint pfContextAttr, 
            out TimeStamp ptsExpiry);

        #endregion

        /// <summary>The sec buffer.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SecBuffer : IDisposable
        {
            /// <summary>The struct size.</summary>
            public static readonly int StructSize = Marshal.SizeOf(typeof(SecBuffer));

            /// <summary>The cb buffer.</summary>
            public readonly uint cbBuffer;

            /// <summary>The buffer type.</summary>
            public readonly SecBufferType BufferType;

            /// <summary>The pv buffer.</summary>
            public IntPtr pvBuffer;

            /// <summary>Initializes a new instance of the <see cref="SecBuffer"/> struct.</summary>
            /// <param name="bufferSize">The buffer size.</param>
            public SecBuffer(uint bufferSize)
            {
                this.cbBuffer = bufferSize;
                this.BufferType = SecBufferType.Token;
                this.pvBuffer = Marshal.AllocHGlobal((int)this.cbBuffer);
            }

            /// <summary>Initializes a new instance of the <see cref="SecBuffer"/> struct.</summary>
            /// <param name="secBufferBytes">The sec buffer bytes.</param>
            public SecBuffer(byte[] secBufferBytes)
                : this(secBufferBytes, SecBufferType.Token)
            {
            }

            /// <summary>Initializes a new instance of the <see cref="SecBuffer"/> struct.</summary>
            /// <param name="secBufferBytes">The sec buffer bytes.</param>
            /// <param name="bufferType">The buffer type.</param>
            public SecBuffer(byte[] secBufferBytes, SecBufferType bufferType)
            {
                this.cbBuffer = (uint)secBufferBytes.Length;
                this.BufferType = bufferType;
                this.pvBuffer = Marshal.AllocHGlobal((int)this.cbBuffer);
                Marshal.Copy(secBufferBytes, 0, this.pvBuffer, (int)this.cbBuffer);
            }

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                if (this.pvBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.pvBuffer);
                    this.pvBuffer = IntPtr.Zero;
                }
            }
        }

        /// <summary>The sec buffer desc.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SecBufferDesc : IDisposable
        {
            /// <summary>The ul version.</summary>
            public readonly uint ulVersion;

            /// <summary>The c buffers.</summary>
            public readonly uint cBuffers;

            /// <summary>The p buffers.</summary>
            public IntPtr pBuffers;

            /// <summary>Initializes a new instance of the <see cref="SecBufferDesc"/> struct.</summary>
            /// <param name="bufferSize">The buffer size.</param>
            public SecBufferDesc(uint bufferSize)
                : this(new SecBuffer(bufferSize))
            {
            }

            /// <summary>Initializes a new instance of the <see cref="SecBufferDesc"/> struct.</summary>
            /// <param name="secBufferBytes">The sec buffer bytes.</param>
            public SecBufferDesc(byte[] secBufferBytes)
                : this(new SecBuffer(secBufferBytes))
            {
            }

            /// <summary>Initializes a new instance of the <see cref="SecBufferDesc"/> struct.</summary>
            /// <param name="secBuffer">The sec buffer.</param>
            public SecBufferDesc(SecBuffer secBuffer)
            {
                this.ulVersion = (uint)SecBufferType.Version;
                this.cBuffers = 1;
                this.pBuffers = Marshal.AllocHGlobal(SecBuffer.StructSize);
                Marshal.StructureToPtr(secBuffer, this.pBuffers, false);
            }

            /// <summary>Initializes a new instance of the <see cref="SecBufferDesc"/> struct.</summary>
            /// <param name="secBufferList">The sec buffer list.</param>
            public SecBufferDesc(SecBuffer[] secBufferList)
            {
                this.ulVersion = (uint)SecBufferType.Version;
                this.cBuffers = (uint)secBufferList.Length;
                this.pBuffers = Marshal.AllocHGlobal(SecBuffer.StructSize * (int)this.cBuffers);

                // Copy the array of SecBuffer elements offset from pBuffers
                var offsetPtr = new OffsetIntPtr(this.pBuffers, SecBuffer.StructSize);
                foreach (var secBuffer in secBufferList)
                {
                    Marshal.StructureToPtr(secBuffer, offsetPtr.GetNextPtr(), false);
                }
            }

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                if (this.pBuffers != IntPtr.Zero)
                {
                    if (this.cBuffers == 1)
                    {
                        var secBuffer = (SecBuffer)Marshal.PtrToStructure(this.pBuffers, typeof(SecBuffer));
                        secBuffer.Dispose();
                    }
                    else
                    {
                        var offsetPtr = new OffsetIntPtr(this.pBuffers, SecBuffer.StructSize);
                        for (var index = 0; index < this.cBuffers; index++)
                        {
                            var secBuffer = (SecBuffer)Marshal.PtrToStructure(offsetPtr.GetNextPtr(), typeof(SecBuffer));
                            secBuffer.Dispose();
                        }
                    }

                    Marshal.FreeHGlobal(this.pBuffers);
                    this.pBuffers = IntPtr.Zero;
                }
            }

            /// <summary>The get sec buffer byte array.</summary>
            /// <returns>The System.Byte[].</returns>
            public byte[] GetSecBufferByteArray()
            {
                byte[] returnBuffer;
                if (this.cBuffers == 1)
                {
                    var secBuffer = (SecBuffer)Marshal.PtrToStructure(this.pBuffers, typeof(SecBuffer));

                    returnBuffer = new byte[secBuffer.cbBuffer];
                    Marshal.Copy(secBuffer.pvBuffer, returnBuffer, 0, (int)secBuffer.cbBuffer);
                }
                else
                {
                    var secBufferList = new SecBuffer[this.cBuffers];
                    uint sizeToAllocate = 0;

                    var offsetPtr = new OffsetIntPtr(this.pBuffers, SecBuffer.StructSize);
                    for (var index = 0; index < this.cBuffers; index++)
                    {
                        var secBuffer = (SecBuffer)Marshal.PtrToStructure(offsetPtr.GetNextPtr(), typeof(SecBuffer));
                        sizeToAllocate += secBuffer.cbBuffer;
                        secBufferList[index] = secBuffer;
                    }

                    returnBuffer = new byte[sizeToAllocate];
                    var bufferOffset = 0;
                    foreach (var secBuffer in secBufferList)
                    {
                        Marshal.Copy(secBuffer.pvBuffer, returnBuffer, bufferOffset, (int)secBuffer.cbBuffer);
                        bufferOffset += (int)secBuffer.cbBuffer;
                    }
                }

                return returnBuffer;
            }
        }

        /// <summary>The sec handle.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SecHandle
        {
            /// <summary>The dw lower.</summary>
            public IntPtr dwLower;

            /// <summary>The dw upper.</summary>
            public IntPtr dwUpper;

            /// <summary>The set zero.</summary>
            public void SetZero()
            {
                this.dwLower = IntPtr.Zero;
                this.dwUpper = IntPtr.Zero;
            }

            /// <summary>Gets a value indicating whether is zero.</summary>
            public bool IsZero
            {
                get
                {
                    return this.dwLower == IntPtr.Zero && this.dwUpper == IntPtr.Zero;
                }
            }
        }

        /// <summary>The sec pkg info.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SecPkgInfo
        {
            /// <summary>The struct size.</summary>
            public static readonly int StructSize = Marshal.SizeOf(typeof(SecPkgInfo));

            /// <summary>The f capabilities.</summary>
            public readonly uint fCapabilities;

            /// <summary>The w version.</summary>
            public readonly ushort wVersion;

            /// <summary>The w rpcid.</summary>
            public readonly ushort wRPCID;

            /// <summary>The cb max token.</summary>
            public readonly uint cbMaxToken;

            /// <summary>The name.</summary>
            public readonly IntPtr Name;

            /// <summary>The comment.</summary>
            public readonly IntPtr Comment;

            /// <summary>Gets the name string.</summary>
            public string NameString
            {
                get
                {
                    return Marshal.PtrToStringAuto(this.Name);
                }
            }
        }

        /// <summary>The sec winnt auth identity.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SecWinntAuthIdentity
        {
            /// <summary>The se c_ winn t_ aut h_ identit y_ ansi.</summary>
            private const int SEC_WINNT_AUTH_IDENTITY_ANSI = 1;

            /// <summary>The user.</summary>
            public readonly string User;

            /// <summary>The user length.</summary>
            public readonly uint UserLength;

            /// <summary>The domain.</summary>
            public readonly string Domain;

            /// <summary>The domain length.</summary>
            public readonly uint DomainLength;

            /// <summary>The password.</summary>
            public readonly string Password;

            /// <summary>The password length.</summary>
            public readonly uint PasswordLength;

            /// <summary>The flags.</summary>
            public readonly int Flags;

            /// <summary>Initializes a new instance of the <see cref="SecWinntAuthIdentity"/> struct.</summary>
            /// <param name="user">The user.</param>
            /// <param name="domain">The domain.</param>
            /// <param name="password">The password.</param>
            public SecWinntAuthIdentity(string user, string domain, string password)
            {
                this.User = user;
                this.UserLength = (uint)user.Length;
                this.Domain = domain;
                this.DomainLength = (uint)domain.Length;
                this.Password = password;
                this.PasswordLength = (uint)password.Length;
                this.Flags = SEC_WINNT_AUTH_IDENTITY_ANSI;
            }

            /// <summary>Initializes a new instance of the <see cref="SecWinntAuthIdentity"/> struct.</summary>
            /// <param name="credential">The credential.</param>
            public SecWinntAuthIdentity(NetworkCredential credential)
                : this(credential.UserName, credential.Domain, credential.Password)
            {
            }
        }

        /// <summary>The time stamp.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TimeStamp
        {
            /// <summary>The low part.</summary>
            public readonly uint LowPart;

            /// <summary>The high part.</summary>
            public readonly int HighPart;
        }

        /// <summary>The offset int ptr.</summary>
        /// <remarks>
        ///     A simple helper class to create IntPtr's incrementing
        ///     by a fixed size (struct size).  Simplifies working
        ///     with unmanaged arrays.
        /// </remarks>
        private class OffsetIntPtr
        {
            #region Fields

            /// <summary>The _base address.</summary>
            private readonly long _baseAddress;

            /// <summary>The _increment size.</summary>
            private readonly int _incrementSize;

            /// <summary>The _offset.</summary>
            private int _offset;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="OffsetIntPtr"/> class.</summary>
            /// <param name="intPtr">The int ptr.</param>
            /// <param name="incrementSize">The increment size.</param>
            public OffsetIntPtr(IntPtr intPtr, int incrementSize)
            {
                this._baseAddress = intPtr.ToInt64();
                this._incrementSize = incrementSize;
                this._offset = 0;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The get next ptr.</summary>
            /// <returns>The System.IntPtr.</returns>
            public IntPtr GetNextPtr()
            {
                var address = this._baseAddress + this._offset;
                this._offset += this._incrementSize;

                return new IntPtr(address);
            }

            #endregion
        }
    }
}