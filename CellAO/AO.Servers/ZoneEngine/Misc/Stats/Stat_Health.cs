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
    public class Stat_Health : ClassStat
    {
        public Stat_Health(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
        {
            this.StatNumber = Number;
            this.StatDefault = (uint)Default;

            this.Value = (int)this.StatDefault;
            this.SendBaseValue = sendbase;
            this.DoNotDontWriteToSql = dontwrite;
            this.AnnounceToPlayfield = announce;
        }

        public override void CalcTrickle()
        {
            #region table
            int[,] TableProfHP =
                {
                    //Sol| MA|ENG|FIX|AGE|ADV|TRA|CRA|ENF|DOC| NT| MP| KEP|SHA   // geprüfte Prof & TL = Soldier, Martial Artist, Engineer, Fixer, Agent, Advy, Trader, Crat
                    { 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6 }, //TitleLevel 1
                    { 7, 7, 6, 7, 7, 7, 6, 7, 8, 6, 6, 6, 7, 7 }, //TitleLevel 2
                    { 8, 7, 6, 7, 7, 8, 7, 7, 9, 6, 6, 6, 8, 7 }, //TitleLevel 3
                    { 9, 8, 6, 8, 8, 8, 7, 7, 10, 6, 6, 6, 9, 8 }, //TitleLevel 4
                    { 10, 9, 6, 9, 8, 9, 8, 8, 11, 6, 6, 6, 10, 9 }, //TitleLevel 5
                    { 11, 12, 6, 10, 9, 9, 9, 9, 12, 6, 6, 6, 11, 10 }, //TitleLevel 6
                    { 12, 13, 7, 11, 10, 10, 10, 10, 13, 7, 7, 7, 12, 11 }, //TitleLevel 7
                };
            //Sol|Opi|Nan|Tro
            int[] BreedBaseHP = { 10, 15, 10, 25, 30, 30, 30 };
            int[] BreedMultiHP = { 3, 3, 2, 4, 8, 8, 10 };
            int[] BreedModiHP = { 0, -1, -1, 0, 0, 0, 0 };
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

                //BreedBaseHP+(Level*(TableProfHP+BreedModiHP))+(BodyDevelopment*BreedMultiHP))
                if (this.Parent is NonPlayerCharacterClass)
                {
                    // TODO: correct calculation of mob HP
                    Set(
                        BreedBaseHP[breed - 1] + (ch.Stats.Level.Value * TableProfHP[6, 8])
                        + (ch.Stats.BodyDevelopment.Value + BreedMultiHP[breed - 1]));
                }
                else
                {
                    Set(
                        BreedBaseHP[breed - 1]
                        +
                        (ch.Stats.Level.Value * (TableProfHP[titlelevel - 1, profession - 1] + BreedModiHP[breed - 1]))
                        + (ch.Stats.BodyDevelopment.Value * BreedMultiHP[breed - 1]));
                }

                //ch.Stats.Health.StatBaseValue = (UInt32)Math.Min(ch.Stats.Health.Value, StatBaseValue);
            }
            if (!this.Parent.startup)
            {
                this.AffectStats();
            }
        }
    }
}