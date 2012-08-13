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
    using AO.Core;

    using ZoneEngine.Misc;

    public class RaidClass
    {
        private uint RaidID;

        private bool RaidEnabled;

        private int NumOfTeams;

        private uint RaidLeader;

        private int RaidLocks;

        private uint Team1ID;

        private uint Team2ID;

        private uint Team3ID;

        private uint Team4ID;

        private uint Team5ID;

        private uint Team6ID;

        // : TODO
    }

    public class TeamClass
    {
        private uint TeamID;

        private int NumOfPlayers;

        private uint TeamLeader;

        private int LootMode; // 0 : All, 1 : Alpha, 2 : Leader.

        private uint plr1ID;

        private uint plr2ID;

        private uint plr3ID;

        private uint plr4ID;

        private uint plr5ID;

        private uint plr6ID;

        public void LeaveTeam(Client SendingPlayer)
        {
            // Send Team Request To Other Player

            PacketWriter pktTeamRequest = new PacketWriter();
            pktTeamRequest.PushByte(0xDF);
            pktTeamRequest.PushByte(0xDF); // Header
            pktTeamRequest.PushShort(0xA); // Packet Type
            pktTeamRequest.PushShort(1); // Unknown 1
            pktTeamRequest.PushShort(0); // Legnth
            pktTeamRequest.PushInt(3086); // Sender
            pktTeamRequest.PushInt(SendingPlayer.Character.ID); // Reciever
            pktTeamRequest.PushInt(0x5e477770); // Packet ID
            pktTeamRequest.PushIdentity(50000, SendingPlayer.Character.ID); // TYPE / ID
            pktTeamRequest.PushByte(0);
            pktTeamRequest.PushInt(0x20); // Action ID
            pktTeamRequest.PushInt(0);
            pktTeamRequest.PushInt(50000);
            pktTeamRequest.PushInt(SendingPlayer.Character.ID);
            pktTeamRequest.PushInt(0x2EA0022); // Team ID Variable Goes Here
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushByte(0xFF);
            pktTeamRequest.PushShort(0);

            byte[] TeamRequestPacket = pktTeamRequest.Finish();
            SendingPlayer.SendCompressed(TeamRequestPacket);
        }

        public void SendTeamRequest(Client SendingPlayer, Identity RecievingPlayer)
        {
            if (SendingPlayer.Character.ID != RecievingPlayer.Instance)
            {
                // Send Team Request To Other Player

                PacketWriter pktTeamRequest = new PacketWriter();
                pktTeamRequest.PushByte(0xDF);
                pktTeamRequest.PushByte(0xDF); // Header
                pktTeamRequest.PushShort(0xA); // Packet Type
                pktTeamRequest.PushShort(1); // Unknown 1
                pktTeamRequest.PushShort(0); // Legnth
                pktTeamRequest.PushInt(3086); // Sender
                pktTeamRequest.PushInt(RecievingPlayer.Instance); // Reciever
                pktTeamRequest.PushInt(0x5e477770); // Packet ID
                pktTeamRequest.PushIdentity(RecievingPlayer); // TYPE / ID
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushByte(0x1A); // Action ID
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushInt(RecievingPlayer.Type);
                pktTeamRequest.PushInt(SendingPlayer.Character.ID);
                pktTeamRequest.PushInt(0);
                pktTeamRequest.PushInt(1);
                pktTeamRequest.PushShort(0);

                byte[] TeamRequestPacket = pktTeamRequest.Finish();
                Client TRClient = FindClient.FindClientByID(RecievingPlayer.Instance);
                TRClient.SendCompressed(TeamRequestPacket);
            }
        }

        // Create a New Team ID and Return it to Call

        public uint GenerateNewTeamID(Client SendingPlayer, Identity RecievingPlayer)
        {
            // Generate TeamID

            // Check Current Available Team Number

            // Assign current Team Number

            // Apply To Variable in Core to be accessed 

            uint NewTeamID = 7;
            return NewTeamID;
        }

        // Team Request Reply : CharAction 15

        public void TRRCA15(Client SendingPlayer, Identity RecievingPlayer)
        {
            // Accept Team Request CharAction Hex:15
            PacketWriter pktCharAction15 = new PacketWriter();
            pktCharAction15.PushByte(0xDF);
            pktCharAction15.PushByte(0xDF); // Header
            pktCharAction15.PushShort(0xA); // Packet Type
            pktCharAction15.PushShort(1); // Unknown 1
            pktCharAction15.PushShort(0); // Legnth
            pktCharAction15.PushInt(3086); // Sender
            pktCharAction15.PushInt(SendingPlayer.Character.ID); // Reciever
            pktCharAction15.PushInt(0x5e477770); // Packet ID
            pktCharAction15.PushIdentity(50000, SendingPlayer.Character.ID); // TYPE / ID
            pktCharAction15.PushByte(0);
            pktCharAction15.PushInt(0x15); // Action ID
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0);
            pktCharAction15.PushInt(0x11); // ??
            pktCharAction15.PushShort(0);
            byte[] CharAction15Packet = pktCharAction15.Finish();

            // IF Statement Determining Destination Client to Send Packet To

            Client TRRClient = FindClient.FindClientByID(SendingPlayer.Character.ID);
            TRRClient.SendCompressed(CharAction15Packet);
        }

        // Team Reply Packet 46312D2E : TeamMember

        public void TRPTM(int DestinationClient, Client SendingPlayer, Identity RecievingPlayer, string CharName)
        {
            PacketWriter TRPTM = new PacketWriter();
            TRPTM.PushByte(0xDF);
            TRPTM.PushByte(0xDF); // Header
            TRPTM.PushShort(0xA); // Packet Type
            TRPTM.PushShort(1); // Unknown 1
            TRPTM.PushShort(0); // Legnth
            TRPTM.PushInt(3086); // Sender

            switch (DestinationClient)
            {
                case 0:
                    TRPTM.PushInt(SendingPlayer.Character.ID); // Reciever
                    TRPTM.PushInt(0x46312D2E); // Packet ID
                    TRPTM.PushIdentity(50000, SendingPlayer.Character.ID); // TYPE / ID
                    TRPTM.PushInt(0);
                    TRPTM.PushIdentity(50000, SendingPlayer.Character.ID);
                    TRPTM.PushInt(0xDEA9); // Team Window Information ??
                    TRPTM.PushInt(0x7); // team ID??????
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushInt(0x48); // ??
                    TRPTM.PushShort(0x5); // ???
                    TRPTM.PushInt(0x8); // ????
                    TRPTM.PushByte(0x4B);
                    TRPTM.PushByte(0x61);
                    TRPTM.PushByte(0x6C);
                    TRPTM.PushByte(0x69); // name?
                    TRPTM.PushByte(0x6E);
                    TRPTM.PushByte(0x61);
                    TRPTM.PushByte(0x6D);
                    TRPTM.PushByte(0x61); // Name continued?
                    TRPTM.PushShort(0);
                    byte[] TRPTMPacket = TRPTM.Finish();
                    Client cTRPTM = FindClient.FindClientByID(RecievingPlayer.Instance);
                    cTRPTM.SendCompressed(TRPTMPacket);
                    break;

                case 1:
                    TRPTM.PushInt(RecievingPlayer.Instance); // Reciever
                    TRPTM.PushInt(0x46312D2E); // Packet ID
                    TRPTM.PushIdentity(50000, RecievingPlayer.Instance); // TYPE / ID
                    TRPTM.PushInt(0);
                    TRPTM.PushIdentity(50000, RecievingPlayer.Instance);
                    TRPTM.PushInt(0xDEA9); // Team Window Information ??
                    TRPTM.PushInt(0x7); // team ID??????
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushByte(0xFF);
                    TRPTM.PushInt(0x48); // ??
                    TRPTM.PushShort(0x5); // ??
                    TRPTM.PushInt(0x8); // ????
                    TRPTM.PushByte(0x4B);
                    TRPTM.PushByte(0x61);
                    TRPTM.PushByte(0x6C);
                    TRPTM.PushByte(0x69); // name?
                    TRPTM.PushByte(0x6E);
                    TRPTM.PushByte(0x61);
                    TRPTM.PushByte(0x6D);
                    TRPTM.PushByte(0x61); // Name continued?
                    TRPTM.PushShort(0);
                    byte[] TRPTMPacket2 = TRPTM.Finish();
                    SendingPlayer.SendCompressed(TRPTMPacket2);
                    break;
            }
        }

        // Team Reply Packet 28784248 : TeamMemberInfo + health and nano?????

        public void TRPTMI(int DestinationClient, Client SendingPlayer, Identity RecievingPlayer)
        {
            PacketWriter TRPTMI = new PacketWriter();
            TRPTMI.PushByte(0xDF);
            TRPTMI.PushByte(0xDF); // Header
            TRPTMI.PushShort(0xA); // Packet Type
            TRPTMI.PushShort(1); // Unknown 1
            TRPTMI.PushShort(0); // Legnth
            TRPTMI.PushInt(3086); // Sender

            switch (DestinationClient)
            {
                case 0:
                    TRPTMI.PushInt(SendingPlayer.Character.ID); // Reciever
                    TRPTMI.PushInt(0x46312D2E); // Packet ID
                    TRPTMI.PushIdentity(50000, SendingPlayer.Character.ID); // TYPE / ID
                    TRPTMI.PushInt(0);
                    TRPTMI.PushIdentity(50000, SendingPlayer.Character.ID); // Team Member Information
                    TRPTMI.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushShort(0);
                    byte[] TRPTMIPacket = TRPTMI.Finish();
                    Client cTRPTMI = FindClient.FindClientByID(RecievingPlayer.Instance);
                    cTRPTMI.SendCompressed(TRPTMIPacket);
                    break;

                case 1:
                    TRPTMI.PushInt(RecievingPlayer.Instance); // Reciever
                    TRPTMI.PushInt(0x46312D2E); // Packet ID
                    TRPTMI.PushIdentity(50000, RecievingPlayer.Instance); // TYPE / ID
                    TRPTMI.PushInt(0);
                    TRPTMI.PushIdentity(50000, RecievingPlayer.Instance); // Team Member Information
                    TRPTMI.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x05F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushInt(0x02F4); // HP/NANO?? Actual/MAX???
                    TRPTMI.PushShort(0);
                    byte[] TRPTMIPacket2 = TRPTMI.Finish();
                    SendingPlayer.SendCompressed(TRPTMIPacket2);
                    break;
            }
        }

        // Team Request Reply : CharAction 23

        public void TRRCA23(Client SendingPlayer, Identity RecievingPlayer)
        {
            // Accept Team Request CharAction Hex:23
            PacketWriter pktCharAction23 = new PacketWriter();
            pktCharAction23.PushByte(0xDF);
            pktCharAction23.PushByte(0xDF); // Header
            pktCharAction23.PushShort(0xA); // Packet Type
            pktCharAction23.PushShort(1); // Unknown 1
            pktCharAction23.PushShort(0); // Legnth
            pktCharAction23.PushInt(3086); // Sender
            pktCharAction23.PushInt(SendingPlayer.Character.ID); // Reciever
            pktCharAction23.PushInt(0x5e477770); // Packet ID
            pktCharAction23.PushIdentity(50000, SendingPlayer.Character.ID); // TYPE / ID
            pktCharAction23.PushByte(0);
            pktCharAction23.PushInt(0x23); // Action ID
            pktCharAction23.PushInt(0);
            pktCharAction23.PushInt(50000);
            pktCharAction23.PushInt(SendingPlayer.Character.ID);
            pktCharAction23.PushInt(0xDEA9); // Team Window Information ??
            pktCharAction23.PushInt(0x2EA0022); // Team ID Variable Goes Here
            pktCharAction23.PushShort(0);
            byte[] CharAction23Packet = pktCharAction23.Finish();

            // IF Statement Determining Destination Client to Send Packet To

            Client TRR2Client = FindClient.FindClientByID(SendingPlayer.Character.ID);
            TRR2Client.SendCompressed(CharAction23Packet);
        }
    }
}