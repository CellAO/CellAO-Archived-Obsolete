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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AO.Core
{
    /// <summary>
    /// AOFunctions
    /// </summary>
    [Serializable]
    public class AOFunctions : ISerializable
    {
        /// <summary>
        /// Type of function (constants in ItemHandler)
        /// </summary>
        public int FunctionType;

        /// <summary>
        /// List of Arguments
        /// </summary>
        public List<object> Arguments = new List<object>();

        /// <summary>
        /// TickCount (for timers)
        /// </summary>
        public int TickCount;

        /// <summary>
        /// TickInterval (for timers)
        /// </summary>
        public uint TickInterval;

        /// <summary>
        /// TargetType (constants in ItemHandler)
        /// </summary>
        public int Target;

        /// <summary>
        /// Requirements to execute this function
        /// </summary>
        public List<AORequirements> Requirements = new List<AORequirements>();

        /// <summary>
        /// process local stats (not serialized)
        /// </summary>
        public bool dolocalstats = true;

        /// <summary>
        /// Copy Function
        /// </summary>
        /// <returns>new copy</returns>
        public AOFunctions ShallowCopy()
        {
            AOFunctions newAOF = new AOFunctions();
            foreach (AORequirements aor in Requirements)
            {
                AORequirements newAOR = new AORequirements();
                newAOR.ChildOperator = aor.ChildOperator;
                newAOR.Operator = aor.Operator;
                newAOR.Statnumber = aor.Statnumber;
                newAOR.Target = aor.Target;
                newAOR.Value = aor.Value;
                newAOF.Requirements.Add(newAOR);
            }

            foreach (object ob in Arguments)
            {
                if (ob.GetType() == typeof (string))
                {
                    string z = (string) ob;
                    newAOF.Arguments.Add(z);
                }
                if (ob.GetType() == typeof (int))
                {
                    int i = (int) ob;
                    newAOF.Arguments.Add(i);
                }
                if (ob.GetType() == typeof (Single))
                {
                    Single s = (Single) ob;
                    newAOF.Arguments.Add(s);
                }
            }
            newAOF.dolocalstats = dolocalstats;
            newAOF.FunctionType = FunctionType;
            newAOF.Target = Target;
            newAOF.TickCount = TickCount;
            newAOF.TickInterval = TickInterval;

            return newAOF;
        }

        /// <summary>
        /// Deserialize AOFunction, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AOFunctions(SerializationInfo info, StreamingContext context)
        {
            FunctionType = (int) info.GetValue("FunctionType", typeof (int));
            Arguments = (List<object>) info.GetValue("Arguments", typeof (List<object>));
            TickCount = (int) info.GetValue("TickCount", typeof (int));
            TickInterval = (uint) info.GetValue("TickInterval", typeof (uint));
            Target = (int) info.GetValue("Target", typeof (int));
            Requirements = (List<AORequirements>) info.GetValue("Requirements", typeof (List<AORequirements>));
        }

        /// <summary>
        /// Serialize AOFunction, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FunctionType", FunctionType);
            info.AddValue("Arguments", Arguments);
            info.AddValue("TickCount", TickCount);
            info.AddValue("TickInterval", TickInterval);
            info.AddValue("Target", Target);
            info.AddValue("Requirements", Requirements);
        }

        /// <summary>
        /// Empty
        /// </summary>
        public AOFunctions()
        {
        }

        #region read Function from byte[]
        /// <summary>
        /// Old blob read, do not delete yet
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int ReadFunctionfromBlob(ref byte[] blob, int offset)
        {
            int c = offset;
            FunctionType = BitConverter.ToInt32(blob, c);
            c += 4;
            Target = BitConverter.ToInt32(blob, c);
            c += 4;
            TickCount = BitConverter.ToInt32(blob, c);
            c += 4;
            TickInterval = BitConverter.ToUInt32(blob, c);
            c += 4;

            // Function Arguments
            int c2 = BitConverter.ToInt32(blob, c);
            int c4;
            string temps;
            byte tb;
            Single tempf;
            c += 4;
            while (c2 > 0)
            {
                tb = blob[c++];
                switch (tb)
                {
                    case 0x53:
                        temps = "";
                        c4 = BitConverter.ToInt32(blob, c);
                        c += 4;
                        while (c4 > 0)
                        {
                            temps += (char) blob[c++];
                            c4--;
                        }
                        Arguments.Add(new object());
                        Arguments[Arguments.Count - 1] = temps;
                        break;
                    case 0x49:
                        Arguments.Add(new object());
                        Arguments[Arguments.Count - 1] = BitConverter.ToInt32(blob, c);
                        c += 4;
                        break;
                    case 0x73:
                        tempf = BitConverter.ToSingle(blob, c);
                        Arguments.Add(new object());
                        Arguments[Arguments.Count - 1] = tempf;
                        c += 4;
                        break;
                }
                c2--;
            }
            c2 = BitConverter.ToInt32(blob, c);
            c += 4;
            AORequirements m_a;
            while (c2 > 0)
            {
                m_a = new AORequirements();
                c = m_a.readRequirementfromBlob(ref blob, c);
                Requirements.Add(m_a);
                c2--;
            }
            return c;
        }
        #endregion

        /// <summary>
        /// Serialize Function to hex-string
        /// </summary>
        /// <returns></returns>
        public string ToBlob()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream memstream = new MemoryStream();
            bin.Serialize(memstream, this);
            memstream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = memstream.ToArray();
            memstream.Close();
            string output = "";
            foreach (byte bb in buffer)
            {
                output = output + bb.ToString("X2");
            }
            return output;
        }

        #region Apply Function to target
        /// <summary>
        /// Apply function on target
        /// </summary>
        /// <param name="dolocalstats">Should local stats be processed?</param>
        public void Apply(bool dolocalstats)
        {
            /*
            // At this time Target has already the Dynel ID of the target (be it wearer, target, user or whatever)
            Client client = Misc.FindClient.FindClientByID(Target);
            if (client == null)
                // Client has vanished 
                return;
            Character character = client.Character;
            switch (FunctionType)
            {
                // Set Texture
                case ItemHandler.functiontype_texture:
                    SqlWrapper ms = new SqlWrapper();
                    ms.SqlUpdate("Update " + character.getSQLTablefromDynelType() + " set Textures" + Arguments[1].ToString() + "=" + Arguments[0].ToString() + " WHERE ID=" + character.ID.ToString());
                    break;
                // Set Headmesh
                case ItemHandler.functiontype_headmesh:
                    character.Stats.HeadMesh.Set((uint)Arguments[0]); // Headmesh
                    break;
                // Set Shoulder Mesh Right
                case ItemHandler.functiontype_shouldermesh:
                    character.Stats.ShoulderMeshRight.Set((uint)Arguments[0]); // Shouldermesh Right
                    //character.Stats.ShoulderMeshLeft.Set((uint)Arguments[0]); // Shouldermesh Left
                    break;
                // Set Backmesh
                case ItemHandler.functiontype_backmesh:
                    character.Stats.BackMesh.Set((uint)Arguments[0]); // Backmesh
                    break;
                // Set Hairmesh
                case ItemHandler.functiontype_hairmesh: // Never occurred to me so i suppose Attractormesh=Hairmesh
                    character.Stats.HairMesh.Set((uint)Arguments[0]); // Hairmesh
                    break;
                // Set AttractorMesh
                case ItemHandler.functiontype_attractormesh:
                    character.Stats.HairMesh.Set((uint)Arguments[0]); // Attractormesh
                    break;
                case ItemHandler.functiontype_modify:
                    // TODO: req check for OE
                    if (dolocalstats)
                    {
                        character.Stats.SetModifier((Int32)Arguments[0], character.Stats.GetModifier((Int32)Arguments[0]) + (Int32)Arguments[1]);
                        Console.WriteLine("LS " + Arguments[0].ToString() + " modified by " + Arguments[1].ToString());
                    }
                    break;
                case ItemHandler.functiontype_modifypercentage:
                    // TODO: req check for OE
                    if (dolocalstats)
                    {
                        character.Stats.SetPercentageModifier((Int32)Arguments[0], character.Stats.GetPercentageModifier((Int32)Arguments[0]) + (Int32)Arguments[1]);
                        Console.WriteLine("LS " + Arguments[0].ToString() + " modified by %" + Arguments[1].ToString());
                    }
                    break;
                case ItemHandler.functiontype_hit:
                    if (dolocalstats)
                    {
                        int randval = new Random().Next((Int32)Arguments[1], (Int32)Arguments[2]);
                        UInt32 val = (UInt32)(character.Stats.GetStatbyNumber((Int32)Arguments[0]).Value + randval);
                        character.Stats.Set((Int32)Arguments[0], val);
                    }
                    break;
                default:
                    break;

            }*/
        }
        #endregion
    }
}