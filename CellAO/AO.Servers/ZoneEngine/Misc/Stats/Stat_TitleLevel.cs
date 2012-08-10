#region License
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
#endregion

namespace ZoneEngine.Misc
{
    public class Stat_TitleLevel : Class_Stat
    {
        public Stat_TitleLevel(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
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
            if ((Parent is Character) || (Parent is NonPC)) // This condition could be obsolete
            {
                Character ch = (Character) Parent;
                int level = ch.Stats.Level.Value;

                if (level >= 205)
                {
                    Set(7);
                }
                else if (level >= 190)
                {
                    Set(6);
                }
                else if (level >= 150)
                {
                    Set(5);
                }
                else if (level >= 100)
                {
                    Set(4);
                }
                else if (level >= 50)
                {
                    Set(3);
                }
                else if (level >= 15)
                {
                    Set(2);
                }
                else Set(1);

                if (!Parent.startup)
                {
                    AffectStats();
                }
            }
        }
    }
}