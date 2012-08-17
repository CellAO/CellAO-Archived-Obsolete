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

namespace ZoneEngine.PacketHandlers
{
    using AO.Core;

    using ZoneEngine.Misc;

    public class RaidClass
    {
        private uint raidID;

        private bool raidEnabled;

        private int numberOfTeams;

        private uint raidLeader;

        private int raidLocks;

        private uint team1ID;

        private uint team2ID;

        private uint team3ID;

        private uint team4ID;

        private uint team5ID;

        private uint team6ID;

        // : TODO
    }

    public class TeamClass
    {
        private uint teamID;

        private int numberOfPlayers;

        private uint teamLeader;

        private int lootMode; // 0 : All, 1 : Alpha, 2 : Leader.

        private uint plr1ID;

        private uint plr2ID;

        private uint plr3ID;

        private uint plr4ID;

        private uint plr5ID;

        private uint plr6ID;

        public void LeaveTeam(Client sendingPlayer)
        {
            // Send Team Request To Other Player

            PacketWriter pktTeamRequest = new PacketWriter();
            pktTeamRequest.PushByte(0xDF);
            pktTeamRequest.PushByte(0xDF); // Header
            pktTeamRequest.PushShort(0xA); // Packet Type
            pktTeamRequest.PushShort(1); // Unknown 1
            pktTeamRequest.PushShort(0); // Legnth
            pktTeamRequest.PushInt(3086); // Sender
            pktTeamRequest.PushInt(sendingPlayer.Character.ID); // Reciever
            pktTeamRequest.PushInt(0x5e477770); // Packet ID
            pktTeamRequest.PushIdentity(50000, sendingPlayer.Character.ID); // TYPE / ID
            pktTeamRequest.PushByte(0);
            pktTeamRequest.PushInt(0x20); // Action ID
            pktTeamRequest.PushInt(0);
            pktTeamRequest.PushInt(50000);
            pktTeamRequest.PushInt(sendingPlayer.Character.ID);
            pktTeamRequest.PushInt(0x2EA0022); // Team ID Variable Goes Here
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushShort(0);

            byte[] teamRequestPacket = pktTeamRequest.Finish();
            sendingPlayer.SendCompressed(teamRequestPacket);
        }

        public void SendTeamRequest(Client sendingPlayer, Identity recievingPlayer)
        {
            if (sendingPlayer.Character.ID != recievingPlayer.Instance)
            {
                // Send Team Request To Other Player

                PacketWriter pktTeamRequest = new PacketWriter();
                pktTeamRequest.PushByte(0xDF);
                pktTeamRequest.PushByte(0xDF); // Header
                pktTeamRequest.PushShort(0xA); // Packet Type
                pktTeamRequest.PushShort(1); // Unknown 1
                pktTeamRequest.PushShort(0); // Legnth
                pktTeamRequest.PushInt(3086); // Sender
                pktTeamRequest.PushInt(recievingPlayer.Instance); // Reciever
                pktTeamRequest.PushInt(0x5e477770); // Packet ID
                pktTeamRequest.PushIdentity(recievingPlayer); // TYPE / ID
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushByte(0x1A); // Action ID
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushInt(recievingPlayer.Type);
                pktTeamRequest.PushInt(sendingPlayer.Character.ID);
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushInt(1);
                pktTeamRequest.PushShort(0);

                byte[] teamRequestPacket = pktTeamRequest.Finish();
                Client receiver = FindClient.FindClientById(recievingPlayer.Instance);
                if (receiver != null)
                {
                    receiver.SendCompressed(teamRequestPacket);
                }
            }
        }

        // Create a New Team ID and Return it to Call

        public uint GenerateNewTeamId(Client sendingPlayer, Identity recievingPlayer)
        {
            // Generate TeamID

            // Check Current Available Team Number

            // Assign current Team Number

            // Apply To Variable in Core to be accessed 

            uint newTeamId = 7;
            return newTeamId;
        }

        // Team Request Reply : CharAction 15

        public void TeamRequestReply(Client sendingPlayer, Identity recievingPlayer)
        {
            // Accept Team Request CharAction Hex:15
            PacketWriter pktCharAction15 = new PacketWriter();
            pktCharAction15.PushByte(0xDF);
            pktCharAction15.PushByte(0xDF); // Header
            pktCharAction15.PushShort(0xA); // Packet Type
            pktCharAction15.PushShort(1); // Unknown 1
            pktCharAction15.PushShort(0); // Legnth
            pktCharAction15.PushInt(3086); // Sender
            pktCharAction15.PushInt(sendingPlayer.Character.ID); // Reciever
            pktCharAction15.PushInt(0x5e477770); // Packet ID
            pktCharAction15.PushIdentity(50000, sendingPlayer.Character.ID); // TYPE / ID
            pktCharAction15.PushByte(0);
            pktCharAction15.PushInt(0x15); // Action ID
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0x11); // ??
            pktCharAction15.PushShort(0);
            byte[] characterAction15Packet = pktCharAction15.Finish();

            // IF Statement Determining Destination Client to Send Packet To

            Client receiver = FindClient.FindClientById(sendingPlayer.Character.ID);
            if (receiver != null)
            {
                receiver.SendCompressed(characterAction15Packet);
            }
        }

        // Team Reply Packet 46312D2E : TeamMember

        public void TeamReplyPacketTeamMember(int destinationClient, Client sendingPlayer, Identity recievingPlayer, string charName)
        {
            PacketWriter packetWriter = new PacketWriter();
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF); // Header
            packetWriter.PushShort(0xA); // Packet Type
            packetWriter.PushShort(1); // Unknown 1
            packetWriter.PushShort(0); // Legnth
            packetWriter.PushInt(3086); // Sender

            switch (destinationClient)
            {
                case 0:
                    packetWriter.PushInt(sendingPlayer.Character.ID); // Reciever
                    packetWriter.PushInt(0x46312D2E); // Packet ID
                    packetWriter.PushIdentity(50000, sendingPlayer.Character.ID); // TYPE / ID
                    packetWriter.PushInt(0);
                    packetWriter.PushIdentity(50000, sendingPlayer.Character.ID);
                    packetWriter.PushInt(0xDEA9); // Team Window Information ??
                    packetWriter.PushInt(0x7); // team ID??????
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushInt(0x48); // ??
                    packetWriter.PushShort(0x5); // ???
                    packetWriter.PushInt(0x8); // Length of Team name?
                    packetWriter.PushByte(0x4B);
                    packetWriter.PushByte(0x61);
                    packetWriter.PushByte(0x6C);
                    packetWriter.PushByte(0x69); // name?
                    packetWriter.PushByte(0x6E);
                    packetWriter.PushByte(0x61);
                    packetWriter.PushByte(0x6D);
                    packetWriter.PushByte(0x61); // Name continued?
                    packetWriter.PushShort(0);
                    byte[] packet = packetWriter.Finish();
                    Client receiver = FindClient.FindClientById(recievingPlayer.Instance);
                    if (receiver != null)
                    {
                        receiver.SendCompressed(packet);
                    }
                    break;

                case 1:
                    packetWriter.PushInt(recievingPlayer.Instance); // Reciever
                    packetWriter.PushInt(0x46312D2E); // Packet ID
                    packetWriter.PushIdentity(50000, recievingPlayer.Instance); // TYPE / ID
                    packetWriter.PushInt(0);
                    packetWriter.PushIdentity(50000, recievingPlayer.Instance);
                    packetWriter.PushInt(0xDEA9); // Team Window Information ??
                    packetWriter.PushInt(0x7); // team ID??????
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushByte(0xFF);
                    packetWriter.PushInt(0x48); // ??
                    packetWriter.PushShort(0x5); // ??
                    packetWriter.PushInt(0x8); // Length of Team name?
                    packetWriter.PushByte(0x4B);
                    packetWriter.PushByte(0x61);
                    packetWriter.PushByte(0x6C);
                    packetWriter.PushByte(0x69); // name?
                    packetWriter.PushByte(0x6E);
                    packetWriter.PushByte(0x61);
                    packetWriter.PushByte(0x6D);
                    packetWriter.PushByte(0x61); // Name continued?
                    packetWriter.PushShort(0);
                    byte[] packet2 = packetWriter.Finish();
                    sendingPlayer.SendCompressed(packet2);
                    break;
            }
        }

        // Team Reply Packet 28784248 : TeamMemberInfo + health and nano?????

        public void TeamReplyPacketTeamMemberInfo(int destinationClient, Client sendingPlayer, Identity recievingPlayer)
        {
            PacketWriter packetWriter = new PacketWriter();
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF); // Header
            packetWriter.PushShort(0xA); // Packet Type
            packetWriter.PushShort(1); // Unknown 1
            packetWriter.PushShort(0); // Legnth
            packetWriter.PushInt(3086); // Sender

            switch (destinationClient)
            {
                case 0:
                    packetWriter.PushInt(sendingPlayer.Character.ID); // Reciever
                    packetWriter.PushInt(0x46312D2E); // Packet ID
                    packetWriter.PushIdentity(50000, sendingPlayer.Character.ID); // TYPE / ID
                    packetWriter.PushInt(0);
                    packetWriter.PushIdentity(50000, sendingPlayer.Character.ID); // Team Member Information
                    packetWriter.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushShort(0);
                    byte[] packet = packetWriter.Finish();
                    Client receiver = FindClient.FindClientById(recievingPlayer.Instance);
                    if (receiver != null)
                    {
                        receiver.SendCompressed(packet);
                    }
                    break;

                case 1:
                    packetWriter.PushInt(recievingPlayer.Instance); // Reciever
                    packetWriter.PushInt(0x46312D2E); // Packet ID
                    packetWriter.PushIdentity(50000, recievingPlayer.Instance); // TYPE / ID
                    packetWriter.PushInt(0);
                    packetWriter.PushIdentity(50000, recievingPlayer.Instance); // Team Member Information
                    packetWriter.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    packetWriter.PushShort(0);
                    byte[] packet2 = packetWriter.Finish();
                    sendingPlayer.SendCompressed(packet2);
                    break;
            }
        }

        // Team Request Reply : CharAction 23

        public void TeamRequestReplyCharacterAction23(Client sendingPlayer, Identity recievingPlayer)
        {
            // Accept Team Request CharAction Hex:23
            PacketWriter pktCharAction23 = new PacketWriter();
            pktCharAction23.PushByte(0xDF);
            pktCharAction23.PushByte(0xDF); // Header
            pktCharAction23.PushShort(0xA); // Packet Type
            pktCharAction23.PushShort(1); // Unknown 1
            pktCharAction23.PushShort(0); // Legnth
            pktCharAction23.PushInt(3086); // Sender
            pktCharAction23.PushInt(sendingPlayer.Character.ID); // Reciever
            pktCharAction23.PushInt(0x5e477770); // Packet ID
            pktCharAction23.PushIdentity(50000, sendingPlayer.Character.ID); // TYPE / ID
            pktCharAction23.PushByte(0);
            pktCharAction23.PushInt(0x23); // Action ID
            pktCharAction23.PushInt(0);
            pktCharAction23.PushInt(50000);
            pktCharAction23.PushInt(sendingPlayer.Character.ID);
            pktCharAction23.PushInt(0xDEA9); // Team Window Information ??
            pktCharAction23.PushInt(0x2EA0022); // Team ID Variable Goes Here
            pktCharAction23.PushShort(0);
            byte[] packet = pktCharAction23.Finish();

            // IF Statement Determining Destination Client to Send Packet To

            Client receiver = FindClient.FindClientById(sendingPlayer.Character.ID);
            if (receiver != null)
            {
                receiver.SendCompressed(packet);
            }
        }
    }
}