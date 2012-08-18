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
    public class StatHealth : ClassStat
    {
        public StatHealth(
            int number, int defaultValue, string name, bool sendBaseValue, bool doNotWrite, bool announceToPlayfield)
        {
            this.StatNumber = number;
            this.StatDefaultValue = (uint)defaultValue;

            this.StatBaseValue = this.StatDefaultValue;
            this.SendBaseValue = sendBaseValue;
            this.DoNotDontWriteToSql = doNotWrite;
            this.AnnounceToPlayfield = announceToPlayfield;
        }

        public override void CalcTrickle()
        {
            #region table
            int[,] tableProfessionHitPoints =
                {
                    // Sol| MA|ENG|FIX|AGE|ADV|TRA|CRA|ENF|DOC| NT| MP| KEP|SHA   // geprüfte Prof & TL = Soldier, Martial Artist, Engineer, Fixer, Agent, Advy, Trader, Crat
                    { 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6 }, //TitleLevel 1
                    { 7, 7, 6, 7, 7, 7, 6, 7, 8, 6, 6, 6, 7, 7 }, //TitleLevel 2
                    { 8, 7, 6, 7, 7, 8, 7, 7, 9, 6, 6, 6, 8, 7 }, //TitleLevel 3
                    { 9, 8, 6, 8, 8, 8, 7, 7, 10, 6, 6, 6, 9, 8 }, //TitleLevel 4
                    { 10, 9, 6, 9, 8, 9, 8, 8, 11, 6, 6, 6, 10, 9 }, //TitleLevel 5
                    { 11, 12, 6, 10, 9, 9, 9, 9, 12, 6, 6, 6, 11, 10 }, //TitleLevel 6
                    { 12, 13, 7, 11, 10, 10, 10, 10, 13, 7, 7, 7, 12, 11 }, //TitleLevel 7
                };
            // Sol|Opi|Nan|Tro
            int[] breedBaseHitPoints = { 10, 15, 10, 25, 30, 30, 30 };
            int[] breedMultiplicatorHitPoints = { 3, 3, 2, 4, 8, 8, 10 };
            int[] breedModificatorHitPoints = { 0, -1, -1, 0, 0, 0, 0 };
            #endregion

            if ((this.Parent is Character) || (this.Parent is NonPlayerCharacterClass)) // This condition could be obsolete
            {
                Character character = (Character)this.Parent;
                uint breed = character.Stats.Breed.StatBaseValue;
                uint profession = character.Stats.Profession.StatBaseValue;
                if (profession > 13)
                {
                    profession--;
                }
                uint titleLevel = character.Stats.TitleLevel.StatBaseValue;
                uint level = character.Stats.Level.StatBaseValue;

                //BreedBaseHP+(Level*(TableProfHP+BreedModiHP))+(BodyDevelopment*BreedMultiHP))
                if (this.Parent is NonPlayerCharacterClass)
                {
                    // TODO: correct calculation of mob HP
                    Set(
                        breedBaseHitPoints[breed - 1] + (character.Stats.Level.Value * tableProfessionHitPoints[6, 8])
                        + (character.Stats.BodyDevelopment.Value + breedMultiplicatorHitPoints[breed - 1]));
                }
                else
                {
                    Set(
                        breedBaseHitPoints[breed - 1]
                        +
                        (character.Stats.Level.Value
                         *
                         (tableProfessionHitPoints[titleLevel - 1, profession - 1]
                          + breedModificatorHitPoints[breed - 1]))
                        + (character.Stats.BodyDevelopment.Value * breedMultiplicatorHitPoints[breed - 1]));
                }

                //ch.Stats.Health.StatBaseValue = (UInt32)Math.Min(ch.Stats.Health.Value, StatBaseValue);
            }
            if (!this.Parent.Starting)
            {
                this.AffectStats();
            }
        }
    }
}