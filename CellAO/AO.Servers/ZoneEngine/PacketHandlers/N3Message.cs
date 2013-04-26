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
    using ZoneEngine.Misc;

    /// <summary>
    /// 
    /// </summary>
    public static class N3Message
    {
        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <param name="messageNumber"></param>
        public static void Parse(Client client, byte[] packet, int messageNumber)
        {
            // Some of these messages are never sent from
            // client to server, but I left them in just 
            // in case I'm wrong about some packet. And to
            // allow server to log/disconnect clients that
            // send unexpected messages. *Suiv*

            int type = (((packet[20] << 24) + (packet[21] << 16) + (packet[22] << 8) + packet[23]));
            int dynelId = (((packet[24] << 24) + (packet[25] << 16) + (packet[26] << 8) + packet[27]));

            Dynel dynel = FindDynel.FindDynelById(type, dynelId);
            switch (messageNumber)
            {
                case 0x000a0c5a:
                    //KnuBotNPCDescription
                    break;
                case 0x052e2f0c:
                    //AddTemplate
                    break;
                case 0x0639474d:
                    //GridDestinationSelect
                    break;
                case 0x0C5a5d6d:
                    //WeatherControl
                    break;
                case 0x0d381f02:
                    //PetToMaster
                    break;
                case 0x1078735a:
                    //FlushRDBCaches
                    break;
                case 0x1330734f:
                    //ShopSearchResult
                    break;
                case 0x145a4f66:
                    //ShopSearchRequest
                    break;
                case 0x166a435e:
                    //AcceptBSInvite
                    break;
                case 0x194e4f76:
                    //AddPet
                    break;
                case 0x195e496e:
                    //SetPos
                    break;
                case 0x1c3a4f77:
                    //ReflectAttack
                    break;
                case 0x1d3c0f1c:
                    //SpecialAttackWeapon
                    break;
                case 0x2001377e:
                    //MentorInvite
                    break;
                case 0x2049527c:
                    //Action 
                    break;
                case 0x204f4871:
                    //Script
                    break;
                case 0x206b4b73:
                    //FormatFeedback
                    break;
                case 0x2103247d:
                    //KnuBotAnswer
                    break;
                case 0x212c487a:
                    //Quest
                    break;
                case 0x215b5678:
                    //MineFullUpdate
                    break;
                case 0x2252445f:
                    //LookAt
                    break;
                case 0x25192476:
                    //ShieldAttack
                    break;
                case 0x25314d6d:
                    //CastNanoSpell
                    break;
                case 0x253d024c:
                    //ResearchUpdate
                    break;
                case 0x260f3671:
                    //FollowTarget
                    break;
                case 0x264b514b:
                    //RelocateDynels
                    break;
                case 0x264e5f61:
                    //Absorb
                    break;
                case 0x26515e61:
                    //Reload
                    break;
                case 0x270a4c62:
                    //KnuBotCloseChatWindow
                    break;
                case 0x271b3a6b:
                    //SimpleCharFullUpdate
                    break;
                case 0x28251f01:
                    //StartLogout
                    break;
                case 0x28494070:
                    //Attack
                    break;
                case 0x28784248:
                    //TeamMemberInfo
                    break;
                case 0x29304349:
                    //FullCharacter
                    break;
                case 0x2933154f:
                    //LaserTargetList
                    break;
                case 0x2a253f5f:
                    //TrapDisarmed
                    break;
                case 0x2a293d0f:
                    //Fov
                    break;
                case 0x2b333d6e:
                    //Stat
                    break;
                case 0x2c2f061c:
                    //QueueUpdate
                    break;
                case 0x2d212407:
                    //KnuBotRejectedItems
                    break;
                case 0x2e072a78:
                    //PlayerShopFullUpdate
                    break;
                case 0x2e2a4a6b:
                    //OrgInfoPacket
                    break;
                case 0x30161355:
                    //n3PlayfieldFullUpdate
                    break;
                case 0x3115534d:
                    //ResearchRequest
                    break;
                case 0x3129233b:
                    //AreaFormula
                    break;
                case 0x3301337a:
                    //InfromPlayer
                    break;
                case 0x333b2867:
                    //Mail
                    break;
                case 0x342c1d1d:
                    //ApplySpells
                    break;
                case 0x343c287f:
                    //Bank
                    break;
                case 0x353f4f52:
                    //ShopInventory
                    break;
                case 0x35505644:
                    //TemplateAction
                    break;
                case 0x36284f6e:
                    //Trade
                    break;
                case 0x365a5071:
                    //DoorFullUpdate
                    break;
                case 0x365e555b:
                    //CityAdvantages
                    break;
                case 0x3710256c:
                    //HealthDamage
                    break;
                case 0x371d0542:
                    //FightModeUpdate
                    break;
                case 0x373e3513:
                    //SetShopName
                    break;
                case 0x39343c68:
                    //Buff
                    break;
                case 0x3a1b2c0c:
                    //KnuBotTrade
                    break;
                case 0x3a243f41:
                    //DropTemplate
                    break;
                case 0x3a322a4a:
                    //GridSelected
                    break;
                case 0x3b11256f:
                    //SimpleItemFullUpdate
                    break;
                case 0x3b132d64:
                    //KnuBotOpenChatWindow
                    break;
                case 0x3b1d2268:
                    //WeaponItemFullUpdate
                    break;
                case 0x3b290771:
                    //SocialActionCmd
                    break;
                case 0x3b3b2878:
                    //Raid
                    break;
                case 0x3c1e2803:
                    //ShadowLevel
                    break;
                case 0x3c265179:
                    //Clone
                    break;
                case 0x3d5b4544:
                    //ShopCommission
                    break;
                case 0x3d746c7c:
                    //ServerPathPosDebugInfo
                    break;
                case 0x3e205660:
                    //Skill
                    break;
                case 0x3f3a1914:
                    //LeaveBattle
                    break;
                case 0x405b4f27:
                    //ShopInfo
                    break;
                case 0x41624f0d:
                    //AppearanceUpdate
                    break;
                case 0x43197d22:
                    //n3Teleport
                    break;
                case 0x435f7023:
                    //PerkUpdate
                    break;
                case 0x44483b3a:
                    //SendScore
                    break;
                case 0x445f2a0b:
                    //Resurrect
                    break;
                case 0x45072a0b:
                    //UpdateClientVisual
                    break;
                case 0x45273f0a:
                    //HouseDemolishStart
                    break;
                case 0x455d2938:
                    //PlaySound
                    break;
                case 0x46002f16:
                    //AttackInfo
                    break;
                case 0x46312d2e:
                    //TeamMember
                    break;
                case 0x464d000a:
                    //SpawnMech
                    break;
                case 0x465a4061:
                    //QuestFullUpdate
                    break;
                case 0x465a5d73:
                    //ChestItemFullUpdate
                    break;
                case 0x4727213e:
                    //NanoAttack
                    break;
                case 0x47483633:
                    //DropDynel
                    break;
                case 0x47537a24:
                    //ContainerAddItem
                    ContainerAddItem.AddItemToContainer(packet, client);
                    break;
                case 0x49222612:
                    //Visibility
                    break;
                case 0x4a41203e:
                    //StopFight
                    break;
                case 0x4b062919:
                    //BattleOver
                    break;
                case 0x4b5e7202:
                    //InventoryUpdated
                    break;
                case 0x4c7d403b:
                    //DoorStatusUpdate
                    break;
                case 0x4d2a3a38:
                    //TeamInvite
                    break;
                case 0x4d333027:
                    //ShopStatus
                    break;
                case 0x4d38242e:
                    //InfoPacket
                    break;
                case 0x4d450114:
                    //SpellList
                    break;
                case 0x4e536976:
                    //InventoryUpdate
                    break;
                case 0x4f474e05:
                    //CorpseFullUpdate
                    break;
                case 0x50544d19:
                    //Feedback
                    break;
                case 0x51492120:
                    //CharSecSpecAttack
                    break;
                case 0x52213420:
                    //BankCorpse
                    break;
                case 0x52526858:
                    //GenericCmd
                    GenericCmd.Read(packet, client, dynel);
                    break;
                case 0x5266632a:
                    //PathMoveCmd
                    break;
                case 0x540e3b27:
                    //ArriveAtBs
                    break;
                case 0x54111123:
                    //CharDCMove
                    CharacterDCMove.Read(packet, client);
                    break;
                case 0x55220726:
                    //PlayfieldAllTowers
                    break;
                case 0x55682b24:
                    //KnuBotFinishTrade
                    KnuBotFinishTrade.Read(packet, client);
                    break;
                case 0x55704d31:
                    //KnuBotAnswerList
                    break;
                case 0x56353038:
                    //StopLogout
                    break;
                case 0x570c2039:
                    //CharInPlay
                    CharacterInPlay.Read(packet, client);
                    break;
                case 0x58362220:
                    //ShopUpdate
                    break;
                case 0x58574239:
                    //MechInfo
                    break;
                case 0x58742a0f:
                    //RemovePet
                    break;
                case 0x59210126:
                    //PlayfieldAllCities
                    break;
                case 0x59313928:
                    //TrapItemFullUpdate
                    break;
                case 0x5a585f65:
                    //Inspect
                    break;
                case 0x5b1e052c:
                    //PlayfieldTowerUpdateClient
                    break;
                case 0x5c240404:
                    //ServerPosDebugInfo
                    break;
                case 0x5c436609:
                    //QuestAlternative
                    break;
                case 0x5c4a493a:
                    //FullAuto
                    break;
                case 0x5c525a7b:
                    ChatCommandHandler.Read(packet, client);
                    break;
                case 0x5c654b28:
                    //MissedAttackInfo
                    break;
                case 0x5d70532a:
                    //KnuBotAppendText
                    break;
                case 0x5e477770:
                    CharacterAction.Read(packet, client);
                    break;
                case 0x5e5b6007:
                    //HouseDisappeared
                    break;
                case 0x5f4a4c6c:
                    //Impulse
                    break;
                case 0x5f4b1a39:
                    //PlayfieldAnarchyF
                    break;
                case 0x5f4b442a:
                    //ChatText
                    break;
                case 0x5f52412e:
                    //GameTime
                    break;
                case 0x60201d0e:
                    //SetWantedDirection
                    break;
                case 0x62741e15:
                    //AOTransportSignal
                    break;
                case 0x63333303:
                    //PetCommand
                    break;
                case 0x64582a07:
                    //OrgServer
                    break;
                case 0x6e5f566e:
                    //SetStat
                    break;
                case 0x734e5a7b:
                    //SetName
                    break;
                case 0x742e2314:
                    //StopMovingCmd
                    break;
                case 0x754f1115:
                    //SpecialAttackInfo
                    break;
                case 0x77230927:
                    //GiveQuestToMember
                    break;
                case 0x7864401d:
                    //KnuBotStartTrade
                    KnuBotStartTrade.Read(packet, client);
                    break;
                case 0x7a222202:
                    //GfxTrigger
                    break;
                case 0x7e00312f:
                    //ShopItemPrice
                    break;
                case 0x7f405a16:
                    //NewLevel
                    break;
                case 0x7f4b3108:
                    //OrgClient
                    OrgClient.Read(packet, client);
                    break;
                case 0x7f544905:
                    //VendingMachineFullUpdate
                    break;
                default:
                    client.Server.Warning(client, "Client sent unknown N3Message {0:x8}", messageNumber.ToString());
                    break;
            }
        }
        #endregion
    }
}