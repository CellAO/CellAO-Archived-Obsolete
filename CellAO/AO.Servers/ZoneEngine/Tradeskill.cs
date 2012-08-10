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

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AO.Core;
#endregion

namespace ZoneEngine
{
    class Tradeskill
    {
        #region Properties & Instance Variables

        // Do not alter or remove property comments unless also
        // changing the way in which the properties are used.

        // High IDs only.
        public int SourceID { get; set; }
        public int TargetID { get; set; }

        public int ResultLID { get; set; }
        public int ResultHID { get; set; }

        public string SourceName { get; set; }
        public string TargetName { get; set; }
        public string ResultName { get; set; }

        // Skill => 0 = Do not use.
        // SkillPerBump => 0 = No bump.
        public int FirstSkill { get; set; }
        public int FirstSkillPercent { get; set; }
        public int FirstSkillPerBump { get; set; }
        int FirstSkillValue;
        int FirstSkillRequirement;
        string FirstSkillName;

        int SecondSkill { get; set; }
        int SecondSkillPercent { get; set; }
        int SecondSkillPerBump { get; set; }
        int SecondSkillValue;
        int SecondSkillRequirement;
        string SecondSkillName;

        public int MaxBump { get; set; }

        // 0 = Do not check for range.
        public int RangePercent { get; set; }

        // Bit 0 = Delete source, Bit 1 = Delete target.
        public int DeleteFlag { get; set; }

        public int MinXP { get; set; }
        public int MaxXP { get; set; }

        // For the tradeskill window
        public int MinQL { get; set; }
        public int MaxQL { get; set; }

        public Client Cli { get; set; }
        public int SourcePlacement { get; set; }
        public int TargetPlacement { get; set; }
        public AOItem Source { get; set; }
        public AOItem Target { get; set; }
        public bool isTradeskill { get; set; }
        public bool bDeleteSource { get; set; }
        public bool bDeleteTarget { get; set; }

        #endregion

        public Tradeskill(Client cli, int src_loc, int tgt_loc)
        {
            Cli = cli;
            SourcePlacement = src_loc;
            TargetPlacement = tgt_loc;
            Source = cli.Character.getInventoryAt(src_loc).Item;
            Target = cli.Character.getInventoryAt(tgt_loc).Item;

            SourceID = Source.highID;
            TargetID = Target.highID;

            isTradeskill = false;

            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt = wrapper.ReadDT("SELECT * FROM tradeskill WHERE ID1 = " + Source.highID + " AND ID2 = " + Target.highID + ";");
            DataRowCollection drc = dt.Rows;

            if (drc.Count > 0)
            {
                isTradeskill = true;
                SourceName = (string)drc[0][3];
                TargetName = (string)drc[0][4];
                ResultName = (string)drc[0][5];
                ResultLID = (int)drc[0][6];
                ResultHID = (int)drc[0][7];
                RangePercent = (int)drc[0][8];
                DeleteFlag = (int)drc[0][9];
                FirstSkill = (int)drc[0][10];
                FirstSkillPercent = (int)drc[0][11];
                FirstSkillPerBump = (int)drc[0][12];
                SecondSkill = (int)drc[0][13];
                SecondSkillPercent = (int)drc[0][14];
                SecondSkillPerBump = (int)drc[0][15];
                MaxBump = (int)drc[0][16];
                MinXP = (int)drc[0][17];
                MaxXP = (int)drc[0][18];

                bDeleteSource = ((DeleteFlag & 1) == 1);
                bDeleteTarget = (((DeleteFlag >> 1) & 1) == 1);

                MinQL = Target.Quality;
                SetMaxQL();

                if (FirstSkill != 0)
                {
                    FirstSkillRequirement = (int)Math.Ceiling((decimal)FirstSkillPercent / 100M * (decimal)Target.Quality);
                    FirstSkillValue = Cli.Character.Stats.GetStatbyNumber(FirstSkill).Value;
                    FirstSkillName = StatsList.GetStatName(FirstSkill);
                }

                if (SecondSkill != 0)
                {
                    SecondSkillRequirement = (int)Math.Ceiling((decimal)SecondSkillPercent / 100M * (decimal)Target.Quality);
                    SecondSkillValue = Cli.Character.Stats.GetStatbyNumber(SecondSkill).Value;
                    SecondSkillName = StatsList.GetStatName(SecondSkill);
                }
            }
        }

        public bool Build()
        {
            if (!isTradeskill)
            {
                Cli.SendChatText("It is not possible to combine these two items.");
                return false;
            }
            if (!ValidateRange())
            {
                Cli.SendChatText(string.Format("{0} must be of at least QL{1} to combine with QL{2} {3}.", SourceName, GetSourceMinQL(), Target.Quality, TargetName));
                return false;
            }
            if (!ValidateSkill(FirstSkill, FirstSkillValue, FirstSkillRequirement))
            {
                Cli.SendChatText(GetSkillError(FirstSkillRequirement, FirstSkillName));
                if (SecondSkill != 0)
                {
                    Cli.SendChatText(GetSkillError(SecondSkillRequirement, SecondSkillName));
                }
                return false;
            }
            if (!ValidateSkill(SecondSkill, SecondSkillValue, SecondSkillRequirement))
            {
                Cli.SendChatText(GetSkillError(FirstSkillRequirement, FirstSkillName));
                Cli.SendChatText(GetSkillError(SecondSkillRequirement, SecondSkillName));
                return false;
            }

            int placement = SpawnItem(ResultLID, ResultHID, MaxQL);

            if (placement != 0)
            {
                if (bDeleteSource)
                    DeleteItem(SourcePlacement);
                if (bDeleteTarget)
                    DeleteItem(TargetPlacement);
                Cli.SendChatText(string.Format("You have successfully combined QL{0} {1} with QL{2} {3}. The result is a QL{4} {5}.", Source.Quality, SourceName, Target.Quality, TargetName, Cli.Character.getInventoryAt(placement).Item.Quality, ResultName));
                return true;
            }
            else
                return false;
        }

        private bool ValidateRange()
        {
            if (RangePercent != 0)
            {
                if (((decimal)Target.Quality - (decimal)Source.Quality) / (decimal)Target.Quality <= (decimal)RangePercent / 100M)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        private bool ValidateSkill(int skill, int skillvalue, int skillreq)
        {
            if (skill != 0)
            {
                if (skillvalue >= skillreq)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private string GetSkillError(int skillval, string skillname)
        {
            return (string.Format("It is theoretically possible to combine QL{0} {1} with QL{2} {3} but you need at least {4} in {5}.", Source.Quality, SourceName, Target.Quality, TargetName, skillval, skillname));
        }

        private void SetMaxQL()
        {
            int firstSkillBump;
            int secondSkillBump;
            int leastBump;

            if (FirstSkill != 0 && FirstSkillPerBump != 0)
            {
                firstSkillBump = (FirstSkillValue - FirstSkillRequirement) / FirstSkillPerBump;

                if (SecondSkill != 0 && SecondSkillPerBump != 0)
                {
                    secondSkillBump = (SecondSkillValue - SecondSkillRequirement) / SecondSkillPerBump;

                    leastBump = Math.Min(firstSkillBump, secondSkillBump);
                    leastBump = Math.Min(leastBump, MaxBump);
                }
                else
                {
                    leastBump = Math.Min(firstSkillBump, MaxBump);
                }
                MaxQL = Target.Quality + leastBump;
            }
            else
            {
                MaxQL = Target.Quality;
            }
        }

        private int GetSourceMinQL()
        {
            return (int)Math.Ceiling((decimal)Target.Quality - (decimal)RangePercent * (decimal)Target.Quality / 100M);
        }

        private void DeleteItem(int placement)
        {
            Packets.DeleteItem.Send(Cli.Character, 104, placement);
            Cli.Character.Inventory.Remove(Cli.Character.getInventoryAt(placement));
        }

        private int SpawnItem(int lowid, int highid, int ql)
        {
            // Copied from ChatCmd giveitem. 
            int firstfree = 64;
            firstfree = Cli.Character.GetNextFreeInventory(104);

            if (firstfree <= 93)
            {
                InventoryEntries ie = new InventoryEntries();
                AOItem item = ItemHandler.GetItemTemplate(Convert.ToInt32(lowid));
                ie.Placement = firstfree;
                ie.Container = 104;
                ie.Item.lowID = Convert.ToInt32(lowid);
                ie.Item.highID = Convert.ToInt32(highid);
                ie.Item.Quality = Convert.ToInt32(ql);
                if (item.ItemType != 1)
                {
                    ie.Item.multiplecount = Math.Max(1, (int)item.getItemAttribute(212));
                }
                else
                {
                    bool found = false;
                    foreach (AOItemAttribute a in ie.Item.Stats)
                    {
                        if (a.Stat != 212)
                            continue;
                        found = true;
                        a.Value = Math.Max(1, (int)item.getItemAttribute(212));
                        break;
                    }
                    if (!found)
                    {
                        AOItemAttribute aoi = new AOItemAttribute();
                        aoi.Stat = 212;
                        aoi.Value = Math.Max(1, (int)item.getItemAttribute(212));
                        ie.Item.Stats.Add(aoi);
                    }
                }
                Cli.Character.Inventory.Add(ie);
                Packets.AddTemplate.Send(Cli, ie);
                return firstfree;
            }
            else
            {
                // TODO: open overflow
                Cli.SendChatText("Your Inventory is full");
                return 0;
            }
        }
    }
}