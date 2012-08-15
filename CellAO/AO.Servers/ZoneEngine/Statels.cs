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

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using ZoneEngine.Collision;
    using ZoneEngine.Misc;

    /// <summary>
    /// Combined class for coordinates and heading
    /// </summary>
    public class coordheading
    {
        public AOCoord Coordinates = new AOCoord();

        public Quaternion Heading = new Quaternion(0, 0, 0, 0);
    }

    /// <summary>
    /// Class for static game objects (bank terminals, static shops etc)
    /// </summary>
    public class Statels
    {
        private static DataTable dt_functions;

        private static DataTable dt_events;

        private static DataTable dt_reqs;

        public static DataTable dt_args;

        public static int cnt_args;

        public static int cnt_reqs;

        public static int cnt_events;

        public static int cnt_functions;

        /// <summary>
        /// Statel event class
        /// </summary>
        public class Statel_Event
        {
            public int EventNumber;

            public List<Statel_Function> Functions = new List<Statel_Function>();

            /// <summary>
            /// Load event functions
            /// </summary>
            /// <param name="eventid">Event ID</param>
            /// <param name="statelid">Statel ID</param>
            public void LoadFunctions(Int32 eventid, Int32 statelid)
            {
                while ((cnt_functions < dt_functions.Rows.Count)
                       && ((Int32)dt_functions.Rows[cnt_functions][1] == eventid)
                       && ((Int32)dt_functions.Rows[cnt_functions][2] == statelid))
                {
                    Statel_Function sf = new Statel_Function();
                    sf.FunctionNumber = (Int32)dt_functions.Rows[cnt_functions][3];
                    sf.Target = (Int32)dt_functions.Rows[cnt_functions][4];
                    sf.TickCount = (Int32)dt_functions.Rows[cnt_functions][5];
                    sf.TickInterval = (Int32)dt_functions.Rows[cnt_functions][6];
                    sf.LoadRequirements((Int32)dt_functions.Rows[cnt_functions][0], eventid, statelid);
                    sf.LoadArguments((Int32)dt_functions.Rows[cnt_functions][0], eventid, statelid);
                    this.Functions.Add(sf);
                    cnt_functions++;
                }
            }

            public override string ToString()
            {
                string s = "Eventnum: " + this.EventNumber + "\r\n";
                foreach (Statel_Function sf in this.Functions)
                {
                    s = s + sf;
                }
                return s;
            }
        }

        /// <summary>
        /// Statel function handler class
        /// </summary>
        public class Statel_Function
        {
            #region Operators
            ///
            /// Operators
            /// 
            private const int operator_equalto = 0;

            private const int operator_lessthan = 1;

            private const int operator_greaterthan = 2;

            private const int operator_or = 3;

            private const int operator_and = 4;

            private const int operator_time_less = 5;

            private const int operator_time_larger = 6;

            private const int operator_item_has = 7;

            private const int operator_item_hasnot = 8;

            private const int operator_id = 9;

            private const int operator_targetid = 10;

            private const int operator_targetsignal = 11;

            private const int operator_targetstat = 12;

            private const int operator_primary_item = 13;

            private const int operator_secondary_item = 14;

            private const int operator_area_zminmax = 15;

            private const int operator_user = 16;

            private const int operator_itemanim = 17;

            private const int operator_ontarget = 18;

            private const int operator_onself = 19;

            private const int operator_signal = 20;

            private const int operator_onsecondaryitem = 21;

            private const int operator_bitand = 22;

            private const int operator_bitor = 23;

            private const int operator_unequal = 24;

            private const int operator_illegal = 25;

            private const int operator_onuser = 26;

            private const int operator_onvalidtarget = 27;

            private const int operator_oninvalidtarget = 28;

            private const int operator_onvaliduser = 29;

            private const int operator_oninvaliduser = 30;

            private const int operator_haswornitem = 31;

            private const int operator_hasnotwornitem = 32;

            private const int operator_haswieldeditem = 33;

            private const int operator_hasnotwieldeditem = 34;

            private const int operator_hasformula = 35;

            private const int operator_hasnotformula = 36;

            private const int operator_ongeneralbeholder = 37;

            private const int operator_isvalid = 38;

            private const int operator_isinvalid = 39;

            private const int operator_isalive = 40;

            private const int operator_iswithinvicinity = 41;

            private const int operator_not = 42;

            private const int operator_iswithinweaponrange = 43;

            private const int operator_isnpc = 44;

            private const int operator_isfighting = 45;

            private const int operator_isattacked = 46;

            private const int operator_isanyonelooking = 47;

            private const int operator_isfoe = 48;

            private const int operator_isindungeon = 49;

            private const int operator_issameas = 50;

            private const int operator_distanceto = 51;

            private const int operator_isinnofightingarea = 52;

            private const int operator_template_compare = 53;

            private const int operator_min_max_level_compare = 54;

            private const int operator_monstertemplate = 57;

            private const int operator_hasmaster = 58;

            private const int operator_canexecuteformulaontarget = 59;

            private const int operator_area_targetinvicinity = 60;

            private const int operator_isunderheavyattack = 61;

            private const int operator_islocationok = 62;

            private const int operator_isnottoohighlevel = 63;

            private const int operator_haschangedroomwhilefighting = 64;

            private const int operator_kullnumberof = 65;

            private const int operator_testnumpets = 66;

            private const int operator_numberofitems = 67;

            private const int operator_primarytemplate = 68;

            private const int operator_isteleporting = 69;

            private const int operator_isflying = 70;

            private const int operator_scanforstat = 71;

            private const int operator_hasmeonpetlist = 72;

            private const int operator_trickledownlarger = 73;

            private const int operator_trickledownless = 74;

            private const int operator_ispetoverequipped = 75;

            private const int operator_haspetpendingnanoformula = 76;

            private const int operator_ispet = 77;

            private const int operator_canattackchar = 79;

            private const int operator_istowercreateallowed = 80;

            private const int operator_inventoryslotisfull = 81;

            private const int operator_inventoryslotisempty = 82;

            private const int operator_candisabledefenseshield = 83;

            private const int operator_isnpcornpccontrolledpet = 84;

            private const int operator_sameasselectedtarget = 85;

            private const int operator_isplayerorplayercontrolledpet = 86;

            private const int operator_hasenterednonpvpzone = 87;

            private const int operator_uselocation = 88;

            private const int operator_isfalling = 89;

            private const int operator_isondifferentplayfield = 90;

            private const int operator_hasrunningnano = 91;

            private const int operator_hasrunningnanoline = 92;

            private const int operator_hasperk = 93;

            private const int operator_isperklocked = 94;

            private const int operator_isfactionreactionset = 95;

            private const int operator_hasmovetotarget = 96;

            private const int operator_isperkunlocked = 97;

            private const int operator_true = 98;

            private const int operator_false = 99;

            private const int operator_oncaster = 100;

            private const int operator_hasnotrunningnano = 101;

            private const int operator_hasnotrunningnanoline = 102;

            private const int operator_hasnotperk = 103;

            private const int operator_notbitand = 107;

            private const int operator_obtaineditem = 108;
            #endregion

            #region Targets
            ///
            /// Targets
            /// 
            private const int itemtarget_user = 1;

            private const int itemtarget_wearer = 2;

            private const int itemtarget_target = 3;

            private const int itemtarget_fightingtarget = 14;

            private const int itemtarget_self = 19;

            private const int itemtarget_selectedtarget = 23;
            #endregion

            public int FunctionNumber;

            /// <summary>
            /// Function arguments list
            /// </summary>
            public List<string> Arguments = new List<string>();

            /// <summary>
            /// Function requirements list
            /// </summary>
            public List<Statel_Function_Requirement> Requirements = new List<Statel_Function_Requirement>();

            public int Target;

            public int TickCount;

            public int TickInterval;

            /// <summary>
            /// Read function requirements
            /// </summary>
            /// <param name="functionid">Function ID</param>
            /// <param name="eventid">Event ID</param>
            /// <param name="statelid">Statel ID</param>
            public void LoadRequirements(int functionid, int eventid, int statelid)
            {
                while ((cnt_reqs < dt_reqs.Rows.Count) && ((Int32)dt_reqs.Rows[cnt_reqs][1] == functionid)
                       && ((Int32)dt_reqs.Rows[cnt_reqs][2] == eventid)
                       && ((Int32)dt_reqs.Rows[cnt_reqs][3] == statelid))
                {
                    Statel_Function_Requirement sfr = new Statel_Function_Requirement();
                    sfr.AttributeNumber = (Int32)dt_reqs.Rows[cnt_reqs][4];
                    sfr.AttributeValue = (Int32)dt_reqs.Rows[cnt_reqs][5];
                    sfr.Operator = (Int32)dt_reqs.Rows[cnt_reqs][6];
                    sfr.ChildOperator = (Int32)dt_reqs.Rows[cnt_reqs][7];
                    sfr.Target = (Int32)dt_reqs.Rows[cnt_reqs][8];
                    this.Requirements.Add(sfr);
                    cnt_reqs++;
                }
            }

            /// <summary>
            /// Read function arguments
            /// </summary>
            /// <param name="functionid">Function ID</param>
            /// <param name="eventid">Event ID</param>
            /// <param name="statelid">Statel ID</param>
            public void LoadArguments(int functionid, int eventid, int statelid)
            {
                while ((cnt_args < dt_args.Rows.Count) && ((Int32)dt_args.Rows[cnt_args][1] == functionid)
                       && ((Int32)dt_args.Rows[cnt_args][2] == eventid)
                       && ((Int32)dt_args.Rows[cnt_args][3] == statelid))
                {
                    this.Arguments.Add((string)dt_args.Rows[cnt_args][4]);
                    cnt_args++;
                }
            }

            public override string ToString()
            {
                string s = "Function " + this.FunctionNumber + " TickCount " + this.TickCount + " TickInterval "
                           + this.TickInterval + " Target " + this.Target + "\r\n";
                foreach (string arg in this.Arguments)
                {
                    s = s + "Arg: " + arg + "\r\n";
                }
                return s;
            }

            public coordheading FindEntry(int Playfield, int DestinationNumber)
            {
                coordheading ret = new coordheading();
                ret.Coordinates.x = -1;
                foreach (WallCollision.Line l in WallCollision.Destinations[Playfield].Playfield.Lines)
                {
                    if (l.ID != DestinationNumber)
                    {
                        continue;
                    }
                    ret.Coordinates.x = (l.LineStartPoint.X + l.LineEndPoint.X) / 2;
                    ret.Coordinates.y = (l.LineStartPoint.Y + l.LineEndPoint.Y) / 2;
                    ret.Coordinates.z = (l.LineStartPoint.Z + l.LineEndPoint.Z) / 2;
                    // TODO: Calculate the right Quaternion for the heading...
                    // - Algorithman
                    Quaternion q = new Quaternion(new Vector3((l.LineEndPoint.X - l.LineStartPoint.X), 1, -(l.LineEndPoint.Z - l.LineStartPoint.Z)));
                    ret.Heading.x = q.x;
                    ret.Heading.y = q.y;
                    ret.Heading.z = q.z;
                    ret.Heading.w = q.w;
                }
                return ret;
            }

            public void Execute(Client cli, Statel parent, int Eventnumber)
            {
                bool reqs_met = true;
                int childop = -1;
                Character ftarget = null;
                bool reqresult = true;
                Character chartarget =
                    (Character)FindDynel.FindDynelByID(cli.Character.Target.Type, cli.Character.Target.Instance);

                for (int r = 0; r < this.Requirements.Count; r++)
                {
                    switch (this.Requirements[r].Target)
                    {
                        case itemtarget_user:
                            ftarget = cli.Character;
                            break;
                        case itemtarget_wearer:
                            ftarget = cli.Character;
                            break;
                        case itemtarget_target:
                            ftarget = chartarget;
                            break;
                        case itemtarget_fightingtarget:
                            // Fighting target
                            break;
                        case itemtarget_self:
                            ftarget = cli.Character;
                            break;
                        case itemtarget_selectedtarget:
                            ftarget = chartarget;
                            break;
                    }
                    if (ftarget == null)
                    {
                        reqs_met = false;
                        return;
                    }
                    int statval = ftarget.Stats.Get(this.Requirements[r].AttributeNumber);
                    switch (this.Requirements[r].Operator)
                    {
                        case operator_and:
                            reqresult = ((statval & this.Requirements[r].AttributeValue) != 0);
                            break;
                        case operator_or:
                            reqresult = ((statval | this.Requirements[r].AttributeValue) != 0);
                            break;
                        case operator_equalto:
                            reqresult = (statval == this.Requirements[r].AttributeValue);
                            break;
                        case operator_lessthan:
                            reqresult = (statval < this.Requirements[r].AttributeValue);
                            break;
                        case operator_greaterthan:
                            reqresult = (statval > this.Requirements[r].AttributeValue);
                            break;
                        case operator_unequal:
                            reqresult = (statval != this.Requirements[r].AttributeValue);
                            break;
                        case operator_true:
                            reqresult = (statval != 0);
                            break;
                        case operator_false:
                            reqresult = (statval == 0);
                            break;
                        case operator_bitand:
                            reqresult = ((statval & this.Requirements[r].AttributeValue) != 0);
                            break;
                        case operator_bitor:
                            reqresult = ((statval | this.Requirements[r].AttributeValue) != 0);
                            break;
                        default:
                            reqresult = true;
                            break;
                    }

                    switch (childop)
                    {
                        case operator_and:
                            reqs_met &= reqresult;
                            break;
                        case operator_or:
                            reqs_met |= reqresult;
                            break;
                        case -1:
                            reqs_met = reqresult;
                            break;
                        default:
                            break;
                    }
                    childop = this.Requirements[r].ChildOperator;
                }

                if (!reqs_met)
                {
                    cli.SendChatText("Requirements not met. (better errormessage not coded yet)");
                    return;
                }

                if (
                    !Program.FunctionC.CallFunction(
                        this.FunctionNumber, cli.Character, cli.Character, parent, this.Arguments.ToArray()))
                {
#if DEBUG
                    cli.SendChatText(
                        "Statel " + parent.Type.ToString() + ":" + parent.Instance.ToString() + " handling "
                        + Eventnumber.ToString() + " Function " + this.FunctionNumber.ToString() + " "
                        + cli.Character.Coordinates);
                    foreach (string arg in this.Arguments)
                    {
                        cli.SendChatText("Argument: " + arg);
                    }
#endif
                }
            }
        }

        public class Statel_Function_Requirement
        {
            public int AttributeNumber;

            public int AttributeValue;

            public int Operator;

            public int ChildOperator = 255;

            public int Target;
        }

        public static Dictionary<Int32, List<Statel>> Statelppf = new Dictionary<int, List<Statel>>();

        public static Dictionary<Int32, List<Statel>> StatelppfonEnter = new Dictionary<int, List<Statel>>();

        public static Dictionary<Int32, List<Statel>> StatelppfonUse = new Dictionary<int, List<Statel>>();

        public static int CacheAllStatels()
        {
            SqlWrapper ms = new SqlWrapper();
            int count = 0;
            DataTable dt = ms.ReadDatatable("SELECT * FROM statels ORDER BY id ASC");
            dt_args =
                ms.ReadDatatable(
                    "SELECT * FROM statel_function_arguments ORDER BY statel_id, event_id, function_id, attrid ASC");
            dt_events = ms.ReadDatatable("SELECT * FROM statel_events ORDER BY statel_id, eventid ASC");
            dt_reqs =
                ms.ReadDatatable("SELECT * FROM statel_function_reqs ORDER BY statel_id, event_id, function_id, reqid ASC");
            dt_functions = ms.ReadDatatable("SELECT * FROM statel_functions ORDER BY statel_id, event_id, functionid ASC");
            int maxcount = 0;
            ms.sqlclose();

            List<Statel> temp;
            maxcount = dt.Rows.Count;
            foreach (DataRow dr in dt.Rows)
            {
                int pf = (Int32)dr[0];
                if (Statelppf.ContainsKey(pf) == false)
                {
                    temp = new List<Statel>();
                    Statelppf.Add(pf, temp);
                    temp = new List<Statel>();
                    StatelppfonEnter.Add(pf, temp);
                    temp = new List<Statel>();
                    StatelppfonUse.Add(pf, temp);
                }
                Statel tempstatel = new Statel();
                tempstatel.Coordinates.x = (Single)dr[5];
                tempstatel.Coordinates.y = (Single)dr[6];
                tempstatel.Coordinates.z = (Single)dr[7];
                tempstatel.Type = (Int32)dr[1];
                tempstatel.Instance = (UInt32)dr[2];
                tempstatel.PlayField = (Int32)dr[0];
                tempstatel.Template = (Int32)dr[3];

                tempstatel.LoadEvents((Int32)dr[4]);
                Statelppf[pf].Add(tempstatel);

                foreach (Statel_Event e in tempstatel.Events)
                {
                    if ((e.EventNumber == Constants.eventtype_onenter)
                        || (e.EventNumber == Constants.eventtype_ontargetinvicinity))
                    {
                        StatelppfonEnter[pf].Add(tempstatel);
                    }
                    if (e.EventNumber == Constants.eventtype_onuse)
                    {
                        StatelppfonUse[pf].Add(tempstatel);
                    }
                }
                count++;
                if ((count % 10) == 0)
                {
                    Console.Write(
                        "\rReading statels: " + count.ToString() + "/" + maxcount.ToString()
                        + "                        \r");
                }
            }

            ms.sqlclose();
            Console.Write("                                               \r");
            return count;
        }

        public class Statel
        {
            public List<Statel_Event> Events = new List<Statel_Event>();

            public uint Instance;

            public int Type;

            public int Template;

            public int PlayField;

            public AOCoord Coordinates = new AOCoord();

            public void LoadEvents(Int32 statelid)
            {
                while ((cnt_events < dt_events.Rows.Count) && ((Int32)dt_events.Rows[cnt_events][1] == statelid))
                {
                    Statel_Event e = new Statel_Event();
                    e.EventNumber = (Int32)dt_events.Rows[cnt_events][2];
                    e.LoadFunctions((Int32)dt_events.Rows[cnt_events][0], statelid);
                    this.Events.Add(e);
                    cnt_events++;
                }
            }

            public bool onEnter(Client cli)
            {
                foreach (Statel_Event e in this.Events)
                {
                    if (e.EventNumber != Constants.eventtype_onenter)
                    {
                        continue;
                    }
                    if ((AOCoord.distance2D(cli.Character.Coordinates, this.Coordinates) < 0.8f)
                        && (Math.Abs(cli.Character.Coordinates.y - this.Coordinates.y) < 5))
                    {
                        foreach (Statel_Function f in e.Functions)
                        {
                            f.Execute(cli, this, e.EventNumber);
                        }
                        return true;
                    }
                }
                return false;
            }

            public bool onTargetinVicinity(Client cli)
            {
                foreach (Statel_Event e in this.Events)
                {
                    if (e.EventNumber != Constants.eventtype_ontargetinvicinity)
                    {
                        continue;
                    }
                    if ((AOCoord.distance2D(cli.Character.Coordinates, this.Coordinates) < 1.2f)
                        && (Math.Abs(cli.Character.Coordinates.y - this.Coordinates.y) < 5))
                    {
                        foreach (Statel_Function f in e.Functions)
                        {
                            f.Execute(cli, this, e.EventNumber);
                        }
                        return true;
                    }
                }
                return false;
            }

            public bool onUse(Client cli, Identity target)
            {
                foreach (Statel_Event e in this.Events)
                {
                    if (e.EventNumber != Constants.eventtype_onuse)
                    {
                        continue;
                    }
                    if (((UInt32)target.Instance != this.Instance) || (target.Type != this.Type))
                    {
                        continue;
                    }
                    foreach (Statel_Function f in e.Functions)
                    {
                        f.Execute(cli, this, e.EventNumber);
                    }
                    return true;
                }
                return false;
            }
        }
    }
}