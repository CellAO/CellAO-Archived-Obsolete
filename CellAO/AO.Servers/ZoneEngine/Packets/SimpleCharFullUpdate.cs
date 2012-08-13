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
        public static void SendToPf(Client client)
        {
            byte[] mReply = GetPacket(client);
            Announce.Playfield(client.Character.PlayField, ref mReply);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void SendToPfOthers(Client client)
        {
            Byte[] data = GetPacket(client);
            Announce.PlayfieldOthers(client, ref data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="receiver"></param>
        public static void SendToOne(Character character, Client receiver)
        {
            Byte[] data = WritePacket(character, receiver.Character.ID);
            receiver.SendCompressed(data);
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
        /// <param name="client"></param>
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

            Dictionary<int, int> SocialTab = new Dictionary<int, int>();

            List<AOTextures> Textures = new List<AOTextures>();

            List<AOMeshs> mh = new List<AOMeshs>();

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
                OrgNameLength = character.orgName.Length;
                OrgName = character.orgName;
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
                    SocialTab.Add(num, character.SocialTab[num]);
                }

                foreach (AOTextures at in character.Textures)
                {
                    Textures.Add(new AOTextures(at.place, at.Texture));
                }

                mh = MeshLayers.GetMeshs(character, showsocial, socialonly);

                foreach (AONano an in character.ActiveNanos)
                {
                    AONano AN = new AONano();
                    AN.ID = an.ID;
                    AN.Instance = an.Instance;
                    AN.NanoStrain = an.NanoStrain;
                    AN.Nanotype = an.Nanotype;
                    AN.Time1 = an.Time1;
                    AN.Time2 = an.Time2;
                    AN.Value3 = an.Value3;

                    nanos.Add(AN);
                }

                LOSHeight = character.Stats.LOSHeight.Value;
                NPCFamily = character.Stats.NPCFamily.Value;
                CurrentHealth = character.Stats.Health.Value;
            }
            PacketWriter _writer = new PacketWriter();

            // Packet Header
            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            _writer.PushShort(10);
            _writer.PushShort(1);
            _writer.PushShort(0); // length. writer will take care of this
            _writer.PushInt(3086); // sender. our server ID
            _writer.PushInt(receiver); // receiver
            _writer.PushInt(0x271B3A6B); // packet ID
            _writer.PushIdentity(50000, CharID); // affected identity
            _writer.PushByte(0); // Unknown?
            // End Packet Header

            _writer.PushByte(57); // SCFU packet version (57/0x39)
            _writer.PushInt(0); // packet flags (this is set later based on packetFlags variable above)

            packetFlags |= 0x40; // Has Playfield ID
            _writer.PushInt(CharPlayfield); // playfield

            if (character.FightingTarget.Instance != 0)
            {
                packetFlags |= 20;
                _writer.PushIdentity(character.FightingTarget);
            }

            // Coordinates
            _writer.PushCoord(CharCoord);

            // Heading Data
            packetFlags |= 0x200; // Has Heading Data Flag
            _writer.PushQuat(CharHeading);

            uint m_appearance = SideValue + (FatValue * 8) + (BreedValue * 32) + (SexValue * 256) + (RaceValue * 1024);
                // Race
            _writer.PushUInt(m_appearance); // appearance

            // Name
            _writer.PushByte((byte)(NameLength + 1));
            _writer.PushBytes(Encoding.ASCII.GetBytes(CharName));
            _writer.PushByte(0); // 0 terminator for name

            _writer.PushUInt(CharFlagsValue); // Flags
            _writer.PushShort((short)AccFlagsValue);
            _writer.PushShort((short)ExpansionValue);

            if (character is NonPC)
            {
                packetFlags |= 1;
            }

            packetFlags &= ~0x01; // We are a player
            if ((packetFlags & 0x01) != 0)
            {
                // Are we a NPC (i think anyway)? So far this is _NOT_ used at all

                if (NPCFamily < 256)
                {
                    _writer.PushByte((byte)NPCFamily);
                }
                else
                {
                    packetFlags |= 0x20000;
                    _writer.PushShort((Int16)NPCFamily);
                }

                if (LOSHeight < 256)
                {
                    _writer.PushByte((byte)LOSHeight);
                }
                else
                {
                    packetFlags |= 0x80000;
                    _writer.PushShort((Int16)LOSHeight);
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
                _writer.PushUInt(CurrentNano); // CurrentNano
                _writer.PushInt(0); // team?
                _writer.PushShort(5); // swim?

                // The checks here are to prevent the client doing weird things if the character has really large or small base attributes
                if (StrengthBaseValue > 32767) // Strength
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)StrengthBaseValue);
                }
                if (AgilityBaseValue > 32767) // Agility
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)AgilityBaseValue);
                }
                if (StaminaBaseValue > 32767) //  Stamina
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)StaminaBaseValue);
                }
                if (IntelligenceBaseValue > 32767) // Intelligence
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)IntelligenceBaseValue);
                }
                if (SenseBaseValue > 32767) // Sense
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)SenseBaseValue);
                }
                if (PsychicBaseValue > 32767) // Psychic
                {
                    _writer.PushShort(32767);
                }
                else
                {
                    _writer.PushShort((short)PsychicBaseValue);
                }

                if ((CharFlagsValue & 0x400000) != 0) // has visible names? (Flags)
                {
                    _writer.PushShort((short)FirstNameLength);
                    _writer.PushBytes(Encoding.ASCII.GetBytes(FirstName));
                    _writer.PushShort((short)LastNameLength);
                    _writer.PushBytes(Encoding.ASCII.GetBytes(LastName));
                }

                if (OrgNameLength != 0)
                {
                    packetFlags |= 0x4000000; // Has org name data

                    _writer.PushShort((short)OrgNameLength);
                    _writer.PushBytes(Encoding.ASCII.GetBytes(OrgName));
                }
                else
                {
                    packetFlags &= ~0x4000000; // Does not have org name data
                }
            }

            if (LevelValue > 127) // Level
            {
                packetFlags |= 0x1000; // Has Extended Level
                _writer.PushShort((short)LevelValue);
            }
            else
            {
                packetFlags &= ~0x1000; // Has Small Level
                _writer.PushByte((byte)LevelValue);
            }

            if (HealthValue > 32767) // Health
            {
                packetFlags &= ~0x800; // Has Extended Health
                _writer.PushUInt(HealthValue);
            }
            else
            {
                packetFlags |= 0x800; // Has Small Health
                _writer.PushShort((short)HealthValue);
            }
            int healthdamage = HealthValue - CurrentHealth;
            if (healthdamage < 256)
            {
                packetFlags |= 0x4000;
                _writer.PushByte((byte)healthdamage);
            }
            else
            {
                packetFlags &= ~0x4000;
                if ((packetFlags & 0x800) == 0x800)
                {
                    _writer.PushShort((Int16)healthdamage);
                }
                else
                {
                    _writer.PushInt(healthdamage);
                }
            }

            // If player is in grid or fixer grid
            // make him/her/it a nice upside down pyramid
            if ((CharPlayfield == 152) || (CharPlayfield == 4107))
            {
                _writer.PushInt(99902);
            }
            else
            {
                _writer.PushUInt(MonsterData); // Monsterdata
            }
            _writer.PushShort((short)MonsterScale); // Monsterscale
            _writer.PushShort((short)VisualFlags); // VisualFlags
            _writer.PushByte(0); // visible title?

            _writer.PushInt(42); // 'skipdata' length
            // Start 'skipdata'
            _writer.PushBytes(new Byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 });
            _writer.PushByte((byte)CurrentMovementMode); // CurrentMovementMode
            _writer.PushByte(1); // don't change
            _writer.PushShort(1); // ?
            _writer.PushShort(1); // ?
            _writer.PushShort(1); // ?
            _writer.PushShort(1); // ?
            _writer.PushShort(0); // ?
            _writer.PushShort(3); // ?
            _writer.PushInt(0); //?
            _writer.PushInt(0); //?
            _writer.PushInt(0); //?
            _writer.PushInt(0); //?
            // End 'skipdata'

            if (HeadMeshValue != 0)
            {
                packetFlags |= 0x80; // Has HeadMesh Flag
                _writer.PushUInt(HeadMeshValue); // Headmesh
            }

            if ((RunSpeedBaseValue > 127)) // Runspeed
            {
                packetFlags |= 0x2000;
                _writer.PushShort((short)RunSpeedBaseValue);
            }
            else
            {
                packetFlags &= ~0x2000;
                _writer.PushByte((byte)RunSpeedBaseValue);
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

            _writer.Push3F1Count(nanos.Count); // running nanos count
            foreach (AONano nano in nanos)
            {
                _writer.PushInt(nano.ID);
                _writer.PushInt(nano.Instance);
                _writer.PushInt(nano.Time1);
                _writer.PushInt(nano.Time2);
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
            int c2;
            Textures tx = new Textures();
            tx.GetTextures(CharID);
            _writer.Push3F1Count(5); // textures count

            AOTextures aotemp = new AOTextures(0, 0);
            for (c = 0; c < 5; c++)
            {
                aotemp.Texture = 0;
                aotemp.place = c;
                for (c2 = 0; c2 < TexturesCount; c2++)
                {
                    if (Textures[c2].place == c)
                    {
                        aotemp.Texture = Textures[c2].Texture;
                        break;
                    }
                }
                if (showsocial)
                {
                    if (socialonly)
                    {
                        aotemp.Texture = SocialTab[c];
                    }
                    else
                    {
                        if (SocialTab[c] != 0)
                        {
                            aotemp.Texture = SocialTab[c];
                        }
                    }
                }

                _writer.PushInt(aotemp.place);
                _writer.PushInt(aotemp.Texture);
                _writer.PushInt(0);
            }
            // End Textures

            // ############
            // # Meshs
            // ############

            c = mh.Count;

            _writer.Push3F1Count(c);
            foreach (AOMeshs m2 in mh)
            {
                _writer.PushByte((byte)m2.Position);
                _writer.PushUInt(m2.Mesh);
                _writer.PushInt(m2.OverrideTexture); // Override Texture!!!!!!
                _writer.PushByte((byte)m2.Layer);
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

            _writer.PushInt(0); // packetFlags2

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

            _writer.PushByte(0);

            Byte[] reply = _writer.Finish();

            // Set Packet Flags
            Byte[] m_packetFlags;
            m_packetFlags = BitConverter.GetBytes(packetFlags);
            Array.Reverse(m_packetFlags);
            reply[30] = m_packetFlags[0];
            reply[31] = m_packetFlags[1];
            reply[32] = m_packetFlags[2];
            reply[33] = m_packetFlags[3];

            return reply;
        }
    }
}