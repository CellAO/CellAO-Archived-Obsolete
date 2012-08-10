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
using System.Runtime.Serialization;

namespace AO.Core
{
    /// <summary>
    /// AOTimers
    /// </summary>
    [Serializable]
    public class AOTimers : ISerializable
    {
        /// <summary>
        /// Function triggered by the timer
        /// </summary>
        public AOFunctions Function = new AOFunctions();

        /// <summary>
        /// Timestamp of next trigger
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// Nano-Strain
        /// </summary>
        public Int32 Strain;

        /// <summary>
        /// Empty
        /// </summary>
        public AOTimers()
        {
        }

        #region Serialition stuff
        /// <summary>
        /// Deserialization, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AOTimers(SerializationInfo info, StreamingContext context)
        {
            Strain = (Int32) info.GetValue("Strain", typeof (Int32));
            Timestamp = (DateTime) info.GetValue("Timestamp", typeof (DateTime));
            Function = (AOFunctions) info.GetValue("Function", typeof (AOFunctions));
        }

        /// <summary>
        /// Serialization, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Strain", Strain);
            info.AddValue("Timestamp", Timestamp);
            info.AddValue("Function", Function);
        }
        #endregion
    }
}