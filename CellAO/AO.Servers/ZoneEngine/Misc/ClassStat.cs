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

namespace ZoneEngine.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using ZoneEngine.Packets;

    #region ClassStat  class for one stat
    public class ClassStat
    {
        #region StatChangedEventArgs
        /// <summary>
        /// Event Arguments for changed stats
        /// </summary>
        public class StatChangedEventArgs : EventArgs
        {
            public StatChangedEventArgs(
                ClassStat changedStat, uint valueBeforeChange, uint valueAfterChange, bool announceToPlayfield)
            {
                this.stat = changedStat;
                this.OldValue = valueBeforeChange;
                this.NewValue = valueAfterChange;
                this.AnnounceToPlayfield = announceToPlayfield;
            }

            private readonly ClassStat stat;

            public ClassStat Stat
            {
                get
                {
                    return this.stat;
                }
            }

            public uint OldValue { get; set; }

            public Dynel Parent
            {
                get
                {
                    return this.stat.Parent;
                }
            }

            public uint NewValue { get; set; }

            public bool AnnounceToPlayfield { get; set; }
        }
        #endregion

        #region Eventhandlers
        public event EventHandler<StatChangedEventArgs> RaiseBeforeStatChangedEvent;

        public event EventHandler<StatChangedEventArgs> RaiseAfterStatChangedEvent;

        private void OnBeforeStatChangedEvent(StatChangedEventArgs e)
        {
            EventHandler<StatChangedEventArgs> handler = this.RaiseBeforeStatChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnAfterStatChangedEvent(StatChangedEventArgs e)
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
                    Math.Floor((double)
// ReSharper disable PossibleLossOfFraction
                        ((this.StatBaseValue + this.StatModifier + this.Trickle) * this.StatPercentageModifier / 100));
// ReSharper restore PossibleLossOfFraction
            }
            set
            {
                Set(value);
            }
        }

        public uint StatDefaultValue;

        public uint StatBaseValue;

        public int StatPercentageModifier = 100; // From Items/Perks/Nanos

        public int StatModifier; // From Items/Perks/Nanos

        public int Trickle; // From Attributes (Strength, Stamina, Sense, Agility, Intelligence, Psychic)

        public bool AnnounceToPlayfield = true;

        public bool DoNotDontWriteToSql;

        public bool SendBaseValue = true;

        public bool Changed;

        public List<int> Affects = new List<int>();

        public Dynel Parent;

        public ClassStat(
            int number, uint defaultValue, string name, bool sendBaseValue, bool dontWrite, bool announceToPlayfield)
        {
            this.DoNotDontWriteToSql = true;
            this.StatNumber = number;
            this.StatDefaultValue = defaultValue;
            this.StatBaseValue = defaultValue;
            this.StatDefaultValue = defaultValue;
            this.SendBaseValue = sendBaseValue;
            this.DoNotDontWriteToSql = dontWrite;
            this.AnnounceToPlayfield = announceToPlayfield;
            // Obsolete            StatName = name;
        }

        public ClassStat()
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
                this.Changed = true;
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
            if (this.Changed)
            {
                if (this.Parent is NonPlayerCharacterClass)
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType()
                        + "_stats (ID, Playfield, Stat, Value) VALUES (" + id + ","
                        + this.Parent.PlayField + "," + this.StatNumber + ","
                        + ((Int32)this.StatBaseValue) + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue) + ";");
                }
                else
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType() + "_stats (ID, Stat, Value) VALUES ("
                        + id + "," + this.StatNumber + ","
                        + ((Int32)this.StatBaseValue) + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue) + ";");
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
                if (this.Parent is NonPlayerCharacterClass)
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType()
                        + "_stats (ID, Playfield, Stat, Value) VALUES (" + id + "," + this.Parent.PlayField
                        + "," + this.StatNumber + "," + ((Int32)this.StatBaseValue)
                        + ") ON DUPLICATE KEY UPDATE Value=" + ((Int32)this.StatBaseValue) + ";");
                }
                else
                {
                    sql.SqlInsert(
                        "INSERT INTO " + (this.Parent).getSQLTablefromDynelType() + "_stats (ID, Stat, Value) VALUES ("
                        + id + "," + this.StatNumber + ","
                        + ((Int32)this.StatBaseValue) + ") ON DUPLICATE KEY UPDATE Value="
                        + ((Int32)this.StatBaseValue) + ";");
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
                sql.ReadDatatable(
                    "SELECT Value FROM " + this.Parent.getSQLTablefromDynelType() + " WHERE ID=" + id
                    + " AND Stat=" + this.StatNumber + ";");

            if (dt.Rows.Count > 0)
            {
                this.Value = (Int32)dt.Rows[0][0];
            }
        }
        #endregion

        #region Call Stats affected by this stat
        public void AffectStats()
        {
            if (!(this.Parent is Character) && !(this.Parent is NonPlayerCharacterClass))
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
    public class CharacterStats
    {
        #region stats creation
        private readonly ClassStat flags = new ClassStat(0, 8917569, "Flags", false, false, true);

        private readonly StatHealth life = new StatHealth(1, 1, "Life", true, false, false);

        private readonly ClassStat volumeMass = new ClassStat(2, 1234567890, "VolumeMass", false, false, false);

        private readonly ClassStat attackSpeed = new ClassStat(3, 5, "AttackSpeed", false, false, false);

        private readonly ClassStat breed = new ClassStat(4, 1234567890, "Breed", false, false, false);

        private readonly ClassStat clan = new ClassStat(5, 0, "Clan", false, false, false);

        private readonly ClassStat team = new ClassStat(6, 0, "Team", false, false, false);

        private readonly ClassStat state = new ClassStat(7, 0, "State", false, false, false);

        private readonly ClassStat timeExist = new ClassStat(8, 1234567890, "TimeExist", false, false, false);

        private readonly ClassStat mapFlags = new ClassStat(9, 0, "MapFlags", false, false, false);

        private readonly ClassStat professionLevel = new ClassStat(
            10, 1234567890, "ProfessionLevel", false, true, false);

        private readonly ClassStat previousHealth = new ClassStat(11, 50, "PreviousHealth", false, false, false);

        private readonly ClassStat mesh = new ClassStat(12, 17530, "Mesh", false, false, false);

        private readonly ClassStat anim = new ClassStat(13, 1234567890, "Anim", false, false, false);

        private readonly ClassStat name = new ClassStat(14, 1234567890, "Name", false, false, false);

        private readonly ClassStat info = new ClassStat(15, 1234567890, "Info", false, false, false);

        private readonly ClassStat strength = new ClassStat(16, 0, "Strength", true, false, false);

        private readonly ClassStat agility = new ClassStat(17, 0, "Agility", true, false, false);

        private readonly ClassStat stamina = new ClassStat(18, 0, "Stamina", true, false, false);

        private readonly ClassStat intelligence = new ClassStat(19, 0, "Intelligence", true, false, false);

        private readonly ClassStat sense = new ClassStat(20, 0, "Sense", true, false, false);

        private readonly ClassStat psychic = new ClassStat(21, 0, "Psychic", true, false, false);

        private readonly ClassStat ams = new ClassStat(22, 1234567890, "AMS", false, false, false);

        private readonly ClassStat staticInstance = new ClassStat(23, 1234567890, "StaticInstance", false, false, false);

        private readonly ClassStat maxMass = new ClassStat(24, 1234567890, "MaxMass", false, false, false);

        private readonly ClassStat staticType = new ClassStat(25, 1234567890, "StaticType", false, false, false);

        private readonly ClassStat energy = new ClassStat(26, 1234567890, "Energy", false, false, false);

        private readonly StatHitPoints health = new StatHitPoints(27, 1, "Health", false, false, false);

        private readonly ClassStat height = new ClassStat(28, 1234567890, "Height", false, false, false);

        private readonly ClassStat dms = new ClassStat(29, 1234567890, "DMS", false, false, false);

        private readonly ClassStat can = new ClassStat(30, 1234567890, "Can", false, false, false);

        private readonly ClassStat face = new ClassStat(31, 1234567890, "Face", false, false, false);

        private readonly ClassStat hairMesh = new ClassStat(32, 0, "HairMesh", false, false, false);

        private readonly ClassStat side = new ClassStat(33, 0, "Side", false, false, false);

        private readonly ClassStat deadTimer = new ClassStat(34, 0, "DeadTimer", false, false, false);

        private readonly ClassStat accessCount = new ClassStat(35, 1234567890, "AccessCount", false, false, false);

        private readonly ClassStat attackCount = new ClassStat(36, 1234567890, "AttackCount", false, false, false);

        private readonly StatTitleLevel titleLevel = new StatTitleLevel(37, 1, "TitleLevel", false, false, false);

        private readonly ClassStat backMesh = new ClassStat(38, 0, "BackMesh", false, false, false);

        private readonly ClassStat shoulderMeshHolder = new ClassStat(39, 0, "WeaponMeshRight", false, false, false);

        private readonly ClassStat alienXP = new ClassStat(40, 0, "AlienXP", false, false, false);

        private readonly ClassStat fabricType = new ClassStat(41, 1234567890, "FabricType", false, false, false);

        private readonly ClassStat catMesh = new ClassStat(42, 1234567890, "CATMesh", false, false, false);

        private readonly ClassStat parentType = new ClassStat(43, 1234567890, "ParentType", false, false, false);

        private readonly ClassStat parentInstance = new ClassStat(44, 1234567890, "ParentInstance", false, false, false);

        private readonly ClassStat beltSlots = new ClassStat(45, 0, "BeltSlots", false, false, false);

        private readonly ClassStat bandolierSlots = new ClassStat(46, 1234567890, "BandolierSlots", false, false, false);

        private readonly ClassStat fatness = new ClassStat(47, 1234567890, "Fatness", false, false, false);

        private readonly ClassStat clanLevel = new ClassStat(48, 1234567890, "ClanLevel", false, false, false);

        private readonly ClassStat insuranceTime = new ClassStat(49, 0, "InsuranceTime", false, false, false);

        private readonly ClassStat inventoryTimeout = new ClassStat(
            50, 1234567890, "InventoryTimeout", false, false, false);

        private readonly ClassStat aggDef = new ClassStat(51, 100, "AggDef", false, false, false);

        private readonly ClassStat xp = new ClassStat(52, 0, "XP", false, false, false);

        private readonly StatIP ip = new StatIP(53, 1500, "IP", false, false, false);

        private readonly ClassStat level = new ClassStat(54, 1234567890, "Level", false, false, false);

        private readonly ClassStat inventoryId = new ClassStat(55, 1234567890, "InventoryId", false, false, false);

        private readonly ClassStat timeSinceCreation = new ClassStat(
            56, 1234567890, "TimeSinceCreation", false, false, false);

        private readonly ClassStat lastXP = new ClassStat(57, 0, "LastXP", false, false, false);

        private readonly ClassStat age = new ClassStat(58, 0, "Age", false, false, false);

        private readonly ClassStat sex = new ClassStat(59, 1234567890, "Sex", false, false, false);

        private readonly ClassStat profession = new ClassStat(60, 1234567890, "Profession", false, false, false);

        private readonly ClassStat cash = new ClassStat(61, 0, "Cash", false, false, false);

        private readonly ClassStat alignment = new ClassStat(62, 0, "Alignment", false, false, false);

        private readonly ClassStat attitude = new ClassStat(63, 0, "Attitude", false, false, false);

        private readonly ClassStat headMesh = new ClassStat(64, 0, "HeadMesh", false, false, false);

        private readonly ClassStat missionBits5 = new ClassStat(65, 0, "MissionBits5", false, false, false);

        private readonly ClassStat missionBits6 = new ClassStat(66, 0, "MissionBits6", false, false, false);

        private readonly ClassStat missionBits7 = new ClassStat(67, 0, "MissionBits7", false, false, false);

        private readonly ClassStat veteranPoints = new ClassStat(68, 0, "VeteranPoints", false, false, false);

        private readonly ClassStat monthsPaid = new ClassStat(69, 0, "MonthsPaid", false, false, false);

        private readonly ClassStat speedPenalty = new ClassStat(70, 1234567890, "SpeedPenalty", false, false, false);

        private readonly ClassStat totalMass = new ClassStat(71, 1234567890, "TotalMass", false, false, false);

        private readonly ClassStat itemType = new ClassStat(72, 0, "ItemType", false, false, false);

        private readonly ClassStat repairDifficulty = new ClassStat(
            73, 1234567890, "RepairDifficulty", false, false, false);

        private readonly ClassStat price = new ClassStat(74, 1234567890, "Price", false, false, false);

        private readonly ClassStat metaType = new ClassStat(75, 0, "MetaType", false, false, false);

        private readonly ClassStat itemClass = new ClassStat(76, 1234567890, "ItemClass", false, false, false);

        private readonly ClassStat repairSkill = new ClassStat(77, 1234567890, "RepairSkill", false, false, false);

        private readonly ClassStat currentMass = new ClassStat(78, 0, "CurrentMass", false, false, false);

        private readonly ClassStat icon = new ClassStat(79, 0, "Icon", false, false, false);

        private readonly ClassStat primaryItemType = new ClassStat(
            80, 1234567890, "PrimaryItemType", false, false, false);

        private readonly ClassStat primaryItemInstance = new ClassStat(
            81, 1234567890, "PrimaryItemInstance", false, false, false);

        private readonly ClassStat secondaryItemType = new ClassStat(
            82, 1234567890, "SecondaryItemType", false, false, false);

        private readonly ClassStat secondaryItemInstance = new ClassStat(
            83, 1234567890, "SecondaryItemInstance", false, false, false);

        private readonly ClassStat userType = new ClassStat(84, 1234567890, "UserType", false, false, false);

        private readonly ClassStat userInstance = new ClassStat(85, 1234567890, "UserInstance", false, false, false);

        private readonly ClassStat areaType = new ClassStat(86, 1234567890, "AreaType", false, false, false);

        private readonly ClassStat areaInstance = new ClassStat(87, 1234567890, "AreaInstance", false, false, false);

        private readonly ClassStat defaultPos = new ClassStat(88, 1234567890, "DefaultPos", false, false, false);

        private readonly ClassStat race = new ClassStat(89, 1, "Race", false, false, false);

        private readonly ClassStat projectileAC = new ClassStat(90, 0, "ProjectileAC", true, false, false);

        private readonly ClassStat meleeAC = new ClassStat(91, 0, "MeleeAC", true, false, false);

        private readonly ClassStat energyAC = new ClassStat(92, 0, "EnergyAC", true, false, false);

        private readonly ClassStat chemicalAC = new ClassStat(93, 0, "ChemicalAC", true, false, false);

        private readonly ClassStat radiationAC = new ClassStat(94, 0, "RadiationAC", true, false, false);

        private readonly ClassStat coldAC = new ClassStat(95, 0, "ColdAC", true, false, false);

        private readonly ClassStat poisonAC = new ClassStat(96, 0, "PoisonAC", true, false, false);

        private readonly ClassStat fireAC = new ClassStat(97, 0, "FireAC", true, false, false);

        private readonly ClassStat stateAction = new ClassStat(98, 1234567890, "StateAction", true, false, false);

        private readonly ClassStat itemAnim = new ClassStat(99, 1234567890, "ItemAnim", true, false, false);

        private readonly StatSkill martialArts = new StatSkill(100, 5, "MartialArts", true, false, false);

        private readonly StatSkill meleeMultiple = new StatSkill(101, 5, "MeleeMultiple", true, false, false);

        private readonly StatSkill onehBluntWeapons = new StatSkill(102, 5, "1hBluntWeapons", true, false, false);

        private readonly StatSkill onehEdgedWeapon = new StatSkill(103, 5, "1hEdgedWeapon", true, false, false);

        private readonly StatSkill meleeEnergyWeapon = new StatSkill(104, 5, "MeleeEnergyWeapon", true, false, false);

        private readonly StatSkill twohEdgedWeapons = new StatSkill(105, 5, "2hEdgedWeapons", true, false, false);

        private readonly StatSkill piercing = new StatSkill(106, 5, "Piercing", true, false, false);

        private readonly StatSkill twohBluntWeapons = new StatSkill(107, 5, "2hBluntWeapons", true, false, false);

        private readonly StatSkill throwingKnife = new StatSkill(108, 5, "ThrowingKnife", true, false, false);

        private readonly StatSkill grenade = new StatSkill(109, 5, "Grenade", true, false, false);

        private readonly StatSkill thrownGrapplingWeapons = new StatSkill(
            110, 5, "ThrownGrapplingWeapons", true, false, false);

        private readonly StatSkill bow = new StatSkill(111, 5, "Bow", true, false, false);

        private readonly StatSkill pistol = new StatSkill(112, 5, "Pistol", true, false, false);

        private readonly StatSkill rifle = new StatSkill(113, 5, "Rifle", true, false, false);

        private readonly StatSkill subMachineGun = new StatSkill(114, 5, "SubMachineGun", true, false, false);

        private readonly StatSkill shotgun = new StatSkill(115, 5, "Shotgun", true, false, false);

        private readonly StatSkill assaultRifle = new StatSkill(116, 5, "AssaultRifle", true, false, false);

        private readonly StatSkill driveWater = new StatSkill(117, 5, "DriveWater", true, false, false);

        private readonly StatSkill closeCombatInitiative = new StatSkill(
            118, 5, "CloseCombatInitiative", true, false, false);

        private readonly StatSkill distanceWeaponInitiative = new StatSkill(
            119, 5, "DistanceWeaponInitiative", true, false, false);

        private readonly StatSkill physicalProwessInitiative = new StatSkill(
            120, 5, "PhysicalProwessInitiative", true, false, false);

        private readonly StatSkill bowSpecialAttack = new StatSkill(121, 5, "BowSpecialAttack", true, false, false);

        private readonly StatSkill senseImprovement = new StatSkill(122, 5, "SenseImprovement", true, false, false);

        private readonly StatSkill firstAid = new StatSkill(123, 5, "FirstAid", true, false, false);

        private readonly StatSkill treatment = new StatSkill(124, 5, "Treatment", true, false, false);

        private readonly StatSkill mechanicalEngineering = new StatSkill(
            125, 5, "MechanicalEngineering", true, false, false);

        private readonly StatSkill electricalEngineering = new StatSkill(
            126, 5, "ElectricalEngineering", true, false, false);

        private readonly StatSkill materialMetamorphose = new StatSkill(
            127, 5, "MaterialMetamorphose", true, false, false);

        private readonly StatSkill biologicalMetamorphose = new StatSkill(
            128, 5, "BiologicalMetamorphose", true, false, false);

        private readonly StatSkill psychologicalModification = new StatSkill(
            129, 5, "PsychologicalModification", true, false, false);

        private readonly StatSkill materialCreation = new StatSkill(130, 5, "MaterialCreation", true, false, false);

        private readonly StatSkill materialLocation = new StatSkill(131, 5, "MaterialLocation", true, false, false);

        private readonly StatSkill nanoEnergyPool = new StatSkill(132, 5, "NanoEnergyPool", true, false, false);

        private readonly StatSkill lrEnergyWeapon = new StatSkill(133, 5, "LR_EnergyWeapon", true, false, false);

        private readonly StatSkill lrMultipleWeapon = new StatSkill(134, 5, "LR_MultipleWeapon", true, false, false);

        private readonly StatSkill disarmTrap = new StatSkill(135, 5, "DisarmTrap", true, false, false);

        private readonly StatSkill perception = new StatSkill(136, 5, "Perception", true, false, false);

        private readonly StatSkill adventuring = new StatSkill(137, 5, "Adventuring", true, false, false);

        private readonly StatSkill swim = new StatSkill(138, 5, "Swim", true, false, false);

        private readonly StatSkill driveAir = new StatSkill(139, 5, "DriveAir", true, false, false);

        private readonly StatSkill mapNavigation = new StatSkill(140, 5, "MapNavigation", true, false, false);

        private readonly StatSkill tutoring = new StatSkill(141, 5, "Tutoring", true, false, false);

        private readonly StatSkill brawl = new StatSkill(142, 5, "Brawl", true, false, false);

        private readonly StatSkill riposte = new StatSkill(143, 5, "Riposte", true, false, false);

        private readonly StatSkill dimach = new StatSkill(144, 5, "Dimach", true, false, false);

        private readonly StatSkill parry = new StatSkill(145, 5, "Parry", true, false, false);

        private readonly StatSkill sneakAttack = new StatSkill(146, 5, "SneakAttack", true, false, false);

        private readonly StatSkill fastAttack = new StatSkill(147, 5, "FastAttack", true, false, false);

        private readonly StatSkill burst = new StatSkill(148, 5, "Burst", true, false, false);

        private readonly StatSkill nanoProwessInitiative = new StatSkill(
            149, 5, "NanoProwessInitiative", true, false, false);

        private readonly StatSkill flingShot = new StatSkill(150, 5, "FlingShot", true, false, false);

        private readonly StatSkill aimedShot = new StatSkill(151, 5, "AimedShot", true, false, false);

        private readonly StatSkill bodyDevelopment = new StatSkill(152, 5, "BodyDevelopment", true, false, false);

        private readonly StatSkill duck = new StatSkill(153, 5, "Duck", true, false, false);

        private readonly StatSkill dodge = new StatSkill(154, 5, "Dodge", true, false, false);

        private readonly StatSkill evade = new StatSkill(155, 5, "Evade", true, false, false);

        private readonly StatSkill runSpeed = new StatSkill(156, 5, "RunSpeed", true, false, false);

        private readonly StatSkill fieldQuantumPhysics = new StatSkill(
            157, 5, "FieldQuantumPhysics", true, false, false);

        private readonly StatSkill weaponSmithing = new StatSkill(158, 5, "WeaponSmithing", true, false, false);

        private readonly StatSkill pharmaceuticals = new StatSkill(159, 5, "Pharmaceuticals", true, false, false);

        private readonly StatSkill nanoProgramming = new StatSkill(160, 5, "NanoProgramming", true, false, false);

        private readonly StatSkill computerLiteracy = new StatSkill(161, 5, "ComputerLiteracy", true, false, false);

        private readonly StatSkill psychology = new StatSkill(162, 5, "Psychology", true, false, false);

        private readonly StatSkill chemistry = new StatSkill(163, 5, "Chemistry", true, false, false);

        private readonly StatSkill concealment = new StatSkill(164, 5, "Concealment", true, false, false);

        private readonly StatSkill breakingEntry = new StatSkill(165, 5, "BreakingEntry", true, false, false);

        private readonly StatSkill driveGround = new StatSkill(166, 5, "DriveGround", true, false, false);

        private readonly StatSkill fullAuto = new StatSkill(167, 5, "FullAuto", true, false, false);

        private readonly StatSkill nanoAC = new StatSkill(168, 5, "NanoAC", true, false, false);

        private readonly ClassStat alienLevel = new ClassStat(169, 0, "AlienLevel", false, false, false);

        private readonly ClassStat healthChangeBest = new ClassStat(
            170, 1234567890, "HealthChangeBest", false, false, false);

        private readonly ClassStat healthChangeWorst = new ClassStat(
            171, 1234567890, "HealthChangeWorst", false, false, false);

        private readonly ClassStat healthChange = new ClassStat(172, 1234567890, "HealthChange", false, false, false);

        private readonly ClassStat currentMovementMode = new ClassStat(
            173, 3, "CurrentMovementMode", false, false, false);

        private readonly ClassStat prevMovementMode = new ClassStat(174, 3, "PrevMovementMode", false, false, false);

        private readonly ClassStat autoLockTimeDefault = new ClassStat(
            175, 1234567890, "AutoLockTimeDefault", false, false, false);

        private readonly ClassStat autoUnlockTimeDefault = new ClassStat(
            176, 1234567890, "AutoUnlockTimeDefault", false, false, false);

        private readonly ClassStat moreFlags = new ClassStat(177, 1234567890, "MoreFlags", false, false, true);

        private readonly StatAlienNextXP alienNextXP = new StatAlienNextXP(
            178, 1500, "AlienNextXP", false, false, false);

        private readonly ClassStat npcFlags = new ClassStat(179, 1234567890, "NPCFlags", false, false, false);

        private readonly ClassStat currentNCU = new ClassStat(180, 0, "CurrentNCU", false, false, false);

        private readonly ClassStat maxNCU = new ClassStat(181, 8, "MaxNCU", false, false, false);

        private readonly ClassStat specialization = new ClassStat(182, 0, "Specialization", false, false, false);

        private readonly ClassStat effectIcon = new ClassStat(183, 1234567890, "EffectIcon", false, false, false);

        private readonly ClassStat buildingType = new ClassStat(184, 1234567890, "BuildingType", false, false, false);

        private readonly ClassStat buildingInstance = new ClassStat(
            185, 1234567890, "BuildingInstance", false, false, false);

        private readonly ClassStat cardOwnerType = new ClassStat(186, 1234567890, "CardOwnerType", false, false, false);

        private readonly ClassStat cardOwnerInstance = new ClassStat(
            187, 1234567890, "CardOwnerInstance", false, false, false);

        private readonly ClassStat buildingComplexInst = new ClassStat(
            188, 1234567890, "BuildingComplexInst", false, false, false);

        private readonly ClassStat exitInstance = new ClassStat(189, 1234567890, "ExitInstance", false, false, false);

        private readonly ClassStat nextDoorInBuilding = new ClassStat(
            190, 1234567890, "NextDoorInBuilding", false, false, false);

        private readonly ClassStat lastConcretePlayfieldInstance = new ClassStat(
            191, 0, "LastConcretePlayfieldInstance", false, false, false);

        private readonly ClassStat extenalPlayfieldInstance = new ClassStat(
            192, 1234567890, "ExtenalPlayfieldInstance", false, false, false);

        private readonly ClassStat extenalDoorInstance = new ClassStat(
            193, 1234567890, "ExtenalDoorInstance", false, false, false);

        private readonly ClassStat inPlay = new ClassStat(194, 0, "InPlay", false, false, false);

        private readonly ClassStat accessKey = new ClassStat(195, 1234567890, "AccessKey", false, false, false);

        private readonly ClassStat petMaster = new ClassStat(196, 1234567890, "PetMaster", false, false, false);

        private readonly ClassStat orientationMode = new ClassStat(
            197, 1234567890, "OrientationMode", false, false, false);

        private readonly ClassStat sessionTime = new ClassStat(198, 1234567890, "SessionTime", false, false, false);

        private readonly ClassStat rp = new ClassStat(199, 0, "RP", false, false, false);

        private readonly ClassStat conformity = new ClassStat(200, 1234567890, "Conformity", false, false, false);

        private readonly ClassStat aggressiveness = new ClassStat(
            201, 1234567890, "Aggressiveness", false, false, false);

        private readonly ClassStat stability = new ClassStat(202, 1234567890, "Stability", false, false, false);

        private readonly ClassStat extroverty = new ClassStat(203, 1234567890, "Extroverty", false, false, false);

        private readonly ClassStat breedHostility = new ClassStat(
            204, 1234567890, "BreedHostility", false, false, false);

        private readonly ClassStat reflectProjectileAC = new ClassStat(
            205, 0, "ReflectProjectileAC", true, false, false);

        private readonly ClassStat reflectMeleeAC = new ClassStat(206, 0, "ReflectMeleeAC", true, false, false);

        private readonly ClassStat reflectEnergyAC = new ClassStat(207, 0, "ReflectEnergyAC", true, false, false);

        private readonly ClassStat reflectChemicalAC = new ClassStat(208, 0, "ReflectChemicalAC", true, false, false);

        private readonly ClassStat weaponMeshHolder = new ClassStat(209, 0, "WeaponMeshRight", false, false, false);

        private readonly ClassStat rechargeDelay = new ClassStat(210, 1234567890, "RechargeDelay", false, false, false);

        private readonly ClassStat equipDelay = new ClassStat(211, 1234567890, "EquipDelay", false, false, false);

        private readonly ClassStat maxEnergy = new ClassStat(212, 1234567890, "MaxEnergy", false, false, false);

        private readonly ClassStat teamSide = new ClassStat(213, 0, "TeamSide", false, false, false);

        private readonly StatNanoPoints currentNano = new StatNanoPoints(214, 1, "CurrentNano", false, false, false);

        private readonly ClassStat gmLevel = new ClassStat(215, 0, "GmLevel", false, true, false);

        private readonly ClassStat reflectRadiationAC = new ClassStat(216, 0, "ReflectRadiationAC", true, false, false);

        private readonly ClassStat reflectColdAC = new ClassStat(217, 0, "ReflectColdAC", true, false, false);

        private readonly ClassStat reflectNanoAC = new ClassStat(218, 0, "ReflectNanoAC", true, false, false);

        private readonly ClassStat reflectFireAC = new ClassStat(219, 0, "ReflectFireAC", true, false, false);

        private readonly ClassStat currBodyLocation = new ClassStat(220, 0, "CurrBodyLocation", false, false, false);

        private readonly StatNano maxNanoEnergy = new StatNano(221, 1, "MaxNanoEnergy", false, false, false);

        private readonly ClassStat accumulatedDamage = new ClassStat(
            222, 1234567890, "AccumulatedDamage", false, false, false);

        private readonly ClassStat canChangeClothes = new ClassStat(
            223, 1234567890, "CanChangeClothes", false, false, false);

        private readonly ClassStat features = new ClassStat(224, 6, "Features", false, false, false);

        private readonly ClassStat reflectPoisonAC = new ClassStat(225, 0, "ReflectPoisonAC", false, false, false);

        private readonly ClassStat shieldProjectileAC = new ClassStat(226, 0, "ShieldProjectileAC", true, false, false);

        private readonly ClassStat shieldMeleeAC = new ClassStat(227, 0, "ShieldMeleeAC", true, false, false);

        private readonly ClassStat shieldEnergyAC = new ClassStat(228, 0, "ShieldEnergyAC", true, false, false);

        private readonly ClassStat shieldChemicalAC = new ClassStat(229, 0, "ShieldChemicalAC", true, false, false);

        private readonly ClassStat shieldRadiationAC = new ClassStat(230, 0, "ShieldRadiationAC", true, false, false);

        private readonly ClassStat shieldColdAC = new ClassStat(231, 0, "ShieldColdAC", true, false, false);

        private readonly ClassStat shieldNanoAC = new ClassStat(232, 0, "ShieldNanoAC", true, false, false);

        private readonly ClassStat shieldFireAC = new ClassStat(233, 0, "ShieldFireAC", true, false, false);

        private readonly ClassStat shieldPoisonAC = new ClassStat(234, 0, "ShieldPoisonAC", true, false, false);

        private readonly ClassStat berserkMode = new ClassStat(235, 1234567890, "BerserkMode", false, false, false);

        private readonly ClassStat insurancePercentage = new ClassStat(
            236, 0, "InsurancePercentage", false, false, false);

        private readonly ClassStat changeSideCount = new ClassStat(237, 0, "ChangeSideCount", false, false, false);

        private readonly ClassStat absorbProjectileAC = new ClassStat(238, 0, "AbsorbProjectileAC", true, false, false);

        private readonly ClassStat absorbMeleeAC = new ClassStat(239, 0, "AbsorbMeleeAC", true, false, false);

        private readonly ClassStat absorbEnergyAC = new ClassStat(240, 0, "AbsorbEnergyAC", true, false, false);

        private readonly ClassStat absorbChemicalAC = new ClassStat(241, 0, "AbsorbChemicalAC", true, false, false);

        private readonly ClassStat absorbRadiationAC = new ClassStat(242, 0, "AbsorbRadiationAC", true, false, false);

        private readonly ClassStat absorbColdAC = new ClassStat(243, 0, "AbsorbColdAC", true, false, false);

        private readonly ClassStat absorbFireAC = new ClassStat(244, 0, "AbsorbFireAC", true, false, false);

        private readonly ClassStat absorbPoisonAC = new ClassStat(245, 0, "AbsorbPoisonAC", true, false, false);

        private readonly ClassStat absorbNanoAC = new ClassStat(246, 0, "AbsorbNanoAC", true, false, false);

        private readonly ClassStat temporarySkillReduction = new ClassStat(
            247, 0, "TemporarySkillReduction", false, false, false);

        private readonly ClassStat birthDate = new ClassStat(248, 1234567890, "BirthDate", false, false, false);

        private readonly ClassStat lastSaved = new ClassStat(249, 1234567890, "LastSaved", false, false, false);

        private readonly ClassStat soundVolume = new ClassStat(250, 1234567890, "SoundVolume", false, false, false);

        private readonly ClassStat petCounter = new ClassStat(251, 1234567890, "PetCounter", false, false, false);

        private readonly ClassStat metersWalked = new ClassStat(252, 1234567890, "MeetersWalked", false, false, false);

        private readonly ClassStat questLevelsSolved = new ClassStat(
            253, 1234567890, "QuestLevelsSolved", false, false, false);

        // Accumulated Tokens?

        private readonly ClassStat monsterLevelsKilled = new ClassStat(
            254, 1234567890, "MonsterLevelsKilled", false, false, false);

        private readonly ClassStat pvPLevelsKilled = new ClassStat(
            255, 1234567890, "PvPLevelsKilled", false, false, false);

        private readonly ClassStat missionBits1 = new ClassStat(256, 0, "MissionBits1", false, false, false);

        private readonly ClassStat missionBits2 = new ClassStat(257, 0, "MissionBits2", false, false, false);

        private readonly ClassStat accessGrant = new ClassStat(258, 1234567890, "AccessGrant", false, false, false);

        private readonly ClassStat doorFlags = new ClassStat(259, 1234567890, "DoorFlags", false, false, false);

        private readonly ClassStat clanHierarchy = new ClassStat(260, 1234567890, "ClanHierarchy", false, false, false);

        private readonly ClassStat questStat = new ClassStat(261, 1234567890, "QuestStat", false, false, false);

        private readonly ClassStat clientActivated = new ClassStat(
            262, 1234567890, "ClientActivated", false, false, false);

        private readonly ClassStat personalResearchLevel = new ClassStat(
            263, 0, "PersonalResearchLevel", false, false, false);

        private readonly ClassStat globalResearchLevel = new ClassStat(
            264, 0, "GlobalResearchLevel", false, false, false);

        private readonly ClassStat personalResearchGoal = new ClassStat(
            265, 0, "PersonalResearchGoal", false, false, false);

        private readonly ClassStat globalResearchGoal = new ClassStat(266, 0, "GlobalResearchGoal", false, false, false);

        private readonly ClassStat turnSpeed = new ClassStat(267, 40000, "TurnSpeed", false, false, false);

        private readonly ClassStat liquidType = new ClassStat(268, 1234567890, "LiquidType", false, false, false);

        private readonly ClassStat gatherSound = new ClassStat(269, 1234567890, "GatherSound", false, false, false);

        private readonly ClassStat castSound = new ClassStat(270, 1234567890, "CastSound", false, false, false);

        private readonly ClassStat travelSound = new ClassStat(271, 1234567890, "TravelSound", false, false, false);

        private readonly ClassStat hitSound = new ClassStat(272, 1234567890, "HitSound", false, false, false);

        private readonly ClassStat secondaryItemTemplate = new ClassStat(
            273, 1234567890, "SecondaryItemTemplate", false, false, false);

        private readonly ClassStat equippedWeapons = new ClassStat(
            274, 1234567890, "EquippedWeapons", false, false, false);

        private readonly ClassStat xpKillRange = new ClassStat(275, 5, "XPKillRange", false, false, false);

        private readonly ClassStat amsModifier = new ClassStat(276, 0, "AMSModifier", false, false, false);

        private readonly ClassStat dmsModifier = new ClassStat(277, 0, "DMSModifier", false, false, false);

        private readonly ClassStat projectileDamageModifier = new ClassStat(
            278, 0, "ProjectileDamageModifier", false, false, false);

        private readonly ClassStat meleeDamageModifier = new ClassStat(
            279, 0, "MeleeDamageModifier", false, false, false);

        private readonly ClassStat energyDamageModifier = new ClassStat(
            280, 0, "EnergyDamageModifier", false, false, false);

        private readonly ClassStat chemicalDamageModifier = new ClassStat(
            281, 0, "ChemicalDamageModifier", false, false, false);

        private readonly ClassStat radiationDamageModifier = new ClassStat(
            282, 0, "RadiationDamageModifier", false, false, false);

        private readonly ClassStat itemHateValue = new ClassStat(283, 1234567890, "ItemHateValue", false, false, false);

        private readonly ClassStat damageBonus = new ClassStat(284, 1234567890, "DamageBonus", false, false, false);

        private readonly ClassStat maxDamage = new ClassStat(285, 1234567890, "MaxDamage", false, false, false);

        private readonly ClassStat minDamage = new ClassStat(286, 1234567890, "MinDamage", false, false, false);

        private readonly ClassStat attackRange = new ClassStat(287, 1234567890, "AttackRange", false, false, false);

        private readonly ClassStat hateValueModifyer = new ClassStat(
            288, 1234567890, "HateValueModifyer", false, false, false);

        private readonly ClassStat trapDifficulty = new ClassStat(
            289, 1234567890, "TrapDifficulty", false, false, false);

        private readonly ClassStat statOne = new ClassStat(290, 1234567890, "StatOne", false, false, false);

        private readonly ClassStat numAttackEffects = new ClassStat(
            291, 1234567890, "NumAttackEffects", false, false, false);

        private readonly ClassStat defaultAttackType = new ClassStat(
            292, 1234567890, "DefaultAttackType", false, false, false);

        private readonly ClassStat itemSkill = new ClassStat(293, 1234567890, "ItemSkill", false, false, false);

        private readonly ClassStat itemDelay = new ClassStat(294, 1234567890, "ItemDelay", false, false, false);

        private readonly ClassStat itemOpposedSkill = new ClassStat(
            295, 1234567890, "ItemOpposedSkill", false, false, false);

        private readonly ClassStat itemSIS = new ClassStat(296, 1234567890, "ItemSIS", false, false, false);

        private readonly ClassStat interactionRadius = new ClassStat(
            297, 1234567890, "InteractionRadius", false, false, false);

        private readonly ClassStat placement = new ClassStat(298, 1234567890, "Placement", false, false, false);

        private readonly ClassStat lockDifficulty = new ClassStat(
            299, 1234567890, "LockDifficulty", false, false, false);

        private readonly ClassStat members = new ClassStat(300, 999, "Members", false, false, false);

        private readonly ClassStat minMembers = new ClassStat(301, 1234567890, "MinMembers", false, false, false);

        private readonly ClassStat clanPrice = new ClassStat(302, 1234567890, "ClanPrice", false, false, false);

        private readonly ClassStat missionBits3 = new ClassStat(303, 0, "MissionBits3", false, false, false);

        private readonly ClassStat clanType = new ClassStat(304, 1234567890, "ClanType", false, false, false);

        private readonly ClassStat clanInstance = new ClassStat(305, 1234567890, "ClanInstance", false, false, false);

        private readonly ClassStat voteCount = new ClassStat(306, 1234567890, "VoteCount", false, false, false);

        private readonly ClassStat memberType = new ClassStat(307, 1234567890, "MemberType", false, false, false);

        private readonly ClassStat memberInstance = new ClassStat(
            308, 1234567890, "MemberInstance", false, false, false);

        private readonly ClassStat globalClanType = new ClassStat(
            309, 1234567890, "GlobalClanType", false, false, false);

        private readonly ClassStat globalClanInstance = new ClassStat(
            310, 1234567890, "GlobalClanInstance", false, false, false);

        private readonly ClassStat coldDamageModifier = new ClassStat(
            311, 1234567890, "ColdDamageModifier", false, false, false);

        private readonly ClassStat clanUpkeepInterval = new ClassStat(
            312, 1234567890, "ClanUpkeepInterval", false, false, false);

        private readonly ClassStat timeSinceUpkeep = new ClassStat(
            313, 1234567890, "TimeSinceUpkeep", false, false, false);

        private readonly ClassStat clanFinalized = new ClassStat(314, 1234567890, "ClanFinalized", false, false, false);

        private readonly ClassStat nanoDamageModifier = new ClassStat(315, 0, "NanoDamageModifier", false, false, false);

        private readonly ClassStat fireDamageModifier = new ClassStat(316, 0, "FireDamageModifier", false, false, false);

        private readonly ClassStat poisonDamageModifier = new ClassStat(
            317, 0, "PoisonDamageModifier", false, false, false);

        private readonly ClassStat npCostModifier = new ClassStat(318, 0, "NPCostModifier", false, false, false);

        private readonly ClassStat xpModifier = new ClassStat(319, 0, "XPModifier", false, false, false);

        private readonly ClassStat breedLimit = new ClassStat(320, 1234567890, "BreedLimit", false, false, false);

        private readonly ClassStat genderLimit = new ClassStat(321, 1234567890, "GenderLimit", false, false, false);

        private readonly ClassStat levelLimit = new ClassStat(322, 1234567890, "LevelLimit", false, false, false);

        private readonly ClassStat playerKilling = new ClassStat(323, 1234567890, "PlayerKilling", false, false, false);

        private readonly ClassStat teamAllowed = new ClassStat(324, 1234567890, "TeamAllowed", false, false, false);

        private readonly ClassStat weaponDisallowedType = new ClassStat(
            325, 1234567890, "WeaponDisallowedType", false, false, false);

        private readonly ClassStat weaponDisallowedInstance = new ClassStat(
            326, 1234567890, "WeaponDisallowedInstance", false, false, false);

        private readonly ClassStat taboo = new ClassStat(327, 1234567890, "Taboo", false, false, false);

        private readonly ClassStat compulsion = new ClassStat(328, 1234567890, "Compulsion", false, false, false);

        private readonly ClassStat skillDisabled = new ClassStat(329, 1234567890, "SkillDisabled", false, false, false);

        private readonly ClassStat clanItemType = new ClassStat(330, 1234567890, "ClanItemType", false, false, false);

        private readonly ClassStat clanItemInstance = new ClassStat(
            331, 1234567890, "ClanItemInstance", false, false, false);

        private readonly ClassStat debuffFormula = new ClassStat(332, 1234567890, "DebuffFormula", false, false, false);

        private readonly ClassStat pvpRating = new ClassStat(333, 1300, "PvP_Rating", false, false, false);

        private readonly ClassStat savedXP = new ClassStat(334, 0, "SavedXP", false, false, false);

        private readonly ClassStat doorBlockTime = new ClassStat(335, 1234567890, "DoorBlockTime", false, false, false);

        private readonly ClassStat overrideTexture = new ClassStat(
            336, 1234567890, "OverrideTexture", false, false, false);

        private readonly ClassStat overrideMaterial = new ClassStat(
            337, 1234567890, "OverrideMaterial", false, false, false);

        private readonly ClassStat deathReason = new ClassStat(338, 1234567890, "DeathReason", false, false, false);

        private readonly ClassStat damageOverrideType = new ClassStat(
            339, 1234567890, "DamageOverrideType", false, false, false);

        private readonly ClassStat brainType = new ClassStat(340, 1234567890, "BrainType", false, false, false);

        private readonly ClassStat xpBonus = new ClassStat(341, 1234567890, "XPBonus", false, false, false);

        private readonly StatHealInterval healInterval = new StatHealInterval(
            342, 29, "HealInterval", false, false, false);

        private readonly StatHealDelta healDelta = new StatHealDelta(343, 1234567890, "HealDelta", false, false, false);

        private readonly ClassStat monsterTexture = new ClassStat(
            344, 1234567890, "MonsterTexture", false, false, false);

        private readonly ClassStat hasAlwaysLootable = new ClassStat(
            345, 1234567890, "HasAlwaysLootable", false, false, false);

        private readonly ClassStat tradeLimit = new ClassStat(346, 1234567890, "TradeLimit", false, false, false);

        private readonly ClassStat faceTexture = new ClassStat(347, 1234567890, "FaceTexture", false, false, false);

        private readonly ClassStat specialCondition = new ClassStat(348, 1, "SpecialCondition", false, false, false);

        private readonly ClassStat autoAttackFlags = new ClassStat(349, 5, "AutoAttackFlags", false, false, false);

        private readonly StatNextXP nextXP = new StatNextXP(350, 1450, "NextXP", false, false, false);

        private readonly ClassStat teleportPauseMilliSeconds = new ClassStat(
            351, 1234567890, "TeleportPauseMilliSeconds", false, false, false);

        private readonly ClassStat sisCap = new ClassStat(352, 1234567890, "SISCap", false, false, false);

        private readonly ClassStat animSet = new ClassStat(353, 1234567890, "AnimSet", false, false, false);

        private readonly ClassStat attackType = new ClassStat(354, 1234567890, "AttackType", false, false, false);

        private readonly ClassStat nanoFocusLevel = new ClassStat(355, 0, "NanoFocusLevel", false, false, false);

        private readonly ClassStat npcHash = new ClassStat(356, 1234567890, "NPCHash", false, false, false);

        private readonly ClassStat collisionRadius = new ClassStat(
            357, 1234567890, "CollisionRadius", false, false, false);

        private readonly ClassStat outerRadius = new ClassStat(358, 1234567890, "OuterRadius", false, false, false);

        private readonly ClassStat monsterData = new ClassStat(359, 0, "MonsterData", false, false, true);

        private readonly ClassStat monsterScale = new ClassStat(360, 1234567890, "MonsterScale", false, false, true);

        private readonly ClassStat hitEffectType = new ClassStat(361, 1234567890, "HitEffectType", false, false, false);

        private readonly ClassStat resurrectDest = new ClassStat(362, 1234567890, "ResurrectDest", false, false, false);

        private readonly StatNanoInterval nanoInterval = new StatNanoInterval(
            363, 28, "NanoInterval", false, false, false);

        private readonly StatNanoDelta nanoDelta = new StatNanoDelta(364, 1234567890, "NanoDelta", false, false, false);

        private readonly ClassStat reclaimItem = new ClassStat(365, 1234567890, "ReclaimItem", false, false, false);

        private readonly ClassStat gatherEffectType = new ClassStat(
            366, 1234567890, "GatherEffectType", false, false, false);

        private readonly ClassStat visualBreed = new ClassStat(367, 1234567890, "VisualBreed", false, false, true);

        private readonly ClassStat visualProfession = new ClassStat(
            368, 1234567890, "VisualProfession", false, false, true);

        private readonly ClassStat visualSex = new ClassStat(369, 1234567890, "VisualSex", false, false, true);

        private readonly ClassStat ritualTargetInst = new ClassStat(
            370, 1234567890, "RitualTargetInst", false, false, false);

        private readonly ClassStat skillTimeOnSelectedTarget = new ClassStat(
            371, 1234567890, "SkillTimeOnSelectedTarget", false, false, false);

        private readonly ClassStat lastSaveXP = new ClassStat(372, 0, "LastSaveXP", false, false, false);

        private readonly ClassStat extendedTime = new ClassStat(373, 1234567890, "ExtendedTime", false, false, false);

        private readonly ClassStat burstRecharge = new ClassStat(374, 1234567890, "BurstRecharge", false, false, false);

        private readonly ClassStat fullAutoRecharge = new ClassStat(
            375, 1234567890, "FullAutoRecharge", false, false, false);

        private readonly ClassStat gatherAbstractAnim = new ClassStat(
            376, 1234567890, "GatherAbstractAnim", false, false, false);

        private readonly ClassStat castTargetAbstractAnim = new ClassStat(
            377, 1234567890, "CastTargetAbstractAnim", false, false, false);

        private readonly ClassStat castSelfAbstractAnim = new ClassStat(
            378, 1234567890, "CastSelfAbstractAnim", false, false, false);

        private readonly ClassStat criticalIncrease = new ClassStat(
            379, 1234567890, "CriticalIncrease", false, false, false);

        private readonly ClassStat rangeIncreaserWeapon = new ClassStat(
            380, 0, "RangeIncreaserWeapon", false, false, false);

        private readonly ClassStat rangeIncreaserNF = new ClassStat(381, 0, "RangeIncreaserNF", false, false, false);

        private readonly ClassStat skillLockModifier = new ClassStat(382, 0, "SkillLockModifier", false, false, false);

        private readonly ClassStat interruptModifier = new ClassStat(
            383, 1234567890, "InterruptModifier", false, false, false);

        private readonly ClassStat acgEntranceStyles = new ClassStat(
            384, 1234567890, "ACGEntranceStyles", false, false, false);

        private readonly ClassStat chanceOfBreakOnSpellAttack = new ClassStat(
            385, 1234567890, "ChanceOfBreakOnSpellAttack", false, false, false);

        private readonly ClassStat chanceOfBreakOnDebuff = new ClassStat(
            386, 1234567890, "ChanceOfBreakOnDebuff", false, false, false);

        private readonly ClassStat dieAnim = new ClassStat(387, 1234567890, "DieAnim", false, false, false);

        private readonly ClassStat towerType = new ClassStat(388, 1234567890, "TowerType", false, false, false);

        private readonly ClassStat expansion = new ClassStat(389, 0, "Expansion", false, true, false);

        private readonly ClassStat lowresMesh = new ClassStat(390, 1234567890, "LowresMesh", false, false, false);

        private readonly ClassStat criticalDecrease = new ClassStat(
            391, 1234567890, "CriticalDecrease", false, false, false);

        private readonly ClassStat oldTimeExist = new ClassStat(392, 1234567890, "OldTimeExist", false, false, false);

        private readonly ClassStat resistModifier = new ClassStat(
            393, 1234567890, "ResistModifier", false, false, false);

        private readonly ClassStat chestFlags = new ClassStat(394, 1234567890, "ChestFlags", false, false, false);

        private readonly ClassStat primaryTemplateID = new ClassStat(
            395, 1234567890, "PrimaryTemplateID", false, false, false);

        private readonly ClassStat numberOfItems = new ClassStat(396, 1234567890, "NumberOfItems", false, false, false);

        private readonly ClassStat selectedTargetType = new ClassStat(
            397, 1234567890, "SelectedTargetType", false, false, false);

        private readonly ClassStat corpseHash = new ClassStat(398, 1234567890, "Corpse_Hash", false, false, false);

        private readonly ClassStat ammoName = new ClassStat(399, 1234567890, "AmmoName", false, false, false);

        private readonly ClassStat rotation = new ClassStat(400, 1234567890, "Rotation", false, false, false);

        private readonly ClassStat catAnim = new ClassStat(401, 1234567890, "CATAnim", false, false, false);

        private readonly ClassStat catAnimFlags = new ClassStat(402, 1234567890, "CATAnimFlags", false, false, false);

        private readonly ClassStat displayCATAnim = new ClassStat(
            403, 1234567890, "DisplayCATAnim", false, false, false);

        private readonly ClassStat displayCATMesh = new ClassStat(
            404, 1234567890, "DisplayCATMesh", false, false, false);

        private readonly ClassStat school = new ClassStat(405, 1234567890, "School", false, false, false);

        private readonly ClassStat nanoSpeed = new ClassStat(406, 1234567890, "NanoSpeed", false, false, false);

        private readonly ClassStat nanoPoints = new ClassStat(407, 1234567890, "NanoPoints", false, false, false);

        private readonly ClassStat trainSkill = new ClassStat(408, 1234567890, "TrainSkill", false, false, false);

        private readonly ClassStat trainSkillCost = new ClassStat(
            409, 1234567890, "TrainSkillCost", false, false, false);

        private readonly ClassStat isFightingMe = new ClassStat(410, 1234567890, "IsFightingMe", false, false, false);

        private readonly ClassStat nextFormula = new ClassStat(411, 1234567890, "NextFormula", false, false, false);

        private readonly ClassStat multipleCount = new ClassStat(412, 1234567890, "MultipleCount", false, false, false);

        private readonly ClassStat effectType = new ClassStat(413, 1234567890, "EffectType", false, false, false);

        private readonly ClassStat impactEffectType = new ClassStat(
            414, 1234567890, "ImpactEffectType", false, false, false);

        private readonly ClassStat corpseType = new ClassStat(415, 1234567890, "CorpseType", false, false, false);

        private readonly ClassStat corpseInstance = new ClassStat(
            416, 1234567890, "CorpseInstance", false, false, false);

        private readonly ClassStat corpseAnimKey = new ClassStat(417, 1234567890, "CorpseAnimKey", false, false, false);

        private readonly ClassStat unarmedTemplateInstance = new ClassStat(
            418, 0, "UnarmedTemplateInstance", false, false, false);

        private readonly ClassStat tracerEffectType = new ClassStat(
            419, 1234567890, "TracerEffectType", false, false, false);

        private readonly ClassStat ammoType = new ClassStat(420, 1234567890, "AmmoType", false, false, false);

        private readonly ClassStat charRadius = new ClassStat(421, 1234567890, "CharRadius", false, false, false);

        private readonly ClassStat chanceOfUse = new ClassStat(422, 1234567890, "ChanceOfUse", false, false, false);

        private readonly ClassStat currentState = new ClassStat(423, 0, "CurrentState", false, false, false);

        private readonly ClassStat armourType = new ClassStat(424, 1234567890, "ArmourType", false, false, false);

        private readonly ClassStat restModifier = new ClassStat(425, 1234567890, "RestModifier", false, false, false);

        private readonly ClassStat buyModifier = new ClassStat(426, 1234567890, "BuyModifier", false, false, false);

        private readonly ClassStat sellModifier = new ClassStat(427, 1234567890, "SellModifier", false, false, false);

        private readonly ClassStat castEffectType = new ClassStat(
            428, 1234567890, "CastEffectType", false, false, false);

        private readonly ClassStat npcBrainState = new ClassStat(429, 1234567890, "NPCBrainState", false, false, false);

        private readonly ClassStat waitState = new ClassStat(430, 2, "WaitState", false, false, false);

        private readonly ClassStat selectedTarget = new ClassStat(
            431, 1234567890, "SelectedTarget", false, false, false);

        private readonly ClassStat missionBits4 = new ClassStat(432, 0, "MissionBits4", false, false, false);

        private readonly ClassStat ownerInstance = new ClassStat(433, 1234567890, "OwnerInstance", false, false, false);

        private readonly ClassStat charState = new ClassStat(434, 1234567890, "CharState", false, false, false);

        private readonly ClassStat readOnly = new ClassStat(435, 1234567890, "ReadOnly", false, false, false);

        private readonly ClassStat damageType = new ClassStat(436, 1234567890, "DamageType", false, false, false);

        private readonly ClassStat collideCheckInterval = new ClassStat(
            437, 1234567890, "CollideCheckInterval", false, false, false);

        private readonly ClassStat playfieldType = new ClassStat(438, 1234567890, "PlayfieldType", false, false, false);

        private readonly ClassStat npcCommand = new ClassStat(439, 1234567890, "NPCCommand", false, false, false);

        private readonly ClassStat initiativeType = new ClassStat(
            440, 1234567890, "InitiativeType", false, false, false);

        private readonly ClassStat charTmp1 = new ClassStat(441, 1234567890, "CharTmp1", false, false, false);

        private readonly ClassStat charTmp2 = new ClassStat(442, 1234567890, "CharTmp2", false, false, false);

        private readonly ClassStat charTmp3 = new ClassStat(443, 1234567890, "CharTmp3", false, false, false);

        private readonly ClassStat charTmp4 = new ClassStat(444, 1234567890, "CharTmp4", false, false, false);

        private readonly ClassStat npcCommandArg = new ClassStat(445, 1234567890, "NPCCommandArg", false, false, false);

        private readonly ClassStat nameTemplate = new ClassStat(446, 1234567890, "NameTemplate", false, false, false);

        private readonly ClassStat desiredTargetDistance = new ClassStat(
            447, 1234567890, "DesiredTargetDistance", false, false, false);

        private readonly ClassStat vicinityRange = new ClassStat(448, 1234567890, "VicinityRange", false, false, false);

        private readonly ClassStat npcIsSurrendering = new ClassStat(
            449, 1234567890, "NPCIsSurrendering", false, false, false);

        private readonly ClassStat stateMachine = new ClassStat(450, 1234567890, "StateMachine", false, false, false);

        private readonly ClassStat npcSurrenderInstance = new ClassStat(
            451, 1234567890, "NPCSurrenderInstance", false, false, false);

        private readonly ClassStat npcHasPatrolList = new ClassStat(
            452, 1234567890, "NPCHasPatrolList", false, false, false);

        private readonly ClassStat npcVicinityChars = new ClassStat(
            453, 1234567890, "NPCVicinityChars", false, false, false);

        private readonly ClassStat proximityRangeOutdoors = new ClassStat(
            454, 1234567890, "ProximityRangeOutdoors", false, false, false);

        private readonly ClassStat npcFamily = new ClassStat(455, 1234567890, "NPCFamily", false, false, false);

        private readonly ClassStat commandRange = new ClassStat(456, 1234567890, "CommandRange", false, false, false);

        private readonly ClassStat npcHatelistSize = new ClassStat(
            457, 1234567890, "NPCHatelistSize", false, false, false);

        private readonly ClassStat npcNumPets = new ClassStat(458, 1234567890, "NPCNumPets", false, false, false);

        private readonly ClassStat odMinSizeAdd = new ClassStat(459, 1234567890, "ODMinSizeAdd", false, false, false);

        private readonly ClassStat effectRed = new ClassStat(460, 1234567890, "EffectRed", false, false, false);

        private readonly ClassStat effectGreen = new ClassStat(461, 1234567890, "EffectGreen", false, false, false);

        private readonly ClassStat effectBlue = new ClassStat(462, 1234567890, "EffectBlue", false, false, false);

        private readonly ClassStat odMaxSizeAdd = new ClassStat(463, 1234567890, "ODMaxSizeAdd", false, false, false);

        private readonly ClassStat durationModifier = new ClassStat(
            464, 1234567890, "DurationModifier", false, false, false);

        private readonly ClassStat npcCryForHelpRange = new ClassStat(
            465, 1234567890, "NPCCryForHelpRange", false, false, false);

        private readonly ClassStat losHeight = new ClassStat(466, 1234567890, "LOSHeight", false, false, false);

        private readonly ClassStat petReq1 = new ClassStat(467, 1234567890, "PetReq1", false, false, false);

        private readonly ClassStat petReq2 = new ClassStat(468, 1234567890, "PetReq2", false, false, false);

        private readonly ClassStat petReq3 = new ClassStat(469, 1234567890, "PetReq3", false, false, false);

        private readonly ClassStat mapOptions = new ClassStat(470, 0, "MapOptions", false, false, false);

        private readonly ClassStat mapAreaPart1 = new ClassStat(471, 0, "MapAreaPart1", false, false, false);

        private readonly ClassStat mapAreaPart2 = new ClassStat(472, 0, "MapAreaPart2", false, false, false);

        private readonly ClassStat fixtureFlags = new ClassStat(473, 1234567890, "FixtureFlags", false, false, false);

        private readonly ClassStat fallDamage = new ClassStat(474, 1234567890, "FallDamage", false, false, false);

        private readonly ClassStat reflectReturnedProjectileAC = new ClassStat(
            475, 0, "ReflectReturnedProjectileAC", false, false, false);

        private readonly ClassStat reflectReturnedMeleeAC = new ClassStat(
            476, 0, "ReflectReturnedMeleeAC", false, false, false);

        private readonly ClassStat reflectReturnedEnergyAC = new ClassStat(
            477, 0, "ReflectReturnedEnergyAC", false, false, false);

        private readonly ClassStat reflectReturnedChemicalAC = new ClassStat(
            478, 0, "ReflectReturnedChemicalAC", false, false, false);

        private readonly ClassStat reflectReturnedRadiationAC = new ClassStat(
            479, 0, "ReflectReturnedRadiationAC", false, false, false);

        private readonly ClassStat reflectReturnedColdAC = new ClassStat(
            480, 0, "ReflectReturnedColdAC", false, false, false);

        private readonly ClassStat reflectReturnedNanoAC = new ClassStat(
            481, 0, "ReflectReturnedNanoAC", false, false, false);

        private readonly ClassStat reflectReturnedFireAC = new ClassStat(
            482, 0, "ReflectReturnedFireAC", false, false, false);

        private readonly ClassStat reflectReturnedPoisonAC = new ClassStat(
            483, 0, "ReflectReturnedPoisonAC", false, false, false);

        private readonly ClassStat proximityRangeIndoors = new ClassStat(
            484, 1234567890, "ProximityRangeIndoors", false, false, false);

        private readonly ClassStat petReqVal1 = new ClassStat(485, 1234567890, "PetReqVal1", false, false, false);

        private readonly ClassStat petReqVal2 = new ClassStat(486, 1234567890, "PetReqVal2", false, false, false);

        private readonly ClassStat petReqVal3 = new ClassStat(487, 1234567890, "PetReqVal3", false, false, false);

        private readonly ClassStat targetFacing = new ClassStat(488, 1234567890, "TargetFacing", false, false, false);

        private readonly ClassStat backstab = new ClassStat(489, 1234567890, "Backstab", true, false, false);

        private readonly ClassStat originatorType = new ClassStat(
            490, 1234567890, "OriginatorType", false, false, false);

        private readonly ClassStat questInstance = new ClassStat(491, 1234567890, "QuestInstance", false, false, false);

        private readonly ClassStat questIndex1 = new ClassStat(492, 1234567890, "QuestIndex1", false, false, false);

        private readonly ClassStat questIndex2 = new ClassStat(493, 1234567890, "QuestIndex2", false, false, false);

        private readonly ClassStat questIndex3 = new ClassStat(494, 1234567890, "QuestIndex3", false, false, false);

        private readonly ClassStat questIndex4 = new ClassStat(495, 1234567890, "QuestIndex4", false, false, false);

        private readonly ClassStat questIndex5 = new ClassStat(496, 1234567890, "QuestIndex5", false, false, false);

        private readonly ClassStat qtDungeonInstance = new ClassStat(
            497, 1234567890, "QTDungeonInstance", false, false, false);

        private readonly ClassStat qtNumMonsters = new ClassStat(498, 1234567890, "QTNumMonsters", false, false, false);

        private readonly ClassStat qtKilledMonsters = new ClassStat(
            499, 1234567890, "QTKilledMonsters", false, false, false);

        private readonly ClassStat animPos = new ClassStat(500, 1234567890, "AnimPos", false, false, false);

        private readonly ClassStat animPlay = new ClassStat(501, 1234567890, "AnimPlay", false, false, false);

        private readonly ClassStat animSpeed = new ClassStat(502, 1234567890, "AnimSpeed", false, false, false);

        private readonly ClassStat qtKillNumMonsterID1 = new ClassStat(
            503, 1234567890, "QTKillNumMonsterID1", false, false, false);

        private readonly ClassStat qtKillNumMonsterCount1 = new ClassStat(
            504, 1234567890, "QTKillNumMonsterCount1", false, false, false);

        private readonly ClassStat qtKillNumMonsterID2 = new ClassStat(
            505, 1234567890, "QTKillNumMonsterID2", false, false, false);

        private readonly ClassStat qtKillNumMonsterCount2 = new ClassStat(
            506, 1234567890, "QTKillNumMonsterCount2", false, false, false);

        private readonly ClassStat qtKillNumMonsterID3 = new ClassStat(
            507, 1234567890, "QTKillNumMonsterID3", false, false, false);

        private readonly ClassStat qtKillNumMonsterCount3 = new ClassStat(
            508, 1234567890, "QTKillNumMonsterCount3", false, false, false);

        private readonly ClassStat questIndex0 = new ClassStat(509, 1234567890, "QuestIndex0", false, false, false);

        private readonly ClassStat questTimeout = new ClassStat(510, 1234567890, "QuestTimeout", false, false, false);

        private readonly ClassStat towerNpcHash = new ClassStat(511, 1234567890, "Tower_NPCHash", false, false, false);

        private readonly ClassStat petType = new ClassStat(512, 1234567890, "PetType", false, false, false);

        private readonly ClassStat onTowerCreation = new ClassStat(
            513, 1234567890, "OnTowerCreation", false, false, false);

        private readonly ClassStat ownedTowers = new ClassStat(514, 1234567890, "OwnedTowers", false, false, false);

        private readonly ClassStat towerInstance = new ClassStat(515, 1234567890, "TowerInstance", false, false, false);

        private readonly ClassStat attackShield = new ClassStat(516, 1234567890, "AttackShield", false, false, false);

        private readonly ClassStat specialAttackShield = new ClassStat(
            517, 1234567890, "SpecialAttackShield", false, false, false);

        private readonly ClassStat npcVicinityPlayers = new ClassStat(
            518, 1234567890, "NPCVicinityPlayers", false, false, false);

        private readonly ClassStat npcUseFightModeRegenRate = new ClassStat(
            519, 1234567890, "NPCUseFightModeRegenRate", false, false, false);

        private readonly ClassStat rnd = new ClassStat(520, 1234567890, "Rnd", false, false, false);

        private readonly ClassStat socialStatus = new ClassStat(521, 0, "SocialStatus", false, false, false);

        private readonly ClassStat lastRnd = new ClassStat(522, 1234567890, "LastRnd", false, false, false);

        private readonly ClassStat itemDelayCap = new ClassStat(523, 1234567890, "ItemDelayCap", false, false, false);

        private readonly ClassStat rechargeDelayCap = new ClassStat(
            524, 1234567890, "RechargeDelayCap", false, false, false);

        private readonly ClassStat percentRemainingHealth = new ClassStat(
            525, 1234567890, "PercentRemainingHealth", false, false, false);

        private readonly ClassStat percentRemainingNano = new ClassStat(
            526, 1234567890, "PercentRemainingNano", false, false, false);

        private readonly ClassStat targetDistance = new ClassStat(
            527, 1234567890, "TargetDistance", false, false, false);

        private readonly ClassStat teamCloseness = new ClassStat(528, 1234567890, "TeamCloseness", false, false, false);

        private readonly ClassStat numberOnHateList = new ClassStat(
            529, 1234567890, "NumberOnHateList", false, false, false);

        private readonly ClassStat conditionState = new ClassStat(
            530, 1234567890, "ConditionState", false, false, false);

        private readonly ClassStat expansionPlayfield = new ClassStat(
            531, 1234567890, "ExpansionPlayfield", false, false, false);

        private readonly ClassStat shadowBreed = new ClassStat(532, 0, "ShadowBreed", false, false, false);

        private readonly ClassStat npcFovStatus = new ClassStat(533, 1234567890, "NPCFovStatus", false, false, false);

        private readonly ClassStat dudChance = new ClassStat(534, 1234567890, "DudChance", false, false, false);

        private readonly ClassStat healMultiplier = new ClassStat(
            535, 1234567890, "HealMultiplier", false, false, false);

        private readonly ClassStat nanoDamageMultiplier = new ClassStat(
            536, 0, "NanoDamageMultiplier", false, false, false);

        private readonly ClassStat nanoVulnerability = new ClassStat(
            537, 1234567890, "NanoVulnerability", false, false, false);

        private readonly ClassStat amsCap = new ClassStat(538, 1234567890, "AmsCap", false, false, false);

        private readonly ClassStat procInitiative1 = new ClassStat(
            539, 1234567890, "ProcInitiative1", false, false, false);

        private readonly ClassStat procInitiative2 = new ClassStat(
            540, 1234567890, "ProcInitiative2", false, false, false);

        private readonly ClassStat procInitiative3 = new ClassStat(
            541, 1234567890, "ProcInitiative3", false, false, false);

        private readonly ClassStat procInitiative4 = new ClassStat(
            542, 1234567890, "ProcInitiative4", false, false, false);

        private readonly ClassStat factionModifier = new ClassStat(
            543, 1234567890, "FactionModifier", false, false, false);

        private readonly ClassStat missionBits8 = new ClassStat(544, 0, "MissionBits8", false, false, false);

        private readonly ClassStat missionBits9 = new ClassStat(545, 0, "MissionBits9", false, false, false);

        private readonly ClassStat stackingLine2 = new ClassStat(546, 1234567890, "StackingLine2", false, false, false);

        private readonly ClassStat stackingLine3 = new ClassStat(547, 1234567890, "StackingLine3", false, false, false);

        private readonly ClassStat stackingLine4 = new ClassStat(548, 1234567890, "StackingLine4", false, false, false);

        private readonly ClassStat stackingLine5 = new ClassStat(549, 1234567890, "StackingLine5", false, false, false);

        private readonly ClassStat stackingLine6 = new ClassStat(550, 1234567890, "StackingLine6", false, false, false);

        private readonly ClassStat stackingOrder = new ClassStat(551, 1234567890, "StackingOrder", false, false, false);

        private readonly ClassStat procNano1 = new ClassStat(552, 1234567890, "ProcNano1", false, false, false);

        private readonly ClassStat procNano2 = new ClassStat(553, 1234567890, "ProcNano2", false, false, false);

        private readonly ClassStat procNano3 = new ClassStat(554, 1234567890, "ProcNano3", false, false, false);

        private readonly ClassStat procNano4 = new ClassStat(555, 1234567890, "ProcNano4", false, false, false);

        private readonly ClassStat procChance1 = new ClassStat(556, 1234567890, "ProcChance1", false, false, false);

        private readonly ClassStat procChance2 = new ClassStat(557, 1234567890, "ProcChance2", false, false, false);

        private readonly ClassStat procChance3 = new ClassStat(558, 1234567890, "ProcChance3", false, false, false);

        private readonly ClassStat procChance4 = new ClassStat(559, 1234567890, "ProcChance4", false, false, false);

        private readonly ClassStat otArmedForces = new ClassStat(560, 0, "OTArmedForces", false, false, false);

        private readonly ClassStat clanSentinels = new ClassStat(561, 0, "ClanSentinels", false, false, false);

        private readonly ClassStat otMed = new ClassStat(562, 1234567890, "OTMed", false, false, false);

        private readonly ClassStat clanGaia = new ClassStat(563, 0, "ClanGaia", false, false, false);

        private readonly ClassStat otTrans = new ClassStat(564, 0, "OTTrans", false, false, false);

        private readonly ClassStat clanVanguards = new ClassStat(565, 0, "ClanVanguards", false, false, false);

        private readonly ClassStat gos = new ClassStat(566, 0, "GOS", false, false, false);

        private readonly ClassStat otFollowers = new ClassStat(567, 0, "OTFollowers", false, false, false);

        private readonly ClassStat otOperator = new ClassStat(568, 0, "OTOperator", false, false, false);

        private readonly ClassStat otUnredeemed = new ClassStat(569, 0, "OTUnredeemed", false, false, false);

        private readonly ClassStat clanDevoted = new ClassStat(570, 0, "ClanDevoted", false, false, false);

        private readonly ClassStat clanConserver = new ClassStat(571, 0, "ClanConserver", false, false, false);

        private readonly ClassStat clanRedeemed = new ClassStat(572, 0, "ClanRedeemed", false, false, false);

        private readonly ClassStat sk = new ClassStat(573, 0, "SK", false, false, false);

        private readonly ClassStat lastSK = new ClassStat(574, 0, "LastSK", false, false, false);

        private readonly StatNextSK nextSK = new StatNextSK(575, 0, "NextSK", false, false, false);

        private readonly ClassStat playerOptions = new ClassStat(576, 0, "PlayerOptions", false, false, false);

        private readonly ClassStat lastPerkResetTime = new ClassStat(577, 0, "LastPerkResetTime", false, false, false);

        private readonly ClassStat currentTime = new ClassStat(578, 1234567890, "CurrentTime", false, false, false);

        private readonly ClassStat shadowBreedTemplate = new ClassStat(
            579, 0, "ShadowBreedTemplate", false, false, false);

        private readonly ClassStat npcVicinityFamily = new ClassStat(
            580, 1234567890, "NPCVicinityFamily", false, false, false);

        private readonly ClassStat npcScriptAmsScale = new ClassStat(
            581, 1234567890, "NPCScriptAMSScale", false, false, false);

        private readonly ClassStat apartmentsAllowed = new ClassStat(582, 1, "ApartmentsAllowed", false, false, false);

        private readonly ClassStat apartmentsOwned = new ClassStat(583, 0, "ApartmentsOwned", false, false, false);

        private readonly ClassStat apartmentAccessCard = new ClassStat(
            584, 1234567890, "ApartmentAccessCard", false, false, false);

        private readonly ClassStat mapAreaPart3 = new ClassStat(585, 0, "MapAreaPart3", false, false, false);

        private readonly ClassStat mapAreaPart4 = new ClassStat(586, 0, "MapAreaPart4", false, false, false);

        private readonly ClassStat numberOfTeamMembers = new ClassStat(
            587, 1234567890, "NumberOfTeamMembers", false, false, false);

        private readonly ClassStat actionCategory = new ClassStat(
            588, 1234567890, "ActionCategory", false, false, false);

        private readonly ClassStat currentPlayfield = new ClassStat(
            589, 1234567890, "CurrentPlayfield", false, false, false);

        private readonly ClassStat districtNano = new ClassStat(590, 1234567890, "DistrictNano", false, false, false);

        private readonly ClassStat districtNanoInterval = new ClassStat(
            591, 1234567890, "DistrictNanoInterval", false, false, false);

        private readonly ClassStat unsavedXP = new ClassStat(592, 0, "UnsavedXP", false, false, false);

        private readonly ClassStat regainXPPercentage = new ClassStat(593, 0, "RegainXPPercentage", false, false, false);

        private readonly ClassStat tempSaveTeamID = new ClassStat(594, 0, "TempSaveTeamID", false, false, false);

        private readonly ClassStat tempSavePlayfield = new ClassStat(595, 0, "TempSavePlayfield", false, false, false);

        private readonly ClassStat tempSaveX = new ClassStat(596, 0, "TempSaveX", false, false, false);

        private readonly ClassStat tempSaveY = new ClassStat(597, 0, "TempSaveY", false, false, false);

        private readonly ClassStat extendedFlags = new ClassStat(598, 1234567890, "ExtendedFlags", false, false, false);

        private readonly ClassStat shopPrice = new ClassStat(599, 1234567890, "ShopPrice", false, false, false);

        private readonly ClassStat newbieHP = new ClassStat(600, 1234567890, "NewbieHP", false, false, false);

        private readonly ClassStat hpLevelUp = new ClassStat(601, 1234567890, "HPLevelUp", false, false, false);

        private readonly ClassStat hpPerSkill = new ClassStat(602, 1234567890, "HPPerSkill", false, false, false);

        private readonly ClassStat newbieNP = new ClassStat(603, 1234567890, "NewbieNP", false, false, false);

        private readonly ClassStat npLevelUp = new ClassStat(604, 1234567890, "NPLevelUp", false, false, false);

        private readonly ClassStat npPerSkill = new ClassStat(605, 1234567890, "NPPerSkill", false, false, false);

        private readonly ClassStat maxShopItems = new ClassStat(606, 1234567890, "MaxShopItems", false, false, false);

        private readonly ClassStat playerID = new ClassStat(607, 1234567890, "PlayerID", false, true, false);

        private readonly ClassStat shopRent = new ClassStat(608, 1234567890, "ShopRent", false, false, false);

        private readonly ClassStat synergyHash = new ClassStat(609, 1234567890, "SynergyHash", false, false, false);

        private readonly ClassStat shopFlags = new ClassStat(610, 1234567890, "ShopFlags", false, false, false);

        private readonly ClassStat shopLastUsed = new ClassStat(611, 1234567890, "ShopLastUsed", false, false, false);

        private readonly ClassStat shopType = new ClassStat(612, 1234567890, "ShopType", false, false, false);

        private readonly ClassStat lockDownTime = new ClassStat(613, 1234567890, "LockDownTime", false, false, false);

        private readonly ClassStat leaderLockDownTime = new ClassStat(
            614, 1234567890, "LeaderLockDownTime", false, false, false);

        private readonly ClassStat invadersKilled = new ClassStat(615, 0, "InvadersKilled", false, false, false);

        private readonly ClassStat killedByInvaders = new ClassStat(616, 0, "KilledByInvaders", false, false, false);

        private readonly ClassStat missionBits10 = new ClassStat(617, 0, "MissionBits10", false, false, false);

        private readonly ClassStat missionBits11 = new ClassStat(618, 0, "MissionBits11", false, false, false);

        private readonly ClassStat missionBits12 = new ClassStat(619, 0, "MissionBits12", false, false, false);

        private readonly ClassStat houseTemplate = new ClassStat(620, 1234567890, "HouseTemplate", false, false, false);

        private readonly ClassStat percentFireDamage = new ClassStat(
            621, 1234567890, "PercentFireDamage", false, false, false);

        private readonly ClassStat percentColdDamage = new ClassStat(
            622, 1234567890, "PercentColdDamage", false, false, false);

        private readonly ClassStat percentMeleeDamage = new ClassStat(
            623, 1234567890, "PercentMeleeDamage", false, false, false);

        private readonly ClassStat percentProjectileDamage = new ClassStat(
            624, 1234567890, "PercentProjectileDamage", false, false, false);

        private readonly ClassStat percentPoisonDamage = new ClassStat(
            625, 1234567890, "PercentPoisonDamage", false, false, false);

        private readonly ClassStat percentRadiationDamage = new ClassStat(
            626, 1234567890, "PercentRadiationDamage", false, false, false);

        private readonly ClassStat percentEnergyDamage = new ClassStat(
            627, 1234567890, "PercentEnergyDamage", false, false, false);

        private readonly ClassStat percentChemicalDamage = new ClassStat(
            628, 1234567890, "PercentChemicalDamage", false, false, false);

        private readonly ClassStat totalDamage = new ClassStat(629, 1234567890, "TotalDamage", false, false, false);

        private readonly ClassStat trackProjectileDamage = new ClassStat(
            630, 1234567890, "TrackProjectileDamage", false, false, false);

        private readonly ClassStat trackMeleeDamage = new ClassStat(
            631, 1234567890, "TrackMeleeDamage", false, false, false);

        private readonly ClassStat trackEnergyDamage = new ClassStat(
            632, 1234567890, "TrackEnergyDamage", false, false, false);

        private readonly ClassStat trackChemicalDamage = new ClassStat(
            633, 1234567890, "TrackChemicalDamage", false, false, false);

        private readonly ClassStat trackRadiationDamage = new ClassStat(
            634, 1234567890, "TrackRadiationDamage", false, false, false);

        private readonly ClassStat trackColdDamage = new ClassStat(
            635, 1234567890, "TrackColdDamage", false, false, false);

        private readonly ClassStat trackPoisonDamage = new ClassStat(
            636, 1234567890, "TrackPoisonDamage", false, false, false);

        private readonly ClassStat trackFireDamage = new ClassStat(
            637, 1234567890, "TrackFireDamage", false, false, false);

        private readonly ClassStat npcSpellArg1 = new ClassStat(638, 1234567890, "NPCSpellArg1", false, false, false);

        private readonly ClassStat npcSpellRet1 = new ClassStat(639, 1234567890, "NPCSpellRet1", false, false, false);

        private readonly ClassStat cityInstance = new ClassStat(640, 1234567890, "CityInstance", false, false, false);

        private readonly ClassStat distanceToSpawnpoint = new ClassStat(
            641, 1234567890, "DistanceToSpawnpoint", false, false, false);

        private readonly ClassStat cityTerminalRechargePercent = new ClassStat(
            642, 1234567890, "CityTerminalRechargePercent", false, false, false);

        private readonly ClassStat unreadMailCount = new ClassStat(649, 0, "UnreadMailCount", false, false, false);

        private readonly ClassStat lastMailCheckTime = new ClassStat(
            650, 1283065897, "LastMailCheckTime", false, false, false);

        private readonly ClassStat advantageHash1 = new ClassStat(
            651, 1234567890, "AdvantageHash1", false, false, false);

        private readonly ClassStat advantageHash2 = new ClassStat(
            652, 1234567890, "AdvantageHash2", false, false, false);

        private readonly ClassStat advantageHash3 = new ClassStat(
            653, 1234567890, "AdvantageHash3", false, false, false);

        private readonly ClassStat advantageHash4 = new ClassStat(
            654, 1234567890, "AdvantageHash4", false, false, false);

        private readonly ClassStat advantageHash5 = new ClassStat(
            655, 1234567890, "AdvantageHash5", false, false, false);

        private readonly ClassStat shopIndex = new ClassStat(656, 1234567890, "ShopIndex", false, false, false);

        private readonly ClassStat shopID = new ClassStat(657, 1234567890, "ShopID", false, false, false);

        private readonly ClassStat isVehicle = new ClassStat(658, 1234567890, "IsVehicle", false, false, false);

        private readonly ClassStat damageToNano = new ClassStat(659, 1234567890, "DamageToNano", false, false, false);

        private readonly ClassStat accountFlags = new ClassStat(660, 1234567890, "AccountFlags", false, true, false);

        private readonly ClassStat damageToNanoMultiplier = new ClassStat(
            661, 1234567890, "DamageToNanoMultiplier", false, false, false);

        private readonly ClassStat mechData = new ClassStat(662, 0, "MechData", false, false, false);

        private readonly ClassStat vehicleAC = new ClassStat(664, 1234567890, "VehicleAC", false, false, false);

        private readonly ClassStat vehicleDamage = new ClassStat(665, 1234567890, "VehicleDamage", false, false, false);

        private readonly ClassStat vehicleHealth = new ClassStat(666, 1234567890, "VehicleHealth", false, false, false);

        private readonly ClassStat vehicleSpeed = new ClassStat(667, 1234567890, "VehicleSpeed", false, false, false);

        private readonly ClassStat battlestationSide = new ClassStat(668, 0, "BattlestationSide", false, false, false);

        private readonly ClassStat victoryPoints = new ClassStat(669, 0, "VP", false, false, false);

        private readonly ClassStat battlestationRep = new ClassStat(670, 10, "BattlestationRep", false, false, false);

        private readonly ClassStat petState = new ClassStat(671, 1234567890, "PetState", false, false, false);

        private readonly ClassStat paidPoints = new ClassStat(672, 0, "PaidPoints", false, false, false);

        private readonly ClassStat visualFlags = new ClassStat(673, 31, "VisualFlags", false, false, false);

        private readonly ClassStat pvpDuelKills = new ClassStat(674, 0, "PVPDuelKills", false, false, false);

        private readonly ClassStat pvpDuelDeaths = new ClassStat(675, 0, "PVPDuelDeaths", false, false, false);

        private readonly ClassStat pvpProfessionDuelKills = new ClassStat(
            676, 0, "PVPProfessionDuelKills", false, false, false);

        private readonly ClassStat pvpProfessionDuelDeaths = new ClassStat(
            677, 0, "PVPProfessionDuelDeaths", false, false, false);

        private readonly ClassStat pvpRankedSoloKills = new ClassStat(678, 0, "PVPRankedSoloKills", false, false, false);

        private readonly ClassStat pvpRankedSoloDeaths = new ClassStat(
            679, 0, "PVPRankedSoloDeaths", false, false, false);

        private readonly ClassStat pvpRankedTeamKills = new ClassStat(680, 0, "PVPRankedTeamKills", false, false, false);

        private readonly ClassStat pvpRankedTeamDeaths = new ClassStat(
            681, 0, "PVPRankedTeamDeaths", false, false, false);

        private readonly ClassStat pvpSoloScore = new ClassStat(682, 0, "PVPSoloScore", false, false, false);

        private readonly ClassStat pvpTeamScore = new ClassStat(683, 0, "PVPTeamScore", false, false, false);

        private readonly ClassStat pvpDuelScore = new ClassStat(684, 0, "PVPDuelScore", false, false, false);

        private readonly ClassStat acgItemSeed = new ClassStat(700, 1234567890, "ACGItemSeed", false, false, false);

        private readonly ClassStat acgItemLevel = new ClassStat(701, 1234567890, "ACGItemLevel", false, false, false);

        private readonly ClassStat acgItemTemplateID = new ClassStat(
            702, 1234567890, "ACGItemTemplateID", false, false, false);

        private readonly ClassStat acgItemTemplateID2 = new ClassStat(
            703, 1234567890, "ACGItemTemplateID2", false, false, false);

        private readonly ClassStat acgItemCategoryID = new ClassStat(
            704, 1234567890, "ACGItemCategoryID", false, false, false);

        private readonly ClassStat hasKnuBotData = new ClassStat(768, 1234567890, "HasKnuBotData", false, false, false);

        private readonly ClassStat questBoothDifficulty = new ClassStat(
            800, 1234567890, "QuestBoothDifficulty", false, false, false);

        private readonly ClassStat questAsMinimumRange = new ClassStat(
            801, 1234567890, "QuestASMinimumRange", false, false, false);

        private readonly ClassStat questAsMaximumRange = new ClassStat(
            802, 1234567890, "QuestASMaximumRange", false, false, false);

        private readonly ClassStat visualLodLevel = new ClassStat(
            888, 1234567890, "VisualLODLevel", false, false, false);

        private readonly ClassStat targetDistanceChange = new ClassStat(
            889, 1234567890, "TargetDistanceChange", false, false, false);

        private readonly ClassStat tideRequiredDynelID = new ClassStat(
            900, 1234567890, "TideRequiredDynelID", false, false, false);

        private readonly ClassStat streamCheckMagic = new ClassStat(
            999, 1234567890, "StreamCheckMagic", false, false, false);

        private readonly ClassStat type = new ClassStat(1001, 1234567890, "Type", false, true, false);

        private readonly ClassStat instance = new ClassStat(1002, 1234567890, "Instance", false, true, false);

        private readonly ClassStat weaponsStyle = new ClassStat(1003, 1234567890, "WeaponType", false, false, false);

        private readonly ClassStat shoulderMeshRight = new ClassStat(1004, 0, "ShoulderMeshRight", false, false, false);

        private readonly ClassStat shoulderMeshLeft = new ClassStat(1005, 0, "ShoulderMeshLeft", false, false, false);

        private readonly ClassStat weaponMeshRight = new ClassStat(1006, 0, "WeaponMeshRight", false, false, false);

        private readonly ClassStat weaponMeshLeft = new ClassStat(1007, 0, "WeaponMeshLeft", false, false, false);

        private readonly ClassStat overrideTextureHead = new ClassStat(
            1008, 0, "OverrideTextureHead", false, false, false);

        private readonly ClassStat overrideTextureWeaponRight = new ClassStat(
            1009, 0, "OverrideTextureWeaponRight", false, false, false);

        private readonly ClassStat overrideTextureWeaponLeft = new ClassStat(
            1010, 0, "OverrideTextureWeaponLeft", false, false, false);

        private readonly ClassStat overrideTextureShoulderpadRight = new ClassStat(
            1011, 0, "OverrideTextureShoulderpadRight", false, false, false);

        private readonly ClassStat overrideTextureShoulderpadLeft = new ClassStat(
            1012, 0, "OverrideTextureShoulderpadLeft", false, false, false);

        private readonly ClassStat overrideTextureBack = new ClassStat(
            1013, 0, "OverrideTextureBack", false, false, false);

        private readonly ClassStat overrideTextureAttractor = new ClassStat(
            1014, 0, "OverrideTextureAttractor", false, false, false);

        private readonly ClassStat weaponStyleLeft = new ClassStat(1015, 0, "WeaponStyleLeft", false, false, false);

        private readonly ClassStat weaponStyleRight = new ClassStat(1016, 0, "WeaponStyleRight", false, false, false);
        #endregion

        public List<ClassStat> All = new List<ClassStat>();

        #region Create Stats
        /// <summary>
        /// Character_Stats
        /// Class for character's stats
        /// </summary>
        /// <param name="parent">Stat's owner (Character or derived class)</param>
        public CharacterStats(Character parent)
        {
            #region Add stats to list
            this.All.Add(this.flags);
            this.All.Add(this.life);
            this.All.Add(this.volumeMass);
            this.All.Add(this.attackSpeed);
            this.All.Add(this.breed);
            this.All.Add(this.clan);
            this.All.Add(this.team);
            this.All.Add(this.state);
            this.All.Add(this.timeExist);
            this.All.Add(this.mapFlags);
            this.All.Add(this.professionLevel);
            this.All.Add(this.previousHealth);
            this.All.Add(this.mesh);
            this.All.Add(this.anim);
            this.All.Add(this.name);
            this.All.Add(this.info);
            this.All.Add(this.strength);
            this.All.Add(this.agility);
            this.All.Add(this.stamina);
            this.All.Add(this.intelligence);
            this.All.Add(this.sense);
            this.All.Add(this.psychic);
            this.All.Add(this.ams);
            this.All.Add(this.staticInstance);
            this.All.Add(this.maxMass);
            this.All.Add(this.staticType);
            this.All.Add(this.energy);
            this.All.Add(this.health);
            this.All.Add(this.height);
            this.All.Add(this.dms);
            this.All.Add(this.can);
            this.All.Add(this.face);
            this.All.Add(this.hairMesh);
            this.All.Add(this.side);
            this.All.Add(this.deadTimer);
            this.All.Add(this.accessCount);
            this.All.Add(this.attackCount);
            this.All.Add(this.titleLevel);
            this.All.Add(this.backMesh);
            this.All.Add(this.alienXP);
            this.All.Add(this.fabricType);
            this.All.Add(this.catMesh);
            this.All.Add(this.parentType);
            this.All.Add(this.parentInstance);
            this.All.Add(this.beltSlots);
            this.All.Add(this.bandolierSlots);
            this.All.Add(this.fatness);
            this.All.Add(this.clanLevel);
            this.All.Add(this.insuranceTime);
            this.All.Add(this.inventoryTimeout);
            this.All.Add(this.aggDef);
            this.All.Add(this.xp);
            this.All.Add(this.ip);
            this.All.Add(this.level);
            this.All.Add(this.inventoryId);
            this.All.Add(this.timeSinceCreation);
            this.All.Add(this.lastXP);
            this.All.Add(this.age);
            this.All.Add(this.sex);
            this.All.Add(this.profession);
            this.All.Add(this.cash);
            this.All.Add(this.alignment);
            this.All.Add(this.attitude);
            this.All.Add(this.headMesh);
            this.All.Add(this.missionBits5);
            this.All.Add(this.missionBits6);
            this.All.Add(this.missionBits7);
            this.All.Add(this.veteranPoints);
            this.All.Add(this.monthsPaid);
            this.All.Add(this.speedPenalty);
            this.All.Add(this.totalMass);
            this.All.Add(this.itemType);
            this.All.Add(this.repairDifficulty);
            this.All.Add(this.price);
            this.All.Add(this.metaType);
            this.All.Add(this.itemClass);
            this.All.Add(this.repairSkill);
            this.All.Add(this.currentMass);
            this.All.Add(this.icon);
            this.All.Add(this.primaryItemType);
            this.All.Add(this.primaryItemInstance);
            this.All.Add(this.secondaryItemType);
            this.All.Add(this.secondaryItemInstance);
            this.All.Add(this.userType);
            this.All.Add(this.userInstance);
            this.All.Add(this.areaType);
            this.All.Add(this.areaInstance);
            this.All.Add(this.defaultPos);
            this.All.Add(this.race);
            this.All.Add(this.projectileAC);
            this.All.Add(this.meleeAC);
            this.All.Add(this.energyAC);
            this.All.Add(this.chemicalAC);
            this.All.Add(this.radiationAC);
            this.All.Add(this.coldAC);
            this.All.Add(this.poisonAC);
            this.All.Add(this.fireAC);
            this.All.Add(this.stateAction);
            this.All.Add(this.itemAnim);
            this.All.Add(this.martialArts);
            this.All.Add(this.meleeMultiple);
            this.All.Add(this.onehBluntWeapons);
            this.All.Add(this.onehEdgedWeapon);
            this.All.Add(this.meleeEnergyWeapon);
            this.All.Add(this.twohEdgedWeapons);
            this.All.Add(this.piercing);
            this.All.Add(this.twohBluntWeapons);
            this.All.Add(this.throwingKnife);
            this.All.Add(this.grenade);
            this.All.Add(this.thrownGrapplingWeapons);
            this.All.Add(this.bow);
            this.All.Add(this.pistol);
            this.All.Add(this.rifle);
            this.All.Add(this.subMachineGun);
            this.All.Add(this.shotgun);
            this.All.Add(this.assaultRifle);
            this.All.Add(this.driveWater);
            this.All.Add(this.closeCombatInitiative);
            this.All.Add(this.distanceWeaponInitiative);
            this.All.Add(this.physicalProwessInitiative);
            this.All.Add(this.bowSpecialAttack);
            this.All.Add(this.senseImprovement);
            this.All.Add(this.firstAid);
            this.All.Add(this.treatment);
            this.All.Add(this.mechanicalEngineering);
            this.All.Add(this.electricalEngineering);
            this.All.Add(this.materialMetamorphose);
            this.All.Add(this.biologicalMetamorphose);
            this.All.Add(this.psychologicalModification);
            this.All.Add(this.materialCreation);
            this.All.Add(this.materialLocation);
            this.All.Add(this.nanoEnergyPool);
            this.All.Add(this.lrEnergyWeapon);
            this.All.Add(this.lrMultipleWeapon);
            this.All.Add(this.disarmTrap);
            this.All.Add(this.perception);
            this.All.Add(this.adventuring);
            this.All.Add(this.swim);
            this.All.Add(this.driveAir);
            this.All.Add(this.mapNavigation);
            this.All.Add(this.tutoring);
            this.All.Add(this.brawl);
            this.All.Add(this.riposte);
            this.All.Add(this.dimach);
            this.All.Add(this.parry);
            this.All.Add(this.sneakAttack);
            this.All.Add(this.fastAttack);
            this.All.Add(this.burst);
            this.All.Add(this.nanoProwessInitiative);
            this.All.Add(this.flingShot);
            this.All.Add(this.aimedShot);
            this.All.Add(this.bodyDevelopment);
            this.All.Add(this.duck);
            this.All.Add(this.dodge);
            this.All.Add(this.evade);
            this.All.Add(this.runSpeed);
            this.All.Add(this.fieldQuantumPhysics);
            this.All.Add(this.weaponSmithing);
            this.All.Add(this.pharmaceuticals);
            this.All.Add(this.nanoProgramming);
            this.All.Add(this.computerLiteracy);
            this.All.Add(this.psychology);
            this.All.Add(this.chemistry);
            this.All.Add(this.concealment);
            this.All.Add(this.breakingEntry);
            this.All.Add(this.driveGround);
            this.All.Add(this.fullAuto);
            this.All.Add(this.nanoAC);
            this.All.Add(this.alienLevel);
            this.All.Add(this.healthChangeBest);
            this.All.Add(this.healthChangeWorst);
            this.All.Add(this.healthChange);
            this.All.Add(this.currentMovementMode);
            this.All.Add(this.prevMovementMode);
            this.All.Add(this.autoLockTimeDefault);
            this.All.Add(this.autoUnlockTimeDefault);
            this.All.Add(this.moreFlags);
            this.All.Add(this.alienNextXP);
            this.All.Add(this.npcFlags);
            this.All.Add(this.currentNCU);
            this.All.Add(this.maxNCU);
            this.All.Add(this.specialization);
            this.All.Add(this.effectIcon);
            this.All.Add(this.buildingType);
            this.All.Add(this.buildingInstance);
            this.All.Add(this.cardOwnerType);
            this.All.Add(this.cardOwnerInstance);
            this.All.Add(this.buildingComplexInst);
            this.All.Add(this.exitInstance);
            this.All.Add(this.nextDoorInBuilding);
            this.All.Add(this.lastConcretePlayfieldInstance);
            this.All.Add(this.extenalPlayfieldInstance);
            this.All.Add(this.extenalDoorInstance);
            this.All.Add(this.inPlay);
            this.All.Add(this.accessKey);
            this.All.Add(this.petMaster);
            this.All.Add(this.orientationMode);
            this.All.Add(this.sessionTime);
            this.All.Add(this.rp);
            this.All.Add(this.conformity);
            this.All.Add(this.aggressiveness);
            this.All.Add(this.stability);
            this.All.Add(this.extroverty);
            this.All.Add(this.breedHostility);
            this.All.Add(this.reflectProjectileAC);
            this.All.Add(this.reflectMeleeAC);
            this.All.Add(this.reflectEnergyAC);
            this.All.Add(this.reflectChemicalAC);
            this.All.Add(this.rechargeDelay);
            this.All.Add(this.equipDelay);
            this.All.Add(this.maxEnergy);
            this.All.Add(this.teamSide);
            this.All.Add(this.currentNano);
            this.All.Add(this.gmLevel);
            this.All.Add(this.reflectRadiationAC);
            this.All.Add(this.reflectColdAC);
            this.All.Add(this.reflectNanoAC);
            this.All.Add(this.reflectFireAC);
            this.All.Add(this.currBodyLocation);
            this.All.Add(this.maxNanoEnergy);
            this.All.Add(this.accumulatedDamage);
            this.All.Add(this.canChangeClothes);
            this.All.Add(this.features);
            this.All.Add(this.reflectPoisonAC);
            this.All.Add(this.shieldProjectileAC);
            this.All.Add(this.shieldMeleeAC);
            this.All.Add(this.shieldEnergyAC);
            this.All.Add(this.shieldChemicalAC);
            this.All.Add(this.shieldRadiationAC);
            this.All.Add(this.shieldColdAC);
            this.All.Add(this.shieldNanoAC);
            this.All.Add(this.shieldFireAC);
            this.All.Add(this.shieldPoisonAC);
            this.All.Add(this.berserkMode);
            this.All.Add(this.insurancePercentage);
            this.All.Add(this.changeSideCount);
            this.All.Add(this.absorbProjectileAC);
            this.All.Add(this.absorbMeleeAC);
            this.All.Add(this.absorbEnergyAC);
            this.All.Add(this.absorbChemicalAC);
            this.All.Add(this.absorbRadiationAC);
            this.All.Add(this.absorbColdAC);
            this.All.Add(this.absorbFireAC);
            this.All.Add(this.absorbPoisonAC);
            this.All.Add(this.absorbNanoAC);
            this.All.Add(this.temporarySkillReduction);
            this.All.Add(this.birthDate);
            this.All.Add(this.lastSaved);
            this.All.Add(this.soundVolume);
            this.All.Add(this.petCounter);
            this.All.Add(this.metersWalked);
            this.All.Add(this.questLevelsSolved);
            this.All.Add(this.monsterLevelsKilled);
            this.All.Add(this.pvPLevelsKilled);
            this.All.Add(this.missionBits1);
            this.All.Add(this.missionBits2);
            this.All.Add(this.accessGrant);
            this.All.Add(this.doorFlags);
            this.All.Add(this.clanHierarchy);
            this.All.Add(this.questStat);
            this.All.Add(this.clientActivated);
            this.All.Add(this.personalResearchLevel);
            this.All.Add(this.globalResearchLevel);
            this.All.Add(this.personalResearchGoal);
            this.All.Add(this.globalResearchGoal);
            this.All.Add(this.turnSpeed);
            this.All.Add(this.liquidType);
            this.All.Add(this.gatherSound);
            this.All.Add(this.castSound);
            this.All.Add(this.travelSound);
            this.All.Add(this.hitSound);
            this.All.Add(this.secondaryItemTemplate);
            this.All.Add(this.equippedWeapons);
            this.All.Add(this.xpKillRange);
            this.All.Add(this.amsModifier);
            this.All.Add(this.dmsModifier);
            this.All.Add(this.projectileDamageModifier);
            this.All.Add(this.meleeDamageModifier);
            this.All.Add(this.energyDamageModifier);
            this.All.Add(this.chemicalDamageModifier);
            this.All.Add(this.radiationDamageModifier);
            this.All.Add(this.itemHateValue);
            this.All.Add(this.damageBonus);
            this.All.Add(this.maxDamage);
            this.All.Add(this.minDamage);
            this.All.Add(this.attackRange);
            this.All.Add(this.hateValueModifyer);
            this.All.Add(this.trapDifficulty);
            this.All.Add(this.statOne);
            this.All.Add(this.numAttackEffects);
            this.All.Add(this.defaultAttackType);
            this.All.Add(this.itemSkill);
            this.All.Add(this.itemDelay);
            this.All.Add(this.itemOpposedSkill);
            this.All.Add(this.itemSIS);
            this.All.Add(this.interactionRadius);
            this.All.Add(this.placement);
            this.All.Add(this.lockDifficulty);
            this.All.Add(this.members);
            this.All.Add(this.minMembers);
            this.All.Add(this.clanPrice);
            this.All.Add(this.missionBits3);
            this.All.Add(this.clanType);
            this.All.Add(this.clanInstance);
            this.All.Add(this.voteCount);
            this.All.Add(this.memberType);
            this.All.Add(this.memberInstance);
            this.All.Add(this.globalClanType);
            this.All.Add(this.globalClanInstance);
            this.All.Add(this.coldDamageModifier);
            this.All.Add(this.clanUpkeepInterval);
            this.All.Add(this.timeSinceUpkeep);
            this.All.Add(this.clanFinalized);
            this.All.Add(this.nanoDamageModifier);
            this.All.Add(this.fireDamageModifier);
            this.All.Add(this.poisonDamageModifier);
            this.All.Add(this.npCostModifier);
            this.All.Add(this.xpModifier);
            this.All.Add(this.breedLimit);
            this.All.Add(this.genderLimit);
            this.All.Add(this.levelLimit);
            this.All.Add(this.playerKilling);
            this.All.Add(this.teamAllowed);
            this.All.Add(this.weaponDisallowedType);
            this.All.Add(this.weaponDisallowedInstance);
            this.All.Add(this.taboo);
            this.All.Add(this.compulsion);
            this.All.Add(this.skillDisabled);
            this.All.Add(this.clanItemType);
            this.All.Add(this.clanItemInstance);
            this.All.Add(this.debuffFormula);
            this.All.Add(this.pvpRating);
            this.All.Add(this.savedXP);
            this.All.Add(this.doorBlockTime);
            this.All.Add(this.overrideTexture);
            this.All.Add(this.overrideMaterial);
            this.All.Add(this.deathReason);
            this.All.Add(this.damageOverrideType);
            this.All.Add(this.brainType);
            this.All.Add(this.xpBonus);
            this.All.Add(this.healInterval);
            this.All.Add(this.healDelta);
            this.All.Add(this.monsterTexture);
            this.All.Add(this.hasAlwaysLootable);
            this.All.Add(this.tradeLimit);
            this.All.Add(this.faceTexture);
            this.All.Add(this.specialCondition);
            this.All.Add(this.autoAttackFlags);
            this.All.Add(this.nextXP);
            this.All.Add(this.teleportPauseMilliSeconds);
            this.All.Add(this.sisCap);
            this.All.Add(this.animSet);
            this.All.Add(this.attackType);
            this.All.Add(this.nanoFocusLevel);
            this.All.Add(this.npcHash);
            this.All.Add(this.collisionRadius);
            this.All.Add(this.outerRadius);
            this.All.Add(this.monsterData);
            this.All.Add(this.monsterScale);
            this.All.Add(this.hitEffectType);
            this.All.Add(this.resurrectDest);
            this.All.Add(this.nanoInterval);
            this.All.Add(this.nanoDelta);
            this.All.Add(this.reclaimItem);
            this.All.Add(this.gatherEffectType);
            this.All.Add(this.visualBreed);
            this.All.Add(this.visualProfession);
            this.All.Add(this.visualSex);
            this.All.Add(this.ritualTargetInst);
            this.All.Add(this.skillTimeOnSelectedTarget);
            this.All.Add(this.lastSaveXP);
            this.All.Add(this.extendedTime);
            this.All.Add(this.burstRecharge);
            this.All.Add(this.fullAutoRecharge);
            this.All.Add(this.gatherAbstractAnim);
            this.All.Add(this.castTargetAbstractAnim);
            this.All.Add(this.castSelfAbstractAnim);
            this.All.Add(this.criticalIncrease);
            this.All.Add(this.rangeIncreaserWeapon);
            this.All.Add(this.rangeIncreaserNF);
            this.All.Add(this.skillLockModifier);
            this.All.Add(this.interruptModifier);
            this.All.Add(this.acgEntranceStyles);
            this.All.Add(this.chanceOfBreakOnSpellAttack);
            this.All.Add(this.chanceOfBreakOnDebuff);
            this.All.Add(this.dieAnim);
            this.All.Add(this.towerType);
            this.All.Add(this.expansion);
            this.All.Add(this.lowresMesh);
            this.All.Add(this.criticalDecrease);
            this.All.Add(this.oldTimeExist);
            this.All.Add(this.resistModifier);
            this.All.Add(this.chestFlags);
            this.All.Add(this.primaryTemplateID);
            this.All.Add(this.numberOfItems);
            this.All.Add(this.selectedTargetType);
            this.All.Add(this.corpseHash);
            this.All.Add(this.ammoName);
            this.All.Add(this.rotation);
            this.All.Add(this.catAnim);
            this.All.Add(this.catAnimFlags);
            this.All.Add(this.displayCATAnim);
            this.All.Add(this.displayCATMesh);
            this.All.Add(this.school);
            this.All.Add(this.nanoSpeed);
            this.All.Add(this.nanoPoints);
            this.All.Add(this.trainSkill);
            this.All.Add(this.trainSkillCost);
            this.All.Add(this.isFightingMe);
            this.All.Add(this.nextFormula);
            this.All.Add(this.multipleCount);
            this.All.Add(this.effectType);
            this.All.Add(this.impactEffectType);
            this.All.Add(this.corpseType);
            this.All.Add(this.corpseInstance);
            this.All.Add(this.corpseAnimKey);
            this.All.Add(this.unarmedTemplateInstance);
            this.All.Add(this.tracerEffectType);
            this.All.Add(this.ammoType);
            this.All.Add(this.charRadius);
            this.All.Add(this.chanceOfUse);
            this.All.Add(this.currentState);
            this.All.Add(this.armourType);
            this.All.Add(this.restModifier);
            this.All.Add(this.buyModifier);
            this.All.Add(this.sellModifier);
            this.All.Add(this.castEffectType);
            this.All.Add(this.npcBrainState);
            this.All.Add(this.waitState);
            this.All.Add(this.selectedTarget);
            this.All.Add(this.missionBits4);
            this.All.Add(this.ownerInstance);
            this.All.Add(this.charState);
            this.All.Add(this.readOnly);
            this.All.Add(this.damageType);
            this.All.Add(this.collideCheckInterval);
            this.All.Add(this.playfieldType);
            this.All.Add(this.npcCommand);
            this.All.Add(this.initiativeType);
            this.All.Add(this.charTmp1);
            this.All.Add(this.charTmp2);
            this.All.Add(this.charTmp3);
            this.All.Add(this.charTmp4);
            this.All.Add(this.npcCommandArg);
            this.All.Add(this.nameTemplate);
            this.All.Add(this.desiredTargetDistance);
            this.All.Add(this.vicinityRange);
            this.All.Add(this.npcIsSurrendering);
            this.All.Add(this.stateMachine);
            this.All.Add(this.npcSurrenderInstance);
            this.All.Add(this.npcHasPatrolList);
            this.All.Add(this.npcVicinityChars);
            this.All.Add(this.proximityRangeOutdoors);
            this.All.Add(this.npcFamily);
            this.All.Add(this.commandRange);
            this.All.Add(this.npcHatelistSize);
            this.All.Add(this.npcNumPets);
            this.All.Add(this.odMinSizeAdd);
            this.All.Add(this.effectRed);
            this.All.Add(this.effectGreen);
            this.All.Add(this.effectBlue);
            this.All.Add(this.odMaxSizeAdd);
            this.All.Add(this.durationModifier);
            this.All.Add(this.npcCryForHelpRange);
            this.All.Add(this.losHeight);
            this.All.Add(this.petReq1);
            this.All.Add(this.petReq2);
            this.All.Add(this.petReq3);
            this.All.Add(this.mapOptions);
            this.All.Add(this.mapAreaPart1);
            this.All.Add(this.mapAreaPart2);
            this.All.Add(this.fixtureFlags);
            this.All.Add(this.fallDamage);
            this.All.Add(this.reflectReturnedProjectileAC);
            this.All.Add(this.reflectReturnedMeleeAC);
            this.All.Add(this.reflectReturnedEnergyAC);
            this.All.Add(this.reflectReturnedChemicalAC);
            this.All.Add(this.reflectReturnedRadiationAC);
            this.All.Add(this.reflectReturnedColdAC);
            this.All.Add(this.reflectReturnedNanoAC);
            this.All.Add(this.reflectReturnedFireAC);
            this.All.Add(this.reflectReturnedPoisonAC);
            this.All.Add(this.proximityRangeIndoors);
            this.All.Add(this.petReqVal1);
            this.All.Add(this.petReqVal2);
            this.All.Add(this.petReqVal3);
            this.All.Add(this.targetFacing);
            this.All.Add(this.backstab);
            this.All.Add(this.originatorType);
            this.All.Add(this.questInstance);
            this.All.Add(this.questIndex1);
            this.All.Add(this.questIndex2);
            this.All.Add(this.questIndex3);
            this.All.Add(this.questIndex4);
            this.All.Add(this.questIndex5);
            this.All.Add(this.qtDungeonInstance);
            this.All.Add(this.qtNumMonsters);
            this.All.Add(this.qtKilledMonsters);
            this.All.Add(this.animPos);
            this.All.Add(this.animPlay);
            this.All.Add(this.animSpeed);
            this.All.Add(this.qtKillNumMonsterID1);
            this.All.Add(this.qtKillNumMonsterCount1);
            this.All.Add(this.qtKillNumMonsterID2);
            this.All.Add(this.qtKillNumMonsterCount2);
            this.All.Add(this.qtKillNumMonsterID3);
            this.All.Add(this.qtKillNumMonsterCount3);
            this.All.Add(this.questIndex0);
            this.All.Add(this.questTimeout);
            this.All.Add(this.towerNpcHash);
            this.All.Add(this.petType);
            this.All.Add(this.onTowerCreation);
            this.All.Add(this.ownedTowers);
            this.All.Add(this.towerInstance);
            this.All.Add(this.attackShield);
            this.All.Add(this.specialAttackShield);
            this.All.Add(this.npcVicinityPlayers);
            this.All.Add(this.npcUseFightModeRegenRate);
            this.All.Add(this.rnd);
            this.All.Add(this.socialStatus);
            this.All.Add(this.lastRnd);
            this.All.Add(this.itemDelayCap);
            this.All.Add(this.rechargeDelayCap);
            this.All.Add(this.percentRemainingHealth);
            this.All.Add(this.percentRemainingNano);
            this.All.Add(this.targetDistance);
            this.All.Add(this.teamCloseness);
            this.All.Add(this.numberOnHateList);
            this.All.Add(this.conditionState);
            this.All.Add(this.expansionPlayfield);
            this.All.Add(this.shadowBreed);
            this.All.Add(this.npcFovStatus);
            this.All.Add(this.dudChance);
            this.All.Add(this.healMultiplier);
            this.All.Add(this.nanoDamageMultiplier);
            this.All.Add(this.nanoVulnerability);
            this.All.Add(this.amsCap);
            this.All.Add(this.procInitiative1);
            this.All.Add(this.procInitiative2);
            this.All.Add(this.procInitiative3);
            this.All.Add(this.procInitiative4);
            this.All.Add(this.factionModifier);
            this.All.Add(this.missionBits8);
            this.All.Add(this.missionBits9);
            this.All.Add(this.stackingLine2);
            this.All.Add(this.stackingLine3);
            this.All.Add(this.stackingLine4);
            this.All.Add(this.stackingLine5);
            this.All.Add(this.stackingLine6);
            this.All.Add(this.stackingOrder);
            this.All.Add(this.procNano1);
            this.All.Add(this.procNano2);
            this.All.Add(this.procNano3);
            this.All.Add(this.procNano4);
            this.All.Add(this.procChance1);
            this.All.Add(this.procChance2);
            this.All.Add(this.procChance3);
            this.All.Add(this.procChance4);
            this.All.Add(this.otArmedForces);
            this.All.Add(this.clanSentinels);
            this.All.Add(this.otMed);
            this.All.Add(this.clanGaia);
            this.All.Add(this.otTrans);
            this.All.Add(this.clanVanguards);
            this.All.Add(this.gos);
            this.All.Add(this.otFollowers);
            this.All.Add(this.otOperator);
            this.All.Add(this.otUnredeemed);
            this.All.Add(this.clanDevoted);
            this.All.Add(this.clanConserver);
            this.All.Add(this.clanRedeemed);
            this.All.Add(this.sk);
            this.All.Add(this.lastSK);
            this.All.Add(this.nextSK);
            this.All.Add(this.playerOptions);
            this.All.Add(this.lastPerkResetTime);
            this.All.Add(this.currentTime);
            this.All.Add(this.shadowBreedTemplate);
            this.All.Add(this.npcVicinityFamily);
            this.All.Add(this.npcScriptAmsScale);
            this.All.Add(this.apartmentsAllowed);
            this.All.Add(this.apartmentsOwned);
            this.All.Add(this.apartmentAccessCard);
            this.All.Add(this.mapAreaPart3);
            this.All.Add(this.mapAreaPart4);
            this.All.Add(this.numberOfTeamMembers);
            this.All.Add(this.actionCategory);
            this.All.Add(this.currentPlayfield);
            this.All.Add(this.districtNano);
            this.All.Add(this.districtNanoInterval);
            this.All.Add(this.unsavedXP);
            this.All.Add(this.regainXPPercentage);
            this.All.Add(this.tempSaveTeamID);
            this.All.Add(this.tempSavePlayfield);
            this.All.Add(this.tempSaveX);
            this.All.Add(this.tempSaveY);
            this.All.Add(this.extendedFlags);
            this.All.Add(this.shopPrice);
            this.All.Add(this.newbieHP);
            this.All.Add(this.hpLevelUp);
            this.All.Add(this.hpPerSkill);
            this.All.Add(this.newbieNP);
            this.All.Add(this.npLevelUp);
            this.All.Add(this.npPerSkill);
            this.All.Add(this.maxShopItems);
            this.All.Add(this.playerID);
            this.All.Add(this.shopRent);
            this.All.Add(this.synergyHash);
            this.All.Add(this.shopFlags);
            this.All.Add(this.shopLastUsed);
            this.All.Add(this.shopType);
            this.All.Add(this.lockDownTime);
            this.All.Add(this.leaderLockDownTime);
            this.All.Add(this.invadersKilled);
            this.All.Add(this.killedByInvaders);
            this.All.Add(this.missionBits10);
            this.All.Add(this.missionBits11);
            this.All.Add(this.missionBits12);
            this.All.Add(this.houseTemplate);
            this.All.Add(this.percentFireDamage);
            this.All.Add(this.percentColdDamage);
            this.All.Add(this.percentMeleeDamage);
            this.All.Add(this.percentProjectileDamage);
            this.All.Add(this.percentPoisonDamage);
            this.All.Add(this.percentRadiationDamage);
            this.All.Add(this.percentEnergyDamage);
            this.All.Add(this.percentChemicalDamage);
            this.All.Add(this.totalDamage);
            this.All.Add(this.trackProjectileDamage);
            this.All.Add(this.trackMeleeDamage);
            this.All.Add(this.trackEnergyDamage);
            this.All.Add(this.trackChemicalDamage);
            this.All.Add(this.trackRadiationDamage);
            this.All.Add(this.trackColdDamage);
            this.All.Add(this.trackPoisonDamage);
            this.All.Add(this.trackFireDamage);
            this.All.Add(this.npcSpellArg1);
            this.All.Add(this.npcSpellRet1);
            this.All.Add(this.cityInstance);
            this.All.Add(this.distanceToSpawnpoint);
            this.All.Add(this.cityTerminalRechargePercent);
            this.All.Add(this.unreadMailCount);
            this.All.Add(this.lastMailCheckTime);
            this.All.Add(this.advantageHash1);
            this.All.Add(this.advantageHash2);
            this.All.Add(this.advantageHash3);
            this.All.Add(this.advantageHash4);
            this.All.Add(this.advantageHash5);
            this.All.Add(this.shopIndex);
            this.All.Add(this.shopID);
            this.All.Add(this.isVehicle);
            this.All.Add(this.damageToNano);
            this.All.Add(this.accountFlags);
            this.All.Add(this.damageToNanoMultiplier);
            this.All.Add(this.mechData);
            this.All.Add(this.vehicleAC);
            this.All.Add(this.vehicleDamage);
            this.All.Add(this.vehicleHealth);
            this.All.Add(this.vehicleSpeed);
            this.All.Add(this.battlestationSide);
            this.All.Add(this.victoryPoints);
            this.All.Add(this.battlestationRep);
            this.All.Add(this.petState);
            this.All.Add(this.paidPoints);
            this.All.Add(this.visualFlags);
            this.All.Add(this.pvpDuelKills);
            this.All.Add(this.pvpDuelDeaths);
            this.All.Add(this.pvpProfessionDuelKills);
            this.All.Add(this.pvpProfessionDuelDeaths);
            this.All.Add(this.pvpRankedSoloKills);
            this.All.Add(this.pvpRankedSoloDeaths);
            this.All.Add(this.pvpRankedTeamKills);
            this.All.Add(this.pvpRankedTeamDeaths);
            this.All.Add(this.pvpSoloScore);
            this.All.Add(this.pvpTeamScore);
            this.All.Add(this.pvpDuelScore);
            this.All.Add(this.acgItemSeed);
            this.All.Add(this.acgItemLevel);
            this.All.Add(this.acgItemTemplateID);
            this.All.Add(this.acgItemTemplateID2);
            this.All.Add(this.acgItemCategoryID);
            this.All.Add(this.hasKnuBotData);
            this.All.Add(this.questBoothDifficulty);
            this.All.Add(this.questAsMinimumRange);
            this.All.Add(this.questAsMaximumRange);
            this.All.Add(this.visualLodLevel);
            this.All.Add(this.targetDistanceChange);
            this.All.Add(this.tideRequiredDynelID);
            this.All.Add(this.streamCheckMagic);
            this.All.Add(this.type);
            this.All.Add(this.instance);
            this.All.Add(this.weaponsStyle);
            this.All.Add(this.shoulderMeshRight);
            this.All.Add(this.shoulderMeshLeft);
            this.All.Add(this.weaponMeshRight);
            this.All.Add(this.weaponMeshLeft);
            this.All.Add(this.overrideTextureAttractor);
            this.All.Add(this.overrideTextureBack);
            this.All.Add(this.overrideTextureHead);
            this.All.Add(this.overrideTextureShoulderpadLeft);
            this.All.Add(this.overrideTextureShoulderpadRight);
            this.All.Add(this.overrideTextureWeaponLeft);
            this.All.Add(this.overrideTextureWeaponRight);
            #endregion

            #region Trickles and effects
            // add Tricklers, try not to do circulars!!
            this.SetAbilityTricklers();
            this.bodyDevelopment.Affects.Add(this.life.StatNumber);
            this.nanoEnergyPool.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.level.Affects.Add(this.life.StatNumber);
            this.level.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.level.Affects.Add(this.titleLevel.StatNumber);
            this.level.Affects.Add(this.nextSK.StatNumber);
            this.level.Affects.Add(this.nextXP.StatNumber);
            this.alienLevel.Affects.Add(this.alienNextXP.StatNumber);
            this.xp.Affects.Add(this.level.StatNumber);
            this.sk.Affects.Add(this.level.StatNumber);
            this.alienXP.Affects.Add(this.alienLevel.StatNumber);
            this.profession.Affects.Add(this.health.StatNumber);
            this.profession.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.profession.Affects.Add(this.ip.StatNumber);
            this.stamina.Affects.Add(this.healDelta.StatNumber);
            this.psychic.Affects.Add(this.nanoDelta.StatNumber);
            this.stamina.Affects.Add(this.healInterval.StatNumber);
            this.psychic.Affects.Add(this.nanoInterval.StatNumber);
            this.level.Affects.Add(this.ip.StatNumber);
            #endregion

            foreach (ClassStat c in this.All)
            {
                c.SetParent(parent);
            }
            if (!(parent is NonPlayerCharacterClass))
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
            this.expansion.DoNotDontWriteToSql = true;
            this.accountFlags.DoNotDontWriteToSql = true;
            this.playerID.DoNotDontWriteToSql = true;
            this.professionLevel.DoNotDontWriteToSql = true;
            this.gmLevel.DoNotDontWriteToSql = true;
            this.type.DoNotDontWriteToSql = true;
            this.instance.DoNotDontWriteToSql = true;
            #endregion
        }

        public ClassStat Flags
        {
            get
            {
                return this.flags;
            }
        }

        public StatHealth Life
        {
            get
            {
                return this.life;
            }
        }

        public ClassStat VolumeMass
        {
            get
            {
                return this.volumeMass;
            }
        }

        public ClassStat AttackSpeed
        {
            get
            {
                return this.attackSpeed;
            }
        }

        public ClassStat Breed
        {
            get
            {
                return this.breed;
            }
        }

        public ClassStat Clan
        {
            get
            {
                return this.clan;
            }
        }

        public ClassStat Team
        {
            get
            {
                return this.team;
            }
        }

        public ClassStat State
        {
            get
            {
                return this.state;
            }
        }

        public ClassStat TimeExist
        {
            get
            {
                return this.timeExist;
            }
        }

        public ClassStat MapFlags
        {
            get
            {
                return this.mapFlags;
            }
        }

        public ClassStat ProfessionLevel
        {
            get
            {
                return this.professionLevel;
            }
        }

        public ClassStat PreviousHealth
        {
            get
            {
                return this.previousHealth;
            }
        }

        public ClassStat Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        public ClassStat Anim
        {
            get
            {
                return this.anim;
            }
        }

        public ClassStat Name
        {
            get
            {
                return this.name;
            }
        }

        public ClassStat Info
        {
            get
            {
                return this.info;
            }
        }

        public ClassStat Strength
        {
            get
            {
                return this.strength;
            }
        }

        public ClassStat Agility
        {
            get
            {
                return this.agility;
            }
        }

        public ClassStat Stamina
        {
            get
            {
                return this.stamina;
            }
        }

        public ClassStat Intelligence
        {
            get
            {
                return this.intelligence;
            }
        }

        public ClassStat Sense
        {
            get
            {
                return this.sense;
            }
        }

        public ClassStat Psychic
        {
            get
            {
                return this.psychic;
            }
        }

        public ClassStat Ams
        {
            get
            {
                return this.ams;
            }
        }

        public ClassStat StaticInstance
        {
            get
            {
                return this.staticInstance;
            }
        }

        public ClassStat MaxMass
        {
            get
            {
                return this.maxMass;
            }
        }

        public ClassStat StaticType
        {
            get
            {
                return this.staticType;
            }
        }

        public ClassStat Energy
        {
            get
            {
                return this.energy;
            }
        }

        public StatHitPoints Health
        {
            get
            {
                return this.health;
            }
        }

        public ClassStat Height
        {
            get
            {
                return this.height;
            }
        }

        public ClassStat Dms
        {
            get
            {
                return this.dms;
            }
        }

        public ClassStat Can
        {
            get
            {
                return this.can;
            }
        }

        public ClassStat Face
        {
            get
            {
                return this.face;
            }
        }

        public ClassStat HairMesh
        {
            get
            {
                return this.hairMesh;
            }
        }

        public ClassStat Side
        {
            get
            {
                return this.side;
            }
        }

        public ClassStat DeadTimer
        {
            get
            {
                return this.deadTimer;
            }
        }

        public ClassStat AccessCount
        {
            get
            {
                return this.accessCount;
            }
        }

        public ClassStat AttackCount
        {
            get
            {
                return this.attackCount;
            }
        }

        public StatTitleLevel TitleLevel
        {
            get
            {
                return this.titleLevel;
            }
        }

        public ClassStat BackMesh
        {
            get
            {
                return this.backMesh;
            }
        }

        public ClassStat ShoulderMeshHolder
        {
            get
            {
                return this.shoulderMeshHolder;
            }
        }

        public ClassStat AlienXp
        {
            get
            {
                return this.alienXP;
            }
        }

        public ClassStat FabricType
        {
            get
            {
                return this.fabricType;
            }
        }

        public ClassStat CatMesh
        {
            get
            {
                return this.catMesh;
            }
        }

        public ClassStat ParentType
        {
            get
            {
                return this.parentType;
            }
        }

        public ClassStat ParentInstance
        {
            get
            {
                return this.parentInstance;
            }
        }

        public ClassStat BeltSlots
        {
            get
            {
                return this.beltSlots;
            }
        }

        public ClassStat BandolierSlots
        {
            get
            {
                return this.bandolierSlots;
            }
        }

        public ClassStat Fatness
        {
            get
            {
                return this.fatness;
            }
        }

        public ClassStat ClanLevel
        {
            get
            {
                return this.clanLevel;
            }
        }

        public ClassStat InsuranceTime
        {
            get
            {
                return this.insuranceTime;
            }
        }

        public ClassStat InventoryTimeout
        {
            get
            {
                return this.inventoryTimeout;
            }
        }

        public ClassStat AggDef
        {
            get
            {
                return this.aggDef;
            }
        }

        public ClassStat Xp
        {
            get
            {
                return this.xp;
            }
        }

        public StatIP Ip
        {
            get
            {
                return this.ip;
            }
        }

        public ClassStat Level
        {
            get
            {
                return this.level;
            }
        }

        public ClassStat InventoryId
        {
            get
            {
                return this.inventoryId;
            }
        }

        public ClassStat TimeSinceCreation
        {
            get
            {
                return this.timeSinceCreation;
            }
        }

        public ClassStat LastXp
        {
            get
            {
                return this.lastXP;
            }
        }

        public ClassStat Age
        {
            get
            {
                return this.age;
            }
        }

        public ClassStat Sex
        {
            get
            {
                return this.sex;
            }
        }

        public ClassStat Profession
        {
            get
            {
                return this.profession;
            }
        }

        public ClassStat Cash
        {
            get
            {
                return this.cash;
            }
        }

        public ClassStat Alignment
        {
            get
            {
                return this.alignment;
            }
        }

        public ClassStat Attitude
        {
            get
            {
                return this.attitude;
            }
        }

        public ClassStat HeadMesh
        {
            get
            {
                return this.headMesh;
            }
        }

        public ClassStat MissionBits5
        {
            get
            {
                return this.missionBits5;
            }
        }

        public ClassStat MissionBits6
        {
            get
            {
                return this.missionBits6;
            }
        }

        public ClassStat MissionBits7
        {
            get
            {
                return this.missionBits7;
            }
        }

        public ClassStat VeteranPoints
        {
            get
            {
                return this.veteranPoints;
            }
        }

        public ClassStat MonthsPaid
        {
            get
            {
                return this.monthsPaid;
            }
        }

        public ClassStat SpeedPenalty
        {
            get
            {
                return this.speedPenalty;
            }
        }

        public ClassStat TotalMass
        {
            get
            {
                return this.totalMass;
            }
        }

        public ClassStat ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        public ClassStat RepairDifficulty
        {
            get
            {
                return this.repairDifficulty;
            }
        }

        public ClassStat Price
        {
            get
            {
                return this.price;
            }
        }

        public ClassStat MetaType
        {
            get
            {
                return this.metaType;
            }
        }

        public ClassStat ItemClass
        {
            get
            {
                return this.itemClass;
            }
        }

        public ClassStat RepairSkill
        {
            get
            {
                return this.repairSkill;
            }
        }

        public ClassStat CurrentMass
        {
            get
            {
                return this.currentMass;
            }
        }

        public ClassStat PrimaryItemType
        {
            get
            {
                return this.primaryItemType;
            }
        }

        public ClassStat Icon
        {
            get
            {
                return this.icon;
            }
        }

        public ClassStat PrimaryItemInstance
        {
            get
            {
                return this.primaryItemInstance;
            }
        }

        public ClassStat SecondaryItemType
        {
            get
            {
                return this.secondaryItemType;
            }
        }

        public ClassStat SecondaryItemInstance
        {
            get
            {
                return this.secondaryItemInstance;
            }
        }

        public ClassStat UserType
        {
            get
            {
                return this.userType;
            }
        }

        public ClassStat UserInstance
        {
            get
            {
                return this.userInstance;
            }
        }

        public ClassStat AreaType
        {
            get
            {
                return this.areaType;
            }
        }

        public ClassStat AreaInstance
        {
            get
            {
                return this.areaInstance;
            }
        }

        public ClassStat DefaultPos
        {
            get
            {
                return this.defaultPos;
            }
        }

        public ClassStat Race
        {
            get
            {
                return this.race;
            }
        }

        public ClassStat ProjectileAc
        {
            get
            {
                return this.projectileAC;
            }
        }

        public ClassStat MeleeAc
        {
            get
            {
                return this.meleeAC;
            }
        }

        public ClassStat EnergyAc
        {
            get
            {
                return this.energyAC;
            }
        }

        public ClassStat ChemicalAc
        {
            get
            {
                return this.chemicalAC;
            }
        }

        public ClassStat RadiationAc
        {
            get
            {
                return this.radiationAC;
            }
        }

        public ClassStat ColdAc
        {
            get
            {
                return this.coldAC;
            }
        }

        public ClassStat PoisonAc
        {
            get
            {
                return this.poisonAC;
            }
        }

        public ClassStat FireAc
        {
            get
            {
                return this.fireAC;
            }
        }

        public ClassStat StateAction
        {
            get
            {
                return this.stateAction;
            }
        }

        public ClassStat ItemAnim
        {
            get
            {
                return this.itemAnim;
            }
        }

        public StatSkill MartialArts
        {
            get
            {
                return this.martialArts;
            }
        }

        public StatSkill MeleeMultiple
        {
            get
            {
                return this.meleeMultiple;
            }
        }

        public StatSkill OnehBluntWeapons
        {
            get
            {
                return this.onehBluntWeapons;
            }
        }

        public StatSkill OnehEdgedWeapon
        {
            get
            {
                return this.onehEdgedWeapon;
            }
        }

        public StatSkill MeleeEnergyWeapon
        {
            get
            {
                return this.meleeEnergyWeapon;
            }
        }

        public StatSkill TwohEdgedWeapons
        {
            get
            {
                return this.twohEdgedWeapons;
            }
        }

        public StatSkill Piercing
        {
            get
            {
                return this.piercing;
            }
        }

        public StatSkill TwohBluntWeapons
        {
            get
            {
                return this.twohBluntWeapons;
            }
        }

        public StatSkill ThrowingKnife
        {
            get
            {
                return this.throwingKnife;
            }
        }

        public StatSkill Grenade
        {
            get
            {
                return this.grenade;
            }
        }

        public StatSkill ThrownGrapplingWeapons
        {
            get
            {
                return this.thrownGrapplingWeapons;
            }
        }

        public StatSkill Bow
        {
            get
            {
                return this.bow;
            }
        }

        public StatSkill Pistol
        {
            get
            {
                return this.pistol;
            }
        }

        public StatSkill Rifle
        {
            get
            {
                return this.rifle;
            }
        }

        public StatSkill SubMachineGun
        {
            get
            {
                return this.subMachineGun;
            }
        }

        public StatSkill Shotgun
        {
            get
            {
                return this.shotgun;
            }
        }

        public StatSkill AssaultRifle
        {
            get
            {
                return this.assaultRifle;
            }
        }

        public StatSkill DriveWater
        {
            get
            {
                return this.driveWater;
            }
        }

        public StatSkill CloseCombatInitiative
        {
            get
            {
                return this.closeCombatInitiative;
            }
        }

        public StatSkill DistanceWeaponInitiative
        {
            get
            {
                return this.distanceWeaponInitiative;
            }
        }

        public StatSkill PhysicalProwessInitiative
        {
            get
            {
                return this.physicalProwessInitiative;
            }
        }

        public StatSkill BowSpecialAttack
        {
            get
            {
                return this.bowSpecialAttack;
            }
        }

        public StatSkill SenseImprovement
        {
            get
            {
                return this.senseImprovement;
            }
        }

        public StatSkill FirstAid
        {
            get
            {
                return this.firstAid;
            }
        }

        public StatSkill Treatment
        {
            get
            {
                return this.treatment;
            }
        }

        public StatSkill MechanicalEngineering
        {
            get
            {
                return this.mechanicalEngineering;
            }
        }

        public StatSkill ElectricalEngineering
        {
            get
            {
                return this.electricalEngineering;
            }
        }

        public StatSkill MaterialMetamorphose
        {
            get
            {
                return this.materialMetamorphose;
            }
        }

        public StatSkill BiologicalMetamorphose
        {
            get
            {
                return this.biologicalMetamorphose;
            }
        }

        public StatSkill PsychologicalModification
        {
            get
            {
                return this.psychologicalModification;
            }
        }

        public StatSkill MaterialCreation
        {
            get
            {
                return this.materialCreation;
            }
        }

        public StatSkill MaterialLocation
        {
            get
            {
                return this.materialLocation;
            }
        }

        public StatSkill NanoEnergyPool
        {
            get
            {
                return this.nanoEnergyPool;
            }
        }

        public StatSkill LrEnergyWeapon
        {
            get
            {
                return this.lrEnergyWeapon;
            }
        }

        public StatSkill LrMultipleWeapon
        {
            get
            {
                return this.lrMultipleWeapon;
            }
        }

        public StatSkill DisarmTrap
        {
            get
            {
                return this.disarmTrap;
            }
        }

        public StatSkill Perception
        {
            get
            {
                return this.perception;
            }
        }

        public StatSkill Adventuring
        {
            get
            {
                return this.adventuring;
            }
        }

        public StatSkill Swim
        {
            get
            {
                return this.swim;
            }
        }

        public StatSkill DriveAir
        {
            get
            {
                return this.driveAir;
            }
        }

        public StatSkill MapNavigation
        {
            get
            {
                return this.mapNavigation;
            }
        }

        public StatSkill Tutoring
        {
            get
            {
                return this.tutoring;
            }
        }

        public StatSkill Brawl
        {
            get
            {
                return this.brawl;
            }
        }

        public StatSkill Riposte
        {
            get
            {
                return this.riposte;
            }
        }

        public StatSkill Dimach
        {
            get
            {
                return this.dimach;
            }
        }

        public StatSkill Parry
        {
            get
            {
                return this.parry;
            }
        }

        public StatSkill SneakAttack
        {
            get
            {
                return this.sneakAttack;
            }
        }

        public StatSkill FastAttack
        {
            get
            {
                return this.fastAttack;
            }
        }

        public StatSkill Burst
        {
            get
            {
                return this.burst;
            }
        }

        public StatSkill NanoProwessInitiative
        {
            get
            {
                return this.nanoProwessInitiative;
            }
        }

        public StatSkill FlingShot
        {
            get
            {
                return this.flingShot;
            }
        }

        public StatSkill AimedShot
        {
            get
            {
                return this.aimedShot;
            }
        }

        public StatSkill BodyDevelopment
        {
            get
            {
                return this.bodyDevelopment;
            }
        }

        public StatSkill Duck
        {
            get
            {
                return this.duck;
            }
        }

        public StatSkill Dodge
        {
            get
            {
                return this.dodge;
            }
        }

        public StatSkill Evade
        {
            get
            {
                return this.evade;
            }
        }

        public StatSkill RunSpeed
        {
            get
            {
                return this.runSpeed;
            }
        }

        public StatSkill FieldQuantumPhysics
        {
            get
            {
                return this.fieldQuantumPhysics;
            }
        }

        public StatSkill WeaponSmithing
        {
            get
            {
                return this.weaponSmithing;
            }
        }

        public StatSkill Pharmaceuticals
        {
            get
            {
                return this.pharmaceuticals;
            }
        }

        public StatSkill NanoProgramming
        {
            get
            {
                return this.nanoProgramming;
            }
        }

        public StatSkill ComputerLiteracy
        {
            get
            {
                return this.computerLiteracy;
            }
        }

        public StatSkill Psychology
        {
            get
            {
                return this.psychology;
            }
        }

        public StatSkill Chemistry
        {
            get
            {
                return this.chemistry;
            }
        }

        public StatSkill Concealment
        {
            get
            {
                return this.concealment;
            }
        }

        public StatSkill BreakingEntry
        {
            get
            {
                return this.breakingEntry;
            }
        }

        public StatSkill DriveGround
        {
            get
            {
                return this.driveGround;
            }
        }

        public StatSkill FullAuto
        {
            get
            {
                return this.fullAuto;
            }
        }

        public StatSkill NanoAc
        {
            get
            {
                return this.nanoAC;
            }
        }

        public ClassStat AlienLevel
        {
            get
            {
                return this.alienLevel;
            }
        }

        public ClassStat HealthChangeBest
        {
            get
            {
                return this.healthChangeBest;
            }
        }

        public ClassStat HealthChangeWorst
        {
            get
            {
                return this.healthChangeWorst;
            }
        }

        public ClassStat HealthChange
        {
            get
            {
                return this.healthChange;
            }
        }

        public ClassStat CurrentMovementMode
        {
            get
            {
                return this.currentMovementMode;
            }
        }

        public ClassStat PrevMovementMode
        {
            get
            {
                return this.prevMovementMode;
            }
        }

        public ClassStat AutoLockTimeDefault
        {
            get
            {
                return this.autoLockTimeDefault;
            }
        }

        public ClassStat AutoUnlockTimeDefault
        {
            get
            {
                return this.autoUnlockTimeDefault;
            }
        }

        public ClassStat MoreFlags
        {
            get
            {
                return this.moreFlags;
            }
        }

        public StatAlienNextXP AlienNextXp
        {
            get
            {
                return this.alienNextXP;
            }
        }

        public ClassStat NpcFlags
        {
            get
            {
                return this.npcFlags;
            }
        }

        public ClassStat CurrentNcu
        {
            get
            {
                return this.currentNCU;
            }
        }

        public ClassStat MaxNcu
        {
            get
            {
                return this.maxNCU;
            }
        }

        public ClassStat Specialization
        {
            get
            {
                return this.specialization;
            }
        }

        public ClassStat EffectIcon
        {
            get
            {
                return this.effectIcon;
            }
        }

        public ClassStat BuildingType
        {
            get
            {
                return this.buildingType;
            }
        }

        public ClassStat BuildingInstance
        {
            get
            {
                return this.buildingInstance;
            }
        }

        public ClassStat CardOwnerType
        {
            get
            {
                return this.cardOwnerType;
            }
        }

        public ClassStat CardOwnerInstance
        {
            get
            {
                return this.cardOwnerInstance;
            }
        }

        public ClassStat BuildingComplexInst
        {
            get
            {
                return this.buildingComplexInst;
            }
        }

        public ClassStat ExitInstance
        {
            get
            {
                return this.exitInstance;
            }
        }

        public ClassStat NextDoorInBuilding
        {
            get
            {
                return this.nextDoorInBuilding;
            }
        }

        public ClassStat LastConcretePlayfieldInstance
        {
            get
            {
                return this.lastConcretePlayfieldInstance;
            }
        }

        public ClassStat ExtenalPlayfieldInstance
        {
            get
            {
                return this.extenalPlayfieldInstance;
            }
        }

        public ClassStat ExtenalDoorInstance
        {
            get
            {
                return this.extenalDoorInstance;
            }
        }

        public ClassStat InPlay
        {
            get
            {
                return this.inPlay;
            }
        }

        public ClassStat AccessKey
        {
            get
            {
                return this.accessKey;
            }
        }

        public ClassStat PetMaster
        {
            get
            {
                return this.petMaster;
            }
        }

        public ClassStat OrientationMode
        {
            get
            {
                return this.orientationMode;
            }
        }

        public ClassStat SessionTime
        {
            get
            {
                return this.sessionTime;
            }
        }

        public ClassStat Rp
        {
            get
            {
                return this.rp;
            }
        }

        public ClassStat Conformity
        {
            get
            {
                return this.conformity;
            }
        }

        public ClassStat Aggressiveness
        {
            get
            {
                return this.aggressiveness;
            }
        }

        public ClassStat Stability
        {
            get
            {
                return this.stability;
            }
        }

        public ClassStat Extroverty
        {
            get
            {
                return this.extroverty;
            }
        }

        public ClassStat BreedHostility
        {
            get
            {
                return this.breedHostility;
            }
        }

        public ClassStat ReflectProjectileAc
        {
            get
            {
                return this.reflectProjectileAC;
            }
        }

        public ClassStat ReflectMeleeAc
        {
            get
            {
                return this.reflectMeleeAC;
            }
        }

        public ClassStat ReflectEnergyAc
        {
            get
            {
                return this.reflectEnergyAC;
            }
        }

        public ClassStat ReflectChemicalAC
        {
            get
            {
                return this.reflectChemicalAC;
            }
        }

        public ClassStat WeaponMeshHolder
        {
            get
            {
                return this.weaponMeshHolder;
            }
        }

        public ClassStat RechargeDelay
        {
            get
            {
                return this.rechargeDelay;
            }
        }

        public ClassStat EquipDelay
        {
            get
            {
                return this.equipDelay;
            }
        }

        public ClassStat MaxEnergy
        {
            get
            {
                return this.maxEnergy;
            }
        }

        public ClassStat TeamSide
        {
            get
            {
                return this.teamSide;
            }
        }

        public StatNanoPoints CurrentNano
        {
            get
            {
                return this.currentNano;
            }
        }

        public ClassStat GmLevel
        {
            get
            {
                return this.gmLevel;
            }
        }

        public ClassStat ReflectRadiationAC
        {
            get
            {
                return this.reflectRadiationAC;
            }
        }

        public ClassStat ReflectColdAC
        {
            get
            {
                return this.reflectColdAC;
            }
        }

        public ClassStat ReflectNanoAC
        {
            get
            {
                return this.reflectNanoAC;
            }
        }

        public ClassStat ReflectFireAC
        {
            get
            {
                return this.reflectFireAC;
            }
        }

        public ClassStat CurrBodyLocation
        {
            get
            {
                return this.currBodyLocation;
            }
        }

        public StatNano MaxNanoEnergy
        {
            get
            {
                return this.maxNanoEnergy;
            }
        }

        public ClassStat AccumulatedDamage
        {
            get
            {
                return this.accumulatedDamage;
            }
        }

        public ClassStat CanChangeClothes
        {
            get
            {
                return this.canChangeClothes;
            }
        }

        public ClassStat Features
        {
            get
            {
                return this.features;
            }
        }

        public ClassStat ReflectPoisonAC
        {
            get
            {
                return this.reflectPoisonAC;
            }
        }

        public ClassStat ShieldProjectileAC
        {
            get
            {
                return this.shieldProjectileAC;
            }
        }

        public ClassStat ShieldMeleeAC
        {
            get
            {
                return this.shieldMeleeAC;
            }
        }

        public ClassStat ShieldEnergyAC
        {
            get
            {
                return this.shieldEnergyAC;
            }
        }

        public ClassStat ShieldChemicalAC
        {
            get
            {
                return this.shieldChemicalAC;
            }
        }

        public ClassStat ShieldRadiationAC
        {
            get
            {
                return this.shieldRadiationAC;
            }
        }

        public ClassStat ShieldColdAC
        {
            get
            {
                return this.shieldColdAC;
            }
        }

        public ClassStat ShieldNanoAC
        {
            get
            {
                return this.shieldNanoAC;
            }
        }

        public ClassStat ShieldFireAC
        {
            get
            {
                return this.shieldFireAC;
            }
        }

        public ClassStat ShieldPoisonAC
        {
            get
            {
                return this.shieldPoisonAC;
            }
        }

        public ClassStat BerserkMode
        {
            get
            {
                return this.berserkMode;
            }
        }

        public ClassStat InsurancePercentage
        {
            get
            {
                return this.insurancePercentage;
            }
        }

        public ClassStat ChangeSideCount
        {
            get
            {
                return this.changeSideCount;
            }
        }

        public ClassStat AbsorbProjectileAC
        {
            get
            {
                return this.absorbProjectileAC;
            }
        }

        public ClassStat AbsorbMeleeAC
        {
            get
            {
                return this.absorbMeleeAC;
            }
        }

        public ClassStat AbsorbEnergyAC
        {
            get
            {
                return this.absorbEnergyAC;
            }
        }

        public ClassStat AbsorbChemicalAC
        {
            get
            {
                return this.absorbChemicalAC;
            }
        }

        public ClassStat AbsorbRadiationAC
        {
            get
            {
                return this.absorbRadiationAC;
            }
        }

        public ClassStat AbsorbColdAC
        {
            get
            {
                return this.absorbColdAC;
            }
        }

        public ClassStat AbsorbFireAC
        {
            get
            {
                return this.absorbFireAC;
            }
        }

        public ClassStat AbsorbPoisonAC
        {
            get
            {
                return this.absorbPoisonAC;
            }
        }

        public ClassStat AbsorbNanoAC
        {
            get
            {
                return this.absorbNanoAC;
            }
        }

        public ClassStat TemporarySkillReduction
        {
            get
            {
                return this.temporarySkillReduction;
            }
        }

        public ClassStat BirthDate
        {
            get
            {
                return this.birthDate;
            }
        }

        public ClassStat LastSaved
        {
            get
            {
                return this.lastSaved;
            }
        }

        public ClassStat SoundVolume
        {
            get
            {
                return this.soundVolume;
            }
        }

        public ClassStat PetCounter
        {
            get
            {
                return this.petCounter;
            }
        }

        public ClassStat MetersWalked
        {
            get
            {
                return this.metersWalked;
            }
        }

        public ClassStat QuestLevelsSolved
        {
            get
            {
                return this.questLevelsSolved;
            }
        }

        public ClassStat MonsterLevelsKilled
        {
            get
            {
                return this.monsterLevelsKilled;
            }
        }

        public ClassStat PvPLevelsKilled
        {
            get
            {
                return this.pvPLevelsKilled;
            }
        }

        public ClassStat MissionBits1
        {
            get
            {
                return this.missionBits1;
            }
        }

        public ClassStat MissionBits2
        {
            get
            {
                return this.missionBits2;
            }
        }

        public ClassStat AccessGrant
        {
            get
            {
                return this.accessGrant;
            }
        }

        public ClassStat DoorFlags
        {
            get
            {
                return this.doorFlags;
            }
        }

        public ClassStat ClanHierarchy
        {
            get
            {
                return this.clanHierarchy;
            }
        }

        public ClassStat QuestStat
        {
            get
            {
                return this.questStat;
            }
        }

        public ClassStat ClientActivated
        {
            get
            {
                return this.clientActivated;
            }
        }

        public ClassStat PersonalResearchLevel
        {
            get
            {
                return this.personalResearchLevel;
            }
        }

        public ClassStat GlobalResearchLevel
        {
            get
            {
                return this.globalResearchLevel;
            }
        }

        public ClassStat PersonalResearchGoal
        {
            get
            {
                return this.personalResearchGoal;
            }
        }

        public ClassStat GlobalResearchGoal
        {
            get
            {
                return this.globalResearchGoal;
            }
        }

        public ClassStat TurnSpeed
        {
            get
            {
                return this.turnSpeed;
            }
        }

        public ClassStat LiquidType
        {
            get
            {
                return this.liquidType;
            }
        }

        public ClassStat GatherSound
        {
            get
            {
                return this.gatherSound;
            }
        }

        public ClassStat CastSound
        {
            get
            {
                return this.castSound;
            }
        }

        public ClassStat TravelSound
        {
            get
            {
                return this.travelSound;
            }
        }

        public ClassStat HitSound
        {
            get
            {
                return this.hitSound;
            }
        }

        public ClassStat SecondaryItemTemplate
        {
            get
            {
                return this.secondaryItemTemplate;
            }
        }

        public ClassStat EquippedWeapons
        {
            get
            {
                return this.equippedWeapons;
            }
        }

        public ClassStat XpKillRange
        {
            get
            {
                return this.xpKillRange;
            }
        }

        public ClassStat AmsModifier
        {
            get
            {
                return this.amsModifier;
            }
        }

        public ClassStat DmsModifier
        {
            get
            {
                return this.dmsModifier;
            }
        }

        public ClassStat ProjectileDamageModifier
        {
            get
            {
                return this.projectileDamageModifier;
            }
        }

        public ClassStat MeleeDamageModifier
        {
            get
            {
                return this.meleeDamageModifier;
            }
        }

        public ClassStat EnergyDamageModifier
        {
            get
            {
                return this.energyDamageModifier;
            }
        }

        public ClassStat ChemicalDamageModifier
        {
            get
            {
                return this.chemicalDamageModifier;
            }
        }

        public ClassStat RadiationDamageModifier
        {
            get
            {
                return this.radiationDamageModifier;
            }
        }

        public ClassStat ItemHateValue
        {
            get
            {
                return this.itemHateValue;
            }
        }

        public ClassStat DamageBonus
        {
            get
            {
                return this.damageBonus;
            }
        }

        public ClassStat MaxDamage
        {
            get
            {
                return this.maxDamage;
            }
        }

        public ClassStat MinDamage
        {
            get
            {
                return this.minDamage;
            }
        }

        public ClassStat AttackRange
        {
            get
            {
                return this.attackRange;
            }
        }

        public ClassStat HateValueModifyer
        {
            get
            {
                return this.hateValueModifyer;
            }
        }

        public ClassStat TrapDifficulty
        {
            get
            {
                return this.trapDifficulty;
            }
        }

        public ClassStat StatOne
        {
            get
            {
                return this.statOne;
            }
        }

        public ClassStat NumAttackEffects
        {
            get
            {
                return this.numAttackEffects;
            }
        }

        public ClassStat DefaultAttackType
        {
            get
            {
                return this.defaultAttackType;
            }
        }

        public ClassStat ItemSkill
        {
            get
            {
                return this.itemSkill;
            }
        }

        public ClassStat ItemDelay
        {
            get
            {
                return this.itemDelay;
            }
        }

        public ClassStat ItemOpposedSkill
        {
            get
            {
                return this.itemOpposedSkill;
            }
        }

        public ClassStat ItemSis
        {
            get
            {
                return this.itemSIS;
            }
        }

        public ClassStat InteractionRadius
        {
            get
            {
                return this.interactionRadius;
            }
        }

        public ClassStat Placement
        {
            get
            {
                return this.placement;
            }
        }

        public ClassStat LockDifficulty
        {
            get
            {
                return this.lockDifficulty;
            }
        }

        public ClassStat Members
        {
            get
            {
                return this.members;
            }
        }

        public ClassStat MinMembers
        {
            get
            {
                return this.minMembers;
            }
        }

        public ClassStat ClanPrice
        {
            get
            {
                return this.clanPrice;
            }
        }

        public ClassStat MissionBits3
        {
            get
            {
                return this.missionBits3;
            }
        }

        public ClassStat ClanType
        {
            get
            {
                return this.clanType;
            }
        }

        public ClassStat ClanInstance
        {
            get
            {
                return this.clanInstance;
            }
        }

        public ClassStat VoteCount
        {
            get
            {
                return this.voteCount;
            }
        }

        public ClassStat MemberType
        {
            get
            {
                return this.memberType;
            }
        }

        public ClassStat MemberInstance
        {
            get
            {
                return this.memberInstance;
            }
        }

        public ClassStat GlobalClanType
        {
            get
            {
                return this.globalClanType;
            }
        }

        public ClassStat GlobalClanInstance
        {
            get
            {
                return this.globalClanInstance;
            }
        }

        public ClassStat ColdDamageModifier
        {
            get
            {
                return this.coldDamageModifier;
            }
        }

        public ClassStat ClanUpkeepInterval
        {
            get
            {
                return this.clanUpkeepInterval;
            }
        }

        public ClassStat TimeSinceUpkeep
        {
            get
            {
                return this.timeSinceUpkeep;
            }
        }

        public ClassStat ClanFinalized
        {
            get
            {
                return this.clanFinalized;
            }
        }

        public ClassStat NanoDamageModifier
        {
            get
            {
                return this.nanoDamageModifier;
            }
        }

        public ClassStat FireDamageModifier
        {
            get
            {
                return this.fireDamageModifier;
            }
        }

        public ClassStat PoisonDamageModifier
        {
            get
            {
                return this.poisonDamageModifier;
            }
        }

        public ClassStat NpCostModifier
        {
            get
            {
                return this.npCostModifier;
            }
        }

        public ClassStat XpModifier
        {
            get
            {
                return this.xpModifier;
            }
        }

        public ClassStat BreedLimit
        {
            get
            {
                return this.breedLimit;
            }
        }

        public ClassStat GenderLimit
        {
            get
            {
                return this.genderLimit;
            }
        }

        public ClassStat LevelLimit
        {
            get
            {
                return this.levelLimit;
            }
        }

        public ClassStat PlayerKilling
        {
            get
            {
                return this.playerKilling;
            }
        }

        public ClassStat TeamAllowed
        {
            get
            {
                return this.teamAllowed;
            }
        }

        public ClassStat WeaponDisallowedType
        {
            get
            {
                return this.weaponDisallowedType;
            }
        }

        public ClassStat WeaponDisallowedInstance
        {
            get
            {
                return this.weaponDisallowedInstance;
            }
        }

        public ClassStat Taboo
        {
            get
            {
                return this.taboo;
            }
        }

        public ClassStat Compulsion
        {
            get
            {
                return this.compulsion;
            }
        }

        public ClassStat SkillDisabled
        {
            get
            {
                return this.skillDisabled;
            }
        }

        public ClassStat ClanItemType
        {
            get
            {
                return this.clanItemType;
            }
        }

        public ClassStat ClanItemInstance
        {
            get
            {
                return this.clanItemInstance;
            }
        }

        public ClassStat DebuffFormula
        {
            get
            {
                return this.debuffFormula;
            }
        }

        public ClassStat PvpRating
        {
            get
            {
                return this.pvpRating;
            }
        }

        public ClassStat SavedXp
        {
            get
            {
                return this.savedXP;
            }
        }

        public ClassStat DoorBlockTime
        {
            get
            {
                return this.doorBlockTime;
            }
        }

        public ClassStat OverrideTexture
        {
            get
            {
                return this.overrideTexture;
            }
        }

        public ClassStat OverrideMaterial
        {
            get
            {
                return this.overrideMaterial;
            }
        }

        public ClassStat DeathReason
        {
            get
            {
                return this.deathReason;
            }
        }

        public ClassStat DamageOverrideType
        {
            get
            {
                return this.damageOverrideType;
            }
        }

        public ClassStat BrainType
        {
            get
            {
                return this.brainType;
            }
        }

        public ClassStat XpBonus
        {
            get
            {
                return this.xpBonus;
            }
        }

        public StatHealInterval HealInterval
        {
            get
            {
                return this.healInterval;
            }
        }

        public StatHealDelta HealDelta
        {
            get
            {
                return this.healDelta;
            }
        }

        public ClassStat MonsterTexture
        {
            get
            {
                return this.monsterTexture;
            }
        }

        public ClassStat HasAlwaysLootable
        {
            get
            {
                return this.hasAlwaysLootable;
            }
        }

        public ClassStat TradeLimit
        {
            get
            {
                return this.tradeLimit;
            }
        }

        public ClassStat FaceTexture
        {
            get
            {
                return this.faceTexture;
            }
        }

        public ClassStat SpecialCondition
        {
            get
            {
                return this.specialCondition;
            }
        }

        public ClassStat AutoAttackFlags
        {
            get
            {
                return this.autoAttackFlags;
            }
        }

        public StatNextXP NextXp
        {
            get
            {
                return this.nextXP;
            }
        }

        public ClassStat TeleportPauseMilliSeconds
        {
            get
            {
                return this.teleportPauseMilliSeconds;
            }
        }

        public ClassStat SisCap
        {
            get
            {
                return this.sisCap;
            }
        }

        public ClassStat AnimSet
        {
            get
            {
                return this.animSet;
            }
        }

        public ClassStat AttackType
        {
            get
            {
                return this.attackType;
            }
        }

        public ClassStat NanoFocusLevel
        {
            get
            {
                return this.nanoFocusLevel;
            }
        }

        public ClassStat NpcHash
        {
            get
            {
                return this.npcHash;
            }
        }

        public ClassStat CollisionRadius
        {
            get
            {
                return this.collisionRadius;
            }
        }

        public ClassStat OuterRadius
        {
            get
            {
                return this.outerRadius;
            }
        }

        public ClassStat MonsterData
        {
            get
            {
                return this.monsterData;
            }
        }

        public ClassStat MonsterScale
        {
            get
            {
                return this.monsterScale;
            }
        }

        public ClassStat HitEffectType
        {
            get
            {
                return this.hitEffectType;
            }
        }

        public ClassStat ResurrectDest
        {
            get
            {
                return this.resurrectDest;
            }
        }

        public StatNanoInterval NanoInterval
        {
            get
            {
                return this.nanoInterval;
            }
        }

        public StatNanoDelta NanoDelta
        {
            get
            {
                return this.nanoDelta;
            }
        }

        public ClassStat ReclaimItem
        {
            get
            {
                return this.reclaimItem;
            }
        }

        public ClassStat GatherEffectType
        {
            get
            {
                return this.gatherEffectType;
            }
        }

        public ClassStat VisualBreed
        {
            get
            {
                return this.visualBreed;
            }
        }

        public ClassStat VisualProfession
        {
            get
            {
                return this.visualProfession;
            }
        }

        public ClassStat VisualSex
        {
            get
            {
                return this.visualSex;
            }
        }

        public ClassStat RitualTargetInst
        {
            get
            {
                return this.ritualTargetInst;
            }
        }

        public ClassStat SkillTimeOnSelectedTarget
        {
            get
            {
                return this.skillTimeOnSelectedTarget;
            }
        }

        public ClassStat LastSaveXp
        {
            get
            {
                return this.lastSaveXP;
            }
        }

        public ClassStat ExtendedTime
        {
            get
            {
                return this.extendedTime;
            }
        }

        public ClassStat BurstRecharge
        {
            get
            {
                return this.burstRecharge;
            }
        }

        public ClassStat FullAutoRecharge
        {
            get
            {
                return this.fullAutoRecharge;
            }
        }

        public ClassStat GatherAbstractAnim
        {
            get
            {
                return this.gatherAbstractAnim;
            }
        }

        public ClassStat CastTargetAbstractAnim
        {
            get
            {
                return this.castTargetAbstractAnim;
            }
        }

        public ClassStat CastSelfAbstractAnim
        {
            get
            {
                return this.castSelfAbstractAnim;
            }
        }

        public ClassStat CriticalIncrease
        {
            get
            {
                return this.criticalIncrease;
            }
        }

        public ClassStat RangeIncreaserWeapon
        {
            get
            {
                return this.rangeIncreaserWeapon;
            }
        }

        public ClassStat RangeIncreaserNf
        {
            get
            {
                return this.rangeIncreaserNF;
            }
        }

        public ClassStat SkillLockModifier
        {
            get
            {
                return this.skillLockModifier;
            }
        }

        public ClassStat InterruptModifier
        {
            get
            {
                return this.interruptModifier;
            }
        }

        public ClassStat AcgEntranceStyles
        {
            get
            {
                return this.acgEntranceStyles;
            }
        }

        public ClassStat ChanceOfBreakOnSpellAttack
        {
            get
            {
                return this.chanceOfBreakOnSpellAttack;
            }
        }

        public ClassStat ChanceOfBreakOnDebuff
        {
            get
            {
                return this.chanceOfBreakOnDebuff;
            }
        }

        public ClassStat DieAnim
        {
            get
            {
                return this.dieAnim;
            }
        }

        public ClassStat TowerType
        {
            get
            {
                return this.towerType;
            }
        }

        public ClassStat Expansion
        {
            get
            {
                return this.expansion;
            }
        }

        public ClassStat LowresMesh
        {
            get
            {
                return this.lowresMesh;
            }
        }

        public ClassStat CriticalDecrease
        {
            get
            {
                return this.criticalDecrease;
            }
        }

        public ClassStat OldTimeExist
        {
            get
            {
                return this.oldTimeExist;
            }
        }

        public ClassStat ResistModifier
        {
            get
            {
                return this.resistModifier;
            }
        }

        public ClassStat ChestFlags
        {
            get
            {
                return this.chestFlags;
            }
        }

        public ClassStat PrimaryTemplateID
        {
            get
            {
                return this.primaryTemplateID;
            }
        }

        public ClassStat NumberOfItems
        {
            get
            {
                return this.numberOfItems;
            }
        }

        public ClassStat SelectedTargetType
        {
            get
            {
                return this.selectedTargetType;
            }
        }

        public ClassStat CorpseHash
        {
            get
            {
                return this.corpseHash;
            }
        }

        public ClassStat AmmoName
        {
            get
            {
                return this.ammoName;
            }
        }

        public ClassStat Rotation
        {
            get
            {
                return this.rotation;
            }
        }

        public ClassStat CatAnim
        {
            get
            {
                return this.catAnim;
            }
        }

        public ClassStat CatAnimFlags
        {
            get
            {
                return this.catAnimFlags;
            }
        }

        public ClassStat DisplayCatAnim
        {
            get
            {
                return this.displayCATAnim;
            }
        }

        public ClassStat DisplayCatMesh
        {
            get
            {
                return this.displayCATMesh;
            }
        }

        public ClassStat School
        {
            get
            {
                return this.school;
            }
        }

        public ClassStat NanoSpeed
        {
            get
            {
                return this.nanoSpeed;
            }
        }

        public ClassStat NanoPoints
        {
            get
            {
                return this.nanoPoints;
            }
        }

        public ClassStat TrainSkill
        {
            get
            {
                return this.trainSkill;
            }
        }

        public ClassStat TrainSkillCost
        {
            get
            {
                return this.trainSkillCost;
            }
        }

        public ClassStat IsFightingMe
        {
            get
            {
                return this.isFightingMe;
            }
        }

        public ClassStat NextFormula
        {
            get
            {
                return this.nextFormula;
            }
        }

        public ClassStat MultipleCount
        {
            get
            {
                return this.multipleCount;
            }
        }

        public ClassStat EffectType
        {
            get
            {
                return this.effectType;
            }
        }

        public ClassStat ImpactEffectType
        {
            get
            {
                return this.impactEffectType;
            }
        }

        public ClassStat CorpseType
        {
            get
            {
                return this.corpseType;
            }
        }

        public ClassStat CorpseInstance
        {
            get
            {
                return this.corpseInstance;
            }
        }

        public ClassStat CorpseAnimKey
        {
            get
            {
                return this.corpseAnimKey;
            }
        }

        public ClassStat UnarmedTemplateInstance
        {
            get
            {
                return this.unarmedTemplateInstance;
            }
        }

        public ClassStat TracerEffectType
        {
            get
            {
                return this.tracerEffectType;
            }
        }

        public ClassStat AmmoType
        {
            get
            {
                return this.ammoType;
            }
        }

        public ClassStat CharRadius
        {
            get
            {
                return this.charRadius;
            }
        }

        public ClassStat ChanceOfUse
        {
            get
            {
                return this.chanceOfUse;
            }
        }

        public ClassStat CurrentState
        {
            get
            {
                return this.currentState;
            }
        }

        public ClassStat ArmourType
        {
            get
            {
                return this.armourType;
            }
        }

        public ClassStat RestModifier
        {
            get
            {
                return this.restModifier;
            }
        }

        public ClassStat BuyModifier
        {
            get
            {
                return this.buyModifier;
            }
        }

        public ClassStat SellModifier
        {
            get
            {
                return this.sellModifier;
            }
        }

        public ClassStat CastEffectType
        {
            get
            {
                return this.castEffectType;
            }
        }

        public ClassStat NpcBrainState
        {
            get
            {
                return this.npcBrainState;
            }
        }

        public ClassStat WaitState
        {
            get
            {
                return this.waitState;
            }
        }

        public ClassStat SelectedTarget
        {
            get
            {
                return this.selectedTarget;
            }
        }

        public ClassStat MissionBits4
        {
            get
            {
                return this.missionBits4;
            }
        }

        public ClassStat OwnerInstance
        {
            get
            {
                return this.ownerInstance;
            }
        }

        public ClassStat CharState
        {
            get
            {
                return this.charState;
            }
        }

        public ClassStat ReadOnly
        {
            get
            {
                return this.readOnly;
            }
        }

        public ClassStat DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        public ClassStat CollideCheckInterval
        {
            get
            {
                return this.collideCheckInterval;
            }
        }

        public ClassStat PlayfieldType
        {
            get
            {
                return this.playfieldType;
            }
        }

        public ClassStat NpcCommand
        {
            get
            {
                return this.npcCommand;
            }
        }

        public ClassStat InitiativeType
        {
            get
            {
                return this.initiativeType;
            }
        }

        public ClassStat CharTmp1
        {
            get
            {
                return this.charTmp1;
            }
        }

        public ClassStat CharTmp2
        {
            get
            {
                return this.charTmp2;
            }
        }

        public ClassStat CharTmp3
        {
            get
            {
                return this.charTmp3;
            }
        }

        public ClassStat CharTmp4
        {
            get
            {
                return this.charTmp4;
            }
        }

        public ClassStat NpcCommandArg
        {
            get
            {
                return this.npcCommandArg;
            }
        }

        public ClassStat NameTemplate
        {
            get
            {
                return this.nameTemplate;
            }
        }

        public ClassStat DesiredTargetDistance
        {
            get
            {
                return this.desiredTargetDistance;
            }
        }

        public ClassStat VicinityRange
        {
            get
            {
                return this.vicinityRange;
            }
        }

        public ClassStat NpcIsSurrendering
        {
            get
            {
                return this.npcIsSurrendering;
            }
        }

        public ClassStat StateMachine
        {
            get
            {
                return this.stateMachine;
            }
        }

        public ClassStat NpcSurrenderInstance
        {
            get
            {
                return this.npcSurrenderInstance;
            }
        }

        public ClassStat NpcHasPatrolList
        {
            get
            {
                return this.npcHasPatrolList;
            }
        }

        public ClassStat NpcVicinityChars
        {
            get
            {
                return this.npcVicinityChars;
            }
        }

        public ClassStat ProximityRangeOutdoors
        {
            get
            {
                return this.proximityRangeOutdoors;
            }
        }

        public ClassStat NpcFamily
        {
            get
            {
                return this.npcFamily;
            }
        }

        public ClassStat CommandRange
        {
            get
            {
                return this.commandRange;
            }
        }

        public ClassStat NpcHatelistSize
        {
            get
            {
                return this.npcHatelistSize;
            }
        }

        public ClassStat NpcNumPets
        {
            get
            {
                return this.npcNumPets;
            }
        }

        public ClassStat OdMinSizeAdd
        {
            get
            {
                return this.odMinSizeAdd;
            }
        }

        public ClassStat EffectRed
        {
            get
            {
                return this.effectRed;
            }
        }

        public ClassStat EffectGreen
        {
            get
            {
                return this.effectGreen;
            }
        }

        public ClassStat EffectBlue
        {
            get
            {
                return this.effectBlue;
            }
        }

        public ClassStat OdMaxSizeAdd
        {
            get
            {
                return this.odMaxSizeAdd;
            }
        }

        public ClassStat DurationModifier
        {
            get
            {
                return this.durationModifier;
            }
        }

        public ClassStat NpcCryForHelpRange
        {
            get
            {
                return this.npcCryForHelpRange;
            }
        }

        public ClassStat LosHeight
        {
            get
            {
                return this.losHeight;
            }
        }

        public ClassStat PetReq1
        {
            get
            {
                return this.petReq1;
            }
        }

        public ClassStat PetReq2
        {
            get
            {
                return this.petReq2;
            }
        }

        public ClassStat PetReq3
        {
            get
            {
                return this.petReq3;
            }
        }

        public ClassStat MapOptions
        {
            get
            {
                return this.mapOptions;
            }
        }

        public ClassStat MapAreaPart1
        {
            get
            {
                return this.mapAreaPart1;
            }
        }

        public ClassStat MapAreaPart2
        {
            get
            {
                return this.mapAreaPart2;
            }
        }

        public ClassStat FixtureFlags
        {
            get
            {
                return this.fixtureFlags;
            }
        }

        public ClassStat FallDamage
        {
            get
            {
                return this.fallDamage;
            }
        }

        public ClassStat ReflectReturnedProjectileAC
        {
            get
            {
                return this.reflectReturnedProjectileAC;
            }
        }

        public ClassStat ReflectReturnedMeleeAC
        {
            get
            {
                return this.reflectReturnedMeleeAC;
            }
        }

        public ClassStat ReflectReturnedEnergyAC
        {
            get
            {
                return this.reflectReturnedEnergyAC;
            }
        }

        public ClassStat ReflectReturnedChemicalAC
        {
            get
            {
                return this.reflectReturnedChemicalAC;
            }
        }

        public ClassStat ReflectReturnedRadiationAC
        {
            get
            {
                return this.reflectReturnedRadiationAC;
            }
        }

        public ClassStat ReflectReturnedColdAC
        {
            get
            {
                return this.reflectReturnedColdAC;
            }
        }

        public ClassStat ReflectReturnedNanoAC
        {
            get
            {
                return this.reflectReturnedNanoAC;
            }
        }

        public ClassStat ReflectReturnedFireAC
        {
            get
            {
                return this.reflectReturnedFireAC;
            }
        }

        public ClassStat ReflectReturnedPoisonAC
        {
            get
            {
                return this.reflectReturnedPoisonAC;
            }
        }

        public ClassStat ProximityRangeIndoors
        {
            get
            {
                return this.proximityRangeIndoors;
            }
        }

        public ClassStat PetReqVal1
        {
            get
            {
                return this.petReqVal1;
            }
        }

        public ClassStat PetReqVal2
        {
            get
            {
                return this.petReqVal2;
            }
        }

        public ClassStat PetReqVal3
        {
            get
            {
                return this.petReqVal3;
            }
        }

        public ClassStat TargetFacing
        {
            get
            {
                return this.targetFacing;
            }
        }

        public ClassStat Backstab
        {
            get
            {
                return this.backstab;
            }
        }

        public ClassStat OriginatorType
        {
            get
            {
                return this.originatorType;
            }
        }

        public ClassStat QuestInstance
        {
            get
            {
                return this.questInstance;
            }
        }

        public ClassStat QuestIndex1
        {
            get
            {
                return this.questIndex1;
            }
        }

        public ClassStat QuestIndex2
        {
            get
            {
                return this.questIndex2;
            }
        }

        public ClassStat QuestIndex3
        {
            get
            {
                return this.questIndex3;
            }
        }

        public ClassStat QuestIndex4
        {
            get
            {
                return this.questIndex4;
            }
        }

        public ClassStat QuestIndex5
        {
            get
            {
                return this.questIndex5;
            }
        }

        public ClassStat QtDungeonInstance
        {
            get
            {
                return this.qtDungeonInstance;
            }
        }

        public ClassStat QtNumMonsters
        {
            get
            {
                return this.qtNumMonsters;
            }
        }

        public ClassStat QtKilledMonsters
        {
            get
            {
                return this.qtKilledMonsters;
            }
        }

        public ClassStat AnimPos
        {
            get
            {
                return this.animPos;
            }
        }

        public ClassStat AnimPlay
        {
            get
            {
                return this.animPlay;
            }
        }

        public ClassStat AnimSpeed
        {
            get
            {
                return this.animSpeed;
            }
        }

        public ClassStat QtKillNumMonsterID1
        {
            get
            {
                return this.qtKillNumMonsterID1;
            }
        }

        public ClassStat QtKillNumMonsterCount1
        {
            get
            {
                return this.qtKillNumMonsterCount1;
            }
        }

        public ClassStat QtKillNumMonsterID2
        {
            get
            {
                return this.qtKillNumMonsterID2;
            }
        }

        public ClassStat QtKillNumMonsterCount2
        {
            get
            {
                return this.qtKillNumMonsterCount2;
            }
        }

        public ClassStat QtKillNumMonsterID3
        {
            get
            {
                return this.qtKillNumMonsterID3;
            }
        }

        public ClassStat QtKillNumMonsterCount3
        {
            get
            {
                return this.qtKillNumMonsterCount3;
            }
        }

        public ClassStat QuestIndex0
        {
            get
            {
                return this.questIndex0;
            }
        }

        public ClassStat QuestTimeout
        {
            get
            {
                return this.questTimeout;
            }
        }

        public ClassStat TowerNpcHash
        {
            get
            {
                return this.towerNpcHash;
            }
        }

        public ClassStat PetType
        {
            get
            {
                return this.petType;
            }
        }

        public ClassStat OnTowerCreation
        {
            get
            {
                return this.onTowerCreation;
            }
        }

        public ClassStat OwnedTowers
        {
            get
            {
                return this.ownedTowers;
            }
        }

        public ClassStat TowerInstance
        {
            get
            {
                return this.towerInstance;
            }
        }

        public ClassStat AttackShield
        {
            get
            {
                return this.attackShield;
            }
        }

        public ClassStat SpecialAttackShield
        {
            get
            {
                return this.specialAttackShield;
            }
        }

        public ClassStat NpcVicinityPlayers
        {
            get
            {
                return this.npcVicinityPlayers;
            }
        }

        public ClassStat NpcUseFightModeRegenRate
        {
            get
            {
                return this.npcUseFightModeRegenRate;
            }
        }

        public ClassStat Rnd
        {
            get
            {
                return this.rnd;
            }
        }

        public ClassStat SocialStatus
        {
            get
            {
                return this.socialStatus;
            }
        }

        public ClassStat LastRnd
        {
            get
            {
                return this.lastRnd;
            }
        }

        public ClassStat ItemDelayCap
        {
            get
            {
                return this.itemDelayCap;
            }
        }

        public ClassStat RechargeDelayCap
        {
            get
            {
                return this.rechargeDelayCap;
            }
        }

        public ClassStat PercentRemainingHealth
        {
            get
            {
                return this.percentRemainingHealth;
            }
        }

        public ClassStat PercentRemainingNano
        {
            get
            {
                return this.percentRemainingNano;
            }
        }

        public ClassStat TargetDistance
        {
            get
            {
                return this.targetDistance;
            }
        }

        public ClassStat TeamCloseness
        {
            get
            {
                return this.teamCloseness;
            }
        }

        public ClassStat NumberOnHateList
        {
            get
            {
                return this.numberOnHateList;
            }
        }

        public ClassStat ConditionState
        {
            get
            {
                return this.conditionState;
            }
        }

        public ClassStat ExpansionPlayfield
        {
            get
            {
                return this.expansionPlayfield;
            }
        }

        public ClassStat ShadowBreed
        {
            get
            {
                return this.shadowBreed;
            }
        }

        public ClassStat NpcFovStatus
        {
            get
            {
                return this.npcFovStatus;
            }
        }

        public ClassStat DudChance
        {
            get
            {
                return this.dudChance;
            }
        }

        public ClassStat HealMultiplier
        {
            get
            {
                return this.healMultiplier;
            }
        }

        public ClassStat NanoDamageMultiplier
        {
            get
            {
                return this.nanoDamageMultiplier;
            }
        }

        public ClassStat NanoVulnerability
        {
            get
            {
                return this.nanoVulnerability;
            }
        }

        public ClassStat AmsCap
        {
            get
            {
                return this.amsCap;
            }
        }

        public ClassStat ProcInitiative1
        {
            get
            {
                return this.procInitiative1;
            }
        }

        public ClassStat ProcInitiative2
        {
            get
            {
                return this.procInitiative2;
            }
        }

        public ClassStat ProcInitiative3
        {
            get
            {
                return this.procInitiative3;
            }
        }

        public ClassStat ProcInitiative4
        {
            get
            {
                return this.procInitiative4;
            }
        }

        public ClassStat FactionModifier
        {
            get
            {
                return this.factionModifier;
            }
        }

        public ClassStat MissionBits8
        {
            get
            {
                return this.missionBits8;
            }
        }

        public ClassStat MissionBits9
        {
            get
            {
                return this.missionBits9;
            }
        }

        public ClassStat StackingLine2
        {
            get
            {
                return this.stackingLine2;
            }
        }

        public ClassStat StackingLine3
        {
            get
            {
                return this.stackingLine3;
            }
        }

        public ClassStat StackingLine4
        {
            get
            {
                return this.stackingLine4;
            }
        }

        public ClassStat StackingLine5
        {
            get
            {
                return this.stackingLine5;
            }
        }

        public ClassStat StackingLine6
        {
            get
            {
                return this.stackingLine6;
            }
        }

        public ClassStat StackingOrder
        {
            get
            {
                return this.stackingOrder;
            }
        }

        public ClassStat ProcNano1
        {
            get
            {
                return this.procNano1;
            }
        }

        public ClassStat ProcNano2e
        {
            get
            {
                return this.procNano2;
            }
        }

        public ClassStat ProcNano3
        {
            get
            {
                return this.procNano3;
            }
        }

        public ClassStat ProcNano4
        {
            get
            {
                return this.procNano4;
            }
        }

        public ClassStat ProcChance1
        {
            get
            {
                return this.procChance1;
            }
        }

        public ClassStat ProcChance2
        {
            get
            {
                return this.procChance2;
            }
        }

        public ClassStat ProcChance3
        {
            get
            {
                return this.procChance3;
            }
        }

        public ClassStat ProcChance4
        {
            get
            {
                return this.procChance4;
            }
        }

        public ClassStat OtArmedForces
        {
            get
            {
                return this.otArmedForces;
            }
        }

        public ClassStat ClanSentinels
        {
            get
            {
                return this.clanSentinels;
            }
        }

        public ClassStat OtMed
        {
            get
            {
                return this.otMed;
            }
        }

        public ClassStat ClanGaia
        {
            get
            {
                return this.clanGaia;
            }
        }

        public ClassStat OtTrans
        {
            get
            {
                return this.otTrans;
            }
        }

        public ClassStat ClanVanguards
        {
            get
            {
                return this.clanVanguards;
            }
        }

        public ClassStat Gos
        {
            get
            {
                return this.gos;
            }
        }

        public ClassStat OtFollowers
        {
            get
            {
                return this.otFollowers;
            }
        }

        public ClassStat OtOperator
        {
            get
            {
                return this.otOperator;
            }
        }

        public ClassStat OtUnredeemed
        {
            get
            {
                return this.otUnredeemed;
            }
        }

        public ClassStat ClanDevoted
        {
            get
            {
                return this.clanDevoted;
            }
        }

        public ClassStat ClanConserver
        {
            get
            {
                return this.clanConserver;
            }
        }

        public ClassStat ClanRedeemed
        {
            get
            {
                return this.clanRedeemed;
            }
        }

        public ClassStat Sk
        {
            get
            {
                return this.sk;
            }
        }

        public ClassStat LastSk
        {
            get
            {
                return this.lastSK;
            }
        }

        public StatNextSK NextSk
        {
            get
            {
                return this.nextSK;
            }
        }

        public ClassStat PlayerOptions
        {
            get
            {
                return this.playerOptions;
            }
        }

        public ClassStat LastPerkResetTime
        {
            get
            {
                return this.lastPerkResetTime;
            }
        }

        public ClassStat CurrentTime
        {
            get
            {
                return this.currentTime;
            }
        }

        public ClassStat ShadowBreedTemplate
        {
            get
            {
                return this.shadowBreedTemplate;
            }
        }

        public ClassStat NpcVicinityFamily
        {
            get
            {
                return this.npcVicinityFamily;
            }
        }

        public ClassStat NpcScriptAmsScale
        {
            get
            {
                return this.npcScriptAmsScale;
            }
        }

        public ClassStat ApartmentsAllowed
        {
            get
            {
                return this.apartmentsAllowed;
            }
        }

        public ClassStat ApartmentsOwned
        {
            get
            {
                return this.apartmentsOwned;
            }
        }

        public ClassStat ApartmentAccessCard
        {
            get
            {
                return this.apartmentAccessCard;
            }
        }

        public ClassStat MapAreaPart3
        {
            get
            {
                return this.mapAreaPart3;
            }
        }

        public ClassStat MapAreaPart4
        {
            get
            {
                return this.mapAreaPart4;
            }
        }

        public ClassStat NumberOfTeamMembers
        {
            get
            {
                return this.numberOfTeamMembers;
            }
        }

        public ClassStat ActionCategory
        {
            get
            {
                return this.actionCategory;
            }
        }

        public ClassStat CurrentPlayfield
        {
            get
            {
                return this.currentPlayfield;
            }
        }

        public ClassStat DistrictNano
        {
            get
            {
                return this.districtNano;
            }
        }

        public ClassStat DistrictNanoInterval
        {
            get
            {
                return this.districtNanoInterval;
            }
        }

        public ClassStat UnsavedXp
        {
            get
            {
                return this.unsavedXP;
            }
        }

        public ClassStat RegainXpPercentage
        {
            get
            {
                return this.regainXPPercentage;
            }
        }

        public ClassStat TempSaveTeamID
        {
            get
            {
                return this.tempSaveTeamID;
            }
        }

        public ClassStat TempSavePlayfield
        {
            get
            {
                return this.tempSavePlayfield;
            }
        }

        public ClassStat TempSaveX
        {
            get
            {
                return this.tempSaveX;
            }
        }

        public ClassStat TempSaveY
        {
            get
            {
                return this.tempSaveY;
            }
        }

        public ClassStat ExtendedFlags
        {
            get
            {
                return this.extendedFlags;
            }
        }

        public ClassStat ShopPrice
        {
            get
            {
                return this.shopPrice;
            }
        }

        public ClassStat NewbieHP
        {
            get
            {
                return this.newbieHP;
            }
        }

        public ClassStat HpLevelUp
        {
            get
            {
                return this.hpLevelUp;
            }
        }

        public ClassStat HpPerSkill
        {
            get
            {
                return this.hpPerSkill;
            }
        }

        public ClassStat NewbieNp
        {
            get
            {
                return this.newbieNP;
            }
        }

        public ClassStat NpLevelUp
        {
            get
            {
                return this.npLevelUp;
            }
        }

        public ClassStat NpPerSkill
        {
            get
            {
                return this.npPerSkill;
            }
        }

        public ClassStat MaxShopItems
        {
            get
            {
                return this.maxShopItems;
            }
        }

        public ClassStat PlayerID
        {
            get
            {
                return this.playerID;
            }
        }

        public ClassStat ShopRent
        {
            get
            {
                return this.shopRent;
            }
        }

        public ClassStat SynergyHash
        {
            get
            {
                return this.synergyHash;
            }
        }

        public ClassStat ShopFlags
        {
            get
            {
                return this.shopFlags;
            }
        }

        public ClassStat ShopLastUsed
        {
            get
            {
                return this.shopLastUsed;
            }
        }

        public ClassStat ShopType
        {
            get
            {
                return this.shopType;
            }
        }

        public ClassStat LockDownTime
        {
            get
            {
                return this.lockDownTime;
            }
        }

        public ClassStat LeaderLockDownTime
        {
            get
            {
                return this.leaderLockDownTime;
            }
        }

        public ClassStat InvadersKilled
        {
            get
            {
                return this.invadersKilled;
            }
        }

        public ClassStat KilledByInvaders
        {
            get
            {
                return this.killedByInvaders;
            }
        }

        public ClassStat MissionBits10
        {
            get
            {
                return this.missionBits10;
            }
        }

        public ClassStat MissionBits11
        {
            get
            {
                return this.missionBits11;
            }
        }

        public ClassStat MissionBits12
        {
            get
            {
                return this.missionBits12;
            }
        }

        public ClassStat HouseTemplate
        {
            get
            {
                return this.houseTemplate;
            }
        }

        public ClassStat PercentFireDamage
        {
            get
            {
                return this.percentFireDamage;
            }
        }

        public ClassStat PercentColdDamage
        {
            get
            {
                return this.percentColdDamage;
            }
        }

        public ClassStat PercentMeleeDamage
        {
            get
            {
                return this.percentMeleeDamage;
            }
        }

        public ClassStat PercentProjectileDamage
        {
            get
            {
                return this.percentProjectileDamage;
            }
        }

        public ClassStat PercentPoisonDamage
        {
            get
            {
                return this.percentPoisonDamage;
            }
        }

        public ClassStat PercentRadiationDamage
        {
            get
            {
                return this.percentRadiationDamage;
            }
        }

        public ClassStat PercentEnergyDamage
        {
            get
            {
                return this.percentEnergyDamage;
            }
        }

        public ClassStat PercentChemicalDamage
        {
            get
            {
                return this.percentChemicalDamage;
            }
        }

        public ClassStat TotalDamage
        {
            get
            {
                return this.totalDamage;
            }
        }

        public ClassStat TrackProjectileDamage
        {
            get
            {
                return this.trackProjectileDamage;
            }
        }

        public ClassStat TrackMeleeDamage
        {
            get
            {
                return this.trackMeleeDamage;
            }
        }

        public ClassStat TrackEnergyDamage
        {
            get
            {
                return this.trackEnergyDamage;
            }
        }

        public ClassStat TrackChemicalDamage
        {
            get
            {
                return this.trackChemicalDamage;
            }
        }

        public ClassStat TrackRadiationDamage
        {
            get
            {
                return this.trackRadiationDamage;
            }
        }

        public ClassStat TrackPoisonDamage
        {
            get
            {
                return this.trackPoisonDamage;
            }
        }

        public ClassStat TrackColdDamage
        {
            get
            {
                return this.trackColdDamage;
            }
        }

        public ClassStat TrackFireDamage
        {
            get
            {
                return this.trackFireDamage;
            }
        }

        public ClassStat NpcSpellArg1
        {
            get
            {
                return this.npcSpellArg1;
            }
        }

        public ClassStat NpcSpellRet1
        {
            get
            {
                return this.npcSpellRet1;
            }
        }

        public ClassStat CityInstance
        {
            get
            {
                return this.cityInstance;
            }
        }

        public ClassStat DistanceToSpawnpoint
        {
            get
            {
                return this.distanceToSpawnpoint;
            }
        }

        public ClassStat CityTerminalRechargePercent
        {
            get
            {
                return this.cityTerminalRechargePercent;
            }
        }

        public ClassStat UnreadMailCount
        {
            get
            {
                return this.unreadMailCount;
            }
        }

        public ClassStat LastMailCheckTime
        {
            get
            {
                return this.lastMailCheckTime;
            }
        }

        public ClassStat AdvantageHash1
        {
            get
            {
                return this.advantageHash1;
            }
        }

        public ClassStat AdvantageHash2
        {
            get
            {
                return this.advantageHash2;
            }
        }

        public ClassStat AdvantageHash3
        {
            get
            {
                return this.advantageHash3;
            }
        }

        public ClassStat AdvantageHash4
        {
            get
            {
                return this.advantageHash4;
            }
        }

        public ClassStat AdvantageHash5
        {
            get
            {
                return this.advantageHash5;
            }
        }

        public ClassStat ShopIndex
        {
            get
            {
                return this.shopIndex;
            }
        }

        public ClassStat ShopID
        {
            get
            {
                return this.shopID;
            }
        }

        public ClassStat IsVehicle
        {
            get
            {
                return this.isVehicle;
            }
        }

        public ClassStat DamageToNano
        {
            get
            {
                return this.damageToNano;
            }
        }

        public ClassStat AccountFlags
        {
            get
            {
                return this.accountFlags;
            }
        }

        public ClassStat DamageToNanoMultiplier
        {
            get
            {
                return this.damageToNanoMultiplier;
            }
        }

        public ClassStat MechData
        {
            get
            {
                return this.mechData;
            }
        }

        public ClassStat VehicleAC
        {
            get
            {
                return this.vehicleAC;
            }
        }

        public ClassStat VehicleDamage
        {
            get
            {
                return this.vehicleDamage;
            }
        }

        public ClassStat VehicleHealth
        {
            get
            {
                return this.vehicleHealth;
            }
        }

        public ClassStat VehicleSpeed
        {
            get
            {
                return this.vehicleSpeed;
            }
        }

        public ClassStat BattlestationSide
        {
            get
            {
                return this.battlestationSide;
            }
        }

        public ClassStat VictoryPoints
        {
            get
            {
                return this.victoryPoints;
            }
        }

        public ClassStat BattlestationRep
        {
            get
            {
                return this.battlestationRep;
            }
        }

        public ClassStat PetState
        {
            get
            {
                return this.petState;
            }
        }

        public ClassStat PaidPoints
        {
            get
            {
                return this.paidPoints;
            }
        }

        public ClassStat VisualFlags
        {
            get
            {
                return this.visualFlags;
            }
        }

        public ClassStat PvpDuelKills
        {
            get
            {
                return this.pvpDuelKills;
            }
        }

        public ClassStat PvpDuelDeaths
        {
            get
            {
                return this.pvpDuelDeaths;
            }
        }

        public ClassStat PvpProfessionDuelKills
        {
            get
            {
                return this.pvpProfessionDuelKills;
            }
        }

        public ClassStat PvpProfessionDuelDeaths
        {
            get
            {
                return this.pvpProfessionDuelDeaths;
            }
        }

        public ClassStat PvpRankedSoloKills
        {
            get
            {
                return this.pvpRankedSoloKills;
            }
        }

        public ClassStat PvpRankedSoloDeaths
        {
            get
            {
                return this.pvpRankedSoloDeaths;
            }
        }

        public ClassStat PvpRankedTeamKills
        {
            get
            {
                return this.pvpRankedTeamKills;
            }
        }

        public ClassStat PvpRankedTeamDeaths
        {
            get
            {
                return this.pvpRankedTeamDeaths;
            }
        }

        public ClassStat PvpSoloScore
        {
            get
            {
                return this.pvpSoloScore;
            }
        }

        public ClassStat PvpTeamScore
        {
            get
            {
                return this.pvpTeamScore;
            }
        }

        public ClassStat PvpDuelScore
        {
            get
            {
                return this.pvpDuelScore;
            }
        }

        public ClassStat AcgItemSeed
        {
            get
            {
                return this.acgItemSeed;
            }
        }

        public ClassStat AcgItemLevel
        {
            get
            {
                return this.acgItemLevel;
            }
        }

        public ClassStat AcgItemTemplateID
        {
            get
            {
                return this.acgItemTemplateID;
            }
        }

        public ClassStat AcgItemTemplateID2
        {
            get
            {
                return this.acgItemTemplateID2;
            }
        }

        public ClassStat AcgItemCategoryID
        {
            get
            {
                return this.acgItemCategoryID;
            }
        }

        public ClassStat HasKnuBotData
        {
            get
            {
                return this.hasKnuBotData;
            }
        }

        public ClassStat QuestBoothDifficulty
        {
            get
            {
                return this.questBoothDifficulty;
            }
        }

        public ClassStat QuestAsMinimumRange
        {
            get
            {
                return this.questAsMinimumRange;
            }
        }

        public ClassStat QuestAsMaximumRange
        {
            get
            {
                return this.questAsMaximumRange;
            }
        }

        public ClassStat VisualLodLevel
        {
            get
            {
                return this.visualLodLevel;
            }
        }

        public ClassStat TargetDistanceChange
        {
            get
            {
                return this.targetDistanceChange;
            }
        }

        public ClassStat TideRequiredDynelID
        {
            get
            {
                return this.tideRequiredDynelID;
            }
        }

        public ClassStat StreamCheckMagic
        {
            get
            {
                return this.streamCheckMagic;
            }
        }

        public ClassStat Type
        {
            get
            {
                return this.type;
            }
        }

        public ClassStat Instance
        {
            get
            {
                return this.instance;
            }
        }

        public ClassStat WeaponsStyle
        {
            get
            {
                return this.weaponsStyle;
            }
        }

        public ClassStat ShoulderMeshRight
        {
            get
            {
                return this.shoulderMeshRight;
            }
        }

        public ClassStat ShoulderMeshLeft
        {
            get
            {
                return this.shoulderMeshLeft;
            }
        }

        public ClassStat WeaponMeshRight
        {
            get
            {
                return this.weaponMeshRight;
            }
        }

        public ClassStat WeaponMeshLeft
        {
            get
            {
                return this.weaponMeshLeft;
            }
        }

        public ClassStat OverrideTextureHead
        {
            get
            {
                return this.overrideTextureHead;
            }
        }

        public ClassStat OverrideTextureWeaponRight
        {
            get
            {
                return this.overrideTextureWeaponRight;
            }
        }

        public ClassStat OverrideTextureWeaponLeft
        {
            get
            {
                return this.overrideTextureWeaponLeft;
            }
        }

        public ClassStat OverrideTextureShoulderpadRight
        {
            get
            {
                return this.overrideTextureShoulderpadRight;
            }
        }

        public ClassStat OverrideTextureShoulderpadLeft
        {
            get
            {
                return this.overrideTextureShoulderpadLeft;
            }
        }

        public ClassStat OverrideTextureBack
        {
            get
            {
                return this.overrideTextureBack;
            }
        }

        public ClassStat OverrideTextureAttractor
        {
            get
            {
                return this.overrideTextureAttractor;
            }
        }

        public ClassStat WeaponStyleLeft
        {
            get
            {
                return this.weaponStyleLeft;
            }
        }

        public ClassStat WeaponStyleRight
        {
            get
            {
                return this.weaponStyleRight;
            }
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
                    this.strength.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 2] > 0)
                {
                    this.stamina.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 3] > 0)
                {
                    this.sense.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 4] > 0)
                {
                    this.agility.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 5] > 0)
                {
                    this.intelligence.Affects.Add(skillnum);
                }
                if (SkillTrickleTable.table[c, 6] > 0)
                {
                    this.psychic.Affects.Add(skillnum);
                }
            }
        }
        #endregion

        #region Get Stat object by number
        public ClassStat GetStatbyNumber(int number)
        {
            foreach (ClassStat c in this.All)
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
        public void Send(object sender, ClassStat.StatChangedEventArgs e)
        {
            if (!((Character)((ClassStat)sender).Parent).dontdotimers)
            {
                if (e.Stat.SendBaseValue)
                {
                    Stat.Send(((Character)e.Stat.Parent).client, e.Stat.StatNumber, e.NewValue, e.AnnounceToPlayfield);
                }
                else
                {
                    Stat.Send(((Character)e.Stat.Parent).client, e.Stat.StatNumber, e.NewValue, e.AnnounceToPlayfield);
                }
                e.Stat.Changed = false;
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
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }
                return c.Value;
            }
            throw new StatDoesNotExistException("Stat " + number + " does not exist.\r\nMethod: Get");
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="number">Stat number</param>
        /// <param name="newValue">Stat's new value</param>
        public void Set(int number, uint newValue)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }
                c.Set(newValue);
                return;
            }
            throw new StatDoesNotExistException(
                "Stat " + number + " does not exist.\r\nValue: " + newValue + "\r\nMethod: Set");
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
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                return c.Value;
            }
            throw new StatDoesNotExistException("Stat " + name + " does not exist.\r\nMethod: Get");
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="name">Stat's name</param>
        /// <param name="newValue">Stat's new value</param>
        public void Set(string name, uint newValue)
        {
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                c.Set(newValue);
                return;
            }
            throw new StatDoesNotExistException(
                "Stat " + name + " does not exist.\r\nValue: " + newValue + "\r\nMethod: GetID");
        }

        public int GetID(string name)
        {
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }
                return c.StatNumber;
            }
            throw new StatDoesNotExistException("Stat " + name + " does not exist.\r\nMethod: GetID");
        }
        #endregion

        #region Read all Stats from SQL
        /// <summary>
        /// Read all stats from SQL
        /// </summary>
        public void ReadStatsfromSql()
        {
            SqlWrapper sql = new SqlWrapper();
            DataTable dt =
                sql.ReadDatatable(
                    "SELECT Stat,Value FROM " + this.flags.Parent.getSQLTablefromDynelType() + "_stats WHERE ID="
                    + this.flags.Parent.ID); // Using Flags to address parent object
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
            foreach (ClassStat c in this.All)
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
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatModifier;
            }
            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetModifier");
        }

        public void SetModifier(int stat, int value)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.StatModifier = value;
                return;
            }
            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetModifier");
        }
        #endregion

        #region Get/Set Stat Percentage Modifier
        public int GetPercentageModifier(int stat)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatPercentageModifier;
            }
            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetPercentageModifier");
        }

        public void SetPercentageModifier(int stat, int value)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.StatPercentageModifier = value;
                return;
            }
            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetPercentageModifier");
        }
        #endregion

        #region Get/Set Stat Base Value
        public uint GetBaseValue(int stat)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                return c.StatBaseValue;
            }
            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetBaseValue");
        }

        public void SetBaseValue(int stat, uint value)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }
                c.Changed = c.StatBaseValue != value;
                c.StatBaseValue = value;
                return;
            }
            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetBaseValue");
        }
        #endregion

        #region Clear Modifiers for recalculation
        public void ClearModifiers()
        {
            foreach (ClassStat c in this.All)
            {
                c.StatModifier = 0;
                c.StatPercentageModifier = 100;
                c.Trickle = 0;
            }
        }
        #endregion

        #region Set Trickle values
        public void SetTrickle(int statId, int value)
        {
            foreach (ClassStat c in this.All)
            {
                if (c.StatNumber != statId)
                {
                    continue;
                }
                c.Trickle = value;
                return;
            }
            throw new StatDoesNotExistException("Stat " + statId + " does not exist.\r\nMethod: Trickle");
        }
        #endregion

        #region send stat value by ID
        public void Send(Client client, int statId)
        {
            foreach (ClassStat c in this.All)
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
                Stat.Send(client, statId, val, c.AnnounceToPlayfield);
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + statId + " does not exist.\r\nClient: " + client.Character.ID + "\r\nMethod: Send");
        }
        #endregion

        public void ClearChangedFlags()
        {
            foreach (ClassStat cs in this.All)
            {
                cs.Changed = false;
            }
        }
    }
    #endregion
}