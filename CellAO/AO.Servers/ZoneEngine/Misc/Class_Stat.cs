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
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using ZoneEngine.Packets;

    #region Class_Stat  class for one stat
    public class Class_Stat
    {
        #region StatChangedEventArgs
        /// <summary>
        /// Event Arguments for changed stats
        /// </summary>
        public class StatChangedEventArgs : EventArgs
        {
            public StatChangedEventArgs(Class_Stat Stat, uint Old_Value, uint New_Value, bool announce)
            {
                this.stat = Stat;
                this.oldvalue = Old_Value;
                this.newvalue = New_Value;
                this.AnnounceToPlayfield = announce;
            }

            private readonly Class_Stat stat;

            public uint oldvalue;

            public uint newvalue;

            public bool AnnounceToPlayfield;

            public Class_Stat Stat
            {
                get
                {
                    return this.stat;
                }
            }

            public uint Oldvalue
            {
                get
                {
                    return this.oldvalue;
                }
                set
                {
                    this.oldvalue = value;
                }
            }

            public Dynel Parent
            {
                get
                {
                    return this.stat.Parent;
                }
            }
        }
        #endregion

        #region Eventhandlers
        public event EventHandler<StatChangedEventArgs> RaiseBeforeStatChangedEvent;

        public event EventHandler<StatChangedEventArgs> RaiseAfterStatChangedEvent;

        protected virtual void OnBeforeStatChangedEvent(StatChangedEventArgs e)
        {
            EventHandler<StatChangedEventArgs> handler = this.RaiseBeforeStatChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterStatChangedEvent(StatChangedEventArgs e)
        {
            EventHandler<StatChangedEventArgs> handler = this.RaiseAfterStatChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public int StatNumber;

        public virtual int Value
        {
            get
            {
                return
                    (int)
                    Math.Floor(
                        (double)
                        ((this.StatBaseValue + this.StatModifier + this.Trickle) * this.StatPercentageModifier / 100));
            }
            set
            {
                Set(value);
            }
        }

        public uint StatDefault;

        public uint StatBaseValue;

        public int StatPercentageModifier = 100; // From Items/Perks/Nanos

        public int StatModifier; // From Items/Perks/Nanos

        public int Trickle; // From Attributes (Strength, Stamina, Sense, Agility, Intelligence, Psychic)

        public bool AnnounceToPlayfield = true;

        public bool DoNotDontWriteToSql;

        public bool SendBaseValue = true;

        public bool changed;

        public List<int> Affects = new List<int>();

        public Dynel Parent;

        public Class_Stat(
            int Number, uint Default, string name, bool sendbase, bool dontwrite, bool announcetoplayfield)
        {
            this.DoNotDontWriteToSql = true;
            this.StatNumber = Number;
            this.StatDefault = Default;
            this.Value = (int)Default;
            this.StatDefault = Default;
            this.SendBaseValue = sendbase;
            this.DoNotDontWriteToSql = dontwrite;
            this.AnnounceToPlayfield = announcetoplayfield;
            // Obsolete            StatName = name;
        }

        public Class_Stat()
        {
        }

        /// <summary>
        /// Calculate trickle value (prototype)
        /// </summary>
        public virtual void CalcTrickle()
        {
            if (!this.Parent.startup)
            {
                this.AffectStats();
            }
        }

        public virtual uint GetMaxValue(uint val)
        {
            return val;
        }

        public void Set(uint value)
        {
            if ((this.Parent == null) || (this.Parent.startup))
            {
                this.StatBaseValue = value;
                return;
            }
            if (value != this.StatBaseValue)
            {
                uint oldvalue = (uint)this.Value;
                uint max = this.GetMaxValue(value);
                this.OnBeforeStatChangedEvent(new StatChangedEventArgs(this, oldvalue, max, this.AnnounceToPlayfield));
                this.StatBaseValue = max;
                this.OnAfterStatChangedEvent(new StatChangedEventArgs(this, oldvalue, max, this.AnnounceToPlayfield));
                this.changed = true;
                this.WriteStatToSQL();

                if (!this.Parent.startup)
                {
                    this.AffectStats();
                }
            }
        }

        public void Set(int value)
        {
            this.Set((uint)value);
        }

        public void SetParent(Dynel parent)
        {
            this.Parent = parent;
        }

        #region read and write to SQL
        /// <summary>
        /// Write Stat to SQL
        /// </summary>
        public void WriteStatToSQL()
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }
            int id = this.Parent.ID;
            SqlWrapper sql = new SqlWrapper();
            if (this.changed)
            {
                if (this.Parent is NonPC)
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType()
                        + "_stats (ID, Playfield, Stat, Value) VALUES (" + id.ToString() + ","
                        + this.Parent.PlayField.ToString() + "," + this.StatNumber.ToString() + ","
                        + ((Int32)this.StatBaseValue).ToString() + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue).ToString() + ";");
                }
                else
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType() + "_stats (ID, Stat, Value) VALUES ("
                        + id.ToString() + "," + this.StatNumber.ToString() + ","
                        + ((Int32)this.StatBaseValue).ToString() + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue).ToString() + ";");
                }
            }
        }

        /// <summary>
        /// Write Stat to SQL
        /// </summary>
        public void WriteStatToSQL(bool doit)
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }
            int id = this.Parent.ID;
            SqlWrapper sql = new SqlWrapper();
            if (doit)
            {
                if (this.Parent is NonPC)
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType()
                        + "_stats (ID, Playfield, Stat, Value) VALUES (" + id.ToString() + "," + this.Parent.PlayField
                        + "," + this.StatNumber.ToString() + "," + ((Int32)this.StatBaseValue).ToString()
                        + ") ON DUPLICATE KEY UPDATE Value=" + ((Int32)this.StatBaseValue).ToString() + ";");
                }
                else
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType() + "_stats (ID, Stat, Value) VALUES ("
                        + id.ToString() + "," + this.StatNumber.ToString() + ","
                        + ((Int32)this.StatBaseValue).ToString() + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue).ToString() + ";");
                }
            }
        }

        /// <summary>
        /// Read stat from SQL
        /// </summary>
        public void ReadStatFromSQL()
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }
            SqlWrapper sql = new SqlWrapper();
            int id = this.Parent.ID;
            DataTable dt =
                sql.ReadDT(
                    "SELECT Value FROM " + this.Parent.getSQLTablefromDynelType() + " WHERE ID=" + id.ToString()
                    + " AND Stat=" + this.StatNumber.ToString() + ";");

            if (dt.Rows.Count > 0)
            {
                this.Value = (Int32)dt.Rows[0][0];
            }
        }
        #endregion

        #region Call Stats affected by this stat
        public void AffectStats()
        {
            if (!(this.Parent is Character) && !(this.Parent is NonPC))
            {
                return;
            }
            foreach (int c in this.Affects)
            {
                ((Character)this.Parent).Stats.GetStatbyNumber(c).CalcTrickle();
            }
        }
        #endregion
    }
    #endregion

    #region Character_Stats holder for Character's stats
    public class Character_Stats
    {
        #region stats creation
        public Class_Stat Flags = new Class_Stat(0, 8917569, "Flags", false, false, true);

        public Stat_Health Life = new Stat_Health(1, 1, "Life", true, false, false);

        public Class_Stat VolumeMass = new Class_Stat(2, 1234567890, "VolumeMass", false, false, false);

        public Class_Stat AttackSpeed = new Class_Stat(3, 5, "AttackSpeed", false, false, false);

        public Class_Stat Breed = new Class_Stat(4, 1234567890, "Breed", false, false, false);

        public Class_Stat Clan = new Class_Stat(5, 0, "Clan", false, false, false);

        public Class_Stat Team = new Class_Stat(6, 0, "Team", false, false, false);

        public Class_Stat State = new Class_Stat(7, 0, "State", false, false, false);

        public Class_Stat TimeExist = new Class_Stat(8, 1234567890, "TimeExist", false, false, false);

        public Class_Stat MapFlags = new Class_Stat(9, 0, "MapFlags", false, false, false);

        public Class_Stat ProfessionLevel = new Class_Stat(10, 1234567890, "ProfessionLevel", false, true, false);

        public Class_Stat PreviousHealth = new Class_Stat(11, 50, "PreviousHealth", false, false, false);

        public Class_Stat Mesh = new Class_Stat(12, 17530, "Mesh", false, false, false);

        public Class_Stat Anim = new Class_Stat(13, 1234567890, "Anim", false, false, false);

        public Class_Stat Name = new Class_Stat(14, 1234567890, "Name", false, false, false);

        public Class_Stat Info = new Class_Stat(15, 1234567890, "Info", false, false, false);

        public Class_Stat Strength = new Class_Stat(16, 0, "Strength", true, false, false);

        public Class_Stat Agility = new Class_Stat(17, 0, "Agility", true, false, false);

        public Class_Stat Stamina = new Class_Stat(18, 0, "Stamina", true, false, false);

        public Class_Stat Intelligence = new Class_Stat(19, 0, "Intelligence", true, false, false);

        public Class_Stat Sense = new Class_Stat(20, 0, "Sense", true, false, false);

        public Class_Stat Psychic = new Class_Stat(21, 0, "Psychic", true, false, false);

        public Class_Stat AMS = new Class_Stat(22, 1234567890, "AMS", false, false, false);

        public Class_Stat StaticInstance = new Class_Stat(23, 1234567890, "StaticInstance", false, false, false);

        public Class_Stat MaxMass = new Class_Stat(24, 1234567890, "MaxMass", false, false, false);

        public Class_Stat StaticType = new Class_Stat(25, 1234567890, "StaticType", false, false, false);

        public Class_Stat Energy = new Class_Stat(26, 1234567890, "Energy", false, false, false);

        public Stat_HP Health = new Stat_HP(27, 1, "Health", false, false, false);

        public Class_Stat Height = new Class_Stat(28, 1234567890, "Height", false, false, false);

        public Class_Stat DMS = new Class_Stat(29, 1234567890, "DMS", false, false, false);

        public Class_Stat Can = new Class_Stat(30, 1234567890, "Can", false, false, false);

        public Class_Stat Face = new Class_Stat(31, 1234567890, "Face", false, false, false);

        public Class_Stat HairMesh = new Class_Stat(32, 0, "HairMesh", false, false, false);

        public Class_Stat Side = new Class_Stat(33, 0, "Side", false, false, false);

        public Class_Stat DeadTimer = new Class_Stat(34, 0, "DeadTimer", false, false, false);

        public Class_Stat AccessCount = new Class_Stat(35, 1234567890, "AccessCount", false, false, false);

        public Class_Stat AttackCount = new Class_Stat(36, 1234567890, "AttackCount", false, false, false);

        public Stat_TitleLevel TitleLevel = new Stat_TitleLevel(37, 1, "TitleLevel", false, false, false);

        public Class_Stat BackMesh = new Class_Stat(38, 0, "BackMesh", false, false, false);

        public Class_Stat ShoulderMeshHolder = new Class_Stat(39, 0, "WeaponMeshRight", false, false, false);

        public Class_Stat AlienXP = new Class_Stat(40, 0, "AlienXP", false, false, false);

        public Class_Stat FabricType = new Class_Stat(41, 1234567890, "FabricType", false, false, false);

        public Class_Stat CATMesh = new Class_Stat(42, 1234567890, "CATMesh", false, false, false);

        public Class_Stat ParentType = new Class_Stat(43, 1234567890, "ParentType", false, false, false);

        public Class_Stat ParentInstance = new Class_Stat(44, 1234567890, "ParentInstance", false, false, false);

        public Class_Stat BeltSlots = new Class_Stat(45, 0, "BeltSlots", false, false, false);

        public Class_Stat BandolierSlots = new Class_Stat(46, 1234567890, "BandolierSlots", false, false, false);

        public Class_Stat Fatness = new Class_Stat(47, 1234567890, "Fatness", false, false, false);

        public Class_Stat ClanLevel = new Class_Stat(48, 1234567890, "ClanLevel", false, false, false);

        public Class_Stat InsuranceTime = new Class_Stat(49, 0, "InsuranceTime", false, false, false);

        public Class_Stat InventoryTimeout = new Class_Stat(50, 1234567890, "InventoryTimeout", false, false, false);

        public Class_Stat AggDef = new Class_Stat(51, 100, "AggDef", false, false, false);

        public Class_Stat XP = new Class_Stat(52, 0, "XP", false, false, false);

        public Stat_IP IP = new Stat_IP(53, 1500, "IP", false, false, false);

        public Class_Stat Level = new Class_Stat(54, 1234567890, "Level", false, false, false);

        public Class_Stat InventoryId = new Class_Stat(55, 1234567890, "InventoryId", false, false, false);

        public Class_Stat TimeSinceCreation = new Class_Stat(56, 1234567890, "TimeSinceCreation", false, false, false);

        public Class_Stat LastXP = new Class_Stat(57, 0, "LastXP", false, false, false);

        public Class_Stat Age = new Class_Stat(58, 0, "Age", false, false, false);

        public Class_Stat Sex = new Class_Stat(59, 1234567890, "Sex", false, false, false);

        public Class_Stat Profession = new Class_Stat(60, 1234567890, "Profession", false, false, false);

        public Class_Stat Cash = new Class_Stat(61, 0, "Cash", false, false, false);

        public Class_Stat Alignment = new Class_Stat(62, 0, "Alignment", false, false, false);

        public Class_Stat Attitude = new Class_Stat(63, 0, "Attitude", false, false, false);

        public Class_Stat HeadMesh = new Class_Stat(64, 0, "HeadMesh", false, false, false);

        public Class_Stat MissionBits5 = new Class_Stat(65, 0, "MissionBits5", false, false, false);

        public Class_Stat MissionBits6 = new Class_Stat(66, 0, "MissionBits6", false, false, false);

        public Class_Stat MissionBits7 = new Class_Stat(67, 0, "MissionBits7", false, false, false);

        public Class_Stat VeteranPoints = new Class_Stat(68, 0, "VeteranPoints", false, false, false);

        public Class_Stat MonthsPaid = new Class_Stat(69, 0, "MonthsPaid", false, false, false);

        public Class_Stat SpeedPenalty = new Class_Stat(70, 1234567890, "SpeedPenalty", false, false, false);

        public Class_Stat TotalMass = new Class_Stat(71, 1234567890, "TotalMass", false, false, false);

        public Class_Stat ItemType = new Class_Stat(72, 0, "ItemType", false, false, false);

        public Class_Stat RepairDifficulty = new Class_Stat(73, 1234567890, "RepairDifficulty", false, false, false);

        public Class_Stat Price = new Class_Stat(74, 1234567890, "Price", false, false, false);

        public Class_Stat MetaType = new Class_Stat(75, 0, "MetaType", false, false, false);

        public Class_Stat ItemClass = new Class_Stat(76, 1234567890, "ItemClass", false, false, false);

        public Class_Stat RepairSkill = new Class_Stat(77, 1234567890, "RepairSkill", false, false, false);

        public Class_Stat CurrentMass = new Class_Stat(78, 0, "CurrentMass", false, false, false);

        public Class_Stat Icon = new Class_Stat(79, 0, "Icon", false, false, false);

        public Class_Stat PrimaryItemType = new Class_Stat(80, 1234567890, "PrimaryItemType", false, false, false);

        public Class_Stat PrimaryItemInstance = new Class_Stat(
            81, 1234567890, "PrimaryItemInstance", false, false, false);

        public Class_Stat SecondaryItemType = new Class_Stat(82, 1234567890, "SecondaryItemType", false, false, false);

        public Class_Stat SecondaryItemInstance = new Class_Stat(
            83, 1234567890, "SecondaryItemInstance", false, false, false);

        public Class_Stat UserType = new Class_Stat(84, 1234567890, "UserType", false, false, false);

        public Class_Stat UserInstance = new Class_Stat(85, 1234567890, "UserInstance", false, false, false);

        public Class_Stat AreaType = new Class_Stat(86, 1234567890, "AreaType", false, false, false);

        public Class_Stat AreaInstance = new Class_Stat(87, 1234567890, "AreaInstance", false, false, false);

        public Class_Stat DefaultPos = new Class_Stat(88, 1234567890, "DefaultPos", false, false, false);

        public Class_Stat Race = new Class_Stat(89, 1, "Race", false, false, false);

        public Class_Stat ProjectileAC = new Class_Stat(90, 0, "ProjectileAC", true, false, false);

        public Class_Stat MeleeAC = new Class_Stat(91, 0, "MeleeAC", true, false, false);

        public Class_Stat EnergyAC = new Class_Stat(92, 0, "EnergyAC", true, false, false);

        public Class_Stat ChemicalAC = new Class_Stat(93, 0, "ChemicalAC", true, false, false);

        public Class_Stat RadiationAC = new Class_Stat(94, 0, "RadiationAC", true, false, false);

        public Class_Stat ColdAC = new Class_Stat(95, 0, "ColdAC", true, false, false);

        public Class_Stat PoisonAC = new Class_Stat(96, 0, "PoisonAC", true, false, false);

        public Class_Stat FireAC = new Class_Stat(97, 0, "FireAC", true, false, false);

        public Class_Stat StateAction = new Class_Stat(98, 1234567890, "StateAction", true, false, false);

        public Class_Stat ItemAnim = new Class_Stat(99, 1234567890, "ItemAnim", true, false, false);

        public Stat_Skill MartialArts = new Stat_Skill(100, 5, "MartialArts", true, false, false);

        public Stat_Skill MeleeMultiple = new Stat_Skill(101, 5, "MeleeMultiple", true, false, false);

        public Stat_Skill OnehBluntWeapons = new Stat_Skill(102, 5, "1hBluntWeapons", true, false, false);

        public Stat_Skill OnehEdgedWeapon = new Stat_Skill(103, 5, "1hEdgedWeapon", true, false, false);

        public Stat_Skill MeleeEnergyWeapon = new Stat_Skill(104, 5, "MeleeEnergyWeapon", true, false, false);

        public Stat_Skill TwohEdgedWeapons = new Stat_Skill(105, 5, "2hEdgedWeapons", true, false, false);

        public Stat_Skill Piercing = new Stat_Skill(106, 5, "Piercing", true, false, false);

        public Stat_Skill TwohBluntWeapons = new Stat_Skill(107, 5, "2hBluntWeapons", true, false, false);

        public Stat_Skill ThrowingKnife = new Stat_Skill(108, 5, "ThrowingKnife", true, false, false);

        public Stat_Skill Grenade = new Stat_Skill(109, 5, "Grenade", true, false, false);

        public Stat_Skill ThrownGrapplingWeapons = new Stat_Skill(110, 5, "ThrownGrapplingWeapons", true, false, false);

        public Stat_Skill Bow = new Stat_Skill(111, 5, "Bow", true, false, false);

        public Stat_Skill Pistol = new Stat_Skill(112, 5, "Pistol", true, false, false);

        public Stat_Skill Rifle = new Stat_Skill(113, 5, "Rifle", true, false, false);

        public Stat_Skill SubMachineGun = new Stat_Skill(114, 5, "SubMachineGun", true, false, false);

        public Stat_Skill Shotgun = new Stat_Skill(115, 5, "Shotgun", true, false, false);

        public Stat_Skill AssaultRifle = new Stat_Skill(116, 5, "AssaultRifle", true, false, false);

        public Stat_Skill DriveWater = new Stat_Skill(117, 5, "DriveWater", true, false, false);

        public Stat_Skill CloseCombatInitiative = new Stat_Skill(118, 5, "CloseCombatInitiative", true, false, false);

        public Stat_Skill DistanceWeaponInitiative = new Stat_Skill(
            119, 5, "DistanceWeaponInitiative", true, false, false);

        public Stat_Skill PhysicalProwessInitiative = new Stat_Skill(
            120, 5, "PhysicalProwessInitiative", true, false, false);

        public Stat_Skill BowSpecialAttack = new Stat_Skill(121, 5, "BowSpecialAttack", true, false, false);

        public Stat_Skill SenseImprovement = new Stat_Skill(122, 5, "SenseImprovement", true, false, false);

        public Stat_Skill FirstAid = new Stat_Skill(123, 5, "FirstAid", true, false, false);

        public Stat_Skill Treatment = new Stat_Skill(124, 5, "Treatment", true, false, false);

        public Stat_Skill MechanicalEngineering = new Stat_Skill(125, 5, "MechanicalEngineering", true, false, false);

        public Stat_Skill ElectricalEngineering = new Stat_Skill(126, 5, "ElectricalEngineering", true, false, false);

        public Stat_Skill MaterialMetamorphose = new Stat_Skill(127, 5, "MaterialMetamorphose", true, false, false);

        public Stat_Skill BiologicalMetamorphose = new Stat_Skill(128, 5, "BiologicalMetamorphose", true, false, false);

        public Stat_Skill PsychologicalModification = new Stat_Skill(
            129, 5, "PsychologicalModification", true, false, false);

        public Stat_Skill MaterialCreation = new Stat_Skill(130, 5, "MaterialCreation", true, false, false);

        public Stat_Skill MaterialLocation = new Stat_Skill(131, 5, "MaterialLocation", true, false, false);

        public Stat_Skill NanoEnergyPool = new Stat_Skill(132, 5, "NanoEnergyPool", true, false, false);

        public Stat_Skill LR_EnergyWeapon = new Stat_Skill(133, 5, "LR_EnergyWeapon", true, false, false);

        public Stat_Skill LR_MultipleWeapon = new Stat_Skill(134, 5, "LR_MultipleWeapon", true, false, false);

        public Stat_Skill DisarmTrap = new Stat_Skill(135, 5, "DisarmTrap", true, false, false);

        public Stat_Skill Perception = new Stat_Skill(136, 5, "Perception", true, false, false);

        public Stat_Skill Adventuring = new Stat_Skill(137, 5, "Adventuring", true, false, false);

        public Stat_Skill Swim = new Stat_Skill(138, 5, "Swim", true, false, false);

        public Stat_Skill DriveAir = new Stat_Skill(139, 5, "DriveAir", true, false, false);

        public Stat_Skill MapNavigation = new Stat_Skill(140, 5, "MapNavigation", true, false, false);

        public Stat_Skill Tutoring = new Stat_Skill(141, 5, "Tutoring", true, false, false);

        public Stat_Skill Brawl = new Stat_Skill(142, 5, "Brawl", true, false, false);

        public Stat_Skill Riposte = new Stat_Skill(143, 5, "Riposte", true, false, false);

        public Stat_Skill Dimach = new Stat_Skill(144, 5, "Dimach", true, false, false);

        public Stat_Skill Parry = new Stat_Skill(145, 5, "Parry", true, false, false);

        public Stat_Skill SneakAttack = new Stat_Skill(146, 5, "SneakAttack", true, false, false);

        public Stat_Skill FastAttack = new Stat_Skill(147, 5, "FastAttack", true, false, false);

        public Stat_Skill Burst = new Stat_Skill(148, 5, "Burst", true, false, false);

        public Stat_Skill NanoProwessInitiative = new Stat_Skill(149, 5, "NanoProwessInitiative", true, false, false);

        public Stat_Skill FlingShot = new Stat_Skill(150, 5, "FlingShot", true, false, false);

        public Stat_Skill AimedShot = new Stat_Skill(151, 5, "AimedShot", true, false, false);

        public Stat_Skill BodyDevelopment = new Stat_Skill(152, 5, "BodyDevelopment", true, false, false);

        public Stat_Skill Duck = new Stat_Skill(153, 5, "Duck", true, false, false);

        public Stat_Skill Dodge = new Stat_Skill(154, 5, "Dodge", true, false, false);

        public Stat_Skill Evade = new Stat_Skill(155, 5, "Evade", true, false, false);

        public Stat_Skill RunSpeed = new Stat_Skill(156, 5, "RunSpeed", true, false, false);

        public Stat_Skill FieldQuantumPhysics = new Stat_Skill(157, 5, "FieldQuantumPhysics", true, false, false);

        public Stat_Skill WeaponSmithing = new Stat_Skill(158, 5, "WeaponSmithing", true, false, false);

        public Stat_Skill Pharmaceuticals = new Stat_Skill(159, 5, "Pharmaceuticals", true, false, false);

        public Stat_Skill NanoProgramming = new Stat_Skill(160, 5, "NanoProgramming", true, false, false);

        public Stat_Skill ComputerLiteracy = new Stat_Skill(161, 5, "ComputerLiteracy", true, false, false);

        public Stat_Skill Psychology = new Stat_Skill(162, 5, "Psychology", true, false, false);

        public Stat_Skill Chemistry = new Stat_Skill(163, 5, "Chemistry", true, false, false);

        public Stat_Skill Concealment = new Stat_Skill(164, 5, "Concealment", true, false, false);

        public Stat_Skill BreakingEntry = new Stat_Skill(165, 5, "BreakingEntry", true, false, false);

        public Stat_Skill DriveGround = new Stat_Skill(166, 5, "DriveGround", true, false, false);

        public Stat_Skill FullAuto = new Stat_Skill(167, 5, "FullAuto", true, false, false);

        public Stat_Skill NanoAC = new Stat_Skill(168, 5, "NanoAC", true, false, false);

        public Class_Stat AlienLevel = new Class_Stat(169, 0, "AlienLevel", false, false, false);

        public Class_Stat HealthChangeBest = new Class_Stat(170, 1234567890, "HealthChangeBest", false, false, false);

        public Class_Stat HealthChangeWorst = new Class_Stat(171, 1234567890, "HealthChangeWorst", false, false, false);

        public Class_Stat HealthChange = new Class_Stat(172, 1234567890, "HealthChange", false, false, false);

        public Class_Stat CurrentMovementMode = new Class_Stat(173, 3, "CurrentMovementMode", false, false, false);

        public Class_Stat PrevMovementMode = new Class_Stat(174, 3, "PrevMovementMode", false, false, false);

        public Class_Stat AutoLockTimeDefault = new Class_Stat(
            175, 1234567890, "AutoLockTimeDefault", false, false, false);

        public Class_Stat AutoUnlockTimeDefault = new Class_Stat(
            176, 1234567890, "AutoUnlockTimeDefault", false, false, false);

        public Class_Stat MoreFlags = new Class_Stat(177, 1234567890, "MoreFlags", false, false, true);

        public Stat_AlienNextXP AlienNextXP = new Stat_AlienNextXP(178, 1500, "AlienNextXP", false, false, false);

        public Class_Stat NPCFlags = new Class_Stat(179, 1234567890, "NPCFlags", false, false, false);

        public Class_Stat CurrentNCU = new Class_Stat(180, 0, "CurrentNCU", false, false, false);

        public Class_Stat MaxNCU = new Class_Stat(181, 8, "MaxNCU", false, false, false);

        public Class_Stat Specialization = new Class_Stat(182, 0, "Specialization", false, false, false);

        public Class_Stat EffectIcon = new Class_Stat(183, 1234567890, "EffectIcon", false, false, false);

        public Class_Stat BuildingType = new Class_Stat(184, 1234567890, "BuildingType", false, false, false);

        public Class_Stat BuildingInstance = new Class_Stat(185, 1234567890, "BuildingInstance", false, false, false);

        public Class_Stat CardOwnerType = new Class_Stat(186, 1234567890, "CardOwnerType", false, false, false);

        public Class_Stat CardOwnerInstance = new Class_Stat(187, 1234567890, "CardOwnerInstance", false, false, false);

        public Class_Stat BuildingComplexInst = new Class_Stat(
            188, 1234567890, "BuildingComplexInst", false, false, false);

        public Class_Stat ExitInstance = new Class_Stat(189, 1234567890, "ExitInstance", false, false, false);

        public Class_Stat NextDoorInBuilding = new Class_Stat(
            190, 1234567890, "NextDoorInBuilding", false, false, false);

        public Class_Stat LastConcretePlayfieldInstance = new Class_Stat(
            191, 0, "LastConcretePlayfieldInstance", false, false, false);

        public Class_Stat ExtenalPlayfieldInstance = new Class_Stat(
            192, 1234567890, "ExtenalPlayfieldInstance", false, false, false);

        public Class_Stat ExtenalDoorInstance = new Class_Stat(
            193, 1234567890, "ExtenalDoorInstance", false, false, false);

        public Class_Stat InPlay = new Class_Stat(194, 0, "InPlay", false, false, false);

        public Class_Stat AccessKey = new Class_Stat(195, 1234567890, "AccessKey", false, false, false);

        public Class_Stat PetMaster = new Class_Stat(196, 1234567890, "PetMaster", false, false, false);

        public Class_Stat OrientationMode = new Class_Stat(197, 1234567890, "OrientationMode", false, false, false);

        public Class_Stat SessionTime = new Class_Stat(198, 1234567890, "SessionTime", false, false, false);

        public Class_Stat RP = new Class_Stat(199, 0, "RP", false, false, false);

        public Class_Stat Conformity = new Class_Stat(200, 1234567890, "Conformity", false, false, false);

        public Class_Stat Aggressiveness = new Class_Stat(201, 1234567890, "Aggressiveness", false, false, false);

        public Class_Stat Stability = new Class_Stat(202, 1234567890, "Stability", false, false, false);

        public Class_Stat Extroverty = new Class_Stat(203, 1234567890, "Extroverty", false, false, false);

        public Class_Stat BreedHostility = new Class_Stat(204, 1234567890, "BreedHostility", false, false, false);

        public Class_Stat ReflectProjectileAC = new Class_Stat(205, 0, "ReflectProjectileAC", true, false, false);

        public Class_Stat ReflectMeleeAC = new Class_Stat(206, 0, "ReflectMeleeAC", true, false, false);

        public Class_Stat ReflectEnergyAC = new Class_Stat(207, 0, "ReflectEnergyAC", true, false, false);

        public Class_Stat ReflectChemicalAC = new Class_Stat(208, 0, "ReflectChemicalAC", true, false, false);

        public Class_Stat WeaponMeshHolder = new Class_Stat(209, 0, "WeaponMeshRight", false, false, false);

        public Class_Stat RechargeDelay = new Class_Stat(210, 1234567890, "RechargeDelay", false, false, false);

        public Class_Stat EquipDelay = new Class_Stat(211, 1234567890, "EquipDelay", false, false, false);

        public Class_Stat MaxEnergy = new Class_Stat(212, 1234567890, "MaxEnergy", false, false, false);

        public Class_Stat TeamSide = new Class_Stat(213, 0, "TeamSide", false, false, false);

        public Stat_NP CurrentNano = new Stat_NP(214, 1, "CurrentNano", false, false, false);

        public Class_Stat GmLevel = new Class_Stat(215, 0, "GmLevel", false, true, false);

        public Class_Stat ReflectRadiationAC = new Class_Stat(216, 0, "ReflectRadiationAC", true, false, false);

        public Class_Stat ReflectColdAC = new Class_Stat(217, 0, "ReflectColdAC", true, false, false);

        public Class_Stat ReflectNanoAC = new Class_Stat(218, 0, "ReflectNanoAC", true, false, false);

        public Class_Stat ReflectFireAC = new Class_Stat(219, 0, "ReflectFireAC", true, false, false);

        public Class_Stat CurrBodyLocation = new Class_Stat(220, 0, "CurrBodyLocation", false, false, false);

        public Stat_Nano MaxNanoEnergy = new Stat_Nano(221, 1, "MaxNanoEnergy", false, false, false);

        public Class_Stat AccumulatedDamage = new Class_Stat(222, 1234567890, "AccumulatedDamage", false, false, false);

        public Class_Stat CanChangeClothes = new Class_Stat(223, 1234567890, "CanChangeClothes", false, false, false);

        public Class_Stat Features = new Class_Stat(224, 6, "Features", false, false, false);

        public Class_Stat ReflectPoisonAC = new Class_Stat(225, 0, "ReflectPoisonAC", false, false, false);

        public Class_Stat ShieldProjectileAC = new Class_Stat(226, 0, "ShieldProjectileAC", true, false, false);

        public Class_Stat ShieldMeleeAC = new Class_Stat(227, 0, "ShieldMeleeAC", true, false, false);

        public Class_Stat ShieldEnergyAC = new Class_Stat(228, 0, "ShieldEnergyAC", true, false, false);

        public Class_Stat ShieldChemicalAC = new Class_Stat(229, 0, "ShieldChemicalAC", true, false, false);

        public Class_Stat ShieldRadiationAC = new Class_Stat(230, 0, "ShieldRadiationAC", true, false, false);

        public Class_Stat ShieldColdAC = new Class_Stat(231, 0, "ShieldColdAC", true, false, false);

        public Class_Stat ShieldNanoAC = new Class_Stat(232, 0, "ShieldNanoAC", true, false, false);

        public Class_Stat ShieldFireAC = new Class_Stat(233, 0, "ShieldFireAC", true, false, false);

        public Class_Stat ShieldPoisonAC = new Class_Stat(234, 0, "ShieldPoisonAC", true, false, false);

        public Class_Stat BerserkMode = new Class_Stat(235, 1234567890, "BerserkMode", false, false, false);

        public Class_Stat InsurancePercentage = new Class_Stat(236, 0, "InsurancePercentage", false, false, false);

        public Class_Stat ChangeSideCount = new Class_Stat(237, 0, "ChangeSideCount", false, false, false);

        public Class_Stat AbsorbProjectileAC = new Class_Stat(238, 0, "AbsorbProjectileAC", true, false, false);

        public Class_Stat AbsorbMeleeAC = new Class_Stat(239, 0, "AbsorbMeleeAC", true, false, false);

        public Class_Stat AbsorbEnergyAC = new Class_Stat(240, 0, "AbsorbEnergyAC", true, false, false);

        public Class_Stat AbsorbChemicalAC = new Class_Stat(241, 0, "AbsorbChemicalAC", true, false, false);

        public Class_Stat AbsorbRadiationAC = new Class_Stat(242, 0, "AbsorbRadiationAC", true, false, false);

        public Class_Stat AbsorbColdAC = new Class_Stat(243, 0, "AbsorbColdAC", true, false, false);

        public Class_Stat AbsorbFireAC = new Class_Stat(244, 0, "AbsorbFireAC", true, false, false);

        public Class_Stat AbsorbPoisonAC = new Class_Stat(245, 0, "AbsorbPoisonAC", true, false, false);

        public Class_Stat AbsorbNanoAC = new Class_Stat(246, 0, "AbsorbNanoAC", true, false, false);

        public Class_Stat TemporarySkillReduction = new Class_Stat(
            247, 0, "TemporarySkillReduction", false, false, false);

        public Class_Stat BirthDate = new Class_Stat(248, 1234567890, "BirthDate", false, false, false);

        public Class_Stat LastSaved = new Class_Stat(249, 1234567890, "LastSaved", false, false, false);

        public Class_Stat SoundVolume = new Class_Stat(250, 1234567890, "SoundVolume", false, false, false);

        public Class_Stat PetCounter = new Class_Stat(251, 1234567890, "PetCounter", false, false, false);

        public Class_Stat MeetersWalked = new Class_Stat(252, 1234567890, "MeetersWalked", false, false, false);

        public Class_Stat QuestLevelsSolved = new Class_Stat(253, 1234567890, "QuestLevelsSolved", false, false, false);

        // Accumulated Tokens?

        public Class_Stat MonsterLevelsKilled = new Class_Stat(
            254, 1234567890, "MonsterLevelsKilled", false, false, false);

        public Class_Stat PvPLevelsKilled = new Class_Stat(255, 1234567890, "PvPLevelsKilled", false, false, false);

        public Class_Stat MissionBits1 = new Class_Stat(256, 0, "MissionBits1", false, false, false);

        public Class_Stat MissionBits2 = new Class_Stat(257, 0, "MissionBits2", false, false, false);

        public Class_Stat AccessGrant = new Class_Stat(258, 1234567890, "AccessGrant", false, false, false);

        public Class_Stat DoorFlags = new Class_Stat(259, 1234567890, "DoorFlags", false, false, false);

        public Class_Stat ClanHierarchy = new Class_Stat(260, 1234567890, "ClanHierarchy", false, false, false);

        public Class_Stat QuestStat = new Class_Stat(261, 1234567890, "QuestStat", false, false, false);

        public Class_Stat ClientActivated = new Class_Stat(262, 1234567890, "ClientActivated", false, false, false);

        public Class_Stat PersonalResearchLevel = new Class_Stat(263, 0, "PersonalResearchLevel", false, false, false);

        public Class_Stat GlobalResearchLevel = new Class_Stat(264, 0, "GlobalResearchLevel", false, false, false);

        public Class_Stat PersonalResearchGoal = new Class_Stat(265, 0, "PersonalResearchGoal", false, false, false);

        public Class_Stat GlobalResearchGoal = new Class_Stat(266, 0, "GlobalResearchGoal", false, false, false);

        public Class_Stat TurnSpeed = new Class_Stat(267, 40000, "TurnSpeed", false, false, false);

        public Class_Stat LiquidType = new Class_Stat(268, 1234567890, "LiquidType", false, false, false);

        public Class_Stat GatherSound = new Class_Stat(269, 1234567890, "GatherSound", false, false, false);

        public Class_Stat CastSound = new Class_Stat(270, 1234567890, "CastSound", false, false, false);

        public Class_Stat TravelSound = new Class_Stat(271, 1234567890, "TravelSound", false, false, false);

        public Class_Stat HitSound = new Class_Stat(272, 1234567890, "HitSound", false, false, false);

        public Class_Stat SecondaryItemTemplate = new Class_Stat(
            273, 1234567890, "SecondaryItemTemplate", false, false, false);

        public Class_Stat EquippedWeapons = new Class_Stat(274, 1234567890, "EquippedWeapons", false, false, false);

        public Class_Stat XPKillRange = new Class_Stat(275, 5, "XPKillRange", false, false, false);

        public Class_Stat AMSModifier = new Class_Stat(276, 0, "AMSModifier", false, false, false);

        public Class_Stat DMSModifier = new Class_Stat(277, 0, "DMSModifier", false, false, false);

        public Class_Stat ProjectileDamageModifier = new Class_Stat(
            278, 0, "ProjectileDamageModifier", false, false, false);

        public Class_Stat MeleeDamageModifier = new Class_Stat(279, 0, "MeleeDamageModifier", false, false, false);

        public Class_Stat EnergyDamageModifier = new Class_Stat(280, 0, "EnergyDamageModifier", false, false, false);

        public Class_Stat ChemicalDamageModifier = new Class_Stat(281, 0, "ChemicalDamageModifier", false, false, false);

        public Class_Stat RadiationDamageModifier = new Class_Stat(
            282, 0, "RadiationDamageModifier", false, false, false);

        public Class_Stat ItemHateValue = new Class_Stat(283, 1234567890, "ItemHateValue", false, false, false);

        public Class_Stat DamageBonus = new Class_Stat(284, 1234567890, "DamageBonus", false, false, false);

        public Class_Stat MaxDamage = new Class_Stat(285, 1234567890, "MaxDamage", false, false, false);

        public Class_Stat MinDamage = new Class_Stat(286, 1234567890, "MinDamage", false, false, false);

        public Class_Stat AttackRange = new Class_Stat(287, 1234567890, "AttackRange", false, false, false);

        public Class_Stat HateValueModifyer = new Class_Stat(288, 1234567890, "HateValueModifyer", false, false, false);

        public Class_Stat TrapDifficulty = new Class_Stat(289, 1234567890, "TrapDifficulty", false, false, false);

        public Class_Stat StatOne = new Class_Stat(290, 1234567890, "StatOne", false, false, false);

        public Class_Stat NumAttackEffects = new Class_Stat(291, 1234567890, "NumAttackEffects", false, false, false);

        public Class_Stat DefaultAttackType = new Class_Stat(292, 1234567890, "DefaultAttackType", false, false, false);

        public Class_Stat ItemSkill = new Class_Stat(293, 1234567890, "ItemSkill", false, false, false);

        public Class_Stat ItemDelay = new Class_Stat(294, 1234567890, "ItemDelay", false, false, false);

        public Class_Stat ItemOpposedSkill = new Class_Stat(295, 1234567890, "ItemOpposedSkill", false, false, false);

        public Class_Stat ItemSIS = new Class_Stat(296, 1234567890, "ItemSIS", false, false, false);

        public Class_Stat InteractionRadius = new Class_Stat(297, 1234567890, "InteractionRadius", false, false, false);

        public Class_Stat Placement = new Class_Stat(298, 1234567890, "Placement", false, false, false);

        public Class_Stat LockDifficulty = new Class_Stat(299, 1234567890, "LockDifficulty", false, false, false);

        public Class_Stat Members = new Class_Stat(300, 999, "Members", false, false, false);

        public Class_Stat MinMembers = new Class_Stat(301, 1234567890, "MinMembers", false, false, false);

        public Class_Stat ClanPrice = new Class_Stat(302, 1234567890, "ClanPrice", false, false, false);

        public Class_Stat MissionBits3 = new Class_Stat(303, 0, "MissionBits3", false, false, false);

        public Class_Stat ClanType = new Class_Stat(304, 1234567890, "ClanType", false, false, false);

        public Class_Stat ClanInstance = new Class_Stat(305, 1234567890, "ClanInstance", false, false, false);

        public Class_Stat VoteCount = new Class_Stat(306, 1234567890, "VoteCount", false, false, false);

        public Class_Stat MemberType = new Class_Stat(307, 1234567890, "MemberType", false, false, false);

        public Class_Stat MemberInstance = new Class_Stat(308, 1234567890, "MemberInstance", false, false, false);

        public Class_Stat GlobalClanType = new Class_Stat(309, 1234567890, "GlobalClanType", false, false, false);

        public Class_Stat GlobalClanInstance = new Class_Stat(
            310, 1234567890, "GlobalClanInstance", false, false, false);

        public Class_Stat ColdDamageModifier = new Class_Stat(
            311, 1234567890, "ColdDamageModifier", false, false, false);

        public Class_Stat ClanUpkeepInterval = new Class_Stat(
            312, 1234567890, "ClanUpkeepInterval", false, false, false);

        public Class_Stat TimeSinceUpkeep = new Class_Stat(313, 1234567890, "TimeSinceUpkeep", false, false, false);

        public Class_Stat ClanFinalized = new Class_Stat(314, 1234567890, "ClanFinalized", false, false, false);

        public Class_Stat NanoDamageModifier = new Class_Stat(315, 0, "NanoDamageModifier", false, false, false);

        public Class_Stat FireDamageModifier = new Class_Stat(316, 0, "FireDamageModifier", false, false, false);

        public Class_Stat PoisonDamageModifier = new Class_Stat(317, 0, "PoisonDamageModifier", false, false, false);

        public Class_Stat NPCostModifier = new Class_Stat(318, 0, "NPCostModifier", false, false, false);

        public Class_Stat XPModifier = new Class_Stat(319, 0, "XPModifier", false, false, false);

        public Class_Stat BreedLimit = new Class_Stat(320, 1234567890, "BreedLimit", false, false, false);

        public Class_Stat GenderLimit = new Class_Stat(321, 1234567890, "GenderLimit", false, false, false);

        public Class_Stat LevelLimit = new Class_Stat(322, 1234567890, "LevelLimit", false, false, false);

        public Class_Stat PlayerKilling = new Class_Stat(323, 1234567890, "PlayerKilling", false, false, false);

        public Class_Stat TeamAllowed = new Class_Stat(324, 1234567890, "TeamAllowed", false, false, false);

        public Class_Stat WeaponDisallowedType = new Class_Stat(
            325, 1234567890, "WeaponDisallowedType", false, false, false);

        public Class_Stat WeaponDisallowedInstance = new Class_Stat(
            326, 1234567890, "WeaponDisallowedInstance", false, false, false);

        public Class_Stat Taboo = new Class_Stat(327, 1234567890, "Taboo", false, false, false);

        public Class_Stat Compulsion = new Class_Stat(328, 1234567890, "Compulsion", false, false, false);

        public Class_Stat SkillDisabled = new Class_Stat(329, 1234567890, "SkillDisabled", false, false, false);

        public Class_Stat ClanItemType = new Class_Stat(330, 1234567890, "ClanItemType", false, false, false);

        public Class_Stat ClanItemInstance = new Class_Stat(331, 1234567890, "ClanItemInstance", false, false, false);

        public Class_Stat DebuffFormula = new Class_Stat(332, 1234567890, "DebuffFormula", false, false, false);

        public Class_Stat PvP_Rating = new Class_Stat(333, 1300, "PvP_Rating", false, false, false);

        public Class_Stat SavedXP = new Class_Stat(334, 0, "SavedXP", false, false, false);

        public Class_Stat DoorBlockTime = new Class_Stat(335, 1234567890, "DoorBlockTime", false, false, false);

        public Class_Stat OverrideTexture = new Class_Stat(336, 1234567890, "OverrideTexture", false, false, false);

        public Class_Stat OverrideMaterial = new Class_Stat(337, 1234567890, "OverrideMaterial", false, false, false);

        public Class_Stat DeathReason = new Class_Stat(338, 1234567890, "DeathReason", false, false, false);

        public Class_Stat DamageOverrideType = new Class_Stat(
            339, 1234567890, "DamageOverrideType", false, false, false);

        public Class_Stat BrainType = new Class_Stat(340, 1234567890, "BrainType", false, false, false);

        public Class_Stat XPBonus = new Class_Stat(341, 1234567890, "XPBonus", false, false, false);

        public Stat_HealInterval HealInterval = new Stat_HealInterval(342, 29, "HealInterval", false, false, false);

        public Stat_HealDelta HealDelta = new Stat_HealDelta(343, 1234567890, "HealDelta", false, false, false);

        public Class_Stat MonsterTexture = new Class_Stat(344, 1234567890, "MonsterTexture", false, false, false);

        public Class_Stat HasAlwaysLootable = new Class_Stat(345, 1234567890, "HasAlwaysLootable", false, false, false);

        public Class_Stat TradeLimit = new Class_Stat(346, 1234567890, "TradeLimit", false, false, false);

        public Class_Stat FaceTexture = new Class_Stat(347, 1234567890, "FaceTexture", false, false, false);

        public Class_Stat SpecialCondition = new Class_Stat(348, 1, "SpecialCondition", false, false, false);

        public Class_Stat AutoAttackFlags = new Class_Stat(349, 5, "AutoAttackFlags", false, false, false);

        public Stat_NextXP NextXP = new Stat_NextXP(350, 1450, "NextXP", false, false, false);

        public Class_Stat TeleportPauseMilliSeconds = new Class_Stat(
            351, 1234567890, "TeleportPauseMilliSeconds", false, false, false);

        public Class_Stat SISCap = new Class_Stat(352, 1234567890, "SISCap", false, false, false);

        public Class_Stat AnimSet = new Class_Stat(353, 1234567890, "AnimSet", false, false, false);

        public Class_Stat AttackType = new Class_Stat(354, 1234567890, "AttackType", false, false, false);

        public Class_Stat NanoFocusLevel = new Class_Stat(355, 0, "NanoFocusLevel", false, false, false);

        public Class_Stat NPCHash = new Class_Stat(356, 1234567890, "NPCHash", false, false, false);

        public Class_Stat CollisionRadius = new Class_Stat(357, 1234567890, "CollisionRadius", false, false, false);

        public Class_Stat OuterRadius = new Class_Stat(358, 1234567890, "OuterRadius", false, false, false);

        public Class_Stat MonsterData = new Class_Stat(359, 0, "MonsterData", false, false, true);

        public Class_Stat MonsterScale = new Class_Stat(360, 1234567890, "MonsterScale", false, false, true);

        public Class_Stat HitEffectType = new Class_Stat(361, 1234567890, "HitEffectType", false, false, false);

        public Class_Stat ResurrectDest = new Class_Stat(362, 1234567890, "ResurrectDest", false, false, false);

        public Stat_NanoInterval NanoInterval = new Stat_NanoInterval(363, 28, "NanoInterval", false, false, false);

        public Stat_NanoDelta NanoDelta = new Stat_NanoDelta(364, 1234567890, "NanoDelta", false, false, false);

        public Class_Stat ReclaimItem = new Class_Stat(365, 1234567890, "ReclaimItem", false, false, false);

        public Class_Stat GatherEffectType = new Class_Stat(366, 1234567890, "GatherEffectType", false, false, false);

        public Class_Stat VisualBreed = new Class_Stat(367, 1234567890, "VisualBreed", false, false, true);

        public Class_Stat VisualProfession = new Class_Stat(368, 1234567890, "VisualProfession", false, false, true);

        public Class_Stat VisualSex = new Class_Stat(369, 1234567890, "VisualSex", false, false, true);

        public Class_Stat RitualTargetInst = new Class_Stat(370, 1234567890, "RitualTargetInst", false, false, false);

        public Class_Stat SkillTimeOnSelectedTarget = new Class_Stat(
            371, 1234567890, "SkillTimeOnSelectedTarget", false, false, false);

        public Class_Stat LastSaveXP = new Class_Stat(372, 0, "LastSaveXP", false, false, false);

        public Class_Stat ExtendedTime = new Class_Stat(373, 1234567890, "ExtendedTime", false, false, false);

        public Class_Stat BurstRecharge = new Class_Stat(374, 1234567890, "BurstRecharge", false, false, false);

        public Class_Stat FullAutoRecharge = new Class_Stat(375, 1234567890, "FullAutoRecharge", false, false, false);

        public Class_Stat GatherAbstractAnim = new Class_Stat(
            376, 1234567890, "GatherAbstractAnim", false, false, false);

        public Class_Stat CastTargetAbstractAnim = new Class_Stat(
            377, 1234567890, "CastTargetAbstractAnim", false, false, false);

        public Class_Stat CastSelfAbstractAnim = new Class_Stat(
            378, 1234567890, "CastSelfAbstractAnim", false, false, false);

        public Class_Stat CriticalIncrease = new Class_Stat(379, 1234567890, "CriticalIncrease", false, false, false);

        public Class_Stat RangeIncreaserWeapon = new Class_Stat(380, 0, "RangeIncreaserWeapon", false, false, false);

        public Class_Stat RangeIncreaserNF = new Class_Stat(381, 0, "RangeIncreaserNF", false, false, false);

        public Class_Stat SkillLockModifier = new Class_Stat(382, 0, "SkillLockModifier", false, false, false);

        public Class_Stat InterruptModifier = new Class_Stat(383, 1234567890, "InterruptModifier", false, false, false);

        public Class_Stat ACGEntranceStyles = new Class_Stat(384, 1234567890, "ACGEntranceStyles", false, false, false);

        public Class_Stat ChanceOfBreakOnSpellAttack = new Class_Stat(
            385, 1234567890, "ChanceOfBreakOnSpellAttack", false, false, false);

        public Class_Stat ChanceOfBreakOnDebuff = new Class_Stat(
            386, 1234567890, "ChanceOfBreakOnDebuff", false, false, false);

        public Class_Stat DieAnim = new Class_Stat(387, 1234567890, "DieAnim", false, false, false);

        public Class_Stat TowerType = new Class_Stat(388, 1234567890, "TowerType", false, false, false);

        public Class_Stat Expansion = new Class_Stat(389, 0, "Expansion", false, true, false);

        public Class_Stat LowresMesh = new Class_Stat(390, 1234567890, "LowresMesh", false, false, false);

        public Class_Stat CriticalDecrease = new Class_Stat(391, 1234567890, "CriticalDecrease", false, false, false);

        public Class_Stat OldTimeExist = new Class_Stat(392, 1234567890, "OldTimeExist", false, false, false);

        public Class_Stat ResistModifier = new Class_Stat(393, 1234567890, "ResistModifier", false, false, false);

        public Class_Stat ChestFlags = new Class_Stat(394, 1234567890, "ChestFlags", false, false, false);

        public Class_Stat PrimaryTemplateID = new Class_Stat(395, 1234567890, "PrimaryTemplateID", false, false, false);

        public Class_Stat NumberOfItems = new Class_Stat(396, 1234567890, "NumberOfItems", false, false, false);

        public Class_Stat SelectedTargetType = new Class_Stat(
            397, 1234567890, "SelectedTargetType", false, false, false);

        public Class_Stat Corpse_Hash = new Class_Stat(398, 1234567890, "Corpse_Hash", false, false, false);

        public Class_Stat AmmoName = new Class_Stat(399, 1234567890, "AmmoName", false, false, false);

        public Class_Stat Rotation = new Class_Stat(400, 1234567890, "Rotation", false, false, false);

        public Class_Stat CATAnim = new Class_Stat(401, 1234567890, "CATAnim", false, false, false);

        public Class_Stat CATAnimFlags = new Class_Stat(402, 1234567890, "CATAnimFlags", false, false, false);

        public Class_Stat DisplayCATAnim = new Class_Stat(403, 1234567890, "DisplayCATAnim", false, false, false);

        public Class_Stat DisplayCATMesh = new Class_Stat(404, 1234567890, "DisplayCATMesh", false, false, false);

        public Class_Stat School = new Class_Stat(405, 1234567890, "School", false, false, false);

        public Class_Stat NanoSpeed = new Class_Stat(406, 1234567890, "NanoSpeed", false, false, false);

        public Class_Stat NanoPoints = new Class_Stat(407, 1234567890, "NanoPoints", false, false, false);

        public Class_Stat TrainSkill = new Class_Stat(408, 1234567890, "TrainSkill", false, false, false);

        public Class_Stat TrainSkillCost = new Class_Stat(409, 1234567890, "TrainSkillCost", false, false, false);

        public Class_Stat IsFightingMe = new Class_Stat(410, 1234567890, "IsFightingMe", false, false, false);

        public Class_Stat NextFormula = new Class_Stat(411, 1234567890, "NextFormula", false, false, false);

        public Class_Stat MultipleCount = new Class_Stat(412, 1234567890, "MultipleCount", false, false, false);

        public Class_Stat EffectType = new Class_Stat(413, 1234567890, "EffectType", false, false, false);

        public Class_Stat ImpactEffectType = new Class_Stat(414, 1234567890, "ImpactEffectType", false, false, false);

        public Class_Stat CorpseType = new Class_Stat(415, 1234567890, "CorpseType", false, false, false);

        public Class_Stat CorpseInstance = new Class_Stat(416, 1234567890, "CorpseInstance", false, false, false);

        public Class_Stat CorpseAnimKey = new Class_Stat(417, 1234567890, "CorpseAnimKey", false, false, false);

        public Class_Stat UnarmedTemplateInstance = new Class_Stat(
            418, 0, "UnarmedTemplateInstance", false, false, false);

        public Class_Stat TracerEffectType = new Class_Stat(419, 1234567890, "TracerEffectType", false, false, false);

        public Class_Stat AmmoType = new Class_Stat(420, 1234567890, "AmmoType", false, false, false);

        public Class_Stat CharRadius = new Class_Stat(421, 1234567890, "CharRadius", false, false, false);

        public Class_Stat ChanceOfUse = new Class_Stat(422, 1234567890, "ChanceOfUse", false, false, false);

        public Class_Stat CurrentState = new Class_Stat(423, 0, "CurrentState", false, false, false);

        public Class_Stat ArmourType = new Class_Stat(424, 1234567890, "ArmourType", false, false, false);

        public Class_Stat RestModifier = new Class_Stat(425, 1234567890, "RestModifier", false, false, false);

        public Class_Stat BuyModifier = new Class_Stat(426, 1234567890, "BuyModifier", false, false, false);

        public Class_Stat SellModifier = new Class_Stat(427, 1234567890, "SellModifier", false, false, false);

        public Class_Stat CastEffectType = new Class_Stat(428, 1234567890, "CastEffectType", false, false, false);

        public Class_Stat NPCBrainState = new Class_Stat(429, 1234567890, "NPCBrainState", false, false, false);

        public Class_Stat WaitState = new Class_Stat(430, 2, "WaitState", false, false, false);

        public Class_Stat SelectedTarget = new Class_Stat(431, 1234567890, "SelectedTarget", false, false, false);

        public Class_Stat MissionBits4 = new Class_Stat(432, 0, "MissionBits4", false, false, false);

        public Class_Stat OwnerInstance = new Class_Stat(433, 1234567890, "OwnerInstance", false, false, false);

        public Class_Stat CharState = new Class_Stat(434, 1234567890, "CharState", false, false, false);

        public Class_Stat ReadOnly = new Class_Stat(435, 1234567890, "ReadOnly", false, false, false);

        public Class_Stat DamageType = new Class_Stat(436, 1234567890, "DamageType", false, false, false);

        public Class_Stat CollideCheckInterval = new Class_Stat(
            437, 1234567890, "CollideCheckInterval", false, false, false);

        public Class_Stat PlayfieldType = new Class_Stat(438, 1234567890, "PlayfieldType", false, false, false);

        public Class_Stat NPCCommand = new Class_Stat(439, 1234567890, "NPCCommand", false, false, false);

        public Class_Stat InitiativeType = new Class_Stat(440, 1234567890, "InitiativeType", false, false, false);

        public Class_Stat CharTmp1 = new Class_Stat(441, 1234567890, "CharTmp1", false, false, false);

        public Class_Stat CharTmp2 = new Class_Stat(442, 1234567890, "CharTmp2", false, false, false);

        public Class_Stat CharTmp3 = new Class_Stat(443, 1234567890, "CharTmp3", false, false, false);

        public Class_Stat CharTmp4 = new Class_Stat(444, 1234567890, "CharTmp4", false, false, false);

        public Class_Stat NPCCommandArg = new Class_Stat(445, 1234567890, "NPCCommandArg", false, false, false);

        public Class_Stat NameTemplate = new Class_Stat(446, 1234567890, "NameTemplate", false, false, false);

        public Class_Stat DesiredTargetDistance = new Class_Stat(
            447, 1234567890, "DesiredTargetDistance", false, false, false);

        public Class_Stat VicinityRange = new Class_Stat(448, 1234567890, "VicinityRange", false, false, false);

        public Class_Stat NPCIsSurrendering = new Class_Stat(449, 1234567890, "NPCIsSurrendering", false, false, false);

        public Class_Stat StateMachine = new Class_Stat(450, 1234567890, "StateMachine", false, false, false);

        public Class_Stat NPCSurrenderInstance = new Class_Stat(
            451, 1234567890, "NPCSurrenderInstance", false, false, false);

        public Class_Stat NPCHasPatrolList = new Class_Stat(452, 1234567890, "NPCHasPatrolList", false, false, false);

        public Class_Stat NPCVicinityChars = new Class_Stat(453, 1234567890, "NPCVicinityChars", false, false, false);

        public Class_Stat ProximityRangeOutdoors = new Class_Stat(
            454, 1234567890, "ProximityRangeOutdoors", false, false, false);

        public Class_Stat NPCFamily = new Class_Stat(455, 1234567890, "NPCFamily", false, false, false);

        public Class_Stat CommandRange = new Class_Stat(456, 1234567890, "CommandRange", false, false, false);

        public Class_Stat NPCHatelistSize = new Class_Stat(457, 1234567890, "NPCHatelistSize", false, false, false);

        public Class_Stat NPCNumPets = new Class_Stat(458, 1234567890, "NPCNumPets", false, false, false);

        public Class_Stat ODMinSizeAdd = new Class_Stat(459, 1234567890, "ODMinSizeAdd", false, false, false);

        public Class_Stat EffectRed = new Class_Stat(460, 1234567890, "EffectRed", false, false, false);

        public Class_Stat EffectGreen = new Class_Stat(461, 1234567890, "EffectGreen", false, false, false);

        public Class_Stat EffectBlue = new Class_Stat(462, 1234567890, "EffectBlue", false, false, false);

        public Class_Stat ODMaxSizeAdd = new Class_Stat(463, 1234567890, "ODMaxSizeAdd", false, false, false);

        public Class_Stat DurationModifier = new Class_Stat(464, 1234567890, "DurationModifier", false, false, false);

        public Class_Stat NPCCryForHelpRange = new Class_Stat(
            465, 1234567890, "NPCCryForHelpRange", false, false, false);

        public Class_Stat LOSHeight = new Class_Stat(466, 1234567890, "LOSHeight", false, false, false);

        public Class_Stat PetReq1 = new Class_Stat(467, 1234567890, "PetReq1", false, false, false);

        public Class_Stat PetReq2 = new Class_Stat(468, 1234567890, "PetReq2", false, false, false);

        public Class_Stat PetReq3 = new Class_Stat(469, 1234567890, "PetReq3", false, false, false);

        public Class_Stat MapOptions = new Class_Stat(470, 0, "MapOptions", false, false, false);

        public Class_Stat MapAreaPart1 = new Class_Stat(471, 0, "MapAreaPart1", false, false, false);

        public Class_Stat MapAreaPart2 = new Class_Stat(472, 0, "MapAreaPart2", false, false, false);

        public Class_Stat FixtureFlags = new Class_Stat(473, 1234567890, "FixtureFlags", false, false, false);

        public Class_Stat FallDamage = new Class_Stat(474, 1234567890, "FallDamage", false, false, false);

        public Class_Stat ReflectReturnedProjectileAC = new Class_Stat(
            475, 0, "ReflectReturnedProjectileAC", false, false, false);

        public Class_Stat ReflectReturnedMeleeAC = new Class_Stat(476, 0, "ReflectReturnedMeleeAC", false, false, false);

        public Class_Stat ReflectReturnedEnergyAC = new Class_Stat(
            477, 0, "ReflectReturnedEnergyAC", false, false, false);

        public Class_Stat ReflectReturnedChemicalAC = new Class_Stat(
            478, 0, "ReflectReturnedChemicalAC", false, false, false);

        public Class_Stat ReflectReturnedRadiationAC = new Class_Stat(
            479, 0, "ReflectReturnedRadiationAC", false, false, false);

        public Class_Stat ReflectReturnedColdAC = new Class_Stat(480, 0, "ReflectReturnedColdAC", false, false, false);

        public Class_Stat ReflectReturnedNanoAC = new Class_Stat(481, 0, "ReflectReturnedNanoAC", false, false, false);

        public Class_Stat ReflectReturnedFireAC = new Class_Stat(482, 0, "ReflectReturnedFireAC", false, false, false);

        public Class_Stat ReflectReturnedPoisonAC = new Class_Stat(
            483, 0, "ReflectReturnedPoisonAC", false, false, false);

        public Class_Stat ProximityRangeIndoors = new Class_Stat(
            484, 1234567890, "ProximityRangeIndoors", false, false, false);

        public Class_Stat PetReqVal1 = new Class_Stat(485, 1234567890, "PetReqVal1", false, false, false);

        public Class_Stat PetReqVal2 = new Class_Stat(486, 1234567890, "PetReqVal2", false, false, false);

        public Class_Stat PetReqVal3 = new Class_Stat(487, 1234567890, "PetReqVal3", false, false, false);

        public Class_Stat TargetFacing = new Class_Stat(488, 1234567890, "TargetFacing", false, false, false);

        public Class_Stat Backstab = new Class_Stat(489, 1234567890, "Backstab", true, false, false);

        public Class_Stat OriginatorType = new Class_Stat(490, 1234567890, "OriginatorType", false, false, false);

        public Class_Stat QuestInstance = new Class_Stat(491, 1234567890, "QuestInstance", false, false, false);

        public Class_Stat QuestIndex1 = new Class_Stat(492, 1234567890, "QuestIndex1", false, false, false);

        public Class_Stat QuestIndex2 = new Class_Stat(493, 1234567890, "QuestIndex2", false, false, false);

        public Class_Stat QuestIndex3 = new Class_Stat(494, 1234567890, "QuestIndex3", false, false, false);

        public Class_Stat QuestIndex4 = new Class_Stat(495, 1234567890, "QuestIndex4", false, false, false);

        public Class_Stat QuestIndex5 = new Class_Stat(496, 1234567890, "QuestIndex5", false, false, false);

        public Class_Stat QTDungeonInstance = new Class_Stat(497, 1234567890, "QTDungeonInstance", false, false, false);

        public Class_Stat QTNumMonsters = new Class_Stat(498, 1234567890, "QTNumMonsters", false, false, false);

        public Class_Stat QTKilledMonsters = new Class_Stat(499, 1234567890, "QTKilledMonsters", false, false, false);

        public Class_Stat AnimPos = new Class_Stat(500, 1234567890, "AnimPos", false, false, false);

        public Class_Stat AnimPlay = new Class_Stat(501, 1234567890, "AnimPlay", false, false, false);

        public Class_Stat AnimSpeed = new Class_Stat(502, 1234567890, "AnimSpeed", false, false, false);

        public Class_Stat QTKillNumMonsterID1 = new Class_Stat(
            503, 1234567890, "QTKillNumMonsterID1", false, false, false);

        public Class_Stat QTKillNumMonsterCount1 = new Class_Stat(
            504, 1234567890, "QTKillNumMonsterCount1", false, false, false);

        public Class_Stat QTKillNumMonsterID2 = new Class_Stat(
            505, 1234567890, "QTKillNumMonsterID2", false, false, false);

        public Class_Stat QTKillNumMonsterCount2 = new Class_Stat(
            506, 1234567890, "QTKillNumMonsterCount2", false, false, false);

        public Class_Stat QTKillNumMonsterID3 = new Class_Stat(
            507, 1234567890, "QTKillNumMonsterID3", false, false, false);

        public Class_Stat QTKillNumMonsterCount3 = new Class_Stat(
            508, 1234567890, "QTKillNumMonsterCount3", false, false, false);

        public Class_Stat QuestIndex0 = new Class_Stat(509, 1234567890, "QuestIndex0", false, false, false);

        public Class_Stat QuestTimeout = new Class_Stat(510, 1234567890, "QuestTimeout", false, false, false);

        public Class_Stat Tower_NPCHash = new Class_Stat(511, 1234567890, "Tower_NPCHash", false, false, false);

        public Class_Stat PetType = new Class_Stat(512, 1234567890, "PetType", false, false, false);

        public Class_Stat OnTowerCreation = new Class_Stat(513, 1234567890, "OnTowerCreation", false, false, false);

        public Class_Stat OwnedTowers = new Class_Stat(514, 1234567890, "OwnedTowers", false, false, false);

        public Class_Stat TowerInstance = new Class_Stat(515, 1234567890, "TowerInstance", false, false, false);

        public Class_Stat AttackShield = new Class_Stat(516, 1234567890, "AttackShield", false, false, false);

        public Class_Stat SpecialAttackShield = new Class_Stat(
            517, 1234567890, "SpecialAttackShield", false, false, false);

        public Class_Stat NPCVicinityPlayers = new Class_Stat(
            518, 1234567890, "NPCVicinityPlayers", false, false, false);

        public Class_Stat NPCUseFightModeRegenRate = new Class_Stat(
            519, 1234567890, "NPCUseFightModeRegenRate", false, false, false);

        public Class_Stat Rnd = new Class_Stat(520, 1234567890, "Rnd", false, false, false);

        public Class_Stat SocialStatus = new Class_Stat(521, 0, "SocialStatus", false, false, false);

        public Class_Stat LastRnd = new Class_Stat(522, 1234567890, "LastRnd", false, false, false);

        public Class_Stat ItemDelayCap = new Class_Stat(523, 1234567890, "ItemDelayCap", false, false, false);

        public Class_Stat RechargeDelayCap = new Class_Stat(524, 1234567890, "RechargeDelayCap", false, false, false);

        public Class_Stat PercentRemainingHealth = new Class_Stat(
            525, 1234567890, "PercentRemainingHealth", false, false, false);

        public Class_Stat PercentRemainingNano = new Class_Stat(
            526, 1234567890, "PercentRemainingNano", false, false, false);

        public Class_Stat TargetDistance = new Class_Stat(527, 1234567890, "TargetDistance", false, false, false);

        public Class_Stat TeamCloseness = new Class_Stat(528, 1234567890, "TeamCloseness", false, false, false);

        public Class_Stat NumberOnHateList = new Class_Stat(529, 1234567890, "NumberOnHateList", false, false, false);

        public Class_Stat ConditionState = new Class_Stat(530, 1234567890, "ConditionState", false, false, false);

        public Class_Stat ExpansionPlayfield = new Class_Stat(
            531, 1234567890, "ExpansionPlayfield", false, false, false);

        public Class_Stat ShadowBreed = new Class_Stat(532, 0, "ShadowBreed", false, false, false);

        public Class_Stat NPCFovStatus = new Class_Stat(533, 1234567890, "NPCFovStatus", false, false, false);

        public Class_Stat DudChance = new Class_Stat(534, 1234567890, "DudChance", false, false, false);

        public Class_Stat HealMultiplier = new Class_Stat(535, 1234567890, "HealMultiplier", false, false, false);

        public Class_Stat NanoDamageMultiplier = new Class_Stat(536, 0, "NanoDamageMultiplier", false, false, false);

        public Class_Stat NanoVulnerability = new Class_Stat(537, 1234567890, "NanoVulnerability", false, false, false);

        public Class_Stat AmsCap = new Class_Stat(538, 1234567890, "AmsCap", false, false, false);

        public Class_Stat ProcInitiative1 = new Class_Stat(539, 1234567890, "ProcInitiative1", false, false, false);

        public Class_Stat ProcInitiative2 = new Class_Stat(540, 1234567890, "ProcInitiative2", false, false, false);

        public Class_Stat ProcInitiative3 = new Class_Stat(541, 1234567890, "ProcInitiative3", false, false, false);

        public Class_Stat ProcInitiative4 = new Class_Stat(542, 1234567890, "ProcInitiative4", false, false, false);

        public Class_Stat FactionModifier = new Class_Stat(543, 1234567890, "FactionModifier", false, false, false);

        public Class_Stat MissionBits8 = new Class_Stat(544, 0, "MissionBits8", false, false, false);

        public Class_Stat MissionBits9 = new Class_Stat(545, 0, "MissionBits9", false, false, false);

        public Class_Stat StackingLine2 = new Class_Stat(546, 1234567890, "StackingLine2", false, false, false);

        public Class_Stat StackingLine3 = new Class_Stat(547, 1234567890, "StackingLine3", false, false, false);

        public Class_Stat StackingLine4 = new Class_Stat(548, 1234567890, "StackingLine4", false, false, false);

        public Class_Stat StackingLine5 = new Class_Stat(549, 1234567890, "StackingLine5", false, false, false);

        public Class_Stat StackingLine6 = new Class_Stat(550, 1234567890, "StackingLine6", false, false, false);

        public Class_Stat StackingOrder = new Class_Stat(551, 1234567890, "StackingOrder", false, false, false);

        public Class_Stat ProcNano1 = new Class_Stat(552, 1234567890, "ProcNano1", false, false, false);

        public Class_Stat ProcNano2 = new Class_Stat(553, 1234567890, "ProcNano2", false, false, false);

        public Class_Stat ProcNano3 = new Class_Stat(554, 1234567890, "ProcNano3", false, false, false);

        public Class_Stat ProcNano4 = new Class_Stat(555, 1234567890, "ProcNano4", false, false, false);

        public Class_Stat ProcChance1 = new Class_Stat(556, 1234567890, "ProcChance1", false, false, false);

        public Class_Stat ProcChance2 = new Class_Stat(557, 1234567890, "ProcChance2", false, false, false);

        public Class_Stat ProcChance3 = new Class_Stat(558, 1234567890, "ProcChance3", false, false, false);

        public Class_Stat ProcChance4 = new Class_Stat(559, 1234567890, "ProcChance4", false, false, false);

        public Class_Stat OTArmedForces = new Class_Stat(560, 0, "OTArmedForces", false, false, false);

        public Class_Stat ClanSentinels = new Class_Stat(561, 0, "ClanSentinels", false, false, false);

        public Class_Stat OTMed = new Class_Stat(562, 1234567890, "OTMed", false, false, false);

        public Class_Stat ClanGaia = new Class_Stat(563, 0, "ClanGaia", false, false, false);

        public Class_Stat OTTrans = new Class_Stat(564, 0, "OTTrans", false, false, false);

        public Class_Stat ClanVanguards = new Class_Stat(565, 0, "ClanVanguards", false, false, false);

        public Class_Stat GOS = new Class_Stat(566, 0, "GOS", false, false, false);

        public Class_Stat OTFollowers = new Class_Stat(567, 0, "OTFollowers", false, false, false);

        public Class_Stat OTOperator = new Class_Stat(568, 0, "OTOperator", false, false, false);

        public Class_Stat OTUnredeemed = new Class_Stat(569, 0, "OTUnredeemed", false, false, false);

        public Class_Stat ClanDevoted = new Class_Stat(570, 0, "ClanDevoted", false, false, false);

        public Class_Stat ClanConserver = new Class_Stat(571, 0, "ClanConserver", false, false, false);

        public Class_Stat ClanRedeemed = new Class_Stat(572, 0, "ClanRedeemed", false, false, false);

        public Class_Stat SK = new Class_Stat(573, 0, "SK", false, false, false);

        public Class_Stat LastSK = new Class_Stat(574, 0, "LastSK", false, false, false);

        public Stat_NextSK NextSK = new Stat_NextSK(575, 0, "NextSK", false, false, false);

        public Class_Stat PlayerOptions = new Class_Stat(576, 0, "PlayerOptions", false, false, false);

        public Class_Stat LastPerkResetTime = new Class_Stat(577, 0, "LastPerkResetTime", false, false, false);

        public Class_Stat CurrentTime = new Class_Stat(578, 1234567890, "CurrentTime", false, false, false);

        public Class_Stat ShadowBreedTemplate = new Class_Stat(579, 0, "ShadowBreedTemplate", false, false, false);

        public Class_Stat NPCVicinityFamily = new Class_Stat(580, 1234567890, "NPCVicinityFamily", false, false, false);

        public Class_Stat NPCScriptAMSScale = new Class_Stat(581, 1234567890, "NPCScriptAMSScale", false, false, false);

        public Class_Stat ApartmentsAllowed = new Class_Stat(582, 1, "ApartmentsAllowed", false, false, false);

        public Class_Stat ApartmentsOwned = new Class_Stat(583, 0, "ApartmentsOwned", false, false, false);

        public Class_Stat ApartmentAccessCard = new Class_Stat(
            584, 1234567890, "ApartmentAccessCard", false, false, false);

        public Class_Stat MapAreaPart3 = new Class_Stat(585, 0, "MapAreaPart3", false, false, false);

        public Class_Stat MapAreaPart4 = new Class_Stat(586, 0, "MapAreaPart4", false, false, false);

        public Class_Stat NumberOfTeamMembers = new Class_Stat(
            587, 1234567890, "NumberOfTeamMembers", false, false, false);

        public Class_Stat ActionCategory = new Class_Stat(588, 1234567890, "ActionCategory", false, false, false);

        public Class_Stat CurrentPlayfield = new Class_Stat(589, 1234567890, "CurrentPlayfield", false, false, false);

        public Class_Stat DistrictNano = new Class_Stat(590, 1234567890, "DistrictNano", false, false, false);

        public Class_Stat DistrictNanoInterval = new Class_Stat(
            591, 1234567890, "DistrictNanoInterval", false, false, false);

        public Class_Stat UnsavedXP = new Class_Stat(592, 0, "UnsavedXP", false, false, false);

        public Class_Stat RegainXPPercentage = new Class_Stat(593, 0, "RegainXPPercentage", false, false, false);

        public Class_Stat TempSaveTeamID = new Class_Stat(594, 0, "TempSaveTeamID", false, false, false);

        public Class_Stat TempSavePlayfield = new Class_Stat(595, 0, "TempSavePlayfield", false, false, false);

        public Class_Stat TempSaveX = new Class_Stat(596, 0, "TempSaveX", false, false, false);

        public Class_Stat TempSaveY = new Class_Stat(597, 0, "TempSaveY", false, false, false);

        public Class_Stat ExtendedFlags = new Class_Stat(598, 1234567890, "ExtendedFlags", false, false, false);

        public Class_Stat ShopPrice = new Class_Stat(599, 1234567890, "ShopPrice", false, false, false);

        public Class_Stat NewbieHP = new Class_Stat(600, 1234567890, "NewbieHP", false, false, false);

        public Class_Stat HPLevelUp = new Class_Stat(601, 1234567890, "HPLevelUp", false, false, false);

        public Class_Stat HPPerSkill = new Class_Stat(602, 1234567890, "HPPerSkill", false, false, false);

        public Class_Stat NewbieNP = new Class_Stat(603, 1234567890, "NewbieNP", false, false, false);

        public Class_Stat NPLevelUp = new Class_Stat(604, 1234567890, "NPLevelUp", false, false, false);

        public Class_Stat NPPerSkill = new Class_Stat(605, 1234567890, "NPPerSkill", false, false, false);

        public Class_Stat MaxShopItems = new Class_Stat(606, 1234567890, "MaxShopItems", false, false, false);

        public Class_Stat PlayerID = new Class_Stat(607, 1234567890, "PlayerID", false, true, false);

        public Class_Stat ShopRent = new Class_Stat(608, 1234567890, "ShopRent", false, false, false);

        public Class_Stat SynergyHash = new Class_Stat(609, 1234567890, "SynergyHash", false, false, false);

        public Class_Stat ShopFlags = new Class_Stat(610, 1234567890, "ShopFlags", false, false, false);

        public Class_Stat ShopLastUsed = new Class_Stat(611, 1234567890, "ShopLastUsed", false, false, false);

        public Class_Stat ShopType = new Class_Stat(612, 1234567890, "ShopType", false, false, false);

        public Class_Stat LockDownTime = new Class_Stat(613, 1234567890, "LockDownTime", false, false, false);

        public Class_Stat LeaderLockDownTime = new Class_Stat(
            614, 1234567890, "LeaderLockDownTime", false, false, false);

        public Class_Stat InvadersKilled = new Class_Stat(615, 0, "InvadersKilled", false, false, false);

        public Class_Stat KilledByInvaders = new Class_Stat(616, 0, "KilledByInvaders", false, false, false);

        public Class_Stat MissionBits10 = new Class_Stat(617, 0, "MissionBits10", false, false, false);

        public Class_Stat MissionBits11 = new Class_Stat(618, 0, "MissionBits11", false, false, false);

        public Class_Stat MissionBits12 = new Class_Stat(619, 0, "MissionBits12", false, false, false);

        public Class_Stat HouseTemplate = new Class_Stat(620, 1234567890, "HouseTemplate", false, false, false);

        public Class_Stat PercentFireDamage = new Class_Stat(621, 1234567890, "PercentFireDamage", false, false, false);

        public Class_Stat PercentColdDamage = new Class_Stat(622, 1234567890, "PercentColdDamage", false, false, false);

        public Class_Stat PercentMeleeDamage = new Class_Stat(
            623, 1234567890, "PercentMeleeDamage", false, false, false);

        public Class_Stat PercentProjectileDamage = new Class_Stat(
            624, 1234567890, "PercentProjectileDamage", false, false, false);

        public Class_Stat PercentPoisonDamage = new Class_Stat(
            625, 1234567890, "PercentPoisonDamage", false, false, false);

        public Class_Stat PercentRadiationDamage = new Class_Stat(
            626, 1234567890, "PercentRadiationDamage", false, false, false);

        public Class_Stat PercentEnergyDamage = new Class_Stat(
            627, 1234567890, "PercentEnergyDamage", false, false, false);

        public Class_Stat PercentChemicalDamage = new Class_Stat(
            628, 1234567890, "PercentChemicalDamage", false, false, false);

        public Class_Stat TotalDamage = new Class_Stat(629, 1234567890, "TotalDamage", false, false, false);

        public Class_Stat TrackProjectileDamage = new Class_Stat(
            630, 1234567890, "TrackProjectileDamage", false, false, false);

        public Class_Stat TrackMeleeDamage = new Class_Stat(631, 1234567890, "TrackMeleeDamage", false, false, false);

        public Class_Stat TrackEnergyDamage = new Class_Stat(632, 1234567890, "TrackEnergyDamage", false, false, false);

        public Class_Stat TrackChemicalDamage = new Class_Stat(
            633, 1234567890, "TrackChemicalDamage", false, false, false);

        public Class_Stat TrackRadiationDamage = new Class_Stat(
            634, 1234567890, "TrackRadiationDamage", false, false, false);

        public Class_Stat TrackColdDamage = new Class_Stat(635, 1234567890, "TrackColdDamage", false, false, false);

        public Class_Stat TrackPoisonDamage = new Class_Stat(636, 1234567890, "TrackPoisonDamage", false, false, false);

        public Class_Stat TrackFireDamage = new Class_Stat(637, 1234567890, "TrackFireDamage", false, false, false);

        public Class_Stat NPCSpellArg1 = new Class_Stat(638, 1234567890, "NPCSpellArg1", false, false, false);

        public Class_Stat NPCSpellRet1 = new Class_Stat(639, 1234567890, "NPCSpellRet1", false, false, false);

        public Class_Stat CityInstance = new Class_Stat(640, 1234567890, "CityInstance", false, false, false);

        public Class_Stat DistanceToSpawnpoint = new Class_Stat(
            641, 1234567890, "DistanceToSpawnpoint", false, false, false);

        public Class_Stat CityTerminalRechargePercent = new Class_Stat(
            642, 1234567890, "CityTerminalRechargePercent", false, false, false);

        public Class_Stat UnreadMailCount = new Class_Stat(649, 0, "UnreadMailCount", false, false, false);

        public Class_Stat LastMailCheckTime = new Class_Stat(650, 1283065897, "LastMailCheckTime", false, false, false);

        public Class_Stat AdvantageHash1 = new Class_Stat(651, 1234567890, "AdvantageHash1", false, false, false);

        public Class_Stat AdvantageHash2 = new Class_Stat(652, 1234567890, "AdvantageHash2", false, false, false);

        public Class_Stat AdvantageHash3 = new Class_Stat(653, 1234567890, "AdvantageHash3", false, false, false);

        public Class_Stat AdvantageHash4 = new Class_Stat(654, 1234567890, "AdvantageHash4", false, false, false);

        public Class_Stat AdvantageHash5 = new Class_Stat(655, 1234567890, "AdvantageHash5", false, false, false);

        public Class_Stat ShopIndex = new Class_Stat(656, 1234567890, "ShopIndex", false, false, false);

        public Class_Stat ShopID = new Class_Stat(657, 1234567890, "ShopID", false, false, false);

        public Class_Stat IsVehicle = new Class_Stat(658, 1234567890, "IsVehicle", false, false, false);

        public Class_Stat DamageToNano = new Class_Stat(659, 1234567890, "DamageToNano", false, false, false);

        public Class_Stat AccountFlags = new Class_Stat(660, 1234567890, "AccountFlags", false, true, false);

        public Class_Stat DamageToNanoMultiplier = new Class_Stat(
            661, 1234567890, "DamageToNanoMultiplier", false, false, false);

        public Class_Stat MechData = new Class_Stat(662, 0, "MechData", false, false, false);

        public Class_Stat VehicleAC = new Class_Stat(664, 1234567890, "VehicleAC", false, false, false);

        public Class_Stat VehicleDamage = new Class_Stat(665, 1234567890, "VehicleDamage", false, false, false);

        public Class_Stat VehicleHealth = new Class_Stat(666, 1234567890, "VehicleHealth", false, false, false);

        public Class_Stat VehicleSpeed = new Class_Stat(667, 1234567890, "VehicleSpeed", false, false, false);

        public Class_Stat BattlestationSide = new Class_Stat(668, 0, "BattlestationSide", false, false, false);

        public Class_Stat VP = new Class_Stat(669, 0, "VP", false, false, false);

        public Class_Stat BattlestationRep = new Class_Stat(670, 10, "BattlestationRep", false, false, false);

        public Class_Stat PetState = new Class_Stat(671, 1234567890, "PetState", false, false, false);

        public Class_Stat PaidPoints = new Class_Stat(672, 0, "PaidPoints", false, false, false);

        public Class_Stat VisualFlags = new Class_Stat(673, 31, "VisualFlags", false, false, false);

        public Class_Stat PVPDuelKills = new Class_Stat(674, 0, "PVPDuelKills", false, false, false);

        public Class_Stat PVPDuelDeaths = new Class_Stat(675, 0, "PVPDuelDeaths", false, false, false);

        public Class_Stat PVPProfessionDuelKills = new Class_Stat(676, 0, "PVPProfessionDuelKills", false, false, false);

        public Class_Stat PVPProfessionDuelDeaths = new Class_Stat(
            677, 0, "PVPProfessionDuelDeaths", false, false, false);

        public Class_Stat PVPRankedSoloKills = new Class_Stat(678, 0, "PVPRankedSoloKills", false, false, false);

        public Class_Stat PVPRankedSoloDeaths = new Class_Stat(679, 0, "PVPRankedSoloDeaths", false, false, false);

        public Class_Stat PVPRankedTeamKills = new Class_Stat(680, 0, "PVPRankedTeamKills", false, false, false);

        public Class_Stat PVPRankedTeamDeaths = new Class_Stat(681, 0, "PVPRankedTeamDeaths", false, false, false);

        public Class_Stat PVPSoloScore = new Class_Stat(682, 0, "PVPSoloScore", false, false, false);

        public Class_Stat PVPTeamScore = new Class_Stat(683, 0, "PVPTeamScore", false, false, false);

        public Class_Stat PVPDuelScore = new Class_Stat(684, 0, "PVPDuelScore", false, false, false);

        public Class_Stat ACGItemSeed = new Class_Stat(700, 1234567890, "ACGItemSeed", false, false, false);

        public Class_Stat ACGItemLevel = new Class_Stat(701, 1234567890, "ACGItemLevel", false, false, false);

        public Class_Stat ACGItemTemplateID = new Class_Stat(702, 1234567890, "ACGItemTemplateID", false, false, false);

        public Class_Stat ACGItemTemplateID2 = new Class_Stat(
            703, 1234567890, "ACGItemTemplateID2", false, false, false);

        public Class_Stat ACGItemCategoryID = new Class_Stat(704, 1234567890, "ACGItemCategoryID", false, false, false);

        public Class_Stat HasKnuBotData = new Class_Stat(768, 1234567890, "HasKnuBotData", false, false, false);

        public Class_Stat QuestBoothDifficulty = new Class_Stat(
            800, 1234567890, "QuestBoothDifficulty", false, false, false);

        public Class_Stat QuestASMinimumRange = new Class_Stat(
            801, 1234567890, "QuestASMinimumRange", false, false, false);

        public Class_Stat QuestASMaximumRange = new Class_Stat(
            802, 1234567890, "QuestASMaximumRange", false, false, false);

        public Class_Stat VisualLODLevel = new Class_Stat(888, 1234567890, "VisualLODLevel", false, false, false);

        public Class_Stat TargetDistanceChange = new Class_Stat(
            889, 1234567890, "TargetDistanceChange", false, false, false);

        public Class_Stat TideRequiredDynelID = new Class_Stat(
            900, 1234567890, "TideRequiredDynelID", false, false, false);

        public Class_Stat StreamCheckMagic = new Class_Stat(999, 1234567890, "StreamCheckMagic", false, false, false);

        public Class_Stat Type = new Class_Stat(1001, 1234567890, "Type", false, true, false);

        public Class_Stat Instance = new Class_Stat(1002, 1234567890, "Instance", false, true, false);

        public Class_Stat WeaponsStyle = new Class_Stat(1003, 1234567890, "WeaponType", false, false, false);

        public Class_Stat ShoulderMeshRight = new Class_Stat(1004, 0, "ShoulderMeshRight", false, false, false);

        public Class_Stat ShoulderMeshLeft = new Class_Stat(1005, 0, "ShoulderMeshLeft", false, false, false);

        public Class_Stat WeaponMeshRight = new Class_Stat(1006, 0, "WeaponMeshRight", false, false, false);

        public Class_Stat WeaponMeshLeft = new Class_Stat(1007, 0, "WeaponMeshLeft", false, false, false);

        public Class_Stat OverrideTextureHead = new Class_Stat(1008, 0, "OverrideTextureHead", false, false, false);

        public Class_Stat OverrideTextureWeaponRight = new Class_Stat(
            1009, 0, "OverrideTextureWeaponRight", false, false, false);

        public Class_Stat OverrideTextureWeaponLeft = new Class_Stat(
            1010, 0, "OverrideTextureWeaponLeft", false, false, false);

        public Class_Stat OverrideTextureShoulderpadRight = new Class_Stat(
            1011, 0, "OverrideTextureShoulderpadRight", false, false, false);

        public Class_Stat OverrideTextureShoulderpadLeft = new Class_Stat(
            1012, 0, "OverrideTextureShoulderpadLeft", false, false, false);

        public Class_Stat OverrideTextureBack = new Class_Stat(1013, 0, "OverrideTextureBack", false, false, false);

        public Class_Stat OverrideTextureAttractor = new Class_Stat(
            1014, 0, "OverrideTextureAttractor", false, false, false);

        public Class_Stat WeaponStyleLeft = new Class_Stat(1015, 0, "WeaponStyleLeft", false, false, false);

        public Class_Stat WeaponStyleRight = new Class_Stat(1016, 0, "WeaponStyleRight", false, false, false);
        #endregion

        public List<Class_Stat> all = new List<Class_Stat>();

        #region Create Stats
        /// <summary>
        /// Character_Stats
        /// Class for character's stats
        /// </summary>
        /// <param name="parent">Stat's owner (Character or derived class)</param>
        public Character_Stats(Character parent)
        {
            #region Add stats to list
            this.all.Add(this.Flags);
            this.all.Add(this.Life);
            this.all.Add(this.VolumeMass);
            this.all.Add(this.AttackSpeed);
            this.all.Add(this.Breed);
            this.all.Add(this.Clan);
            this.all.Add(this.Team);
            this.all.Add(this.State);
            this.all.Add(this.TimeExist);
            this.all.Add(this.MapFlags);
            this.all.Add(this.ProfessionLevel);
            this.all.Add(this.PreviousHealth);
            this.all.Add(this.Mesh);
            this.all.Add(this.Anim);
            this.all.Add(this.Name);
            this.all.Add(this.Info);
            this.all.Add(this.Strength);
            this.all.Add(this.Agility);
            this.all.Add(this.Stamina);
            this.all.Add(this.Intelligence);
            this.all.Add(this.Sense);
            this.all.Add(this.Psychic);
            this.all.Add(this.AMS);
            this.all.Add(this.StaticInstance);
            this.all.Add(this.MaxMass);
            this.all.Add(this.StaticType);
            this.all.Add(this.Energy);
            this.all.Add(this.Health);
            this.all.Add(this.Height);
            this.all.Add(this.DMS);
            this.all.Add(this.Can);
            this.all.Add(this.Face);
            this.all.Add(this.HairMesh);
            this.all.Add(this.Side);
            this.all.Add(this.DeadTimer);
            this.all.Add(this.AccessCount);
            this.all.Add(this.AttackCount);
            this.all.Add(this.TitleLevel);
            this.all.Add(this.BackMesh);
            this.all.Add(this.AlienXP);
            this.all.Add(this.FabricType);
            this.all.Add(this.CATMesh);
            this.all.Add(this.ParentType);
            this.all.Add(this.ParentInstance);
            this.all.Add(this.BeltSlots);
            this.all.Add(this.BandolierSlots);
            this.all.Add(this.Fatness);
            this.all.Add(this.ClanLevel);
            this.all.Add(this.InsuranceTime);
            this.all.Add(this.InventoryTimeout);
            this.all.Add(this.AggDef);
            this.all.Add(this.XP);
            this.all.Add(this.IP);
            this.all.Add(this.Level);
            this.all.Add(this.InventoryId);
            this.all.Add(this.TimeSinceCreation);
            this.all.Add(this.LastXP);
            this.all.Add(this.Age);
            this.all.Add(this.Sex);
            this.all.Add(this.Profession);
            this.all.Add(this.Cash);
            this.all.Add(this.Alignment);
            this.all.Add(this.Attitude);
            this.all.Add(this.HeadMesh);
            this.all.Add(this.MissionBits5);
            this.all.Add(this.MissionBits6);
            this.all.Add(this.MissionBits7);
            this.all.Add(this.VeteranPoints);
            this.all.Add(this.MonthsPaid);
            this.all.Add(this.SpeedPenalty);
            this.all.Add(this.TotalMass);
            this.all.Add(this.ItemType);
            this.all.Add(this.RepairDifficulty);
            this.all.Add(this.Price);
            this.all.Add(this.MetaType);
            this.all.Add(this.ItemClass);
            this.all.Add(this.RepairSkill);
            this.all.Add(this.CurrentMass);
            this.all.Add(this.Icon);
            this.all.Add(this.PrimaryItemType);
            this.all.Add(this.PrimaryItemInstance);
            this.all.Add(this.SecondaryItemType);
            this.all.Add(this.SecondaryItemInstance);
            this.all.Add(this.UserType);
            this.all.Add(this.UserInstance);
            this.all.Add(this.AreaType);
            this.all.Add(this.AreaInstance);
            this.all.Add(this.DefaultPos);
            this.all.Add(this.Race);
            this.all.Add(this.ProjectileAC);
            this.all.Add(this.MeleeAC);
            this.all.Add(this.EnergyAC);
            this.all.Add(this.ChemicalAC);
            this.all.Add(this.RadiationAC);
            this.all.Add(this.ColdAC);
            this.all.Add(this.PoisonAC);
            this.all.Add(this.FireAC);
            this.all.Add(this.StateAction);
            this.all.Add(this.ItemAnim);
            this.all.Add(this.MartialArts);
            this.all.Add(this.MeleeMultiple);
            this.all.Add(this.OnehBluntWeapons);
            this.all.Add(this.OnehEdgedWeapon);
            this.all.Add(this.MeleeEnergyWeapon);
            this.all.Add(this.TwohEdgedWeapons);
            this.all.Add(this.Piercing);
            this.all.Add(this.TwohBluntWeapons);
            this.all.Add(this.ThrowingKnife);
            this.all.Add(this.Grenade);
            this.all.Add(this.ThrownGrapplingWeapons);
            this.all.Add(this.Bow);
            this.all.Add(this.Pistol);
            this.all.Add(this.Rifle);
            this.all.Add(this.SubMachineGun);
            this.all.Add(this.Shotgun);
            this.all.Add(this.AssaultRifle);
            this.all.Add(this.DriveWater);
            this.all.Add(this.CloseCombatInitiative);
            this.all.Add(this.DistanceWeaponInitiative);
            this.all.Add(this.PhysicalProwessInitiative);
            this.all.Add(this.BowSpecialAttack);
            this.all.Add(this.SenseImprovement);
            this.all.Add(this.FirstAid);
            this.all.Add(this.Treatment);
            this.all.Add(this.MechanicalEngineering);
            this.all.Add(this.ElectricalEngineering);
            this.all.Add(this.MaterialMetamorphose);
            this.all.Add(this.BiologicalMetamorphose);
            this.all.Add(this.PsychologicalModification);
            this.all.Add(this.MaterialCreation);
            this.all.Add(this.MaterialLocation);
            this.all.Add(this.NanoEnergyPool);
            this.all.Add(this.LR_EnergyWeapon);
            this.all.Add(this.LR_MultipleWeapon);
            this.all.Add(this.DisarmTrap);
            this.all.Add(this.Perception);
            this.all.Add(this.Adventuring);
            this.all.Add(this.Swim);
            this.all.Add(this.DriveAir);
            this.all.Add(this.MapNavigation);
            this.all.Add(this.Tutoring);
            this.all.Add(this.Brawl);
            this.all.Add(this.Riposte);
            this.all.Add(this.Dimach);
            this.all.Add(this.Parry);
            this.all.Add(this.SneakAttack);
            this.all.Add(this.FastAttack);
            this.all.Add(this.Burst);
            this.all.Add(this.NanoProwessInitiative);
            this.all.Add(this.FlingShot);
            this.all.Add(this.AimedShot);
            this.all.Add(this.BodyDevelopment);
            this.all.Add(this.Duck);
            this.all.Add(this.Dodge);
            this.all.Add(this.Evade);
            this.all.Add(this.RunSpeed);
            this.all.Add(this.FieldQuantumPhysics);
            this.all.Add(this.WeaponSmithing);
            this.all.Add(this.Pharmaceuticals);
            this.all.Add(this.NanoProgramming);
            this.all.Add(this.ComputerLiteracy);
            this.all.Add(this.Psychology);
            this.all.Add(this.Chemistry);
            this.all.Add(this.Concealment);
            this.all.Add(this.BreakingEntry);
            this.all.Add(this.DriveGround);
            this.all.Add(this.FullAuto);
            this.all.Add(this.NanoAC);
            this.all.Add(this.AlienLevel);
            this.all.Add(this.HealthChangeBest);
            this.all.Add(this.HealthChangeWorst);
            this.all.Add(this.HealthChange);
            this.all.Add(this.CurrentMovementMode);
            this.all.Add(this.PrevMovementMode);
            this.all.Add(this.AutoLockTimeDefault);
            this.all.Add(this.AutoUnlockTimeDefault);
            this.all.Add(this.MoreFlags);
            this.all.Add(this.AlienNextXP);
            this.all.Add(this.NPCFlags);
            this.all.Add(this.CurrentNCU);
            this.all.Add(this.MaxNCU);
            this.all.Add(this.Specialization);
            this.all.Add(this.EffectIcon);
            this.all.Add(this.BuildingType);
            this.all.Add(this.BuildingInstance);
            this.all.Add(this.CardOwnerType);
            this.all.Add(this.CardOwnerInstance);
            this.all.Add(this.BuildingComplexInst);
            this.all.Add(this.ExitInstance);
            this.all.Add(this.NextDoorInBuilding);
            this.all.Add(this.LastConcretePlayfieldInstance);
            this.all.Add(this.ExtenalPlayfieldInstance);
            this.all.Add(this.ExtenalDoorInstance);
            this.all.Add(this.InPlay);
            this.all.Add(this.AccessKey);
            this.all.Add(this.PetMaster);
            this.all.Add(this.OrientationMode);
            this.all.Add(this.SessionTime);
            this.all.Add(this.RP);
            this.all.Add(this.Conformity);
            this.all.Add(this.Aggressiveness);
            this.all.Add(this.Stability);
            this.all.Add(this.Extroverty);
            this.all.Add(this.BreedHostility);
            this.all.Add(this.ReflectProjectileAC);
            this.all.Add(this.ReflectMeleeAC);
            this.all.Add(this.ReflectEnergyAC);
            this.all.Add(this.ReflectChemicalAC);
            this.all.Add(this.RechargeDelay);
            this.all.Add(this.EquipDelay);
            this.all.Add(this.MaxEnergy);
            this.all.Add(this.TeamSide);
            this.all.Add(this.CurrentNano);
            this.all.Add(this.GmLevel);
            this.all.Add(this.ReflectRadiationAC);
            this.all.Add(this.ReflectColdAC);
            this.all.Add(this.ReflectNanoAC);
            this.all.Add(this.ReflectFireAC);
            this.all.Add(this.CurrBodyLocation);
            this.all.Add(this.MaxNanoEnergy);
            this.all.Add(this.AccumulatedDamage);
            this.all.Add(this.CanChangeClothes);
            this.all.Add(this.Features);
            this.all.Add(this.ReflectPoisonAC);
            this.all.Add(this.ShieldProjectileAC);
            this.all.Add(this.ShieldMeleeAC);
            this.all.Add(this.ShieldEnergyAC);
            this.all.Add(this.ShieldChemicalAC);
            this.all.Add(this.ShieldRadiationAC);
            this.all.Add(this.ShieldColdAC);
            this.all.Add(this.ShieldNanoAC);
            this.all.Add(this.ShieldFireAC);
            this.all.Add(this.ShieldPoisonAC);
            this.all.Add(this.BerserkMode);
            this.all.Add(this.InsurancePercentage);
            this.all.Add(this.ChangeSideCount);
            this.all.Add(this.AbsorbProjectileAC);
            this.all.Add(this.AbsorbMeleeAC);
            this.all.Add(this.AbsorbEnergyAC);
            this.all.Add(this.AbsorbChemicalAC);
            this.all.Add(this.AbsorbRadiationAC);
            this.all.Add(this.AbsorbColdAC);
            this.all.Add(this.AbsorbFireAC);
            this.all.Add(this.AbsorbPoisonAC);
            this.all.Add(this.AbsorbNanoAC);
            this.all.Add(this.TemporarySkillReduction);
            this.all.Add(this.BirthDate);
            this.all.Add(this.LastSaved);
            this.all.Add(this.SoundVolume);
            this.all.Add(this.PetCounter);
            this.all.Add(this.MeetersWalked);
            this.all.Add(this.QuestLevelsSolved);
            this.all.Add(this.MonsterLevelsKilled);
            this.all.Add(this.PvPLevelsKilled);
            this.all.Add(this.MissionBits1);
            this.all.Add(this.MissionBits2);
            this.all.Add(this.AccessGrant);
            this.all.Add(this.DoorFlags);
            this.all.Add(this.ClanHierarchy);
            this.all.Add(this.QuestStat);
            this.all.Add(this.ClientActivated);
            this.all.Add(this.PersonalResearchLevel);
            this.all.Add(this.GlobalResearchLevel);
            this.all.Add(this.PersonalResearchGoal);
            this.all.Add(this.GlobalResearchGoal);
            this.all.Add(this.TurnSpeed);
            this.all.Add(this.LiquidType);
            this.all.Add(this.GatherSound);
            this.all.Add(this.CastSound);
            this.all.Add(this.TravelSound);
            this.all.Add(this.HitSound);
            this.all.Add(this.SecondaryItemTemplate);
            this.all.Add(this.EquippedWeapons);
            this.all.Add(this.XPKillRange);
            this.all.Add(this.AMSModifier);
            this.all.Add(this.DMSModifier);
            this.all.Add(this.ProjectileDamageModifier);
            this.all.Add(this.MeleeDamageModifier);
            this.all.Add(this.EnergyDamageModifier);
            this.all.Add(this.ChemicalDamageModifier);
            this.all.Add(this.RadiationDamageModifier);
            this.all.Add(this.ItemHateValue);
            this.all.Add(this.DamageBonus);
            this.all.Add(this.MaxDamage);
            this.all.Add(this.MinDamage);
            this.all.Add(this.AttackRange);
            this.all.Add(this.HateValueModifyer);
            this.all.Add(this.TrapDifficulty);
            this.all.Add(this.StatOne);
            this.all.Add(this.NumAttackEffects);
            this.all.Add(this.DefaultAttackType);
            this.all.Add(this.ItemSkill);
            this.all.Add(this.ItemDelay);
            this.all.Add(this.ItemOpposedSkill);
            this.all.Add(this.ItemSIS);
            this.all.Add(this.InteractionRadius);
            this.all.Add(this.Placement);
            this.all.Add(this.LockDifficulty);
            this.all.Add(this.Members);
            this.all.Add(this.MinMembers);
            this.all.Add(this.ClanPrice);
            this.all.Add(this.MissionBits3);
            this.all.Add(this.ClanType);
            this.all.Add(this.ClanInstance);
            this.all.Add(this.VoteCount);
            this.all.Add(this.MemberType);
            this.all.Add(this.MemberInstance);
            this.all.Add(this.GlobalClanType);
            this.all.Add(this.GlobalClanInstance);
            this.all.Add(this.ColdDamageModifier);
            this.all.Add(this.ClanUpkeepInterval);
            this.all.Add(this.TimeSinceUpkeep);
            this.all.Add(this.ClanFinalized);
            this.all.Add(this.NanoDamageModifier);
            this.all.Add(this.FireDamageModifier);
            this.all.Add(this.PoisonDamageModifier);
            this.all.Add(this.NPCostModifier);
            this.all.Add(this.XPModifier);
            this.all.Add(this.BreedLimit);
            this.all.Add(this.GenderLimit);
            this.all.Add(this.LevelLimit);
            this.all.Add(this.PlayerKilling);
            this.all.Add(this.TeamAllowed);
            this.all.Add(this.WeaponDisallowedType);
            this.all.Add(this.WeaponDisallowedInstance);
            this.all.Add(this.Taboo);
            this.all.Add(this.Compulsion);
            this.all.Add(this.SkillDisabled);
            this.all.Add(this.ClanItemType);
            this.all.Add(this.ClanItemInstance);
            this.all.Add(this.DebuffFormula);
            this.all.Add(this.PvP_Rating);
            this.all.Add(this.SavedXP);
            this.all.Add(this.DoorBlockTime);
            this.all.Add(this.OverrideTexture);
            this.all.Add(this.OverrideMaterial);
            this.all.Add(this.DeathReason);
            this.all.Add(this.DamageOverrideType);
            this.all.Add(this.BrainType);
            this.all.Add(this.XPBonus);
            this.all.Add(this.HealInterval);
            this.all.Add(this.HealDelta);
            this.all.Add(this.MonsterTexture);
            this.all.Add(this.HasAlwaysLootable);
            this.all.Add(this.TradeLimit);
            this.all.Add(this.FaceTexture);
            this.all.Add(this.SpecialCondition);
            this.all.Add(this.AutoAttackFlags);
            this.all.Add(this.NextXP);
            this.all.Add(this.TeleportPauseMilliSeconds);
            this.all.Add(this.SISCap);
            this.all.Add(this.AnimSet);
            this.all.Add(this.AttackType);
            this.all.Add(this.NanoFocusLevel);
            this.all.Add(this.NPCHash);
            this.all.Add(this.CollisionRadius);
            this.all.Add(this.OuterRadius);
            this.all.Add(this.MonsterData);
            this.all.Add(this.MonsterScale);
            this.all.Add(this.HitEffectType);
            this.all.Add(this.ResurrectDest);
            this.all.Add(this.NanoInterval);
            this.all.Add(this.NanoDelta);
            this.all.Add(this.ReclaimItem);
            this.all.Add(this.GatherEffectType);
            this.all.Add(this.VisualBreed);
            this.all.Add(this.VisualProfession);
            this.all.Add(this.VisualSex);
            this.all.Add(this.RitualTargetInst);
            this.all.Add(this.SkillTimeOnSelectedTarget);
            this.all.Add(this.LastSaveXP);
            this.all.Add(this.ExtendedTime);
            this.all.Add(this.BurstRecharge);
            this.all.Add(this.FullAutoRecharge);
            this.all.Add(this.GatherAbstractAnim);
            this.all.Add(this.CastTargetAbstractAnim);
            this.all.Add(this.CastSelfAbstractAnim);
            this.all.Add(this.CriticalIncrease);
            this.all.Add(this.RangeIncreaserWeapon);
            this.all.Add(this.RangeIncreaserNF);
            this.all.Add(this.SkillLockModifier);
            this.all.Add(this.InterruptModifier);
            this.all.Add(this.ACGEntranceStyles);
            this.all.Add(this.ChanceOfBreakOnSpellAttack);
            this.all.Add(this.ChanceOfBreakOnDebuff);
            this.all.Add(this.DieAnim);
            this.all.Add(this.TowerType);
            this.all.Add(this.Expansion);
            this.all.Add(this.LowresMesh);
            this.all.Add(this.CriticalDecrease);
            this.all.Add(this.OldTimeExist);
            this.all.Add(this.ResistModifier);
            this.all.Add(this.ChestFlags);
            this.all.Add(this.PrimaryTemplateID);
            this.all.Add(this.NumberOfItems);
            this.all.Add(this.SelectedTargetType);
            this.all.Add(this.Corpse_Hash);
            this.all.Add(this.AmmoName);
            this.all.Add(this.Rotation);
            this.all.Add(this.CATAnim);
            this.all.Add(this.CATAnimFlags);
            this.all.Add(this.DisplayCATAnim);
            this.all.Add(this.DisplayCATMesh);
            this.all.Add(this.School);
            this.all.Add(this.NanoSpeed);
            this.all.Add(this.NanoPoints);
            this.all.Add(this.TrainSkill);
            this.all.Add(this.TrainSkillCost);
            this.all.Add(this.IsFightingMe);
            this.all.Add(this.NextFormula);
            this.all.Add(this.MultipleCount);
            this.all.Add(this.EffectType);
            this.all.Add(this.ImpactEffectType);
            this.all.Add(this.CorpseType);
            this.all.Add(this.CorpseInstance);
            this.all.Add(this.CorpseAnimKey);
            this.all.Add(this.UnarmedTemplateInstance);
            this.all.Add(this.TracerEffectType);
            this.all.Add(this.AmmoType);
            this.all.Add(this.CharRadius);
            this.all.Add(this.ChanceOfUse);
            this.all.Add(this.CurrentState);
            this.all.Add(this.ArmourType);
            this.all.Add(this.RestModifier);
            this.all.Add(this.BuyModifier);
            this.all.Add(this.SellModifier);
            this.all.Add(this.CastEffectType);
            this.all.Add(this.NPCBrainState);
            this.all.Add(this.WaitState);
            this.all.Add(this.SelectedTarget);
            this.all.Add(this.MissionBits4);
            this.all.Add(this.OwnerInstance);
            this.all.Add(this.CharState);
            this.all.Add(this.ReadOnly);
            this.all.Add(this.DamageType);
            this.all.Add(this.CollideCheckInterval);
            this.all.Add(this.PlayfieldType);
            this.all.Add(this.NPCCommand);
            this.all.Add(this.InitiativeType);
            this.all.Add(this.CharTmp1);
            this.all.Add(this.CharTmp2);
            this.all.Add(this.CharTmp3);
            this.all.Add(this.CharTmp4);
            this.all.Add(this.NPCCommandArg);
            this.all.Add(this.NameTemplate);
            this.all.Add(this.DesiredTargetDistance);
            this.all.Add(this.VicinityRange);
            this.all.Add(this.NPCIsSurrendering);
            this.all.Add(this.StateMachine);
            this.all.Add(this.NPCSurrenderInstance);
            this.all.Add(this.NPCHasPatrolList);
            this.all.Add(this.NPCVicinityChars);
            this.all.Add(this.ProximityRangeOutdoors);
            this.all.Add(this.NPCFamily);
            this.all.Add(this.CommandRange);
            this.all.Add(this.NPCHatelistSize);
            this.all.Add(this.NPCNumPets);
            this.all.Add(this.ODMinSizeAdd);
            this.all.Add(this.EffectRed);
            this.all.Add(this.EffectGreen);
            this.all.Add(this.EffectBlue);
            this.all.Add(this.ODMaxSizeAdd);
            this.all.Add(this.DurationModifier);
            this.all.Add(this.NPCCryForHelpRange);
            this.all.Add(this.LOSHeight);
            this.all.Add(this.PetReq1);
            this.all.Add(this.PetReq2);
            this.all.Add(this.PetReq3);
            this.all.Add(this.MapOptions);
            this.all.Add(this.MapAreaPart1);
            this.all.Add(this.MapAreaPart2);
            this.all.Add(this.FixtureFlags);
            this.all.Add(this.FallDamage);
            this.all.Add(this.ReflectReturnedProjectileAC);
            this.all.Add(this.ReflectReturnedMeleeAC);
            this.all.Add(this.ReflectReturnedEnergyAC);
            this.all.Add(this.ReflectReturnedChemicalAC);
            this.all.Add(this.ReflectReturnedRadiationAC);
            this.all.Add(this.ReflectReturnedColdAC);
            this.all.Add(this.ReflectReturnedNanoAC);
            this.all.Add(this.ReflectReturnedFireAC);
            this.all.Add(this.ReflectReturnedPoisonAC);
            this.all.Add(this.ProximityRangeIndoors);
            this.all.Add(this.PetReqVal1);
            this.all.Add(this.PetReqVal2);
            this.all.Add(this.PetReqVal3);
            this.all.Add(this.TargetFacing);
            this.all.Add(this.Backstab);
            this.all.Add(this.OriginatorType);
            this.all.Add(this.QuestInstance);
            this.all.Add(this.QuestIndex1);
            this.all.Add(this.QuestIndex2);
            this.all.Add(this.QuestIndex3);
            this.all.Add(this.QuestIndex4);
            this.all.Add(this.QuestIndex5);
            this.all.Add(this.QTDungeonInstance);
            this.all.Add(this.QTNumMonsters);
            this.all.Add(this.QTKilledMonsters);
            this.all.Add(this.AnimPos);
            this.all.Add(this.AnimPlay);
            this.all.Add(this.AnimSpeed);
            this.all.Add(this.QTKillNumMonsterID1);
            this.all.Add(this.QTKillNumMonsterCount1);
            this.all.Add(this.QTKillNumMonsterID2);
            this.all.Add(this.QTKillNumMonsterCount2);
            this.all.Add(this.QTKillNumMonsterID3);
            this.all.Add(this.QTKillNumMonsterCount3);
            this.all.Add(this.QuestIndex0);
            this.all.Add(this.QuestTimeout);
            this.all.Add(this.Tower_NPCHash);
            this.all.Add(this.PetType);
            this.all.Add(this.OnTowerCreation);
            this.all.Add(this.OwnedTowers);
            this.all.Add(this.TowerInstance);
            this.all.Add(this.AttackShield);
            this.all.Add(this.SpecialAttackShield);
            this.all.Add(this.NPCVicinityPlayers);
            this.all.Add(this.NPCUseFightModeRegenRate);
            this.all.Add(this.Rnd);
            this.all.Add(this.SocialStatus);
            this.all.Add(this.LastRnd);
            this.all.Add(this.ItemDelayCap);
            this.all.Add(this.RechargeDelayCap);
            this.all.Add(this.PercentRemainingHealth);
            this.all.Add(this.PercentRemainingNano);
            this.all.Add(this.TargetDistance);
            this.all.Add(this.TeamCloseness);
            this.all.Add(this.NumberOnHateList);
            this.all.Add(this.ConditionState);
            this.all.Add(this.ExpansionPlayfield);
            this.all.Add(this.ShadowBreed);
            this.all.Add(this.NPCFovStatus);
            this.all.Add(this.DudChance);
            this.all.Add(this.HealMultiplier);
            this.all.Add(this.NanoDamageMultiplier);
            this.all.Add(this.NanoVulnerability);
            this.all.Add(this.AmsCap);
            this.all.Add(this.ProcInitiative1);
            this.all.Add(this.ProcInitiative2);
            this.all.Add(this.ProcInitiative3);
            this.all.Add(this.ProcInitiative4);
            this.all.Add(this.FactionModifier);
            this.all.Add(this.MissionBits8);
            this.all.Add(this.MissionBits9);
            this.all.Add(this.StackingLine2);
            this.all.Add(this.StackingLine3);
            this.all.Add(this.StackingLine4);
            this.all.Add(this.StackingLine5);
            this.all.Add(this.StackingLine6);
            this.all.Add(this.StackingOrder);
            this.all.Add(this.ProcNano1);
            this.all.Add(this.ProcNano2);
            this.all.Add(this.ProcNano3);
            this.all.Add(this.ProcNano4);
            this.all.Add(this.ProcChance1);
            this.all.Add(this.ProcChance2);
            this.all.Add(this.ProcChance3);
            this.all.Add(this.ProcChance4);
            this.all.Add(this.OTArmedForces);
            this.all.Add(this.ClanSentinels);
            this.all.Add(this.OTMed);
            this.all.Add(this.ClanGaia);
            this.all.Add(this.OTTrans);
            this.all.Add(this.ClanVanguards);
            this.all.Add(this.GOS);
            this.all.Add(this.OTFollowers);
            this.all.Add(this.OTOperator);
            this.all.Add(this.OTUnredeemed);
            this.all.Add(this.ClanDevoted);
            this.all.Add(this.ClanConserver);
            this.all.Add(this.ClanRedeemed);
            this.all.Add(this.SK);
            this.all.Add(this.LastSK);
            this.all.Add(this.NextSK);
            this.all.Add(this.PlayerOptions);
            this.all.Add(this.LastPerkResetTime);
            this.all.Add(this.CurrentTime);
            this.all.Add(this.ShadowBreedTemplate);
            this.all.Add(this.NPCVicinityFamily);
            this.all.Add(this.NPCScriptAMSScale);
            this.all.Add(this.ApartmentsAllowed);
            this.all.Add(this.ApartmentsOwned);
            this.all.Add(this.ApartmentAccessCard);
            this.all.Add(this.MapAreaPart3);
            this.all.Add(this.MapAreaPart4);
            this.all.Add(this.NumberOfTeamMembers);
            this.all.Add(this.ActionCategory);
            this.all.Add(this.CurrentPlayfield);
            this.all.Add(this.DistrictNano);
            this.all.Add(this.DistrictNanoInterval);
            this.all.Add(this.UnsavedXP);
            this.all.Add(this.RegainXPPercentage);
            this.all.Add(this.TempSaveTeamID);
            this.all.Add(this.TempSavePlayfield);
            this.all.Add(this.TempSaveX);
            this.all.Add(this.TempSaveY);
            this.all.Add(this.ExtendedFlags);
            this.all.Add(this.ShopPrice);
            this.all.Add(this.NewbieHP);
            this.all.Add(this.HPLevelUp);
            this.all.Add(this.HPPerSkill);
            this.all.Add(this.NewbieNP);
            this.all.Add(this.NPLevelUp);
            this.all.Add(this.NPPerSkill);
            this.all.Add(this.MaxShopItems);
            this.all.Add(this.PlayerID);
            this.all.Add(this.ShopRent);
            this.all.Add(this.SynergyHash);
            this.all.Add(this.ShopFlags);
            this.all.Add(this.ShopLastUsed);
            this.all.Add(this.ShopType);
            this.all.Add(this.LockDownTime);
            this.all.Add(this.LeaderLockDownTime);
            this.all.Add(this.InvadersKilled);
            this.all.Add(this.KilledByInvaders);
            this.all.Add(this.MissionBits10);
            this.all.Add(this.MissionBits11);
            this.all.Add(this.MissionBits12);
            this.all.Add(this.HouseTemplate);
            this.all.Add(this.PercentFireDamage);
            this.all.Add(this.PercentColdDamage);
            this.all.Add(this.PercentMeleeDamage);
            this.all.Add(this.PercentProjectileDamage);
            this.all.Add(this.PercentPoisonDamage);
            this.all.Add(this.PercentRadiationDamage);
            this.all.Add(this.PercentEnergyDamage);
            this.all.Add(this.PercentChemicalDamage);
            this.all.Add(this.TotalDamage);
            this.all.Add(this.TrackProjectileDamage);
            this.all.Add(this.TrackMeleeDamage);
            this.all.Add(this.TrackEnergyDamage);
            this.all.Add(this.TrackChemicalDamage);
            this.all.Add(this.TrackRadiationDamage);
            this.all.Add(this.TrackColdDamage);
            this.all.Add(this.TrackPoisonDamage);
            this.all.Add(this.TrackFireDamage);
            this.all.Add(this.NPCSpellArg1);
            this.all.Add(this.NPCSpellRet1);
            this.all.Add(this.CityInstance);
            this.all.Add(this.DistanceToSpawnpoint);
            this.all.Add(this.CityTerminalRechargePercent);
            this.all.Add(this.UnreadMailCount);
            this.all.Add(this.LastMailCheckTime);
            this.all.Add(this.AdvantageHash1);
            this.all.Add(this.AdvantageHash2);
            this.all.Add(this.AdvantageHash3);
            this.all.Add(this.AdvantageHash4);
            this.all.Add(this.AdvantageHash5);
            this.all.Add(this.ShopIndex);
            this.all.Add(this.ShopID);
            this.all.Add(this.IsVehicle);
            this.all.Add(this.DamageToNano);
            this.all.Add(this.AccountFlags);
            this.all.Add(this.DamageToNanoMultiplier);
            this.all.Add(this.MechData);
            this.all.Add(this.VehicleAC);
            this.all.Add(this.VehicleDamage);
            this.all.Add(this.VehicleHealth);
            this.all.Add(this.VehicleSpeed);
            this.all.Add(this.BattlestationSide);
            this.all.Add(this.VP);
            this.all.Add(this.BattlestationRep);
            this.all.Add(this.PetState);
            this.all.Add(this.PaidPoints);
            this.all.Add(this.VisualFlags);
            this.all.Add(this.PVPDuelKills);
            this.all.Add(this.PVPDuelDeaths);
            this.all.Add(this.PVPProfessionDuelKills);
            this.all.Add(this.PVPProfessionDuelDeaths);
            this.all.Add(this.PVPRankedSoloKills);
            this.all.Add(this.PVPRankedSoloDeaths);
            this.all.Add(this.PVPRankedTeamKills);
            this.all.Add(this.PVPRankedTeamDeaths);
            this.all.Add(this.PVPSoloScore);
            this.all.Add(this.PVPTeamScore);
            this.all.Add(this.PVPDuelScore);
            this.all.Add(this.ACGItemSeed);
            this.all.Add(this.ACGItemLevel);
            this.all.Add(this.ACGItemTemplateID);
            this.all.Add(this.ACGItemTemplateID2);
            this.all.Add(this.ACGItemCategoryID);
            this.all.Add(this.HasKnuBotData);
            this.all.Add(this.QuestBoothDifficulty);
            this.all.Add(this.QuestASMinimumRange);
            this.all.Add(this.QuestASMaximumRange);
            this.all.Add(this.VisualLODLevel);
            this.all.Add(this.TargetDistanceChange);
            this.all.Add(this.TideRequiredDynelID);
            this.all.Add(this.StreamCheckMagic);
            this.all.Add(this.Type);
            this.all.Add(this.Instance);
            this.all.Add(this.WeaponsStyle);
            this.all.Add(this.ShoulderMeshRight);
            this.all.Add(this.ShoulderMeshLeft);
            this.all.Add(this.WeaponMeshRight);
            this.all.Add(this.WeaponMeshLeft);
            this.all.Add(this.OverrideTextureAttractor);
            this.all.Add(this.OverrideTextureBack);
            this.all.Add(this.OverrideTextureHead);
            this.all.Add(this.OverrideTextureShoulderpadLeft);
            this.all.Add(this.OverrideTextureShoulderpadRight);
            this.all.Add(this.OverrideTextureWeaponLeft);
            this.all.Add(this.OverrideTextureWeaponRight);
            #endregion

            #region Trickles and effects
            // add Tricklers, try not to do circulars!!
            this.SetAbilityTricklers();
            this.BodyDevelopment.Affects.Add(this.Life.StatNumber);
            this.NanoEnergyPool.Affects.Add(this.MaxNanoEnergy.StatNumber);
            this.Level.Affects.Add(this.Life.StatNumber);
            this.Level.Affects.Add(this.MaxNanoEnergy.StatNumber);
            this.Level.Affects.Add(this.TitleLevel.StatNumber);
            this.Level.Affects.Add(this.NextSK.StatNumber);
            this.Level.Affects.Add(this.NextXP.StatNumber);
            this.AlienLevel.Affects.Add(this.AlienNextXP.StatNumber);
            this.XP.Affects.Add(this.Level.StatNumber);
            this.SK.Affects.Add(this.Level.StatNumber);
            this.AlienXP.Affects.Add(this.AlienLevel.StatNumber);
            this.Profession.Affects.Add(this.Health.StatNumber);
            this.Profession.Affects.Add(this.MaxNanoEnergy.StatNumber);
            this.Profession.Affects.Add(this.IP.StatNumber);
            this.Stamina.Affects.Add(this.HealDelta.StatNumber);
            this.Psychic.Affects.Add(this.NanoDelta.StatNumber);
            this.Stamina.Affects.Add(this.HealInterval.StatNumber);
            this.Psychic.Affects.Add(this.NanoInterval.StatNumber);
            this.Level.Affects.Add(this.IP.StatNumber);
            #endregion

            foreach (Class_Stat c in this.all)
            {
                c.SetParent(parent);
            }
            if (!(parent is NonPC))
            {
                #region Set standard Eventhandler for Stats (announce to player or playfield)
                /*
                Flags.RaiseBeforeStatChangedEvent += Send;
                Life.RaiseBeforeStatChangedEvent += Send;
                VolumeMass.RaiseBeforeStatChangedEvent += Send;
                AttackSpeed.RaiseBeforeStatChangedEvent += Send;
                Breed.RaiseBeforeStatChangedEvent += Send;
                Clan.RaiseBeforeStatChangedEvent += Send;
                Team.RaiseBeforeStatChangedEvent += Send;
                State.RaiseBeforeStatChangedEvent += Send;
                TimeExist.RaiseBeforeStatChangedEvent += Send;
                MapFlags.RaiseBeforeStatChangedEvent += Send;
                ProfessionLevel.RaiseBeforeStatChangedEvent += Send;
                PreviousHealth.RaiseBeforeStatChangedEvent += Send;
                Mesh.RaiseBeforeStatChangedEvent += Send;
                Anim.RaiseBeforeStatChangedEvent += Send;
                Name.RaiseBeforeStatChangedEvent += Send;
                Info.RaiseBeforeStatChangedEvent += Send;
                Strength.RaiseBeforeStatChangedEvent += Send;
                Agility.RaiseBeforeStatChangedEvent += Send;
                Stamina.RaiseBeforeStatChangedEvent += Send;
                Intelligence.RaiseBeforeStatChangedEvent += Send;
                Sense.RaiseBeforeStatChangedEvent += Send;
                Psychic.RaiseBeforeStatChangedEvent += Send;
                AMS.RaiseBeforeStatChangedEvent += Send;
                StaticInstance.RaiseBeforeStatChangedEvent += Send;
                MaxMass.RaiseBeforeStatChangedEvent += Send;
                StaticType.RaiseBeforeStatChangedEvent += Send;
                Energy.RaiseBeforeStatChangedEvent += Send;
                Health.RaiseBeforeStatChangedEvent += Send;
                Height.RaiseBeforeStatChangedEvent += Send;
                DMS.RaiseBeforeStatChangedEvent += Send;
                Can.RaiseBeforeStatChangedEvent += Send;
                Face.RaiseBeforeStatChangedEvent += Send;
                HairMesh.RaiseBeforeStatChangedEvent += Send;
                Side.RaiseBeforeStatChangedEvent += Send;
                DeadTimer.RaiseBeforeStatChangedEvent += Send;
                AccessCount.RaiseBeforeStatChangedEvent += Send;
                AttackCount.RaiseBeforeStatChangedEvent += Send;
                TitleLevel.RaiseBeforeStatChangedEvent += Send;
                BackMesh.RaiseBeforeStatChangedEvent += Send;
                AlienXP.RaiseBeforeStatChangedEvent += Send;
                FabricType.RaiseBeforeStatChangedEvent += Send;
                CATMesh.RaiseBeforeStatChangedEvent += Send;
                ParentType.RaiseBeforeStatChangedEvent += Send;
                ParentInstance.RaiseBeforeStatChangedEvent += Send;
                BeltSlots.RaiseBeforeStatChangedEvent += Send;
                BandolierSlots.RaiseBeforeStatChangedEvent += Send;
                Fatness.RaiseBeforeStatChangedEvent += Send;
                ClanLevel.RaiseBeforeStatChangedEvent += Send;
                InsuranceTime.RaiseBeforeStatChangedEvent += Send;
                InventoryTimeout.RaiseBeforeStatChangedEvent += Send;
                AggDef.RaiseBeforeStatChangedEvent += Send;
                XP.RaiseBeforeStatChangedEvent += Send;
                IP.RaiseBeforeStatChangedEvent += Send;
                Level.RaiseBeforeStatChangedEvent += Send;
                InventoryId.RaiseBeforeStatChangedEvent += Send;
                TimeSinceCreation.RaiseBeforeStatChangedEvent += Send;
                LastXP.RaiseBeforeStatChangedEvent += Send;
                Age.RaiseBeforeStatChangedEvent += Send;
                Sex.RaiseBeforeStatChangedEvent += Send;
                Profession.RaiseBeforeStatChangedEvent += Send;
                Cash.RaiseBeforeStatChangedEvent += Send;
                Alignment.RaiseBeforeStatChangedEvent += Send;
                Attitude.RaiseBeforeStatChangedEvent += Send;
                HeadMesh.RaiseBeforeStatChangedEvent += Send;
                MissionBits5.RaiseBeforeStatChangedEvent += Send;
                MissionBits6.RaiseBeforeStatChangedEvent += Send;
                MissionBits7.RaiseBeforeStatChangedEvent += Send;
                VeteranPoints.RaiseBeforeStatChangedEvent += Send;
                MonthsPaid.RaiseBeforeStatChangedEvent += Send;
                SpeedPenalty.RaiseBeforeStatChangedEvent += Send;
                TotalMass.RaiseBeforeStatChangedEvent += Send;
                ItemType.RaiseBeforeStatChangedEvent += Send;
                RepairDifficulty.RaiseBeforeStatChangedEvent += Send;
                Price.RaiseBeforeStatChangedEvent += Send;
                MetaType.RaiseBeforeStatChangedEvent += Send;
                ItemClass.RaiseBeforeStatChangedEvent += Send;
                RepairSkill.RaiseBeforeStatChangedEvent += Send;
                CurrentMass.RaiseBeforeStatChangedEvent += Send;
                Icon.RaiseBeforeStatChangedEvent += Send;
                PrimaryItemType.RaiseBeforeStatChangedEvent += Send;
                PrimaryItemInstance.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemType.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemInstance.RaiseBeforeStatChangedEvent += Send;
                UserType.RaiseBeforeStatChangedEvent += Send;
                UserInstance.RaiseBeforeStatChangedEvent += Send;
                AreaType.RaiseBeforeStatChangedEvent += Send;
                AreaInstance.RaiseBeforeStatChangedEvent += Send;
                DefaultPos.RaiseBeforeStatChangedEvent += Send;
                Race.RaiseBeforeStatChangedEvent += Send;
                ProjectileAC.RaiseBeforeStatChangedEvent += Send;
                MeleeAC.RaiseBeforeStatChangedEvent += Send;
                EnergyAC.RaiseBeforeStatChangedEvent += Send;
                ChemicalAC.RaiseBeforeStatChangedEvent += Send;
                RadiationAC.RaiseBeforeStatChangedEvent += Send;
                ColdAC.RaiseBeforeStatChangedEvent += Send;
                PoisonAC.RaiseBeforeStatChangedEvent += Send;
                FireAC.RaiseBeforeStatChangedEvent += Send;
                StateAction.RaiseBeforeStatChangedEvent += Send;
                ItemAnim.RaiseBeforeStatChangedEvent += Send;
                MartialArts.RaiseBeforeStatChangedEvent += Send;
                MeleeMultiple.RaiseBeforeStatChangedEvent += Send;
                OnehBluntWeapons.RaiseBeforeStatChangedEvent += Send;
                OnehEdgedWeapon.RaiseBeforeStatChangedEvent += Send;
                MeleeEnergyWeapon.RaiseBeforeStatChangedEvent += Send;
                TwohEdgedWeapons.RaiseBeforeStatChangedEvent += Send;
                Piercing.RaiseBeforeStatChangedEvent += Send;
                TwohBluntWeapons.RaiseBeforeStatChangedEvent += Send;
                ThrowingKnife.RaiseBeforeStatChangedEvent += Send;
                Grenade.RaiseBeforeStatChangedEvent += Send;
                ThrownGrapplingWeapons.RaiseBeforeStatChangedEvent += Send;
                Bow.RaiseBeforeStatChangedEvent += Send;
                Pistol.RaiseBeforeStatChangedEvent += Send;
                Rifle.RaiseBeforeStatChangedEvent += Send;
                SubMachineGun.RaiseBeforeStatChangedEvent += Send;
                Shotgun.RaiseBeforeStatChangedEvent += Send;
                AssaultRifle.RaiseBeforeStatChangedEvent += Send;
                DriveWater.RaiseBeforeStatChangedEvent += Send;
                CloseCombatInitiative.RaiseBeforeStatChangedEvent += Send;
                DistanceWeaponInitiative.RaiseBeforeStatChangedEvent += Send;
                PhysicalProwessInitiative.RaiseBeforeStatChangedEvent += Send;
                BowSpecialAttack.RaiseBeforeStatChangedEvent += Send;
                SenseImprovement.RaiseBeforeStatChangedEvent += Send;
                FirstAid.RaiseBeforeStatChangedEvent += Send;
                Treatment.RaiseBeforeStatChangedEvent += Send;
                MechanicalEngineering.RaiseBeforeStatChangedEvent += Send;
                ElectricalEngineering.RaiseBeforeStatChangedEvent += Send;
                MaterialMetamorphose.RaiseBeforeStatChangedEvent += Send;
                BiologicalMetamorphose.RaiseBeforeStatChangedEvent += Send;
                PsychologicalModification.RaiseBeforeStatChangedEvent += Send;
                MaterialCreation.RaiseBeforeStatChangedEvent += Send;
                MaterialLocation.RaiseBeforeStatChangedEvent += Send;
                NanoEnergyPool.RaiseBeforeStatChangedEvent += Send;
                LR_EnergyWeapon.RaiseBeforeStatChangedEvent += Send;
                LR_MultipleWeapon.RaiseBeforeStatChangedEvent += Send;
                DisarmTrap.RaiseBeforeStatChangedEvent += Send;
                Perception.RaiseBeforeStatChangedEvent += Send;
                Adventuring.RaiseBeforeStatChangedEvent += Send;
                Swim.RaiseBeforeStatChangedEvent += Send;
                DriveAir.RaiseBeforeStatChangedEvent += Send;
                MapNavigation.RaiseBeforeStatChangedEvent += Send;
                Tutoring.RaiseBeforeStatChangedEvent += Send;
                Brawl.RaiseBeforeStatChangedEvent += Send;
                Riposte.RaiseBeforeStatChangedEvent += Send;
                Dimach.RaiseBeforeStatChangedEvent += Send;
                Parry.RaiseBeforeStatChangedEvent += Send;
                SneakAttack.RaiseBeforeStatChangedEvent += Send;
                FastAttack.RaiseBeforeStatChangedEvent += Send;
                Burst.RaiseBeforeStatChangedEvent += Send;
                NanoProwessInitiative.RaiseBeforeStatChangedEvent += Send;
                FlingShot.RaiseBeforeStatChangedEvent += Send;
                AimedShot.RaiseBeforeStatChangedEvent += Send;
                BodyDevelopment.RaiseBeforeStatChangedEvent += Send;
                Duck.RaiseBeforeStatChangedEvent += Send;
                Dodge.RaiseBeforeStatChangedEvent += Send;
                Evade.RaiseBeforeStatChangedEvent += Send;
                RunSpeed.RaiseBeforeStatChangedEvent += Send;
                FieldQuantumPhysics.RaiseBeforeStatChangedEvent += Send;
                WeaponSmithing.RaiseBeforeStatChangedEvent += Send;
                Pharmaceuticals.RaiseBeforeStatChangedEvent += Send;
                NanoProgramming.RaiseBeforeStatChangedEvent += Send;
                ComputerLiteracy.RaiseBeforeStatChangedEvent += Send;
                Psychology.RaiseBeforeStatChangedEvent += Send;
                Chemistry.RaiseBeforeStatChangedEvent += Send;
                Concealment.RaiseBeforeStatChangedEvent += Send;
                BreakingEntry.RaiseBeforeStatChangedEvent += Send;
                DriveGround.RaiseBeforeStatChangedEvent += Send;
                FullAuto.RaiseBeforeStatChangedEvent += Send;
                NanoAC.RaiseBeforeStatChangedEvent += Send;
                AlienLevel.RaiseBeforeStatChangedEvent += Send;
                HealthChangeBest.RaiseBeforeStatChangedEvent += Send;
                HealthChangeWorst.RaiseBeforeStatChangedEvent += Send;
                HealthChange.RaiseBeforeStatChangedEvent += Send;
                CurrentMovementMode.RaiseBeforeStatChangedEvent += Send;
                PrevMovementMode.RaiseBeforeStatChangedEvent += Send;
                AutoLockTimeDefault.RaiseBeforeStatChangedEvent += Send;
                AutoUnlockTimeDefault.RaiseBeforeStatChangedEvent += Send;
                MoreFlags.RaiseBeforeStatChangedEvent += Send;
                AlienNextXP.RaiseBeforeStatChangedEvent += Send;
                NPCFlags.RaiseBeforeStatChangedEvent += Send;
                CurrentNCU.RaiseBeforeStatChangedEvent += Send;
                MaxNCU.RaiseBeforeStatChangedEvent += Send;
                Specialization.RaiseBeforeStatChangedEvent += Send;
                EffectIcon.RaiseBeforeStatChangedEvent += Send;
                BuildingType.RaiseBeforeStatChangedEvent += Send;
                BuildingInstance.RaiseBeforeStatChangedEvent += Send;
                CardOwnerType.RaiseBeforeStatChangedEvent += Send;
                CardOwnerInstance.RaiseBeforeStatChangedEvent += Send;
                BuildingComplexInst.RaiseBeforeStatChangedEvent += Send;
                ExitInstance.RaiseBeforeStatChangedEvent += Send;
                NextDoorInBuilding.RaiseBeforeStatChangedEvent += Send;
                LastConcretePlayfieldInstance.RaiseBeforeStatChangedEvent += Send;
                ExtenalPlayfieldInstance.RaiseBeforeStatChangedEvent += Send;
                ExtenalDoorInstance.RaiseBeforeStatChangedEvent += Send;
                InPlay.RaiseBeforeStatChangedEvent += Send;
                AccessKey.RaiseBeforeStatChangedEvent += Send;
                PetMaster.RaiseBeforeStatChangedEvent += Send;
                OrientationMode.RaiseBeforeStatChangedEvent += Send;
                SessionTime.RaiseBeforeStatChangedEvent += Send;
                RP.RaiseBeforeStatChangedEvent += Send;
                Conformity.RaiseBeforeStatChangedEvent += Send;
                Aggressiveness.RaiseBeforeStatChangedEvent += Send;
                Stability.RaiseBeforeStatChangedEvent += Send;
                Extroverty.RaiseBeforeStatChangedEvent += Send;
                BreedHostility.RaiseBeforeStatChangedEvent += Send;
                ReflectProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ReflectMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ReflectEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ReflectChemicalAC.RaiseBeforeStatChangedEvent += Send;
                RechargeDelay.RaiseBeforeStatChangedEvent += Send;
                EquipDelay.RaiseBeforeStatChangedEvent += Send;
                MaxEnergy.RaiseBeforeStatChangedEvent += Send;
                TeamSide.RaiseBeforeStatChangedEvent += Send;
                CurrentNano.RaiseBeforeStatChangedEvent += Send;
                GmLevel.RaiseBeforeStatChangedEvent += Send;
                ReflectRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ReflectColdAC.RaiseBeforeStatChangedEvent += Send;
                ReflectNanoAC.RaiseBeforeStatChangedEvent += Send;
                ReflectFireAC.RaiseBeforeStatChangedEvent += Send;
                CurrBodyLocation.RaiseBeforeStatChangedEvent += Send;
                MaxNanoEnergy.RaiseBeforeStatChangedEvent += Send;
                AccumulatedDamage.RaiseBeforeStatChangedEvent += Send;
                CanChangeClothes.RaiseBeforeStatChangedEvent += Send;
                Features.RaiseBeforeStatChangedEvent += Send;
                ReflectPoisonAC.RaiseBeforeStatChangedEvent += Send;
                ShieldProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ShieldMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ShieldEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ShieldChemicalAC.RaiseBeforeStatChangedEvent += Send;
                ShieldRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ShieldColdAC.RaiseBeforeStatChangedEvent += Send;
                ShieldNanoAC.RaiseBeforeStatChangedEvent += Send;
                ShieldFireAC.RaiseBeforeStatChangedEvent += Send;
                ShieldPoisonAC.RaiseBeforeStatChangedEvent += Send;
                BerserkMode.RaiseBeforeStatChangedEvent += Send;
                InsurancePercentage.RaiseBeforeStatChangedEvent += Send;
                ChangeSideCount.RaiseBeforeStatChangedEvent += Send;
                AbsorbProjectileAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbMeleeAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbEnergyAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbChemicalAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbRadiationAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbColdAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbFireAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbPoisonAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbNanoAC.RaiseBeforeStatChangedEvent += Send;
                TemporarySkillReduction.RaiseBeforeStatChangedEvent += Send;
                BirthDate.RaiseBeforeStatChangedEvent += Send;
                LastSaved.RaiseBeforeStatChangedEvent += Send;
                SoundVolume.RaiseBeforeStatChangedEvent += Send;
                PetCounter.RaiseBeforeStatChangedEvent += Send;
                MeetersWalked.RaiseBeforeStatChangedEvent += Send;
                QuestLevelsSolved.RaiseBeforeStatChangedEvent += Send;
                MonsterLevelsKilled.RaiseBeforeStatChangedEvent += Send;
                PvPLevelsKilled.RaiseBeforeStatChangedEvent += Send;
                MissionBits1.RaiseBeforeStatChangedEvent += Send;
                MissionBits2.RaiseBeforeStatChangedEvent += Send;
                AccessGrant.RaiseBeforeStatChangedEvent += Send;
                DoorFlags.RaiseBeforeStatChangedEvent += Send;
                ClanHierarchy.RaiseBeforeStatChangedEvent += Send;
                QuestStat.RaiseBeforeStatChangedEvent += Send;
                ClientActivated.RaiseBeforeStatChangedEvent += Send;
                PersonalResearchLevel.RaiseBeforeStatChangedEvent += Send;
                GlobalResearchLevel.RaiseBeforeStatChangedEvent += Send;
                PersonalResearchGoal.RaiseBeforeStatChangedEvent += Send;
                GlobalResearchGoal.RaiseBeforeStatChangedEvent += Send;
                TurnSpeed.RaiseBeforeStatChangedEvent += Send;
                LiquidType.RaiseBeforeStatChangedEvent += Send;
                GatherSound.RaiseBeforeStatChangedEvent += Send;
                CastSound.RaiseBeforeStatChangedEvent += Send;
                TravelSound.RaiseBeforeStatChangedEvent += Send;
                HitSound.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemTemplate.RaiseBeforeStatChangedEvent += Send;
                EquippedWeapons.RaiseBeforeStatChangedEvent += Send;
                XPKillRange.RaiseBeforeStatChangedEvent += Send;
                AMSModifier.RaiseBeforeStatChangedEvent += Send;
                DMSModifier.RaiseBeforeStatChangedEvent += Send;
                ProjectileDamageModifier.RaiseBeforeStatChangedEvent += Send;
                MeleeDamageModifier.RaiseBeforeStatChangedEvent += Send;
                EnergyDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ChemicalDamageModifier.RaiseBeforeStatChangedEvent += Send;
                RadiationDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ItemHateValue.RaiseBeforeStatChangedEvent += Send;
                DamageBonus.RaiseBeforeStatChangedEvent += Send;
                MaxDamage.RaiseBeforeStatChangedEvent += Send;
                MinDamage.RaiseBeforeStatChangedEvent += Send;
                AttackRange.RaiseBeforeStatChangedEvent += Send;
                HateValueModifyer.RaiseBeforeStatChangedEvent += Send;
                TrapDifficulty.RaiseBeforeStatChangedEvent += Send;
                StatOne.RaiseBeforeStatChangedEvent += Send;
                NumAttackEffects.RaiseBeforeStatChangedEvent += Send;
                DefaultAttackType.RaiseBeforeStatChangedEvent += Send;
                ItemSkill.RaiseBeforeStatChangedEvent += Send;
                ItemDelay.RaiseBeforeStatChangedEvent += Send;
                ItemOpposedSkill.RaiseBeforeStatChangedEvent += Send;
                ItemSIS.RaiseBeforeStatChangedEvent += Send;
                InteractionRadius.RaiseBeforeStatChangedEvent += Send;
                Placement.RaiseBeforeStatChangedEvent += Send;
                LockDifficulty.RaiseBeforeStatChangedEvent += Send;
                Members.RaiseBeforeStatChangedEvent += Send;
                MinMembers.RaiseBeforeStatChangedEvent += Send;
                ClanPrice.RaiseBeforeStatChangedEvent += Send;
                MissionBits3.RaiseBeforeStatChangedEvent += Send;
                ClanType.RaiseBeforeStatChangedEvent += Send;
                ClanInstance.RaiseBeforeStatChangedEvent += Send;
                VoteCount.RaiseBeforeStatChangedEvent += Send;
                MemberType.RaiseBeforeStatChangedEvent += Send;
                MemberInstance.RaiseBeforeStatChangedEvent += Send;
                GlobalClanType.RaiseBeforeStatChangedEvent += Send;
                GlobalClanInstance.RaiseBeforeStatChangedEvent += Send;
                ColdDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ClanUpkeepInterval.RaiseBeforeStatChangedEvent += Send;
                TimeSinceUpkeep.RaiseBeforeStatChangedEvent += Send;
                ClanFinalized.RaiseBeforeStatChangedEvent += Send;
                NanoDamageModifier.RaiseBeforeStatChangedEvent += Send;
                FireDamageModifier.RaiseBeforeStatChangedEvent += Send;
                PoisonDamageModifier.RaiseBeforeStatChangedEvent += Send;
                NPCostModifier.RaiseBeforeStatChangedEvent += Send;
                XPModifier.RaiseBeforeStatChangedEvent += Send;
                BreedLimit.RaiseBeforeStatChangedEvent += Send;
                GenderLimit.RaiseBeforeStatChangedEvent += Send;
                LevelLimit.RaiseBeforeStatChangedEvent += Send;
                PlayerKilling.RaiseBeforeStatChangedEvent += Send;
                TeamAllowed.RaiseBeforeStatChangedEvent += Send;
                WeaponDisallowedType.RaiseBeforeStatChangedEvent += Send;
                WeaponDisallowedInstance.RaiseBeforeStatChangedEvent += Send;
                Taboo.RaiseBeforeStatChangedEvent += Send;
                Compulsion.RaiseBeforeStatChangedEvent += Send;
                SkillDisabled.RaiseBeforeStatChangedEvent += Send;
                ClanItemType.RaiseBeforeStatChangedEvent += Send;
                ClanItemInstance.RaiseBeforeStatChangedEvent += Send;
                DebuffFormula.RaiseBeforeStatChangedEvent += Send;
                PvP_Rating.RaiseBeforeStatChangedEvent += Send;
                SavedXP.RaiseBeforeStatChangedEvent += Send;
                DoorBlockTime.RaiseBeforeStatChangedEvent += Send;
                OverrideTexture.RaiseBeforeStatChangedEvent += Send;
                OverrideMaterial.RaiseBeforeStatChangedEvent += Send;
                DeathReason.RaiseBeforeStatChangedEvent += Send;
                DamageOverrideType.RaiseBeforeStatChangedEvent += Send;
                BrainType.RaiseBeforeStatChangedEvent += Send;
                XPBonus.RaiseBeforeStatChangedEvent += Send;
                HealInterval.RaiseBeforeStatChangedEvent += Send;
                HealDelta.RaiseBeforeStatChangedEvent += Send;
                MonsterTexture.RaiseBeforeStatChangedEvent += Send;
                HasAlwaysLootable.RaiseBeforeStatChangedEvent += Send;
                TradeLimit.RaiseBeforeStatChangedEvent += Send;
                FaceTexture.RaiseBeforeStatChangedEvent += Send;
                SpecialCondition.RaiseBeforeStatChangedEvent += Send;
                AutoAttackFlags.RaiseBeforeStatChangedEvent += Send;
                NextXP.RaiseBeforeStatChangedEvent += Send;
                TeleportPauseMilliSeconds.RaiseBeforeStatChangedEvent += Send;
                SISCap.RaiseBeforeStatChangedEvent += Send;
                AnimSet.RaiseBeforeStatChangedEvent += Send;
                AttackType.RaiseBeforeStatChangedEvent += Send;
                NanoFocusLevel.RaiseBeforeStatChangedEvent += Send;
                NPCHash.RaiseBeforeStatChangedEvent += Send;
                CollisionRadius.RaiseBeforeStatChangedEvent += Send;
                OuterRadius.RaiseBeforeStatChangedEvent += Send;
                MonsterData.RaiseBeforeStatChangedEvent += Send;
                MonsterScale.RaiseBeforeStatChangedEvent += Send;
                HitEffectType.RaiseBeforeStatChangedEvent += Send;
                ResurrectDest.RaiseBeforeStatChangedEvent += Send;
                NanoInterval.RaiseBeforeStatChangedEvent += Send;
                NanoDelta.RaiseBeforeStatChangedEvent += Send;
                ReclaimItem.RaiseBeforeStatChangedEvent += Send;
                GatherEffectType.RaiseBeforeStatChangedEvent += Send;
                VisualBreed.RaiseBeforeStatChangedEvent += Send;
                VisualProfession.RaiseBeforeStatChangedEvent += Send;
                VisualSex.RaiseBeforeStatChangedEvent += Send;
                RitualTargetInst.RaiseBeforeStatChangedEvent += Send;
                SkillTimeOnSelectedTarget.RaiseBeforeStatChangedEvent += Send;
                LastSaveXP.RaiseBeforeStatChangedEvent += Send;
                ExtendedTime.RaiseBeforeStatChangedEvent += Send;
                BurstRecharge.RaiseBeforeStatChangedEvent += Send;
                FullAutoRecharge.RaiseBeforeStatChangedEvent += Send;
                GatherAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CastTargetAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CastSelfAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CriticalIncrease.RaiseBeforeStatChangedEvent += Send;
                RangeIncreaserWeapon.RaiseBeforeStatChangedEvent += Send;
                RangeIncreaserNF.RaiseBeforeStatChangedEvent += Send;
                SkillLockModifier.RaiseBeforeStatChangedEvent += Send;
                InterruptModifier.RaiseBeforeStatChangedEvent += Send;
                ACGEntranceStyles.RaiseBeforeStatChangedEvent += Send;
                ChanceOfBreakOnSpellAttack.RaiseBeforeStatChangedEvent += Send;
                ChanceOfBreakOnDebuff.RaiseBeforeStatChangedEvent += Send;
                DieAnim.RaiseBeforeStatChangedEvent += Send;
                TowerType.RaiseBeforeStatChangedEvent += Send;
                Expansion.RaiseBeforeStatChangedEvent += Send;
                LowresMesh.RaiseBeforeStatChangedEvent += Send;
                CriticalDecrease.RaiseBeforeStatChangedEvent += Send;
                OldTimeExist.RaiseBeforeStatChangedEvent += Send;
                ResistModifier.RaiseBeforeStatChangedEvent += Send;
                ChestFlags.RaiseBeforeStatChangedEvent += Send;
                PrimaryTemplateID.RaiseBeforeStatChangedEvent += Send;
                NumberOfItems.RaiseBeforeStatChangedEvent += Send;
                SelectedTargetType.RaiseBeforeStatChangedEvent += Send;
                Corpse_Hash.RaiseBeforeStatChangedEvent += Send;
                AmmoName.RaiseBeforeStatChangedEvent += Send;
                Rotation.RaiseBeforeStatChangedEvent += Send;
                CATAnim.RaiseBeforeStatChangedEvent += Send;
                CATAnimFlags.RaiseBeforeStatChangedEvent += Send;
                DisplayCATAnim.RaiseBeforeStatChangedEvent += Send;
                DisplayCATMesh.RaiseBeforeStatChangedEvent += Send;
                School.RaiseBeforeStatChangedEvent += Send;
                NanoSpeed.RaiseBeforeStatChangedEvent += Send;
                NanoPoints.RaiseBeforeStatChangedEvent += Send;
                TrainSkill.RaiseBeforeStatChangedEvent += Send;
                TrainSkillCost.RaiseBeforeStatChangedEvent += Send;
                IsFightingMe.RaiseBeforeStatChangedEvent += Send;
                NextFormula.RaiseBeforeStatChangedEvent += Send;
                MultipleCount.RaiseBeforeStatChangedEvent += Send;
                EffectType.RaiseBeforeStatChangedEvent += Send;
                ImpactEffectType.RaiseBeforeStatChangedEvent += Send;
                CorpseType.RaiseBeforeStatChangedEvent += Send;
                CorpseInstance.RaiseBeforeStatChangedEvent += Send;
                CorpseAnimKey.RaiseBeforeStatChangedEvent += Send;
                UnarmedTemplateInstance.RaiseBeforeStatChangedEvent += Send;
                TracerEffectType.RaiseBeforeStatChangedEvent += Send;
                AmmoType.RaiseBeforeStatChangedEvent += Send;
                CharRadius.RaiseBeforeStatChangedEvent += Send;
                ChanceOfUse.RaiseBeforeStatChangedEvent += Send;
                CurrentState.RaiseBeforeStatChangedEvent += Send;
                ArmourType.RaiseBeforeStatChangedEvent += Send;
                RestModifier.RaiseBeforeStatChangedEvent += Send;
                BuyModifier.RaiseBeforeStatChangedEvent += Send;
                SellModifier.RaiseBeforeStatChangedEvent += Send;
                CastEffectType.RaiseBeforeStatChangedEvent += Send;
                NPCBrainState.RaiseBeforeStatChangedEvent += Send;
                WaitState.RaiseBeforeStatChangedEvent += Send;
                SelectedTarget.RaiseBeforeStatChangedEvent += Send;
                MissionBits4.RaiseBeforeStatChangedEvent += Send;
                OwnerInstance.RaiseBeforeStatChangedEvent += Send;
                CharState.RaiseBeforeStatChangedEvent += Send;
                ReadOnly.RaiseBeforeStatChangedEvent += Send;
                DamageType.RaiseBeforeStatChangedEvent += Send;
                CollideCheckInterval.RaiseBeforeStatChangedEvent += Send;
                PlayfieldType.RaiseBeforeStatChangedEvent += Send;
                NPCCommand.RaiseBeforeStatChangedEvent += Send;
                InitiativeType.RaiseBeforeStatChangedEvent += Send;
                CharTmp1.RaiseBeforeStatChangedEvent += Send;
                CharTmp2.RaiseBeforeStatChangedEvent += Send;
                CharTmp3.RaiseBeforeStatChangedEvent += Send;
                CharTmp4.RaiseBeforeStatChangedEvent += Send;
                NPCCommandArg.RaiseBeforeStatChangedEvent += Send;
                NameTemplate.RaiseBeforeStatChangedEvent += Send;
                DesiredTargetDistance.RaiseBeforeStatChangedEvent += Send;
                VicinityRange.RaiseBeforeStatChangedEvent += Send;
                NPCIsSurrendering.RaiseBeforeStatChangedEvent += Send;
                StateMachine.RaiseBeforeStatChangedEvent += Send;
                NPCSurrenderInstance.RaiseBeforeStatChangedEvent += Send;
                NPCHasPatrolList.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityChars.RaiseBeforeStatChangedEvent += Send;
                ProximityRangeOutdoors.RaiseBeforeStatChangedEvent += Send;
                NPCFamily.RaiseBeforeStatChangedEvent += Send;
                CommandRange.RaiseBeforeStatChangedEvent += Send;
                NPCHatelistSize.RaiseBeforeStatChangedEvent += Send;
                NPCNumPets.RaiseBeforeStatChangedEvent += Send;
                ODMinSizeAdd.RaiseBeforeStatChangedEvent += Send;
                EffectRed.RaiseBeforeStatChangedEvent += Send;
                EffectGreen.RaiseBeforeStatChangedEvent += Send;
                EffectBlue.RaiseBeforeStatChangedEvent += Send;
                ODMaxSizeAdd.RaiseBeforeStatChangedEvent += Send;
                DurationModifier.RaiseBeforeStatChangedEvent += Send;
                NPCCryForHelpRange.RaiseBeforeStatChangedEvent += Send;
                LOSHeight.RaiseBeforeStatChangedEvent += Send;
                PetReq1.RaiseBeforeStatChangedEvent += Send;
                PetReq2.RaiseBeforeStatChangedEvent += Send;
                PetReq3.RaiseBeforeStatChangedEvent += Send;
                MapOptions.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart1.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart2.RaiseBeforeStatChangedEvent += Send;
                FixtureFlags.RaiseBeforeStatChangedEvent += Send;
                FallDamage.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedChemicalAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedColdAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedNanoAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedFireAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedPoisonAC.RaiseBeforeStatChangedEvent += Send;
                ProximityRangeIndoors.RaiseBeforeStatChangedEvent += Send;
                PetReqVal1.RaiseBeforeStatChangedEvent += Send;
                PetReqVal2.RaiseBeforeStatChangedEvent += Send;
                PetReqVal3.RaiseBeforeStatChangedEvent += Send;
                TargetFacing.RaiseBeforeStatChangedEvent += Send;
                Backstab.RaiseBeforeStatChangedEvent += Send;
                OriginatorType.RaiseBeforeStatChangedEvent += Send;
                QuestInstance.RaiseBeforeStatChangedEvent += Send;
                QuestIndex1.RaiseBeforeStatChangedEvent += Send;
                QuestIndex2.RaiseBeforeStatChangedEvent += Send;
                QuestIndex3.RaiseBeforeStatChangedEvent += Send;
                QuestIndex4.RaiseBeforeStatChangedEvent += Send;
                QuestIndex5.RaiseBeforeStatChangedEvent += Send;
                QTDungeonInstance.RaiseBeforeStatChangedEvent += Send;
                QTNumMonsters.RaiseBeforeStatChangedEvent += Send;
                QTKilledMonsters.RaiseBeforeStatChangedEvent += Send;
                AnimPos.RaiseBeforeStatChangedEvent += Send;
                AnimPlay.RaiseBeforeStatChangedEvent += Send;
                AnimSpeed.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID1.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount1.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID2.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount2.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID3.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount3.RaiseBeforeStatChangedEvent += Send;
                QuestIndex0.RaiseBeforeStatChangedEvent += Send;
                QuestTimeout.RaiseBeforeStatChangedEvent += Send;
                Tower_NPCHash.RaiseBeforeStatChangedEvent += Send;
                PetType.RaiseBeforeStatChangedEvent += Send;
                OnTowerCreation.RaiseBeforeStatChangedEvent += Send;
                OwnedTowers.RaiseBeforeStatChangedEvent += Send;
                TowerInstance.RaiseBeforeStatChangedEvent += Send;
                AttackShield.RaiseBeforeStatChangedEvent += Send;
                SpecialAttackShield.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityPlayers.RaiseBeforeStatChangedEvent += Send;
                NPCUseFightModeRegenRate.RaiseBeforeStatChangedEvent += Send;
                Rnd.RaiseBeforeStatChangedEvent += Send;
                SocialStatus.RaiseBeforeStatChangedEvent += Send;
                LastRnd.RaiseBeforeStatChangedEvent += Send;
                ItemDelayCap.RaiseBeforeStatChangedEvent += Send;
                RechargeDelayCap.RaiseBeforeStatChangedEvent += Send;
                PercentRemainingHealth.RaiseBeforeStatChangedEvent += Send;
                PercentRemainingNano.RaiseBeforeStatChangedEvent += Send;
                TargetDistance.RaiseBeforeStatChangedEvent += Send;
                TeamCloseness.RaiseBeforeStatChangedEvent += Send;
                NumberOnHateList.RaiseBeforeStatChangedEvent += Send;
                ConditionState.RaiseBeforeStatChangedEvent += Send;
                ExpansionPlayfield.RaiseBeforeStatChangedEvent += Send;
                ShadowBreed.RaiseBeforeStatChangedEvent += Send;
                NPCFovStatus.RaiseBeforeStatChangedEvent += Send;
                DudChance.RaiseBeforeStatChangedEvent += Send;
                HealMultiplier.RaiseBeforeStatChangedEvent += Send;
                NanoDamageMultiplier.RaiseBeforeStatChangedEvent += Send;
                NanoVulnerability.RaiseBeforeStatChangedEvent += Send;
                AmsCap.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative1.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative2.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative3.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative4.RaiseBeforeStatChangedEvent += Send;
                FactionModifier.RaiseBeforeStatChangedEvent += Send;
                MissionBits8.RaiseBeforeStatChangedEvent += Send;
                MissionBits9.RaiseBeforeStatChangedEvent += Send;
                StackingLine2.RaiseBeforeStatChangedEvent += Send;
                StackingLine3.RaiseBeforeStatChangedEvent += Send;
                StackingLine4.RaiseBeforeStatChangedEvent += Send;
                StackingLine5.RaiseBeforeStatChangedEvent += Send;
                StackingLine6.RaiseBeforeStatChangedEvent += Send;
                StackingOrder.RaiseBeforeStatChangedEvent += Send;
                ProcNano1.RaiseBeforeStatChangedEvent += Send;
                ProcNano2.RaiseBeforeStatChangedEvent += Send;
                ProcNano3.RaiseBeforeStatChangedEvent += Send;
                ProcNano4.RaiseBeforeStatChangedEvent += Send;
                ProcChance1.RaiseBeforeStatChangedEvent += Send;
                ProcChance2.RaiseBeforeStatChangedEvent += Send;
                ProcChance3.RaiseBeforeStatChangedEvent += Send;
                ProcChance4.RaiseBeforeStatChangedEvent += Send;
                OTArmedForces.RaiseBeforeStatChangedEvent += Send;
                ClanSentinels.RaiseBeforeStatChangedEvent += Send;
                OTMed.RaiseBeforeStatChangedEvent += Send;
                ClanGaia.RaiseBeforeStatChangedEvent += Send;
                OTTrans.RaiseBeforeStatChangedEvent += Send;
                ClanVanguards.RaiseBeforeStatChangedEvent += Send;
                GOS.RaiseBeforeStatChangedEvent += Send;
                OTFollowers.RaiseBeforeStatChangedEvent += Send;
                OTOperator.RaiseBeforeStatChangedEvent += Send;
                OTUnredeemed.RaiseBeforeStatChangedEvent += Send;
                ClanDevoted.RaiseBeforeStatChangedEvent += Send;
                ClanConserver.RaiseBeforeStatChangedEvent += Send;
                ClanRedeemed.RaiseBeforeStatChangedEvent += Send;
                SK.RaiseBeforeStatChangedEvent += Send;
                LastSK.RaiseBeforeStatChangedEvent += Send;
                NextSK.RaiseBeforeStatChangedEvent += Send;
                PlayerOptions.RaiseBeforeStatChangedEvent += Send;
                LastPerkResetTime.RaiseBeforeStatChangedEvent += Send;
                CurrentTime.RaiseBeforeStatChangedEvent += Send;
                ShadowBreedTemplate.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityFamily.RaiseBeforeStatChangedEvent += Send;
                NPCScriptAMSScale.RaiseBeforeStatChangedEvent += Send;
                ApartmentsAllowed.RaiseBeforeStatChangedEvent += Send;
                ApartmentsOwned.RaiseBeforeStatChangedEvent += Send;
                ApartmentAccessCard.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart3.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart4.RaiseBeforeStatChangedEvent += Send;
                NumberOfTeamMembers.RaiseBeforeStatChangedEvent += Send;
                ActionCategory.RaiseBeforeStatChangedEvent += Send;
                CurrentPlayfield.RaiseBeforeStatChangedEvent += Send;
                DistrictNano.RaiseBeforeStatChangedEvent += Send;
                DistrictNanoInterval.RaiseBeforeStatChangedEvent += Send;
                UnsavedXP.RaiseBeforeStatChangedEvent += Send;
                RegainXPPercentage.RaiseBeforeStatChangedEvent += Send;
                TempSaveTeamID.RaiseBeforeStatChangedEvent += Send;
                TempSavePlayfield.RaiseBeforeStatChangedEvent += Send;
                TempSaveX.RaiseBeforeStatChangedEvent += Send;
                TempSaveY.RaiseBeforeStatChangedEvent += Send;
                ExtendedFlags.RaiseBeforeStatChangedEvent += Send;
                ShopPrice.RaiseBeforeStatChangedEvent += Send;
                NewbieHP.RaiseBeforeStatChangedEvent += Send;
                HPLevelUp.RaiseBeforeStatChangedEvent += Send;
                HPPerSkill.RaiseBeforeStatChangedEvent += Send;
                NewbieNP.RaiseBeforeStatChangedEvent += Send;
                NPLevelUp.RaiseBeforeStatChangedEvent += Send;
                NPPerSkill.RaiseBeforeStatChangedEvent += Send;
                MaxShopItems.RaiseBeforeStatChangedEvent += Send;
                PlayerID.RaiseBeforeStatChangedEvent += Send;
                ShopRent.RaiseBeforeStatChangedEvent += Send;
                SynergyHash.RaiseBeforeStatChangedEvent += Send;
                ShopFlags.RaiseBeforeStatChangedEvent += Send;
                ShopLastUsed.RaiseBeforeStatChangedEvent += Send;
                ShopType.RaiseBeforeStatChangedEvent += Send;
                LockDownTime.RaiseBeforeStatChangedEvent += Send;
                LeaderLockDownTime.RaiseBeforeStatChangedEvent += Send;
                InvadersKilled.RaiseBeforeStatChangedEvent += Send;
                KilledByInvaders.RaiseBeforeStatChangedEvent += Send;
                MissionBits10.RaiseBeforeStatChangedEvent += Send;
                MissionBits11.RaiseBeforeStatChangedEvent += Send;
                MissionBits12.RaiseBeforeStatChangedEvent += Send;
                HouseTemplate.RaiseBeforeStatChangedEvent += Send;
                PercentFireDamage.RaiseBeforeStatChangedEvent += Send;
                PercentColdDamage.RaiseBeforeStatChangedEvent += Send;
                PercentMeleeDamage.RaiseBeforeStatChangedEvent += Send;
                PercentProjectileDamage.RaiseBeforeStatChangedEvent += Send;
                PercentPoisonDamage.RaiseBeforeStatChangedEvent += Send;
                PercentRadiationDamage.RaiseBeforeStatChangedEvent += Send;
                PercentEnergyDamage.RaiseBeforeStatChangedEvent += Send;
                PercentChemicalDamage.RaiseBeforeStatChangedEvent += Send;
                TotalDamage.RaiseBeforeStatChangedEvent += Send;
                TrackProjectileDamage.RaiseBeforeStatChangedEvent += Send;
                TrackMeleeDamage.RaiseBeforeStatChangedEvent += Send;
                TrackEnergyDamage.RaiseBeforeStatChangedEvent += Send;
                TrackChemicalDamage.RaiseBeforeStatChangedEvent += Send;
                TrackRadiationDamage.RaiseBeforeStatChangedEvent += Send;
                TrackColdDamage.RaiseBeforeStatChangedEvent += Send;
                TrackPoisonDamage.RaiseBeforeStatChangedEvent += Send;
                TrackFireDamage.RaiseBeforeStatChangedEvent += Send;
                NPCSpellArg1.RaiseBeforeStatChangedEvent += Send;
                NPCSpellRet1.RaiseBeforeStatChangedEvent += Send;
                CityInstance.RaiseBeforeStatChangedEvent += Send;
                DistanceToSpawnpoint.RaiseBeforeStatChangedEvent += Send;
                CityTerminalRechargePercent.RaiseBeforeStatChangedEvent += Send;
                UnreadMailCount.RaiseBeforeStatChangedEvent += Send;
                LastMailCheckTime.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash1.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash2.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash3.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash4.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash5.RaiseBeforeStatChangedEvent += Send;
                ShopIndex.RaiseBeforeStatChangedEvent += Send;
                ShopID.RaiseBeforeStatChangedEvent += Send;
                IsVehicle.RaiseBeforeStatChangedEvent += Send;
                DamageToNano.RaiseBeforeStatChangedEvent += Send;
                AccountFlags.RaiseBeforeStatChangedEvent += Send;
                DamageToNanoMultiplier.RaiseBeforeStatChangedEvent += Send;
                MechData.RaiseBeforeStatChangedEvent += Send;
                VehicleAC.RaiseBeforeStatChangedEvent += Send;
                VehicleDamage.RaiseBeforeStatChangedEvent += Send;
                VehicleHealth.RaiseBeforeStatChangedEvent += Send;
                VehicleSpeed.RaiseBeforeStatChangedEvent += Send;
                BattlestationSide.RaiseBeforeStatChangedEvent += Send;
                VP.RaiseBeforeStatChangedEvent += Send;
                BattlestationRep.RaiseBeforeStatChangedEvent += Send;
                PetState.RaiseBeforeStatChangedEvent += Send;
                PaidPoints.RaiseBeforeStatChangedEvent += Send;
                VisualFlags.RaiseBeforeStatChangedEvent += Send;
                PVPDuelKills.RaiseBeforeStatChangedEvent += Send;
                PVPDuelDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPProfessionDuelKills.RaiseBeforeStatChangedEvent += Send;
                PVPProfessionDuelDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPRankedSoloKills.RaiseBeforeStatChangedEvent += Send;
                PVPRankedSoloDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPRankedTeamKills.RaiseBeforeStatChangedEvent += Send;
                PVPRankedTeamDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPSoloScore.RaiseBeforeStatChangedEvent += Send;
                PVPTeamScore.RaiseBeforeStatChangedEvent += Send;
                PVPDuelScore.RaiseBeforeStatChangedEvent += Send;
                ACGItemSeed.RaiseBeforeStatChangedEvent += Send;
                ACGItemLevel.RaiseBeforeStatChangedEvent += Send;
                ACGItemTemplateID.RaiseBeforeStatChangedEvent += Send;
                ACGItemTemplateID2.RaiseBeforeStatChangedEvent += Send;
                ACGItemCategoryID.RaiseBeforeStatChangedEvent += Send;
                HasKnuBotData.RaiseBeforeStatChangedEvent += Send;
                QuestBoothDifficulty.RaiseBeforeStatChangedEvent += Send;
                QuestASMinimumRange.RaiseBeforeStatChangedEvent += Send;
                QuestASMaximumRange.RaiseBeforeStatChangedEvent += Send;
                VisualLODLevel.RaiseBeforeStatChangedEvent += Send;
                TargetDistanceChange.RaiseBeforeStatChangedEvent += Send;
                TideRequiredDynelID.RaiseBeforeStatChangedEvent += Send;
                StreamCheckMagic.RaiseBeforeStatChangedEvent += Send;
                Type.RaiseBeforeStatChangedEvent += Send;
                Instance.RaiseBeforeStatChangedEvent += Send;
                ShoulderMeshRight.RaiseBeforeStatChangedEvent += Send;
                ShoulderMeshLeft.RaiseBeforeStatChangedEvent += Send;
                WeaponMeshRight.RaiseBeforeStatChangedEvent += Send;
                WeaponMeshLeft.RaiseBeforeStatChangedEvent += Send;
                */
                #endregion
            }

            #region Setting our special stats to 'dontwriteme'-mode
            this.Expansion.DoNotDontWriteToSql = true;
            this.AccountFlags.DoNotDontWriteToSql = true;
            this.PlayerID.DoNotDontWriteToSql = true;
            this.ProfessionLevel.DoNotDontWriteToSql = true;
            this.GmLevel.DoNotDontWriteToSql = true;
            this.Type.DoNotDontWriteToSql = true;
            this.Instance.DoNotDontWriteToSql = true;
            #endregion
        }
        #endregion

        #region SetAbilityTricklers
        public void SetAbilityTricklers()
        {
            for (int c = 0; c < SkillTrickleTable.table.Length / 7; c++)
            {
                int skillnum = Convert.ToInt32(SkillTrickleTable.table[c, 0]);
                if (SkillTrickleTable.table[c, 1] > 0)
                {
                    this.Strength.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 2] > 0)
                {
                    this.Stamina.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 3] > 0)
                {
                    this.Sense.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 4] > 0)
                {
                    this.Agility.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 5] > 0)
                {
                    this.Intelligence.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 6] > 0)
                {
                    this.Psychic.Affects.Add(skillnum);
                }
            }
        }
        #endregion

        #region Get Stat object by number
        public Class_Stat GetStatbyNumber(int number)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }
                return c;
            }
            return null;
        }
        #endregion

        #region Announce Statchange to player(s)
        public void Send(object sender, Class_Stat.StatChangedEventArgs e)
        {
            if (!((Character)((Class_Stat)sender).Parent).dontdotimers)
            {
                if (e.Stat.SendBaseValue)
                {
                    Stat.Send(((Character)e.Stat.Parent).client, e.Stat.StatNumber, e.newvalue, e.AnnounceToPlayfield);
                }
                else
                {
                    Stat.Send(((Character)e.Stat.Parent).client, e.Stat.StatNumber, e.newvalue, e.AnnounceToPlayfield);
                }
                e.Stat.changed = false;
            }
        }
        #endregion

        #region Access Stat by number
        /// <summary>
        /// Returns Stat's value
        /// </summary>
        /// <param name="number">Stat number</param>
        /// <returns>Stat's value</returns>
        public int Get(int number)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }
                return c.Value;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="number">Stat number</param>
        /// <param name="newvalue">Stat's new value</param>
        public void Set(int number, uint newvalue)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }
                c.Set(newvalue);
                return;
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region Access Stat by name
        /// <summary>
        /// Returns Stat's value
        /// </summary>
        /// <param name="name">Name of the Stat</param>
        /// <returns>Stat's value</returns>
        public int Get(string name)
        {
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                return c.Value;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="name">Stat's name</param>
        /// <param name="newvalue">Stat's new value</param>
        public void Set(string name, uint newvalue)
        {
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                c.Set(newvalue);
                return;
            }
            throw new IndexOutOfRangeException();
        }

        public int GetID(string name)
        {
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                return c.StatNumber;
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region Read all Stats from SQL
        /// <summary>
        /// Read all stats from SQL
        /// </summary>
        public void ReadStatsfromSQL()
        {
            SqlWrapper sql = new SqlWrapper();
            DataTable dt =
                sql.ReadDT(
                    "SELECT Stat,Value FROM " + this.Flags.Parent.getSQLTablefromDynelType() + "_stats WHERE ID="
                    + this.Flags.Parent.ID); // Using Flags to address parent object
            foreach (DataRow row in dt.Rows)
            {
                this.SetBaseValue((Int32)row[0], (UInt32)((Int32)row[1]));
            }
        }

        /// <summary>
        /// Write all Stats to SQL
        /// </summary>
        public void WriteStatstoSQL()
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.DoNotDontWriteToSql)
                {
                    continue;
                }
                c.WriteStatToSQL(true);
            }
        }
        #endregion

        #region Get/Set Stat Modifier
        public int GetModifier(int stat)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatModifier;
            }
            throw new IndexOutOfRangeException();
        }

        public void SetModifier(int stat, int value)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.StatModifier = value;
                return;
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region Get/Set Stat Percentage Modifier
        public int GetPercentageModifier(int stat)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatPercentageModifier;
            }
            throw new IndexOutOfRangeException();
        }

        public void SetPercentageModifier(int stat, int value)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.StatPercentageModifier = value;
                return;
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region Get/Set Stat Base Value
        public uint GetBaseValue(int stat)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatBaseValue;
            }
            throw new IndexOutOfRangeException();
        }

        public void SetBaseValue(int stat, uint value)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.changed = c.StatBaseValue != value;
                c.StatBaseValue = value;
                return;
            }
            throw new IndexOutOfRangeException("Out of Range " + stat + " " + value);
        }
        #endregion

        #region Clear Modifiers for recalculation
        public void ClearModifiers()
        {
            foreach (Class_Stat c in this.all)
            {
                c.StatModifier = 0;
                c.StatPercentageModifier = 100;
                c.Trickle = 0;
            }
        }
        #endregion

        #region Set Trickle values
        public void SetTrickle(int statid, int value)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                c.Trickle = value;
                return;
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region send stat value by ID
        public void Send(Client cli, int statId)
        {
            foreach (Class_Stat c in this.all)
            {
                if (c.StatNumber != statId)
                {
                    continue;
                }
                int val;
                if (c.SendBaseValue)
                {
                    val = (Int32)c.StatBaseValue;
                }
                else
                {
                    val = c.Value;
                }
                Stat.Send(cli, statId, val, c.AnnounceToPlayfield);
                return;
            }

            throw new IndexOutOfRangeException();
        }
        #endregion

        public void ClearChangedFlags()
        {
            foreach (Class_Stat cs in this.all)
            {
                cs.changed = false;
            }
        }
    }
    #endregion
}