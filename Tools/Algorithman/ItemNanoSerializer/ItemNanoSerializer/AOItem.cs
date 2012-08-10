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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace ZoneEngine
{
    [Serializable()]
    public class AOItem : ISerializable
    {
        public int flags = 0;
        public int lowID = 0;
        public int highID = 0;
        public int Quality = 0;
        public int multiplecount = 0;
        public int Type = 0;
        public int Instance = 0;
        public int Nothing = 0;
        public List<AOItemAttribute> Stats = new List<AOItemAttribute>();
        public List<AOEvents> Events = new List<AOEvents>();

        /// Methods to do:
        /// Read Item
        /// Write Item
        /// Return Dynel Item (placing on the ground)
        public AOItem ShallowCopy()
        {
            return (AOItem)this.MemberwiseClone();
        }

        /// <summary>
        /// Empty
        /// </summary>
        public AOItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isInstanced()
        {
            if ((Type == 0) && (Instance == 0))
            {
                return false;
            }
            return true;
        }

        public AOItem(SerializationInfo info, StreamingContext context)
        {
            flags = (int)info.GetValue("flags", typeof(int));
            lowID = (int)info.GetValue("lowID", typeof(int));
            highID = (int)info.GetValue("highID", typeof(int));
            Quality = (int)info.GetValue("Quality", typeof(int));
            multiplecount = (int)info.GetValue("multiplecount", typeof(int));
            Type = (int)info.GetValue("Type", typeof(int));
            Instance = (int)info.GetValue("Instance", typeof(int));
            Nothing = (int)info.GetValue("Nothing", typeof(int));
            Stats = (List<AOItemAttribute>)info.GetValue("Stats", typeof(List<AOItemAttribute>));
            Events = (List<AOEvents>)info.GetValue("Events", typeof(List<AOEvents>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("flags", flags);
            info.AddValue("lowID", lowID);
            info.AddValue("highID", highID);
            info.AddValue("Quality", Quality);
            info.AddValue("multiplecount", multiplecount);
            info.AddValue("Type", Type);
            info.AddValue("Instance", Instance);
            info.AddValue("Nothing", Nothing);
            info.AddValue("Stats", Stats);
            info.AddValue("Events", Events);
        }
    }

    [Serializable()]
    public class AOItemAttribute : ISerializable
    {
        public int Stat;
        public Int64 Value;

        public AOItemAttribute()
        {
            Stat = 0;
            Value = 0;
        }

        public AOItemAttribute(SerializationInfo info, StreamingContext context)
        {
            Stat = (int)info.GetValue("Stats", typeof(int));
            Value = (int)info.GetValue("Value", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Stat", Stat);
            info.AddValue("Value", Value);
        }
    }

}
