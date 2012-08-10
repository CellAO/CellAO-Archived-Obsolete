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
using System.Linq;
using AO.Core;
#endregion

namespace ZoneEngine.Misc
{
    public class MeshLayers
    {
        public SortedList<int, int> Mesh = new SortedList<int, int>();
        public SortedList<int, int> Override = new SortedList<int, int>();

        public int Count()
        {
            return Mesh.Count;
        }

        public int GetMesh(int number)
        {
            return Mesh.ElementAt(number).Value;
        }

        public int GetOverride(int number)
        {
            return Override.ElementAt(number).Value;
        }

        public int GetKey(int number)
        {
            return Mesh.ElementAt(number).Key;
        }

        public void AddMesh(int position, int mesh, int overridetexture, int layer)
        {
            int key = (position << 16) + layer;
            if (Mesh.ContainsKey(key))
            {
                Mesh[key] = mesh;
            }
            else
            {
                Mesh.Add(key, mesh);
            }
            if (Override.ContainsKey(key))
            {
                Override[key] = overridetexture;
            }
            else
            {
                Override.Add(key, overridetexture);
            }
        }

        public void RemoveMesh(int position, int mesh, int overridetexture, int layer)
        {
            int key = (position << 16) + layer;
            try
            {
                Mesh.Remove(key);
                Override.Remove(key);
            }
            catch
            {
            }
        }

        public List<AOMeshs> GetMeshs()
        {
            List<AOMeshs> am = new List<AOMeshs>();
            int c = 0;
            for (c = 0; c < Mesh.Count; c++)
            {
                AOMeshs _am = new AOMeshs();
                _am.Position = Mesh.ElementAt(c).Key >> 16;
                _am.Mesh = Mesh.ElementAt(c).Value;
                _am.OverrideTexture = Override.ElementAt(c).Value;
                _am.Layer = Mesh.ElementAt(c).Key & 0xffff;
                am.Add(_am);
            }
            return am;
        }

        public AOMeshs GetMeshAtPosition(int pos)
        {
            foreach (int key in Mesh.Keys)
            {
                if ((key >> 16) == pos)
                {
                    // Just return mesh with highest priority (0 = highest)
                    AOMeshs ma = new AOMeshs();
                    ma.Layer = key & 0xffff;
                    ma.Position = key >> 16;
                    ma.Mesh = Mesh[key];
                    ma.OverrideTexture = Override[key];
                    return ma;
                }
            }
            // No mesh at this position found
            return null;
        }


        public static List<AOMeshs> GetMeshs(Character character, bool showsocial, bool socialonly)
        {
            List<AOMeshs> _meshs;
            List<AOMeshs> _socials;
            List<AOMeshs> output = new List<AOMeshs>();


            bool LeftPadVisible;
            bool RightPadVisible;
            bool DoubleLeftPad;
            bool DoubleRightPad;
            bool ShowHelmet;

            lock (character)
            {
                _meshs = character.MeshLayer.GetMeshs();
                _socials = character.SocialMeshLayer.GetMeshs();

                int VisualFlags = character.Stats.VisualFlags.Value;
                RightPadVisible = ((VisualFlags & 0x1) > 0);
                LeftPadVisible = ((VisualFlags & 0x2) > 0);
                ShowHelmet = ((VisualFlags & 0x4) > 0);
                DoubleLeftPad = ((VisualFlags & 0x8) > 0);
                DoubleRightPad = ((VisualFlags & 0x10) > 0);

                if (!ShowHelmet)
                {
                    if (_meshs.ElementAt(0).Position == 0) // Helmet there?
                        // This probably needs to be looked at (glasses/visors)
                    {
                        if (_meshs.ElementAt(0).Mesh != character.Stats.HeadMesh.StatBaseValue)
                            // Dont remove the head :)
                        {
                            _meshs.RemoveAt(0);
                        }
                    }

                    if (_socials.ElementAt(0).Position == 0) // Helmet there?
                        // This probably needs to be looked at (glasses/visors)
                    {
                        if (_socials.ElementAt(0).Mesh != character.Stats.HeadMesh.StatBaseValue)
                            // Dont remove the head :)
                        {
                            _socials.RemoveAt(0);
                        }
                    }
                }
            }

            socialonly &= showsocial; // Disable socialonly flag if showsocial is false

            // preapply visual flags

            #region Applying visual flags
            if (socialonly)
            {
                _meshs.Clear();
            }
            if ((!socialonly) && (!showsocial))
            {
                _socials.Clear();
            }

            AOMeshs leftshoulder = null;
            AOMeshs rightshoulder = null;
            int rightshouldernum = -1;
            int leftshouldernum = -1;


            for (int c1 = 0; c1 < _meshs.Count; c1++)
            {
                AOMeshs m = _meshs.ElementAt(c1);
                if (m.Position == 3)
                {
                    if (!RightPadVisible)
                    {
                        _meshs.RemoveAt(c1);
                        c1--;
                        continue;
                    }
                    else
                    {
                        rightshoulder = m;
                        rightshouldernum = c1;
                    }
                }
                if (m.Position == 4)
                {
                    if (!LeftPadVisible)
                    {
                        _meshs.RemoveAt(c1);
                        c1--;
                        continue;
                    }
                    else
                    {
                        leftshouldernum = c1;
                        leftshoulder = m;
                    }
                }
            }
            #endregion

            AOMeshs cloth;
            AOMeshs social;

            int c = 0;
            for (c = 0; c < 7; c++)
            {
                cloth = null;
                social = null;

                foreach (AOMeshs m in _meshs)
                {
                    if (m.Position == c)
                    {
                        cloth = m;
                        break;
                    }
                }

                foreach (AOMeshs m in _socials)
                {
                    if (m.Position == c)
                    {
                        social = m;
                    }
                }

                if (social != null)
                {
                    if ((cloth != null) && (social != null))
                    {
                        // Compare layer only when both slots are set
                        if (cloth.Position == 0)
                        {
                            if (social.Mesh != character.Stats.HeadMesh.StatBaseValue)
                            {
                                cloth = social;
                            }
                        }
                        else if (social.Layer - 8 < cloth.Layer)
                        {
                            if (cloth.Position == 5) // Backmeshs act different
                            {
                                output.Add(cloth);
                            }
                            cloth = social;
                        }
                    }
                    else
                    {
                        if ((showsocial) || (socialonly))
                        {
                            cloth = social;
                        }
                    }
                }
                if (cloth != null)
                {
                    output.Add(cloth);

                    // Moved check for Double pads here
                    if ((cloth.Position == 3) && DoubleRightPad)
                    {
                        AOMeshs temp = new AOMeshs();
                        temp.Position = 4;
                        temp.Layer = cloth.Layer;
                        temp.Mesh = cloth.Mesh;
                        temp.OverrideTexture = cloth.OverrideTexture;
                        output.Add(temp);
                    }

                    if ((cloth.Position == 4) && DoubleLeftPad)
                    {
                        AOMeshs temp = new AOMeshs();
                        temp.Position = 3;
                        temp.Layer = cloth.Layer;
                        temp.Mesh = cloth.Mesh;
                        temp.OverrideTexture = cloth.OverrideTexture;
                        output.Add(temp);
                    }
                }
            }
            return output;
        }

        #region GetLayer
        public static int GetLayer(int placement)
        {
            switch (placement)
            {
                    // Equipment pages
                case 18: // Head
                    return 0;
                case 19: // Back
                    return 4;
                case 20: // Shoulders
                case 22:
                    return 4;
                case 6: // Hands (Weapons)
                case 8:
                    return 4;

                    // Social page
                case 50: // Head
                    return 0;
                case 51: // Back
                    return 4;
                case 52: // Shoulders
                case 54:
                    return 4;
                case 56: // Hands (Weapons)
                case 58:
                    return 4;

                default:
                    return 0;
            }
        }
        #endregion
    }
}