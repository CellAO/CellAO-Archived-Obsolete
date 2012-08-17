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
using System.Runtime.Serialization;

namespace AO.Core
{
    /// <summary>
    /// AOActions covers all action types, with their reqs
    /// </summary>
    public class AOActions
    {
        /// <summary>
        /// Type of Action (constants in ItemHandler)
        /// </summary>
        public int ActionType;

        /// <summary>
        /// List of Requirements for this action
        /// </summary>
        public List<AORequirements> Requirements = new List<AORequirements>();

        /// <summary>
        /// Empty
        /// </summary>
        public AOActions()
        {
        }

        /// <summary>
        /// Deserialize AOEvent, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AOActions(SerializationInfo info, StreamingContext context)
        {
            ActionType = (int) info.GetValue("ActionType", typeof (int));
            Requirements = (List<AORequirements>) info.GetValue("Requirements", typeof (List<AORequirements>));
        }

        /// <summary>
        /// Serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ActionType", ActionType);
            info.AddValue("Requirements", Requirements);
        }


        /// Methods to do:
        /// Read Action
        /// 

        #region Action read from blob
        public int readActionfromBlob(byte[] blob, int offset)
        {
            int c = offset;

            ActionType = BitConverter.ToInt32(blob, c);
            c += 4;

            int c2 = BitConverter.ToInt32(blob, c);
            c += 4;

            AORequirements m_aor;

            while (c2 > 0)
            {
                m_aor = new AORequirements();
                c = m_aor.readRequirementfromBlob(blob, c);
                c2--;
            }
            return c;
        }
        #endregion
    }
}