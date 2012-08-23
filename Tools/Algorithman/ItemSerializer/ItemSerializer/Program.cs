using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using AO.Core;
using System.Data;
using ComponentAce.Compression.Libs.zlib;

namespace ItemSerializer
{
    class Program
    {
        const int maxnum = 250;
        static void Main(string[] args)
        {
            Console.WriteLine("********************************************************");
            Console.WriteLine("**               Item/Nano Serializer                 **");
            Console.WriteLine("********************************************************");

            if (!File.Exists("Config.xml"))
            {
                Console.WriteLine("CellAO Configuration file not found. Please copy this file into your Built/Debug or Built/Release folder and execute it there.");
                Console.WriteLine("Please press Enter to exit");
                Console.ReadLine();
                return;
            }

            Convertitems();
            Convertnanos();

        }


        public static void Convertitems()
        {

            SqlWrapper sql = new SqlWrapper();
            DataTable dt_items = sql.ReadDatatable("SELECT * FROM Items order by aoid asc");

            Stream sf = new FileStream("items.dat", FileMode.Create);

            ZOutputStream ds = new ZOutputStream(sf, zlibConst.Z_BEST_COMPRESSION);
            MemoryStream sm = new MemoryStream();

            //DeflateStream sm = new DeflateStream(sf, CompressionMode.Compress);



            BinaryFormatter bf = new BinaryFormatter();

            Console.WriteLine("Processing data... This will take a while!");
            List<AOItem> ITEMS = new List<AOItem>();
            int count = dt_items.Rows.Count;
            int oldperc = 0;
            int countall = 0;
            byte[] buffer = BitConverter.GetBytes(maxnum);
            sm.Write(buffer, 0, buffer.Length);
            if (File.Exists("items2.dat"))
            {
                ItemHandler.CacheAllItems("items2.dat");
                foreach (AOItem aoi in ItemHandler.ItemList)
                {
                    ITEMS.Add(aoi);
                    if (ITEMS.Count == maxnum)
                    {
                        bf.Serialize(sm, ITEMS);
                        sm.Flush();
                        ITEMS.Clear();
                        countall += maxnum;
                    }
                }
            }
            else
            {
                foreach (DataRow itemrow in dt_items.Rows)
                {
                    AOItem _item = new AOItem();
                    _item.LowID = (Int32)itemrow[0];
                    _item.HighID = (Int32)itemrow[0];
                    _item.Quality = (Int32)itemrow[2];
                    _item.ItemType = (Int32)itemrow[3];

                    DataTable dt_itemevents = sql.ReadDatatable("SELECT * FROM item_events WHERE itemid=" + _item.LowID + " ORDER BY eventid asc");

                    foreach (DataRow eventrow in dt_itemevents.Rows)
                    {
                        AOEvents aoe = new AOEvents();
                        aoe.EventType = (Int32)eventrow[2];
                        int eventid = (Int32)eventrow["eventid"];

                        DataTable dt_itemeventfunctions = sql.ReadDatatable("SELECT * FROM item_functions WHERE itemid=" + _item.LowID + " AND eventid=" + eventid + " ORDER BY functionid asc");

                        foreach (DataRow eventfunctionrow in dt_itemeventfunctions.Rows)
                        {
                            int eventfuncid = (Int32)eventfunctionrow["functionid"];
                            AOFunctions aof = new AOFunctions();
                            aof.FunctionType = (Int32)eventfunctionrow[3];
                            aof.Target = (Int32)eventfunctionrow[4];
                            aof.TickCount = (Int32)eventfunctionrow[5];
                            aof.TickInterval = (uint)(Int32)eventfunctionrow[6];

                            DataTable functionargs = sql.ReadDatatable("SELECT * FROM item_function_arguments WHERE functionid=" + eventfuncid + " AND eventid=" + eventid + " AND itemid=" + _item.LowID + " ORDER BY attrid asc");

                            foreach (DataRow attrs in functionargs.Rows)
                            {
                                if (!(attrs["argvalint"] is DBNull))
                                {
                                    aof.Arguments.Add((Int32)attrs["argvalint"]);
                                }
                                else
                                    if (!(attrs["argvalsingle"] is DBNull))
                                    {
                                        aof.Arguments.Add((Single)(float)attrs["argvalsingle"]);
                                    }
                                    else
                                        if (!(attrs["argvalstring"] is DBNull))
                                        {
                                            string s = attrs["argvalstring"].ToString();
                                            aof.Arguments.Add(s);
                                        }
                                        else
                                            throw (new NotSupportedException("No Argument value given, all NULL: " + _item.LowID));
                            }

                            DataTable reqs = sql.ReadDatatable("SELECT * from  item_function_reqs WHERE functionid=" + eventfuncid + " AND eventid=" + eventid + " AND itemid=" + _item.LowID + " ORDER BY reqid asc");

                            foreach (DataRow rrow in reqs.Rows)
                            {
                                AORequirements aor = new AORequirements();
                                aor.Statnumber = (Int32)rrow["attrnum"];
                                aor.Value = (Int32)rrow["attrval"];
                                aor.Target = (Int32)rrow["target"];
                                aor.Operator = (Int32)rrow["operator"];
                                aor.ChildOperator = (Int32)rrow["child_op"];
                                aof.Requirements.Add(aor);
                            }

                            aoe.Functions.Add(aof);
                        }

                        _item.Events.Add(aoe);
                    }

                    DataTable dt_actions = sql.ReadDatatable("SELECT * FROM item_actions WHERE itemid=" + _item.LowID);

                    foreach (DataRow acrow in dt_actions.Rows)
                    {
                        AOActions aoa = new AOActions();
                        aoa.ActionType = (Int32)acrow["actionnum"];

                        DataTable reqs = sql.ReadDatatable("SELECT * FROM item_action_reqs WHERE itemid=" + _item.LowID + " AND actionid=" + ((Int32)acrow["actionid"]) + " ORDER BY reqid ASC");

                        foreach (DataRow rrow in reqs.Rows)
                        {
                            AORequirements aor = new AORequirements();
                            aor.Statnumber = (Int32)rrow["attrnum"];
                            aor.Value = (Int32)rrow["attrval"];
                            aor.Target = (Int32)rrow["target"];
                            aor.Operator = (Int32)rrow["operator"];
                            aor.ChildOperator = (Int32)rrow["child_op"];
                            aoa.Requirements.Add(aor);
                        }
                        _item.Actions.Add(aoa);
                    }

                    DataTable dtdef = sql.ReadDatatable("SELECT * FROM item_defense_attributes where itemid=" + _item.LowID + " ORDER BY defenseid asc");

                    foreach (DataRow defrow in dtdef.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)defrow["num"];
                        aoia.Value = (Int32)defrow["value"];
                        _item.Defend.Add(aoia);
                    }

                    DataTable dtatt = sql.ReadDatatable("select * FROM item_attack_attributes where itemid=" + _item.LowID + " ORDER BY attackid asc");
                    foreach (DataRow defrow in dtatt.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)defrow["num"];
                        aoia.Value = (Int32)defrow["value"];
                        _item.Defend.Add(aoia);
                    }

                    DataTable attributes = sql.ReadDatatable("SELECT * FROM item_attributes WHERE itemid=" + _item.LowID + " ORDER BY attributeid asc");
                    foreach (DataRow atrow in attributes.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)atrow["num"];
                        aoia.Value = (Int32)atrow["value"];
                        _item.Stats.Add(aoia);
                    }


                    ITEMS.Add(_item);

                    int perc = Convert.ToInt32(Math.Floor((double)(ITEMS.Count + countall) / count * 100.0));
                    if (perc != oldperc)
                    {
                        Console.Write("\rDone " + perc.ToString().PadLeft(3) + "%");
                        oldperc = perc;
                    }
                    if (ITEMS.Count == maxnum)
                    {
                        bf.Serialize(sm, ITEMS);
                        sm.Flush();
                        ITEMS.Clear();
                        countall += maxnum;
                    }
                }
            }
            bf.Serialize(sm, ITEMS);
            sm.Seek(0, SeekOrigin.Begin);
            Console.WriteLine();
            CopyStream(sm, ds);
            sm.Close();
            ds.Close();
        }

        public static void Convertnanos()
        {
            SqlWrapper sql = new SqlWrapper();
            DataTable dt_items = sql.ReadDatatable("SELECT * FROM nanos order by aoid asc");

            Stream sf = new FileStream("nanos.dat", FileMode.Create);

            ZOutputStream ds = new ZOutputStream(sf, zlibConst.Z_BEST_COMPRESSION);
            MemoryStream sm = new MemoryStream();

            //DeflateStream sm = new DeflateStream(sf, CompressionMode.Compress);



            BinaryFormatter bf = new BinaryFormatter();

            Console.WriteLine("Processing data... This will take a while!");
            List<AONanos> ITEMS = new List<AONanos>();
            int count = dt_items.Rows.Count;
            int oldperc = 0;
            int countall = 0;
            byte[] buffer = BitConverter.GetBytes(maxnum);
            sm.Write(buffer, 0, buffer.Length);
            if (File.Exists("nanos2.dat"))
            {
                NanoHandler.CacheAllNanos("nanos2.dat");
                foreach (AONanos aoi in NanoHandler.NanoList)
                {
                    ITEMS.Add(aoi);
                    if (ITEMS.Count == maxnum)
                    {
                        bf.Serialize(sm, ITEMS);
                        sm.Flush();
                        ITEMS.Clear();
                        countall += maxnum;
                    }
                }
            }
            else
            {
                foreach (DataRow itemrow in dt_items.Rows)
                {
                    AONanos _item = new AONanos();
                    _item.ID = (Int32)itemrow[0];
                    _item.NCUCost = (Int32)itemrow[2];
                    _item.ItemType = (Int32)itemrow[3];

                    DataTable dt_itemevents = sql.ReadDatatable("SELECT * FROM nano_events WHERE nanoid=" + _item.ID + " ORDER BY eventid asc");

                    foreach (DataRow eventrow in dt_itemevents.Rows)
                    {
                        AOEvents aoe = new AOEvents();
                        aoe.EventType = (Int32)eventrow[2];
                        int eventid = (Int32)eventrow["eventid"];

                        DataTable dt_itemeventfunctions = sql.ReadDatatable("SELECT * FROM nano_functions WHERE nanoid=" + _item.ID + " AND eventid=" + eventid + " ORDER BY functionid asc");

                        foreach (DataRow eventfunctionrow in dt_itemeventfunctions.Rows)
                        {
                            int eventfuncid = (Int32)eventfunctionrow["functionid"];
                            AOFunctions aof = new AOFunctions();
                            aof.FunctionType = (Int32)eventfunctionrow[3];
                            aof.Target = (Int32)eventfunctionrow[4];
                            aof.TickCount = (Int32)eventfunctionrow[5];
                            aof.TickInterval = (uint)(Int32)eventfunctionrow[6];

                            DataTable functionargs = sql.ReadDatatable("SELECT * FROM nano_function_arguments WHERE functionid=" + eventfuncid + " AND eventid=" + eventid + " AND nanoid=" + _item.ID + " ORDER BY attrid asc");

                            foreach (DataRow attrs in functionargs.Rows)
                            {
                                if (!(attrs["argvalint"] is DBNull))
                                {
                                    aof.Arguments.Add((Int32)attrs["argvalint"]);
                                }
                                else
                                    if (!(attrs["argvalsingle"] is DBNull))
                                    {
                                        aof.Arguments.Add((Single)(float)attrs["argvalsingle"]);
                                    }
                                    else
                                        if (!(attrs["argvalstring"] is DBNull))
                                        {
                                            string s = attrs["argvalstring"].ToString();
                                            aof.Arguments.Add(s);
                                        }
                                        else
                                            throw (new NotSupportedException("No Argument value given, all NULL: " + _item.ID));
                            }

                            DataTable reqs = sql.ReadDatatable("SELECT * from  nano_function_reqs WHERE functionid=" + eventfuncid + " AND eventid=" + eventid + " AND nanoid=" + _item.ID + " ORDER BY reqid asc");

                            foreach (DataRow rrow in reqs.Rows)
                            {
                                AORequirements aor = new AORequirements();
                                aor.Statnumber = (Int32)rrow["attrnum"];
                                aor.Value = (Int32)rrow["attrval"];
                                aor.Target = (Int32)rrow["target"];
                                aor.Operator = (Int32)rrow["operator"];
                                aor.ChildOperator = (Int32)rrow["child_op"];
                                aof.Requirements.Add(aor);
                            }

                            aoe.Functions.Add(aof);
                        }

                        _item.Events.Add(aoe);
                    }

                    DataTable dt_actions = sql.ReadDatatable("SELECT * FROM nano_actions WHERE nanoid=" + _item.ID);

                    foreach (DataRow acrow in dt_actions.Rows)
                    {
                        AOActions aoa = new AOActions();
                        aoa.ActionType = (Int32)acrow["actionnum"];

                        DataTable reqs = sql.ReadDatatable("SELECT * FROM nano_action_reqs WHERE nanoid=" + _item.ID + " AND actionid=" + ((Int32)acrow["actionid"]) + " ORDER BY reqid ASC");

                        foreach (DataRow rrow in reqs.Rows)
                        {
                            AORequirements aor = new AORequirements();
                            aor.Statnumber = (Int32)rrow["attrnum"];
                            aor.Value = (Int32)rrow["attrval"];
                            aor.Target = (Int32)rrow["target"];
                            aor.Operator = (Int32)rrow["operator"];
                            aor.ChildOperator = (Int32)rrow["child_op"];
                            aoa.Requirements.Add(aor);
                        }
                        _item.Actions.Add(aoa);
                    }

                    DataTable dtdef = sql.ReadDatatable("SELECT * FROM nano_defense_attributes where nanoid=" + _item.ID + " ORDER BY defenseid asc");

                    foreach (DataRow defrow in dtdef.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)defrow["num"];
                        aoia.Value = (Int32)defrow["value"];
                        _item.Defend.Add(aoia);
                    }

                    DataTable dtatt = sql.ReadDatatable("select * FROM nano_attack_attributes where nanoid=" + _item.ID + " ORDER BY attackid asc");
                    foreach (DataRow defrow in dtatt.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)defrow["num"];
                        aoia.Value = (Int32)defrow["value"];
                        _item.Defend.Add(aoia);
                    }

                    DataTable attributes = sql.ReadDatatable("SELECT * FROM nano_attributes WHERE nanoid=" + _item.ID + " ORDER BY attributeid asc");
                    foreach (DataRow atrow in attributes.Rows)
                    {
                        AOItemAttribute aoia = new AOItemAttribute();
                        aoia.Stat = (Int32)atrow["num"];
                        aoia.Value = (Int32)atrow["value"];
                        _item.Stats.Add(aoia);
                    }


                    ITEMS.Add(_item);

                    int perc = Convert.ToInt32(Math.Floor((double)(ITEMS.Count + countall) / count * 100.0));
                    if (perc != oldperc)
                    {
                        Console.Write("\rDone " + perc.ToString().PadLeft(3) + "%");
                        oldperc = perc;
                    }
                    if (ITEMS.Count == maxnum)
                    {
                        bf.Serialize(sm, ITEMS);
                        sm.Flush();
                        ITEMS.Clear();
                        countall += maxnum;
                    }
                }
            }
            bf.Serialize(sm, ITEMS);
            sm.Seek(0, SeekOrigin.Begin);
            Console.WriteLine();
            CopyStream(sm, ds);
            sm.Close();
            ds.Close();
        }
        
        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2097152];
            int len;
            while ((len = input.Read(buffer, 0, 2097152)) > 0)
            {
                output.Write(buffer, 0, len);
                Console.Write("\rCompressing " + Convert.ToInt32(Math.Floor((double)input.Position / input.Length * 100.0)) + "%");
            }
            output.Flush();
        }

    }
}
