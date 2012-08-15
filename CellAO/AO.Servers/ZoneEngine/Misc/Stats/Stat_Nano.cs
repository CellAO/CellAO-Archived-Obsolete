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
    public class Stat_Nano : ClassStat
    {
        public Stat_Nano(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
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
            #region table
            int[,] TableProfNP =
                {
                    //Sol|MA|ENG|FIX|AGE|ADV|TRA|CRA|ENF|DOC| NT| MP|KEP|SHA  // geprüfte Prof & TL = Soldier, Martial Artist, Engineer, Fixer, Agent, Advy, Trader, Doc
                    { 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4 }, //TitleLevel 1
                    { 4, 4, 5, 4, 5, 5, 5, 5, 4, 5, 5, 5, 4, 4 }, //TitleLevel 2
                    { 4, 4, 6, 4, 6, 5, 5, 5, 4, 6, 6, 6, 4, 4 }, //TitleLevel 3
                    { 4, 4, 7, 4, 6, 6, 5, 5, 4, 7, 7, 7, 4, 4 }, //TitleLevel 4
                    { 4, 4, 8, 4, 7, 6, 6, 6, 4, 8, 8, 8, 4, 4 }, //TitleLevel 5
                    { 4, 4, 9, 4, 7, 7, 7, 7, 4, 10, 10, 10, 4, 5 }, //TitleLevel 6
                    { 5, 5, 10, 5, 8, 8, 8, 8, 5, 11, 11, 11, 5, 7 }, //TitleLevel 7
                };
            //Sol|Opi|Nan|Tro
            int[] BreedBaseNP = { 10, 10, 15, 8, 10, 10, 10 };
            int[] BreedMultiNP = { 3, 3, 4, 2, 3, 3, 3 };
            int[] BreedModiNP = { 0, -1, 1, -2, 0, 0, 0 };
            #endregion

            if ((this.Parent is Character) || (this.Parent is NonPlayerCharacterClass)) // This condition could be obsolete
            {
                Character ch = (Character)this.Parent;
                uint breed = ch.Stats.Breed.StatBaseValue;
                uint profession = ch.Stats.Profession.StatBaseValue;
                if (profession > 13)
                {
                    profession--;
                }
                uint titlelevel = ch.Stats.TitleLevel.StatBaseValue;
                uint level = ch.Stats.Level.StatBaseValue;

                //BreedBaseNP+(Level*(TableProfNP+BreedModiNP))+(NanoEnergyPool*BreedMultiNP))
                if (this.Parent is NonPlayerCharacterClass)
                {
                    // TODO: correct calculation of mob NP
                    this.Set(
                        (uint)
                        (BreedBaseNP[breed - 1] + (ch.Stats.Level.Value * (TableProfNP[6, 8] + BreedModiNP[breed - 1]))
                         + (ch.Stats.NanoEnergyPool.Value * BreedMultiNP[breed - 1])));
                }
                else
                {
                    this.Set(
                        (uint)
                        (BreedBaseNP[breed - 1]
                         +
                         (ch.Stats.Level.Value * (TableProfNP[titlelevel - 1, profession - 1] + BreedModiNP[breed - 1]))
                         + (ch.Stats.NanoEnergyPool.Value * BreedMultiNP[breed - 1])));
                }
            }
            if (!this.Parent.startup)
            {
                this.AffectStats();
            }
        }
    }
}