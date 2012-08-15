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

namespace ZoneEngine.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using AO.Core;

    using ZoneEngine.Database;
    using ZoneEngine.Misc;

    /// <summary>
    /// 
    /// </summary>
    public static class SimpleCharFullUpdate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void SendToPlayfield(Client client)
        {
            byte[] packet = GetPacket(client);
            Announce.Playfield(client.Character.PlayField, ref packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void SendToOthersInPlayfield(Client client)
        {
            Byte[] packet = GetPacket(client);
            Announce.PlayfieldOthers(client, ref packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="receiver"></param>
        public static void SendToOne(Character character, Client receiver)
        {
            Byte[] packet = WritePacket(character, receiver.Character.ID);
            receiver.SendCompressed(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Byte[] GetPacket(Client client)
        {
            return WritePacket(client.Character, client.Character.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public static Byte[] WritePacket(Character character, int receiver)
        {
            /*
             * To set a packetFlag use       packetFlags |= <flagHere>;
             * To unset a packetFlag use     packetFlags &= ~<flagHere>;
             * 
             * Wherever you set a flag conditionally, be sure to unset the same flag if the reverse
             * condition is true 'just in case'.
             */
            int packetFlags = 0; // Try setting to 0x042062C8 if you have problems (old value)

            //
            // Character Variables

            bool socialonly;
            bool showsocial;
            bool showhelmet;
            bool LeftPadVisible;
            bool RightPadVisible;
            bool DoubleLeftPad;
            bool DoubleRightPad;

            int CharPlayfield;
            AOCoord CharCoord;
            int CharID;
            Quaternion CharHeading;

            uint SideValue;
            uint FatValue;
            uint BreedValue;
            uint SexValue;
            uint RaceValue;

            int NameLength;
            string CharName;
            int CharFlagsValue;
            int AccFlagsValue;

            int ExpansionValue;
            int CurrentNano;
            int CurrentHealth;

            uint StrengthBaseValue;
            uint StaminaBaseValue;
            uint AgilityBaseValue;
            uint SenseBaseValue;
            uint IntelligenceBaseValue;
            uint PsychicBaseValue;

            int FirstNameLength;
            int LastNameLength;
            string FirstName;
            string LastName;
            int OrgNameLength;
            string OrgName;
            int LevelValue;
            int HealthValue;
            int LOSHeight;

            int MonsterData;
            int MonsterScale;
            int VisualFlags;

            int CurrentMovementMode;
            uint RunSpeedBaseValue;

            int TexturesCount;
            int HairMeshValue;
            int WeaponMeshRightValue;
            int WeaponMeshLeftValue;

            uint HeadMeshBaseValue;
            int HeadMeshValue;

            int BackMeshValue;
            int ShoulderMeshRightValue;
            //int ShoulderMeshLeftValue;

            int OverrideTextureHead;
            int OverrideTextureWeaponRight;
            int OverrideTextureWeaponLeft;
            int OverrideTextureShoulderpadRight;
            int OverrideTextureShoulderpadLeft;
            int OverrideTextureBack;
            int OverrideTextureAttractor;

            //NPC Values

            int NPCFamily;

            Dictionary<int, int> socialTab = new Dictionary<int, int>();

            List<AOTextures> textures = new List<AOTextures>();

            List<AOMeshs> meshs = new List<AOMeshs>();

            List<AONano> nanos = new List<AONano>();

            lock (character)
            {
                socialonly = ((character.Stats.VisualFlags.Value & 0x40) > 0);
                showsocial = ((character.Stats.VisualFlags.Value & 0x20) > 0);
                showhelmet = ((character.Stats.VisualFlags.Value & 0x4) > 0);
                LeftPadVisible = ((character.Stats.VisualFlags.Value & 0x1) > 0);
                RightPadVisible = ((character.Stats.VisualFlags.Value & 0x2) > 0);
                DoubleLeftPad = ((character.Stats.VisualFlags.Value & 0x8) > 0);
                DoubleRightPad = ((character.Stats.VisualFlags.Value & 0x10) > 0);

                CharPlayfield = character.PlayField;
                CharCoord = character.Coordinates;
                CharID = character.ID;
                CharHeading = character.Heading;

                SideValue = character.Stats.Side.StatBaseValue;
                FatValue = character.Stats.Fatness.StatBaseValue;
                BreedValue = character.Stats.Breed.StatBaseValue;
                SexValue = character.Stats.Sex.StatBaseValue;
                RaceValue = character.Stats.Race.StatBaseValue;

                NameLength = character.Name.Length;
                CharName = character.Name;
                CharFlagsValue = character.Stats.Flags.Value;
                AccFlagsValue = character.Stats.AccountFlags.Value;

                ExpansionValue = character.Stats.Expansion.Value;
                CurrentNano = character.Stats.CurrentNano.Value;

                StrengthBaseValue = character.Stats.Strength.StatBaseValue;
                StaminaBaseValue = character.Stats.Strength.StatBaseValue;
                AgilityBaseValue = character.Stats.Strength.StatBaseValue;
                SenseBaseValue = character.Stats.Strength.StatBaseValue;
                IntelligenceBaseValue = character.Stats.Strength.StatBaseValue;
                PsychicBaseValue = character.Stats.Strength.StatBaseValue;

                FirstNameLength = character.FirstName.Length;
                LastNameLength = character.LastName.Length;
                FirstName = character.FirstName;
                LastName = character.LastName;
                OrgNameLength = character.OrgName.Length;
                OrgName = character.OrgName;
                LevelValue = character.Stats.Level.Value;
                HealthValue = character.Stats.Life.Value;

                MonsterData = character.Stats.MonsterData.Value;
                MonsterScale = character.Stats.MonsterScale.Value;
                VisualFlags = character.Stats.VisualFlags.Value;

                CurrentMovementMode = character.Stats.CurrentMovementMode.Value;
                RunSpeedBaseValue = character.Stats.RunSpeed.StatBaseValue;

                TexturesCount = character.Textures.Count;
                HairMeshValue = character.Stats.HairMesh.Value;
                WeaponMeshRightValue = character.Stats.WeaponMeshRight.Value;
                WeaponMeshLeftValue = character.Stats.WeaponMeshLeft.Value;

                HeadMeshBaseValue = character.Stats.HeadMesh.StatBaseValue;
                HeadMeshValue = character.Stats.HeadMesh.Value;

                BackMeshValue = character.Stats.BackMesh.Value;
                ShoulderMeshRightValue = character.Stats.ShoulderMeshRight.Value;
                //ShoulderMeshLeftValue = character.Stats.ShoulderMeshLeft.Value;

                OverrideTextureHead = character.Stats.OverrideTextureHead.Value;
                OverrideTextureWeaponRight = character.Stats.OverrideTextureWeaponRight.Value;
                OverrideTextureWeaponLeft = character.Stats.OverrideTextureWeaponLeft.Value;
                OverrideTextureShoulderpadRight = character.Stats.OverrideTextureShoulderpadRight.Value;
                OverrideTextureShoulderpadLeft = character.Stats.OverrideTextureShoulderpadLeft.Value;
                OverrideTextureBack = character.Stats.OverrideTextureBack.Value;
                OverrideTextureAttractor = character.Stats.OverrideTextureAttractor.Value;

                foreach (int num in character.SocialTab.Keys)
                {
                    socialTab.Add(num, character.SocialTab[num]);
                }

                foreach (AOTextures at in character.Textures)
                {
                    textures.Add(new AOTextures(at.place, at.Texture));
                }

                meshs = MeshLayers.GetMeshs(character, showsocial, socialonly);

                foreach (AONano nano in character.ActiveNanos)
                {
                    AONano tempNano = new AONano();
                    tempNano.ID = nano.ID;
                    tempNano.Instance = nano.Instance;
                    tempNano.NanoStrain = nano.NanoStrain;
                    tempNano.Nanotype = nano.Nanotype;
                    tempNano.Time1 = nano.Time1;
                    tempNano.Time2 = nano.Time2;
                    tempNano.Value3 = nano.Value3;

                    nanos.Add(tempNano);
                }

                LOSHeight = character.Stats.LOSHeight.Value;
                NPCFamily = character.Stats.NPCFamily.Value;
                CurrentHealth = character.Stats.Health.Value;
            }
            PacketWriter packetWriter = new PacketWriter();

            // Packet Header
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0); // length. writer will take care of this
            packetWriter.PushInt(3086); // sender. our server ID
            packetWriter.PushInt(receiver); // receiver
            packetWriter.PushInt(0x271B3A6B); // packet ID
            packetWriter.PushIdentity(50000, CharID); // affected identity
            packetWriter.PushByte(0); // Unknown?
            // End Packet Header

            packetWriter.PushByte(57); // SCFU packet version (57/0x39)
            packetWriter.PushInt(0); // packet flags (this is set later based on packetFlags variable above)

            packetFlags |= 0x40; // Has Playfield ID
            packetWriter.PushInt(CharPlayfield); // playfield

            if (character.FightingTarget.Instance != 0)
            {
                packetFlags |= 20;
                packetWriter.PushIdentity(character.FightingTarget);
            }

            // Coordinates
            packetWriter.PushCoord(CharCoord);

            // Heading Data
            packetFlags |= 0x200; // Has Heading Data Flag
            packetWriter.PushQuat(CharHeading);

            uint appearance = SideValue + (FatValue * 8) + (BreedValue * 32) + (SexValue * 256) + (RaceValue * 1024);
                // Race
            packetWriter.PushUInt(appearance); // appearance

            // Name
            packetWriter.PushByte((byte)(NameLength + 1));
            packetWriter.PushBytes(Encoding.ASCII.GetBytes(CharName));
            packetWriter.PushByte(0); // 0 terminator for name

            packetWriter.PushUInt(CharFlagsValue); // Flags
            packetWriter.PushShort((short)AccFlagsValue);
            packetWriter.PushShort((short)ExpansionValue);

            if (character is NonPlayerCharacterClass)
            {
                packetFlags |= 1;
            }

            packetFlags &= ~0x01; // We are a player
            if ((packetFlags & 0x01) != 0)
            {
                // Are we a NPC (i think anyway)? So far this is _NOT_ used at all

                if (NPCFamily < 256)
                {
                    packetWriter.PushByte((byte)NPCFamily);
                }
                else
                {
                    packetFlags |= 0x20000;
                    packetWriter.PushShort((Int16)NPCFamily);
                }

                if (LOSHeight < 256)
                {
                    packetWriter.PushByte((byte)LOSHeight);
                }
                else
                {
                    packetFlags |= 0x80000;
                    packetWriter.PushShort((Int16)LOSHeight);
                }

                //if (packetFlags & 0x2000000)
                //{
                //    char PetType;
                //}
                //else
                //{
                //    short PetType;
                //}

                //short TowerType;

                //if (TowerType > 0)
                //{
                //    char unknown;
                //}
            }
            else
            {
                // Are we a player?
                packetWriter.PushUInt(CurrentNano); // CurrentNano
                packetWriter.PushInt(0); // team?
                packetWriter.PushShort(5); // swim?

                // The checks here are to prevent the client doing weird things if the character has really large or small base attributes
                if (StrengthBaseValue > 32767) // Strength
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)StrengthBaseValue);
                }
                if (AgilityBaseValue > 32767) // Agility
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)AgilityBaseValue);
                }
                if (StaminaBaseValue > 32767) //  Stamina
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)StaminaBaseValue);
                }
                if (IntelligenceBaseValue > 32767) // Intelligence
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)IntelligenceBaseValue);
                }
                if (SenseBaseValue > 32767) // Sense
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)SenseBaseValue);
                }
                if (PsychicBaseValue > 32767) // Psychic
                {
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)PsychicBaseValue);
                }

                if ((CharFlagsValue & 0x400000) != 0) // has visible names? (Flags)
                {
                    packetWriter.PushShort((short)FirstNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(FirstName));
                    packetWriter.PushShort((short)LastNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(LastName));
                }

                if (OrgNameLength != 0)
                {
                    packetFlags |= 0x4000000; // Has org name data

                    packetWriter.PushShort((short)OrgNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(OrgName));
                }
                else
                {
                    packetFlags &= ~0x4000000; // Does not have org name data
                }
            }

            if (LevelValue > 127) // Level
            {
                packetFlags |= 0x1000; // Has Extended Level
                packetWriter.PushShort((short)LevelValue);
            }
            else
            {
                packetFlags &= ~0x1000; // Has Small Level
                packetWriter.PushByte((byte)LevelValue);
            }

            if (HealthValue > 32767) // Health
            {
                packetFlags &= ~0x800; // Has Extended Health
                packetWriter.PushUInt(HealthValue);
            }
            else
            {
                packetFlags |= 0x800; // Has Small Health
                packetWriter.PushShort((short)HealthValue);
            }
            int healthdamage = HealthValue - CurrentHealth;
            if (healthdamage < 256)
            {
                packetFlags |= 0x4000;
                packetWriter.PushByte((byte)healthdamage);
            }
            else
            {
                packetFlags &= ~0x4000;
                if ((packetFlags & 0x800) == 0x800)
                {
                    packetWriter.PushShort((Int16)healthdamage);
                }
                else
                {
                    packetWriter.PushInt(healthdamage);
                }
            }

            // If player is in grid or fixer grid
            // make him/her/it a nice upside down pyramid
            if ((CharPlayfield == 152) || (CharPlayfield == 4107))
            {
                packetWriter.PushInt(99902);
            }
            else
            {
                packetWriter.PushUInt(MonsterData); // Monsterdata
            }
            packetWriter.PushShort((short)MonsterScale); // Monsterscale
            packetWriter.PushShort((short)VisualFlags); // VisualFlags
            packetWriter.PushByte(0); // visible title?

            packetWriter.PushInt(42); // 'skipdata' length
            // Start 'skipdata'
            packetWriter.PushBytes(new Byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 });
            packetWriter.PushByte((byte)CurrentMovementMode); // CurrentMovementMode
            packetWriter.PushByte(1); // don't change
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(0); // ?
            packetWriter.PushShort(3); // ?
            packetWriter.PushInt(0); //?
            packetWriter.PushInt(0); //?
            packetWriter.PushInt(0); //?
            packetWriter.PushInt(0); //?
            // End 'skipdata'

            if (HeadMeshValue != 0)
            {
                packetFlags |= 0x80; // Has HeadMesh Flag
                packetWriter.PushUInt(HeadMeshValue); // Headmesh
            }

            if ((RunSpeedBaseValue > 127)) // Runspeed
            {
                packetFlags |= 0x2000;
                packetWriter.PushShort((short)RunSpeedBaseValue);
            }
            else
            {
                packetFlags &= ~0x2000;
                packetWriter.PushByte((byte)RunSpeedBaseValue);
            }

            //if (packetFlags & 0x400)
            //{
            //    // Pop2Long
            //    /*
            //     * Is this a Type:Instance pair?
            //     * Suspect so as it uses Pop2Long
            //     * which is used for Type:Instance
            //     * pairs. Perhaps pet master?
            //     * (Just a wild guess though that its pet master)
            //     */
            //    long unknown;
            //    long unknown;
            //}

            //if (packetFlags & 0x10)
            //{
            //    long counter;
            //    repeat (counter / 0x3F1 - 1) times
            //        char texturepositionname[32]; // Null padded at the end
            //        long textureid; // Or is this mesh id?
            //        long unknown;
            //        long unknown;
            //    end repeat
            //}

            // Is char/NPC in hide mode?
            //if (packetFlags & 0x100000)
            //{
            //    short Concealment;
            //}

            //if (packetFlags & 0x800000)
            //{
            //    char unknown;
            //}

            //if (packetFlags & 0x1000000)
            //{
            //    char unknown;
            //}

            packetWriter.Push3F1Count(nanos.Count); // running nanos count
            foreach (AONano nano in nanos)
            {
                packetWriter.PushInt(nano.ID);
                packetWriter.PushInt(nano.Instance);
                packetWriter.PushInt(nano.Time1);
                packetWriter.PushInt(nano.Time2);
            }
            // longx5: aoid, instance, unknown(0?), timer1, timer2

            //if (flags & 0x10000)
            //{
            //    // Waypoint Info
            //    // Pop2Long (1010E2D3)
            //    long type;
            //    long instance;
            //    // Waypoint Counter - 3x float per entry
            //    long counter;
            //    repeat counter times
            //        float x;
            //        float y;
            //        float z;
            //    end repeat
            //    // End Waypoint Counter
            //}

            // Texture/Cloth Data
            int c;
            packetWriter.Push3F1Count(5); // textures count

            AOTextures aotemp = new AOTextures(0, 0);
            for (c = 0; c < 5; c++)
            {
                aotemp.Texture = 0;
                aotemp.place = c;
                int c2;
                for (c2 = 0; c2 < TexturesCount; c2++)
                {
                    if (textures[c2].place == c)
                    {
                        aotemp.Texture = textures[c2].Texture;
                        break;
                    }
                }
                if (showsocial)
                {
                    if (socialonly)
                    {
                        aotemp.Texture = socialTab[c];
                    }
                    else
                    {
                        if (socialTab[c] != 0)
                        {
                            aotemp.Texture = socialTab[c];
                        }
                    }
                }

                packetWriter.PushInt(aotemp.place);
                packetWriter.PushInt(aotemp.Texture);
                packetWriter.PushInt(0);
            }
            // End Textures

            // ############
            // # Meshs
            // ############

            c = meshs.Count;

            packetWriter.Push3F1Count(c);
            foreach (AOMeshs aoMeshs in meshs)
            {
                packetWriter.PushByte((byte)aoMeshs.Position);
                packetWriter.PushUInt(aoMeshs.Mesh);
                packetWriter.PushInt(aoMeshs.OverrideTexture); // Override Texture!!!!!!
                packetWriter.PushByte((byte)aoMeshs.Layer);
            }
            // End Meshs

            //if (packetFlags & 0x100)
            //{
            //    // 0x3F1 Unknown Counter - 4x long per entry
            //    long counter;
            //    repeat (counter / 0x3F1 - 1) times
            //        long unknown;
            //        long unknown;
            //        long unknown;
            //        long unknown;
            //    end repeat
            //}

            //if (packetFlags & 0x20000000)
            //{
            //    char ShadowBreed;
            //}

            //if (packetFlags & 0x40000000)
            //{
            //    // 0x3F1 Unknown Counter - 4x long per entry
            //    long counter;
            //    repeat (counter / 0x3F1 - 1) times
            //        Pop2Long (Type:Instance pair maybe?)
            //        long unknown;
            //        long unknown;
            //    end repeat
            //}

            packetWriter.PushInt(0); // packetFlags2

            // Some mech stuff
            //if (packetFlags2 & 0x01)
            //{
            //    long counter;
            //    repeat (counter) times
            //        long unknown;
            //        long unknown;
            //    end repeat

            //    long MechData;
            //    // Pop2Long (Type:Instance pair maybe?)
            //    long unknown;
            //    long unknown;
            //}

            // maybe check if we are in battlestation
            //if (packetFlags2 & 0x02)
            //{
            //    char BattleStationSide;
            //}

            // Are we a pet?
            //if (packetFlags2 & 0x04)
            //{
            //    long PetMaster;
            //}

            packetWriter.PushByte(0);

            Byte[] reply = packetWriter.Finish();

            // Set Packet Flags
            Byte[] packetFlagBytes;
            packetFlagBytes = BitConverter.GetBytes(packetFlags);
            Array.Reverse(packetFlagBytes);
            reply[30] = packetFlagBytes[0];
            reply[31] = packetFlagBytes[1];
            reply[32] = packetFlagBytes[2];
            reply[33] = packetFlagBytes[3];

            return reply;
        }
    }
}