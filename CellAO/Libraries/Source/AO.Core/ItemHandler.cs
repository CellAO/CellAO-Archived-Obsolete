#region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ComponentAce.Compression.Libs.zlib;
using MsgPack.Serialization;
#endregion

namespace AO.Core
{
    /// <summary>
    /// Item handler class
    /// </summary>
    public class ItemHandler
    {
        private SqlWrapper mySql = new SqlWrapper();

        /// <summary>
        /// Cache of all item templates
        /// </summary>
        public static List<AOItem> ItemList = new List<AOItem>();

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllItems()
        {
            DateTime _now = DateTime.Now;
            ItemList = new List<AOItem>();
            Stream sf = new FileStream("items.dat", FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            BinaryReader br = new BinaryReader(ms);
            var bf2 = MessagePackSerializer.Create<List<AOItem>>();

            while (true)
            {
                List<AOItem> templist = bf2.Unpack(ms);
                ItemList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} items in {1}\r",
                              new object[]
                                  {ItemList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff")});
            }
            GC.Collect();
            return ItemList.Count;
        }

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllItems(string fname)
        {
            DateTime _now = DateTime.Now;
            ItemList = new List<AOItem>();
            Stream sf = new FileStream(fname, FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);
            var bf2 = MessagePackSerializer.Create<List<AOItem>>();


            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            while (true)
            {
                List<AOItem> templist = bf2.Unpack(ms);
                ItemList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} items in {1}\r",
                              new object[]
                                  {ItemList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff")});
            }
            GC.Collect();
            return ItemList.Count;
        }


        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2097152];
            int len;
            while ((len = input.Read(buffer, 0, 2097152)) > 0)
            {
                output.Write(buffer, 0, len);
                Console.Write("\rDeflating " + Convert.ToInt32(Math.Floor((double) input.Position/input.Length*100.0)) +
                              "%");
            }
            output.Flush();
            Console.Write("\r                                             \r");
        }

        #region Item
        /// <summary>
        /// Get new object of item template with specified ID
        /// </summary>
        /// <param name="ID">AOID</param>
        /// <returns>copied AOItem</returns>
        public static AOItem GetItemTemplate(int ID)
        {
            foreach (AOItem it in ItemList)
            {
                if (it.LowID == ID)
                {
                    return it.ShallowCopy();
                }
            }
            return null; // Should not ever happen
        }

        /// <summary>
        /// Returns a interpolated version of the items
        /// </summary>
        /// <param name="lowID">low ID</param>
        /// <param name="highID">high ID</param>
        /// <param name="_QL">Quality level</param>
        /// <returns>interpolated AOItem</returns>
        public static AOItem interpolate(int lowID, int highID, int _QL)
        {
            AOItem low = GetItemTemplate(lowID);
            AOItem high = GetItemTemplate(highID);
            AOItem interp;
            if (_QL < low.Quality)
            {
                _QL = low.Quality;
            }

            if (_QL > high.Quality)
            {
                _QL = high.Quality;
            }

            interp = high.ShallowCopy();
            interp.LowID = low.LowID;
            if (_QL < high.Quality)
            {
                interp = low.ShallowCopy();
            }
            interp.HighID = high.HighID;
            interp.Quality = _QL;
            if ((_QL == low.Quality) || (_QL == high.Quality))
            {
                return interp;
            }
            int attnum = 0;

            // Effecting all attributes, even flags, it doesnt matter, High and low have always the same
            Single ival;
            Single factor = ((_QL - low.Quality)/(Single) (high.Quality - low.Quality));
            while (attnum < low.Stats.Count)
            {
                ival = (factor*(high.Stats[attnum].Value - low.Stats[attnum].Value)) + low.Stats[attnum].Value;
                interp.Stats[attnum].Value = Convert.ToInt32(ival); // Had to go int64 cos of the flags
                attnum++;
            }

            // TODO Requirements need interpolation too
            int evnum = 0;
            int fnum;
            int anum;
            Single fval;
            while (evnum < interp.Events.Count)
            {
                fnum = 0;
                while (fnum < interp.Events[evnum].Functions.Count)
                {
                    anum = 0;
                    while (anum < interp.Events[evnum].Functions[fnum].Arguments.Values.Count)
                    {
                        if (high.Events[evnum].Functions[fnum].Arguments.Values[anum] is int)
                        {
                            ival = (factor*
                                    ((int)high.Events[evnum].Functions[fnum].Arguments.Values[anum]-
                                     (int)low.Events[evnum].Functions[fnum].Arguments.Values[anum])) +
                                   (int)low.Events[evnum].Functions[fnum].Arguments.Values[anum];
                            interp.Events[evnum].Functions[fnum].Arguments.Values[anum] = Convert.ToInt32(ival);
                        }
                        if (high.Events[evnum].Functions[fnum].Arguments.Values[anum] is Single)
                        {
                            fval = (factor*
                                    ((Single)high.Events[evnum].Functions[fnum].Arguments.Values[anum]-
                                     (Single)low.Events[evnum].Functions[fnum].Arguments.Values[anum])) +
                                   (Single)low.Events[evnum].Functions[fnum].Arguments.Values[anum];
                            interp.Events[evnum].Functions[fnum].Arguments.Values[anum] = fval;
                        }
                        anum++;
                    }
                    fnum++;
                }
                evnum++;
            }
            return interp;
        }
        #endregion

        #region Function handling
        /// <summary>
        /// Function Pack will be moved soon
        /// </summary>
        public class FunctionPack
        {
            #region Function handling (execute function arguments)
            /*
            public static bool func_do(Character ch, AOFunctions func, bool dolocalstats, bool tosocialtab, int placement)
            {
                return func_do(ch, func, dolocalstats, tosocialtab, placement, true);
            }

            public static bool func_do(Character ch, AOFunctions func, bool dolocalstats, bool tosocialtab, int placement, bool doreqs)
            {
                int c;
                int r;
                Character chartarget = (Character)Misc.FindDynel.FindDynelByID(ch.Target.Type, ch.Target.Instance);
                Boolean reqs_met;
                Character ftarget = null;
                int statval;
                Boolean reqresult;
                if (ch != null)
                {
                    for (c = 0; c < func.TickCount; c++)
                    {
                        reqs_met = true;
                        int childop = -1;
                        if (!doreqs)
                        {
                            for (r = 0; r < func.Requirements.Count; r++)
                            {
                                switch (func.Requirements[r].Target)
                                {
                                    case itemtarget_user:
                                        ftarget = ch;
                                        break;
                                    case itemtarget_wearer:
                                        ftarget = ch;
                                        break;
                                    case itemtarget_target:
                                        ftarget = chartarget;
                                        break;
                                    case itemtarget_fightingtarget:
                                        // Fighting target
                                        break;
                                    case itemtarget_self:
                                        ftarget = ch;
                                        break;
                                    case itemtarget_selectedtarget:
                                        ftarget = chartarget;
                                        break;
                                }
                                if (ftarget == null)
                                {
                                    reqs_met = false;
                                    return false;
                                }
                                statval = ftarget.Stats.Get(func.Requirements[r].Statnumber);
                                switch (func.Requirements[r].Operator)
                                {
                                    case operator_and:
                                        reqresult = ((statval & func.Requirements[r].Value) != 0);
                                        break;
                                    case operator_or:
                                        reqresult = ((statval | func.Requirements[r].Value) != 0);
                                        break;
                                    case operator_equalto:
                                        reqresult = (statval == func.Requirements[r].Value);
                                        break;
                                    case operator_lessthan:
                                        reqresult = (statval < func.Requirements[r].Value);
                                        break;
                                    case operator_greaterthan:
                                        reqresult = (statval > func.Requirements[r].Value);
                                        break;
                                    case operator_unequal:
                                        reqresult = (statval != func.Requirements[r].Value);
                                        break;
                                    case operator_true:
                                        reqresult = (statval != 0);
                                        break;
                                    case operator_false:
                                        reqresult = (statval == 0);
                                        break;
                                    case operator_bitand:
                                        reqresult = ((statval & func.Requirements[r].Value) != 0);
                                        break;
                                    case operator_bitor:
                                        reqresult = ((statval | func.Requirements[r].Value) != 0);
                                        break;
                                    default:
                                        reqresult = true;
                                        break;
                                }

                                switch (childop)
                                {
                                    case operator_and:
                                        reqs_met &= reqresult;
                                        break;
                                    case operator_or:
                                        reqs_met |= reqresult;
                                        break;
                                    case -1:
                                        reqs_met = reqresult;
                                        break;
                                    default:
                                        break;
                                }
                                childop = func.Requirements[r].ChildOperator;
                            }
                        }

                        if (!reqs_met)
                        {
                            return reqs_met;
                        }

                        switch (func.FunctionType)
                        {
                            // Set new Texture
                            case ItemHandler.functiontype_texture:
                                SqlWrapper ms = new SqlWrapper();
                                if (!tosocialtab)
                                {
                                    ms.SqlUpdate("Update " + ch.getSQLTablefromDynelType() + " set Textures" + func.Arguments[1].ToString() + "=" + func.Arguments[0].ToString() + " WHERE ID=" + ch.ID.ToString());
                                    AOTextures ao = new AOTextures((int)func.Arguments[1], (int)func.Arguments[0]);
                                    ch.Textures.Add(ao);
                                }
                                else
                                {
                                    int texnum = Int32.Parse(func.Arguments[1].ToString());
                                    int texval = Int32.Parse(func.Arguments[0].ToString());
                                    if (ch.SocialTab.ContainsKey(texnum))
                                    {
                                        ch.SocialTab[texnum] = texval;
                                    }
                                    else
                                    {
                                        ch.SocialTab.Add(texnum, texval);
                                    }
                                    ch.SaveSocialTab();
                                }

                                break;
                            // Set Headmesh
                            case ItemHandler.functiontype_headmesh:
                                if (!tosocialtab)
                                {
                                    ch.Stats.HeadMesh.StatModifier = (Int32)((Int32)func.Arguments[1] - ch.Stats.HeadMesh.StatBaseValue); // Headmesh
                                    ch.MeshLayer.AddMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], 0);
                                }
                                else
                                {
                                    if (ch.SocialTab.ContainsKey(ch.Stats.HeadMesh.StatNumber))
                                    {
                                        ch.SocialTab[ch.Stats.HeadMesh.StatNumber] = (Int32)func.Arguments[0];
                                        ch.SocialMeshLayer.AddMesh(0, (Int32)func.Arguments[0], (Int32)func.Arguments[1], 0);
                                    }
                                    else
                                    {
                                        ch.SocialTab.Add(ch.Stats.HeadMesh.StatNumber, (Int32)func.Arguments[0]);
                                        ch.SocialMeshLayer.AddMesh(0, (Int32)func.Arguments[0], (Int32)func.Arguments[1], 0);
                                    }
                                    ch.SaveSocialTab();
                                }
                                break;
                            // Set Shouldermesh
                            case ItemHandler.functiontype_shouldermesh:
                                if ((placement == 19) || (placement == 51))
                                {
                                    if (!tosocialtab)
                                    {
                                        ch.Stats.ShoulderMeshRight.Set((Int32)func.Arguments[1]);
                                        ch.Stats.ShoulderMeshLeft.Set((Int32)func.Arguments[1]);
                                        ch.MeshLayer.AddMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        ch.MeshLayer.AddMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                    else
                                    {
                                        ch.SocialMeshLayer.AddMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        ch.SocialMeshLayer.AddMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                }
                                else
                                {
                                    if (!tosocialtab)
                                    {
                                        if (placement == 20)
                                        {
                                            ch.Stats.ShoulderMeshLeft.Set((Int32)func.Arguments[1]);
                                            ch.MeshLayer.AddMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        }
                                        if (placement == 22)
                                        {
                                            ch.Stats.ShoulderMeshLeft.Set((Int32)func.Arguments[1]);
                                            ch.MeshLayer.AddMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        }
                                    }
                                    else
                                    {
                                        if (placement == 52)
                                        {
                                            ch.Stats.ShoulderMeshRight.Set((Int32)func.Arguments[1]);
                                            ch.SocialMeshLayer.AddMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        }
                                        if (placement == 54)
                                        {
                                            ch.Stats.ShoulderMeshLeft.Set((Int32)func.Arguments[1]);
                                            ch.SocialMeshLayer.AddMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        }
                                    }


                                }
                                break;
                            // Set Backmesh
                            case ItemHandler.functiontype_backmesh:
                                if (!tosocialtab)
                                {
                                    ch.Stats.BackMesh.Set((Int32)func.Arguments[0]); // Shouldermesh
                                    ch.MeshLayer.AddMesh(5, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    ch.SocialMeshLayer.AddMesh(5, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                break;
                            // Set Hairmesh
                            case ItemHandler.functiontype_hairmesh:
                                if (!tosocialtab)
                                {
                                    ch.Stats.HairMesh.Set((Int32)func.Arguments[0]); // HairMesh
                                    ch.MeshLayer.AddMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    ch.SocialMeshLayer.AddMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                break;
                            case ItemHandler.functiontype_attractormesh:
                                if (!tosocialtab)
                                {
                                    ch.Stats.HairMesh.Set((Int32)func.Arguments[0]); // HairMesh
                                    ch.MeshLayer.AddMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    ch.SocialMeshLayer.AddMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                break;
                            case ItemHandler.functiontype_modify:
                                // TODO: req check for OE
                                if (dolocalstats)
                                {
                                    if (!tosocialtab)
                                    {
                                        ch.Stats.SetModifier((Int32)func.Arguments[0], ch.Stats.GetModifier((Int32)func.Arguments[0]) + (Int32)func.Arguments[1]);
                                    }
                                }
                                break;
                            case ItemHandler.functiontype_modifypercentage:
                                // TODO: req check for OE
                                if (dolocalstats)
                                {
                                    if (!tosocialtab)
                                    {
                                        ch.Stats.SetPercentageModifier((Int32)func.Arguments[0], ch.Stats.GetPercentageModifier((Int32)func.Arguments[0]) + (Int32)func.Arguments[1]);
                                    }
                                }
                                break;
                            case ItemHandler.functiontype_uploadnano:
                                ch.UploadNano((Int32)func.Arguments[0]);
                                Packets.UploadNanoupdate.Send(ch, 53019, (Int32)func.Arguments[0]);
                                break;
                            case ItemHandler.functiontype_shophash:
                                // Do nothing, it's covered in 
                                break;
                            default:
                                break;
                        }
                    }
                }
                return false;
            }*/
            #endregion

            /*
            #region Function Handling (reverting to original state)
            public static bool func_revert(Character ch, AOFunctions func, bool fromsocialtab, int placement)
            {
                int c;

                if (ch != null)
                {
                    for (c = 0; c < func.TickCount; c++)
                    {
                        switch (func.FunctionType)
                        {
                            case ItemHandler.functiontype_texture:
                                // Todo: check for second Arm item
                                SqlWrapper ms = new SqlWrapper();
                                if (!fromsocialtab)
                                {
                                    ms.SqlUpdate("Update " + ch.getSQLTablefromDynelType() + " set Textures" + func.Arguments[1].ToString() + "=0 WHERE ID=" + ch.ID.ToString() + " AND Textures" + func.Arguments[1].ToString() + "=" + func.Arguments[0].ToString());
                                    int ct = ch.Textures.Count - 1;
                                    while (ct >= 0)
                                    {
                                        if (ch.Textures[ct].place == (int)func.Arguments[1])
                                        {
                                            ch.Textures.RemoveAt(ct);
                                            break;
                                        }
                                        ct--;
                                    }
                                }
                                else
                                {
                                    if (ch.SocialTab.ContainsKey((Int32)func.Arguments[1]))
                                    {
                                        ch.SocialTab[(Int32)func.Arguments[1]] = 0;
                                    }
                                    else
                                    {
                                        ch.SocialTab.Add((Int32)func.Arguments[1], 0);
                                    }
                                }
                                break;
                            case ItemHandler.functiontype_headmesh:
                                if (!fromsocialtab)
                                {
                                    ch.Stats.HeadMesh.StatModifier = 0;
                                    ch.MeshLayer.RemoveMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    ch.SocialMeshLayer.RemoveMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                // Reverting the Head modification
                                break;
                            case ItemHandler.functiontype_shouldermesh:
                                // TODO: check for second shoulder item
                                if (!fromsocialtab)
                                {
                                    if (placement == 19)
                                    {
                                        ch.MeshLayer.RemoveMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                        ch.MeshLayer.RemoveMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                    if (placement == 20) // Right
                                    {
                                        ch.Stats.ShoulderMeshRight.Set(0); // Shouldermesh Right
                                        ch.MeshLayer.RemoveMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                    if (placement == 22) // Left
                                    {
                                        ch.Stats.ShoulderMeshLeft.Set(0); // Shouldermesh Left
                                        ch.MeshLayer.RemoveMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                }
                                else
                                {
                                    if (placement == 52) // Right
                                    {
                                        ch.SocialMeshLayer.RemoveMesh(3, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                    if (placement == 54) // Left
                                    {
                                        ch.SocialMeshLayer.RemoveMesh(4, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                    }
                                }
                                break;
                            case ItemHandler.functiontype_backmesh:
                                if (!fromsocialtab)
                                {
                                    ch.Stats.BackMesh.Set(0); // Backmesh
                                    ch.MeshLayer.RemoveMesh(5, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    ch.SocialMeshLayer.RemoveMesh(5, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                break;
                            case ItemHandler.functiontype_attractormesh:
                                if (!fromsocialtab)
                                {
                                    ch.Stats.HairMesh.Set(0); // Attractormesh
                                    ch.MeshLayer.RemoveMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                else
                                {
                                    if (ch.SocialTab.ContainsKey(32))
                                    {
                                        ch.SocialTab[32] = 0;
                                    }
                                    else
                                    {
                                        ch.SocialTab.Add(32, 0);
                                    }
                                    ch.SocialMeshLayer.RemoveMesh(0, (Int32)func.Arguments[1], (Int32)func.Arguments[0], Misc.MeshLayers.GetLayer(placement));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                return false;
            }
            #endregion
             */
        }
        #endregion
    }
}