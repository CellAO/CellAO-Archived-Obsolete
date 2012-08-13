#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...

#endregion

namespace ZoneEngine.Misc
{
    using System;

    using ZoneEngine.PacketHandlers;

    public class Stat_IP : Class_Stat
    {
        public Stat_IP(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
        {
            this.StatNumber = Number;
            this.StatDefault = (uint)Default;

            this.Value = (int)this.StatDefault;
            this.SendBaseValue = true;
            this.DoNotDontWriteToSql = false;
            this.AnnounceToPlayfield = false;
        }

        public override void CalcTrickle()
        {
            if ((this.Parent is Character) || (this.Parent is NonPC)) // This condition could be obsolete
            {
                Character ch = (Character)this.Parent;
                int baseIP = 0;
                int charlevel;

                charlevel = (Int32)ch.Stats.Level.StatBaseValue;

                // Calculate base IP value for character level
                if (charlevel > 204)
                {
                    baseIP += (charlevel - 204) * 600000;
                    charlevel = 204;
                }
                if (charlevel > 189)
                {
                    baseIP += (charlevel - 189) * 150000;
                    charlevel = 189;
                }
                if (charlevel > 149)
                {
                    baseIP += (charlevel - 149) * 80000;
                    charlevel = 149;
                }
                if (charlevel > 99)
                {
                    baseIP += (charlevel - 99) * 40000;
                    charlevel = 99;
                }
                if (charlevel > 49)
                {
                    baseIP += (charlevel - 49) * 20000;
                    charlevel = 49;
                }
                if (charlevel > 14)
                {
                    baseIP += (charlevel - 14) * 10000; // Change 99 => 14 by Wizard
                    charlevel = 14;
                }
                baseIP += 1500 + (charlevel - 1) * 4000;

                this.Set(baseIP - Convert.ToInt32(SkillUpdate.calcIP(ch.client)));

                if (!this.Parent.startup)
                {
                    this.AffectStats();
                }
            }
        }
    }
}