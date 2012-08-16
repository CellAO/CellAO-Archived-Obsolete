#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...

#endregion

namespace LoginEngine
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using AO.Core;
    using AO.Core.Config;

    using LoginEngine.Packets;

    public class Parser
    {
        #region Constructors
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <param name="messageNumber"></param>
        public void Parse(Client client, byte[] packet, uint messageNumber)
        {
            #region Setup...
            PacketReader reader = new PacketReader(ref packet);

            string encryptedPassword;
            int encPasswordLength;
            byte[] wrongLogin = LoginWrong.GetPacket();
            CheckLogin cbl = new CheckLogin();
            CharacterName char_name = new CharacterName();
            #endregion

            switch (messageNumber)
            {
                case 0x22:
                    SaltAuthentication salt = new SaltAuthentication();

                    // Username and version info
                    reader.ReadBytes(24);
                    client.AccountName = Encoding.ASCII.GetString(reader.ReadBytes(40)).TrimEnd(char.MinValue);
                    client.ClientVersion = Encoding.ASCII.GetString(reader.ReadBytes(20)).TrimEnd(char.MinValue);
                    reader.Finish();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        "Client '" + client.AccountName + "' connected using version '" + client.ClientVersion + "'");
                    Console.ResetColor();

                    // Send Authentication Salt to client
                    client.ServerSalt = salt.SendPacket(client);

                    break;

                case 0x25:
                    // Username and encrypted password
                    // m_stream.Position = 20;
                    reader.ReadBytes(20);
                    string loginAccountName = reader.PopString(40).TrimEnd(char.MinValue);
                    encPasswordLength = reader.PopInt();
                    encryptedPassword = reader.PopString(encPasswordLength).TrimEnd(char.MinValue);
                    reader.Finish();

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
                        Console.WriteLine(
                            "Client '" + client.AccountName
                            + "' banned, not a valid username, or sent a malformed Authentication Packet");
                        Console.ResetColor();

                        client.Send(ref wrongLogin);

                        client.Server.DisconnectClient(client);

                        break;
                    }

                    if (cbl.IsLoginCorrect(client, encryptedPassword) == false)
                    {
                        // Username/Password Incorrect

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client '" + client.AccountName + "' failed Authentication.");
                        Console.ResetColor();

                        client.Send(ref wrongLogin);

                        client.Server.DisconnectClient(client);

                        break;
                    }
                    else
                    {
                        // All's well, send CharacterList Packet

                        CharacterListPacket charlist = new CharacterListPacket();

                        charlist.SendPacket(client, client.AccountName);
                    }

                    break;

                case 0x16:
                    // Player selected a character and client sends us selected characters ID
                    reader.ReadBytes(20);
                    int selectedCharID = reader.PopInt();
                    reader.Finish();

                    if (cbl.IsCharacterOnAccount(client, selectedCharID) == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(
                            "Client '" + client.AccountName + "' tried to log in as CharID " + selectedCharID
                            + " but it is not on their account!");
                        Console.ResetColor();

                        // NV: Is this really what we want to send? Should find out sometime...
                        client.Send(ref wrongLogin);

                        client.Server.DisconnectClient(client);

                        break;
                    }
                    if (OnlineChars.IsOnline(selectedCharID))
                    {
                        Console.WriteLine(
                            "Client '" + client.AccountName
                            + "' is trying to login, but the requested character is already logged in.");
                        client.Send(ref wrongLogin);
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
                    byte[] ZonePort =
                        BitConverter.GetBytes(Convert.ToInt32(ConfigReadWrite.Instance.CurrentConfig.ZonePort));

                    PacketWriter writer = new PacketWriter();
                    writer.PushByte(0xDF);
                    writer.PushByte(0xDF);
                    writer.PushShort(1); // Packet type
                    writer.PushShort(1);
                    writer.PushShort(0); // Packet length (will be set in packet writer)
                    writer.PushInt(1);
                    writer.PushInt(0x615B); // Server ID
                    writer.PushInt(23); // Packet ID
                    writer.PushInt(selectedCharID);
                    writer.PushBytes(ZoneIP);
                    writer.PushByte(ZonePort[1]);
                    writer.PushByte(ZonePort[0]);
                    writer.PushShort(0);
                    writer.PushInt(0);
                    writer.PushInt(0);
                    Byte[] pkt = writer.Finish();
                    client.Send(ref pkt);
                    break;

                case 0x55:
                    /* player ask for 'suggest name'; server sends 'random' name. */
                    char_name.GetRandomName(client, reader.PopInt());
                    reader.Finish();
                    break;

                case 0xf:
                    /* client created new character */
                    Int32 character_id;
                    bool start_in_sl = false;
                    char_name.AccountName = client.AccountName;

                    /* start reading packet */

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // As of 18.5.1 heading for this packet is 69 bytes (65 bytes for lower versions)
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    reader.ReadBytes(69);

                    /* name length */
                    int m_name_len = reader.PopInt();
                    /* name */
                    char_name.Name = reader.PopString(m_name_len);
                    /* breed */
                    char_name.Breed = reader.PopInt();
                    /* gender */
                    char_name.Gender = reader.PopInt();
                    /* profession */
                    char_name.Profession = reader.PopInt();
                    /* level (should always be 0 )*/
                    char_name.Level = reader.PopInt();
                    /* lets skip some stuff */
                    int skip_len = reader.PopInt();
                    reader.ReadBytes(skip_len + 8);
                    /* head mesh */
                    char_name.HeadMesh = reader.PopInt();
                    /* monster scale */
                    char_name.MonsterScale = reader.PopInt();
                    /* fatness */
                    char_name.Fatness = reader.PopInt();
                    /* start in SL? */
                    int sl = reader.PopInt();
                    if (sl == 1)
                    {
                        start_in_sl = true;
                    }
                    /* close reader and stream */
                    reader.Finish();

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
                    character_id = char_name.CheckAgainstDatabase();
                    if (character_id < 1)
                    {
                        char_name.SendNameInUse(client);
                    }
                    else
                    {
                        char_name.SendNameToStartPlayfield(client, start_in_sl, character_id);
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
                    reader.ReadBytes(20);
                    uid = reader.PopInt();
                    char_name.DeleteChar(client, uid);
                    reader.Finish();
                    break;
                default:
                    reader.Finish();
                    client.Server.Warning(client, "Client sent unknown message {0}", messageNumber.ToString());
                    break;
            }
        }
    }
}