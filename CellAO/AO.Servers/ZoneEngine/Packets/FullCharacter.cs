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

namespace ZoneEngine.Packets
{
    using System;

    using AO.Core;

    /// <summary>
    /// 
    /// </summary>
    public static class FullCharacter
    {
        #region WriteStat's
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statId"></param>
        private static void WriteStat3232(Client client, PacketWriter writer, int statId)
        {
            /* Stat */
            writer.PushInt(statId);
            /* Value */
            writer.PushUInt(client.Character.Stats.GetBaseValue(statId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statName"></param>
        private static void WriteStat3232(Client client, PacketWriter writer, string statName)
        {
            WriteStat3232(client, writer, client.Character.Stats.GetID(statName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statId"></param>
        private static void WriteStat816(Client client, PacketWriter writer, int statId)
        {
            if (statId > 255)
            {
                Console.WriteLine("WriteStat816 statId(" + statId + ") > 255");
            }

            /* Stat */
            writer.PushByte((byte)statId);
            /* Value */
            writer.PushShort((short)client.Character.Stats.GetBaseValue(statId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statName"></param>
        private static void WriteStat816(Client client, PacketWriter writer, string statName)
        {
            WriteStat816(client, writer, client.Character.Stats.GetID(statName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statId"></param>
        private static void WriteStat88(Client client, PacketWriter writer, int statId)
        {
            if (statId > 255)
            {
                Console.WriteLine("WriteStat88 statId(" + statId + ") > 255");
            }
            /* Stat */
            writer.PushByte((byte)statId);
            /* Value */
            writer.PushByte((byte)client.Character.Stats.GetBaseValue(statId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="writer"></param>
        /// <param name="statName"></param>
        private static void WriteStat88(Client client, PacketWriter writer, string statName)
        {
            WriteStat88(client, writer, client.Character.Stats.GetID(statName));
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void Send(Client client)
        {
            PacketWriter writer = new PacketWriter();

            #region Header
            writer.PushBytes(new byte[] { 0xDF, 0xDF });
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(client.Character.ID);
            writer.PushInt(0x29304349);
            writer.PushIdentity(50000, client.Character.ID);
            writer.PushByte(0);
            #endregion

            writer.PushInt(25);

            #region Data01 (Inventory)
            /* part 1 of data */
            int count;
            writer.Push3F1Count(client.Character.Inventory.Count);
            for (count = 0; count < client.Character.Inventory.Count; count++)
            {
                writer.PushInt(client.Character.Inventory[count].Placement);
                writer.PushShort((short)client.Character.Inventory[count].Item.Flags);
                writer.PushShort((short)client.Character.Inventory[count].Item.MultipleCount);
                writer.PushIdentity(
                    client.Character.Inventory[count].Item.Type, client.Character.Inventory[count].Item.Instance);
                writer.PushInt(client.Character.Inventory[count].Item.LowID);
                writer.PushInt(client.Character.Inventory[count].Item.HighID);
                writer.PushInt(client.Character.Inventory[count].Item.Quality);
                writer.PushInt(client.Character.Inventory[count].Item.Nothing);
            }
            #endregion

            #region Data02 (Uploaded nanos)
            /* part 2 of data */
            /* number of entries */
            writer.Push3F1Count(client.Character.UploadedNanos.Count);
            foreach (AOUploadedNanos au in client.Character.UploadedNanos)
            {
                writer.PushInt(au.Nano);
            }
            #endregion

            #region Data03 (Empty)
            /* part 3 of data */
            /* number of entries */
            writer.Push3F1Count(0);
            #endregion

            #region Unknown
            /* No idea what these are */
            /* used to be skill locks + some unknown data */
            // TODO: Find out what following 6 ints are
            writer.PushInt(1);
            writer.PushInt(0);
            writer.PushInt(1);
            writer.PushInt(0);
            writer.PushInt(1);
            writer.PushInt(0);
            #endregion

            #region Data06 (Stats 1) (32bit - 32bit)
            /* part 6 of data (1-st stats block) */

            /* Int32 stat number
               Int32 stat value */

            /* number of entries */
            writer.Push3F1Count(69);

            /* State */
            WriteStat3232(client, writer, 7);

            /* UnarmedTemplateInstance */
            WriteStat3232(client, writer, 418);

            /* Invaders Killed */
            WriteStat3232(client, writer, 615);

            /* KilledByInvaders */
            WriteStat3232(client, writer, 616);

            /* AccountFlags */
            WriteStat3232(client, writer, 660);

            /* VP */
            WriteStat3232(client, writer, 669);

            /* UnsavedXP */
            WriteStat3232(client, writer, 592);

            /* NanoFocusLevel */
            WriteStat3232(client, writer, 355);

            /* Specialization */
            WriteStat3232(client, writer, 182);

            /* ShadowBreedTemplate */
            WriteStat3232(client, writer, 579);

            /* ShadowBreed */
            WriteStat3232(client, writer, 532);

            /* LastPerkResetTime */
            WriteStat3232(client, writer, 577);

            /* SocialStatus */
            WriteStat3232(client, writer, 521);

            /* PlayerOptions */
            WriteStat3232(client, writer, 576);

            /* TempSaveTeamID */
            WriteStat3232(client, writer, 594);

            /* TempSavePlayfield */
            WriteStat3232(client, writer, 595);

            /* TempSaveX */
            WriteStat3232(client, writer, 596);

            /* TempSaveY */
            WriteStat3232(client, writer, 597);

            /* VisualFlags */
            WriteStat3232(client, writer, 673);

            /* PvPDuelKills */
            WriteStat3232(client, writer, 674);

            /* PvPDuelDeaths */
            WriteStat3232(client, writer, 675);

            /* PvPProfessionDuelKills */
            WriteStat3232(client, writer, 676);

            /* PvPProfessionDuelDeaths */
            WriteStat3232(client, writer, 677);

            /* PvPRankedSoloKills */
            WriteStat3232(client, writer, 678);

            /* PvPRankedSoloDeaths */
            WriteStat3232(client, writer, 679);

            /* PvPRankedTeamKills */
            WriteStat3232(client, writer, 680);

            /* PvPRankedTeamDeaths */
            WriteStat3232(client, writer, 681);

            /* PvPSoloScore */
            WriteStat3232(client, writer, 682);

            /* PvPTeamScore */
            WriteStat3232(client, writer, 683);

            /* PvPDuelScore */
            WriteStat3232(client, writer, 684);

            WriteStat3232(client, writer, 0x289);
            WriteStat3232(client, writer, 0x28a);

            /* SavedXP */
            WriteStat3232(client, writer, 334);

            /* Flags */
            WriteStat3232(client, writer, 0);

            /* Features */
            WriteStat3232(client, writer, 224);

            /* ApartmentsAllowed */
            WriteStat3232(client, writer, 582);

            /* ApartmentsOwned */
            WriteStat3232(client, writer, 583);

            /* MonsterScale */
            WriteStat3232(client, writer, 360);

            /* VisualProfession */
            WriteStat3232(client, writer, 368);

            /* NanoAC */
            WriteStat3232(client, writer, 168);

            WriteStat3232(client, writer, 214);
            WriteStat3232(client, writer, 221);
            /* LastConcretePlayfieldInstance */
            WriteStat3232(client, writer, 191);

            /* MapOptions */
            WriteStat3232(client, writer, 470);

            /* MapAreaPart1 */
            WriteStat3232(client, writer, 471);

            /* MapAreaPart2 */
            WriteStat3232(client, writer, 472);

            /* MapAreaPart3 */
            WriteStat3232(client, writer, 585);

            /* MapAreaPart4 */
            WriteStat3232(client, writer, 586);

            /* MissionBits1 */
            WriteStat3232(client, writer, 256);

            /* MissionBits2 */
            WriteStat3232(client, writer, 257);

            /* MissionBits3 */
            WriteStat3232(client, writer, 303);

            /* MissionBits4 */
            WriteStat3232(client, writer, 432);

            /* MissionBits5 */
            WriteStat3232(client, writer, 65);

            /* MissionBits6 */
            WriteStat3232(client, writer, 66);

            /* MissionBits7 */
            WriteStat3232(client, writer, 67);

            /* MissionBits8 */
            WriteStat3232(client, writer, 544);

            /* MissionBits9 */
            WriteStat3232(client, writer, 545);

            /* MissionBits10 */
            WriteStat3232(client, writer, 617);
            WriteStat3232(client, writer, 618);
            WriteStat3232(client, writer, 619);
            WriteStat3232(client, writer, 198);

            /* AutoAttackFlags */
            WriteStat3232(client, writer, 349);

            /* PersonalResearchLevel */
            WriteStat3232(client, writer, 263);

            /* GlobalResearchLevel */
            WriteStat3232(client, writer, 264);

            /* PersonalResearchGoal */
            WriteStat3232(client, writer, 265);

            /* GlobalResearchGoal */
            WriteStat3232(client, writer, 266);

            /* BattlestationSide */
            WriteStat3232(client, writer, 668);

            /* BattlestationRep */
            WriteStat3232(client, writer, 670);

            /* Members */
            WriteStat3232(client, writer, 300);
            #endregion

            #region Data07 (Stats 2) (32bit - 32bit)
            /* number of entries */
            writer.Push3F1Count(144);

            /* Int32 stat number
               Int32 stat value */

            /* VeteranPoints */
            WriteStat3232(client, writer, 68);

            /* MonthsPaid */
            WriteStat3232(client, writer, 69);

            /* PaidPoints */
            WriteStat3232(client, writer, 672);

            /* AutoAttackFlags */
            WriteStat3232(client, writer, 349);

            /* XPKillRange */
            WriteStat3232(client, writer, 275);

            /* InPlay */
            WriteStat3232(client, writer, 194);

            /* Health (current health)*/
            WriteStat3232(client, writer, 27);

            /* Life (max health)*/
            WriteStat3232(client, writer, 1);

            /* Psychic */
            WriteStat3232(client, writer, 21);

            /* Sense */
            WriteStat3232(client, writer, 20);

            /* Intelligence */
            WriteStat3232(client, writer, 19);

            /* Stamina */
            WriteStat3232(client, writer, 18);

            /* Agility */
            WriteStat3232(client, writer, 17);

            /* Strength */
            WriteStat3232(client, writer, 16);

            /* Attitude */
            WriteStat3232(client, writer, 63);

            /* Alignment (Clan Tokens) */
            WriteStat3232(client, writer, 62);

            /* Cash */
            WriteStat3232(client, writer, 61);

            /* Profession */
            WriteStat3232(client, writer, 60);

            /* AggDef */
            WriteStat3232(client, writer, 51);

            /* Icon */
            WriteStat3232(client, writer, 79);

            /* Mesh */
            WriteStat3232(client, writer, 12);

            /* RunSpeed */
            WriteStat3232(client, writer, 156);

            /* DeadTimer */
            WriteStat3232(client, writer, 34);

            /* Team */
            WriteStat3232(client, writer, 6);

            /* Breed */
            WriteStat3232(client, writer, 4);

            /* Sex */
            WriteStat3232(client, writer, 59);

            /* LastSaveXP */
            WriteStat3232(client, writer, 372);

            /* NextXP */
            WriteStat3232(client, writer, 350);

            /* LastXP */
            WriteStat3232(client, writer, 57);

            /* Level */
            WriteStat3232(client, writer, 54);

            /* XP */
            WriteStat3232(client, writer, 52);

            /* IP */
            WriteStat3232(client, writer, 53);

            /* CurrentMass */
            WriteStat3232(client, writer, 78);

            /* ItemType */
            WriteStat3232(client, writer, 72);

            /* PreviousHealth */
            WriteStat3232(client, writer, 11);

            /* CurrentState */
            WriteStat3232(client, writer, 423);

            /* Age */
            WriteStat3232(client, writer, 58);

            /* Side */
            WriteStat3232(client, writer, 33);

            /* WaitState */
            WriteStat3232(client, writer, 430);

            /* DriveWater */
            WriteStat3232(client, writer, 117);

            /* MeleeMultiple */
            WriteStat3232(client, writer, 101);

            /* LR_MultipleWeapon */
            WriteStat3232(client, writer, 134);

            /* LR_EnergyWeapon */
            WriteStat3232(client, writer, 133);

            /* RadiationAC */
            WriteStat3232(client, writer, 94);

            /* SenseImprovement */
            WriteStat3232(client, writer, 122);

            /* BowSpecialAttack */
            WriteStat3232(client, writer, 121);

            /* Burst */
            WriteStat3232(client, writer, 148);

            /* FullAuto */
            WriteStat3232(client, writer, 167);

            /* MapNavigation */
            WriteStat3232(client, writer, 140);

            /* DriveAir */
            WriteStat3232(client, writer, 139);

            /* DriveGround */
            WriteStat3232(client, writer, 166);

            /* BreakingEntry */
            WriteStat3232(client, writer, 165);

            /* Concealment */
            WriteStat3232(client, writer, 164);

            /* Chemistry */
            WriteStat3232(client, writer, 163);

            /* Psychology */
            WriteStat3232(client, writer, 162);

            /* ComputerLiteracy */
            WriteStat3232(client, writer, 161);

            /* NanoProgramming */
            WriteStat3232(client, writer, 160);

            /* Pharmaceuticals */
            WriteStat3232(client, writer, 159);

            /* WeaponSmithing */
            WriteStat3232(client, writer, 158);

            /* FieldQuantumPhysics */
            WriteStat3232(client, writer, 157);

            /* AttackSpeed */
            WriteStat3232(client, writer, 3);

            /* Evade */
            WriteStat3232(client, writer, 155);

            /* Dodge */
            WriteStat3232(client, writer, 154);

            /* Duck */
            WriteStat3232(client, writer, 153);

            /* BodyDevelopment */
            WriteStat3232(client, writer, 152);

            /* AimedShot */
            WriteStat3232(client, writer, 151);

            /* FlingShot */
            WriteStat3232(client, writer, 150);

            /* NanoProwessInitiative */
            WriteStat3232(client, writer, 149);

            /* FastAttack */
            WriteStat3232(client, writer, 147);

            /* SneakAttack */
            WriteStat3232(client, writer, 146);

            /* Parry */
            WriteStat3232(client, writer, 145);

            /* Dimach */
            WriteStat3232(client, writer, 144);

            /* Riposte */
            WriteStat3232(client, writer, 143);

            /* Brawl */
            WriteStat3232(client, writer, 142);

            /* Tutoring */
            WriteStat3232(client, writer, 141);

            /* Swim */
            WriteStat3232(client, writer, 138);

            /* Adventuring */
            WriteStat3232(client, writer, 137);

            /* Perception */
            WriteStat3232(client, writer, 136);

            /* DisarmTraps */
            WriteStat3232(client, writer, 135);

            /* NanoEnergyPool */
            WriteStat3232(client, writer, 132);

            /* MaterialLocation */
            WriteStat3232(client, writer, 131);

            /* MaterialCreation */
            WriteStat3232(client, writer, 130);

            /* PsychologicalModification */
            WriteStat3232(client, writer, 129);

            /* BiologicalMetamorphose */
            WriteStat3232(client, writer, 128);

            /* MaterialMetamorphose */
            WriteStat3232(client, writer, 127);

            /* ElectricalEngineering */
            WriteStat3232(client, writer, 126);

            /* MechanicalEngineering */
            WriteStat3232(client, writer, 125);

            /* Treatment */
            WriteStat3232(client, writer, 124);

            /* FirstAid */
            WriteStat3232(client, writer, 123);

            /* PhysicalProwessInitiative */
            WriteStat3232(client, writer, 120);

            /* DistanceWeaponInitiative */
            WriteStat3232(client, writer, 119);

            /* CloseCombatInitiative */
            WriteStat3232(client, writer, 118);

            /* AssaultRifle */
            WriteStat3232(client, writer, 116);

            /* Shotgun */
            WriteStat3232(client, writer, 115);

            /* SubMachineGun */
            WriteStat3232(client, writer, 114);

            /* Rifle */
            WriteStat3232(client, writer, 113);

            /* Pistol */
            WriteStat3232(client, writer, 112);

            /* Bow */
            WriteStat3232(client, writer, 111);

            /* ThrownGrapplingWeapons */
            WriteStat3232(client, writer, 110);

            /* Grenade */
            WriteStat3232(client, writer, 109);

            /* ThrowingKnife */
            WriteStat3232(client, writer, 108);

            /* 2HBluntWeapons */
            WriteStat3232(client, writer, 107);

            /* Piercing */
            WriteStat3232(client, writer, 106);

            /* 2HEdgedWeapons */
            WriteStat3232(client, writer, 105);

            /* MeleeEnergyWeapon */
            WriteStat3232(client, writer, 104);

            /* 1HEdgedWeapons */
            WriteStat3232(client, writer, 103);

            /* 1HBluntWeapons */
            WriteStat3232(client, writer, 102);

            /* MartialArts */
            WriteStat3232(client, writer, 100);

            /* Alignment (Clan Tokens) */
            WriteStat3232(client, writer, 62);

            /* MetaType (Omni Tokens) */
            WriteStat3232(client, writer, 75);

            /* TitleLevel */
            WriteStat3232(client, writer, 37);

            /* GmLevel */
            WriteStat3232(client, writer, 215);

            /* FireAC */
            WriteStat3232(client, writer, 97);

            /* PoisonAC */
            WriteStat3232(client, writer, 96);

            /* ColdAC */
            WriteStat3232(client, writer, 95);

            /* RadiationAC */
            WriteStat3232(client, writer, 94);

            /* ChemicalAC */
            WriteStat3232(client, writer, 93);

            /* EnergyAC */
            WriteStat3232(client, writer, 92);

            /* MeleeAC */
            WriteStat3232(client, writer, 91);

            /* ProjectileAC */
            WriteStat3232(client, writer, 90);

            /* RP */
            WriteStat3232(client, writer, 199);

            /* SpecialCondition */
            WriteStat3232(client, writer, 348);

            /* SK */
            WriteStat3232(client, writer, 573);

            /* Expansions */
            WriteStat3232(client, writer, 389);

            /* ClanRedeemed */
            WriteStat3232(client, writer, 572);

            /* ClanConserver */
            WriteStat3232(client, writer, 571);

            /* ClanDevoted */
            WriteStat3232(client, writer, 570);

            /* OTUnredeemed */
            WriteStat3232(client, writer, 569);

            /* OTOperator */
            WriteStat3232(client, writer, 568);

            /* OTFollowers */
            WriteStat3232(client, writer, 567);

            /* GOS */
            WriteStat3232(client, writer, 566);

            /* ClanVanguards */
            WriteStat3232(client, writer, 565);

            /* OTTrans */
            WriteStat3232(client, writer, 564);

            /* ClanGaia */
            WriteStat3232(client, writer, 563);

            /* OTMed*/
            WriteStat3232(client, writer, 562);

            /* ClanSentinels */
            WriteStat3232(client, writer, 561);

            /* OTArmedForces */
            WriteStat3232(client, writer, 560);

            /* SocialStatus */
            WriteStat3232(client, writer, 521);

            /* PlayerID */
            WriteStat3232(client, writer, 607);

            /* KilledByInvaders */
            WriteStat3232(client, writer, 616);

            /* InvadersKilled */
            WriteStat3232(client, writer, 615);

            /* AlienLevel */
            WriteStat3232(client, writer, 169);

            /* AlienNextXP */
            WriteStat3232(client, writer, 178);

            /* AlienXP */
            WriteStat3232(client, writer, 40);
            #endregion

            #region Data08 (Stats 3) (8bit - 8bit)
            /* number of entries */
            writer.Push3F1Count(8);

            /* Byte stat number
               Byte stat value */

            /* InsurancePercentage */
            WriteStat88(client, writer, 236);

            /* ProfessionLevel */
            WriteStat88(client, writer, 10);

            /* PrevMovementMode */
            WriteStat88(client, writer, 174);

            /* CurrentMovementMode */
            WriteStat88(client, writer, 173);

            /* Fatness */
            WriteStat88(client, writer, 47);

            /* Race */
            WriteStat88(client, writer, 89);

            /* TeamSide */
            WriteStat88(client, writer, 213);

            /* BeltSlots */
            WriteStat88(client, writer, 45);
            #endregion

            #region Data09 (Stats 4) (8bit - 16bit)
            /* number of stats */
            writer.Push3F1Count(16);

            /* Byte stat number
               Int16 (short) stat value */

            /* AbsorbProjectileAC */
            WriteStat816(client, writer, 238);

            /* AbsorbMeleeAC */
            WriteStat816(client, writer, 239);

            /* AbsorbEnergyAC */
            WriteStat816(client, writer, 240);

            /* AbsorbChemicalAC */
            WriteStat816(client, writer, 241);

            /* AbsorbRadiationAC */
            WriteStat816(client, writer, 242);

            /* AbsorbColdAC */
            WriteStat816(client, writer, 243);

            /* AbsorbNanoAC */
            WriteStat816(client, writer, 246);

            /* AbsorbFireAC */
            WriteStat816(client, writer, 244);

            /* AbsorbPoisonAC */
            WriteStat816(client, writer, 245);

            /* TemporarySkillReduction */
            WriteStat816(client, writer, 247);

            /* InsuranceTime */
            WriteStat816(client, writer, 49);

            /* CurrentNano */
            WriteStat816(client, writer, 214);

            /* MaxNanoEnergy */
            WriteStat816(client, writer, 221);

            /* MaxNCU */
            WriteStat816(client, writer, 181);

            /* MapFlags */
            WriteStat816(client, writer, 9);

            /* ChangeSideCount */
            WriteStat816(client, writer, 237);
            #endregion

            /* ? */
            writer.PushInt(0);
            /* ? */
            writer.PushInt(0);

            #region Data10 (Empty)
            /* number of entries */
            writer.Push3F1Count(0);
            #endregion

            #region Data11 (Empty)
            /* number of entries */
            writer.Push3F1Count(0);
            #endregion

            #region Data12 (Empty)
            /* number of entries */
            writer.Push3F1Count(0);
            #endregion

            byte[] reply = writer.Finish();
            client.SendCompressed(reply);
        }
    }
}