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

namespace ZoneEngine.PacketHandlers
{
    using System;
    using System.Data;
    using System.Text;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    public static class CharacterAction
    {
        public static void Read(ref byte[] packet, Client client)
        {
            SqlWrapper mys = new SqlWrapper();

            // Packet Reader Unknown Values are Returning 0 Integers, Unable to Store Needed Packet data To Reply.

            #region PacketReader
            PacketReader m_reader = new PacketReader(ref packet);
            Header m_header = m_reader.PopHeader(); // 0 - 28
            byte unknown = m_reader.PopByte(); // 29
            int actionNum = m_reader.PopInt(); // 30 - 33
            int unknown1 = m_reader.PopInt(); // 34 - 37
            Identity m_ident = m_reader.PopIdentity(); // 38 - 35
            int unknown2 = m_reader.PopInt(); // 36 - 39
            int unknown3 = m_reader.PopInt(); // 40 - 43
            short unknown4 = m_reader.PopShort(); // 44 - 45
            #endregion

            switch (actionNum)
            {
                    #region Cast nano
                case 19: // Cast nano
                    {
                        // CastNanoSpell
                        PacketWriter castNanoSpell = new PacketWriter();
                        castNanoSpell.PushByte(0xDF);
                        castNanoSpell.PushByte(0xDF);
                        castNanoSpell.PushShort(10);
                        castNanoSpell.PushShort(1);
                        castNanoSpell.PushShort(0);
                        castNanoSpell.PushInt(3086);
                        castNanoSpell.PushInt(client.Character.ID);
                        castNanoSpell.PushInt(0x25314D6D);
                        castNanoSpell.PushIdentity(50000, client.Character.ID);
                        castNanoSpell.PushByte(0);
                        castNanoSpell.PushInt(unknown3); // Nano ID
                        castNanoSpell.PushIdentity(m_ident); // Target
                        castNanoSpell.PushInt(0);
                        castNanoSpell.PushIdentity(50000, client.Character.ID); // Caster
                        byte[] castNanoSpellA = castNanoSpell.Finish();
                        Announce.Playfield(client.Character.PlayField, ref castNanoSpellA);

                        // CharacterAction 107
                        PacketWriter characterAction107 = new PacketWriter();
                        characterAction107.PushByte(0xDF);
                        characterAction107.PushByte(0xDF);
                        characterAction107.PushShort(10);
                        characterAction107.PushShort(1);
                        characterAction107.PushShort(0);
                        characterAction107.PushInt(3086);
                        characterAction107.PushInt(client.Character.ID);
                        characterAction107.PushInt(0x5E477770);
                        characterAction107.PushIdentity(50000, client.Character.ID);
                        characterAction107.PushByte(0);
                        characterAction107.PushInt(107);
                        characterAction107.PushInt(0);
                        characterAction107.PushInt(0);
                        characterAction107.PushInt(0);
                        characterAction107.PushInt(1);
                        characterAction107.PushInt(unknown3);
                        characterAction107.PushShort(0);
                        byte[] characterAction107A = characterAction107.Finish();
                        Announce.Playfield(client.Character.PlayField, ref characterAction107A);

                        // CharacterAction 98
                        PacketWriter characterAction98 = new PacketWriter();
                        characterAction98.PushByte(0xDF);
                        characterAction98.PushByte(0xDF);
                        characterAction98.PushShort(10);
                        characterAction98.PushShort(1);
                        characterAction98.PushShort(0);
                        characterAction98.PushInt(3086);
                        characterAction98.PushInt(client.Character.ID);
                        characterAction98.PushInt(0x5E477770);
                        characterAction98.PushIdentity(m_ident);
                        characterAction98.PushByte(0);
                        characterAction98.PushInt(98);
                        characterAction98.PushInt(0);
                        characterAction98.PushInt(0xCF1B);
                        characterAction98.PushInt(unknown3);
                        characterAction98.PushInt(client.Character.ID);
                        characterAction98.PushInt(0x249F0); // duration?
                        characterAction98.PushShort(0);
                        byte[] characterAction98A = characterAction98.Finish();
                        Announce.Playfield(client.Character.PlayField, ref characterAction98A);
                    }
                    break;
                    #endregion

                    #region search
                    /* this is here to prevent server crash that is caused by
                 * search action if server doesn't reply if something is
                 * found or not */
                case 66: // If action == search
                    {
                        /* Msg 110:136744723 = "No hidden objects found." */
                        client.SendFeedback(110, 136744723);
                    }
                    break;
                    #endregion

                    #region info
                case 105: // If action == Info Request
                    {
                        Client tPlayer = null;
                        if (FindClient.FindClientByID(m_ident.Instance, out tPlayer))
                        {
                            #region Titles
                            uint LegacyScore = tPlayer.Character.Stats.PvP_Rating.StatBaseValue;
                            string LegacyTitle = null;
                            if (LegacyScore < 1400)
                            {
                                LegacyTitle = "";
                            }
                            else if (LegacyScore < 1500)
                            {
                                LegacyTitle = "Freshman";
                            }
                            else if (LegacyScore < 1600)
                            {
                                LegacyTitle = "Rookie";
                            }
                            else if (LegacyScore < 1700)
                            {
                                LegacyTitle = "Apprentice";
                            }
                            else if (LegacyScore < 1800)
                            {
                                LegacyTitle = "Novice";
                            }
                            else if (LegacyScore < 1900)
                            {
                                LegacyTitle = "Neophyte";
                            }
                            else if (LegacyScore < 2000)
                            {
                                LegacyTitle = "Experienced";
                            }
                            else if (LegacyScore < 2100)
                            {
                                LegacyTitle = "Expert";
                            }
                            else if (LegacyScore < 2300)
                            {
                                LegacyTitle = "Master";
                            }
                            else if (LegacyScore < 2500)
                            {
                                LegacyTitle = "Champion";
                            }
                            else
                            {
                                LegacyTitle = "Grand Master";
                            }
                            #endregion

                            int orgGoverningForm = 0;
                            SqlWrapper ms = new SqlWrapper();
                            DataTable dt =
                                ms.ReadDT(
                                    "SELECT `GovernmentForm` FROM organizations WHERE ID=" + tPlayer.Character.OrgId);

                            if (dt.Rows.Count > 0)
                            {
                                orgGoverningForm = (Int32)dt.Rows[0][0];
                            }

                            string orgRank = OrgClient.GetRank(
                                orgGoverningForm, tPlayer.Character.Stats.ClanLevel.StatBaseValue);
                            // Uses methods in ZoneEngine\PacketHandlers\OrgClient.cs
                            /* Known packetFlags--
                             * 0x40 - No org | 0x41 - Org | 0x43 - Org and towers | 0x47 - Org, towers, player has personal towers | 0x50 - No pvp data shown
                             * Bitflags--
                             * Bit0 = hasOrg, Bit1 = orgTowers, Bit2 = personalTowers, Bit3 = (Int32) time until supression changes (Byte) type of supression level?, Bit4 = noPvpDataShown, Bit5 = hasFaction, Bit6 = ?, Bit 7 = null.
                            */
                            byte packetFlags = 0x40; // Player has no Org
                            if (tPlayer.Character.OrgId != 0)
                            {
                                packetFlags = 0x41; // Player has Org, no towers
                            }
                            PacketWriter infoPacket = new PacketWriter();

                            // Start packet header
                            infoPacket.PushByte(0xDF);
                            infoPacket.PushByte(0xDF);
                            infoPacket.PushShort(10);
                            infoPacket.PushShort(1);
                            infoPacket.PushShort(0);
                            infoPacket.PushInt(3086); // sender (server ID)
                            infoPacket.PushInt(client.Character.ID); // receiver 
                            infoPacket.PushInt(0x4D38242E); // packet ID
                            infoPacket.PushIdentity(50000, tPlayer.Character.ID); // affected identity
                            infoPacket.PushByte(0); // ?
                            // End packet header

                            infoPacket.PushByte(packetFlags); // Based on flags above
                            infoPacket.PushByte(1); // esi_001?
                            infoPacket.PushByte((byte)tPlayer.Character.Stats.Profession.Value); // Profession
                            infoPacket.PushByte((byte)tPlayer.Character.Stats.Level.Value); // Level
                            infoPacket.PushByte((byte)tPlayer.Character.Stats.TitleLevel.Value); // Titlelevel
                            infoPacket.PushByte((byte)tPlayer.Character.Stats.VisualProfession.Value);
                            // Visual Profession
                            infoPacket.PushShort(0); // Side XP Bonus
                            infoPacket.PushUInt(tPlayer.Character.Stats.Health.Value); // Current Health (Health)
                            infoPacket.PushUInt(tPlayer.Character.Stats.Life.Value); // Max Health (Life)
                            infoPacket.PushInt(0); // BreedHostility?
                            infoPacket.PushUInt(tPlayer.Character.OrgId); // org ID
                            infoPacket.PushShort((short)tPlayer.Character.FirstName.Length);
                            infoPacket.PushBytes(Encoding.ASCII.GetBytes(tPlayer.Character.FirstName));
                            infoPacket.PushShort((short)tPlayer.Character.LastName.Length);
                            infoPacket.PushBytes(Encoding.ASCII.GetBytes(tPlayer.Character.LastName));
                            infoPacket.PushShort((short)LegacyTitle.Length);
                            infoPacket.PushBytes(Encoding.ASCII.GetBytes(LegacyTitle));
                            infoPacket.PushShort(0); // Title 2

                            // If receiver is in the same org as affected identity, whom is not orgless, send org rank and city playfield
                            if ((client.Character.OrgId == tPlayer.Character.OrgId) && (tPlayer.Character.OrgId != 0))
                            {
                                infoPacket.PushShort((short)orgRank.Length);
                                infoPacket.PushBytes(Encoding.ASCII.GetBytes(orgRank));
                                infoPacket.PushInt(0);
                                //infoPacket.PushIdentity(0, 0); // City (50201, Playfield) // Pushed 1 zero to much and screwed info for characters in orgs, but I´ll leave it for later just incase.
                            }

                            infoPacket.PushUInt(tPlayer.Character.Stats.InvadersKilled.Value); // Invaders Killed
                            infoPacket.PushUInt(tPlayer.Character.Stats.KilledByInvaders.Value); // Killed by Invaders
                            infoPacket.PushUInt(tPlayer.Character.Stats.AlienLevel.Value); // Alien Level
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPDuelKills.Value); // Pvp Duel Kills 
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPDuelDeaths.Value); // Pvp Duel Deaths
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPProfessionDuelDeaths.Value);
                            // Pvp Profession Duel Kills 
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPRankedSoloKills.Value); // Pvp Solo Kills
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPRankedSoloDeaths.Value); // Pvp Team Kills
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPSoloScore.Value); // Pvp Solo Score
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPTeamScore.Value); // Pvp Team Score
                            infoPacket.PushUInt(tPlayer.Character.Stats.PVPDuelScore.Value); // Pvp Duel Score

                            byte[] infoPacketA = infoPacket.Finish();
                            client.SendCompressed(infoPacketA);
                        }
                        else
                        {
                            NonPC npc = (NonPC)FindDynel.FindDynelByID(m_ident.Type, m_ident.Instance);
                            if (npc != null)
                            {
                                PacketWriter infoPacket = new PacketWriter();

                                // Start packet header
                                infoPacket.PushByte(0xDF);
                                infoPacket.PushByte(0xDF);
                                infoPacket.PushShort(10);
                                infoPacket.PushShort(1);
                                infoPacket.PushShort(0);
                                infoPacket.PushInt(3086); // sender (server ID)
                                infoPacket.PushInt(client.Character.ID); // receiver 
                                infoPacket.PushInt(0x4D38242E); // packet ID
                                infoPacket.PushIdentity(50000, npc.ID); // affected identity
                                infoPacket.PushByte(0); // ?
                                // End packet header

                                infoPacket.PushByte(0x50); // npc's just have 0x50
                                infoPacket.PushByte(1); // esi_001?
                                infoPacket.PushByte((byte)npc.Stats.Profession.Value); // Profession
                                infoPacket.PushByte((byte)npc.Stats.Level.Value); // Level
                                infoPacket.PushByte((byte)npc.Stats.TitleLevel.Value); // Titlelevel
                                infoPacket.PushByte((byte)npc.Stats.VisualProfession.Value); // Visual Profession

                                infoPacket.PushShort(0); // no idea for npc's
                                infoPacket.PushUInt(npc.Stats.Health.Value); // Current Health (Health)
                                infoPacket.PushUInt(npc.Stats.Life.Value); // Max Health (Life)
                                infoPacket.PushInt(0); // BreedHostility?
                                infoPacket.PushUInt(0); // org ID
                                infoPacket.PushShort(0);
                                infoPacket.PushShort(0);
                                infoPacket.PushShort(0);
                                infoPacket.PushShort(0);
                                infoPacket.PushInt(0x499602d2);
                                infoPacket.PushInt(0x499602d2);
                                infoPacket.PushInt(0x499602d2);
                                byte[] infoPacketA = infoPacket.Finish();
                                client.SendCompressed(infoPacketA);
                            }
                        }
                    }
                    break;
                    #endregion

                    #region logout
                case 120: // If action == Logout
                    {
                        //Start 30 second logout timer if client is not a GM (statid 215)
                        if (client.Character.Stats.GmLevel.Value == 0)
                        {
                            client.startLogoutTimer();
                        }
                        else // If client is a GM, disconnect without timer
                        {
                            client.Server.DisconnectClient(client);
                        }
                    }
                    break;
                case 121: // If action == Stop Logout
                    {
                        //Stop current logout timer and send stop logout packet
                        client.Character.updateMoveType((byte)client.Character.prevMoveMode);
                        client.CancelLogOut();
                    }
                    break;
                    #endregion

                    #region stand
                case 87: // If action == Stand
                    {
                        client.Character.updateMoveType(37);
                        //Send stand up packet, and cancel timer/send stop logout packet if timer is enabled
                        client.StandCancelLogout();
                    }
                    break;
                    #endregion

                    #region Team
                case 22: //Kick Team Member
                    {
                    }
                    break;
                case 24: //Leave Team
                    {
                        TeamClass Team = new TeamClass();
                        Team.LeaveTeam(client);
                    }
                    break;
                case 25: //Transfer Team Leadership
                    {
                    }
                    break;
                case 26: //Team Join Request
                    {
                        // Send Team Invite Request To Target Player

                        TeamClass Team = new TeamClass();
                        Team.SendTeamRequest(client, m_ident);
                    }
                    break;
                case 28: //Request Reply
                    {
                        // Check if positive or negative response

                        // if positive

                        TeamClass Team = new TeamClass();
                        uint TeamID = Team.GenerateNewTeamID(client, m_ident);

                        // Destination Client 0 = Sender, 1 = Reciever

                        // Reciever Packets
                        ///////////////////

                        // CharAction 15
                        Team.TRRCA15(client, m_ident);
                        // CharAction 23
                        Team.TRRCA23(client, m_ident);

                        // TeamMember Packet
                        Team.TRPTM(1, client, m_ident, "Member1");
                        // TeamMemberInfo Packet
                        Team.TRPTMI(1, client, m_ident);
                        // TeamMember Packet
                        Team.TRPTM(1, client, m_ident, "Member2");

                        // Sender Packets
                        /////////////////

                        // TeamMember Packet
                        Team.TRPTM(0, client, m_ident, "Member1");
                        // TeamMemberInfo Packet
                        Team.TRPTMI(0, client, m_ident);
                        // TeamMember Packet
                        Team.TRPTM(0, client, m_ident, "Member2");
                    }
                    break;
                    #endregion

                    #region Delete Item
                case 0x70:
                    mys.SqlDelete(
                        "DELETE FROM " + client.Character.getSQLTablefromDynelType() + "inventory WHERE placement="
                        + m_ident.Instance.ToString() + " AND container=" + m_ident.Type.ToString());
                    InventoryEntries i_del = client.Character.getInventoryAt(m_ident.Instance);
                    client.Character.Inventory.Remove(i_del);
                    byte[] action2 = new byte[0x37];
                    Array.Copy(packet, action2, 0x37);
                    action2[8] = 0x00;
                    action2[9] = 0x00;
                    action2[10] = 0x0C;
                    action2[11] = 0x0E;
                    client.SendCompressed(action2);
                    break;
                    #endregion

                    #region Split item
                case 0x34:
                    int nextid = client.Character.GetNextFreeInventory(m_ident.Type);
                    InventoryEntries i = client.Character.getInventoryAt(m_ident.Instance);
                    i.Item.multiplecount -= unknown3;
                    InventoryEntries i2 = new InventoryEntries();
                    i2.Item = i.Item.ShallowCopy();
                    i2.Item.multiplecount = unknown3;
                    i2.Placement = nextid;
                    client.Character.Inventory.Add(i2);
                    client.Character.writeInventorytoSQL();
                    break;
                    #endregion

                    #region Join item
                case 0x35:
                    InventoryEntries j1 = client.Character.getInventoryAt(m_ident.Instance);
                    InventoryEntries j2 = client.Character.getInventoryAt(unknown3);
                    j1.Item.multiplecount += j2.Item.multiplecount;
                    client.Character.Inventory.Remove(j2);
                    client.Character.writeInventorytoSQL();

                    byte[] joined = new byte[0x37];
                    Array.Copy(packet, joined, 0x37);
                    joined[8] = 0x00;
                    joined[9] = 0x00;
                    joined[10] = 0x0C;
                    joined[11] = 0x0E;
                    client.SendCompressed(joined);
                    break;
                    #endregion

                    #region Sneak Action
                    // ###################################################################################
                    // Spandexpants: This is all i have done so far as to make sneak turn on and off, 
                    // currently i cannot find a missing packet or link which tells the server the player
                    // has stopped sneaking, hidden packet or something, will come back to later.
                    // ###################################################################################

                    // Sneak Packet Received

                case 163:
                    {
                        PacketWriter Sneak = new PacketWriter();
                        // TODO: IF SNEAKING IS ALLOWED RUN THIS CODE.
                        // Send Action 162 : Enable Sneak
                        Sneak.PushByte(0xDF);
                        Sneak.PushByte(0xDF);
                        Sneak.PushShort(0xA);
                        Sneak.PushShort(1);
                        Sneak.PushShort(0);
                        Sneak.PushInt(3086); // Send 
                        Sneak.PushInt(client.Character.ID); // Reciever
                        Sneak.PushInt(0x5e477770); // Packet ID
                        Sneak.PushIdentity(50000, client.Character.ID); // TYPE / ID
                        Sneak.PushInt(0);
                        Sneak.PushByte(0xA2); // Action ID
                        Sneak.PushInt(0);
                        Sneak.PushInt(0);
                        Sneak.PushInt(0);
                        Sneak.PushInt(0);
                        Sneak.PushInt(0);
                        Sneak.PushShort(0);
                        byte[] sneakpacket = Sneak.Finish();
                        client.SendCompressed(sneakpacket);
                        // End of Enable sneak
                        // TODO: IF SNEAKING IS NOT ALLOWED SEND REJECTION PACKET
                    }
                    break;
                    #endregion

                    #region Use Item on Item
                case 81:
                    {
                        Identity item1 = new Identity();
                        Identity item2 = new Identity();

                        item1.Type = m_ident.Type;
                        item1.Instance = m_ident.Instance;

                        item2.Type = unknown2;
                        item2.Instance = unknown3;

                        Tradeskill cts = new Tradeskill(client, item1.Instance, item2.Instance);
                        cts.ClickBuild();
                        break;
                    }
                    #endregion

                    #region Change Visual Flag
                case 166:
                    {
                        client.Character.Stats.VisualFlags.Set(unknown3);
                        // client.SendChatText("Setting Visual Flag to "+unknown3.ToString());
                        AppearanceUpdate.Appearance_Update(client.Character);
                        break;
                    }
                    #endregion

                    #region Tradeskill Source Changed
                case 0xdc:
                    TSReceiver.TSSourceChanged(client, unknown2, unknown3);
                    break;
                    #endregion

                    #region Tradeskill Target Changed
                case 0xdd:
                    TSReceiver.TSTargetChanged(client, unknown2, unknown3);
                    break;
                    #endregion

                    #region Tradeskill Build Pressed
                case 0xde:
                    TSReceiver.TSBuildPressed(client, m_ident.Instance);
                    break;
                    #endregion

                    #region default
                default:
                    {
                        byte[] action = new byte[0x37];
                        Array.Copy(packet, action, 0x37);
                        action[8] = 0x00;
                        action[9] = 0x00;
                        action[10] = 0x0C;
                        action[11] = 0x0E;
                        Announce.Playfield(client.Character.PlayField, ref action);
                    }
                    break;
                    #endregion
            }
            m_reader.Finish();
        }
    }
}