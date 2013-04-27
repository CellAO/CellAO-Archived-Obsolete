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
using System.Runtime.Serialization.Formatters.Binary;

namespace AO.Core
{
    using MsgPack;
    using MsgPack.Serialization;

    /// <summary>
    /// AOFunctions
    /// </summary>
    [Serializable]
    public class AOFunctions
    {
        /// <summary>
        /// Type of function (constants in ItemHandler)
        /// </summary>
        public int FunctionType;

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
        /// List of Arguments
        /// </summary>
        public AOFunctionArguments Arguments = new AOFunctionArguments();

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
            foreach (AORequirements aor in this.Requirements)
            {
                AORequirements newAOR = new AORequirements();
                newAOR.ChildOperator = aor.ChildOperator;
                newAOR.Operator = aor.Operator;
                newAOR.Statnumber = aor.Statnumber;
                newAOR.Target = aor.Target;
                newAOR.Value = aor.Value;
                newAOF.Requirements.Add(newAOR);
            }

            foreach (object ob in Arguments.Values)
            {
                if (ob.GetType() == typeof(string))
                {
                    string z = (string)ob;
                    newAOF.Arguments.Values.Add(z);
                }
                if (ob.GetType() == typeof(int))
                {
                    int i = (int)ob;
                    newAOF.Arguments.Values.Add(i);
                }
                if (ob.GetType() == typeof(Single))
                {
                    Single s = (Single)ob;
                    newAOF.Arguments.Values.Add(s);
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
        /// Empty
        /// </summary>
        public AOFunctions()
        {
        }

        public string Serialize()
        {
            var toByte = MessagePackSerializer.Create<AOFunctions>();
            var ms = new MemoryStream();
            toByte.Pack(ms, this);

            ms.Position = 0;
            return BitConverter.ToString(ms.ToArray()).Replace("-",string.Empty);
        }

        public static AOFunctions Deserialize(MemoryStream ms)
        {
            var fromByte = MessagePackSerializer.Create<AOFunctions>();
            return fromByte.Unpack(ms);
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