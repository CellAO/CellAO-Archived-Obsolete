/*************************************************************************
 *                             Configuration.cs
 *                            ------------------
 *   begin                : Jan 24, 2007
 *   copyright            : (C) The WCell Team
 *   email                : info@wcell.org
 *************************************************************************/

/*************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Cell.Core.Localization;
using NLog;

#pragma warning disable 1591
#pragma warning disable 419

namespace Cell.Core
{
    /// <summary>
    /// Container class for the server object and the client IP.
    /// </summary>
    public class UDPSendToArgs
    {
        private ServerBase m_srvr;
        private IPEndPoint m_client;

        /// <summary>
        /// The server object receiving the UDP communications.
        /// </summary>
        public ServerBase Server
        {
            get { return m_srvr; }
        }

        /// <summary>
        /// The IP address the data was received from.
        /// </summary>
        public IPEndPoint ClientIP
        {
            get { return m_client; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="srvr">The server object receiving the UP communications.</param>
        /// <param name="client">The IP address the data was received from.</param>
        public UDPSendToArgs(ServerBase srvr, IPEndPoint client)
        {
            m_srvr = srvr;
            m_client = client;
        }
    }

    #region Event Delegates
    /// <summary>
    /// Handler used for the client connected event
    /// </summary>
    /// <param name="client">The client connection</param>
    public delegate void ClientConnectedHandler(ClientBase client);

    /// <summary>
    /// Handler used for client disconnected event
    /// </summary>
    /// <param name="client">The client connection</param>
    /// <param name="forced">Indicates if the client disconnection was forced</param>
    public delegate void ClientDisconnectedHandler(ClientBase client, bool forced);

    #endregion

    /// <summary>
    /// This is the base class for all server classes.
    /// <seealso cref="ClientBase"/>
    /// </summary>
    public abstract class ServerBase
    {
        #region Private variables

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A hashtable containing all of the clients connected to the server.
        /// <seealso cref="ClientBase"/>
        /// </summary>
        protected List<ClientBase> m_clients = new List<ClientBase>();

        /// <summary>
        /// The remote endpoint (IP address and port) of the adapter to use with TCP communiations.
        /// </summary>
        protected IPEndPoint m_tcpEndPoint = new IPEndPoint(GetDefaultExternalIPAddress(), 0);

        /// <summary>
        /// The remote endpoint (IP address and port) of the adapter to use with UDP communiations.
        /// </summary>
        protected IPEndPoint m_udpEndPoint = new IPEndPoint(GetDefaultExternalIPAddress(), 0);

        /// <summary>
        /// The socket the server listens on for incoming TCP connections.
        /// <seealso cref="ServerBase.Start"/>
        /// <seealso cref="ServerBase.TcpIP"/>
        /// <seealso cref="ServerBase.TcpPort"/>
        /// </summary>
        protected Socket m_tcpListen;

        /// <summary>
        /// The socket the server listens on for incoming UDP packets.
        /// </summary>
        protected Socket m_udpListen;

        /// <summary>
        /// The maximum number of pending connections.
        /// </summary>
        protected int m_maxPendingCon = 100;

        /// <summary>
        /// True if the server is currently accepting connections.
        /// </summary>
        private volatile bool m_running;

        /// <summary>
        /// True if TCP is enabled, default is true.
        /// </summary>
        protected bool m_tcpEnabled = true;

        /// <summary>
        /// True if UDP is enabled, default is false.
        /// </summary>
        protected bool m_udpEnabled;

        /// <summary>
        /// The buffer for incoming UDP data.
        /// </summary>
        private byte[] m_udpBuffer = new byte[1024];

        /// <summary>
        /// The counter for all sent UDP packets
        /// </summary>
        private ushort m_udpCounter;

        #endregion

        #region Public Properties

        public List<ClientBase> Clients
        {
            get
            {
                return m_clients;
            }
        }

        private Dictionary<UInt32, ClientBase> m_ConnectedClients = new Dictionary<UInt32, ClientBase>();

        public Dictionary<UInt32, ClientBase> ConnectedClients
        {
            get
            {
                return m_ConnectedClients;
            }
            set
            {
                m_ConnectedClients = value;
            }
        }

        /// <summary>
        /// Gets the current status of the server.
        /// </summary>
        public bool Running
        {
            get { return m_running; }
        }

        /// <summary>
        /// Gets/Sets the maximum number of pending connections.
        /// </summary>
        /// <value>The maximum number of pending connections.</value>
        public virtual int MaximumPendingConnections
        {
            get { return m_maxPendingCon; }
            set
            {
                if (value > 0)
                {
                    m_maxPendingCon = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the port the server will listen on for incoming TCP connections.
        /// <seealso cref="ServerBase.Start"/>
        /// <seealso cref="ServerBase.TcpIP"/>
        /// </summary>
        [CLSCompliant(false)]
        public virtual int TcpPort
        {
            get { return m_tcpEndPoint.Port; }
            set { m_tcpEndPoint.Port = value; }
        }

        /// <summary>
        /// Gets/Sets the port the server will listen on for incoming UDP connections.
        /// <seealso cref="ServerBase.Start"/>
        /// <seealso cref="ServerBase.UdpIP"/>
        /// </summary>
        [CLSCompliant(false)]
        public virtual int UdpPort
        {
            get { return m_udpEndPoint.Port; }
            set { m_udpEndPoint.Port = value; }
        }

        /// <summary>
        /// The IP address of the adapter the server will use for TCP communications.
        /// <seealso cref="ServerBase.Start"/>
        /// <seealso cref="ServerBase.TcpPort"/>
        /// </summary>
        public virtual IPAddress TcpIP
        {
            get { return m_tcpEndPoint.Address; }
            set { m_tcpEndPoint.Address = value; }
        }

        /// <summary>
        /// The IP address of the adapter the server will use for UDP communications.
        /// </summary>
        public virtual IPAddress UdpIP
        {
            get { return m_udpEndPoint.Address; }
            set { m_udpEndPoint.Address = value; }
        }

        /// <summary>
        /// The endpoint clients will connect to for TCP connections
        /// </summary>
        public virtual IPEndPoint TcpEndPoint
        {
            get { return m_tcpEndPoint; }
            set { m_tcpEndPoint = value; }
        }

        /// <summary>
        /// The endpoint clients will connect to for UDP connections
        /// </summary>
        public virtual IPEndPoint UdpEndPoint
        {
            get { return m_udpEndPoint; }
            set { m_udpEndPoint = value; }
        }

        /// <summary>
        /// Gets the number of clients currently connected to the server.
        /// </summary>
        public int NumberOfClients
        {
            get { return m_clients.Count; }
        }

        /// <summary>
        /// The root path of this server assembly.
        /// </summary>
        public string RootPath
        {
            get { return Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName; }
        }

        /// <summary>
        /// Begin listening for TCP connections. Should not be called directly - instead use <see cref="Start"/>
        /// <seealso cref="EnableTCP"/>
        /// </summary>
        protected void StartTCP()
        {
            if (!m_tcpEnabled || !m_running)
            {
                m_tcpListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    m_tcpListen.Bind(TcpEndPoint);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Error binding socket: {0}", ex.Message);
                    return;
                }
                m_tcpListen.Listen(MaximumPendingConnections);

                /*SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = this;
                args.Completed += AcceptAsyncComplete;

                m_tcpListen.AcceptAsync(args);*/
                using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                {
                    args.UserToken = this;
                    args.Completed += AcceptAsyncComplete;
                    m_tcpListen.AcceptAsync(args);
                }

                m_tcpEnabled = true;
                Info(null, Resources.ListeningTCPSocket, TcpEndPoint);
            }
        }

        /// <summary>
        /// Gets/Sets whether or not to use TCP communications.
        /// </summary>
        public bool EnableTCP
        {
            get { return m_tcpEnabled; }
            set
            {
                if (m_tcpEnabled && !value && m_running)
                {
                    RemoveAllClients();
                    m_tcpListen.Close(60);
                }
                else if (!m_tcpEnabled && value && m_running)
                {
                    StartTCP();
                }
            }
        }

        /// <summary>
        /// Begin listening for UDP connections. Should not be called directly - instead use <see cref="Start"/>
        /// <seealso cref="EnableTCP"/>
        /// </summary>
        protected void StartUDP()
        {
            if (!m_tcpEnabled || !m_running)
            {
                m_udpListen.Bind(new IPEndPoint(UdpIP, UdpPort));
                EndPoint tempEP = new IPEndPoint(IPAddress.Any, 0);
                m_udpListen.BeginReceiveFrom(m_udpBuffer, 0, m_udpBuffer.Length, SocketFlags.None, ref tempEP,
                                             new AsyncCallback(RecvDgramCallback), this);

                m_udpEnabled = true;
                Info(null, Resources.ListeningUDPSocket, UdpEndPoint);
            }
        }

        /// <summary>
        /// Gets/Sets whether or not to use UDP communications.
        /// </summary>
        public bool EnableUDP
        {
            get { return m_udpEnabled; }
            set
            {
                if (m_udpEnabled && !value && m_running)
                {
                    m_udpListen.Close(60);
                }
                else if (!m_udpEnabled && value && m_running)
                {
                    StartUDP();
                }
            }
        }

        /// <summary>
        /// Holds the sequence number for UDP packets
        /// </summary>
        [CLSCompliant(false)]
        public ushort UdpCounter
        {
            get { return m_udpCounter; }
            set { m_udpCounter = value; }
        }

        #endregion

        #region Public Events
        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;
        #endregion

        #region State management

        /// <summary>
        /// Starts the server and begins accepting connections
        /// <seealso cref="ServerBase.Stop"/>
        /// </summary>
        public virtual void Start()
        {
            if (!m_running)
            {
                if (m_tcpEnabled)
                {
                    StartTCP();
                }

                if (m_udpEnabled)
                {
                    StartUDP();
                }

                m_running = true;
                log.Info(string.Format(Resources.ReadyForConnections, this));
            }
        }

        /// <summary>
        /// Stops the server and disconnects all clients.
        /// <seealso cref="ServerBase.Start"/>
        /// <seealso cref="ServerBase.RemoveAllClients"/>
        /// <seealso cref="ServerBase.DisconnectClient"/>
        /// </summary>
        public virtual void Stop()
        {
            if (m_running)
            {
                m_running = false;
                if (m_tcpListen != null)
                {
                    RemoveAllClients();
                    m_tcpListen.Close(60);
                }

                if (m_udpListen != null)
                {
                    m_udpListen.Close();
                }
            }
        }

        #endregion

        #region Client management

        /// <summary>
        /// Creates a new client object.
        /// <seealso cref="ServerBase.Start"/>
        /// </summary>
        /// <returns>A client object to wrap an incoming connection.</returns>
        protected abstract ClientBase CreateClient();

        /// <summary>
        /// Removes a client from the internal client list.
        /// <seealso cref="ServerBase.RemoveAllClients"/>
        /// </summary>
        /// <param name="client">The client to be removed</param>
        protected void RemoveClient(ClientBase client)
        {
            lock (m_clients)
            {
                if (m_clients.Contains(client))
                {
                    m_clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// Disconnects and removes a client.
        /// <seealso cref="ServerBase.Stop"/>
        /// <seealso cref="ServerBase.RemoveAllClients"/>
        /// </summary>
        /// <param name="client">The client to be disconnected/removed</param>
        /// <param name="forced">Flag indicating if the client was disconnected already</param>
        public void DisconnectClient(ClientBase client, bool forced)
        {
            RemoveClient(client);

            try
            {
                //NOTE: if client.TcpSocket.Connected == false that means the remote host forced the connection
                //to close
                OnClientDisconnected(client, forced);

                client.TcpSocket.Shutdown(SocketShutdown.Both);
                client.TcpSocket.Close();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", e);
            }
        }

        /// <summary>
        /// Disconnects and removes a client.
        /// <seealso cref="ServerBase.Stop"/>
        /// <seealso cref="ServerBase.RemoveAllClients"/>
        /// </summary>
        /// <param name="client">The client to be disconnected/removed</param>
        public void DisconnectClient(ClientBase client)
        {
            DisconnectClient(client, true);
        }

        /// <summary>
        /// Disconnects all clients currently connected to the server.
        /// <seealso cref="ServerBase.Stop"/>
        /// <seealso cref="ServerBase.DisconnectClient(ClientBase)"/>
        /// </summary>
        public void RemoveAllClients()
        {
            lock (m_clients)
            {
                foreach (ClientBase client in m_clients)
                {
                    try
                    {
                        //NOTE: if client.TcpSocket.Connected == false that means the remote host forced the connection
                        //to close
                        OnClientDisconnected(client, true);

                        client.TcpSocket.Shutdown(SocketShutdown.Both);
                        client.TcpSocket.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (Exception e)
                    {
                        LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", e);
                    }
                }

                m_clients.Clear();
            }
        }

        /// <summary>
        /// Called when a client has connected to the server.
        /// </summary>
        /// <param name="client">The client that has connected.</param>
        /// <returns>True if the connection is to be accepted.</returns>
        protected virtual bool OnClientConnected(ClientBase client)
        {
            Info(client, Resources.Connected);

            ClientConnectedHandler handler = ClientConnected;

            if (handler != null)
                handler(client);

            return true;
        }

        /// <summary>
        /// Called when a client has been disconnected from the server.
        /// </summary>
        /// <param name="client">The client that has been disconnected.</param>
        /// <param name="forced">Indicates if the client disconnection was forced</param>
        protected virtual void OnClientDisconnected(ClientBase client, bool forced)
        {
            client.Cleanup();
            Info(client, Resources.Disconnected);

            ClientDisconnectedHandler handler = ClientDisconnected;

            if (handler != null)
                handler(client, forced);
        }

        #endregion

        #region Socket management

        /// <summary>
        /// Get the default external IP address for the current machine. This is always the first
        /// IP listed in the host address list.
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetDefaultExternalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address;
            }

            return null;
        }

        private static void AcceptAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ServerBase server = args.UserToken as ServerBase;

            try
            {
                if (!server.Running)
                {
                    LogManager.GetLogger(CellDef.CORE_LOG_FNAME).Info(Resources.ServerNotRunning);
                    return;
                }

                ClientBase client = server.CreateClient();
                client.TcpSocket = args.AcceptSocket;
                client.BeginReceive();

                using (SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs())
                {
                    socketArgs.UserToken = server;
                    socketArgs.Completed += AcceptAsyncComplete;
                    server.m_tcpListen.AcceptAsync(socketArgs);
                }

                if (server.OnClientConnected(client))
                {
                    lock (server.m_clients)
                    {
                        server.m_clients.Add(client);
                    }
                }
                else
                {
                    client.TcpSocket.Shutdown(SocketShutdown.Both);
                    client.TcpSocket.Close();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException e)
            {
                //#warning TODO: Add a proper exception handling for the different SocketExceptions Error Codes.
                LogManager.GetLogger(CellDef.CORE_LOG_FNAME).Warn(Resources.SocketExceptionAsyncAccept, e);
            }
            catch (Exception e)
            {
                LogManager.GetLogger(CellDef.CORE_LOG_FNAME).Fatal(Resources.FatalAsyncAccept, e);
                server.Stop();
            }
        }

        /// <summary>
        /// Handles an incoming UDP datagram.
        /// </summary>
        /// <param name="ar">The results of the asynchronous operation.</param>
        private static void RecvDgramCallback(IAsyncResult ar)
        {
            ServerBase srvr = ar.AsyncState as ServerBase;

            EndPoint ip = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                if (srvr != null)
                {
                    int num_bytes = srvr.m_udpListen.EndReceiveFrom(ar, ref ip);
                    srvr.OnReceiveUDP(num_bytes, srvr.m_udpBuffer, ip as IPEndPoint);
                }

                EndPoint tempEP = new IPEndPoint(IPAddress.Any, 0);

                if (srvr != null)
                    srvr.m_udpListen.BeginReceiveFrom(srvr.m_udpBuffer, 0, srvr.m_udpBuffer.Length, SocketFlags.None,
                                                      ref tempEP, new AsyncCallback(RecvDgramCallback), srvr);
            }
            catch (ObjectDisposedException)
            {
                //srvr.OnReceiveUDP(0, srvr.m_udpBuffer, ip as IPEndPoint);
            }
            catch (Exception e)
            {
                if (srvr != null) srvr.Error(null, e);
            }
        }

        /// <summary>
        /// Handler for a UDP datagram.
        /// </summary>
        /// <param name="num_bytes">The number of bytes in the datagram.</param>
        /// <param name="buf">The buffer holding the datagram.</param>
        /// <param name="ip">The IP address of the sender.</param>
        protected abstract void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip);

        /// <summary>
        /// Asynchronously sends a UDP datagram to the client.
        /// </summary>
        /// <param name="buf">An array of bytes containing the packet to be sent.</param>
        /// <param name="client">An IPEndPoint for the datagram to be sent to.</param>
        public void SendUDP(byte[] buf, IPEndPoint client)
        {
            if (m_udpListen != null)
            {
                m_udpListen.BeginSendTo(buf, 0, buf.Length, SocketFlags.None, client, new AsyncCallback(SendToCallback),
                                        new UDPSendToArgs(this, client));
            }
        }

        /// <summary>
        /// Called when a datagram has been sent.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        private static void SendToCallback(IAsyncResult ar)
        {
            UDPSendToArgs args = ar.AsyncState as UDPSendToArgs;
            try
            {
                if (args != null)
                {
                    int num_bytes = args.Server.m_udpListen.EndSendTo(ar);
                    args.Server.OnSendTo(args.ClientIP, num_bytes);
                }
            }
            catch (Exception e)
            {
                if (args != null) args.Server.Error(null, e);
            }
        }

        /// <summary>
        /// Called when a datagram has been sent.
        /// </summary>
        /// <param name="clientIP">The IP address of the recipient.</param>
        /// <param name="num_bytes">The number of bytes sent.</param>
        protected abstract void OnSendTo(IPEndPoint clientIP, int num_bytes);

        #endregion

        #region Logging

        /// <summary>
        /// Create a string for logging information about a given client given a formatted message and parameters
        /// </summary>
        /// <param name="client">Client which caused the event</param>
        /// <param name="msg">Message describing the event</param>
        /// <param name="parms">Parameters for formatting the message.</param>
        protected static string FormatLogString(ClientBase client, string msg, params object[] parms)
        {
            msg = (parms == null ? msg : string.Format(msg, parms));

            if (client == null)
            {
                return msg;
            }
            else
            {
                return string.Format("({0}) -> {1}", ((IPEndPoint)client.TcpSocket.RemoteEndPoint).ToString(), msg);
            }
        }

        /// <summary>
        /// Generates a server error.
        /// </summary>
        /// <param name="e">An exception describing the error.</param>
        /// <param name="client">The client that generated the error.</param>
        public void Error(ClientBase client, Exception e)
        {
            if (log.IsErrorEnabled)
            {
                log.Error("{0} - {1}", client, e);
                log.ErrorException("", e);
            }

            if (client != null)
            {
                DisconnectClient(client);
            }
        }

        /// <summary>
        /// Generates a server error.
        /// </summary>
        /// <param name="parms">Parameters for formatting the message.</param>
        /// <param name="msg">The message describing the error.</param>
        /// <param name="client">The client that generated the error.</param>
        public void Error(ClientBase client, string msg, params object[] parms)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(FormatLogString(client, msg, parms));
            }

            if (client != null)
            {
                DisconnectClient(client);
            }
        }

        /// <summary>
        /// Generates a server warning.
        /// </summary>
        /// <param name="e">An exception describing the warning.</param>
        /// <param name="client">The client that generated the error.</param>
        public virtual void Warning(ClientBase client, Exception e)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn("{0} - {1}", client, e);
            }
        }

        /// <summary>
        /// Generates a server warning.
        /// </summary>
        /// <param name="parms">Parameters for formatting the message.</param>
        /// <param name="msg">The message describing the warning.</param>
        /// <param name="client">The client that generated the error.</param>
        public virtual void Warning(ClientBase client, string msg, params object[] parms)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(FormatLogString(client, msg, parms));
            }
        }

        /// <summary>
        /// Generates a server notification.
        /// </summary>
        /// <param name="msg">Text describing the notification.</param>
        /// <param name="parms">The parameters to pass to the function for formatting.</param>
        /// <param name="client">The client that generated the error.</param>
        public virtual void Info(ClientBase client, string msg, params object[] parms)
        {
            if (log.IsWarnEnabled)
            {
                log.Info(FormatLogString(client, msg, parms));
            }
        }

        /// <summary>
        /// Generates a server debug message.
        /// </summary>
        /// <param name="msg">Text describing the notification.</param>
        /// <param name="parms">The parameters to pass to the function for formatting.</param>
        /// <param name="client">The client that generated the error.</param>
        public virtual void Debug(ClientBase client, string msg, params object[] parms)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(FormatLogString(client, msg, parms));
            }
        }

        #endregion
    }
}