#region License
/*
Copyright (c) 2005-2011, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using AO.Core.Config;
using AO.Core;
using LoginEngine.Packets;
#endregion

namespace LoginEngine
{
    public class Parser
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Parser()
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <param name="messageNumber"></param>
        public void Parse(Client client, ref byte[] packet, uint messageNumber)
        {
            #region Setup...
            MemoryStream m_stream = new MemoryStream(packet);
            BinaryReader m_reader = new BinaryReader(m_stream);
            string encryptedPassword;
            int encPasswordLength;
            LoginWrong nb = new LoginWrong();
            CheckLogin cbl = new CheckLogin();
            CharacterName char_name = new CharacterName();
            #endregion

            switch (messageNumber)
            {
                case 0x22:
                    SaltAuthentication salt = new SaltAuthentication();

                    // Username and version info
                    m_stream.Position = 24;
                    client.accountName = Encoding.ASCII.GetString(m_reader.ReadBytes(40)).TrimEnd(char.MinValue);
                    client.clientVersion = Encoding.ASCII.GetString(m_reader.ReadBytes(20)).TrimEnd(char.MinValue);
                    m_reader.Close();
                    m_stream.Dispose();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Client '" + client.accountName + "' connected using version '" + client.clientVersion + "'");
                    Console.ResetColor();

                    // Send Authentication Salt to client
                    salt.SendPacket(client, out client.serverSalt);

                    break;

                case 0x25:
                    // Username and encrypted password
                    m_stream.Position = 20;
                    string loginAccountName = Encoding.ASCII.GetString(m_reader.ReadBytes(40)).TrimEnd(char.MinValue);
                    encPasswordLength = IPAddress.NetworkToHostOrder(m_reader.ReadInt32()) - 1; // Login Key is Null Terminated. We do not need this Null Terminator.
                    encryptedPassword = Encoding.ASCII.GetString(m_reader.ReadBytes(encPasswordLength));
                    m_reader.Close();
                    m_stream.Dispose();

                    if (cbl.IsLoginAllowed(client, loginAccountName) == false)
                    {
                        /* 
                         * Account name not found or client banned/otherwise not permitted on the server
                         * Note, this is done here not in the above packet (0x22), even though we have
                         * the username or the client will complain. Also a security measure as you can
                         * not tell if an account name is correct or not, only if both the username and
                         * password are correct.
                         * 
                         * We also double check that the Account Name is the same as origionally sent
                         * or there could be a possibility to log in with someone elses account.
                         * 
                         */

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client '" + client.accountName + "' banned, not a valid username, or sent a malformed Authentication Packet");
                        Console.ResetColor();

                        client.Send(ref nb.wrongbyte);

                        client.Server.DisconnectClient(client);

                        break;
                    }

                    if (cbl.IsLoginCorrect(client, encryptedPassword) == false)
                    {
                        // Username/Password Incorrect

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client '" + client.accountName + "' failed Authentication.");
                        Console.ResetColor();

                        client.Send(ref nb.wrongbyte);

                        client.Server.DisconnectClient(client);

                        break;
                    }
                    else
                    {
                        // All's well, send CharacterList Packet

                        CharacterListPacket charlist = new CharacterListPacket();

                        charlist.SendPacket(client, client.accountName);
                    }

                    break;

                case 0x16:
                    // Player selected a character and client sends us selected characters ID
                    m_stream.Position = 20;
                    int selectedCharID = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    m_reader.Close();
                    m_stream.Dispose();

                    if (cbl.IsCharacterOnAccount(client, selectedCharID) == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client '" + client.accountName + "' tried to log in as CharID " + selectedCharID + " but it is not on their account!");
                        Console.ResetColor();

                        // NV: Is this really what we want to send? Should find out sometime...
                        client.Send(ref nb.wrongbyte);

                        client.Server.DisconnectClient(client);

                        break;
                    }
                    if (OnlineChars.IsOnline(selectedCharID) == true)
                    {
                        Console.WriteLine("Client '" + client.accountName + "' is trying to login, but the requested character is already logged in.");
                        client.Send(ref nb.wrongbyte);
                        client.Server.DisconnectClient(client);
                        break;
                    }
                    OnlineChars.SetOnline(selectedCharID);

                    IPAddress tempIP;
                    if (IPAddress.TryParse(ConfigReadWrite.Instance.CurrentConfig.ZoneIP, out tempIP) == false)
                    {
                        IPHostEntry zoneHost = Dns.GetHostEntry(ConfigReadWrite.Instance.CurrentConfig.ZoneIP);
                        foreach (IPAddress ip in zoneHost.AddressList)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                tempIP = ip;
                                break;
                            }
                        }
                    }
                    byte[] ZoneIP = tempIP.GetAddressBytes();
                    byte[] ZonePort = BitConverter.GetBytes(Convert.ToInt32(ConfigReadWrite.Instance.CurrentConfig.ZonePort));

                    Byte[] pkt = new byte[] 
                    {
                        0xDF, 0xDF,
                        0x00, 0x01,
                        0x00, 0x01,
                        0x00, 0x26,
                        0x00, 0x00, 0x00, 0x01,
                        0x00, 0x00, 0x61, 0x5B, // Server ID
                        0x00, 0x00, 0x00, 0x17,
                        packet[20], packet[21], packet[22], packet[23], // character ID
                        ZoneIP[0], ZoneIP[1], ZoneIP[2], ZoneIP[3],
                        ZonePort[1], ZonePort[0],
                        0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00
                    };
                    
                    client.Send(ref pkt);
                    break;

                case 0x55:
                    /* player ask for 'suggest name'; server sends 'random' name. */
                    m_stream.Position = 28;
                    char_name.GetRandomName(client, IPAddress.NetworkToHostOrder(m_reader.ReadInt32()));
                    break;

                case 0xf:
                    /* client created new character */
                    Int32 character_id;
                    bool start_in_sl = false;
                    char_name.m_accountName = client.accountName;

                    /* start reading packet */
                    m_stream.Position = 65;
                    /* name length */
                    int m_name_len = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* name (as byte array) */
                    char_name.m_name = Encoding.ASCII.GetString(m_reader.ReadBytes(m_name_len));
                    /* breed */
                    char_name.m_breed = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* gender */
                    char_name.m_gender = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* profession */
                    char_name.m_profession = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* level (should always be 0 )*/
                    char_name.m_level = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* lets skip some stuff */
                    int skip_len = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    m_stream.Position += (skip_len + 8);
                    /* head mesh */
                    char_name.m_headmesh = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* monster scale */
                    char_name.m_monsterscale = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* fatness */
                    char_name.m_fatness = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    /* start in SL? */
                    int sl = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    if (sl == 1)
                    {
                        start_in_sl = true;
                    }
                    /* close reader and stream */
                    m_reader.Close();
                    m_stream.Dispose();

                    /* now you got the data..
                     * do whatever you have to do with it
                     * but please.. in some other class :)
                     */

                    /* check name against database 
                     * 
                     * if name exist, return 0.
                     * if name doesnt exist, creates default char setup and returns character_id
                     * 
                     */
                    character_id = char_name.CheckAgainstDB();
                    if (character_id < 1)
                    {
                        char_name.SendNameInUse(client);
                    }
                    else
                    {
                        char_name.SendNameToStartPF(client, start_in_sl, character_id);
                    }

                    /* reply will work only if character creation
                     * works, so its disabled for now and should be
                     * handled in other class in /Packets/<class name>
                     */
                    break;
                case 0x14:
                    /* client deletes char */
                    Int32 uid;
                    /* start reading packet */
                    m_stream.Position = 20;
                    uid = IPAddress.NetworkToHostOrder(m_reader.ReadInt32());
                    char_name.DeleteChar(client, uid);
                    break;
                default:
                    client.Server.Warning(client, "Client sent unknown message {0}", messageNumber.ToString());
                    break;

            }
        }
    }
}
