﻿#region License
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

namespace ZoneEngine.PacketHandlers
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;

    public static class SkillUpdate
    {
        public static double CalculateIP(Client cli)
        {
            double calc = 0;
            uint breed = cli.Character.Stats.Breed.StatBaseValue; // 4 = Breed
            uint profession = cli.Character.Stats.Profession.StatBaseValue - 1; // 60 = Profession

            #region tables...
            Double[,] skillCosts =
                {
                    { 100, 2.8, 1.6, 2.8, 2.0, 1.6, 2.8, 2.8, 3.0, 1.0, 2.8, 2.8, 1.6, 2.0, 2.0 },
                    { 101, 1.4, 2.5, 4.0, 3.2, 1.0, 4.0, 2.5, 3.2, 2.5, 4.0, 4.0, 1.0, 2.0, 3.2 },
                    { 102, 1.5, 1.6, 3.2, 2.4, 1.0, 2.4, 2.5, 3.2, 2.5, 2.6, 4.0, 4.0, 2.5, 1.8 },
                    { 103, 1.0, 2.0, 4.0, 2.4, 1.0, 3.2, 2.0, 3.2, 2.0, 4.0, 4.0, 4.0, 2.0, 3.2 },
                    { 106, 1.5, 2.5, 4.0, 2.4, 1.0, 3.2, 2.5, 3.2, 2.0, 3.2, 4.0, 1.0, 2.5, 2.5 },
                    { 107, 1.5, 2.5, 4.0, 3.2, 1.4, 3.2, 3.2, 3.2, 2.0, 4.0, 3.2, 4.0, 2.5, 2.5 },
                    { 105, 1.5, 2.5, 4.0, 3.2, 1.0, 3.2, 2.5, 1.0, 2.0, 2.5, 2.5, 4.0, 2.5, 2.5 },
                    { 104, 1.5, 3.2, 4.0, 3.2, 1.8, 4.0, 3.2, 3.2, 3.0, 4.0, 3.2, 4.0, 2.2, 2.5 },
                    { 108, 1.6, 1.2, 3.2, 3.2, 1.6, 3.2, 2.5, 4.0, 1.0, 3.2, 2.4, 1.6, 1.6, 2.4 },
                    { 109, 1.6, 1.6, 4.0, 3.2, 2.5, 2.0, 2.2, 4.0, 2.4, 4.0, 2.4, 4.0, 1.6, 2.4 },
                    { 110, 3.0, 3.0, 4.0, 4.0, 2.5, 2.0, 2.5, 4.0, 4.0, 4.0, 4.0, 4.0, 1.0, 4.0 },
                    { 111, 1.8, 2.0, 4.0, 4.0, 4.0, 4.0, 2.4, 4.0, 1.0, 2.5, 4.0, 4.0, 2.4, 4.0 },
                    { 112, 1.0, 1.8, 1.6, 1.6, 3.0, 1.5, 1.6, 4.0, 3.5, 2.4, 1.6, 4.0, 1.0, 2.0 },
                    { 113, 1.7, 1.3, 4.0, 4.0, 4.0, 4.0, 2.0, 4.0, 4.0, 4.0, 4.0, 4.0, 2.0, 2.8 },
                    { 114, 2.5, 2.5, 3.2, 3.2, 2.5, 3.2, 1.0, 4.0, 3.0, 3.2, 3.2, 4.0, 1.5, 2.4 },
                    { 115, 2.4, 3.2, 3.2, 2.4, 2.5, 3.2, 1.8, 4.0, 4.0, 4.0, 3.0, 4.0, 1.5, 1.5 },
                    { 116, 1.6, 3.0, 4.0, 4.0, 3.5, 3.0, 2.8, 4.0, 4.0, 4.0, 4.5, 4.0, 1.0, 4.0 },
                    { 117, 1.0, 2.4, 2.4, 2.4, 1.6, 1.6, 1.0, 2.4, 2.5, 2.5, 2.4, 3.2, 1.6, 1.4 },
                    { 118, 1.8, 1.6, 4.0, 3.2, 1.0, 3.2, 2.5, 1.0, 2.0, 3.0, 3.5, 1.0, 2.4, 3.2 },
                    { 119, 2.0, 1.6, 3.2, 3.2, 3.0, 3.2, 1.6, 3.8, 2.4, 4.0, 3.0, 4.0, 1.0, 2.5 },
                    { 120, 1.6, 1.6, 2.0, 2.0, 1.6, 3.0, 2.4, 3.8, 1.0, 3.2, 3.2, 3.4, 2.4, 3.2 },
                    { 121, 1.6, 2.0, 4.0, 4.0, 2.0, 3.5, 2.0, 4.0, 1.0, 2.5, 4.0, 4.0, 2.0, 4.0 },
                    { 122, 1.6, 1.6, 1.0, 1.6, 2.5, 2.4, 2.4, 2.4, 1.6, 1.6, 1.0, 1.6, 2.4, 1.8 },
                    { 123, 1.2, 2.0, 2.0, 1.0, 1.6, 2.0, 1.2, 1.2, 1.6, 2.0, 2.0, 2.5, 2.0, 1.6 },
                    { 124, 1.0, 2.0, 2.0, 1.0, 2.0, 1.6, 1.2, 1.8, 2.0, 2.0, 2.0, 1.5, 2.0, 1.6 },
                    { 125, 1.2, 1.5, 1.8, 2.0, 2.0, 1.0, 1.5, 3.2, 2.4, 2.0, 2.0, 3.2, 2.0, 1.2 },
                    { 126, 1.6, 2.0, 2.4, 1.6, 1.8, 1.0, 1.5, 3.2, 3.2, 2.0, 1.6, 3.2, 2.4, 1.0 },
                    { 127, 1.8, 1.2, 1.6, 1.0, 2.5, 1.0, 2.4, 3.2, 2.0, 1.0, 1.0, 3.2, 2.0, 1.6 },
                    { 128, 1.5, 1.6, 1.0, 1.0, 2.5, 2.4, 3.2, 1.8, 1.6, 1.0, 1.0, 1.9, 2.4, 1.8 },
                    { 129, 1.8, 1.6, 1.0, 1.6, 2.5, 2.4, 2.4, 1.6, 2.0, 1.6, 1.0, 1.4, 2.0, 1.5 },
                    { 130, 1.8, 1.4, 1.6, 1.6, 2.5, 1.0, 2.5, 3.2, 2.4, 1.0, 1.0, 3.2, 2.5, 1.5 },
                    { 131, 1.8, 2.4, 1.6, 1.6, 2.5, 1.0, 3.2, 1.4, 1.6, 1.0, 1.0, 1.9, 3.2, 1.5 },
                    { 132, 1.6, 1.2, 1.4, 1.0, 2.0, 1.8, 1.6, 2.2, 1.6, 1.0, 1.0, 2.5, 2.0, 1.2 },
                    { 133, 2.4, 2.5, 4.0, 4.0, 4.0, 3.0, 2.5, 4.0, 4.0, 4.0, 4.0, 4.0, 1.0, 3.0 },
                    { 134, 1.5, 1.8, 4.0, 4.0, 4.0, 4.0, 2.0, 4.0, 4.0, 2.5, 4.0, 4.0, 2.0, 2.5 },
                    { 135, 1.6, 2.0, 2.4, 2.4, 2.4, 1.6, 1.0, 2.4, 2.5, 2.4, 2.4, 1.8, 2.4, 2.4 },
                    { 136, 1.6, 1.0, 1.6, 2.4, 2.4, 2.4, 1.0, 1.2, 1.6, 2.4, 2.4, 2.4, 2.4, 1.4 },
                    { 137, 1.0, 3.0, 2.0, 2.0, 1.5, 2.0, 2.0, 1.8, 1.6, 2.0, 2.0, 1.6, 1.5, 1.4 },
                    { 138, 1.0, 1.6, 2.0, 2.0, 2.0, 2.0, 2.0, 1.8, 1.4, 2.0, 2.0, 1.4, 1.5, 1.5 },
                    { 139, 1.0, 2.4, 2.0, 2.4, 1.6, 1.6, 1.0, 2.4, 3.0, 2.5, 2.4, 3.2, 1.6, 1.4 },
                    { 140, 1.0, 1.6, 2.0, 1.6, 1.6, 1.6, 2.0, 1.2, 2.0, 2.0, 1.6, 2.4, 1.6, 1.3 },
                    { 141, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 },
                    { 142, 2.4, 2.8, 3.2, 2.8, 1.0, 2.4, 1.8, 2.0, 1.2, 2.8, 2.8, 4.0, 2.0, 2.0 },
                    { 143, 3.2, 3.0, 3.2, 3.2, 1.2, 3.2, 2.4, 1.0, 1.0, 3.2, 2.4, 1.4, 2.4, 3.2 },
                    { 144, 4.0, 1.6, 3.0, 4.0, 4.0, 4.0, 4.0, 1.3, 1.2, 2.5, 2.5, 1.0, 4.0, 4.0 },
                    { 145, 1.5, 1.6, 3.2, 2.4, 1.4, 4.0, 2.4, 1.0, 1.5, 4.0, 3.2, 1.4, 2.5, 2.5 },
                    { 146, 1.5, 1.0, 2.4, 3.2, 2.0, 4.0, 3.9, 4.0, 3.0, 4.0, 3.2, 1.0, 3.0, 4.0 },
                    { 147, 2.0, 2.5, 4.0, 2.4, 1.5, 4.0, 2.5, 1.0, 2.0, 4.0, 3.2, 1.4, 2.4, 3.0 },
                    { 148, 1.8, 3.2, 4.0, 3.0, 3.0, 3.0, 1.5, 4.0, 4.0, 4.0, 4.0, 4.0, 1.5, 3.5 },
                    { 149, 2.0, 1.6, 1.0, 1.0, 2.4, 1.6, 2.4, 3.2, 2.5, 1.0, 1.0, 2.8, 4.0, 1.5 },
                    { 150, 1.0, 3.2, 4.0, 2.4, 3.5, 3.2, 1.6, 4.0, 3.2, 4.0, 4.0, 4.0, 1.0, 2.5 },
                    { 151, 2.2, 1.1, 4.0, 3.2, 3.5, 4.0, 2.5, 4.0, 3.0, 4.0, 3.2, 4.0, 1.8, 2.5 },
                    { 152, 1.2, 2.4, 2.4, 2.0, 1.0, 2.4, 1.8, 1.2, 1.5, 2.4, 2.4, 2.6, 1.1, 2.0 },
                    { 153, 1.6, 1.6, 2.4, 2.4, 2.0, 2.2, 1.0, 1.6, 1.0, 2.4, 2.4, 1.2, 1.8, 1.9 },
                    { 154, 1.6, 2.1, 2.4, 2.4, 2.0, 2.5, 1.0, 1.6, 1.0, 1.6, 2.4, 2.4, 1.5, 1.9 },
                    { 155, 1.8, 2.4, 2.4, 3.2, 1.5, 4.0, 1.6, 1.4, 1.0, 1.6, 3.2, 1.0, 2.0, 1.9 },
                    { 156, 1.0, 1.6, 2.4, 2.4, 2.4, 2.0, 1.0, 2.0, 1.0, 2.4, 2.4, 1.0, 2.0, 1.9 },
                    { 157, 1.6, 2.0, 2.4, 1.6, 3.2, 1.0, 1.5, 3.2, 3.2, 2.4, 1.6, 1.4, 2.4, 1.2 },
                    { 158, 1.6, 4.0, 2.5, 1.5, 1.5, 1.0, 1.3, 2.0, 2.4, 2.5, 3.2, 3.2, 1.5, 1.0 },
                    { 159, 2.4, 1.6, 2.4, 1.0, 1.6, 1.5, 1.5, 3.2, 2.4, 2.0, 2.4, 1.8, 2.0, 1.0 },
                    { 160, 4.0, 2.4, 1.6, 1.6, 2.4, 1.2, 2.0, 3.2, 2.4, 1.0, 1.0, 4.0, 2.0, 1.4 },
                    { 161, 1.6, 1.6, 1.0, 1.0, 1.6, 1.3, 1.0, 2.4, 2.0, 1.0, 1.0, 2.4, 2.0, 1.5 },
                    { 162, 1.6, 1.0, 1.0, 2.3, 1.0, 2.4, 1.5, 1.0, 1.6, 1.6, 2.4, 2.4, 1.5, 1.0 },
                    { 163, 1.6, 1.5, 2.4, 2.0, 1.0, 1.2, 2.0, 3.2, 2.4, 2.0, 2.0, 3.2, 2.4, 1.3 },
                    { 164, 1.7, 1.0, 2.4, 2.4, 2.0, 3.2, 1.5, 3.2, 1.5, 2.5, 2.5, 1.0, 2.0, 1.8 },
                    { 165, 2.0, 1.5, 2.0, 2.0, 2.0, 1.6, 1.0, 2.4, 2.0, 2.4, 2.5, 1.6, 2.0, 1.8 },
                    { 166, 1.0, 2.4, 2.4, 1.6, 1.0, 1.6, 1.0, 2.4, 2.5, 2.5, 2.4, 3.2, 1.0, 1.4 },
                    { 167, 2.4, 4.0, 4.0, 4.0, 3.0, 3.0, 2.2, 4.0, 4.0, 4.0, 5.0, 4.0, 1.5, 3.5 },
                    { 168, 2.4, 1.6, 1.6, 1.2, 2.2, 1.5, 1.6, 1.8, 1.6, 1.6, 1.0, 1.5, 2.2, 1.6 }
                };

            int[] professionMatrix = { 12, 8, 5, 6, 1, 0, 13, 2, 4, 3, 10, 9, -1, 7, 11 };
            double[,] attributeCost =
                {
                    { 16, 2, 2, 3, 1 }, { 17, 2, 1, 3, 2 }, { 18, 2, 3, 2, 1 }, { 19, 2, 2, 1, 3 },
                    { 20, 2, 1, 2, 3 }, { 21, 2, 2, 1, 3 }
                };

            double[,] baseAttributes =
                {
                    { 6, 6, 6, 6, 6, 6 }, { 3, 15, 6, 6, 10, 3 }, { 3, 3, 3, 15, 6, 10 },
                    { 15, 6, 10, 3, 3, 3 }
                };
            #endregion

            int counter = 0;
            int c2;
            int stat = 0;
            // start with attributes...
            for (counter = 0; counter < 6; counter++)
            {
                stat = (Int32)cli.Character.Stats.GetBaseValue(Convert.ToInt32(attributeCost[counter, 0]));
                for (c2 = (Int32)baseAttributes[breed - 1, counter]; c2 < stat; c2++)
                {
                    calc += attributeCost[counter, breed] * c2;
                }
            }

            for (counter = 0; counter < 69; counter++)
            {
                stat = (Int32)cli.Character.Stats.GetBaseValue(Convert.ToInt32(skillCosts[counter, 0])) - 1;
                for (c2 = 5; c2 <= stat; c2++)
                {
                    calc += Math.Floor(skillCosts[counter, professionMatrix[profession] + 1] * c2);
                }
            }

            return calc;
        }

        public static uint CalculateHitPoints(Client cli)
        {
            #region table
            int[,] tableProfessionHitPoints =
                {
                    //Sol| MA|ENG|FIX|AGE|ADV|TRA|CRA|ENF|DOC| NT| MP| KEP|SHA   // geprüfte Prof & TL = Soldier, Martial Artist, Engineer, Fixer
                    { 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6 }, //TitleLevel 1
                    { 7, 7, 6, 7, 7, 7, 6, 7, 8, 6, 6, 6, 7, 7 }, //TitleLevel 2
                    { 8, 7, 6, 7, 7, 8, 7, 7, 9, 6, 6, 6, 8, 8 }, //TitleLevel 3
                    { 9, 8, 6, 8, 8, 8, 7, 7, 10, 6, 6, 6, 9, 9 }, //TitleLevel 4
                    { 10, 9, 6, 9, 8, 9, 8, 8, 11, 6, 6, 6, 10, 9 }, //TitleLevel 5
                    { 11, 12, 6, 10, 9, 9, 9, 9, 12, 6, 6, 6, 11, 10 }, //TitleLevel 6
                    { 12, 13, 7, 11, 10, 10, 9, 9, 13, 6, 6, 6, 11, 10 }, //TitleLevel 7
                };
            //Sol|Opi|Nan|Tro
            int[] breedBaseHitPoints = { 10, 15, 10, 25 };
            int[] breedMultiplicatorHitPoints = { 3, 3, 2, 4 };
            int[] breedModificatorHitPoints = { 0, -1, -1, 0 };
            #endregion

            uint breed = cli.Character.Stats.Breed.StatBaseValue;
            uint profession = cli.Character.Stats.Profession.StatBaseValue;
            uint titlelevel = cli.Character.Stats.TitleLevel.StatBaseValue;
            uint level = cli.Character.Stats.Level.StatBaseValue;

            //BreedBaseHP+(Level*(TableProfHP+BreedModiHP))+(BodyDevelopment*BreedMultiHP))
            return
                (uint)
                (breedBaseHitPoints[breed - 1]
                 +
                 (cli.Character.Stats.Level.Value
                  * (tableProfessionHitPoints[titlelevel - 1, profession - 1] + breedModificatorHitPoints[breed - 1]))
                 + (cli.Character.Stats.BodyDevelopment.Value * breedMultiplicatorHitPoints[breed - 1]));
        }

        public static uint CalculateNanoPoints(Client cli)
        {
            #region table
            int[,] tableProfNanoPoints =
                {
                    // Sol|MA|ENG|FIX|AGE|ADV|TRA|CRA|ENF|DOC| NT| MP|KEP|SHA  // geprüfte Prof & TL = Soldier, Martial Artist, Engineer, Fixer
                    { 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4 }, //TitleLevel 1
                    { 4, 4, 5, 4, 5, 5, 5, 5, 4, 5, 5, 5, 4, 4 }, //TitleLevel 2
                    { 4, 4, 6, 4, 6, 5, 5, 5, 4, 6, 6, 6, 4, 4 }, //TitleLevel 3
                    { 4, 4, 7, 4, 6, 6, 5, 5, 4, 7, 7, 7, 4, 4 }, //TitleLevel 4
                    { 4, 4, 8, 4, 7, 6, 6, 6, 4, 8, 8, 8, 4, 4 }, //TitleLevel 5
                    { 4, 4, 9, 4, 7, 7, 7, 7, 4, 10, 10, 10, 4, 4 }, //TitleLevel 6
                    { 5, 5, 10, 5, 8, 8, 7, 7, 5, 10, 10, 10, 4, 4 }, //TitleLevel 7
                };
            // Sol|Opi|Nan|Tro
            int[] breedBaseNanoPoints = { 10, 10, 15, 8 };
            int[] breedMultiplicatorNanoPoints = { 3, 3, 4, 2 };
            int[] breedModificatorNanoPoints = { 0, -1, 1, -2 };
            #endregion

            uint breed = cli.Character.Stats.Breed.StatBaseValue;
            uint profession = cli.Character.Stats.Profession.StatBaseValue;
            uint titleLevel = cli.Character.Stats.TitleLevel.StatBaseValue;
            uint level = cli.Character.Stats.Level.StatBaseValue;

            //BreedBaseNP+(Level*(TableProfNP+BreedModiNP))+(NanoEnergyPool*BreedMultiNP))
            return
                (uint)
                (breedBaseNanoPoints[breed - 1]
                 +
                 (cli.Character.Stats.Level.Value
                  * (tableProfNanoPoints[titleLevel - 1, profession - 1] + breedModificatorNanoPoints[breed - 1]))
                 + (cli.Character.Stats.NanoEnergyPool.Value * breedMultiplicatorNanoPoints[breed - 1]));
        }

        public static void SendStat(Client client, int statnum, int value, Boolean announce)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushBytes(new byte[] { 0xDF, 0xDF, });
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.Id);
            packetWriter.PushInt(0x2B333D6E);
            packetWriter.PushIdentity(50000, client.Character.Id);
            packetWriter.PushByte(1);
            packetWriter.PushInt(1);
            packetWriter.PushInt(statnum);
            packetWriter.PushInt(value);

            byte[] reply = packetWriter.Finish();
            client.SendCompressed(reply);

            /* announce to playfield? */
            if (announce)
            {
                Announce.Playfield(client.Character.PlayField, reply);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public static void Read(byte[] packet, Client client)
        {
            PacketWriter packetWriter = new PacketWriter();
            PacketReader packetReader = new PacketReader(packet);

            Header header = packetReader.PopHeader();
            byte unknown1 = packetReader.PopByte();
            uint[] nanodelta = { 3, 3, 4, 2 };
            uint[] healdelta = { 3, 3, 2, 4 };

            uint baseIP = 0;
            uint characterLevel;

            characterLevel = client.Character.Stats.Level.StatBaseValue; // 54 = Level

            // Calculate base IP value for character level
            if (characterLevel > 204)
            {
                baseIP += (characterLevel - 204) * 600000;
                characterLevel = 204;
            }
            if (characterLevel > 189)
            {
                baseIP += (characterLevel - 189) * 150000;
                characterLevel = 189;
            }
            if (characterLevel > 149)
            {
                baseIP += (characterLevel - 149) * 80000;
                characterLevel = 149;
            }
            if (characterLevel > 99)
            {
                baseIP += (characterLevel - 99) * 40000;
                characterLevel = 99;
            }
            if (characterLevel > 49)
            {
                baseIP += (characterLevel - 49) * 20000;
                characterLevel = 49;
            }
            if (characterLevel > 14)
            {
                baseIP += (characterLevel - 14) * 10000; // Change 99 => 14 by Wizard
                characterLevel = 14;
            }
            baseIP += 1500 + (characterLevel - 1) * 4000;

            // Prepare reply packet

            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            packetWriter.PushShort(0x0a);
            packetWriter.PushShort(0x01);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(header.Sender);
            packetWriter.PushInt(0x3e205660);
            packetWriter.PushIdentity(50000, header.Sender);
            packetWriter.PushByte(0);

            int count = packetReader.PopInt();
            uint statval;
            List<int> statlist = new List<int>();
            while (count > 0)
            {
                int statNumber = packetReader.PopInt();
                statval = packetReader.PopUInt();
                client.Character.Stats.SetBaseValue(statNumber, statval);
                statlist.Add(statNumber);
                count--;
            }

            packetReader.Finish();

            statlist.Add(53); // IP
            uint usedIP = baseIP - (uint)Math.Floor(CalculateIP(client));
            client.Character.Stats.IP.StatBaseValue = usedIP;

            // Send the changed stats back to the client
            packetWriter.PushInt(statlist.Count);
            count = 0;
            while (count < statlist.Count)
            {
                statval = client.Character.Stats.GetBaseValue(statlist[count]);
                packetWriter.PushInt(statlist[count]);
                packetWriter.PushUInt(statval);
                count++;
            }

            byte[] reply = packetWriter.Finish();
            client.SendCompressed(reply);

            // and save the changes to the statsdb
            client.Character.WriteStats();
            client.Character.CalculateSkills();
        }
    }
}