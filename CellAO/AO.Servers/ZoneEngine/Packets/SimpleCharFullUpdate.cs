// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleCharFullUpdate.cs" company="CellAO Team">
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
//   Defines the SimpleCharFullUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;
    using Quaternion = AO.Core.Quaternion;
    using Vector3 = SmokeLounge.AOtomation.Messaging.GameData.Vector3;

    public static class SimpleCharFullUpdate
    {
        #region Public Methods and Operators

        public static SimpleCharFullUpdateMessage ConstructMessage(Character character)
        {
            // Character Variables
            bool socialonly;
            bool showsocial;

            int charPlayfield;
            AOCoord charCoord;
            int charId;
            Quaternion charHeading;

            uint sideValue;
            uint fatValue;
            uint breedValue;
            uint sexValue;
            uint raceValue;

            string charName;
            int charFlagsValue;
            int accFlagsValue;

            int expansionValue;
            int currentNano;
            int currentHealth;

            uint strengthBaseValue;
            uint staminaBaseValue;
            uint agilityBaseValue;
            uint senseBaseValue;
            uint intelligenceBaseValue;
            uint psychicBaseValue;

            string firstName;
            string lastName;
            int orgNameLength;
            string orgName;
            int levelValue;
            int healthValue;
            int losHeight;

            int monsterData;
            int monsterScale;
            int visualFlags;

            int currentMovementMode;
            uint runSpeedBaseValue;

            int texturesCount;

            int headMeshValue;

            // NPC Values
            int NPCFamily;

            var socialTab = new Dictionary<int, int>();

            var textures = new List<AOTextures>();

            List<AOMeshs> meshs;

            var nanos = new List<AONano>();

            lock (character)
            {
                socialonly = (character.Stats.VisualFlags.Value & 0x40) > 0;
                showsocial = (character.Stats.VisualFlags.Value & 0x20) > 0;

                charPlayfield = character.PlayField;
                charCoord = character.Coordinates;
                charId = character.Id;
                charHeading = character.Heading;

                sideValue = character.Stats.Side.StatBaseValue;
                fatValue = character.Stats.Fatness.StatBaseValue;
                breedValue = character.Stats.Breed.StatBaseValue;
                sexValue = character.Stats.Sex.StatBaseValue;
                raceValue = character.Stats.Race.StatBaseValue;

                charName = character.Name;
                charFlagsValue = character.Stats.Flags.Value;
                accFlagsValue = character.Stats.AccountFlags.Value;

                expansionValue = character.Stats.Expansion.Value;
                currentNano = character.Stats.CurrentNano.Value;

                strengthBaseValue = character.Stats.Strength.StatBaseValue;
                staminaBaseValue = character.Stats.Strength.StatBaseValue;
                agilityBaseValue = character.Stats.Strength.StatBaseValue;
                senseBaseValue = character.Stats.Strength.StatBaseValue;
                intelligenceBaseValue = character.Stats.Strength.StatBaseValue;
                psychicBaseValue = character.Stats.Strength.StatBaseValue;

                firstName = character.FirstName;
                lastName = character.LastName;
                orgNameLength = character.OrgName.Length;
                orgName = character.OrgName;
                levelValue = character.Stats.Level.Value;
                healthValue = character.Stats.Life.Value;

                monsterData = character.Stats.MonsterData.Value;
                monsterScale = character.Stats.MonsterScale.Value;
                visualFlags = character.Stats.VisualFlags.Value;

                currentMovementMode = character.Stats.CurrentMovementMode.Value;
                runSpeedBaseValue = character.Stats.RunSpeed.StatBaseValue;

                texturesCount = character.Textures.Count;

                headMeshValue = character.Stats.HeadMesh.Value;

                foreach (var num in character.SocialTab.Keys)
                {
                    socialTab.Add(num, character.SocialTab[num]);
                }

                foreach (var at in character.Textures)
                {
                    textures.Add(new AOTextures(at.place, at.Texture));
                }

                meshs = MeshLayers.GetMeshs(character, showsocial, socialonly);

                foreach (var nano in character.ActiveNanos)
                {
                    var tempNano = new AONano();
                    tempNano.ID = nano.ID;
                    tempNano.Instance = nano.Instance;
                    tempNano.NanoStrain = nano.NanoStrain;
                    tempNano.Nanotype = nano.Nanotype;
                    tempNano.Time1 = nano.Time1;
                    tempNano.Time2 = nano.Time2;
                    tempNano.Value3 = nano.Value3;

                    nanos.Add(tempNano);
                }

                losHeight = character.Stats.LosHeight.Value;
                NPCFamily = character.Stats.NpcFamily.Value;
                currentHealth = character.Stats.Health.Value;
            }

            var scfu = new SimpleCharFullUpdateMessage();

            // affected identity
            scfu.Identity = new Identity { Type = IdentityType.CanbeAffected, Instance = charId };

            scfu.Version = 57; // SCFU packet version (57/0x39)
            scfu.Flags = SimpleCharFullUpdateFlags.None; // Try setting to 0x042062C8 if you have problems (old value)
            scfu.Flags |= SimpleCharFullUpdateFlags.HasPlayfieldId; // Has Playfield ID
            scfu.PlayfieldId = charPlayfield; // playfield

            if (character.FightingTarget.Instance != 0)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasFightingTarget;
                scfu.FightingTarget = new Identity
                                          {
                                              Type = (IdentityType)character.FightingTarget.Type, 
                                              Instance = character.FightingTarget.Instance
                                          };
            }

            // Coordinates
            scfu.Coordinates = new Vector3 { X = charCoord.x, Y = charCoord.y, Z = charCoord.z };

            // Heading Data
            scfu.Flags |= SimpleCharFullUpdateFlags.HasHeading;
            scfu.Heading = new SmokeLounge.AOtomation.Messaging.GameData.Quaternion
                               {
                                   W = charHeading.wf, 
                                   X = charHeading.xf, 
                                   Y = charHeading.yf, 
                                   Z = charHeading.zf
                               };

            // Race
            scfu.Appearance = new Appearance
                                  {
                                      Side = (Side)sideValue, 
                                      Fatness = (Fatness)fatValue, 
                                      Breed = (Breed)breedValue, 
                                      Gender = (Gender)sexValue, 
                                      Race = raceValue
                                  }; // appearance

            // Name
            scfu.Name = charName;

            scfu.CharacterFlags = (CharacterFlags)charFlagsValue; // Flags
            scfu.AccountFlags = (short)accFlagsValue;
            scfu.Expansions = (short)expansionValue;

            var isNpc = character is NonPlayerCharacterClass;

            if (isNpc)
            {
                // Are we a NPC (i think anyway)? So far this is _NOT_ used at all
                scfu.Flags |= SimpleCharFullUpdateFlags.IsNpc;

                var snpc = new SimpleNpcInfo { Family = (short)NPCFamily, LosHeight = (short)losHeight };
                scfu.CharacterInfo = snpc;
            }
            else
            {
                // Are we a player?
                var spc = new SimplePcInfo();

                spc.CurrentNano = (uint)currentNano; // CurrentNano
                spc.Team = 0; // team?
                spc.Swim = 5; // swim?

                // The checks here are to prevent the client doing weird things if the character has really large or small base attributes
                spc.StrengthBase = (short)Math.Min(strengthBaseValue, short.MaxValue); // Strength
                spc.AgilityBase = (short)Math.Min(agilityBaseValue, short.MaxValue); // Agility
                spc.StaminaBase = (short)Math.Min(staminaBaseValue, short.MaxValue); // Stamina
                spc.IntelligenceBase = (short)Math.Min(intelligenceBaseValue, short.MaxValue); // Intelligence
                spc.SenseBase = (short)Math.Min(senseBaseValue, short.MaxValue); // Sense
                spc.PsychicBase = (short)Math.Min(psychicBaseValue, short.MaxValue); // Psychic

                if (scfu.CharacterFlags.HasFlag(CharacterFlags.HasVisibleName))
                {
                    // has visible names? (Flags)
                    spc.FirstName = firstName;
                    spc.LastName = lastName;
                }

                if (orgNameLength != 0)
                {
                    scfu.Flags |= SimpleCharFullUpdateFlags.HasOrgName; // Has org name data
                    spc.OrgName = orgName;
                }

                scfu.CharacterInfo = spc;
            }

            // Level
            scfu.Level = (short)levelValue;
            if (scfu.Level > sbyte.MaxValue)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasExtendedLevel;
            }

            // Health
            scfu.Health = (uint)healthValue;
            if (scfu.Health <= short.MaxValue)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasSmallHealth;
            }

            scfu.HealthDamage = healthValue - currentHealth;
            if (scfu.HealthDamage <= byte.MaxValue)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasSmallHealthDamage;
            }

            // If player is in grid or fixer grid
            // make him/her/it a nice upside down pyramid
            if ((charPlayfield == 152) || (charPlayfield == 4107))
            {
                scfu.MonsterData = 99902;
            }
            else
            {
                scfu.MonsterData = (uint)monsterData; // Monsterdata
            }

            scfu.MonsterScale = (short)monsterScale; // Monsterscale
            scfu.VisualFlags = (short)visualFlags; // VisualFlags
            scfu.VisibleTitle = 0; // visible title?

            // 42 bytes long
            scfu.Unknown1 = new byte[]
                                {
                                    0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 
                                    (byte)currentMovementMode, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 
                                    0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                    0x00, 0x00, 0x00, 0x00, 0x00
                                };

            if (headMeshValue != 0)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasHeadMesh; // Has HeadMesh Flag
                scfu.HeadMesh = (uint?)headMeshValue; // Headmesh
            }

            // Runspeed
            scfu.RunSpeedBase = (short)runSpeedBaseValue;
            if (runSpeedBaseValue > sbyte.MaxValue)
            {
                scfu.Flags |= SimpleCharFullUpdateFlags.HasExtendedRunSpeed;
            }

            scfu.ActiveNanos = (from nano in nanos
                                select
                                    new ActiveNano
                                        {
                                            NanoId = nano.ID, 
                                            NanoInstance = nano.Instance, 
                                            Time1 = nano.Time1, 
                                            Time2 = nano.Time2
                                        }).ToArray();

            // Texture/Cloth Data
            var scfuTextures = new List<Texture>();

            var aotemp = new AOTextures(0, 0);
            for (var c = 0; c < 5; c++)
            {
                aotemp.Texture = 0;
                aotemp.place = c;
                for (var c2 = 0; c2 < texturesCount; c2++)
                {
                    if (textures[c2].place != c)
                    {
                        continue;
                    }

                    aotemp.Texture = textures[c2].Texture;
                    break;
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

                scfuTextures.Add(new Texture { Place = aotemp.place, Id = aotemp.Texture, Unknown = 0 });
            }

            scfu.Textures = scfuTextures.ToArray();

            // End Textures

            // ############
            // # Meshs
            // ############
            scfu.Meshes = (from aoMesh in meshs
                           select
                               new Mesh
                                   {
                                       Position = (byte)aoMesh.Position, 
                                       Id = (uint)aoMesh.Mesh, 
                                       OverrideTextureId = aoMesh.OverrideTexture, 
                                       Layer = (byte)aoMesh.Layer
                                   }).ToArray();

            // End Meshs
            scfu.Flags2 = 0; // packetFlags2
            scfu.Unknown2 = 0;

            return scfu;
        }

        public static SimpleCharFullUpdateMessage ConstructMessage(Client client)
        {
            return ConstructMessage(client.Character);
        }

        public static void SendToOne(Character character, Client receiver)
        {
            var message = ConstructMessage(character);
            receiver.SendCompressed(message);
        }

        public static void SendToPlayfield(Client client)
        {
            var message = ConstructMessage(client);
            Announce.Playfield(client.Character.PlayField, message);
        }

        [Obsolete]
        public static byte[] WritePacket(Character character, int receiver)
        {
            /*
             * To set a packetFlag use       packetFlags |= <flagHere>;
             * To unset a packetFlag use     packetFlags &= ~<flagHere>;
             * 
             * Wherever you set a flag conditionally, be sure to unset the same flag if the reverse
             * condition is true 'just in case'.
             */
            var packetFlags = 0; // Try setting to 0x042062C8 if you have problems (old value)

            // Character Variables
            bool socialonly;
            bool showsocial;

            /*
            bool showhelmet;
            bool LeftPadVisible;
            bool RightPadVisible;
            bool DoubleLeftPad;
            bool DoubleRightPad;
            */
            int charPlayfield;
            AOCoord charCoord;
            int charId;
            Quaternion charHeading;

            uint sideValue;
            uint fatValue;
            uint breedValue;
            uint sexValue;
            uint raceValue;

            int nameLength;
            string charName;
            int charFlagsValue;
            int accFlagsValue;

            int expansionValue;
            int currentNano;
            int currentHealth;

            uint strengthBaseValue;
            uint staminaBaseValue;
            uint agilityBaseValue;
            uint senseBaseValue;
            uint intelligenceBaseValue;
            uint psychicBaseValue;

            int firstNameLength;
            int lastNameLength;
            string firstName;
            string lastName;
            int orgNameLength;
            string orgName;
            int levelValue;
            int healthValue;
            int losHeight;

            int monsterData;
            int monsterScale;
            int visualFlags;

            int currentMovementMode;
            uint runSpeedBaseValue;

            int texturesCount;

            /*
            int HairMeshValue;
            int WeaponMeshRightValue;
            int WeaponMeshLeftValue;

            uint HeadMeshBaseValue;
             */
            int headMeshValue;

            /*
            int BackMeshValue;
            int ShoulderMeshRightValue;
             */
            // int ShoulderMeshLeftValue;

            /*
            int OverrideTextureHead;
            int OverrideTextureWeaponRight;
            int OverrideTextureWeaponLeft;
            int OverrideTextureShoulderpadRight;
            int OverrideTextureShoulderpadLeft;
            int OverrideTextureBack;
            int OverrideTextureAttractor;
            */
            // NPC Values
            int NPCFamily;

            var socialTab = new Dictionary<int, int>();

            var textures = new List<AOTextures>();

            List<AOMeshs> meshs;

            var nanos = new List<AONano>();

            lock (character)
            {
                socialonly = (character.Stats.VisualFlags.Value & 0x40) > 0;
                showsocial = (character.Stats.VisualFlags.Value & 0x20) > 0;

                /*
                showhelmet = ((character.Stats.VisualFlags.Value & 0x4) > 0);
                LeftPadVisible = ((character.Stats.VisualFlags.Value & 0x1) > 0);
                RightPadVisible = ((character.Stats.VisualFlags.Value & 0x2) > 0);
                DoubleLeftPad = ((character.Stats.VisualFlags.Value & 0x8) > 0);
                DoubleRightPad = ((character.Stats.VisualFlags.Value & 0x10) > 0);
                */
                charPlayfield = character.PlayField;
                charCoord = character.Coordinates;
                charId = character.Id;
                charHeading = character.Heading;

                sideValue = character.Stats.Side.StatBaseValue;
                fatValue = character.Stats.Fatness.StatBaseValue;
                breedValue = character.Stats.Breed.StatBaseValue;
                sexValue = character.Stats.Sex.StatBaseValue;
                raceValue = character.Stats.Race.StatBaseValue;

                nameLength = character.Name.Length;
                charName = character.Name;
                charFlagsValue = character.Stats.Flags.Value;
                accFlagsValue = character.Stats.AccountFlags.Value;

                expansionValue = character.Stats.Expansion.Value;
                currentNano = character.Stats.CurrentNano.Value;

                strengthBaseValue = character.Stats.Strength.StatBaseValue;
                staminaBaseValue = character.Stats.Strength.StatBaseValue;
                agilityBaseValue = character.Stats.Strength.StatBaseValue;
                senseBaseValue = character.Stats.Strength.StatBaseValue;
                intelligenceBaseValue = character.Stats.Strength.StatBaseValue;
                psychicBaseValue = character.Stats.Strength.StatBaseValue;

                firstNameLength = character.FirstName.Length;
                lastNameLength = character.LastName.Length;
                firstName = character.FirstName;
                lastName = character.LastName;
                orgNameLength = character.OrgName.Length;
                orgName = character.OrgName;
                levelValue = character.Stats.Level.Value;
                healthValue = character.Stats.Life.Value;

                monsterData = character.Stats.MonsterData.Value;
                monsterScale = character.Stats.MonsterScale.Value;
                visualFlags = character.Stats.VisualFlags.Value;

                currentMovementMode = character.Stats.CurrentMovementMode.Value;
                runSpeedBaseValue = character.Stats.RunSpeed.StatBaseValue;

                texturesCount = character.Textures.Count;

                /*
                HairMeshValue = character.Stats.HairMesh.Value;
                WeaponMeshRightValue = character.Stats.WeaponMeshRight.Value;
                WeaponMeshLeftValue = character.Stats.WeaponMeshLeft.Value;

                HeadMeshBaseValue = character.Stats.HeadMesh.StatBaseValue;
                 */
                headMeshValue = character.Stats.HeadMesh.Value;

                /*
                BackMeshValue = character.Stats.BackMesh.Value;
                ShoulderMeshRightValue = character.Stats.ShoulderMeshRight.Value;
                 */
                // ShoulderMeshLeftValue = character.Stats.ShoulderMeshLeft.Value;
                /*
                OverrideTextureHead = character.Stats.OverrideTextureHead.Value;
                OverrideTextureWeaponRight = character.Stats.OverrideTextureWeaponRight.Value;
                OverrideTextureWeaponLeft = character.Stats.OverrideTextureWeaponLeft.Value;
                OverrideTextureShoulderpadRight = character.Stats.OverrideTextureShoulderpadRight.Value;
                OverrideTextureShoulderpadLeft = character.Stats.OverrideTextureShoulderpadLeft.Value;
                OverrideTextureBack = character.Stats.OverrideTextureBack.Value;
                OverrideTextureAttractor = character.Stats.OverrideTextureAttractor.Value;
                 */
                foreach (var num in character.SocialTab.Keys)
                {
                    socialTab.Add(num, character.SocialTab[num]);
                }

                foreach (var at in character.Textures)
                {
                    textures.Add(new AOTextures(at.place, at.Texture));
                }

                meshs = MeshLayers.GetMeshs(character, showsocial, socialonly);

                foreach (var nano in character.ActiveNanos)
                {
                    var tempNano = new AONano();
                    tempNano.ID = nano.ID;
                    tempNano.Instance = nano.Instance;
                    tempNano.NanoStrain = nano.NanoStrain;
                    tempNano.Nanotype = nano.Nanotype;
                    tempNano.Time1 = nano.Time1;
                    tempNano.Time2 = nano.Time2;
                    tempNano.Value3 = nano.Value3;

                    nanos.Add(tempNano);
                }

                losHeight = character.Stats.LosHeight.Value;
                NPCFamily = character.Stats.NpcFamily.Value;
                currentHealth = character.Stats.Health.Value;
            }

            var packetWriter = new PacketWriter();

            // Packet Header
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0); // length. writer will take care of this
            packetWriter.PushInt(3086); // sender. our server ID
            packetWriter.PushInt(receiver); // receiver
            packetWriter.PushInt(0x271B3A6B); // packet ID
            packetWriter.PushIdentity(50000, charId); // affected identity
            packetWriter.PushByte(0); // Unknown?

            // End Packet Header
            packetWriter.PushByte(57); // SCFU packet version (57/0x39)
            packetWriter.PushInt(0); // packet flags (this is set later based on packetFlags variable above)

            packetFlags |= 0x40; // Has Playfield ID
            packetWriter.PushInt(charPlayfield); // playfield

            if (character.FightingTarget.Instance != 0)
            {
                packetFlags |= 20;
                packetWriter.PushIdentity(character.FightingTarget);
            }

            // Coordinates
            packetWriter.PushCoord(charCoord);

            // Heading Data
            packetFlags |= 0x200; // Has Heading Data Flag
            packetWriter.PushQuat(charHeading);

            var appearance = sideValue + (fatValue * 8) + (breedValue * 32) + (sexValue * 256) + (raceValue * 1024);

            // Race
            packetWriter.PushUInt(appearance); // appearance

            // Name
            packetWriter.PushByte((byte)(nameLength + 1));
            packetWriter.PushBytes(Encoding.ASCII.GetBytes(charName));
            packetWriter.PushByte(0); // 0 terminator for name

            packetWriter.PushUInt(charFlagsValue); // Flags
            packetWriter.PushShort((short)accFlagsValue);
            packetWriter.PushShort((short)expansionValue);

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

                if (losHeight < 256)
                {
                    packetWriter.PushByte((byte)losHeight);
                }
                else
                {
                    packetFlags |= 0x80000;
                    packetWriter.PushShort((Int16)losHeight);
                }

                // if (packetFlags & 0x2000000)
                // {
                // char PetType;
                // }
                // else
                // {
                // short PetType;
                // }

                // short TowerType;

                // if (TowerType > 0)
                // {
                // char unknown;
                // }
            }
            else
            {
                // Are we a player?
                packetWriter.PushUInt(currentNano); // CurrentNano
                packetWriter.PushInt(0); // team?
                packetWriter.PushShort(5); // swim?

                // The checks here are to prevent the client doing weird things if the character has really large or small base attributes
                if (strengthBaseValue > 32767)
                {
                    // Strength
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)strengthBaseValue);
                }

                if (agilityBaseValue > 32767)
                {
                    // Agility
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)agilityBaseValue);
                }

                if (staminaBaseValue > 32767)
                {
                    // Stamina
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)staminaBaseValue);
                }

                if (intelligenceBaseValue > 32767)
                {
                    // Intelligence
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)intelligenceBaseValue);
                }

                if (senseBaseValue > 32767)
                {
                    // Sense
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)senseBaseValue);
                }

                if (psychicBaseValue > 32767)
                {
                    // Psychic
                    packetWriter.PushShort(32767);
                }
                else
                {
                    packetWriter.PushShort((short)psychicBaseValue);
                }

                if ((charFlagsValue & 0x400000) != 0)
                {
                    // has visible names? (Flags)
                    packetWriter.PushShort((short)firstNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(firstName));
                    packetWriter.PushShort((short)lastNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(lastName));
                }

                if (orgNameLength != 0)
                {
                    packetFlags |= 0x4000000; // Has org name data

                    packetWriter.PushShort((short)orgNameLength);
                    packetWriter.PushBytes(Encoding.ASCII.GetBytes(orgName));
                }
                else
                {
                    packetFlags &= ~0x4000000; // Does not have org name data
                }
            }

            if (levelValue > 127)
            {
                // Level
                packetFlags |= 0x1000; // Has Extended Level
                packetWriter.PushShort((short)levelValue);
            }
            else
            {
                packetFlags &= ~0x1000; // Has Small Level
                packetWriter.PushByte((byte)levelValue);
            }

            if (healthValue > 32767)
            {
                // Health
                packetFlags &= ~0x800; // Has Extended Health
                packetWriter.PushUInt(healthValue);
            }
            else
            {
                packetFlags |= 0x800; // Has Small Health
                packetWriter.PushShort((short)healthValue);
            }

            var healthdamage = healthValue - currentHealth;
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
            if ((charPlayfield == 152) || (charPlayfield == 4107))
            {
                packetWriter.PushInt(99902);
            }
            else
            {
                packetWriter.PushUInt(monsterData); // Monsterdata
            }

            packetWriter.PushShort((short)monsterScale); // Monsterscale
            packetWriter.PushShort((short)visualFlags); // VisualFlags
            packetWriter.PushByte(0); // visible title?

            packetWriter.PushInt(42); // 'skipdata' length

            // Start 'skipdata'
            packetWriter.PushBytes(
                new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 });
            packetWriter.PushByte((byte)currentMovementMode); // CurrentMovementMode
            packetWriter.PushByte(1); // don't change
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(1); // ?
            packetWriter.PushShort(0); // ?
            packetWriter.PushShort(3); // ?
            packetWriter.PushInt(0); // ?
            packetWriter.PushInt(0); // ?
            packetWriter.PushInt(0); // ?
            packetWriter.PushInt(0); // ?

            // End 'skipdata'
            if (headMeshValue != 0)
            {
                packetFlags |= 0x80; // Has HeadMesh Flag
                packetWriter.PushUInt(headMeshValue); // Headmesh
            }

            if (runSpeedBaseValue > 127)
            {
                // Runspeed
                packetFlags |= 0x2000;
                packetWriter.PushShort((short)runSpeedBaseValue);
            }
            else
            {
                packetFlags &= ~0x2000;
                packetWriter.PushByte((byte)runSpeedBaseValue);
            }

            // if (packetFlags & 0x400)
            // {
            // // Pop2Long
            // /*
            // * Is this a Type:Instance pair?
            // * Suspect so as it uses Pop2Long
            // * which is used for Type:Instance
            // * pairs. Perhaps pet master?
            // * (Just a wild guess though that its pet master)
            // */
            // long unknown;
            // long unknown;
            // }

            // if (packetFlags & 0x10)
            // {
            // long counter;
            // repeat (counter / 0x3F1 - 1) times
            // char texturepositionname[32]; // Null padded at the end
            // long textureid; // Or is this mesh id?
            // long unknown;
            // long unknown;
            // end repeat
            // }

            // Is char/NPC in hide mode?
            // if (packetFlags & 0x100000)
            // {
            // short Concealment;
            // }

            // if (packetFlags & 0x800000)
            // {
            // char unknown;
            // }

            // if (packetFlags & 0x1000000)
            // {
            // char unknown;
            // }
            packetWriter.Push3F1Count(nanos.Count); // running nanos count
            foreach (var nano in nanos)
            {
                packetWriter.PushInt(nano.ID);
                packetWriter.PushInt(nano.Instance);
                packetWriter.PushInt(nano.Time1);
                packetWriter.PushInt(nano.Time2);
            }

            // longx5: aoid, instance, unknown(0?), timer1, timer2

            // if (flags & 0x10000)
            // {
            // // Waypoint Info
            // // Pop2Long (1010E2D3)
            // long type;
            // long instance;
            // // Waypoint Counter - 3x float per entry
            // long counter;
            // repeat counter times
            // float x;
            // float y;
            // float z;
            // end repeat
            // // End Waypoint Counter
            // }

            // Texture/Cloth Data
            int c;
            packetWriter.Push3F1Count(5); // textures count

            var aotemp = new AOTextures(0, 0);
            for (c = 0; c < 5; c++)
            {
                aotemp.Texture = 0;
                aotemp.place = c;
                int c2;
                for (c2 = 0; c2 < texturesCount; c2++)
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
            foreach (var aoMeshs in meshs)
            {
                packetWriter.PushByte((byte)aoMeshs.Position);
                packetWriter.PushUInt(aoMeshs.Mesh);
                packetWriter.PushInt(aoMeshs.OverrideTexture); // Override Texture!!!!!!
                packetWriter.PushByte((byte)aoMeshs.Layer);
            }

            // End Meshs

            // if (packetFlags & 0x100)
            // {
            // // 0x3F1 Unknown Counter - 4x long per entry
            // long counter;
            // repeat (counter / 0x3F1 - 1) times
            // long unknown;
            // long unknown;
            // long unknown;
            // long unknown;
            // end repeat
            // }

            // if (packetFlags & 0x20000000)
            // {
            // char ShadowBreed;
            // }

            // if (packetFlags & 0x40000000)
            // {
            // // 0x3F1 Unknown Counter - 4x long per entry
            // long counter;
            // repeat (counter / 0x3F1 - 1) times
            // Pop2Long (Type:Instance pair maybe?)
            // long unknown;
            // long unknown;
            // end repeat
            // }
            packetWriter.PushInt(0); // packetFlags2

            // Some mech stuff
            // if (packetFlags2 & 0x01)
            // {
            // long counter;
            // repeat (counter) times
            // long unknown;
            // long unknown;
            // end repeat

            // long MechData;
            // // Pop2Long (Type:Instance pair maybe?)
            // long unknown;
            // long unknown;
            // }

            // maybe check if we are in battlestation
            // if (packetFlags2 & 0x02)
            // {
            // char BattleStationSide;
            // }

            // Are we a pet?
            // if (packetFlags2 & 0x04)
            // {
            // long PetMaster;
            // }
            packetWriter.PushByte(0);

            var reply = packetWriter.Finish();

            // Set Packet Flags
            byte[] packetFlagBytes;
            packetFlagBytes = BitConverter.GetBytes(packetFlags);
            Array.Reverse(packetFlagBytes);
            reply[30] = packetFlagBytes[0];
            reply[31] = packetFlagBytes[1];
            reply[32] = packetFlagBytes[2];
            reply[33] = packetFlagBytes[3];

            return reply;
        }

        #endregion
    }
}