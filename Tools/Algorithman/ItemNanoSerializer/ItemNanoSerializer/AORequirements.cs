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
using System.Runtime.Serialization;

namespace ZoneEngine
{
    [Serializable()]
    public class AORequirements : ISerializable
    {
        public int Target;
        public int Statnumber;
        public int Operator;
        public int Value;
        public int ChildOperator;

        /// Methods to do:
        /// Read Requirement
        /// Check Requirement

        public AORequirements(SerializationInfo info, StreamingContext context)
        {
            Target = (int)info.GetValue("Target", typeof(int));
            Statnumber= (int)info.GetValue("Statnumber", typeof(int));
            Operator = (int)info.GetValue("Operator", typeof(int));
            Value= (int)info.GetValue("Value", typeof(int));
            ChildOperator = (int)info.GetValue("ChildOperator", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Target", Target);
            info.AddValue("Statnumber", Statnumber);
            info.AddValue("Operator", Operator);
            info.AddValue("Value", Value);
            info.AddValue("ChildOperator", ChildOperator);
        }

        public AORequirements()
        {
        }


        #region Requirement to blob (hex string)
        public string ToBlob()
        {
            string output = "";
            output += Target.ToString("X8");
            output += Statnumber.ToString("X8");
            output += Operator.ToString("X8");
            output += Value.ToString("X8");
            output += ChildOperator.ToString("X8");
            return output;
        }
        #endregion

        #region Requirement from blob (byte[] with offset)
        public int readRequirementfromBlob(ref byte[] _p, int offset)
        {
            int c = offset;
            Target = BitConverter.ToInt32(_p, c);
            c += 4;

            Statnumber= BitConverter.ToInt32(_p, c);
            c += 4;

            Operator = BitConverter.ToInt32(_p, c);
            c += 4;

            Value = BitConverter.ToInt32(_p, c);
            c += 4;

            ChildOperator = BitConverter.ToInt32(_p, c);
            c += 4;

            return c;
        }
        #endregion
    }
}
