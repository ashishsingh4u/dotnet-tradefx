//  ===================================================================================
//  <copyright file="SocketConnector.cs" company="TechieNotes">
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
//     The SocketConnector.cs file.
//  </summary>
//  ===================================================================================

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TradeFx.Common.Transport.ProxyConnector
{
    /// <summary>The socket connector.</summary>
    public sealed class SocketConnector : ISocketConnector
    {
        // Emulating IE 6 as some proxies filter based on standard HTTP headers
        #region Constants

        /// <summary>The authorization prefix.</summary>
        private const string AuthorizationPrefix = "Proxy-Authorization: ";

        /// <summary>The http end.</summary>
        private const string HttpEnd = "\r\n\r\n";

        /// <summary>The http newline.</summary>
        private const string HttpNewline = "\r\n";

        /// <summary>The proxy connect format.</summary>
        private const string ProxyConnectFormat =
            "CONNECT {0}:{1} HTTP/1.0\r\n"
            + "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.1; .NET CLR 2.0.50727; .NET CLR 1.1.4322)\r\n"
            + "Host: {0}\r\n" + "Content-Length: 0\r\n" + "Proxy-Connection: Keep-Alive\r\n" + "Pragma: no-cache\r\n";

        /// <summary>The proxy recieve timeout.</summary>
        private const int ProxyRecieveTimeout = 5000;

        #endregion

        #region Fields

        /// <summary>The _proxy.</summary>
        private readonly IWebProxy _proxy;

        /// <summary>The _address family.</summary>
        private AddressFamily _addressFamily;

        /// <summary>The _protocol type.</summary>
        private ProtocolType _protocolType;

        /// <summary>The _socket type.</summary>
        private SocketType _socketType;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SocketConnector"/> class. Initializes an instance of the SocketConnector class.</summary>
        /// <param name="proxy">IWebProxy object containing proxy details.</param>
        public SocketConnector(IWebProxy proxy)
        {
            this._proxy = proxy;

            // Set default values for new socket.
            this.SetSocketDetails(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="host">IP address or host name of remote server.</param>
        /// <param name="port">Port of remote server.</param>
        /// <returns>Connected socket.</returns>
        public Socket GetConnectedSocket(string host, int port)
        {
            var remoteUri = new UriBuilder(Uri.UriSchemeHttps, host, port).Uri;

            return this.GetConnectedSocket(remoteUri);
        }

        /// <summary>Gets the connected socket via the configured proxy server (if required).</summary>
        /// <param name="remoteUri">The remote Uri.</param>
        /// <returns>Connected socket.</returns>
        /// <exception cref="SocketConnectorException">Thrown on error establishing connected socket.</exception>
        public Socket GetConnectedSocket(Uri remoteUri)
        {
            // Once assigned, authenticator is persisted through all request / response loops.
            IProxyAuthenticator authenticator = null;

            // Socket is connected to proxy if required, or directly to server if not required.
            var socket = this.CreateAndConnectSocket(remoteUri);

            try
            {
                // If proxy is null, bypassed or not required, return directly connected socket.
                if (this._proxy == null || this._proxy.IsBypassed(remoteUri))
                {
                    return socket;
                }

                // Send initial generic proxy connect request (no authentication).
                var proxyRequest = GetProxyRequest(remoteUri, null, null);
                SendProxyRequest(socket, proxyRequest);

                // Iterate request / response loop until failure or succesfully connected.
                // Each authenticator should monitor its own success or failure according
                // to the underlying protocol.
                while (true)
                {
                    var proxyResponse = ReadProxyResponse(socket);
                    switch (proxyResponse.Status)
                    {
                        case HttpStatusCode.OK:
                            return socket;

                        case HttpStatusCode.ProxyAuthenticationRequired:
                            socket = this.GetNewSocketIfRequired(socket, remoteUri, proxyResponse);
                            authenticator = this.GetProxyAuthenticator(authenticator, proxyResponse);
                            proxyRequest = GetProxyRequest(remoteUri, authenticator, proxyResponse);
                            SendProxyRequest(socket, proxyRequest);
                            break;

                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.ServiceUnavailable:
                            throw new SocketConnectorException(
                                SocketConnectorErrorType.RemoteServerNotAvailable, proxyResponse.Status, remoteUri);

                        case HttpStatusCode.BadRequest:
                            throw new SocketConnectorException(
                                SocketConnectorErrorType.ProtocolNegotiationFailed, proxyResponse.Status, remoteUri);

                        case HttpStatusCode.Forbidden:
                            throw new SocketConnectorException(
                                SocketConnectorErrorType.AuthenticationFailed, proxyResponse.Status, remoteUri);

                        default:
                            throw new SocketConnectorException(
                                SocketConnectorErrorType.UnexpectedError, proxyResponse.Status, remoteUri);
                    }
                }
            }
            catch (SocketConnectorException)
            {
                CloseSocket(socket);
                throw;
            }
            catch (Exception ex)
            {
                CloseSocket(socket);
                throw new SocketConnectorException(SocketConnectorErrorType.UnexpectedError, ex);
            }
            finally
            {
                if (authenticator != null)
                {
                    authenticator.Dispose();
                }
            }
        }

        /// <summary>Allows overriding of the default socket values for new socket.</summary>
        /// <param name="addressFamily">One of the AddressFamily values.</param>
        /// <param name="socketType">One of the SocketType values.</param>
        /// <param name="protocolType">One of the ProtocolType values.</param>
        public void SetSocketDetails(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            this._addressFamily = addressFamily;
            this._socketType = socketType;
            this._protocolType = protocolType;
        }

        #endregion

        #region Methods

        /// <summary>The close socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <exception cref="SocketConnectorException"></exception>
        private static void CloseSocket(Socket socket)
        {
            try
            {
                socket.Close();
            }
            catch (SocketException ex)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SocketCloseError, ex);
            }
        }

        /// <summary>The get proxy request.</summary>
        /// <param name="remoteUri">The remote uri.</param>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="proxyResponse">The proxy response.</param>
        /// <returns>The System.Byte[].</returns>
        private static byte[] GetProxyRequest(
            Uri remoteUri, IProxyAuthenticator authenticator, ProxyResponse proxyResponse)
        {
            var connectRequest = string.Format(
                CultureInfo.InvariantCulture, ProxyConnectFormat, remoteUri.Host, remoteUri.Port);

            if (authenticator != null)
            {
                // Get the authentication token, generally default encoding but can be utf-8 etc.
                // As a result, we must not attempt to convert this to a string to generate request.
                var token = authenticator.GetAuthenticatorToken(proxyResponse);

                // Calculate total length of return byte array.
                // Regardless of encoding, connectRequest, AuthorizationPrefix and HttpEnd will all 
                // result in one byte per char as they are all within ASCII byte range.
                var returnLength = connectRequest.Length + AuthorizationPrefix.Length + token.Length + HttpEnd.Length;
                var returnBytes = new byte[returnLength];
                var offset = 0;

                // Return byte array is concatination of connectRequest, AuthorizationPrefix, token and HttpEnd (double newline).
                offset += ProxyEncoding.Default.GetBytes(connectRequest, 0, connectRequest.Length, returnBytes, offset);
                offset += ProxyEncoding.Default.GetBytes(
                    AuthorizationPrefix, 0, AuthorizationPrefix.Length, returnBytes, offset);
                Buffer.BlockCopy(token, 0, returnBytes, offset, token.Length);
                offset += token.Length;
                ProxyEncoding.Default.GetBytes(HttpEnd, 0, HttpEnd.Length, returnBytes, offset);

                return returnBytes;
            }

            // Non-authenticated connect request ends with double newline
            return ProxyEncoding.Default.GetBytes(connectRequest + HttpNewline);
        }

        /// <summary>The read proxy response.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns>The TechieNotes.Common.ProxySupport.ProxyResponse.</returns>
        /// <exception cref="SocketConnectorException"></exception>
        private static ProxyResponse ReadProxyResponse(Socket socket)
        {
            try
            {
                // Set a receive timeout to ensure this method doesn't block for a 
                // long time waiting for a response from the proxy.
                // A SocketException will be thrown if the timeout is exceeded.
                socket.ReceiveTimeout = ProxyRecieveTimeout;

                var responseBuilder = new StringBuilder();
                var buffer = new byte[1024];
                int byteCount;

                // 0 byteCount means remote disconnect so assume receive complete
                while ((byteCount = socket.Receive(buffer)) != 0)
                {
                    var receiveString = ProxyEncoding.Default.GetString(buffer, 0, byteCount);
                    responseBuilder.Append(receiveString);

                    // As the initial receive is complete, and no more bytes are available
                    // we know that we have all of the data
                    if (socket.Available == 0)
                    {
                        break;
                    }
                }

                // Restore the default infinite timeout for synchronous reads
                socket.ReceiveTimeout = 0;

                return ProxyResponse.Parse(responseBuilder.ToString());
            }
            catch (SocketException ex)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SocketReceiveError, ex);
            }
        }

        /// <summary>The send proxy request.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="SocketConnectorException"></exception>
        private static void SendProxyRequest(Socket socket, byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch (SocketException ex)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SocketSendError, ex);
            }
        }

        /// <summary>The create and connect socket.</summary>
        /// <param name="remoteUri">The remote uri.</param>
        /// <returns>The System.Net.Sockets.Socket.</returns>
        /// <exception cref="SocketConnectorException"></exception>
        private Socket CreateAndConnectSocket(Uri remoteUri)
        {
            try
            {
                // This step is required as IWebProxy does not directly return proxy host / port.
                // If the proxy is null, bypassed or not reqired, this returns the remote Uri.
                var connectUri = this._proxy != null ? this._proxy.GetProxy(remoteUri) : remoteUri;

                // Create socket and connect to the proxy or the socket server.
                var socket = new Socket(this._addressFamily, this._socketType, this._protocolType);
                socket.Connect(connectUri.Host, connectUri.Port);
                return socket;
            }
            catch (SocketException ex)
            {
                throw new SocketConnectorException(SocketConnectorErrorType.SocketConnectError, ex);
            }
        }

        /// <summary>The get new socket if required.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="remoteUri">The remote uri.</param>
        /// <param name="proxyResponse">The proxy response.</param>
        /// <returns>The System.Net.Sockets.Socket.</returns>
        private Socket GetNewSocketIfRequired(Socket socket, Uri remoteUri, ProxyResponse proxyResponse)
        {
            // If connection header not set to keep alive, reconnection is essential
            var connectionHeader = proxyResponse.Headers["Connection"];
            if (connectionHeader == null || !connectionHeader.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase))
            {
                // Close existing socket and create new connected socket
                CloseSocket(socket);
                socket = this.CreateAndConnectSocket(remoteUri);
            }

            return socket;
        }

        /// <summary>The get proxy authenticator.</summary>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="proxyResponse">The proxy response.</param>
        /// <returns>The TechieNotes.Common.ProxySupport.IProxyAuthenticator.</returns>
        /// <exception cref="SocketConnectorException"></exception>
        private IProxyAuthenticator GetProxyAuthenticator(
            IProxyAuthenticator authenticator, ProxyResponse proxyResponse)
        {
            // Simply return the existing authenticator if one is already assigned.
            // As authenticators are stateful, it is important that the same authenticator
            // be used for all iterations of the request / response loop.
            if (authenticator != null)
            {
                return authenticator;
            }

            // Get reference to the proxy credential.  
            // We want a null credential object if not set, or assigned CredentialCache.DefaultCredentials.
            var credential = this._proxy.Credentials as NetworkCredential;
            if (credential == CredentialCache.DefaultCredentials)
            {
                credential = null;
            }

            // Iterate all offered authentication protocols until a supported option is matched.
            foreach (var authHeader in proxyResponse.Headers.GetValues("Proxy-Authenticate"))
            {
                var spaceIndex = authHeader.IndexOf(' ');
                var protocolName = spaceIndex == -1
                                       ? authHeader.ToUpperInvariant()
                                       : authHeader.ToUpperInvariant().Substring(0, spaceIndex);

                switch (protocolName)
                {
                        // CAN NOT SUPPORT NEGOTIATE OR KERBEROS UNTIL THEY ARE TESTED
                        // case "NEGOTIATE":
                        // case "KERBEROS":
                    case "NTLM":
                        if (SspiProxyAuthenticator.IsSupported(protocolName))
                        {
                            return new SspiProxyAuthenticator(protocolName, credential);
                        }

                        break;

                    case "WDIGEST":
                    case "DIGEST":
                        if (WDigestProxyAuthenticator.IsSupported)
                        {
                            return new WDigestProxyAuthenticator(credential);
                        }

                        break;

                    case "BASIC":
                        return new BasicProxyAuthenticator(credential);
                }
            }

            // No supported authenticator type, throw ex
            throw new SocketConnectorException(SocketConnectorErrorType.UnsupportedAuthenticationType);
        }

        #endregion
    }
}