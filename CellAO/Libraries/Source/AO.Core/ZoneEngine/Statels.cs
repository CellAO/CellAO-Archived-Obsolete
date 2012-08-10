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
using AO.Core;
using System.Data;

namespace ZoneEngine
{
    public class coordheading
    {
        public AOCoord Coordinates = new AOCoord();
        public Quaternion Heading = new Quaternion(0, 0, 0, 0);
    }
    public class Statels
    {
        public static DataTable dt_functions;
        public static DataTable dt_events;
        public static DataTable dt_reqs;
        public static DataTable dt_args;
        public static int cnt_args = 0;
        public static int cnt_reqs = 0;
        public static int cnt_events = 0;
        public static int cnt_functions = 0;


        public class Statel_Event
        {
            public int EventNumber = 0;
            public List<Statel_Function> Functions = new List<Statel_Function>();

            public void LoadFunctions(Int32 eventid, Int32 statelid)
            {
                while ((cnt_functions < dt_functions.Rows.Count) && ((Int32)dt_functions.Rows[cnt_functions][1] == eventid) && ((Int32)dt_functions.Rows[cnt_functions][2] == statelid))
                {
                    Statel_Function sf = new Statel_Function();
                    sf.FunctionNumber = (Int32)dt_functions.Rows[cnt_functions][3];
                    sf.Target = (Int32)dt_functions.Rows[cnt_functions][4];
                    sf.TickCount = (Int32)dt_functions.Rows[cnt_functions][5];
                    sf.TickInterval = (Int32)dt_functions.Rows[cnt_functions][6];
                    sf.LoadRequirements((Int32)dt_functions.Rows[cnt_functions][0], eventid, statelid);
                    sf.LoadArguments((Int32)dt_functions.Rows[cnt_functions][0], eventid, statelid);
                    Functions.Add(sf);
                    cnt_functions++;
                }
            }
            public override string ToString()
            {
                string s = "Eventnum: " + EventNumber + "\r\n";
                foreach (Statel_Function sf in Functions)
                {
                    s = s + sf.ToString();
                }
                return s;
            }
        }

        public class Statel_Function
        {
            public int FunctionNumber = 0;
            public List<string> Arguments = new List<string>();
            public List<Statel_Function_Requirement> Requirements = new List<Statel_Function_Requirement>();
            public int Target = 0;
            public int TickCount = 0;
            public int TickInterval = 0;

            public void LoadRequirements(int functionid, int eventid, int statelid)
            {
                while ((cnt_reqs < dt_reqs.Rows.Count) && ((Int32)dt_reqs.Rows[cnt_reqs][1] == functionid) && ((Int32)dt_reqs.Rows[cnt_reqs][2] == eventid) && ((Int32)dt_reqs.Rows[cnt_reqs][3] == statelid))
                {
                    Statel_Function_Requirement sfr = new Statel_Function_Requirement();
                    sfr.AttributeNumber = (Int32)dt_reqs.Rows[cnt_reqs][4];
                    sfr.AttributeValue = (Int32)dt_reqs.Rows[cnt_reqs][5];
                    sfr.Operator = (Int32)dt_reqs.Rows[cnt_reqs][6];
                    sfr.ChildOperator = (Int32)dt_reqs.Rows[cnt_reqs][7];
                    sfr.Target = (Int32)dt_reqs.Rows[cnt_reqs][8];
                    Requirements.Add(sfr);
                    cnt_reqs++;
                }
            }

            public void LoadArguments(int functionid, int eventid, int statelid)
            {
                while ((cnt_args < dt_args.Rows.Count) && ((Int32)dt_args.Rows[cnt_args][1] == functionid) && ((Int32)dt_args.Rows[cnt_args][2] == eventid) && ((Int32)dt_args.Rows[cnt_args][3] == statelid))
                {
                    Arguments.Add((string)dt_args.Rows[cnt_args][4]);
                    cnt_args++;
                }
            }

            public override string ToString()
            {
                string s = "Function " + FunctionNumber + " TickCount " + TickCount + " TickInterval " + TickInterval + " Target " + Target + "\r\n";
                foreach (string arg in Arguments)
                {
                    s = s + "Arg: " + arg + "\r\n";
                }
                return s;
            }

            public coordheading FindEntry(int Playfield, int DestinationNumber)
            {
                coordheading ret = new coordheading();
                ret.Coordinates.x = -1;
                foreach (Collision.WallCollision.Line l in Collision.WallCollision.destinations[Playfield].playfield.lines)
                {
                    if (l.ID != DestinationNumber)
                    {
                        continue;
                    }
                    ret.Coordinates.x = (l.start.X + l.end.X) / 2;
                    ret.Coordinates.y = (l.start.Y + l.end.Y) / 2;
                    ret.Coordinates.z = (l.start.Z + l.end.Z) / 2;
                    // TODO: Calculate the right Quaternion for the heading...
                    // - Algorithman
                    Quaternion q = new Quaternion(new Vector3((l.end.X - l.start.X), 1, -(l.end.Z - l.start.Z)));
                    ret.Heading.x = q.x;
                    ret.Heading.y = q.y;
                    ret.Heading.z = q.z;
                    ret.Heading.w = q.w;
                }
                return ret;
            }

            public void Execute(Client cli, Statel parent, int Eventnumber)
            {
                switch (FunctionNumber)
                {
                    // Hit
                    case 53002:
                        {
                            int statnum = Int32.Parse(Arguments.ElementAt(0));
                            int min = Int32.Parse(Arguments.ElementAt(1));
                            int max = Int32.Parse(Arguments.ElementAt(2));
                            if (min > max)
                            {
                                min = max;
                                max = Int32.Parse(Arguments.ElementAt(1));
                            }
                            Random rnd = new Random();
                            cli.Character.Stats.Set(statnum, (uint)(cli.Character.Stats.Get(statnum) + rnd.Next(min, max)));
                            break;
                        }
                    // Lineteleport
                    //
                    case 53059:
                        {
#if DEBUG
                            Console.WriteLine("Function 53059 (LineTeleport)");
                            Console.WriteLine("Object: " + parent.Type + ":" + parent.Instance);
#endif
                            uint arg2 = UInt32.Parse(Arguments.ElementAt(1)); // Linesegment and playfield (lower word)
                            arg2 = arg2 >> 16;
                            int to_pf = Int32.Parse(Arguments.ElementAt(2));
                            coordheading a = FindEntry(to_pf, (Int32)arg2);
                            if (a.Coordinates.x != -1)
                            {
                                cli.Teleport(a.Coordinates, a.Heading, to_pf);
                                break;
                            }
                            break;
                        }
                    case 53082: // Teleport Proxy
                        {
                            Identity pfinstance = new Identity();
                            pfinstance.Type = Int32.Parse(Arguments.ElementAt(0));
                            pfinstance.Instance = Int32.Parse(Arguments.ElementAt(1));
                            Identity id2 = new Identity();
                            id2.Type = Int32.Parse(Arguments.ElementAt(2));
                            id2.Instance = Int32.Parse(Arguments.ElementAt(3));
                            Identity id3 = new Identity();
                            id3.Type = Int32.Parse(Arguments.ElementAt(4));
                            id3.Instance = Int32.Parse(Arguments.ElementAt(5));

                            SqlWrapper ms = new SqlWrapper();
                            DataTable dt = ms.ReadDT("SELECT * from proxydestinations WHERE playfield=" + pfinstance.Instance);
                            if (dt.Rows.Count == 0)
                            {
#if DEBUG
                                cli.SendChatText("No Destination found for playfield " + pfinstance.Instance);
                                cli.SendChatText("Statel " + parent.Type.ToString() + ":" + parent.Instance.ToString() + " handling " + Eventnumber.ToString() + " Function " + FunctionNumber.ToString() + " " + cli.Character.Coordinates.ToString());
                                foreach (string arg in Arguments)
                                {
                                    cli.SendChatText("Argument: " + arg);
                                }
#endif
                            }
                            else
                            {
                                AOCoord a = new AOCoord();
                                a.x= (Single)dt.Rows[0][1];
                                a.y= (Single)dt.Rows[0][2];
                                a.z = (Single)dt.Rows[0][3];
                                Quaternion q = new Quaternion(0, 0, 0, 0);
                                q.x = (Single)dt.Rows[0][4];
                                q.y = (Single)dt.Rows[0][5];
                                q.z = (Single)dt.Rows[0][6];
                                q.w = (Single)dt.Rows[0][7];
                                cli.TeleportProxy(a, q, pfinstance.Instance, pfinstance, 1, (Int32)parent.Instance, id2, id3);
                            }
                            break;
                        }
                    case 53092: // Bank
                        {
                            Packets.BankOpen.Send(cli);
                            break;
                        }

                    case 53083: // Teleport Proxy 2
                        {

                            Identity pfinstance = new Identity();
                            pfinstance.Type = Int32.Parse(Arguments.ElementAt(0));
                            pfinstance.Instance = Int32.Parse(Arguments.ElementAt(1));
                            int gs = 1;
                            int sg = 0;
                            Identity R = new Identity();
                            R.Type = Int32.Parse(Arguments.ElementAt(2));
                            R.Instance = Int32.Parse(Arguments.ElementAt(3));
                            Identity dest = new Identity();
                            dest.Type = Int32.Parse(Arguments.ElementAt(4));
                            dest.Instance = Int32.Parse(Arguments.ElementAt(5));
                            int to_pf = (Int32)((UInt32)(dest.Instance & 0xffff));
                            int arg2 = (Int32)((UInt32)(dest.Instance >> 16));
                            coordheading a = FindEntry(to_pf, arg2);

                            if (a.Coordinates.x != -1)
                            {
                                cli.TeleportProxy(a.Coordinates, a.Heading, to_pf, pfinstance, gs, sg, R, dest);
                                break;
                            }
                            break;
                        }
                        // Teleport
                    case 53016:
                        {
                            Quaternion q = new Quaternion(0, 1, 0, 0);
                            AOCoord a = new AOCoord();
                            a.x = Int32.Parse(Arguments.ElementAt(0));
                            a.y = Int32.Parse(Arguments.ElementAt(1));
                            a.z = Int32.Parse(Arguments.ElementAt(2));
                            cli.Teleport(a, q, Int32.Parse(Arguments.ElementAt(3)));
                            break;
                        }

                    case 53044:
                        {
                            string text = Arguments.ElementAt(0);
                            Packets.SystemText.Send(cli, text, 0);
                            break;
                        }
                    default:
                        {
#if DEBUG
                            cli.SendChatText("Statel " + parent.Type.ToString() + ":" + parent.Instance.ToString() + " handling " + Eventnumber.ToString() + " Function " + FunctionNumber.ToString() + " " + cli.Character.Coordinates.ToString());
                            foreach (string arg in Arguments)
                            {
                                cli.SendChatText("Argument: " + arg);
                            }
#endif
                            break;
                        }

                }
            }
        }

        public class Statel_Function_Requirement
        {
            public int AttributeNumber = 0;
            public int AttributeValue = 0;
            public int Operator = 0;
            public int ChildOperator = 255;
            public int Target = 0;
        }

        public static Dictionary<Int32, List<Statel>> Statelppf = new Dictionary<int, List<Statel>>();
        public static Dictionary<Int32, List<Statel>> StatelppfonEnter = new Dictionary<int, List<Statel>>();
        public static Dictionary<Int32, List<Statel>> StatelppfonUse = new Dictionary<int, List<Statel>>();

        public static int CacheAllStatels()
        {
            SqlWrapper ms = new SqlWrapper();
            int count = 0;
            DataTable dt = ms.ReadDT("SELECT * FROM statels ORDER BY id ASC");
            dt_args = ms.ReadDT("SELECT * FROM statel_function_arguments ORDER BY statel_id, event_id, function_id, attrid ASC");
            dt_events = ms.ReadDT("SELECT * FROM statel_events ORDER BY statel_id, eventid ASC");
            dt_reqs = ms.ReadDT("SELECT * FROM statel_function_reqs ORDER BY statel_id, event_id, function_id, reqid ASC");
            dt_functions = ms.ReadDT("SELECT * FROM statel_functions ORDER BY statel_id, event_id, functionid ASC");
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
                    if ((e.EventNumber == ItemHandler.eventtype_onenter) || (e.EventNumber == ItemHandler.eventtype_ontargetinvicinity))
                    {
                        StatelppfonEnter[pf].Add(tempstatel);
                    }
                    if (e.EventNumber == ItemHandler.eventtype_onuse)
                    {
                        StatelppfonUse[pf].Add(tempstatel);
                    }
                }
                count++;
                if ((count % 10) == 0)
                {
                    Console.Write("\rReading statels: " + count.ToString() + "/" + maxcount.ToString() + "                        \r");
                }
            }
            
            ms.sqlclose();
            Console.Write("                                               \r");
            dt_args.Clear();
            dt_events.Clear();
            dt_functions.Clear();
            dt_reqs.Clear();
            dt_args = null;
            dt_events = null;
            dt_functions = null;
            dt_reqs = null;
            dt.Clear();
            return count;
        }

        public class Statel
        {

            public List<Statel_Event> Events = new List<Statel_Event>();
            public uint Instance = 0;
            public int Type = 0;
            public int Template = 0;
            public int PlayField = 0;
            public AOCoord Coordinates = new AOCoord();

            public void LoadEvents(Int32 statelid)
            {

                while ((cnt_events < dt_events.Rows.Count) && ((Int32)dt_events.Rows[cnt_events][1] == statelid))
                {
                    Statel_Event e = new Statel_Event();
                    e.EventNumber = (Int32)dt_events.Rows[cnt_events][2];
                    e.LoadFunctions((Int32)dt_events.Rows[cnt_events][0], statelid);
                    Events.Add(e);
                    cnt_events++;
                }
            }


            public bool onEnter(Client cli)
            {
                foreach (Statel_Event e in Events)
                {
                    if (e.EventNumber != ItemHandler.eventtype_onenter)
                    {
                        continue;
                    }
                    if ((AOCoord.distance2D(cli.Character.Coordinates, Coordinates) < 0.8f) && (Math.Abs(cli.Character.Coordinates.y - Coordinates.y) < 5))
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
                foreach (Statel_Event e in Events)
                {
                    if (e.EventNumber != ItemHandler.eventtype_ontargetinvicinity)
                    {
                        continue;
                    }
                    if ((AOCoord.distance2D(cli.Character.Coordinates, Coordinates) < 1.2f) && (Math.Abs(cli.Character.Coordinates.y - Coordinates.y) < 5))
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
                foreach (Statel_Event e in Events)
                {
                    if (e.EventNumber != ItemHandler.eventtype_onuse)
                    {
                        continue;
                    }
                    if (((UInt32)target.Instance != Instance) || (target.Type != Type))
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

