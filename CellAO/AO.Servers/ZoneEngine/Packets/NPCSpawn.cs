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
    using System.Text;

    using AO.Core;

    using ZoneEngine.Misc;

    public static class NPCSpawn
    {
        public static void NPCSpawntoClient(NonPC mob, Client cli, bool wholeplayfield)
        {
            int unknown1 = 0;
            int unknown2 = 0;
            int unknown3 = 0;
            PacketWriter spawn = new PacketWriter();
            int counter, counter2;
            int packetflags = 0x0001; // We want to spawn a NPC
            packetflags |= 0x0200; // Heading flag
            packetflags |= 0x0040; // packet has playfield
            packetflags |= 0x1000; // push level as 2 byte
            //            if (mob.Stats.GetStat(466) <= 255)
            {
                packetflags |= 0x80000; // LOS Height 1 byte
            }

            if (mob.Attacking != 0)
            {
                packetflags |= 0x400;
            }

            packetflags |= 0x2000000;
            packetflags |= 0x0200000;
            packetflags |= 0x0000002;

            spawn.PushByte(0xDF);
            spawn.PushByte(0xDF);
            spawn.PushShort(10);
            spawn.PushShort(1);
            spawn.PushShort(0); // Length, packetwriter will take care of this
            spawn.PushInt(3086);
            if (cli == null)
            {
                spawn.PushInt(0); // will be sent to whole playfield
            }
            else
            {
                spawn.PushInt(cli.Character.ID);
            }
            spawn.PushInt(0x271B3A6B);
            spawn.PushIdentity(50000, mob.ID);
            spawn.PushByte(0);
            spawn.PushByte(0x39); // version 0x39
            spawn.PushInt(packetflags); // packetflags
            spawn.PushInt(mob.PlayField);
            spawn.PushCoord(mob.Coordinates);
            spawn.PushQuat(mob.Heading);
            // Side, Fatness, Breed, Sex, Race
            //   33,      47,     4,  59,    89
            spawn.PushUInt(
                mob.Stats.Side.Value + (mob.Stats.Fatness.Value * 8) + (mob.Stats.Breed.Value * 32)
                + (mob.Stats.Sex.Value * 256) + (mob.Stats.Race.Value * 1024));
            spawn.PushByte((byte)(mob.Name.Length + 1));
            spawn.PushBytes(Encoding.ASCII.GetBytes(mob.Name));
            spawn.PushByte(0);
            spawn.PushUInt(mob.Stats.Flags.Value);
            spawn.PushShort(0); // AccountFlags
            spawn.PushShort(0); // Expansion
            if (mob.Stats.NPCFamily.Value <= 255) // NPCFamily
            {
                packetflags |= 0x20000; // NPC Family 1 byte
                spawn.PushByte((byte)mob.Stats.NPCFamily.Value);
            }
            else
            {
                packetflags &= ~0x20000; // NPC Family 2 byte
                spawn.PushShort((short)mob.Stats.NPCFamily.Value);
            }

            spawn.PushByte(0);
            spawn.PushByte(0);
            spawn.PushShort(0);

            // TODO: set packetflag for levelsize
            spawn.PushShort((short)mob.Stats.Level.Value); // 54 = Level

            // TODO: set packetflag for Healthsize/damagesize
            spawn.PushUInt(mob.Stats.Life.Value); // 1 = Life (max HP)
            spawn.PushUInt(mob.Stats.Health.Value); // 27 = Health left?? (same Size as Health, flag for 1byte not set)

            // If NPC is in grid or fixer grid
            // make him look like nice upside down pyramid
            if ((mob.PlayField == 152) || (mob.PlayField == 4107))
            {
                spawn.PushInt(99902);
            }
            else
            {
                spawn.PushUInt(mob.Stats.MonsterData.Value); // 359=Monsterdata
            }

            spawn.PushShort((short)mob.Stats.MonsterScale.Value); // 360 = monsterscale
            spawn.PushShort(0x1F); // VisualFlags
            spawn.PushByte(0); // Visible title?
            spawn.PushInt(0x1C);
            spawn.PushInt(unknown1); // KnuBot values?
            spawn.PushInt(unknown2);
            spawn.PushInt(unknown3);

            // TODO: Movement Modes
            spawn.PushByte(1); // CurrentMovementMode
            spawn.PushByte(1); // Don't change
            spawn.PushShort(1);
            spawn.PushShort(1);
            spawn.PushShort(1);
            spawn.PushShort(1);
            spawn.PushShort(0);
            spawn.PushShort(2);
            spawn.PushShort(0);

            if (mob.Stats.HeadMesh.Value != 0) // 64 = headmesh
            {
                packetflags |= 0x80;
                spawn.PushUInt(mob.Stats.HeadMesh.Value);
            }

            // TODO: runspeedsize+flag
            if (mob.Stats.RunSpeed.Value > 255)
            {
                packetflags |= 0x2000;
                spawn.PushShort((short)mob.Stats.RunSpeed.Value); // 156 = RunSpeed
            }
            else
            {
                spawn.PushByte((byte)mob.Stats.RunSpeed.Value); // 156 = RunSpeed
            }

            if (mob.Attacking != 0)
            {
                spawn.PushInt(0xc350);
                spawn.PushInt(mob.Attacking);
            }

            if (mob.Meshs.Count > 0)
            {
                packetflags |= 0x10; // Meshs on mob
                spawn.Push3F1Count(mob.Meshs.Count);
                for (counter = 0; counter < mob.Meshs.Count; counter++)
                {
                    // Name for meshtemplate not needed, sending 32byte 00 instead, thx to Suiv
                    for (counter2 = 0; counter2 < 8; counter2++)
                    {
                        spawn.PushInt(0);
                    }
                    spawn.PushInt(mob.Meshs[counter].Position);
                    spawn.PushInt(mob.Meshs[counter].Mesh);
                    spawn.PushInt(mob.Meshs[counter].OverrideTexture);
                }
            }

            // Running Nanos/Nano Effects
            spawn.Push3F1Count(mob.ActiveNanos.Count);
            for (counter = 0; counter < mob.ActiveNanos.Count; counter++)
            {
                spawn.PushInt(mob.ActiveNanos[counter].Nanotype);
                spawn.PushInt(mob.ActiveNanos[counter].Instance);
                spawn.PushInt(mob.ActiveNanos[counter].Value3);
                spawn.PushInt(mob.ActiveNanos[counter].Time1);
                spawn.PushInt(mob.ActiveNanos[counter].Time2);
            }

            // Waypoints
            if (mob.Waypoints.Count > 0)
            {
                packetflags |= 0x10000; // Waypoints
                spawn.PushInt(0xc350);
                spawn.PushInt(mob.ID);
                spawn.Push3F1Count(mob.Waypoints.Count); // Waypoints
                for (counter = 0; counter < mob.Waypoints.Count; counter++)
                {
                    spawn.PushCoord(mob.Waypoints[counter]);
                }
            }

            /// Textures have to be rewritten too
            /// mobs should get a equip table
            /// and get the textures from the equipped items

            spawn.Push3F1Count(mob.Textures.Count); // Texture count (should be 5 at all times iirc)
            int c;
            for (c = 0; c < mob.Textures.Count; c++)
            {
                spawn.PushInt(mob.Textures[c].place);
                spawn.PushInt(mob.Textures[c].Texture);
                spawn.PushInt(0);
            }

            /// same as texture part, equip table should define the additional meshs
            /// data could be stored with the item entries

            int addmeshs = 0;
            if (mob.Stats.WeaponMeshRight.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.WeaponMeshLeft.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.HeadMesh.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.BackMesh.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.ShoulderMeshRight.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.ShoulderMeshLeft.Value != 0)
            {
                addmeshs++;
            }
            if (mob.Stats.HairMesh.Value != 0)
            {
                addmeshs++;
            }
            //            if (mob.Stats.GetStat(42) != 0) // 42 = CATMesh, what is this?
            //                addmeshs++;
            if (mob.Stats.HairMesh.Value != 0)
            {
                addmeshs++;
            }

            spawn.Push3F1Count(addmeshs);
            if (addmeshs > 0)
            {
                // 0 head, 1 r_hand, 2 l_hand, 3 r_shoulder, 4 l_shoulder, 5 back, 6 hip, 7 r_thigh, 8 l_thigh, 9 r_crus, 10 l_crus, 11 r_arm, 12 l_arm, 13 r_forearm, 14 l_forearm
                if (mob.Stats.HeadMesh.Value != 0)
                {
                    spawn.PushByte(0);
                    spawn.PushUInt(mob.Stats.HeadMesh.Value);
                    spawn.PushInt(0);
                    spawn.PushByte(4);
                }
                if (mob.Stats.WeaponMeshRight.Value != 0)
                {
                    spawn.PushByte(1); // Position
                    spawn.PushUInt(mob.Stats.WeaponMeshRight.Value); // Mesh ID
                    spawn.PushInt(0); // Unknown
                    spawn.PushByte(4); // Priority
                }
                if (mob.Stats.WeaponMeshLeft.Value != 0)
                {
                    spawn.PushByte(2); // Position
                    spawn.PushUInt(mob.Stats.WeaponMeshLeft.Value); // Mesh ID
                    spawn.PushInt(0); // Unknown
                    spawn.PushByte(4); // Priority
                }
                if (mob.Stats.ShoulderMeshRight.Value != 0)
                {
                    spawn.PushByte(3);
                    spawn.PushUInt(mob.Stats.ShoulderMeshRight.Value);
                    spawn.PushInt(0);
                    spawn.PushByte(4);
                }
                if (mob.Stats.ShoulderMeshLeft.Value != 0)
                {
                    spawn.PushByte(4);
                    spawn.PushUInt(mob.Stats.ShoulderMeshLeft.Value);
                    spawn.PushInt(0);
                    spawn.PushByte(4);
                }
                if (mob.Stats.BackMesh.Value != 0)
                {
                    spawn.PushByte(5);
                    spawn.PushUInt(mob.Stats.BackMesh.Value);
                    spawn.PushInt(0);
                    spawn.PushByte(4);
                }
                if (mob.Stats.HairMesh.Value != 0)
                {
                    spawn.PushByte(0);
                    spawn.PushUInt(mob.Stats.HairMesh.Value);
                    spawn.PushInt(0);
                    spawn.PushByte(2); // Hairmesh is prio 2?
                }
                /*                if (mob.Stats.GetStat(20001) != 0)
                {
                    spawn.PushByte(0);
                    spawn.PushUInt(mob.Stats.GetStat(20001));
                    spawn.PushInt(0);
                    spawn.PushByte(0); // Attractor Mesh prio = 0
                }
 */
            }

            if (mob.Weaponpairs.Count > 0)
            {
                spawn.Push3F1Count(mob.Weaponpairs.Count);
                for (counter = 0; counter < mob.Weaponpairs.Count; counter++)
                {
                    spawn.PushInt(mob.Weaponpairs[counter].value1);
                    spawn.PushInt(mob.Weaponpairs[counter].value2);
                    spawn.PushInt(mob.Weaponpairs[counter].value3);
                    spawn.PushInt(mob.Weaponpairs[counter].value4);
                }
            }
            // Finishing output with 5byte 00
            spawn.PushInt(0);
            spawn.PushByte(0);

            byte[] spawnReply = spawn.Finish();

            // setting the packetflags
            spawnReply[30] = (byte)((packetflags >> 24) & 0xff);
            spawnReply[31] = (byte)((packetflags >> 16) & 0xff);
            spawnReply[32] = (byte)((packetflags >> 8) & 0xff);
            spawnReply[33] = (byte)(packetflags & 0xff);

            if (wholeplayfield)
            {
                Announce.Playfield(mob.PlayField, ref spawnReply);
            }
            else
            {
                cli.SendCompressed(spawnReply);
            }
        }
    }
}