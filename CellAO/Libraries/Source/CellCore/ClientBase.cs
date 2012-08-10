/*************************************************************************
 *                              ClientBase.cs
 *                            -----------------
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
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Resources;

#pragma warning disable 1591

namespace Cell.Core
{
    /// <summary>
    /// Base class for all clients.
    /// </summary>
    /// <seealso cref="ServerBase"/>
    public abstract class ClientBase
    {
        public class ProcessDataExecObj : IExecObj
        {
            private ClientBase m_client;
            private int m_bytesReceived = 0;

            public ProcessDataExecObj()
            {
            }

            public ClientBase Client
            {
                set
                {
                    m_client = value;
                }
            }

            public int BytesReceived
            {
                get
                {
                    return m_bytesReceived;
                }
                set
                {
                    m_bytesReceived = value;
                }
            }

            public void Execute()
            {
                m_client.OnReceive(m_bytesReceived);
                m_client.ResumeReceive();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="server">The server this client is connected to.</param>
        protected ClientBase(ServerBase server)
        {
            m_server = server;

            m_readBuffer = BufferManager.Instance.CheckOut();

            m_processDataObj = new ProcessDataExecObj();
            m_processDataObj.Client = this;
        }

        #region Private variables

        /// <summary>
        /// Total number of bytes that have been received by all clients.
        /// </summary>
        private static volatile uint s_totalBytesReceived;

        /// <summary>
        /// Total number of bytes that have been sent by all clients.
        /// </summary>
        private static volatile uint s_totalBytesSent;

        /// <summary>
        /// Number of bytes that have been received by this client.
        /// </summary>
        private int m_bytesReceived;

        /// <summary>
        /// Number of bytes that have been sent by this client.
        /// </summary>
        private int m_bytesSent;

        /// <summary>
        /// The socket containing the TCP connection this client is using.
        /// </summary>
        protected Socket m_tcpSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Pointer to the server this client is connected to.
        /// </summary>
        protected ServerBase m_server;

        /// <summary>
        /// The port the client should receive UDP datagrams on.
        /// </summary>
        protected IPEndPoint m_udpEp;

        /// <summary>
        /// The buffer containing the data received.
        /// </summary>
        protected ArraySegment<byte> m_readBuffer;

        /// <summary>
        /// The adjustment to make in the packet buffer.
        /// </summary>
        protected int m_packetDelta;

        /// <summary>
        /// The execution object for processing data.
        /// </summary>
        protected ProcessDataExecObj m_processDataObj;

        #endregion

        #region Public properties

        public ServerBase Server
        {
            get { return m_server; }
        }

        /// <summary>
        /// Gets the total number of bytes sent to all clients.
        /// </summary>
        [CLSCompliant(false)]
        public static uint TotalBytesSent
        {
            get { return s_totalBytesSent; }
        }

        /// <summary>
        /// Gets the total number of bytes received by all clients.
        /// </summary>
        [CLSCompliant(false)]
        public static uint TotalBytesReceived
        {
            get { return s_totalBytesReceived; }
        }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        public IPAddress TcpIP
        {
            get { return (m_tcpSock.RemoteEndPoint as IPEndPoint).Address; }
        }

        /// <summary>
        /// Gets the port the client is communicating on.
        /// </summary>
        public int TcpPort
        {
            get { return (m_tcpSock.RemoteEndPoint as IPEndPoint).Port; }
        }

        /// <summary>
        /// Gets the port the client should receive UDP datagrams on.
        /// </summary>
        public IPEndPoint UdpEndpoint
        {
            get { return m_udpEp; }
            set { m_udpEp = value; }
        }

        /// <summary>
        /// Gets/Sets the socket this client is using for TCP communication.
        /// </summary>
        public Socket TcpSocket
        {
            get { return m_tcpSock; }
            set
            {
                if (m_tcpSock.Connected)
                {
                    m_tcpSock.Shutdown(SocketShutdown.Both);
                    m_tcpSock.Close();
                }

                if (value != null)
                {
                    m_tcpSock = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Begins asynchronous TCP receiving for this client.
        /// </summary>
        public void BeginReceive()
        {
            if (m_tcpSock.Connected)
            {
                using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                {
                    args.SetBuffer(m_readBuffer.Array, m_readBuffer.Offset, m_readBuffer.Count);
                    args.UserToken = this;
                    args.Completed += ReceiveAsyncComplete;

                    m_tcpSock.ReceiveAsync(args);
                }
            }
        }

        /// <summary>
        /// Resumes asynchronous TCP receiving for this client.
        /// </summary>
        public void ResumeReceive()
        {
            if (m_tcpSock.Connected)
            {
                using (SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs())
                {
                    socketArgs.SetBuffer(m_readBuffer.Array, m_readBuffer.Offset + m_packetDelta, m_readBuffer.Count - m_packetDelta);
                    socketArgs.UserToken = this;
                    socketArgs.Completed += ReceiveAsyncComplete;

                    m_tcpSock.ReceiveAsync(socketArgs);
                }
            }
        }

        private static void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ClientBase client = args.UserToken as ClientBase;

            try
            {
                int bytesTransferred = args.BytesTransferred;

                if (bytesTransferred == 0)
                {
                    client.m_server.DisconnectClient(client, true);
                }
                else
                {
                    unchecked
                    {
                        client.m_bytesReceived += bytesTransferred;
                        s_totalBytesReceived += (uint)bytesTransferred;
                    }

                    client.m_processDataObj.BytesReceived = bytesTransferred;

                    ThreadMgr.QueueExecObj(client.m_processDataObj);
                }
            }
            catch (ObjectDisposedException e)
            {
                client.m_server.Warning(client, e);
                client.m_server.DisconnectClient(client, true);
            }
            catch (Exception e)
            {
                client.m_server.Warning(client, e);
                client.m_server.DisconnectClient(client, true);
            }
        }

        /// <summary>
        /// Called when a packet has been received and needs to be processed.
        /// </summary>
        /// <param name="numBytes">The size of the packet in bytes.</param>
        protected abstract void OnReceive(int numBytes);

        /// <summary>
        /// Called when the client is disconnected
        /// </summary>
        public virtual void Cleanup()
        {
            BufferManager.Instance.CheckIn(m_readBuffer);
        }

        /// <summary>
        /// Asynchronously sends a packet of data to the client.
        /// </summary>
        /// <param name="packet">An array of bytes containing the packet to be sent.</param>
        public virtual void Send(ref byte[] packet)
        {
            Send(ref packet, 0, packet.Length);
        }

        /// <summary>
        /// Asynchronously sends a packet of data to the client.
        /// </summary>
        /// <param name="packet">An array of bytes containing the packet to be sent.</param>
        /// <param name="length">The number of bytes to send starting at offset.</param>
        /// <param name="offset">The offset into packet where the sending begins.</param>
        public void Send(ref byte[] packet, int offset, int length)
        {
            if (m_tcpSock.Connected)
            {
                using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                {
                    args.Completed += SendAsyncComplete;
                    args.SetBuffer(packet, offset, length);
                    args.UserToken = this;
                    m_tcpSock.SendAsync(args);

                    unchecked
                    {
                        m_bytesSent += length;
                        s_totalBytesSent += (uint)length;
                    }
                }
            }
        }

        private static void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ClientBase client = args.UserToken as ClientBase;

            try
            {
            }
            catch
            {
                // Don't do anything because all errors in the 
                // network stream are handled in the receive code.
            }
        }

        /// <summary>
        /// Connects the client to the server at the specified address and port.
        /// </summary>
        /// <remarks>This function uses IPv4.</remarks>
        /// <param name="host">The IP address of the server to connect to.</param>
        /// <param name="port">The port to use when connecting to the server.</param>
        public void Connect(string host, int port)
        {
            Connect(IPAddress.Parse(host), port);
        }

        /// <summary>
        /// Connects the client to the server at the specified address and port.
        /// </summary>
        /// <remarks>This function uses IPv4.</remarks>
        /// <param name="addr">The IP address of the server to connect to.</param>
        /// <param name="port">The port to use when connecting to the server.</param>
        public void Connect(IPAddress addr, int port)
        {
            if (m_tcpSock.Connected)
            {
                m_tcpSock.Disconnect(true);
            }

            m_tcpSock.Connect(addr, port);

            BeginReceive();
        }

        /// <summary>
        /// Base for processing timers (Attack/Nanos etc)
        /// </summary>
        public virtual void processTimers(DateTime _now)
        {
        }
    }
}
