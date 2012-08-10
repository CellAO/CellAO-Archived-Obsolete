#region License
/*
Copyright (c) 2005-2011, CellAO Team

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace CreateStatelEvents
{

    public static class common
    {

        public static void execSQL(string cmd)
        {
            string conn = MainWindow.cfg.connectstring;
            MySqlConnection sqlConn = new MySqlConnection(conn);
            MySqlCommand command = sqlConn.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = cmd;
            try
            {
                sqlConn.Open();
                Reader = command.ExecuteReader();
                Reader.Close();
            }
            catch
            {
                Console.WriteLine("Error Connecting to Database\n");
            }
            sqlConn.Close();
        }
public static DataSet readSQL(string cmd)
        {
            string conn = MainWindow.cfg.connectstring;
            MySqlConnection sqlConn = new MySqlConnection(conn);
            MySqlCommand command = sqlConn.CreateCommand();
            DataSet itemdata = new DataSet();
            command.CommandText = cmd;
            sqlConn.Open();
            MySqlDataAdapter mda = new MySqlDataAdapter(command);
            mda.Fill(itemdata);
            sqlConn.Close();
            return itemdata;
        }

        public static string cap(string g)
        {
            return g.Substring(0, 1).ToUpper() + g.Substring(1);
        }
    }
    public static class NamesandNumbers
    {
        public static Dictionary<int, string> Stats = new Dictionary<int, string>();
        public static Dictionary<int, string> Operators = new Dictionary<int, string>();
        public static Dictionary<int, string> Targets = new Dictionary<int, string>();
        public static Dictionary<int,string> Events = new Dictionary<int,string>();
        public static Dictionary<int, string> Functions = new Dictionary<int, string>();

        public static Dictionary<int, FunctionTemplates> Functiontemplates = new Dictionary<int, FunctionTemplates>();
        
        public static void set()
        {

            #region Events
            Events.Add(0, common.cap("onuse"));
            Events.Add(1, common.cap("onrepair"));
            Events.Add(2, common.cap("onwield"));
            Events.Add(3, common.cap("ontargetinvicinity"));
            Events.Add(4, common.cap("onuseitemon"));
            Events.Add(5, common.cap("onhit"));
            Events.Add(7, common.cap("oncreate"));
            Events.Add(8, common.cap("oneffects"));
            Events.Add(9, common.cap("onrun"));
            Events.Add(10, common.cap("onactivate"));
            Events.Add(12, common.cap("onstarteffect"));
            Events.Add(13, common.cap("onendeffect"));
            Events.Add(14, common.cap("onwear"));
            Events.Add(15, common.cap("onusefailed"));
            Events.Add(16, common.cap("onenter"));
            Events.Add(18, common.cap("onopen"));
            Events.Add(19, common.cap("onclose"));
            Events.Add(20, common.cap("onterminate"));
            Events.Add(23, common.cap("onendcollide"));
            Events.Add(24, common.cap("onfriendlyinvicinity"));
            Events.Add(25, common.cap("onenemyinvicinity"));
            Events.Add(26, common.cap("personalmodifier"));
            Events.Add(27, common.cap("onfailure"));
            Events.Add(37, common.cap("ontrade"));
            #endregion

            #region Functions
            Functions.Add(53002, common.cap("hit"));
            Functions.Add(53003, common.cap("animeffect"));
            Functions.Add(53004, common.cap("mesh"));
            Functions.Add(53005, common.cap("creation"));
            Functions.Add(53006, common.cap("poison"));
            Functions.Add(53007, common.cap("radius"));
            Functions.Add(53008, common.cap("remove"));
            Functions.Add(53009, common.cap("texteffect"));
            Functions.Add(53010, common.cap("visualeffect"));
            Functions.Add(53011, common.cap("audioeffect"));
            Functions.Add(53012, common.cap("skill"));
            Functions.Add(53013, common.cap("poisonremove"));
            Functions.Add(53014, common.cap("timedeffect"));
            Functions.Add(53015, common.cap("criteria"));
            Functions.Add(53016, common.cap("teleport"));
            Functions.Add(53017, common.cap("playmusic"));
            Functions.Add(53018, common.cap("stopmusic"));
            Functions.Add(53019, common.cap("uploadnano"));
            Functions.Add(53023, common.cap("catmesh"));
            Functions.Add(53024, common.cap("expression"));
            Functions.Add(53025, common.cap("anim"));
            Functions.Add(53026, common.cap("set"));
            Functions.Add(53027, common.cap("createstat"));
            Functions.Add(53028, common.cap("addskill"));
            Functions.Add(53029, common.cap("adddifficulty"));
            Functions.Add(53030, common.cap("gfxeffect"));
            Functions.Add(53031, common.cap("itemanimeffect"));
            Functions.Add(53032, common.cap("savechar"));
            Functions.Add(53033, common.cap("lockskill"));
            Functions.Add(53034, common.cap("directitemanimeffect"));
            Functions.Add(53035, common.cap("headmesh"));
            Functions.Add(53036, common.cap("hairmesh"));
            Functions.Add(53037, common.cap("backmesh"));
            Functions.Add(53038, common.cap("shouldermesh"));
            Functions.Add(53039, common.cap("texture"));
            Functions.Add(53040, common.cap("starteffect"));
            Functions.Add(53041, common.cap("endeffect"));
            Functions.Add(53042, common.cap("weaponeffectcolor"));
            Functions.Add(53043, common.cap("addshopitem"));
            Functions.Add(53044, common.cap("systemtext"));
            Functions.Add(53045, common.cap("modify"));
            Functions.Add(53047, common.cap("animaction"));
            Functions.Add(53048, common.cap("name"));
            Functions.Add(53049, common.cap("spawnmonster"));
            Functions.Add(53050, common.cap("removebuffs"));
            Functions.Add(53051, common.cap("castnano"));
            Functions.Add(53052, common.cap("strtexture"));
            Functions.Add(53053, common.cap("strmesh"));
            Functions.Add(53054, common.cap("changebodymesh"));
            Functions.Add(53055, common.cap("attractormesh"));
            Functions.Add(53056, common.cap("waypoint"));
            Functions.Add(53057, common.cap("headtext"));
            Functions.Add(53058, common.cap("setstate"));
            Functions.Add(53059, common.cap("lineteleport"));
            Functions.Add(53060, common.cap("monstershape"));
            Functions.Add(53061, common.cap("addshopitem2"));
            Functions.Add(53062, common.cap("npcselecttarget"));
            Functions.Add(53063, common.cap("spawnmonster2"));
            Functions.Add(53064, common.cap("spawnitem"));
            Functions.Add(53065, common.cap("attractoreffect"));
            Functions.Add(53066, common.cap("teamcastnano"));
            Functions.Add(53067, common.cap("changeactionrestriction"));
            Functions.Add(53068, common.cap("restrictaction"));
            Functions.Add(53069, common.cap("nexthead"));
            Functions.Add(53070, common.cap("prevhead"));
            Functions.Add(53073, common.cap("areahit"));
            Functions.Add(53074, common.cap("makevendorshop"));
            Functions.Add(53075, common.cap("attractoreffect1"));
            Functions.Add(53076, common.cap("attractoreffect2"));
            Functions.Add(53077, common.cap("npcfightselected"));
            Functions.Add(53078, common.cap("npcsocialanim"));
            Functions.Add(53079, common.cap("changeeffect"));
            Functions.Add(53080, common.cap("npcturntotarget"));
            Functions.Add(53081, common.cap("npchatelisttarget"));
            Functions.Add(53082, common.cap("teleportproxy"));
            Functions.Add(53083, common.cap("teleportproxy2"));
            Functions.Add(53086, common.cap("refreshmodel"));
            Functions.Add(53087, common.cap("areacastnano"));
            Functions.Add(53089, common.cap("caststunnano"));
            Functions.Add(53090, common.cap("npcgettargethatelist"));
            Functions.Add(53091, common.cap("npcsetmaster"));
            Functions.Add(53092, common.cap("openbank"));
            Functions.Add(53095, common.cap("npcfollowselected"));
            Functions.Add(53096, common.cap("npcmoveforward"));
            Functions.Add(53097, common.cap("npcsendplaysync"));
            Functions.Add(53098, common.cap("npctrygroupform"));
            Functions.Add(53100, common.cap("equipmonsterweapon"));
            Functions.Add(53102, common.cap("npcapplynanoformula"));
            Functions.Add(53103, common.cap("npcsendcommand"));
            Functions.Add(53104, common.cap("npcsayrobotspeech"));
            Functions.Add(53105, common.cap("removenanoeffects"));
            Functions.Add(53107, common.cap("npcpushscript"));
            Functions.Add(53108, common.cap("npcpopscript"));
            Functions.Add(53109, common.cap("enterapartment"));
            Functions.Add(53110, common.cap("changevariable"));
            Functions.Add(53111, common.cap("unknown 2"));
            Functions.Add(53113, common.cap("npcstartsurrender"));
            Functions.Add(53114, common.cap("npcstopsurrender"));
            Functions.Add(53115, common.cap("inputbox"));
            Functions.Add(53116, common.cap("npcstopmoving"));
            Functions.Add(53117, common.cap("tauntnpc"));
            Functions.Add(53118, common.cap("pacify"));
            Functions.Add(53119, common.cap("npcclearsignal"));
            Functions.Add(53120, common.cap("npccallforhelp"));
            Functions.Add(53121, common.cap("fear"));
            Functions.Add(53122, common.cap("stun"));
            Functions.Add(53124, common.cap("rndspawnitem"));
            Functions.Add(53125, common.cap("rndspawnmonster"));
            Functions.Add(53126, common.cap("npcwipehatelist"));
            Functions.Add(53127, common.cap("charmnpc"));
            Functions.Add(53128, common.cap("daze"));
            Functions.Add(53129, common.cap("npccreatepet"));
            Functions.Add(53130, common.cap("destroyitem"));
            Functions.Add(53131, common.cap("npckilltarget"));
            Functions.Add(53132, common.cap("generatename"));
            Functions.Add(53133, common.cap("setgovernmenttype"));
            Functions.Add(53134, common.cap("text"));
            Functions.Add(53137, common.cap("createapartment"));
            Functions.Add(53138, common.cap("canfly"));
            Functions.Add(53139, common.cap("setflag"));
            Functions.Add(53140, common.cap("clearflag"));
            Functions.Add(53141, common.cap("toggleflag"));
            Functions.Add(53142, common.cap("unknown function"));
            Functions.Add(53143, common.cap("npcteleporttospawnpoint"));
            Functions.Add(53144, common.cap("gotolastsavepoint"));
            Functions.Add(53145, common.cap("npcfakeattackontarget"));
            Functions.Add(53146, common.cap("npcenabledieofboredom"));
            Functions.Add(53147, common.cap("npchatelisttargetaggroers"));
            Functions.Add(53148, common.cap("npcdisablemovement"));
            Functions.Add(53149, common.cap("areatrigger"));
            Functions.Add(53153, common.cap("mezz"));
            Functions.Add(53154, common.cap("summonplayer"));
            Functions.Add(53155, common.cap("summonteammates"));
            Functions.Add(53159, common.cap("remoteareatrigger"));
            Functions.Add(53160, common.cap("clone"));
            Functions.Add(53161, common.cap("npcclonetarget"));
            Functions.Add(53162, common.cap("resistnanostrain"));
            Functions.Add(53163, common.cap("npcsummonenemy"));
            Functions.Add(53164, common.cap("savehere"));
            Functions.Add(53165, common.cap("proxyteleport_withpethandling"));
            Functions.Add(53166, common.cap("combonamegen"));
            Functions.Add(53167, common.cap("summonpet"));
            Functions.Add(53168, common.cap("opennpcdialog"));
            Functions.Add(53169, common.cap("closenpcdialog"));
            Functions.Add(53170, common.cap("npcenablegroundtoaircombat"));
            Functions.Add(53171, common.cap("npcsetstuckdetectscheme"));
            Functions.Add(53172, common.cap("npcenablepvprules"));
            Functions.Add(53173, common.cap("landcontrolcreate"));
            Functions.Add(53174, common.cap("removetrigger"));
            Functions.Add(53175, common.cap("scalingmodify"));
            Functions.Add(53176, common.cap("organizationgrid"));
            Functions.Add(53177, common.cap("reducenanostrainduration"));
            Functions.Add(53178, common.cap("disabledefenseshield"));
            Functions.Add(53179, common.cap("npctogglefightmoderegenrate"));
            Functions.Add(53180, common.cap("tracer"));
            Functions.Add(53181, common.cap("summonpets"));
            Functions.Add(53182, common.cap("addaction"));
            Functions.Add(53183, common.cap("npctogglefov"));
            Functions.Add(53184, common.cap("modifypercentage"));
            Functions.Add(53185, common.cap("drainhit"));
            Functions.Add(53187, common.cap("lockperk"));
            Functions.Add(53188, common.cap("dialogfeedback"));
            Functions.Add(53189, common.cap("faction"));
            Functions.Add(53190, common.cap("npcsetsneakmode"));
            Functions.Add(53191, common.cap("npcmovementaction"));
            Functions.Add(53192, common.cap("spawnmonsterrot"));
            Functions.Add(53193, common.cap("polymorphattack"));
            Functions.Add(53194, common.cap("npcusespecialattackitem"));
            Functions.Add(53195, common.cap("npcfreezehatelist"));
            Functions.Add(53196, common.cap("specialhit"));
            Functions.Add(53197, common.cap("npcsetconfigstats"));
            Functions.Add(53198, common.cap("npcsetmovetotarget"));
            Functions.Add(53199, common.cap("npcsetwanderingmode"));
            Functions.Add(53200, common.cap("unknown 3"));
            Functions.Add(53201, common.cap("removenano"));
            Functions.Add(53203, common.cap("npcuniqueplayersinhatelist"));
            Functions.Add(53204, common.cap("attractorgfxeffect"));
            Functions.Add(53206, common.cap("castnanoifpossible"));
            Functions.Add(53208, common.cap("setanchor"));
            Functions.Add(53209, common.cap("recalltoanchor"));
            Functions.Add(53210, common.cap("talk"));
            Functions.Add(53211, common.cap("setscriptconfig"));
            Functions.Add(53212, common.cap("castnanoifpossibleonfighttarget"));
            Functions.Add(53213, common.cap("controlhate"));
            Functions.Add(53214, common.cap("npcsendpetstatus"));
            Functions.Add(53215, common.cap("npccastnanoifpossible"));
            Functions.Add(53216, common.cap("npccastnanoifpossibleonfighttarget"));
            Functions.Add(53217, common.cap("npctargethasitem"));
            Functions.Add(53218, common.cap("cityhouseenter"));
            Functions.Add(53219, common.cap("npcstoppetduel"));
            Functions.Add(53220, common.cap("delayedspawnnpc"));
            Functions.Add(53221, common.cap("runscript"));
            Functions.Add(53222, common.cap("addbattlestationqueue"));
            Functions.Add(53223, common.cap("registercontrolpoint"));
            Functions.Add(53224, common.cap("adddefproc"));
            Functions.Add(53225, common.cap("destroyallhumans"));
            Functions.Add(53226, common.cap("spawnquest"));
            Functions.Add(53227, common.cap("addoffproc"));
            Functions.Add(53228, common.cap("playfieldnano"));
            Functions.Add(53229, common.cap("solvequest"));
            Functions.Add(53230, common.cap("knockback"));
            Functions.Add(53231, common.cap("instancelock"));
            Functions.Add(53232, common.cap("mindcontrol"));
            Functions.Add(53233, common.cap("instancedplayercity"));
            Functions.Add(53234, common.cap("resetallperks"));
            Functions.Add(53235, common.cap("createcityguestkey"));
            Functions.Add(53236, common.cap("removenanostrain"));
            #endregion

            #region Stats
            Stats.Add(0, common.cap("Flags"));
            Stats.Add(1, common.cap("Life"));
            Stats.Add(2, common.cap("VolumeMass"));
            Stats.Add(3, common.cap("AttackSpeed"));
            Stats.Add(4, common.cap("Breed"));
            Stats.Add(5, common.cap("Clan"));
            Stats.Add(6, common.cap("Team"));
            Stats.Add(7, common.cap("State"));
            Stats.Add(8, common.cap("TimeExist"));
            Stats.Add(9, common.cap("MapFlags"));
            Stats.Add(10, common.cap("ProfessionLevel"));
            Stats.Add(11, common.cap("PreviousHealth"));
            Stats.Add(12, common.cap("Mesh"));
            Stats.Add(13, common.cap("Anim"));
            Stats.Add(14, common.cap("Name"));
            Stats.Add(15, common.cap("Info"));
            Stats.Add(16, common.cap("Strength"));
            Stats.Add(17, common.cap("Agility"));
            Stats.Add(18, common.cap("Stamina"));
            Stats.Add(19, common.cap("Intelligence"));
            Stats.Add(20, common.cap("Sense"));
            Stats.Add(21, common.cap("Psychic"));
            Stats.Add(22, common.cap("AMS"));
            Stats.Add(23, common.cap("StaticInstance"));
            Stats.Add(24, common.cap("MaxMass"));
            Stats.Add(25, common.cap("StaticType"));
            Stats.Add(26, common.cap("Energy"));
            Stats.Add(27, common.cap("Health"));
            Stats.Add(28, common.cap("Height"));
            Stats.Add(29, common.cap("DMS"));
            Stats.Add(30, common.cap("Can"));
            Stats.Add(31, common.cap("Face"));
            Stats.Add(32, common.cap("HairMesh"));
            Stats.Add(33, common.cap("Side"));
            Stats.Add(34, common.cap("DeadTimer"));
            Stats.Add(35, common.cap("AccessCount"));
            Stats.Add(36, common.cap("AttackCount"));
            Stats.Add(37, common.cap("TitleLevel"));
            Stats.Add(38, common.cap("BackMesh"));
            Stats.Add(39, common.cap("ShoulderMesh"));
            Stats.Add(40, common.cap("AlienXP"));
            Stats.Add(41, common.cap("FabricType"));
            Stats.Add(42, common.cap("CATMesh"));
            Stats.Add(43, common.cap("ParentType"));
            Stats.Add(44, common.cap("ParentInstance"));
            Stats.Add(45, common.cap("BeltSlots"));
            Stats.Add(46, common.cap("BandolierSlots"));
            Stats.Add(47, common.cap("Fatness"));
            Stats.Add(48, common.cap("ClanLevel"));
            Stats.Add(49, common.cap("InsuranceTime"));
            Stats.Add(50, common.cap("InventoryTimeout"));
            Stats.Add(51, common.cap("AggDef"));
            Stats.Add(52, common.cap("XP"));
            Stats.Add(53, common.cap("IP"));
            Stats.Add(54, common.cap("Level"));
            Stats.Add(55, common.cap("InventoryId"));
            Stats.Add(56, common.cap("TimeSinceCreation"));
            Stats.Add(57, common.cap("LastXP"));
            Stats.Add(58, common.cap("Age"));
            Stats.Add(59, common.cap("Sex"));
            Stats.Add(60, common.cap("Profession"));
            Stats.Add(61, common.cap("Cash"));
            Stats.Add(62, common.cap("Alignment"));
            Stats.Add(63, common.cap("Attitude"));
            Stats.Add(64, common.cap("HeadMesh"));
            Stats.Add(65, common.cap("MissionBits5"));
            Stats.Add(66, common.cap("MissionBits6"));
            Stats.Add(67, common.cap("MissionBits7"));
            Stats.Add(68, common.cap("VeteranPoints"));
            Stats.Add(69, common.cap("MonthsPaid"));
            Stats.Add(70, common.cap("SpeedPenalty"));
            Stats.Add(71, common.cap("TotalMass"));
            Stats.Add(72, common.cap("ItemType"));
            Stats.Add(73, common.cap("RepairDifficulty"));
            Stats.Add(74, common.cap("Price"));
            Stats.Add(75, common.cap("MetaType"));
            Stats.Add(76, common.cap("ItemClass"));
            Stats.Add(77, common.cap("RepairSkill"));
            Stats.Add(78, common.cap("CurrentMass"));
            Stats.Add(79, common.cap("Icon"));
            Stats.Add(80, common.cap("PrimaryItemType"));
            Stats.Add(81, common.cap("PrimaryItemInstance"));
            Stats.Add(82, common.cap("SecondaryItemType"));
            Stats.Add(83, common.cap("SecondaryItemInstance"));
            Stats.Add(84, common.cap("UserType"));
            Stats.Add(85, common.cap("UserInstance"));
            Stats.Add(86, common.cap("AreaType"));
            Stats.Add(87, common.cap("AreaInstance"));
            Stats.Add(88, common.cap("DefaultPos"));
            Stats.Add(89, common.cap("Race"));
            Stats.Add(90, common.cap("ProjectileAC"));
            Stats.Add(91, common.cap("MeleeAC"));
            Stats.Add(92, common.cap("EnergyAC"));
            Stats.Add(93, common.cap("ChemicalAC"));
            Stats.Add(94, common.cap("RadiationAC"));
            Stats.Add(95, common.cap("ColdAC"));
            Stats.Add(96, common.cap("PoisonAC"));
            Stats.Add(97, common.cap("FireAC"));
            Stats.Add(98, common.cap("StateAction"));
            Stats.Add(99, common.cap("ItemAnim"));
            Stats.Add(100, common.cap("MartialArts"));
            Stats.Add(101, common.cap("MeleeMultiple"));
            Stats.Add(102, common.cap("1hBluntWeapons"));
            Stats.Add(103, common.cap("1hEdgedWeapon"));
            Stats.Add(104, common.cap("MeleeEnergyWeapon"));
            Stats.Add(105, common.cap("2hEdgedWeapons"));
            Stats.Add(106, common.cap("Piercing"));
            Stats.Add(107, common.cap("2hBluntWeapons"));
            Stats.Add(108, common.cap("ThrowingKnife"));
            Stats.Add(109, common.cap("Grenade"));
            Stats.Add(110, common.cap("ThrownGrapplingWeapons"));
            Stats.Add(111, common.cap("Bow"));
            Stats.Add(112, common.cap("Pistol"));
            Stats.Add(113, common.cap("Rifle"));
            Stats.Add(114, common.cap("SubMachineGun"));
            Stats.Add(115, common.cap("Shotgun"));
            Stats.Add(116, common.cap("AssaultRifle"));
            Stats.Add(117, common.cap("DriveWater"));
            Stats.Add(118, common.cap("CloseCombatInitiative"));
            Stats.Add(119, common.cap("DistanceWeaponInitiative"));
            Stats.Add(120, common.cap("PhysicalProwessInitiative"));
            Stats.Add(121, common.cap("BowSpecialAttack"));
            Stats.Add(122, common.cap("SenseImprovement"));
            Stats.Add(123, common.cap("FirstAid"));
            Stats.Add(124, common.cap("Treatment"));
            Stats.Add(125, common.cap("MechanicalEngineering"));
            Stats.Add(126, common.cap("ElectricalEngineering"));
            Stats.Add(127, common.cap("MaterialMetamorphose"));
            Stats.Add(128, common.cap("BiologicalMetamorphose"));
            Stats.Add(129, common.cap("PsychologicalModification"));
            Stats.Add(130, common.cap("MaterialCreation"));
            Stats.Add(131, common.cap("MaterialLocation"));
            Stats.Add(132, common.cap("NanoEnergyPool"));
            Stats.Add(133, common.cap("LR_EnergyWeapon"));
            Stats.Add(134, common.cap("LR_MultipleWeapon"));
            Stats.Add(135, common.cap("DisarmTrap"));
            Stats.Add(136, common.cap("Perception"));
            Stats.Add(137, common.cap("Adventuring"));
            Stats.Add(138, common.cap("Swim"));
            Stats.Add(139, common.cap("DriveAir"));
            Stats.Add(140, common.cap("MapNavigation"));
            Stats.Add(141, common.cap("Tutoring"));
            Stats.Add(142, common.cap("Brawl"));
            Stats.Add(143, common.cap("Riposte"));
            Stats.Add(144, common.cap("Dimach"));
            Stats.Add(145, common.cap("Parry"));
            Stats.Add(146, common.cap("SneakAttack"));
            Stats.Add(147, common.cap("FastAttack"));
            Stats.Add(148, common.cap("Burst"));
            Stats.Add(149, common.cap("NanoProwessInitiative"));
            Stats.Add(150, common.cap("FlingShot"));
            Stats.Add(151, common.cap("AimedShot"));
            Stats.Add(152, common.cap("BodyDevelopment"));
            Stats.Add(153, common.cap("Duck"));
            Stats.Add(154, common.cap("Dodge"));
            Stats.Add(155, common.cap("Evade"));
            Stats.Add(156, common.cap("RunSpeed"));
            Stats.Add(157, common.cap("FieldQuantumPhysics"));
            Stats.Add(158, common.cap("WeaponSmithing"));
            Stats.Add(159, common.cap("Pharmaceuticals"));
            Stats.Add(160, common.cap("NanoProgramming"));
            Stats.Add(161, common.cap("ComputerLiteracy"));
            Stats.Add(162, common.cap("Psychology"));
            Stats.Add(163, common.cap("Chemistry"));
            Stats.Add(164, common.cap("Concealment"));
            Stats.Add(165, common.cap("BreakingEntry"));
            Stats.Add(166, common.cap("DriveGround"));
            Stats.Add(167, common.cap("FullAuto"));
            Stats.Add(168, common.cap("NanoAC"));
            Stats.Add(169, common.cap("AlienLevel"));
            Stats.Add(170, common.cap("HealthChangeBest"));
            Stats.Add(171, common.cap("HealthChangeWorst"));
            Stats.Add(172, common.cap("HealthChange"));
            Stats.Add(173, common.cap("CurrentMovementMode"));
            Stats.Add(174, common.cap("PrevMovementMode"));
            Stats.Add(175, common.cap("AutoLockTimeDefault"));
            Stats.Add(176, common.cap("AutoUnlockTimeDefault"));
            Stats.Add(177, common.cap("MoreFlags"));
            Stats.Add(178, common.cap("AlienNextXP"));
            Stats.Add(179, common.cap("NPCFlags"));
            Stats.Add(180, common.cap("CurrentNCU"));
            Stats.Add(181, common.cap("MaxNCU"));
            Stats.Add(182, common.cap("Specialization"));
            Stats.Add(183, common.cap("EffectIcon"));
            Stats.Add(184, common.cap("BuildingType"));
            Stats.Add(185, common.cap("BuildingInstance"));
            Stats.Add(186, common.cap("CardOwnerType"));
            Stats.Add(187, common.cap("CardOwnerInstance"));
            Stats.Add(188, common.cap("BuildingComplexInst"));
            Stats.Add(189, common.cap("ExitInstance"));
            Stats.Add(190, common.cap("NextDoorInBuilding"));
            Stats.Add(191, common.cap("LastConcretePlayfieldInstance"));
            Stats.Add(192, common.cap("ExtenalPlayfieldInstance"));
            Stats.Add(193, common.cap("ExtenalDoorInstance"));
            Stats.Add(194, common.cap("InPlay"));
            Stats.Add(195, common.cap("AccessKey"));
            Stats.Add(196, common.cap("PetMaster"));
            Stats.Add(197, common.cap("OrientationMode"));
            Stats.Add(198, common.cap("SessionTime"));
            Stats.Add(199, common.cap("RP"));
            Stats.Add(200, common.cap("Conformity"));
            Stats.Add(201, common.cap("Aggressiveness"));
            Stats.Add(202, common.cap("Stability"));
            Stats.Add(203, common.cap("Extroverty"));
            Stats.Add(204, common.cap("BreedHostility"));
            Stats.Add(205, common.cap("ReflectProjectileAC"));
            Stats.Add(206, common.cap("ReflectMeleeAC"));
            Stats.Add(207, common.cap("ReflectEnergyAC"));
            Stats.Add(208, common.cap("ReflectChemicalAC"));
            Stats.Add(209, common.cap("WeaponMesh"));
            Stats.Add(210, common.cap("RechargeDelay"));
            Stats.Add(211, common.cap("EquipDelay"));
            Stats.Add(212, common.cap("MaxEnergy"));
            Stats.Add(213, common.cap("TeamSide"));
            Stats.Add(214, common.cap("CurrentNano"));
            Stats.Add(215, common.cap("GmLevel"));
            Stats.Add(216, common.cap("ReflectRadiationAC"));
            Stats.Add(217, common.cap("ReflectColdAC"));
            Stats.Add(218, common.cap("ReflectNanoAC"));
            Stats.Add(219, common.cap("ReflectFireAC"));
            Stats.Add(220, common.cap("CurrBodyLocation"));
            Stats.Add(221, common.cap("MaxNanoEnergy"));
            Stats.Add(222, common.cap("AccumulatedDamage"));
            Stats.Add(223, common.cap("CanChangeClothes"));
            Stats.Add(224, common.cap("Features"));
            Stats.Add(225, common.cap("ReflectPoisonAC"));
            Stats.Add(226, common.cap("ShieldProjectileAC"));
            Stats.Add(227, common.cap("ShieldMeleeAC"));
            Stats.Add(228, common.cap("ShieldEnergyAC"));
            Stats.Add(229, common.cap("ShieldChemicalAC"));
            Stats.Add(230, common.cap("ShieldRadiationAC"));
            Stats.Add(231, common.cap("ShieldColdAC"));
            Stats.Add(232, common.cap("ShieldNanoAC"));
            Stats.Add(233, common.cap("ShieldFireAC"));
            Stats.Add(234, common.cap("ShieldPoisonAC"));
            Stats.Add(235, common.cap("BerserkMode"));
            Stats.Add(236, common.cap("InsurancePercentage"));
            Stats.Add(237, common.cap("ChangeSideCount"));
            Stats.Add(238, common.cap("AbsorbProjectileAC"));
            Stats.Add(239, common.cap("AbsorbMeleeAC"));
            Stats.Add(240, common.cap("AbsorbEnergyAC"));
            Stats.Add(241, common.cap("AbsorbChemicalAC"));
            Stats.Add(242, common.cap("AbsorbRadiationAC"));
            Stats.Add(243, common.cap("AbsorbColdAC"));
            Stats.Add(244, common.cap("AbsorbFireAC"));
            Stats.Add(245, common.cap("AbsorbPoisonAC"));
            Stats.Add(246, common.cap("AbsorbNanoAC"));
            Stats.Add(247, common.cap("TemporarySkillReduction"));
            Stats.Add(248, common.cap("BirthDate"));
            Stats.Add(249, common.cap("LastSaved"));
            Stats.Add(250, common.cap("SoundVolume"));
            Stats.Add(251, common.cap("PetCounter"));
            Stats.Add(252, common.cap("MeetersWalked"));
            Stats.Add(253, common.cap("QuestLevelsSolved"));
            Stats.Add(254, common.cap("MonsterLevelsKilled"));
            Stats.Add(255, common.cap("PvPLevelsKilled"));
            Stats.Add(256, common.cap("MissionBits1"));
            Stats.Add(257, common.cap("MissionBits2"));
            Stats.Add(258, common.cap("AccessGrant"));
            Stats.Add(259, common.cap("DoorFlags"));
            Stats.Add(260, common.cap("ClanHierarchy"));
            Stats.Add(261, common.cap("QuestStat"));
            Stats.Add(262, common.cap("ClientActivated"));
            Stats.Add(263, common.cap("PersonalResearchLevel"));
            Stats.Add(264, common.cap("GlobalResearchLevel"));
            Stats.Add(265, common.cap("PersonalResearchGoal"));
            Stats.Add(266, common.cap("GlobalResearchGoal"));
            Stats.Add(267, common.cap("TurnSpeed"));
            Stats.Add(268, common.cap("LiquidType"));
            Stats.Add(269, common.cap("GatherSound"));
            Stats.Add(270, common.cap("CastSound"));
            Stats.Add(271, common.cap("TravelSound"));
            Stats.Add(272, common.cap("HitSound"));
            Stats.Add(273, common.cap("SecondaryItemTemplate"));
            Stats.Add(274, common.cap("EquippedWeapons"));
            Stats.Add(275, common.cap("XPKillRange"));
            Stats.Add(276, common.cap("AMSModifier"));
            Stats.Add(277, common.cap("DMSModifier"));
            Stats.Add(278, common.cap("ProjectileDamageModifier"));
            Stats.Add(279, common.cap("MeleeDamageModifier"));
            Stats.Add(280, common.cap("EnergyDamageModifier"));
            Stats.Add(281, common.cap("ChemicalDamageModifier"));
            Stats.Add(282, common.cap("RadiationDamageModifier"));
            Stats.Add(283, common.cap("ItemHateValue"));
            Stats.Add(284, common.cap("DamageBonus"));
            Stats.Add(285, common.cap("MaxDamage"));
            Stats.Add(286, common.cap("MinDamage"));
            Stats.Add(287, common.cap("AttackRange"));
            Stats.Add(288, common.cap("HateValueModifyer"));
            Stats.Add(289, common.cap("TrapDifficulty"));
            Stats.Add(290, common.cap("StatOne"));
            Stats.Add(291, common.cap("NumAttackEffects"));
            Stats.Add(292, common.cap("DefaultAttackType"));
            Stats.Add(293, common.cap("ItemSkill"));
            Stats.Add(294, common.cap("ItemDelay"));
            Stats.Add(295, common.cap("ItemOpposedSkill"));
            Stats.Add(296, common.cap("ItemSIS"));
            Stats.Add(297, common.cap("InteractionRadius"));
            Stats.Add(298, common.cap("Placement"));
            Stats.Add(299, common.cap("LockDifficulty"));
            Stats.Add(300, common.cap("Members"));
            Stats.Add(301, common.cap("MinMembers"));
            Stats.Add(302, common.cap("ClanPrice"));
            Stats.Add(303, common.cap("MissionBits3"));
            Stats.Add(304, common.cap("ClanType"));
            Stats.Add(305, common.cap("ClanInstance"));
            Stats.Add(306, common.cap("VoteCount"));
            Stats.Add(307, common.cap("MemberType"));
            Stats.Add(308, common.cap("MemberInstance"));
            Stats.Add(309, common.cap("GlobalClanType"));
            Stats.Add(310, common.cap("GlobalClanInstance"));
            Stats.Add(311, common.cap("ColdDamageModifier"));
            Stats.Add(312, common.cap("ClanUpkeepInterval"));
            Stats.Add(313, common.cap("TimeSinceUpkeep"));
            Stats.Add(314, common.cap("ClanFinalized"));
            Stats.Add(315, common.cap("NanoDamageModifier"));
            Stats.Add(316, common.cap("FireDamageModifier"));
            Stats.Add(317, common.cap("PoisonDamageModifier"));
            Stats.Add(318, common.cap("NPCostModifier"));
            Stats.Add(319, common.cap("XPModifier"));
            Stats.Add(320, common.cap("BreedLimit"));
            Stats.Add(321, common.cap("GenderLimit"));
            Stats.Add(322, common.cap("LevelLimit"));
            Stats.Add(323, common.cap("PlayerKilling"));
            Stats.Add(324, common.cap("TeamAllowed"));
            Stats.Add(325, common.cap("WeaponDisallowedType"));
            Stats.Add(326, common.cap("WeaponDisallowedInstance"));
            Stats.Add(327, common.cap("Taboo"));
            Stats.Add(328, common.cap("Compulsion"));
            Stats.Add(329, common.cap("SkillDisabled"));
            Stats.Add(330, common.cap("ClanItemType"));
            Stats.Add(331, common.cap("ClanItemInstance"));
            Stats.Add(332, common.cap("DebuffFormula"));
            Stats.Add(333, common.cap("PvP_Rating"));
            Stats.Add(334, common.cap("SavedXP"));
            Stats.Add(335, common.cap("DoorBlockTime"));
            Stats.Add(336, common.cap("OverrideTexture"));
            Stats.Add(337, common.cap("OverrideMaterial"));
            Stats.Add(338, common.cap("DeathReason"));
            Stats.Add(339, common.cap("DamageOverrideType"));
            Stats.Add(340, common.cap("BrainType"));
            Stats.Add(341, common.cap("XPBonus"));
            Stats.Add(342, common.cap("HealInterval"));
            Stats.Add(343, common.cap("HealDelta"));
            Stats.Add(344, common.cap("MonsterTexture"));
            Stats.Add(345, common.cap("HasAlwaysLootable"));
            Stats.Add(346, common.cap("TradeLimit"));
            Stats.Add(347, common.cap("FaceTexture"));
            Stats.Add(348, common.cap("SpecialCondition"));
            Stats.Add(349, common.cap("AutoAttackFlags"));
            Stats.Add(350, common.cap("NextXP"));
            Stats.Add(351, common.cap("TeleportPauseMilliSeconds"));
            Stats.Add(352, common.cap("SISCap"));
            Stats.Add(353, common.cap("AnimSet"));
            Stats.Add(354, common.cap("AttackType"));
            Stats.Add(355, common.cap("NanoFocusLevel"));
            Stats.Add(356, common.cap("NPCHash"));
            Stats.Add(357, common.cap("CollisionRadius"));
            Stats.Add(358, common.cap("OuterRadius"));
            Stats.Add(359, common.cap("MonsterData"));
            Stats.Add(360, common.cap("MonsterScale"));
            Stats.Add(361, common.cap("HitEffectType"));
            Stats.Add(362, common.cap("ResurrectDest"));
            Stats.Add(363, common.cap("NanoInterval"));
            Stats.Add(364, common.cap("NanoDelta"));
            Stats.Add(365, common.cap("ReclaimItem"));
            Stats.Add(366, common.cap("GatherEffectType"));
            Stats.Add(367, common.cap("VisualBreed"));
            Stats.Add(368, common.cap("VisualProfession"));
            Stats.Add(369, common.cap("VisualSex"));
            Stats.Add(370, common.cap("RitualTargetInst"));
            Stats.Add(371, common.cap("SkillTimeOnSelectedTarget"));
            Stats.Add(372, common.cap("LastSaveXP"));
            Stats.Add(373, common.cap("ExtendedTime"));
            Stats.Add(374, common.cap("BurstRecharge"));
            Stats.Add(375, common.cap("FullAutoRecharge"));
            Stats.Add(376, common.cap("GatherAbstractAnim"));
            Stats.Add(377, common.cap("CastTargetAbstractAnim"));
            Stats.Add(378, common.cap("CastSelfAbstractAnim"));
            Stats.Add(379, common.cap("CriticalIncrease"));
            Stats.Add(380, common.cap("RangeIncreaserWeapon"));
            Stats.Add(381, common.cap("RangeIncreaserNF"));
            Stats.Add(382, common.cap("SkillLockModifier"));
            Stats.Add(383, common.cap("InterruptModifier"));
            Stats.Add(384, common.cap("ACGEntranceStyles"));
            Stats.Add(385, common.cap("ChanceOfBreakOnSpellAttack"));
            Stats.Add(386, common.cap("ChanceOfBreakOnDebuff"));
            Stats.Add(387, common.cap("DieAnim"));
            Stats.Add(388, common.cap("TowerType"));
            Stats.Add(389, common.cap("Expansion"));
            Stats.Add(390, common.cap("LowresMesh"));
            Stats.Add(391, common.cap("CriticalDecrease"));
            Stats.Add(392, common.cap("OldTimeExist"));
            Stats.Add(393, common.cap("ResistModifier"));
            Stats.Add(394, common.cap("ChestFlags"));
            Stats.Add(395, common.cap("PrimaryTemplateID"));
            Stats.Add(396, common.cap("NumberOfItems"));
            Stats.Add(397, common.cap("SelectedTargetType"));
            Stats.Add(398, common.cap("Corpse_Hash"));
            Stats.Add(399, common.cap("AmmoName"));
            Stats.Add(400, common.cap("Rotation"));
            Stats.Add(401, common.cap("CATAnim"));
            Stats.Add(402, common.cap("CATAnimFlags"));
            Stats.Add(403, common.cap("DisplayCATAnim"));
            Stats.Add(404, common.cap("DisplayCATMesh"));
            Stats.Add(405, common.cap("School"));
            Stats.Add(406, common.cap("NanoSpeed"));
            Stats.Add(407, common.cap("NanoPoints"));
            Stats.Add(408, common.cap("TrainSkill"));
            Stats.Add(409, common.cap("TrainSkillCost"));
            Stats.Add(410, common.cap("IsFightingMe"));
            Stats.Add(411, common.cap("NextFormula"));
            Stats.Add(412, common.cap("MultipleCount"));
            Stats.Add(413, common.cap("EffectType"));
            Stats.Add(414, common.cap("ImpactEffectType"));
            Stats.Add(415, common.cap("CorpseType"));
            Stats.Add(416, common.cap("CorpseInstance"));
            Stats.Add(417, common.cap("CorpseAnimKey"));
            Stats.Add(418, common.cap("UnarmedTemplateInstance"));
            Stats.Add(419, common.cap("TracerEffectType"));
            Stats.Add(420, common.cap("AmmoType"));
            Stats.Add(421, common.cap("CharRadius"));
            Stats.Add(422, common.cap("ChanceOfUse"));
            Stats.Add(423, common.cap("CurrentState"));
            Stats.Add(424, common.cap("ArmourType"));
            Stats.Add(425, common.cap("RestModifier"));
            Stats.Add(426, common.cap("BuyModifier"));
            Stats.Add(427, common.cap("SellModifier"));
            Stats.Add(428, common.cap("CastEffectType"));
            Stats.Add(429, common.cap("NPCBrainState"));
            Stats.Add(430, common.cap("WaitState"));
            Stats.Add(431, common.cap("SelectedTarget"));
            Stats.Add(432, common.cap("MissionBits4"));
            Stats.Add(433, common.cap("OwnerInstance"));
            Stats.Add(434, common.cap("CharState"));
            Stats.Add(435, common.cap("ReadOnly"));
            Stats.Add(436, common.cap("DamageType"));
            Stats.Add(437, common.cap("CollideCheckInterval"));
            Stats.Add(438, common.cap("PlayfieldType"));
            Stats.Add(439, common.cap("NPCCommand"));
            Stats.Add(440, common.cap("InitiativeType"));
            Stats.Add(441, common.cap("CharTmp1"));
            Stats.Add(442, common.cap("CharTmp2"));
            Stats.Add(443, common.cap("CharTmp3"));
            Stats.Add(444, common.cap("CharTmp4"));
            Stats.Add(445, common.cap("NPCCommandArg"));
            Stats.Add(446, common.cap("NameTemplate"));
            Stats.Add(447, common.cap("DesiredTargetDistance"));
            Stats.Add(448, common.cap("VicinityRange"));
            Stats.Add(449, common.cap("NPCIsSurrendering"));
            Stats.Add(450, common.cap("StateMachine"));
            Stats.Add(451, common.cap("NPCSurrenderInstance"));
            Stats.Add(452, common.cap("NPCHasPatrolList"));
            Stats.Add(453, common.cap("NPCVicinityChars"));
            Stats.Add(454, common.cap("ProximityRangeOutdoors"));
            Stats.Add(455, common.cap("NPCFamily"));
            Stats.Add(456, common.cap("CommandRange"));
            Stats.Add(457, common.cap("NPCHatelistSize"));
            Stats.Add(458, common.cap("NPCNumPets"));
            Stats.Add(459, common.cap("ODMinSizeAdd"));
            Stats.Add(460, common.cap("EffectRed"));
            Stats.Add(461, common.cap("EffectGreen"));
            Stats.Add(462, common.cap("EffectBlue"));
            Stats.Add(463, common.cap("ODMaxSizeAdd"));
            Stats.Add(464, common.cap("DurationModifier"));
            Stats.Add(465, common.cap("NPCCryForHelpRange"));
            Stats.Add(466, common.cap("LOSHeight"));
            Stats.Add(467, common.cap("PetReq1"));
            Stats.Add(468, common.cap("PetReq2"));
            Stats.Add(469, common.cap("PetReq3"));
            Stats.Add(470, common.cap("MapOptions"));
            Stats.Add(471, common.cap("MapAreaPart1"));
            Stats.Add(472, common.cap("MapAreaPart2"));
            Stats.Add(473, common.cap("FixtureFlags"));
            Stats.Add(474, common.cap("FallDamage"));
            Stats.Add(475, common.cap("ReflectReturnedProjectileAC"));
            Stats.Add(476, common.cap("ReflectReturnedMeleeAC"));
            Stats.Add(477, common.cap("ReflectReturnedEnergyAC"));
            Stats.Add(478, common.cap("ReflectReturnedChemicalAC"));
            Stats.Add(479, common.cap("ReflectReturnedRadiationAC"));
            Stats.Add(480, common.cap("ReflectReturnedColdAC"));
            Stats.Add(481, common.cap("ReflectReturnedNanoAC"));
            Stats.Add(482, common.cap("ReflectReturnedFireAC"));
            Stats.Add(483, common.cap("ReflectReturnedPoisonAC"));
            Stats.Add(484, common.cap("ProximityRangeIndoors"));
            Stats.Add(485, common.cap("PetReqVal1"));
            Stats.Add(486, common.cap("PetReqVal2"));
            Stats.Add(487, common.cap("PetReqVal3"));
            Stats.Add(488, common.cap("TargetFacing"));
            Stats.Add(489, common.cap("Backstab"));
            Stats.Add(490, common.cap("OriginatorType"));
            Stats.Add(491, common.cap("QuestInstance"));
            Stats.Add(492, common.cap("QuestIndex1"));
            Stats.Add(493, common.cap("QuestIndex2"));
            Stats.Add(494, common.cap("QuestIndex3"));
            Stats.Add(495, common.cap("QuestIndex4"));
            Stats.Add(496, common.cap("QuestIndex5"));
            Stats.Add(497, common.cap("QTDungeonInstance"));
            Stats.Add(498, common.cap("QTNumMonsters"));
            Stats.Add(499, common.cap("QTKilledMonsters"));
            Stats.Add(500, common.cap("AnimPos"));
            Stats.Add(501, common.cap("AnimPlay"));
            Stats.Add(502, common.cap("AnimSpeed"));
            Stats.Add(503, common.cap("QTKillNumMonsterID1"));
            Stats.Add(504, common.cap("QTKillNumMonsterCount1"));
            Stats.Add(505, common.cap("QTKillNumMonsterID2"));
            Stats.Add(506, common.cap("QTKillNumMonsterCount2"));
            Stats.Add(507, common.cap("QTKillNumMonsterID3"));
            Stats.Add(508, common.cap("QTKillNumMonsterCount3"));
            Stats.Add(509, common.cap("QuestIndex0"));
            Stats.Add(510, common.cap("QuestTimeout"));
            Stats.Add(511, common.cap("Tower_NPCHash"));
            Stats.Add(512, common.cap("PetType"));
            Stats.Add(513, common.cap("OnTowerCreation"));
            Stats.Add(514, common.cap("OwnedTowers"));
            Stats.Add(515, common.cap("TowerInstance"));
            Stats.Add(516, common.cap("AttackShield"));
            Stats.Add(517, common.cap("SpecialAttackShield"));
            Stats.Add(518, common.cap("NPCVicinityPlayers"));
            Stats.Add(519, common.cap("NPCUseFightModeRegenRate"));
            Stats.Add(520, common.cap("Rnd"));
            Stats.Add(521, common.cap("SocialStatus"));
            Stats.Add(522, common.cap("LastRnd"));
            Stats.Add(523, common.cap("ItemDelayCap"));
            Stats.Add(524, common.cap("RechargeDelayCap"));
            Stats.Add(525, common.cap("PercentRemainingHealth"));
            Stats.Add(526, common.cap("PercentRemainingNano"));
            Stats.Add(527, common.cap("TargetDistance"));
            Stats.Add(528, common.cap("TeamCloseness"));
            Stats.Add(529, common.cap("NumberOnHateList"));
            Stats.Add(530, common.cap("ConditionState"));
            Stats.Add(531, common.cap("ExpansionPlayfield"));
            Stats.Add(532, common.cap("ShadowBreed"));
            Stats.Add(533, common.cap("NPCFovStatus"));
            Stats.Add(534, common.cap("DudChance"));
            Stats.Add(535, common.cap("HealMultiplier"));
            Stats.Add(536, common.cap("NanoDamageMultiplier"));
            Stats.Add(537, common.cap("NanoVulnerability"));
            Stats.Add(538, common.cap("AmsCap"));
            Stats.Add(539, common.cap("ProcInitiative1"));
            Stats.Add(540, common.cap("ProcInitiative2"));
            Stats.Add(541, common.cap("ProcInitiative3"));
            Stats.Add(542, common.cap("ProcInitiative4"));
            Stats.Add(543, common.cap("FactionModifier"));
            Stats.Add(544, common.cap("MissionBits8"));
            Stats.Add(545, common.cap("MissionBits9"));
            Stats.Add(546, common.cap("StackingLine2"));
            Stats.Add(547, common.cap("StackingLine3"));
            Stats.Add(548, common.cap("StackingLine4"));
            Stats.Add(549, common.cap("StackingLine5"));
            Stats.Add(550, common.cap("StackingLine6"));
            Stats.Add(551, common.cap("StackingOrder"));
            Stats.Add(552, common.cap("ProcNano1"));
            Stats.Add(553, common.cap("ProcNano2"));
            Stats.Add(554, common.cap("ProcNano3"));
            Stats.Add(555, common.cap("ProcNano4"));
            Stats.Add(556, common.cap("ProcChance1"));
            Stats.Add(557, common.cap("ProcChance2"));
            Stats.Add(558, common.cap("ProcChance3"));
            Stats.Add(559, common.cap("ProcChance4"));
            Stats.Add(560, common.cap("OTArmedForces"));
            Stats.Add(561, common.cap("ClanSentinels"));
            Stats.Add(562, common.cap("OTMed"));
            Stats.Add(563, common.cap("ClanGaia"));
            Stats.Add(564, common.cap("OTTrans"));
            Stats.Add(565, common.cap("ClanVanguards"));
            Stats.Add(566, common.cap("GOS"));
            Stats.Add(567, common.cap("OTFollowers"));
            Stats.Add(568, common.cap("OTOperator"));
            Stats.Add(569, common.cap("OTUnredeemed"));
            Stats.Add(570, common.cap("ClanDevoted"));
            Stats.Add(571, common.cap("ClanConserver"));
            Stats.Add(572, common.cap("ClanRedeemed"));
            Stats.Add(573, common.cap("SK"));
            Stats.Add(574, common.cap("LastSK"));
            Stats.Add(575, common.cap("NextSK"));
            Stats.Add(576, common.cap("PlayerOptions"));
            Stats.Add(577, common.cap("LastPerkResetTime"));
            Stats.Add(578, common.cap("CurrentTime"));
            Stats.Add(579, common.cap("ShadowBreedTemplate"));
            Stats.Add(580, common.cap("NPCVicinityFamily"));
            Stats.Add(581, common.cap("NPCScriptAMSScale"));
            Stats.Add(582, common.cap("ApartmentsAllowed"));
            Stats.Add(583, common.cap("ApartmentsOwned"));
            Stats.Add(584, common.cap("ApartmentAccessCard"));
            Stats.Add(585, common.cap("MapAreaPart3"));
            Stats.Add(586, common.cap("MapAreaPart4"));
            Stats.Add(587, common.cap("NumberOfTeamMembers"));
            Stats.Add(588, common.cap("ActionCategory"));
            Stats.Add(589, common.cap("CurrentPlayfield"));
            Stats.Add(590, common.cap("DistrictNano"));
            Stats.Add(591, common.cap("DistrictNanoInterval"));
            Stats.Add(592, common.cap("UnsavedXP"));
            Stats.Add(593, common.cap("RegainXPPercentage"));
            Stats.Add(594, common.cap("TempSaveTeamID"));
            Stats.Add(595, common.cap("TempSavePlayfield"));
            Stats.Add(596, common.cap("TempSaveX"));
            Stats.Add(597, common.cap("TempSaveY"));
            Stats.Add(598, common.cap("ExtendedFlags"));
            Stats.Add(599, common.cap("ShopPrice"));
            Stats.Add(600, common.cap("NewbieHP"));
            Stats.Add(601, common.cap("HPLevelUp"));
            Stats.Add(602, common.cap("HPPerSkill"));
            Stats.Add(603, common.cap("NewbieNP"));
            Stats.Add(604, common.cap("NPLevelUp"));
            Stats.Add(605, common.cap("NPPerSkill"));
            Stats.Add(606, common.cap("MaxShopItems"));
            Stats.Add(607, common.cap("PlayerID"));
            Stats.Add(608, common.cap("ShopRent"));
            Stats.Add(609, common.cap("SynergyHash"));
            Stats.Add(610, common.cap("ShopFlags"));
            Stats.Add(611, common.cap("ShopLastUsed"));
            Stats.Add(612, common.cap("ShopType"));
            Stats.Add(613, common.cap("LockDownTime"));
            Stats.Add(614, common.cap("LeaderLockDownTime"));
            Stats.Add(615, common.cap("InvadersKilled"));
            Stats.Add(616, common.cap("KilledByInvaders"));
            Stats.Add(617, common.cap("MissionBits10"));
            Stats.Add(618, common.cap("MissionBits11"));
            Stats.Add(619, common.cap("MissionBits12"));
            Stats.Add(620, common.cap("HouseTemplate"));
            Stats.Add(621, common.cap("PercentFireDamage"));
            Stats.Add(622, common.cap("PercentColdDamage"));
            Stats.Add(623, common.cap("PercentMeleeDamage"));
            Stats.Add(624, common.cap("PercentProjectileDamage"));
            Stats.Add(625, common.cap("PercentPoisonDamage"));
            Stats.Add(626, common.cap("PercentRadiationDamage"));
            Stats.Add(627, common.cap("PercentEnergyDamage"));
            Stats.Add(628, common.cap("PercentChemicalDamage"));
            Stats.Add(629, common.cap("TotalDamage"));
            Stats.Add(630, common.cap("TrackProjectileDamage"));
            Stats.Add(631, common.cap("TrackMeleeDamage"));
            Stats.Add(632, common.cap("TrackEnergyDamage"));
            Stats.Add(633, common.cap("TrackChemicalDamage"));
            Stats.Add(634, common.cap("TrackRadiationDamage"));
            Stats.Add(635, common.cap("TrackColdDamage"));
            Stats.Add(636, common.cap("TrackPoisonDamage"));
            Stats.Add(637, common.cap("TrackFireDamage"));
            Stats.Add(638, common.cap("NPCSpellArg1"));
            Stats.Add(639, common.cap("NPCSpellRet1"));
            Stats.Add(640, common.cap("CityInstance"));
            Stats.Add(641, common.cap("DistanceToSpawnpoint"));
            Stats.Add(642, common.cap("CityTerminalRechargePercent"));
            Stats.Add(649, common.cap("UnreadMailCount"));
            Stats.Add(650, common.cap("LastMailCheckTime"));
            Stats.Add(651, common.cap("AdvantageHash1"));
            Stats.Add(652, common.cap("AdvantageHash2"));
            Stats.Add(653, common.cap("AdvantageHash3"));
            Stats.Add(654, common.cap("AdvantageHash4"));
            Stats.Add(655, common.cap("AdvantageHash5"));
            Stats.Add(656, common.cap("ShopIndex"));
            Stats.Add(657, common.cap("ShopID"));
            Stats.Add(658, common.cap("IsVehicle"));
            Stats.Add(659, common.cap("DamageToNano"));
            Stats.Add(660, common.cap("AccountFlags"));
            Stats.Add(661, common.cap("DamageToNanoMultiplier"));
            Stats.Add(662, common.cap("MechData"));
            Stats.Add(664, common.cap("VehicleAC"));
            Stats.Add(665, common.cap("VehicleDamage"));
            Stats.Add(666, common.cap("VehicleHealth"));
            Stats.Add(667, common.cap("VehicleSpeed"));
            Stats.Add(668, common.cap("BattlestationSide"));
            Stats.Add(669, common.cap("VP"));
            Stats.Add(670, common.cap("BattlestationRep"));
            Stats.Add(671, common.cap("PetState"));
            Stats.Add(672, common.cap("PaidPoints"));
            Stats.Add(673, common.cap("VisualFlags"));
            Stats.Add(674, common.cap("PVPDuelKills"));
            Stats.Add(675, common.cap("PVPDuelDeaths"));
            Stats.Add(676, common.cap("PVPProfessionDuelKills"));
            Stats.Add(677, common.cap("PVPProfessionDuelDeaths"));
            Stats.Add(678, common.cap("PVPRankedSoloKills"));
            Stats.Add(679, common.cap("PVPRankedSoloDeaths"));
            Stats.Add(680, common.cap("PVPRankedTeamKills"));
            Stats.Add(681, common.cap("PVPRankedTeamDeaths"));
            Stats.Add(682, common.cap("PVPSoloScore"));
            Stats.Add(683, common.cap("PVPTeamScore"));
            Stats.Add(684, common.cap("PVPDuelScore"));
            Stats.Add(700, common.cap("ACGItemSeed"));
            Stats.Add(701, common.cap("ACGItemLevel"));
            Stats.Add(702, common.cap("ACGItemTemplateID"));
            Stats.Add(703, common.cap("ACGItemTemplateID2"));
            Stats.Add(704, common.cap("ACGItemCategoryID"));
            Stats.Add(768, common.cap("HasKnubotData"));
            Stats.Add(800, common.cap("QuestBoothDifficulty"));
            Stats.Add(801, common.cap("QuestASMinimumRange"));
            Stats.Add(802, common.cap("QuestASMaximumRange"));
            Stats.Add(888, common.cap("VisualLODLevel"));
            Stats.Add(889, common.cap("TargetDistanceChange"));
            Stats.Add(900, common.cap("TideRequiredDynelID"));
            Stats.Add(999, common.cap("StreamCheckMagic"));
            Stats.Add(1001, common.cap("Type"));
            Stats.Add(1002, common.cap("Instance"));
            #endregion

            #region Operators
            Operators.Add(0, common.cap("equalto"));
            Operators.Add(1, common.cap("lessthan"));
            Operators.Add(2, common.cap("greaterthan"));
            Operators.Add(3, common.cap("or"));
            Operators.Add(4, common.cap("and"));
            Operators.Add(5, common.cap("time_less"));
            Operators.Add(6, common.cap("time_larger"));
            Operators.Add(7, common.cap("item_has"));
            Operators.Add(8, common.cap("item_hasnot"));
            Operators.Add(9, common.cap("id"));
            Operators.Add(10, common.cap("targetid"));
            Operators.Add(11, common.cap("targetsignal"));
            Operators.Add(12, common.cap("targetstat"));
            Operators.Add(13, common.cap("primary_item"));
            Operators.Add(14, common.cap("secondary_item"));
            Operators.Add(15, common.cap("area_zminmax"));
            Operators.Add(16, common.cap("user"));
            Operators.Add(17, common.cap("itemanim"));
            Operators.Add(18, common.cap("ontarget"));
            Operators.Add(19, common.cap("onself"));
            Operators.Add(20, common.cap("signal"));
            Operators.Add(21, common.cap("onsecondaryitem"));
            Operators.Add(22, common.cap("bitand"));
            Operators.Add(23, common.cap("bitor"));
            Operators.Add(24, common.cap("unequal"));
            Operators.Add(25, common.cap("illegal"));
            Operators.Add(26, common.cap("onuser"));
            Operators.Add(27, common.cap("onvalidtarget"));
            Operators.Add(28, common.cap("oninvalidtarget"));
            Operators.Add(29, common.cap("onvaliduser"));
            Operators.Add(30, common.cap("oninvaliduser"));
            Operators.Add(31, common.cap("haswornitem"));
            Operators.Add(32, common.cap("hasnotwornitem"));
            Operators.Add(33, common.cap("haswieldeditem"));
            Operators.Add(34, common.cap("hasnotwieldeditem"));
            Operators.Add(35, common.cap("hasformula"));
            Operators.Add(36, common.cap("hasnotformula"));
            Operators.Add(37, common.cap("ongeneralbeholder"));
            Operators.Add(38, common.cap("isvalid"));
            Operators.Add(39, common.cap("isinvalid"));
            Operators.Add(40, common.cap("isalive"));
            Operators.Add(41, common.cap("iswithinvicinity"));
            Operators.Add(42, common.cap("not"));
            Operators.Add(43, common.cap("iswithinweaponrange"));
            Operators.Add(44, common.cap("isnpc"));
            Operators.Add(45, common.cap("isfighting"));
            Operators.Add(46, common.cap("isattacked"));
            Operators.Add(47, common.cap("isanyonelooking"));
            Operators.Add(48, common.cap("isfoe"));
            Operators.Add(49, common.cap("isindungeon"));
            Operators.Add(50, common.cap("issameas"));
            Operators.Add(51, common.cap("distanceto"));
            Operators.Add(52, common.cap("isinnofightingarea"));
            Operators.Add(53, common.cap("template_compare"));
            Operators.Add(54, common.cap("min_max_level_compare"));
            Operators.Add(57, common.cap("monstertemplate"));
            Operators.Add(58, common.cap("hasmaster"));
            Operators.Add(59, common.cap("canexecuteformulaontarget"));
            Operators.Add(60, common.cap("area_targetinvicinity"));
            Operators.Add(61, common.cap("isunderheavyattack"));
            Operators.Add(62, common.cap("islocationok"));
            Operators.Add(63, common.cap("isnottoohighlevel"));
            Operators.Add(64, common.cap("haschangedroomwhilefighting"));
            Operators.Add(65, common.cap("kullnumberof"));
            Operators.Add(66, common.cap("testnumpets"));
            Operators.Add(67, common.cap("numberofitems"));
            Operators.Add(68, common.cap("primarytemplate"));
            Operators.Add(69, common.cap("isteleporting"));
            Operators.Add(70, common.cap("isflying"));
            Operators.Add(71, common.cap("scanforstat"));
            Operators.Add(72, common.cap("hasmeonpetlist"));
            Operators.Add(73, common.cap("trickledownlarger"));
            Operators.Add(74, common.cap("trickledownless"));
            Operators.Add(75, common.cap("ispetoverequipped"));
            Operators.Add(76, common.cap("haspetpendingnanoformula"));
            Operators.Add(77, common.cap("ispet"));
            Operators.Add(79, common.cap("canattackchar"));
            Operators.Add(80, common.cap("istowercreateallowed"));
            Operators.Add(81, common.cap("inventoryslotisfull"));
            Operators.Add(82, common.cap("inventoryslotisempty"));
            Operators.Add(83, common.cap("candisabledefenseshield"));
            Operators.Add(84, common.cap("isnpcornpccontrolledpet"));
            Operators.Add(85, common.cap("sameasselectedtarget"));
            Operators.Add(86, common.cap("isplayerorplayercontrolledpet"));
            Operators.Add(87, common.cap("hasenterednonpvpzone"));
            Operators.Add(88, common.cap("uselocation"));
            Operators.Add(89, common.cap("isfalling"));
            Operators.Add(90, common.cap("isondifferentplayfield"));
            Operators.Add(91, common.cap("hasrunningnano"));
            Operators.Add(92, common.cap("hasrunningnanoline"));
            Operators.Add(93, common.cap("hasperk"));
            Operators.Add(94, common.cap("isperklocked"));
            Operators.Add(95, common.cap("isfactionreactionset"));
            Operators.Add(96, common.cap("hasmovetotarget"));
            Operators.Add(97, common.cap("isperkunlocked"));
            Operators.Add(98, common.cap("true"));
            Operators.Add(99, common.cap("false"));
            Operators.Add(100, common.cap("oncaster"));
            Operators.Add(101, common.cap("hasnotrunningnano"));
            Operators.Add(102, common.cap("hasnotrunningnanoline"));
            Operators.Add(103, common.cap("hasnotperk"));
            Operators.Add(107, common.cap("notbitand"));
            Operators.Add(108, common.cap("obtaineditem"));
            Operators.Add(255, "No ChildOp");
            #endregion

            #region Targets
            Targets.Add(1, "User");
            Targets.Add(2, "Wearer");
            Targets.Add(3, "Target");
            Targets.Add(14, "Fighting Target");
            Targets.Add(19, "Self");
            Targets.Add(23, "Selected Target");
            #endregion

            #region Function Templates
            FunctionTemplates ft;
            FunctionTemplate e;
            Single dummysingle = 0.0f;
            string dummystring = " ";
            LineValue dummyline = new LineValue();
            UInt32 dummyuint = 0;
            int dummyint = 0;
            PlayfieldValue dummypf = new PlayfieldValue();


            // Line Teleport
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "100001", "Type", true));
            ft.Templates.Add(new FunctionTemplate(dummyline.GetType(), "0", "Line Number", false));
            ft.Templates.Add(new FunctionTemplate(dummypf.GetType(), "0", "Playfield", false));

            Functiontemplates.Add(53059, ft);

            // Hit
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Stat Number", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Min", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Max", false));

            Functiontemplates.Add(53002, ft);

            // Teleport 
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "X", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Y", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Z", false));
            ft.Templates.Add(new FunctionTemplate(dummypf.GetType(), "0", "Playfield", false));

            Functiontemplates.Add(53016,ft);
            
            // Inputbox (Global Market Search Terminal)
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Unknown", true));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "1", "Unknown", true));

            Functiontemplates.Add(53115, ft);

            // Teleportproxy
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "51102", "Type?", true));
            ft.Templates.Add(new FunctionTemplate(dummypf.GetType(), "0", "To Playfield", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Unknown", true));
            ft.Templates.Add(new FunctionTemplate(dummyuint.GetType(), "0", "StatelInstance", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "100002", "Unknown", true));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "1", "Unknown", true));

            Functiontemplates.Add(53082, ft);

            // Text
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummystring.GetType(), "", "Text1", false));
            ft.Templates.Add(new FunctionTemplate(dummystring.GetType(), "", "Text2?", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Unknown", true));

            Functiontemplates.Add(53134, ft);

            // CastNano
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "NanoID", false));

            Functiontemplates.Add(53051, ft);

            // LockSkill
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "3", "Unknown (Target?)", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Stat Number", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Time (s)", false));

            Functiontemplates.Add(53033, ft);

            // AddSkill
            ft = new FunctionTemplates();
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Amount", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "0", "Stat number", false));
            ft.Templates.Add(new FunctionTemplate(dummyint.GetType(), "1", "Unknown", false));

            Functiontemplates.Add(53028, ft);

            // Openbank
            ft = new FunctionTemplates();
            Functiontemplates.Add(53092, ft);

            // Enterapartment
            ft = new FunctionTemplates();
            Functiontemplates.Add(53109, ft);


            #endregion
        }
    }
    public class StatelEvent
    {
        public int eventnum = 0;
        public int eventid = 0;
        public override string ToString()
        {
            if (NamesandNumbers.Events.ContainsKey(eventnum) == true)
            {
                return NamesandNumbers.Events[eventnum] + " (" + eventnum + ")";
            }
            return "Unknown Event ID: " + eventnum;
        }
        public void changeid(int statelid, int to_id)
        {
            if (to_id == eventid)
            {
                return;
            }
            DataSet dt = common.readSQL("SELECT * FROM statel_events WHERE eventid=" + to_id + " AND statel_id=" + statelid);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.execSQL("UPDATE statel_events SET evendid=-1 WHERE eventid=" + to_id + " AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_functions SET event_id=-1 WHERE event_id=" + to_id + " AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_function_arguments SET event_id=-1 WHERE event_id=" + to_id + " AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_function_reqs SET event_id=-1 WHERE event_id=" + to_id + " AND statel_id=" + statelid);
                }
            }
            common.execSQL("UPDATE statel_events SET evendid="+to_id+" WHERE eventid=" + eventid + " AND statel_id=" + statelid);
            common.execSQL("UPDATE statel_functions SET event_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid);
            common.execSQL("UPDATE statel_function_arguments SET event_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid);
            common.execSQL("UPDATE statel_function_reqs SET event_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid);

            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.execSQL("UPDATE statel_events SET evendid="+eventid+" WHERE eventid=-1 AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_functions SET event_id=" + eventid + " WHERE event_id=-1 AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_function_arguments SET event_id=" + eventid + " WHERE event_id=-1 AND statel_id=" + statelid);
                    common.execSQL("UPDATE statel_function_reqs SET event_id=" + eventid + " WHERE event_id=-1 AND statel_id=" + statelid);
                }
            }
        }
    }

    public class StatelFunction
    {
        public int functionid = 0;
        public int functionnum = 0;
        public int tickinterval = 0;
        public int tickcount = 0;
        public int target = 0;
        public override string ToString()
        {
            if (NamesandNumbers.Functions.ContainsKey(functionnum) == true)
            {
                return NamesandNumbers.Functions[functionnum] + " (" + functionnum + ") Target " + target + " TickCount " + tickcount + " TickInterval " + tickinterval;
            }
            return "Unknown Function: " + functionnum;
        }
        public void changeid(int statelid, int eventid, int to_id)
        {
            if (to_id == functionid)
            {
                return;
            }
            DataSet dt = common.readSQL("SELECT * FROM statel_functions WHERE event_id=" + eventid + " AND functionid=" + to_id + " AND statel_id=" + statelid);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.execSQL("UPDATE statel_functions SET function_id=-1 WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + to_id);
                    common.execSQL("UPDATE statel_function_arguments SET function_id=-1 WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + to_id);
                    common.execSQL("UPDATE statel_function_reqs SET function_id=-1 WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + to_id);
                }
            }
            common.execSQL("UPDATE statel_functions SET function_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + functionid);
            common.execSQL("UPDATE statel_function_arguments SET function_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + functionid);
            common.execSQL("UPDATE statel_function_reqs SET function_id=" + to_id + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=" + functionid);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.execSQL("UPDATE statel_functions SET function_id=" + functionid + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=-1");
                    common.execSQL("UPDATE statel_function_arguments SET function_id=" + functionid + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=-1");
                    common.execSQL("UPDATE statel_function_reqs SET function_id=" + functionid + " WHERE event_id=" + eventid + " AND statel_id=" + statelid + " AND functionid=-1");
                }
            }
        }

    }

    public class StatelRequirement
    {
        public int reqid = 0;
        public int attribute = 0;
        public int value = 0;
        public int op = 0;
        public int childop = 0;
        public int target = 0;
        public override string ToString()
        {
            string attr;
            string sop;
            string cop;
            string targ;
            if (NamesandNumbers.Stats.ContainsKey(attribute) == true)
            {
                attr = "Attr "+NamesandNumbers.Stats[attribute] + " (" + attribute + ") ";
            }
            else
            {
                attr = "Unknown Attribute: " + attribute + " ";
            }
            if (NamesandNumbers.Operators.ContainsKey(op) == true)
            {
                sop = "Operator "+NamesandNumbers.Operators[op] + " (" + op + ") ";
            }
            else
            {
                sop = "Unknown Operator " + op + " ";
            }

            if (NamesandNumbers.Operators.ContainsKey(childop) == true)
            {
                cop = "ChildOp "+NamesandNumbers.Operators[childop] + " (" + op + ") ";
            }
            else
            {
                cop = "Unknown Operator " + childop + " ";
            }

            if (NamesandNumbers.Targets.ContainsKey(target) == true)
            {
                targ = "Target "+NamesandNumbers.Targets[target] + " (" + target + ") ";
            }
            else
            {
                targ = "Unknown Target " + target+" ";
            }
            return (string)attr+"Value "+value+" "+sop+targ+cop;
        }

        public void changeid(int statelid, int eventid, int functionid, int to_id)
        {
            if (to_id == reqid)
            {
                return;
            }
            DataSet dt = common.readSQL("SELECT * FROM statel_function_reqs WHERE event_id=" + eventid + " AND function_id=" + functionid + " AND statel_id=" + statelid + " AND reqid=" + to_id);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.readSQL("UDPATE statel_function_reqs SET reqid=-1 WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND reqid=" + to_id);
                }
            }
            common.readSQL("UDPATE statel_function_reqs SET reqid="+to_id+" WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND reqid=" + reqid);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.readSQL("UDPATE statel_function_reqs SET reqid=" + reqid + " WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND reqid=-1");
                }
            }
        }
    }

    public class StatelArgument
    {
        public int attributeid = 0;
        public string value = "";
        public void changeid(int statelid, int eventid, int functionid, int to_id)
        {
            if (to_id == attributeid)
            {
                return;
            }
            DataSet dt = common.readSQL("SELECT * FROM statel_function_arguments WHERE event_id=" + eventid + " AND function_id=" + functionid + " AND statel_id=" + statelid + " AND attrid=" + to_id);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.readSQL("UDPATE statel_function_arguments SET attrid=-1 WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND attrid=" + to_id);
                }
            }
            common.readSQL("UDPATE statel_function_arguments SET attrid=" + to_id + " WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND attrid=" + attributeid);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    common.readSQL("UDPATE statel_function_arguments SET attrid=" + attributeid + " WHERE statel_id=" + statelid + " AND event_id=" + eventid + " AND function_id=" + functionid + " AND attrid=-1");
                }
            }
        }
    }

    public class FunctionTemplate
    {
        public Type DataType = System.Type.GetType("");
        public string baseValue = "";
        public string name = "";
        public bool ro = false;

        public FunctionTemplate(Type dt, string baseval, string nam, bool r_o)
        {
            DataType = dt;
            baseValue = baseval;
            name = nam;
            ro = r_o;
        }

        public FunctionTemplate()
        {
        }
    }

    public class PlayfieldValue
    {
        public int Playfield;
    }

    public class StatValue
    {
        public int Statnumber;
        public string StatName;
    }

    public class FunctionTemplates
    {
        public List<FunctionTemplate> Templates = new List<FunctionTemplate>();
    }

    public class LineValue
    {
        public int Line;
        public int PF;
    }
}
