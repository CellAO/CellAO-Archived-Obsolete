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

#region Usings

#endregion

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using AO.Core;

    using ZoneEngine.Packets;

    #region Tradeskill Class
    public class Tradeskill
    {
        #region SkillInfo Class
        public class SkillInfo
        {
            public int Skill { get; set; }

            public int Percent { get; set; }

            public int PerBump { get; set; }

            public int Value { get; set; }

            public int Requirement { get; set; }

            public string Name { get; set; }

            public SkillInfo(
                int skill, int skillpercent, int skillperbump, int skillvalue, int skillrequirement, string skillname)
            {
                this.Skill = skill;
                this.Percent = skillpercent;
                this.PerBump = skillperbump;
                this.Value = skillvalue;
                this.Requirement = skillrequirement;
                this.Name = skillname;
            }
        }
        #endregion

        #region ResultInfo Class
        public class ResultInfo
        {
            public int LowQL { get; set; }

            public int HighQL { get; set; }

            public int LowID { get; set; }

            public int HighID { get; set; }

            public ResultInfo(int lowql, int highql, int lowid, int highid)
            {
                this.LowQL = lowql;
                this.HighQL = highql;
                this.LowID = lowid;
                this.HighID = highid;
            }
        }
        #endregion

        #region Properties & Instance Variables
        // High IDs only.
        private int SourceID;

        private int TargetID;

        // For things like Tier armor, Engineer pistol etc... 0 = Don't check.
        private readonly int TargetMinQL;

        public int ResultLID { get; set; }

        public int ResultHID { get; set; }

        private readonly string SourceName;

        private readonly string TargetName;

        private readonly string ResultName;

        public List<SkillInfo> Skills;

        private readonly int MaxBump;

        // 0 = Do not check for range, 1 = Source must be greater than or equal to target, anything else is checked.
        private readonly int RangePercent;

        // Bit 0 = Delete source, Bit 1 = Delete target.
        private readonly int DeleteFlag;

        private readonly int MinXP;

        private readonly int MaxXP;

        // For the tradeskill window
        public int MinQL { get; set; }

        public int MaxQL { get; set; }

        private readonly Client Cli;

        private readonly int SourcePlacement;

        private readonly int TargetPlacement;

        private readonly AOItem Source;

        private readonly AOItem Target;

        public bool isTradeskill;

        private readonly bool isDeleteSource;

        private readonly bool isDeleteTarget;

        public int Quality { get; set; }

        public static Dictionary<int, string> ItemNames = new Dictionary<int, string>();

        private readonly List<ResultInfo> ResultProperties = new List<ResultInfo>();
        #endregion

        #region Constructor
        public Tradeskill(Client cli, int src_loc, int tgt_loc)
        {
            this.Cli = cli;
            this.SourcePlacement = src_loc;
            this.TargetPlacement = tgt_loc;
            this.Source = this.Cli.Character.getInventoryAt(src_loc).Item;
            this.Target = this.Cli.Character.getInventoryAt(tgt_loc).Item;

            this.SourceID = this.Source.highID;
            this.TargetID = this.Target.highID;

            this.isTradeskill = false;

            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt =
                wrapper.ReadDT(
                    "SELECT * FROM tradeskill WHERE ID1 = " + this.Source.highID + " AND ID2 = " + this.Target.highID
                    + ";");
            wrapper.Dispose();
            DataRowCollection drc = dt.Rows;

            if (drc.Count > 0)
            {
                this.isTradeskill = true;

                this.SourceName = GetItemName(this.Source.lowID, this.Source.highID, this.Source.Quality);
                this.TargetName = GetItemName(this.Target.lowID, this.Target.highID, this.Target.Quality);

                this.TargetMinQL = (int)drc[0][2];

                List<int> itemids = new List<int>();

                string[] ItemIDS = ((string)drc[0][3]).Split(',');
                foreach (string id in ItemIDS)
                {
                    itemids.Add(Convert.ToInt32(id.Trim()));
                }

                for (int i = 0; i < itemids.Count / 2; i++)
                {
                    int lowid = itemids.ElementAt(i * 2);
                    int highid = itemids.ElementAt(i * 2 + 1);
                    int lowql = ItemHandler.interpolate(lowid, highid, 1).Quality;
                    int highql = ItemHandler.interpolate(lowid, highid, 300).Quality;
                    this.ResultProperties.Add(new ResultInfo(lowql, highql, lowid, highid));
                }

                this.RangePercent = (int)drc[0][4];
                this.DeleteFlag = (int)drc[0][5];
                string skill = (string)drc[0][6];
                string skillpercent = (string)drc[0][7];
                string skillperbump = (string)drc[0][8];
                this.MaxBump = (int)drc[0][9];
                this.MinXP = (int)drc[0][10];
                this.MaxXP = (int)drc[0][11];
                int isImplant = (int)drc[0][12];

                this.isDeleteSource = ((this.DeleteFlag & 1) == 1);
                this.isDeleteTarget = (((this.DeleteFlag >> 1) & 1) == 1);

                string[] skills = skill.Split(',');
                string[] skillpercents = skillpercent.Split(',');
                string[] skillperbumps = skillperbump.Split(',');

                this.Skills = new List<SkillInfo>();

                if (skills[0] != string.Empty)
                {
                    for (int i = 0; i < skills.Count(); ++i)
                    {
                        if (skills[0].Trim() != string.Empty)
                        {
                            this.Skills.Add(
                                new SkillInfo(
                                    Convert.ToInt32(skills[i]),
                                    Convert.ToInt32(skillpercents[i]),
                                    Convert.ToInt32(skillperbumps[i]),
                                    this.Cli.Character.Stats.GetStatbyNumber(Convert.ToInt32(skills[i])).Value,
                                    (int)Math.Ceiling(Convert.ToInt32(skillpercents[i]) / 100M * this.Target.Quality),
                                    StatsList.GetStatName(Convert.ToInt32(skills[i]))));
                        }
                    }
                }

                int leastBump = 0;

                if (isImplant > 0)
                {
                    if (this.Target.Quality >= 250)
                    {
                        this.MaxBump = 5;
                    }
                    else if (this.Target.Quality >= 201)
                    {
                        this.MaxBump = 4;
                    }
                    else if (this.Target.Quality >= 150)
                    {
                        this.MaxBump = 3;
                    }
                    else if (this.Target.Quality >= 100)
                    {
                        this.MaxBump = 2;
                    }
                    else if (this.Target.Quality >= 50)
                    {
                        this.MaxBump = 1;
                    }
                    else
                    {
                        this.MaxBump = 0;
                    }
                }

                foreach (SkillInfo skillinfo in this.Skills)
                {
                    if (skillinfo.PerBump != 0)
                    {
                        leastBump = Math.Min(
                            (skillinfo.Value - skillinfo.Requirement) / skillinfo.PerBump, this.MaxBump);
                    }
                }

                this.MinQL = this.Target.Quality;
                this.MaxQL = Math.Min(
                    this.Target.Quality + leastBump,
                    ItemHandler.interpolate(
                        this.ResultProperties.ElementAt(this.ResultProperties.Count - 1).LowID,
                        this.ResultProperties.ElementAt(this.ResultProperties.Count - 1).HighID,
                        300).Quality);

                this.Quality = this.MaxQL;

                this.SetResultIDS(this.Quality);
                this.ResultName = GetItemName(this.ResultLID, this.ResultHID, this.Quality);
            }
        }
        #endregion

        #region Set Result Lo & Hi AOID
        private void SetResultIDS(int quality)
        {
            this.ResultLID =
                this.ResultProperties.Where(m => m.LowQL <= quality && m.HighQL >= quality).Select(m => m).ElementAt(0).
                    LowID;
            this.ResultHID =
                this.ResultProperties.Where(m => m.LowQL <= quality && m.HighQL >= quality).Select(m => m).ElementAt(0).
                    HighID;
        }
        #endregion

        #region Build Methods
        public bool ClickBuild()
        {
            if (this.isTradeskill)
            {
                if (this.ValidateTargetQL())
                {
                    if (this.ValidateRange())
                    {
                    }
                    else
                    {
                        if (this.srcHi)
                        {
                            this.Cli.SendChatText(this.SourceName + " must be at least " + this.Target.Quality + ".");
                            return false;
                        }
                        else
                        {
                            this.Cli.SendChatText(
                                string.Format(
                                    "\"{0}\" is of a too low quality level. With \"{3}\" at quality of {2} , the \"{0}\" must be at least at quality {1}.",
                                    this.SourceName,
                                    (int)
                                    Math.Ceiling(
                                        this.Target.Quality - this.RangePercent * (decimal)this.Target.Quality / 100M),
                                    this.Target.Quality,
                                    this.TargetName));
                            return false;
                        }
                    }
                }
                else
                {
                    this.Cli.SendChatText(
                        string.Format("\"{0}\" must be at least at quality {1}.", this.TargetName, this.TargetMinQL));
                    return false;
                }
            }
            else
            {
                this.Cli.SendChatText("It is not possible to assemble those two items. Maybe the order was wrong?");
                this.Cli.SendChatText("No combination found!");
                return false;
            }

            string lacking = string.Empty;
            bool isLacking = false;
            foreach (SkillInfo skillinfo in this.Skills)
            {
                if (!this.ValidateSkill(skillinfo.Skill, skillinfo.Value, skillinfo.Requirement))
                {
                    lacking +=
                        string.Format(
                            "It is theoretically possible to combine \"{0}\" with \"{1}\" but you need at least {2} in {3}.\n",
                            this.SourceName,
                            this.TargetName,
                            skillinfo.Requirement,
                            skillinfo.Name);
                    isLacking = true;
                }
            }
            if (isLacking)
            {
                lacking += "Combine failed!";
                this.Cli.SendChatText(lacking.Trim());
                return false;
            }

            int placement = this.SpawnItem();

            if (placement != 0)
            {
                if (this.isDeleteSource)
                {
                    this.DeleteItem(this.SourcePlacement);
                }
                if (this.isDeleteTarget)
                {
                    this.DeleteItem(this.TargetPlacement);
                }
                this.Cli.SendChatText(this.GetSuccessMsg(placement));

                int xp = this.CalculateXP();

                // TODO: GiveXP

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool WindowBuild()
        {
            if (this.isTradeskill)
            {
                if (this.ValidateTargetQL())
                {
                    if (this.ValidateRange())
                    {
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            foreach (SkillInfo skillinfo in this.Skills)
            {
                if (!this.ValidateSkill(skillinfo.Skill, skillinfo.Value, skillinfo.Requirement))
                {
                    return false;
                }
            }

            int placement = this.SpawnItem();

            if (placement != 0)
            {
                if (this.isDeleteSource)
                {
                    this.DeleteItem(this.SourcePlacement);
                }
                if (this.isDeleteTarget)
                {
                    this.DeleteItem(this.TargetPlacement);
                }
                this.Cli.SendChatText(this.GetSuccessMsg(placement));

                int xp = this.CalculateXP();

                // TODO: GiveXP

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Item Names
        public static string GetItemName(int lid, int hid, int ql)
        {
            try
            {
                string lName = ItemNames.Where(m => m.Key == lid).Select(m => m).ElementAt(0).Value;
                string hName = ItemNames.Where(m => m.Key == hid).Select(m => m).ElementAt(0).Value;

                int lQL = ItemHandler.interpolate(lid, hid, 1).Quality;
                int hQL = ItemHandler.interpolate(lid, hid, 300).Quality;

                if (ql > (hQL - lQL) / 2 + lQL)
                {
                    return hName;
                }
                else
                {
                    return lName;
                }
            }
            catch (Exception)
            {
                return "NoName";
            }
        }

        public static void CacheItemNames()
        {
            SqlWrapper wrap = new SqlWrapper();
            DataTable dt = wrap.ReadDT("SELECT * FROM itemnames");
            DataRowCollection drc = dt.Rows;

            foreach (DataRow row in drc)
            {
                ItemNames.Add(Convert.ToInt32(row[0]), row[1].ToString());
            }
        }
        #endregion

        #region Skill & QL Checks
        private bool srcHi;

        public int sMinQl;

        public bool ValidateRange()
        {
            if (this.RangePercent != 0)
            {
                if (this.RangePercent == 1)
                {
                    if (this.Source.Quality >= this.Target.Quality)
                    {
                        return true;
                    }
                    else
                    {
                        this.sMinQl = this.Target.Quality;
                        this.srcHi = true;
                        return false;
                    }
                }
                if ((this.Target.Quality - (decimal)this.Source.Quality) / this.Target.Quality
                    <= this.RangePercent / 100M)
                {
                    return true;
                }
                else
                {
                    this.sMinQl = this.Target.Quality - this.RangePercent * this.Target.Quality / 100;
                    return false;
                }
            }
            else
            {
                return true;
            }
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

        private bool ValidateTargetQL()
        {
            if (this.TargetMinQL >= this.Target.Quality || this.TargetMinQL == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Feedback Messages
        private string GetSuccessMsg(int placement)
        {
            return string.Format(
                "You combined \"{0}\" with \"{1}\" and the result is a quality level {2} \"{3}\".",
                this.SourceName,
                this.TargetName,
                this.Cli.Character.getInventoryAt(placement).Item.Quality,
                this.ResultName);
        }

        public static int GetSourceProcessesCount(int id)
        {
            int count = 0;
            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt = wrapper.ReadDT("SELECT * FROM tradeskill WHERE ID1 = " + id + ";");
            DataRowCollection drc = dt.Rows;
            count += drc.Count;
            wrapper.Dispose();

            return count;
        }

        public static int GetTargetProcessesCount(int id)
        {
            int count = 0;
            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt = wrapper.ReadDT("SELECT * FROM tradeskill WHERE ID2 = " + id + ";");
            DataRowCollection drc = dt.Rows;
            count += drc.Count;
            wrapper.Dispose();

            return count;
        }

        public string GetFeedbackMsg()
        {
            bool isLacking = false;
            foreach (SkillInfo skillinfo in this.Skills)
            {
                if (!this.ValidateSkill(skillinfo.Skill, skillinfo.Value, skillinfo.Requirement))
                {
                    isLacking = true;
                }
            }

            if (!this.ValidateRange())
            {
                string s = string.Empty;
                s += string.Format(
                    "The {0} must be at least quality level {1} to combine with the {2} level {3}.",
                    this.SourceName,
                    (int)Math.Ceiling(this.Target.Quality - this.RangePercent * (decimal)this.Target.Quality / 100M),
                    this.TargetName,
                    this.Target.Quality);
                return s;
            }
            else if (isLacking)
            {
                string s = string.Empty;
                foreach (SkillInfo skillinfo in this.Skills)
                {
                    s += "You need at least " + skillinfo.Requirement + " in " + skillinfo.Name
                         + " to combine these two items.\n\n";
                    s += "Your skill in " + skillinfo.Name + " is " + skillinfo.Value + "\n\n";
                }
                return s;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region Spawn / Delete items
        private int SpawnItem()
        {
            int firstfree = 64;
            firstfree = this.Cli.Character.GetNextFreeInventory(104);
            if (firstfree <= 93)
            {
                InventoryEntries mi = new InventoryEntries();
                AOItem it = ItemHandler.GetItemTemplate(Convert.ToInt32(this.ResultLID));
                mi.Placement = firstfree;
                mi.Container = 104;
                mi.Item.lowID = Convert.ToInt32(this.ResultLID);
                mi.Item.highID = Convert.ToInt32(this.ResultHID);
                mi.Item.Quality = Convert.ToInt32(this.Quality);
                if (it.ItemType != 1)
                {
                    mi.Item.multiplecount = Math.Max(1, it.getItemAttribute(212));
                }
                else
                {
                    bool found = false;
                    foreach (AOItemAttribute a in mi.Item.Stats)
                    {
                        if (a.Stat != 212)
                        {
                            continue;
                        }
                        found = true;
                        a.Value = Math.Max(1, it.getItemAttribute(212));
                        break;
                    }
                    if (!found)
                    {
                        AOItemAttribute aoi = new AOItemAttribute();
                        aoi.Stat = 212;
                        aoi.Value = Math.Max(1, it.getItemAttribute(212));
                        mi.Item.Stats.Add(aoi);
                    }
                }
                this.Cli.Character.Inventory.Add(mi);
                AddTemplate.Send(this.Cli, mi);

                return firstfree;
            }
            else
            {
                this.Cli.SendChatText("Your Inventory is full");
                return 0;
            }
        }

        private void DeleteItem(int placement)
        {
            Packets.DeleteItem.Send(this.Cli.Character, 104, placement);
            this.Cli.Character.Inventory.Remove(this.Cli.Character.getInventoryAt(placement));
        }
        #endregion

        #region CalculateXP
        private int CalculateXP()
        {
            int absMinQL = ItemHandler.interpolate(this.ResultLID, this.ResultHID, 1).Quality;
            int absMaxQL = ItemHandler.interpolate(this.ResultLID, this.ResultHID, 300).Quality;

            if (absMaxQL == absMinQL)
            {
                return this.MaxXP;
            }
            else
            {
                return ((this.MaxXP - this.MinXP) / (absMaxQL - absMinQL)) * (this.Quality - absMinQL) + this.MinXP;
            }
        }
        #endregion
    }
    #endregion

    #region TSReceiver Class
    public static class TSReceiver
    {
        #region TSInfo Class
        public class TSInfo
        {
            public Client Cli { get; set; }

            public int Location { get; set; }

            public int Container { get; set; }

            public int Placement { get; set; }

            public TSInfo(Client cli, int location, int container, int placement)
            {
                this.Cli = cli;
                this.Location = location;
                this.Container = container;
                this.Placement = placement;
            }
        }
        #endregion

        #region Properties & Instance Variables
        private static readonly List<TSInfo> tsInfo = new List<TSInfo>();
        #endregion

        #region Packet Handlers
        public static void TSSourceChanged(Client client, int container, int placement)
        {
            if (container != 0 && placement != 0)
            {
                tsInfo.Add(new TSInfo(client, 0, container, placement));

                AOItem it = client.Character.getInventoryAt(placement).Item;

                TradeskillPacket.SendSource(client.Character, Tradeskill.GetSourceProcessesCount(it.highID));

                var l1 = tsInfo.Where(m => m.Cli == client && m.Location == 0).Select(m => m);
                var l2 = tsInfo.Where(m => m.Cli == client && m.Location == 1).Select(m => m);

                if (l1.Count() == 1 && l2.Count() == 1)
                {
                    TSInfo info1 = l1.ElementAt(0);
                    TSInfo info2 = l2.ElementAt(0);

                    Tradeskill ts = new Tradeskill(client, info1.Placement, info2.Placement);

                    if (ts.isTradeskill)
                    {
                        if (ts.ValidateRange())
                        {
                            foreach (Tradeskill.SkillInfo si in ts.Skills)
                            {
                                TradeskillPacket.SendRequirement(client.Character, si);
                            }
                            TradeskillPacket.SendResult(
                                client.Character, ts.MinQL, ts.MaxQL, ts.ResultLID, ts.ResultHID);
                        }
                        else
                        {
                            TradeskillPacket.SendOutOfRange(client.Character, ts.sMinQl);
                        }
                    }
                    else
                    {
                        TradeskillPacket.SendNotTradeskill(client.Character);
                    }
                }
            }
            else if (container == 0 && placement == 0)
            {
                tsInfo.RemoveAll(m => (m.Cli == client) && (m.Location == 0));
            }
        }

        public static void TSTargetChanged(Client client, int container, int placement)
        {
            if (container != 0 && placement != 0)
            {
                tsInfo.Add(new TSInfo(client, 1, container, placement));

                AOItem it = client.Character.getInventoryAt(placement).Item;

                TradeskillPacket.SendTarget(client.Character, Tradeskill.GetTargetProcessesCount(it.highID));

                var l1 = tsInfo.Where(m => m.Cli == client && m.Location == 0).Select(m => m);
                var l2 = tsInfo.Where(m => m.Cli == client && m.Location == 1).Select(m => m);

                if (l1.Count() == 1 && l2.Count() == 1)
                {
                    TSInfo info1 = l1.ElementAt(0);
                    TSInfo info2 = l2.ElementAt(0);

                    Tradeskill ts = new Tradeskill(client, info1.Placement, info2.Placement);

                    if (ts.isTradeskill)
                    {
                        if (ts.ValidateRange())
                        {
                            foreach (Tradeskill.SkillInfo si in ts.Skills)
                            {
                                TradeskillPacket.SendRequirement(client.Character, si);
                            }
                            TradeskillPacket.SendResult(
                                client.Character, ts.MinQL, ts.MaxQL, ts.ResultLID, ts.ResultHID);
                        }
                        else
                        {
                            TradeskillPacket.SendOutOfRange(client.Character, ts.sMinQl);
                        }
                    }
                    else
                    {
                        TradeskillPacket.SendNotTradeskill(client.Character);
                    }
                }
            }
            else if (container == 0 && placement == 0)
            {
                tsInfo.RemoveAll(m => (m.Cli == client) && (m.Location == 1));
            }
        }

        public static void TSBuildPressed(Client client, int quality)
        {
            int src = tsInfo.Where(m => m.Cli == client && m.Location == 0).Select(m => m).ElementAt(0).Placement;
            int tgt = tsInfo.Where(m => m.Cli == client && m.Location == 1).Select(m => m).ElementAt(0).Placement;

            Tradeskill ts = new Tradeskill(client, src, tgt);

            if (ts.isTradeskill)
            {
                ts.Quality = quality;
                ts.WindowBuild();
            }
            else
            {
                client.SendChatText("It is not possible to assemble those two items. Maybe the order was wrong?");
                client.SendChatText("No combination found!");
            }
            tsInfo.RemoveAll(m => m.Cli == client);
        }
        #endregion
    }
    #endregion
}