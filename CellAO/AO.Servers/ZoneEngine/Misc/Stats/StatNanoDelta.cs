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

namespace ZoneEngine.Misc
{
    using System;

    public class StatNanoDelta : ClassStat
    {
        public StatNanoDelta(int number, int defaultValue, string name, bool sendBaseValue, bool doNotWrite, bool announceToPlayfield)
        {
            this.StatNumber = number;
            this.StatDefaultValue = (uint)defaultValue;

            this.Value = (int)this.StatDefaultValue;
            this.SendBaseValue = true;
            this.DoNotDontWriteToSql = false;
            this.AnnounceToPlayfield = false;
        }

        public override void CalcTrickle()
        {
            if ((this.Parent is Character) || (this.Parent is NonPlayerCharacterClass)) // This condition could be obsolete
            {
                Character character = (Character)this.Parent;
                uint[] nanodelta = { 3, 3, 4, 2, 12, 15, 20 };
                uint nanoDelta = nanodelta[character.Stats.Breed.Value - 1]
                          + (uint)Math.Floor((double)(character.Stats.NanoEnergyPool.Value / 100));
                // Whats this? TODO: Find the original routine again
                this.StatBaseValue = nanoDelta;

                if (!this.Parent.Starting)
                {
                    this.AffectStats();
                }
            }
        }
    }
}