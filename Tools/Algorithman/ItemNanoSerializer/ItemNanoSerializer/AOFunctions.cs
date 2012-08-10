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
using System.Linq;
using System.Text;
using AO.Core;
using System.Runtime.Serialization;

namespace ZoneEngine
{
    [Serializable()]
    public class AOFunctions : ISerializable
    {
        public int FunctionType;
        public List<object> Arguments = new List<object>();
        public int TickCount;
        public uint TickInterval;
        public int Target;
        public List<AORequirements> Requirements = new List<AORequirements>();

        public AOFunctions(SerializationInfo info, StreamingContext context)
        {
            FunctionType = (int)info.GetValue("FunctionType", typeof(int));
            Arguments = (List<object>)info.GetValue("Arguments", typeof(List<object>));
            TickCount = (int)info.GetValue("TickCount", typeof(int));
            TickInterval = (uint)info.GetValue("TickInterval", typeof(int));
            Target = (int)info.GetValue("Target", typeof(int));
            Requirements = (List<AORequirements>)info.GetValue("Arguments", typeof(List<AORequirements>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FunctionType", FunctionType);
            info.AddValue("Arguments", Arguments);
            info.AddValue("TickCount", TickCount);
            info.AddValue("TickInterval", TickInterval);
            info.AddValue("Target", Target);
            info.AddValue("Requirements", Requirements);
        }

        public AOFunctions()
        {
        }


        #region convert function to blob (hexstring)
        public string ToBlob()
        {
            string output = "";
            string temps;
            byte tempb;
            int tempi;
            Single tempsi;

            output += FunctionType.ToString("X8");
            output += TickCount.ToString("X8");
            output += TickInterval.ToString("X8");
            output += Target.ToString("X8");

            // Argument Count
            output += Arguments.Count.ToString("X8");
            int c;
            for (c = 0; c < Arguments.Count; c++)
            {
                if (Arguments[c] is string)
                {
                    output += "53";
                    temps = Arguments[c].ToString();
                    output += temps.Length.ToString("X8");
                    foreach (char ch in temps)
                    {
                        tempb = Convert.ToByte(ch);
                        output += tempb.ToString("X1");
                    }
                }
                if (Arguments[c] is int)
                {
                    output += "49";
                    tempi = Convert.ToInt32(Arguments[c]);
                    output += tempi.ToString("X8");
                }
                if (Arguments[c] is Single)
                {
                    output += "73";
                    tempsi = Convert.ToSingle(Arguments[c]);
                    output += tempsi.ToString("X8");
                }
            }

            // Convert Requirements to string
            output += Requirements.Count.ToString();
            for (c = 0; c < Requirements.Count; c++)
            {
                output += Requirements[c].ToBlob();
            }

            return output;
        }
        #endregion

        #region read Function from byte[]
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
                            temps += (char)blob[c++];
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

    }
}
