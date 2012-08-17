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

        #region Properties & Instance Variables
        // High IDs only.
        private int SourceID;

        private int TargetID;

        // For things like Tier armor, Engineer pistol etc... 0 = Don't check.
        private readonly int TargetMinQL;

        public int ResultLowId { get; set; }

        public int ResultHighId { get; set; }

        private readonly string sourceName;

        private readonly string targetName;

        private readonly string resultName;

        public List<TradeSkillSkillInfo> Skills;

        private readonly int maxBump;

        // 0 = Do not check for range, 1 = Source must be greater than or equal to target, anything else is checked.
        private readonly int rangePercent;

        // Bit 0 = Delete source, Bit 1 = Delete target.
        private readonly int deleteFlag;

        private readonly int minXP;

        private readonly int maxXP;

        // For the tradeskill window
        public int MinQL { get; set; }

        public int MaxQL { get; set; }

        private readonly Client client;

        private readonly int sourcePlacement;

        private readonly int targetPlacement;

        private readonly AOItem source;

        private readonly AOItem target;

        public bool IsTradeSkill { get; set; }

        private readonly bool isDeleteSource;

        private readonly bool isDeleteTarget;

        public int Quality { get; set; }

        public static Dictionary<int, string> ItemNames = new Dictionary<int, string>();

        private readonly List<TradeSkillResultInfo> resultProperties = new List<TradeSkillResultInfo>();
        #endregion

        #region Constructor
        public Tradeskill(Client client, int srcLocation, int targetLocation)
        {
            this.client = client;
            this.sourcePlacement = srcLocation;
            this.targetPlacement = targetLocation;
            this.source = this.client.Character.GetInventoryAt(srcLocation).Item;
            this.target = this.client.Character.GetInventoryAt(targetLocation).Item;

            this.SourceID = this.source.HighID;
            this.TargetID = this.target.HighID;

            this.IsTradeSkill = false;

            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt =
                wrapper.ReadDatatable(
                    "SELECT * FROM tradeskill WHERE ID1 = " + this.source.HighID + " AND ID2 = " + this.target.HighID
                    + ";");
            wrapper.Dispose();
            DataRowCollection drc = dt.Rows;

            if (drc.Count > 0)
            {
                this.IsTradeSkill = true;

                this.sourceName = GetItemName(this.source.LowID, this.source.HighID, this.source.Quality);
                this.targetName = GetItemName(this.target.LowID, this.target.HighID, this.target.Quality);

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
                    this.resultProperties.Add(new TradeSkillResultInfo(lowql, highql, lowid, highid));
                }

                this.rangePercent = (int)drc[0][4];
                this.deleteFlag = (int)drc[0][5];
                string skill = (string)drc[0][6];
                string skillpercent = (string)drc[0][7];
                string skillperbump = (string)drc[0][8];
                this.maxBump = (int)drc[0][9];
                this.minXP = (int)drc[0][10];
                this.maxXP = (int)drc[0][11];
                int isImplant = (int)drc[0][12];

                this.isDeleteSource = ((this.deleteFlag & 1) == 1);
                this.isDeleteTarget = (((this.deleteFlag >> 1) & 1) == 1);

                string[] skills = skill.Split(',');
                string[] skillpercents = skillpercent.Split(',');
                string[] skillperbumps = skillperbump.Split(',');

                this.Skills = new List<TradeSkillSkillInfo>();

                if (skills[0] != string.Empty)
                {
                    for (int i = 0; i < skills.Count(); ++i)
                    {
                        if (skills[0].Trim() != string.Empty)
                        {
                            this.Skills.Add(
                                new TradeSkillSkillInfo(
                                    Convert.ToInt32(skills[i]),
                                    Convert.ToInt32(skillpercents[i]),
                                    Convert.ToInt32(skillperbumps[i]),
                                    this.client.Character.Stats.GetStatbyNumber(Convert.ToInt32(skills[i])).Value,
                                    (int)Math.Ceiling(Convert.ToInt32(skillpercents[i]) / 100M * this.target.Quality),
                                    StatsList.GetStatName(Convert.ToInt32(skills[i]))));
                        }
                    }
                }

                int leastBump = 0;

                if (isImplant > 0)
                {
                    if (this.target.Quality >= 250)
                    {
                        this.maxBump = 5;
                    }
                    else if (this.target.Quality >= 201)
                    {
                        this.maxBump = 4;
                    }
                    else if (this.target.Quality >= 150)
                    {
                        this.maxBump = 3;
                    }
                    else if (this.target.Quality >= 100)
                    {
                        this.maxBump = 2;
                    }
                    else if (this.target.Quality >= 50)
                    {
                        this.maxBump = 1;
                    }
                    else
                    {
                        this.maxBump = 0;
                    }
                }

                foreach (TradeSkillSkillInfo skillinfo in this.Skills)
                {
                    if (skillinfo.PerBump != 0)
                    {
                        leastBump = Math.Min(
                            (skillinfo.Value - skillinfo.Requirement) / skillinfo.PerBump, this.maxBump);
                    }
                }

                this.MinQL = this.target.Quality;
                this.MaxQL = Math.Min(
                    this.target.Quality + leastBump,
                    ItemHandler.interpolate(
                        this.resultProperties.ElementAt(this.resultProperties.Count - 1).LowID,
                        this.resultProperties.ElementAt(this.resultProperties.Count - 1).HighID,
                        300).Quality);

                this.Quality = this.MaxQL;

                this.SetResultIDS(this.Quality);
                this.resultName = GetItemName(this.ResultLowId, this.ResultHighId, this.Quality);
            }
        }
        #endregion

        #region Set Result Lo & Hi AOID
        private void SetResultIDS(int quality)
        {
            this.ResultLowId =
                this.resultProperties.Where(m => m.LowQL <= quality && m.HighQL >= quality).Select(m => m).ElementAt(0).
                    LowID;
            this.ResultHighId =
                this.resultProperties.Where(m => m.LowQL <= quality && m.HighQL >= quality).Select(m => m).ElementAt(0).
                    HighID;
        }
        #endregion

        #region Build Methods
        public bool ClickBuild()
        {
            if (this.IsTradeSkill)
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
                            this.client.SendChatText(this.sourceName + " must be at least " + this.target.Quality + ".");
                            return false;
                        }
                        else
                        {
                            this.client.SendChatText(
                                string.Format(
                                    "\"{0}\" is of a too low quality level. With \"{3}\" at quality of {2} , the \"{0}\" must be at least at quality {1}.",
                                    this.sourceName,
                                    (int)
                                    Math.Ceiling(
                                        this.target.Quality - this.rangePercent * (decimal)this.target.Quality / 100M),
                                    this.target.Quality,
                                    this.targetName));
                            return false;
                        }
                    }
                }
                else
                {
                    this.client.SendChatText(
                        string.Format("\"{0}\" must be at least at quality {1}.", this.targetName, this.TargetMinQL));
                    return false;
                }
            }
            else
            {
                this.client.SendChatText("It is not possible to assemble those two items. Maybe the order was wrong?");
                this.client.SendChatText("No combination found!");
                return false;
            }

            string lacking = string.Empty;
            bool isLacking = false;
            foreach (TradeSkillSkillInfo skillinfo in this.Skills)
            {
                if (!this.ValidateSkill(skillinfo.Skill, skillinfo.Value, skillinfo.Requirement))
                {
                    lacking +=
                        string.Format(
                            "It is theoretically possible to combine \"{0}\" with \"{1}\" but you need at least {2} in {3}.\n",
                            this.sourceName,
                            this.targetName,
                            skillinfo.Requirement,
                            skillinfo.Name);
                    isLacking = true;
                }
            }
            if (isLacking)
            {
                lacking += "Combine failed!";
                this.client.SendChatText(lacking.Trim());
                return false;
            }

            int placement = this.SpawnItem();

            if (placement != 0)
            {
                if (this.isDeleteSource)
                {
                    this.DeleteItem(this.sourcePlacement);
                }
                if (this.isDeleteTarget)
                {
                    this.DeleteItem(this.targetPlacement);
                }
                this.client.SendChatText(this.GetSuccessMsg(placement));

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
            if (this.IsTradeSkill)
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

            foreach (TradeSkillSkillInfo skillinfo in this.Skills)
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
                    this.DeleteItem(this.sourcePlacement);
                }
                if (this.isDeleteTarget)
                {
                    this.DeleteItem(this.targetPlacement);
                }
                this.client.SendChatText(this.GetSuccessMsg(placement));

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
            DataTable dt = wrap.ReadDatatable("SELECT * FROM itemnames");
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
            if (this.rangePercent != 0)
            {
                if (this.rangePercent == 1)
                {
                    if (this.source.Quality >= this.target.Quality)
                    {
                        return true;
                    }
                    else
                    {
                        this.sMinQl = this.target.Quality;
                        this.srcHi = true;
                        return false;
                    }
                }
                if ((this.target.Quality - (decimal)this.source.Quality) / this.target.Quality
                    <= this.rangePercent / 100M)
                {
                    return true;
                }
                else
                {
                    this.sMinQl = this.target.Quality - this.rangePercent * this.target.Quality / 100;
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
            if (this.TargetMinQL >= this.target.Quality || this.TargetMinQL == 0)
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
                this.sourceName,
                this.targetName,
                this.client.Character.GetInventoryAt(placement).Item.Quality,
                this.resultName);
        }

        public static int GetSourceProcessesCount(int id)
        {
            int count = 0;
            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt = wrapper.ReadDatatable("SELECT * FROM tradeskill WHERE ID1 = " + id + ";");
            DataRowCollection drc = dt.Rows;
            count += drc.Count;
            wrapper.Dispose();

            return count;
        }

        public static int GetTargetProcessesCount(int id)
        {
            int count = 0;
            SqlWrapper wrapper = new SqlWrapper();
            DataTable dt = wrapper.ReadDatatable("SELECT * FROM tradeskill WHERE ID2 = " + id + ";");
            DataRowCollection drc = dt.Rows;
            count += drc.Count;
            wrapper.Dispose();

            return count;
        }

        public string GetFeedbackMsg()
        {
            bool isLacking = false;
            foreach (TradeSkillSkillInfo skillinfo in this.Skills)
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
                    this.sourceName,
                    (int)Math.Ceiling(this.target.Quality - this.rangePercent * (decimal)this.target.Quality / 100M),
                    this.targetName,
                    this.target.Quality);
                return s;
            }
            else if (isLacking)
            {
                string s = string.Empty;
                foreach (TradeSkillSkillInfo skillinfo in this.Skills)
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
            firstfree = this.client.Character.GetNextFreeInventory(104);
            if (firstfree <= 93)
            {
                InventoryEntries mi = new InventoryEntries();
                AOItem it = ItemHandler.GetItemTemplate(Convert.ToInt32(this.ResultLowId));
                mi.Placement = firstfree;
                mi.Container = 104;
                mi.Item.LowID = Convert.ToInt32(this.ResultLowId);
                mi.Item.HighID = Convert.ToInt32(this.ResultHighId);
                mi.Item.Quality = Convert.ToInt32(this.Quality);
                if (it.ItemType != 1)
                {
                    mi.Item.MultipleCount = Math.Max(1, it.getItemAttribute(212));
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
                this.client.Character.Inventory.Add(mi);
                AddTemplate.Send(this.client, mi);

                return firstfree;
            }
            else
            {
                this.client.SendChatText("Your Inventory is full");
                return 0;
            }
        }

        private void DeleteItem(int placement)
        {
            Packets.DeleteItem.Send(this.client.Character, 104, placement);
            this.client.Character.Inventory.Remove(this.client.Character.GetInventoryAt(placement));
        }
        #endregion

        #region CalculateXP
        private int CalculateXP()
        {
            int absMinQL = ItemHandler.interpolate(this.ResultLowId, this.ResultHighId, 1).Quality;
            int absMaxQL = ItemHandler.interpolate(this.ResultLowId, this.ResultHighId, 300).Quality;

            if (absMaxQL == absMinQL)
            {
                return this.maxXP;
            }
            else
            {
                return ((this.maxXP - this.minXP) / (absMaxQL - absMinQL)) * (this.Quality - absMinQL) + this.minXP;
            }
        }
        #endregion
    }
    #endregion

    #region TSReceiver Class
    public static class TradeSkillReceiver
    {

        #region Properties & Instance Variables
        private static readonly List<TradeSkillInfo> TradeSkillInfos = new List<TradeSkillInfo>();
        #endregion

        #region Packet Handlers
        public static void TradeSkillSourceChanged(Client client, int container, int placement)
        {
            if (container != 0 && placement != 0)
            {
                TradeSkillInfos.Add(new TradeSkillInfo(client, 0, container, placement));

                AOItem it = client.Character.GetInventoryAt(placement).Item;

                TradeskillPacket.SendSource(client.Character, Tradeskill.GetSourceProcessesCount(it.HighID));

                var l1 = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 0).Select(m => m);
                var l2 = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 1).Select(m => m);

                if (l1.Count() == 1 && l2.Count() == 1)
                {
                    TradeSkillInfo info1 = l1.ElementAt(0);
                    TradeSkillInfo info2 = l2.ElementAt(0);

                    Tradeskill ts = new Tradeskill(client, info1.Placement, info2.Placement);

                    if (ts.IsTradeSkill)
                    {
                        if (ts.ValidateRange())
                        {
                            foreach (TradeSkillSkillInfo si in ts.Skills)
                            {
                                TradeskillPacket.SendRequirement(client.Character, si);
                            }
                            TradeskillPacket.SendResult(
                                client.Character, ts.MinQL, ts.MaxQL, ts.ResultLowId, ts.ResultHighId);
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
                TradeSkillInfos.RemoveAll(m => (m.Cli == client) && (m.Location == 0));
            }
        }

        public static void TradeSkillTargetChanged(Client client, int container, int placement)
        {
            if (container != 0 && placement != 0)
            {
                TradeSkillInfos.Add(new TradeSkillInfo(client, 1, container, placement));

                AOItem it = client.Character.GetInventoryAt(placement).Item;

                TradeskillPacket.SendTarget(client.Character, Tradeskill.GetTargetProcessesCount(it.HighID));

                var l1 = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 0).Select(m => m);
                var l2 = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 1).Select(m => m);

                if (l1.Count() == 1 && l2.Count() == 1)
                {
                    TradeSkillInfo info1 = l1.ElementAt(0);
                    TradeSkillInfo info2 = l2.ElementAt(0);

                    Tradeskill ts = new Tradeskill(client, info1.Placement, info2.Placement);

                    if (ts.IsTradeSkill)
                    {
                        if (ts.ValidateRange())
                        {
                            foreach (TradeSkillSkillInfo si in ts.Skills)
                            {
                                TradeskillPacket.SendRequirement(client.Character, si);
                            }
                            TradeskillPacket.SendResult(
                                client.Character, ts.MinQL, ts.MaxQL, ts.ResultLowId, ts.ResultHighId);
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
                TradeSkillInfos.RemoveAll(m => (m.Cli == client) && (m.Location == 1));
            }
        }

        public static void TradeSkillBuildPressed(Client client, int quality)
        {
            int src = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 0).Select(m => m).ElementAt(0).Placement;
            int tgt = TradeSkillInfos.Where(m => m.Cli == client && m.Location == 1).Select(m => m).ElementAt(0).Placement;

            Tradeskill ts = new Tradeskill(client, src, tgt);

            if (ts.IsTradeSkill)
            {
                ts.Quality = quality;
                ts.WindowBuild();
            }
            else
            {
                client.SendChatText("It is not possible to assemble those two items. Maybe the order was wrong?");
                client.SendChatText("No combination found!");
            }
            TradeSkillInfos.RemoveAll(m => m.Cli == client);
        }
        #endregion
    }
    #endregion

    #region TSInfo Class
    public class TradeSkillInfo
    {
        public Client Cli { get; set; }

        public int Location { get; set; }

        public int Container { get; set; }

        public int Placement { get; set; }

        public TradeSkillInfo(Client cli, int location, int container, int placement)
        {
            this.Cli = cli;
            this.Location = location;
            this.Container = container;
            this.Placement = placement;
        }
    }
    #endregion

    #region SkillInfo Class
    public class TradeSkillSkillInfo
    {
        public int Skill { get; set; }

        public int Percent { get; set; }

        public int PerBump { get; set; }

        public int Value { get; set; }

        public int Requirement { get; set; }

        public string Name { get; set; }

        public TradeSkillSkillInfo(
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
    public class TradeSkillResultInfo
    {
        public int LowQL { get; set; }

        public int HighQL { get; set; }

        public int LowID { get; set; }

        public int HighID { get; set; }

        public TradeSkillResultInfo(int lowql, int highql, int lowid, int highid)
        {
            this.LowQL = lowql;
            this.HighQL = highql;
            this.LowID = lowid;
            this.HighID = highid;
        }
    }
    #endregion

}