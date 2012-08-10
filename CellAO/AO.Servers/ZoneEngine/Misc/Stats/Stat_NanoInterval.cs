﻿#region License
/*
Copyright (c) 2005-2012, CellAO Team

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
using AO.Core;
#endregion

namespace ZoneEngine.Misc
{
    public class Stat_NanoInterval : Class_Stat
    {
        public Stat_NanoInterval(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
        {
            StatNumber = Number;
            StatDefault = (uint) Default;

            Value = (int) StatDefault;
            SendBaseValue = true;
            DontWriteToSQL = false;
            AnnounceToPlayfield = false;
        }

        public override void CalcTrickle()
        {
            if ((Parent is Character) || (Parent is NonPC))
            {
                Character ch = (Character) Parent;

                // calculating Nano and Heal Delta and interval
                int nanointerval = 28 - (Math.Min((int) Math.Floor(Convert.ToDouble(ch.Stats.Psychic.Value)/60), 13)*2);
                ch.Stats.NanoInterval.StatBaseValue = (uint) nanointerval; // Healinterval

                ch.PurgeTimer(1);
                AOTimers at = new AOTimers();
                at.Strain = 1;

                int nd = ch.Stats.NanoDelta.Value;
                if (ch.moveMode == Character.MoveMode.Sit)
                {
                    int nd2 = nd >> 1;
                    nd = nd + nd2;
                }

                at.Timestamp = DateTime.Now + TimeSpan.FromSeconds(ch.Stats.NanoInterval.Value);
                at.Function.Target = Parent.ID; // changed from ItemHandler.itemtarget_self;
                at.Function.TickCount = -2;
                at.Function.TickInterval = (uint) (ch.Stats.NanoInterval.Value*1000);
                at.Function.FunctionType = Constants.functiontype_hit;
                at.Function.Arguments.Add(214);
                at.Function.Arguments.Add(nd);
                at.Function.Arguments.Add(nd);
                at.Function.Arguments.Add(0);
                ch.Timers.Add(at);

                if (!Parent.startup)
                {
                    AffectStats();
                }
            }
        }
    }
}