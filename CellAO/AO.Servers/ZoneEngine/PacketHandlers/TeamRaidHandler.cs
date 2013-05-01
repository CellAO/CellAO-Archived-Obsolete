// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TeamRaidHandler.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the RaidClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;

    using Identity = AO.Core.Identity;

    public class RaidClass
    {
        #region Fields

        private int numberOfTeams;

        private bool raidEnabled;

        private uint raidID;

        private uint raidLeader;

        private int raidLocks;

        private uint team1ID;

        private uint team2ID;

        private uint team3ID;

        private uint team4ID;

        private uint team5ID;

        private uint team6ID;

        #endregion

        // : TODO
    }

    public class TeamClass
    {
        #region Fields

        private int lootMode; // 0 : All, 1 : Alpha, 2 : Leader.

        private int numberOfPlayers;

        private uint plr1ID;

        private uint plr2ID;

        private uint plr3ID;

        private uint plr4ID;

        private uint plr5ID;

        private uint plr6ID;

        private uint teamID;

        private uint teamLeader;

        #endregion

        #region Public Methods and Operators

        public static uint GenerateNewTeamId(Client sendingPlayer, Identity recievingPlayer)
        {
            // Generate TeamID

            // Check Current Available Team Number

            // Assign current Team Number

            // Apply To Variable in Core to be accessed 
            uint newTeamId = 7;
            return newTeamId;
        }

        public void LeaveTeam(Client sendingPlayer)
        {
            // Send Team Request To Other Player
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  sendingPlayer
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.LeaveTeam, 
                                  Target =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  sendingPlayer
                                                  .Character
                                                  .Id
                                          }, 
                                  Parameter1 = 0x02EA0022, // Team ID Variable Goes Here
                                  Parameter2 = -1, 
                                  Unknown2 = 0
                              };

            sendingPlayer.SendCompressed(message);
        }

        public void SendTeamRequest(Client sendingPlayer, Identity recievingPlayer)
        {
            if (sendingPlayer.Character.Id != recievingPlayer.Instance)
            {
                // Send Team Request To Other Player
                var message = new CharacterActionMessage
                                  {
                                      Identity =
                                          new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                              {
                                                  Type
                                                      =
                                                      (
                                                      IdentityType
                                                      )
                                                      recievingPlayer
                                                          .Type, 
                                                  Instance
                                                      =
                                                      recievingPlayer
                                                      .Instance
                                              }, 
                                      Unknown = 0x00, 
                                      Action = CharacterActionType.TeamRequest, 
                                      Target =
                                          new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                              {
                                                  Type
                                                      =
                                                      (
                                                      IdentityType
                                                      )
                                                      recievingPlayer
                                                          .Type, 
                                                  Instance
                                                      =
                                                      sendingPlayer
                                                      .Character
                                                      .Id
                                              }, 
                                      Parameter1 = 0, 
                                      Parameter2 = 1, 
                                      Unknown2 = 0
                                  };

                var receiver = FindClient.FindClientById(recievingPlayer.Instance);
                if (receiver != null)
                {
                    receiver.SendCompressed(message);
                }
            }
        }

        // Create a New Team ID and Return it to Call

        // Team Reply Packet 46312D2E : TeamMember
        public void TeamReplyPacketTeamMember(
            int destinationClient, Client sendingPlayer, Identity recievingPlayer, string charName)
        {
            switch (destinationClient)
            {
                case 0:
                    var toReceiver = new TeamMemberMessage
                                         {
                                             Identity =
                                                 new SmokeLounge.AOtomation.Messaging.GameData.
                                                 Identity
                                                     {
                                                         Type = IdentityType.CanbeAffected, 
                                                         Instance = sendingPlayer.Character.Id
                                                     }, 
                                             Unknown = 0x00, 
                                             Unknown1 = 0x00, 
                                             Unknown2 = 0x0000, 
                                             Character =
                                                 new SmokeLounge.AOtomation.Messaging.GameData.
                                                 Identity
                                                     {
                                                         Type = IdentityType.CanbeAffected, 
                                                         Instance = sendingPlayer.Character.Id
                                                     }, 
                                             Team =
                                                 new SmokeLounge.AOtomation.Messaging.GameData.
                                                 Identity
                                                     {
                                                         Type = IdentityType.TeamWindow, 
                                                         Instance = 0x00000007
                                                     }, 
                                             Unknown3 = 0xFFFFFFFF, 
                                             Unknown4 = 0x00000048, 
                                             Unknown5 = 0x0005, 
                                             Name = "Kalinama", 
                                             Unknown6 = 0x0000
                                         };

                    var receiver = FindClient.FindClientById(recievingPlayer.Instance);
                    if (receiver != null)
                    {
                        receiver.SendCompressed(toReceiver);
                    }

                    break;

                case 1:
                    var toSender = new TeamMemberMessage
                                       {
                                           Identity =
                                               new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                                   {
                                                       Type
                                                           =
                                                           IdentityType
                                                           .CanbeAffected, 
                                                       Instance
                                                           =
                                                           recievingPlayer
                                                           .Instance
                                                   }, 
                                           Unknown = 0x00, 
                                           Unknown1 = 0x00, 
                                           Unknown2 = 0x0000, 
                                           Character =
                                               new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                                   {
                                                       Type
                                                           =
                                                           IdentityType
                                                           .CanbeAffected, 
                                                       Instance
                                                           =
                                                           recievingPlayer
                                                           .Instance
                                                   }, 
                                           Team =
                                               new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                                   {
                                                       Type
                                                           =
                                                           IdentityType
                                                           .TeamWindow, 
                                                       Instance
                                                           =
                                                           0x00000007
                                                   }, 
                                           Unknown3 = 0xFFFFFFFF, 
                                           Unknown4 = 0x00000048, 
                                           Unknown5 = 0x0005, 
                                           Name = "Kalinama", 
                                           Unknown6 = 0x0000
                                       };

                    sendingPlayer.SendCompressed(toSender);
                    break;
            }
        }

        // Team Reply Packet 28784248 : TeamMemberInfo + health and nano?????
        public void TeamReplyPacketTeamMemberInfo(int destinationClient, Client sendingPlayer, Identity recievingPlayer)
        {
            switch (destinationClient)
            {
                case 0:
                    var toReceiver = new TeamMemberInfoMessage
                                         {
                                             Identity =
                                                 new SmokeLounge.AOtomation.Messaging.GameData.
                                                 Identity
                                                     {
                                                         Type = IdentityType.CanbeAffected, 
                                                         Instance = sendingPlayer.Character.Id
                                                     }, 
                                             Unknown = 0x00, 
                                             Unknown1 = 0x00, 
                                             Unknown2 = 0x0000, 
                                             Character =
                                                 new SmokeLounge.AOtomation.Messaging.GameData.
                                                 Identity
                                                     {
                                                         Type = IdentityType.CanbeAffected, 
                                                         Instance = sendingPlayer.Character.Id
                                                     }, 
                                             Unknown3 = 0x000005F4, // HP/NANO?? Actual/MAX???
                                             Unknown4 = 0x000005F4, // HP/NANO?? Actual/MAX???
                                             Unknown5 = 0x000002F4, // HP/NANO?? Actual/MAX???
                                             Unknown6 = 0x000002F4, // HP/NANO?? Actual/MAX???
                                             Unknown7 = 0x0000
                                         };

                    var receiver = FindClient.FindClientById(recievingPlayer.Instance);
                    if (receiver != null)
                    {
                        receiver.SendCompressed(toReceiver);
                    }

                    break;

                case 1:
                    var toSender = new TeamMemberInfoMessage
                                       {
                                           Identity =
                                               new SmokeLounge.AOtomation.Messaging.GameData.
                                               Identity
                                                   {
                                                       Type = IdentityType.CanbeAffected, 
                                                       Instance = recievingPlayer.Instance
                                                   }, 
                                           Unknown = 0x00, 
                                           Unknown1 = 0x00, 
                                           Unknown2 = 0x0000, 
                                           Character =
                                               new SmokeLounge.AOtomation.Messaging.GameData.
                                               Identity
                                                   {
                                                       Type = IdentityType.CanbeAffected, 
                                                       Instance = recievingPlayer.Instance
                                                   }, 
                                           Unknown3 = 0x000005F4, // HP/NANO?? Actual/MAX???
                                           Unknown4 = 0x000005F4, // HP/NANO?? Actual/MAX???
                                           Unknown5 = 0x000002F4, // HP/NANO?? Actual/MAX???
                                           Unknown6 = 0x000002F4, // HP/NANO?? Actual/MAX???
                                           Unknown7 = 0x0000
                                       };

                    sendingPlayer.SendCompressed(toSender);
                    break;
            }
        }

        public void TeamRequestReply(Client sendingPlayer, Identity recievingPlayer)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  sendingPlayer
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TeamRequestReply, 
                                  Target =
                                      SmokeLounge.AOtomation.Messaging.GameData.Identity.None, 
                                  Parameter1 = 0, 
                                  Parameter2 = 0x11, // ??
                                  Unknown2 = 0
                              };

            // IF Statement Determining Destination Client to Send Packet To
            var receiver = FindClient.FindClientById(sendingPlayer.Character.Id);
            if (receiver != null)
            {
                receiver.SendCompressed(message);
            }
        }

        // Team Request Reply : CharAction 23
        public void TeamRequestReplyCharacterAction23(Client sendingPlayer, Identity recievingPlayer)
        {
            // Accept Team Request CharAction Hex:23
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  sendingPlayer
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.AcceptTeamRequest, 
                                  Target =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  sendingPlayer
                                                  .Character
                                                  .Id
                                          }, 
                                  Parameter1 = (int)IdentityType.TeamWindow, 
                                  Parameter2 = 0x2EA0022, // Team ID Variable Goes Here
                                  Unknown2 = 0
                              };

            // IF Statement Determining Destination Client to Send Packet To
            var receiver = FindClient.FindClientById(sendingPlayer.Character.Id);
            if (receiver != null)
            {
                receiver.SendCompressed(message);
            }
        }

        #endregion
    }
}