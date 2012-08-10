#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System.Collections.Generic;
using AO.Core;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine.Packets
{
    public static class AppearanceUpdate
    {
        public static void Appearance_Update(Character character)
        {
            List<AOMeshs> mh;
            PacketWriter wri = new PacketWriter();
            wri.PushByte(0xDF);
            wri.PushByte(0xDF);
            wri.PushShort(10);
            wri.PushShort(1);
            wri.PushShort(0); // Packet size. writer will take care of this
            wri.PushInt(3086);
            wri.PushInt(character.ID);
            wri.PushInt(0x41624F0D);
            wri.PushIdentity(50000, character.ID);
            wri.PushByte(0);
            wri.Push3F1Count(5); // Textures are always transferred
            int c;
            int c2;

            bool socialonly;
            bool showsocial;
            bool showhelmet;
            bool LeftPadVisible;
            bool RightPadVisible;
            bool DoubleLeftPad;
            bool DoubleRightPad;

            int TexturesCount;
            int HairMeshValue;
            int WeaponMeshRightValue;
            int WeaponMeshLeftValue;

            uint HeadMeshBaseValue;
            int HeadMeshValue;

            int BackMeshValue;
            int ShoulderMeshRightValue;
            //int ShoulderMeshRightValue;

            int VisualFlags;
            int PlayField;

            int OverrideTextureHead;
            int OverrideTextureWeaponRight;
            int OverrideTextureWeaponLeft;
            int OverrideTextureShoulderpadRight;
            int OverrideTextureShoulderpadLeft;
            int OverrideTextureBack;
            int OverrideTextureAttractor;

            List<AOTextures> Textures = new List<AOTextures>();

            Dictionary<int, int> SocialTab = new Dictionary<int, int>();

            lock (character)
            {
                VisualFlags = character.Stats.VisualFlags.Value;

                socialonly = ((character.Stats.VisualFlags.Value & 0x40) > 0);
                showsocial = ((character.Stats.VisualFlags.Value & 0x20) > 0);
                showhelmet = ((character.Stats.VisualFlags.Value & 0x4) > 0);
                LeftPadVisible = ((character.Stats.VisualFlags.Value & 0x1) > 0);
                RightPadVisible = ((character.Stats.VisualFlags.Value & 0x2) > 0);
                DoubleLeftPad = ((character.Stats.VisualFlags.Value & 0x8) > 0);
                DoubleRightPad = ((character.Stats.VisualFlags.Value & 0x10) > 0);

                HairMeshValue = character.Stats.HairMesh.Value;
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

                PlayField = character.PlayField;

                foreach (int num in character.SocialTab.Keys)
                {
                    SocialTab.Add(num, character.SocialTab[num]);
                }

                foreach (AOTextures at in character.Textures)
                {
                    Textures.Add(new AOTextures(at.place, at.Texture));
                }

                mh = MeshLayers.GetMeshs(character, showsocial, socialonly);
            }

            AOTextures aotemp = new AOTextures(0, 0);
            for (c = 0; c < 5; c++)
            {
                aotemp.Texture = 0;
                aotemp.place = c;
                for (c2 = 0; c2 < Textures.Count; c2++)
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

                wri.PushInt(aotemp.place);
                wri.PushInt(aotemp.Texture);
                wri.PushInt(0);
            }

            c = mh.Count;
            wri.Push3F1Count(c);
            foreach (AOMeshs m2 in mh)
            {
                wri.PushByte((byte) m2.Position);
                wri.PushUInt(m2.Mesh);
                wri.PushInt(m2.OverrideTexture); // Override Texture!!!!!!
                wri.PushByte((byte) m2.Layer);
            }

            wri.PushShort((short) (VisualFlags)); // 673 = VisualFlags
            wri.PushByte(0);

            byte[] reply = wri.Finish();
            Announce.Playfield(PlayField, ref reply);
        }
    }
}