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

#region Usings...
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AO.Core;
#endregion

namespace ZoneEngine
{
    public class ItemHandlerold
    {
        public SqlWrapper mySql = new SqlWrapper();

        #region Constants
        #region Itemtypes
        public const int itemtype_Misc = 0;
        public const int itemtype_Weapon = 1;
        public const int itemtype_Armor = 2;
        public const int itemtype_Implant = 3;
        public const int itemtype_NPC = 4;
        public const int itemtype_Spirit = 5;
        public const int itemtype_Utility = 6;
        public const int itemtype_Tower = 7;
        #endregion

        #region Armorslots
        /// 
        /// Armorslots
        /// 
        public const int armorslot_neck = 1;
        public const int armorslot_head = 2;
        public const int armorslot_back = 3;
        public const int armorslot_rightshoulder = 4;
        public const int armorslot_chest = 5;
        public const int armorslot_leftshoulder = 6;
        public const int armorslot_rightarm = 7;
        public const int armorslot_hands = 8;
        public const int armorslot_leftarm = 9;
        public const int armorslot_rightwrist = 10;
        public const int armorslot_legs = 11;
        public const int armorslot_leftwrist = 12;
        public const int armorslot_rightfinger = 13;
        public const int armorslot_feet = 14;
        public const int armorslot_leftfinger = 15;
        #endregion

        #region Implantslots
        ///
        /// Implantslots
        /// 
        public const int implantslot_eyes = 1;
        public const int implantslot_head = 2;
        public const int implantslot_ears = 3;
        public const int implantslot_rightarm = 4;
        public const int implantslot_chest = 5;
        public const int implantslot_leftarm = 6;
        public const int implantslot_rightwrist = 7;
        public const int implantslot_waist = 8;
        public const int implantslot_leftwrist = 9;
        public const int implantslot_righthand = 10;
        public const int implantslot_legs = 11;
        public const int implantslot_lefthand = 12;
        public const int implantslot_feet = 13;
        #endregion

        #region Weaponslots
        ///
        /// Weaponslots
        /// 
        public const int weaponslot_hud1 = 1;
        public const int weaponslot_hud3 = 2;
        public const int weaponslot_util1 = 3;
        public const int weaponslot_util2 = 4;
        public const int weaponslot_util3 = 5;
        public const int weaponslot_righthand = 6;
        public const int weaponslot_belt = 7;
        public const int weaponslot_left_hand = 8;
        public const int weaponslot_ncu1 = 9;
        public const int weaponslot_ncu2 = 10;
        public const int weaponslot_ncu3 = 11;
        public const int weaponslot_ncu4 = 12;
        public const int weaponslot_ncu5 = 13;
        public const int weaponslot_ncu6 = 14;
        public const int weaponslot_hud2 = 15;
        #endregion

        #region Targets
        ///
        /// Targets
        /// 
        public const int itemtarget_user = 1;
        public const int itemtarget_wearer = 2;
        public const int itemtarget_target = 3;
        public const int itemtarget_fightingtarget = 14;
        public const int itemtarget_self = 19;
        public const int itemtarget_selectedtarget = 23;
        #endregion

        #region Expansion Flags
        ///
        /// Expansion Flags
        /// 
        public const int expansion_NW = 0x1 << 0;
        public const int expansion_SL = 0x1 << 1;
        public const int expansion_SLP = 0x1 << 2;
        public const int expansion_AI = 0x1 << 3;
        public const int expansion_AIP = 0x1 << 4;
        public const int expansion_LE = 0x1 << 5;
        public const int expansion_LEP = 0x1 << 6;
        public const int expansion_LOX = 0x1 << 7;
        public const int expansion_LOXP = 0x1 << 8;
        #endregion

        #region Actions
        ///
        /// Actions
        /// 
        public const int actiontype_any = 0;
        public const int actiontype_get = 1;
        public const int actiontype_drop = 2;
        public const int actiontype_touse = 3;
        public const int actiontype_repair = 4;
        public const int actiontype_useitemonitem = 5;
        public const int actiontype_towear = 6;
        public const int actiontype_toremove = 7;
        public const int actiontype_towield = 8;
        public const int actiontype_tounwield = 9;
        public const int actiontype_split = 10;
        public const int actiontype_attack = 11;
        public const int actiontype_ams = 12;
        public const int actiontype_dms = 13;
        public const int actiontype_doubleattack = 14;
        public const int actiontype_idle = 15;
        public const int actiontype_combatidle = 16;
        public const int actiontype_walk = 17;
        public const int actiontype_run = 18;
        public const int actiontype_sneak = 19;
        public const int actiontype_crawl = 20;
        public const int actiontype_aimedshot = 21;
        public const int actiontype_burst = 22;
        public const int actiontype_fullauto = 23;
        public const int actiontype_leftattack = 24;
        public const int actiontype_fastattack = 25;
        public const int actiontype_combatidlestart = 26;
        public const int actiontype_combatidleend = 27;
        public const int actiontype_flingshot = 28;
        public const int actiontype_sneakattack = 29;
        public const int actiontype_terminate = 30;
        public const int actiontype_impact = 31;
        public const int actiontype_useitemoncharacter = 32;
        public const int actiontype_leftfoot = 33;
        public const int actiontype_rightfoot = 34;
        public const int actiontype_open = 100;
        public const int actiontype_close = 102;
        public const int actiontype_totriggertargetinvicinity = 111;
        public const int actiontype_playshiftrequirements = 136;
        #endregion

        #region Canflags
        ///
        /// Canflags
        /// 
        public const int canflag_carry = 0x1 << 0;
        public const int canflag_sit = 0x1 << 1;
        public const int canflag_wear = 0x1 << 2;
        public const int canflag_use = 0x1 << 3;
        public const int canflag_confirmuse = 0x1 << 4;
        public const int canflag_consume = 0x1 << 5;
        public const int canflag_tutorchip = 0x1 << 6;
        public const int canflag_tutordevice = 0x1 << 7;
        public const int canflag_breakandenter = 0x1 << 8;
        public const int canflag_stackable = 0x1 << 9;
        public const int canflag_noammo = 0x1 << 10;
        public const int canflag_burst = 0x1 << 11;
        public const int canflag_flingshot = 0x1 << 12;
        public const int canflag_fullauto = 0x1 << 13;
        public const int canflag_aimedshot = 0x1 << 14;
        public const int canflag_bow = 0x1 << 15;
        public const int canflag_throwattack = 0x1 << 16;
        public const int canflag_sneakattack = 0x1 << 17;
        public const int canflag_fastattack = 0x1 << 18;
        public const int canflag_disarmtraps = 0x1 << 19;
        public const int canflag_autoselect = 0x1 << 20;
        public const int canflag_applyonfriendly = 0x1 << 21;
        public const int canflag_applyonhostile = 0x1 << 22;
        public const int canflag_applyonself = 0x1 << 23;
        public const int canflag_cantsplit = 0x1 << 24;
        public const int canflag_brawl = 0x1 << 25;
        public const int canflag_dimach = 0x1 << 26;
        public const int canflag_enablehandattractors = 0x1 << 27;
        public const int canflag_canbewornwithsocialarmor = 0x1 << 28;
        public const int canflag_canparryriposte = 0x1 << 29;
        public const int canflag_canbeparriedriposted = 0x1 << 30;
        public const int canflag_applyonfightingtarget = 0x1 << 31;
        #endregion

        #region Event types
        ///
        /// Event Types
        /// 
        public const int eventtype_onuse = 0;
        public const int eventtype_onrepair = 1;
        public const int eventtype_onwield = 2;
        public const int eventtype_ontargetinvicinity = 3;
        public const int eventtype_onuseitemon = 4;
        public const int eventtype_onhit = 5;
        public const int eventtype_oncreate = 7;
        public const int eventtype_oneffects = 8;
        public const int eventtype_onrun = 9;
        public const int eventtype_onactivate = 10;
        public const int eventtype_onstarteffect = 12;
        public const int eventtype_onendeffect = 13;
        public const int eventtype_onwear = 14;
        public const int eventtype_onusefailed = 15;
        public const int eventtype_onenter = 16;
        public const int eventtype_onopen = 18;
        public const int eventtype_onclose = 19;
        public const int eventtype_onterminate = 20;
        public const int eventtype_oncollide = 22;
        public const int eventtype_onendcollide = 23;
        public const int eventtype_onfriendlyinvicinity = 24;
        public const int eventtype_onenemyinvicinity = 25;
        public const int eventtype_personalmodifier = 26;
        public const int eventtype_onfailure = 27;
        public const int eventtype_ontrade = 37;
        #endregion

        #region Function Types
        /// 
        /// Function Types
        /// 
        public const int functiontype_shophash = 0;
        public const int functiontype_hit = 53002;
        public const int functiontype_animeffect = 53003;
        public const int functiontype_mesh = 53004;
        public const int functiontype_creation = 53005;
        public const int functiontype_poison = 53006;
        public const int functiontype_radius = 53007;
        public const int functiontype_remove = 53008;
        public const int functiontype_texteffect = 53009;
        public const int functiontype_visualeffect = 53010;
        public const int functiontype_audioeffect = 53011;
        public const int functiontype_skill = 53012;
        public const int functiontype_poisonremove = 53013;
        public const int functiontype_timedeffect = 53014;
        public const int functiontype_criteria = 53015;
        public const int functiontype_teleport = 53016;
        public const int functiontype_playmusic = 53017;
        public const int functiontype_stopmusic = 53018;
        public const int functiontype_uploadnano = 53019;
        public const int functiontype_catmesh = 53023;
        public const int functiontype_expression = 53024;
        public const int functiontype_anim = 53025;
        public const int functiontype_set = 53026;
        public const int functiontype_createstat = 53027;
        public const int functiontype_addskill = 53028;
        public const int functiontype_adddifficulty = 53029;
        public const int functiontype_gfxeffect = 53030;
        public const int functiontype_itemanimeffect = 53031;
        public const int functiontype_savechar = 53032;
        public const int functiontype_lockskill = 53033;
        public const int functiontype_directitemanimeffect = 53034;
        public const int functiontype_headmesh = 53035;
        public const int functiontype_hairmesh = 53036;
        public const int functiontype_backmesh = 53037;
        public const int functiontype_shouldermesh = 53038;
        public const int functiontype_texture = 53039;
        public const int functiontype_starteffect = 53040;
        public const int functiontype_endeffect = 53041;
        public const int functiontype_weaponeffectcolor = 53042;
        public const int functiontype_addshopitem = 53043;
        public const int functiontype_systemtext = 53044;
        public const int functiontype_modify = 53045;
        public const int functiontype_animaction = 53047;
        public const int functiontype_name = 53048;
        public const int functiontype_spawnmonster = 53049;
        public const int functiontype_removebuffs = 53050;
        public const int functiontype_castnano = 53051;
        public const int functiontype_strtexture = 53052;
        public const int functiontype_strmesh = 53053;
        public const int functiontype_changebodymesh = 53054;
        public const int functiontype_attractormesh = 53055;
        public const int functiontype_waypoint = 53056;
        public const int functiontype_headtext = 53057;
        public const int functiontype_setstate = 53058;
        public const int functiontype_lineteleport = 53059;
        public const int functiontype_monstershape = 53060;
        public const int functiontype_addshopitem2 = 53061;
        public const int functiontype_npcselecttarget = 53062;
        public const int functiontype_spawnmonster2 = 53063;
        public const int functiontype_spawnitem = 53064;
        public const int functiontype_attractoreffect = 53065;
        public const int functiontype_teamcastnano = 53066;
        public const int functiontype_changeactionrestriction = 53067;
        public const int functiontype_restrictaction = 53068;
        public const int functiontype_nexthead = 53069;
        public const int functiontype_prevhead = 53070;
        public const int functiontype_areahit = 53073;
        public const int functiontype_makevendorshop = 53074;
        public const int functiontype_attractoreffect1 = 53075;
        public const int functiontype_attractoreffect2 = 53076;
        public const int functiontype_npcfightselected = 53077;
        public const int functiontype_npcsocialanim = 53078;
        public const int functiontype_changeeffect = 53079;
        public const int functiontype_npcturntotarget = 53080;
        public const int functiontype_npchatelisttarget = 53081;
        public const int functiontype_teleportproxy = 53082;
        public const int functiontype_teleportproxy2 = 53083;
        public const int functiontype_refreshmodel = 53086;
        public const int functiontype_areacastnano = 53087;
        public const int functiontype_caststunnano = 53089;
        public const int functiontype_npcgettargethatelist = 53090;
        public const int functiontype_npcsetmaster = 53091;
        public const int functiontype_openbank = 53092;
        public const int functiontype_npcfollowselected = 53095;
        public const int functiontype_npcmoveforward = 53096;
        public const int functiontype_npcsendplaysync = 53097;
        public const int functiontype_npctrygroupform = 53098;
        public const int functiontype_equipmonsterweapon = 53100;
        public const int functiontype_npcapplynanoformula = 53102;
        public const int functiontype_npcsendcommand = 53103;
        public const int functiontype_npcsayrobotspeech = 53104;
        public const int functiontype_removenanoeffects = 53105;
        public const int functiontype_npcpushscript = 53107;
        public const int functiontype_npcpopscript = 53108;
        public const int functiontype_enterapartment = 53109;
        public const int functiontype_changevariable = 53110;
        public const int functiontype_npcstartsurrender = 53113;
        public const int functiontype_npcstopsurrender = 53114;
        public const int functiontype_inputbox = 53115;
        public const int functiontype_npcstopmoving = 53116;
        public const int functiontype_tauntnpc = 53117;
        public const int functiontype_pacify = 53118;
        public const int functiontype_npcclearsignal = 53119;
        public const int functiontype_npccallforhelp = 53120;
        public const int functiontype_fear = 53121;
        public const int functiontype_stun = 53122;
        public const int functiontype_rndspawnitem = 53124;
        public const int functiontype_rndspawnmonster = 53125;
        public const int functiontype_npcwipehatelist = 53126;
        public const int functiontype_charmnpc = 53127;
        public const int functiontype_daze = 53128;
        public const int functiontype_npccreatepet = 53129;
        public const int functiontype_destroyitem = 53130;
        public const int functiontype_npckilltarget = 53131;
        public const int functiontype_generatename = 53132;
        public const int functiontype_setgovernmenttype = 53133;
        public const int functiontype_text = 53134;
        public const int functiontype_createapartment = 53137;
        public const int functiontype_canfly = 53138;
        public const int functiontype_setflag = 53139;
        public const int functiontype_clearflag = 53140;
        public const int functiontype_toggleflag = 53141;
        public const int functiontype_npcteleporttospawnpoint = 53143;
        public const int functiontype_gotolastsavepoint = 53144;
        public const int functiontype_npcfakeattackontarget = 53145;
        public const int functiontype_npcenabledieofboredom = 53146;
        public const int functiontype_npchatelisttargetaggroers = 53147;
        public const int functiontype_npcdisablemovement = 53148;
        public const int functiontype_areatrigger = 53149;
        public const int functiontype_mezz = 53153;
        public const int functiontype_summonplayer = 53154;
        public const int functiontype_summonteammates = 53155;
        public const int functiontype_remoteareatrigger = 53159;
        public const int functiontype_clone = 53160;
        public const int functiontype_npcclonetarget = 53161;
        public const int functiontype_resistnanostrain = 53162;
        public const int functiontype_npcsummonenemy = 53163;
        public const int functiontype_savehere = 53164;
        public const int functiontype_proxyteleport_withpethandling = 53165;
        public const int functiontype_combonamegen = 53166;
        public const int functiontype_summonpet = 53167;
        public const int functiontype_opennpcdialog = 53168;
        public const int functiontype_closenpcdialog = 53169;
        public const int functiontype_npcenablegroundtoaircombat = 53170;
        public const int functiontype_npcsetstuckdetectscheme = 53171;
        public const int functiontype_npcenablepvprules = 53172;
        public const int functiontype_landcontrolcreate = 53173;
        public const int functiontype_removetrigger = 53174;
        public const int functiontype_scalingmodify = 53175;
        public const int functiontype_organizationgrid = 53176;
        public const int functiontype_reducenanostrainduration = 53177;
        public const int functiontype_disabledefenseshield = 53178;
        public const int functiontype_npctogglefightmoderegenrate = 53179;
        public const int functiontype_tracer = 53180;
        public const int functiontype_summonpets = 53181;
        public const int functiontype_addaction = 53182;
        public const int functiontype_npctogglefov = 53183;
        public const int functiontype_modifypercentage = 53184;
        public const int functiontype_drainhit = 53185;
        public const int functiontype_lockperk = 53187;
        public const int functiontype_dialogfeedback = 53188;
        public const int functiontype_faction = 53189;
        public const int functiontype_npcsetsneakmode = 53190;
        public const int functiontype_npcmovementaction = 53191;
        public const int functiontype_spawnmonsterrot = 53192;
        public const int functiontype_polymorphattack = 53193;
        public const int functiontype_npcusespecialattackitem = 53194;
        public const int functiontype_npcfreezehatelist = 53195;
        public const int functiontype_specialhit = 53196;
        public const int functiontype_npcsetconfigstats = 53197;
        public const int functiontype_npcsetmovetotarget = 53198;
        public const int functiontype_npcsetwanderingmode = 53199;
        public const int functiontype_removenano = 53201;
        public const int functiontype_npcuniqueplayersinhatelist = 53203;
        public const int functiontype_attractorgfxeffect = 53204;
        public const int functiontype_castnanoifpossible = 53206;
        public const int functiontype_setanchor = 53208;
        public const int functiontype_recalltoanchor = 53209;
        public const int functiontype_talk = 53210;
        public const int functiontype_setscriptconfig = 53211;
        public const int functiontype_castnanoifpossibleonfighttarget = 53212;
        public const int functiontype_controlhate = 53213;
        public const int functiontype_npcsendpetstatus = 53214;
        public const int functiontype_npccastnanoifpossible = 53215;
        public const int functiontype_npccastnanoifpossibleonfighttarget = 53216;
        public const int functiontype_npctargethasitem = 53217;
        public const int functiontype_cityhouseenter = 53218;
        public const int functiontype_npcstoppetduel = 53219;
        public const int functiontype_delayedspawnnpc = 53220;
        public const int functiontype_runscript = 53221;
        public const int functiontype_addbattlestationqueue = 53222;
        public const int functiontype_registercontrolpoint = 53223;
        public const int functiontype_adddefproc = 53224;
        public const int functiontype_destroyallhumans = 53225;
        public const int functiontype_spawnquest = 53226;
        public const int functiontype_addoffproc = 53227;
        public const int functiontype_playfieldnano = 53228;
        public const int functiontype_solvequest = 53229;
        public const int functiontype_knockback = 53230;
        public const int functiontype_instancelock = 53231;
        public const int functiontype_mindcontrol = 53232;
        public const int functiontype_instancedplayercity = 53233;
        public const int functiontype_resetallperks = 53234;
        public const int functiontype_createcityguestkey = 53235;
        public const int functiontype_removenanostrain = 53236;
        public const int functiontype_undefined = 53240;
        public const int functiontype_cast_nano2 = 53242;
        #endregion

        #region Operators
        ///
        /// Operators
        /// 
        public const int operator_equalto = 0;
        public const int operator_lessthan = 1;
        public const int operator_greaterthan = 2;
        public const int operator_or = 3;
        public const int operator_and = 4;
        public const int operator_time_less = 5;
        public const int operator_time_larger = 6;
        public const int operator_item_has = 7;
        public const int operator_item_hasnot = 8;
        public const int operator_id = 9;
        public const int operator_targetid = 10;
        public const int operator_targetsignal = 11;
        public const int operator_targetstat = 12;
        public const int operator_primary_item = 13;
        public const int operator_secondary_item = 14;
        public const int operator_area_zminmax = 15;
        public const int operator_user = 16;
        public const int operator_itemanim = 17;
        public const int operator_ontarget = 18;
        public const int operator_onself = 19;
        public const int operator_signal = 20;
        public const int operator_onsecondaryitem = 21;
        public const int operator_bitand = 22;
        public const int operator_bitor = 23;
        public const int operator_unequal = 24;
        public const int operator_illegal = 25;
        public const int operator_onuser = 26;
        public const int operator_onvalidtarget = 27;
        public const int operator_oninvalidtarget = 28;
        public const int operator_onvaliduser = 29;
        public const int operator_oninvaliduser = 30;
        public const int operator_haswornitem = 31;
        public const int operator_hasnotwornitem = 32;
        public const int operator_haswieldeditem = 33;
        public const int operator_hasnotwieldeditem = 34;
        public const int operator_hasformula = 35;
        public const int operator_hasnotformula = 36;
        public const int operator_ongeneralbeholder = 37;
        public const int operator_isvalid = 38;
        public const int operator_isinvalid = 39;
        public const int operator_isalive = 40;
        public const int operator_iswithinvicinity = 41;
        public const int operator_not = 42;
        public const int operator_iswithinweaponrange = 43;
        public const int operator_isnpc = 44;
        public const int operator_isfighting = 45;
        public const int operator_isattacked = 46;
        public const int operator_isanyonelooking = 47;
        public const int operator_isfoe = 48;
        public const int operator_isindungeon = 49;
        public const int operator_issameas = 50;
        public const int operator_distanceto = 51;
        public const int operator_isinnofightingarea = 52;
        public const int operator_template_compare = 53;
        public const int operator_min_max_level_compare = 54;
        public const int operator_monstertemplate = 57;
        public const int operator_hasmaster = 58;
        public const int operator_canexecuteformulaontarget = 59;
        public const int operator_area_targetinvicinity = 60;
        public const int operator_isunderheavyattack = 61;
        public const int operator_islocationok = 62;
        public const int operator_isnottoohighlevel = 63;
        public const int operator_haschangedroomwhilefighting = 64;
        public const int operator_kullnumberof = 65;
        public const int operator_testnumpets = 66;
        public const int operator_numberofitems = 67;
        public const int operator_primarytemplate = 68;
        public const int operator_isteleporting = 69;
        public const int operator_isflying = 70;
        public const int operator_scanforstat = 71;
        public const int operator_hasmeonpetlist = 72;
        public const int operator_trickledownlarger = 73;
        public const int operator_trickledownless = 74;
        public const int operator_ispetoverequipped = 75;
        public const int operator_haspetpendingnanoformula = 76;
        public const int operator_ispet = 77;
        public const int operator_canattackchar = 79;
        public const int operator_istowercreateallowed = 80;
        public const int operator_inventoryslotisfull = 81;
        public const int operator_inventoryslotisempty = 82;
        public const int operator_candisabledefenseshield = 83;
        public const int operator_isnpcornpccontrolledpet = 84;
        public const int operator_sameasselectedtarget = 85;
        public const int operator_isplayerorplayercontrolledpet = 86;
        public const int operator_hasenterednonpvpzone = 87;
        public const int operator_uselocation = 88;
        public const int operator_isfalling = 89;
        public const int operator_isondifferentplayfield = 90;
        public const int operator_hasrunningnano = 91;
        public const int operator_hasrunningnanoline = 92;
        public const int operator_hasperk = 93;
        public const int operator_isperklocked = 94;
        public const int operator_isfactionreactionset = 95;
        public const int operator_hasmovetotarget = 96;
        public const int operator_isperkunlocked = 97;
        public const int operator_true = 98;
        public const int operator_false = 99;
        public const int operator_oncaster = 100;
        public const int operator_hasnotrunningnano = 101;
        public const int operator_hasnotrunningnanoline = 102;
        public const int operator_hasnotperk = 103;
        public const int operator_notbitand = 107;
        public const int operator_obtaineditem = 108;
        #endregion

        #region Itemflags
        ///
        /// ItemFlags
        /// 
        public const int itemflag_visible = 0x1 << 0;
        public const int itemflag_modifieddescription = 0x1 << 1;
        public const int itemflag_modifiedname = 0x1 << 2;
        public const int itemflag_canbetemplateitem = 0x1 << 3;
        public const int itemflag_turnonuse = 0x1 << 4;
        public const int itemflag_hasmultiplecount = 0x1 << 5;
        public const int itemflag_locked = 0x1 << 6;
        public const int itemflag_open = 0x1 << 7;
        public const int itemflag_itemsocialarmour = 0x1 << 8;
        public const int itemflag_tellcollision = 0x1 << 9;
        public const int itemflag_noselectionindicator = 0x1 << 10;
        public const int itemflag_useemptydestruct = 0x1 << 11;
        public const int itemflag_stationary = 0x1 << 12;
        public const int itemflag_repulsive = 0x1 << 13;
        public const int itemflag_defaulttarget = 0x1 << 14;
        public const int itemflag_itemtextureoverride = 0x1 << 15;
        public const int itemflag_null = 0x1 << 16;
        public const int itemflag_hasanimation = 0x1 << 17;
        public const int itemflag_hasrotation = 0x1 << 18;
        public const int itemflag_wantcollision = 0x1 << 19;
        public const int itemflag_wantsignals = 0x1 << 20;
        public const int itemflag_hassentfirstiir = 0x1 << 21;
        public const int itemflag_hasenergy = 0x1 << 22;
        public const int itemflag_mirrorinlefthand = 0x1 << 23;
        public const int itemflag_illegalclan = 0x1 << 24;
        public const int itemflag_illegalomni = 0x1 << 25;
        public const int itemflag_nodrop = 0x1 << 26;
        public const int itemflag_unique = 0x1 << 27;
        public const int itemflag_canbeattacked = 0x1 << 28;
        public const int itemflag_disablefalling = 0x1 << 29;
        public const int itemflag_hasdamage = 0x1 << 30;
        public const int itemflag_disablestatelcollision = 0x1 << 31;
        #endregion

        #region Function
        public class Function
        {
            public int functionType;
            public int target;
            public int tickcount;
            public int tickinterval;
            public List<object> args;
            public List<AORequirements> reqs;
            public Function()
            {
                args = new List<object>();
                reqs = new List<AORequirements>();
            }
        }


        #endregion
        #endregion

        #region Item
        public static Item interpolate(int lowID, int highID, int _QL)
        {
            Item low = new Item(lowID);
            Item high = new Item(highID);
            Item interp;
            if (_QL < low.QL)
            {
                _QL = low.QL;
            }

            if (_QL > high.QL)
            {
                _QL = high.QL;
            }

            interp = high.ShallowCopy();
            if (_QL < high.QL)
            {
                interp = low.ShallowCopy();
            }
            interp.QL = _QL;
            if ((_QL == low.QL) || (_QL == high.QL))
            {
                return interp;
            }
            int attnum = 0;

            // Effecting all attributes, even flags, it doesnt matter, High and low have always the same
            Single ival;
            Single factor = (Single)((Single)(_QL - low.QL) / (Single)(high.QL - low.QL));
            while (attnum < low.ItemAttributes.Count)
            {
                ival = (factor * (high.ItemAttributes[attnum].Value - low.ItemAttributes[attnum].Value)) + low.ItemAttributes[attnum].Value;
                interp.ItemAttributes[attnum].Value = Convert.ToInt32(ival);  // Had to go int64 cos of the flags
                attnum++;
            }

            // TODO Requirements need interpolation too
            int evnum = 0;
            int fnum;
            int anum;
            Single fval;
            while (evnum < interp.ItemEvents.Count)
            {
                fnum = 0;
                while (fnum < interp.ItemEvents[evnum].Functions.Count)
                {
                    anum = 0;
                    while (anum < interp.ItemEvents[evnum].Functions[fnum].Arguments.Count)
                    {
                        if (high.ItemEvents[evnum].Functions[fnum].Arguments[anum] is int)
                        {
                            ival = (factor * ((int)high.ItemEvents[evnum].Functions[fnum].Arguments[anum] - (int)low.ItemEvents[evnum].Functions[fnum].Arguments[anum])) + (int)low.ItemEvents[evnum].Functions[fnum].Arguments[anum];
                            interp.ItemEvents[evnum].Functions[fnum].Arguments[anum] = Convert.ToInt32(ival);
                        }
                        if (high.ItemEvents[evnum].Functions[fnum].Arguments[anum] is Single)
                        {
                            fval = (factor * ((Single)high.ItemEvents[evnum].Functions[fnum].Arguments[anum] - (Single)low.ItemEvents[evnum].Functions[fnum].Arguments[anum])) + (Single)low.ItemEvents[evnum].Functions[fnum].Arguments[anum];
                            interp.ItemEvents[evnum].Functions[fnum].Arguments[anum] = fval;
                        }
                        anum++;
                    }
                    fnum++;
                }
                evnum++;
            }
            return interp;
        }

        public class ItemAttribute
        {
            public int Stat;
            public Int64 Value;
        }


        public class Item
        {
            public int AOID;
            public string name;
            public int itemtype;
            public int QL;
            public string description;
            public int isnano;
            public List<AOItemAttribute> attack = new List<AOItemAttribute>();
            public List<AOItemAttribute> defend = new List<AOItemAttribute>();
            public List<AOItemAttribute> ItemAttributes;
            public List<AOEvents> ItemEvents;

            public bool isUseable()
            {
                return ((getItemAttribute(30) & canflag_use) > 0);
            }

            public bool isConsumable()
            {
                return ((getItemAttribute(30) & canflag_consume) > 0);
            }

            public bool isStackable()
            {
                return ((getItemAttribute(30) & canflag_stackable) > 0);
            }

            public bool isWearable()
            {
                return ((getItemAttribute(30) & canflag_wear) > 0);
            }

            #region GetWeaponStyle
            public int GetWeaponStyle()
            {
                foreach (AOItemAttribute at in this.ItemAttributes)
                {
                    if (at.Stat != 274) continue;

                    return (int)at.Value;
                }

                // Odd, no WeaponWieldFlags found...
                return 0;
            }
            #endregion

            #region Get Override Texture

            public int GetOverrideTexture()
            {
                foreach (AOItemAttribute attr in this.ItemAttributes)
                {
                    if (attr.Stat != 336) continue;

                    return (int)attr.Value;
                }

                // No Override OH NOES!
                return 0;
            }

            #endregion

            public Item(int ID)
            {
                ItemAttributes = new List<AOItemAttribute>();
                ItemEvents = new List<AOEvents>();
                SqlWrapper ms = new SqlWrapper();

                DataTable dt = ms.ReadDatatable("SELECT * FROM items WHERE AOID='" + ID.ToString() + "'");

                if (dt.Rows.Count > 0)
                {
                    AOID = (Int32)dt.Rows[0]["AOID"];
                    isnano = (Int32)dt.Rows[0]["IsNano"];
                    QL = (Int32)dt.Rows[0]["QL"];
                    itemtype = (Int32)dt.Rows[0]["ItemType"];
                    byte[] blob = (byte[])dt.Rows[0]["EFR"];

                    int blobc = 0;

                    // Read Attack Stat/Values
                    int cc = BitConverter.ToInt32(blob, blobc);
                    AOItemAttribute mm_a;
                    blobc += 4;
                    while (cc > 0)
                    {
                        mm_a = new AOItemAttribute();
                        mm_a.Stat = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        mm_a.Value = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        attack.Add(mm_a);
                        cc--;
                    }

                    // Read Defend Stat/Values
                    cc = BitConverter.ToInt32(blob, blobc);
                    blobc += 4;
                    while (cc > 0)
                    {
                        mm_a = new AOItemAttribute();
                        mm_a.Stat = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        mm_a.Value = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        defend.Add(mm_a);
                        cc--;
                    }

                    // Read Item Attributes
                    int c = BitConverter.ToInt32(blob, blobc);
                    blobc += 4;

                    AOItemAttribute tempa;
                    while (c > 0)
                    {
                        tempa = new AOItemAttribute();
                        tempa.Stat = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        tempa.Value = BitConverter.ToInt32(blob, blobc);
                        blobc += 4;
                        ItemAttributes.Add(tempa);
                        c--;
                    }

                    // Read Item Events
                    AOEvents tempe;
                    c = BitConverter.ToInt32(blob, blobc);
                    blobc += 4;
                    while (c > 0)
                    {
                        tempe = new AOEvents();
                        blobc = tempe.readEventfromBlob(blob, blobc);
                        ItemEvents.Add(tempe);
                        c--;
                    }

                    /*                    // Read Item Actions
                                        AOActions tempac;
                                        c = BitConverter.ToInt32(blob, blobc);
                                        blobc += 4;
                                        while (c > 0)
                                        {
                                            tempac = new AOActions();
                                            blobc = tempac.readActionfromBlob(ref blob, blobc);
                                            c--;
                                        }
                     */
                }
                else
                {
                    // Setting QL to -1 as "invalid item" flag
                    QL = -1;
                }
            }



            public Item ShallowCopy()
            {
                return (Item)this.MemberwiseClone();
            }

            public Int64 getItemAttribute(int number)
            {
                int c;
                for (c = 0; c < ItemAttributes.Count; c++)
                {
                    if (number == ItemAttributes[c].Stat)
                    {
                        return ItemAttributes[c].Value;
                    }
                }
                return 0;
            }

        }
        #endregion

    }
}