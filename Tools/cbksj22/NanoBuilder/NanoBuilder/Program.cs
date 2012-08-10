using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NanoBuilder
{
    public class Program
    {
        public class Item
        {
            public string FullName;
            public string NanoName;
            public string Description;
            public string Type;
            public int AOID;
            public int QL;
        }

        public static List<CA> CAS = new List<CA>();
        public static List<PPPE> PPPES = new List<PPPE>();
        public static List<ID> IDS = new List<ID>();
        public static List<Crystal> Crystals = new List<Crystal>();
        public static List<Crystal> Broken = new List<Crystal>();

        static void Main(string[] args)
        {
            XmlReader reader = XmlReader.Create(new FileStream(Environment.CurrentDirectory + @"\Items.xml", FileMode.Open, FileAccess.Read));

            while (reader.Read())
            {
                if (reader.Name == "Item")
                {
                    Item item = new Item();

                    reader.MoveToAttribute("AOID");
                    item.AOID = Convert.ToInt32(reader.Value);

                    reader.MoveToAttribute("Name");
                    item.FullName = reader.Value;

                    reader.MoveToAttribute("QL");
                    item.QL = Convert.ToInt32(reader.Value);

                    while (reader.Read())
                    {
                        if (reader.Name == "Description")
                        {
                            item.Description = reader.ReadElementString();

                            if (Regex.IsMatch(item.FullName, "^(Nano).?Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Crystals.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Badly Corroded Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, @"\[Badly Eroded Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Blood Stained and Corroded", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Severly Corroded Shadow", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Cracked and Miskept Shadow", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, @"Cracked Crystal \(", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Dirty Money Shadow", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Failed Repaired Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Hacked Corroded Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Overcharged Corroded Nano Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Snow Crashed Shadow Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "Tainted Shadow Crystal", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                Broken.Add(new Crystal(item.FullName, item.NanoName, item.AOID, item.QL));
                            }

                            else if (Regex.IsMatch(item.FullName, "Compiled Algorithm", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                CAS.Add(new CA(item.FullName, item.NanoName, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Programmed Photon Particle Emitter)", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                if (Regex.IsMatch(item.Description, "Space school of nano programs", RegexOptions.IgnoreCase))
                                    item.Type = "Space";
                                else if (Regex.IsMatch(item.Description, "PSI school of nano programs", RegexOptions.IgnoreCase))
                                    item.Type = "PSI";
                                else if (Regex.IsMatch(item.Description, "Medical school of nano programs", RegexOptions.IgnoreCase))
                                    item.Type = "Medical";
                                else if (Regex.IsMatch(item.Description, "Protection school of nano programs", RegexOptions.IgnoreCase))
                                    item.Type = "Protection";
                                else if (Regex.IsMatch(item.Description, "Combat school of nano programs", RegexOptions.IgnoreCase))
                                    item.Type = "Combat";
                                else
                                    break;

                                PPPES.Add(new PPPE(item.FullName, item.NanoName, item.Type, item.AOID, item.QL));
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Instruction Disc)", RegexOptions.IgnoreCase))
                            {
                                item.NanoName = Regex.Match(item.FullName, @"\(.+\)", RegexOptions.IgnoreCase).Value;

                                if (Regex.IsMatch(item.Description, "If combined with a Space Symbol Library", RegexOptions.IgnoreCase))
                                    item.Type = "Space";
                                else if (Regex.IsMatch(item.Description, "If combined with a PSI Symbol Library", RegexOptions.IgnoreCase))
                                    item.Type = "PSI";
                                else if (Regex.IsMatch(item.Description, "If combined with a Medical Symbol Library", RegexOptions.IgnoreCase))
                                    item.Type = "Medical";
                                else if (Regex.IsMatch(item.Description, "If combined with a Protection Symbol Library", RegexOptions.IgnoreCase))
                                    item.Type = "Protection";
                                else if (Regex.IsMatch(item.Description, "If combined with a Combat Symbol Library", RegexOptions.IgnoreCase))
                                    item.Type = "Combat";
                                else
                                    break;

                                IDS.Add(new ID(item.FullName, item.NanoName, item.Type, item.AOID, item.QL));
                            }
                        }
                        break;
                    }
                    while (reader.Name != "Item")
                    {
                        reader.Read();
                    }
                }
            }
            reader.Close();

            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\dump.txt", true, Encoding.GetEncoding("windows-1252"));
            sw.AutoFlush = true;

            foreach (ID id in IDS)
            {
                foreach (CA ca in CAS)
                {
                    if (ca.ShortName == id.ShortName)
                    {
                        switch (id.Type)
                        {
                            case "Medical":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (78455," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149853," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149852," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149851," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149850," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149849," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144795," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                break;
                            case "PSI":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (78454," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149843," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149842," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149841," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149840," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149839," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144791," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                break;
                            case "Combat":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (78452," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149858," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149857," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149856," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149855," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149854," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144798," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                break;
                            case "Protection":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (78453," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149848," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149847," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149846," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149845," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149844," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144793," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                break;
                            case "Space":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (88101," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149838," + id.AOID + ",0," + "\'" + ca.AOID + "," + ca.AOID + "\',25,3,\"160,161\",\"400,400\",\"0,0\",0," + ca.QL * 5 + "," + ca.QL * 5 + ",0);");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            foreach (CA ca in CAS)
            {
                foreach (PPPE pppe in PPPES)
                {
                    if (pppe.ShortName == ca.ShortName)
                    {
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (72806," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (149863," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (149862," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (149861," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (149860," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (149859," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (144812," + ca.AOID + ",0," + "\'" + pppe.AOID + "," + pppe.AOID + "\',50,3,\"160\",\"425\",\"0\",0," + pppe.QL * 5 + "," + pppe.QL * 5 + ",0);");
                    }
                }
            }

            foreach (PPPE pppe in PPPES)
            {
                foreach (Crystal crystal in Crystals)
                {
                    if (crystal.ShortName == pppe.ShortName)
                    {
                        switch (pppe.Type)
                        {
                            case "Medical":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149831," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149830," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144810," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                break;
                            case "PSI":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149827," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149826," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144805," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                break;
                            case "Combat":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149833," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149832," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144811," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                break;
                            case "Protection":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149829," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149828," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144807," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                break;
                            case "Space":
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149825," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (149824," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (144803," + pppe.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',1,3,\"160,125\",\"470,450\",\"0,0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            foreach (Crystal broken in Broken)
            {
                foreach (Crystal crystal in Crystals)
                {
                    if (broken.ShortName == crystal.ShortName)
                    {
                        sw.WriteLine("INSERT INTO `tradeskill` VALUES (161699," + broken.AOID + ",0," + "\'" + crystal.AOID + "," + crystal.AOID + "\',0,3,\"160\",\"700\",\"0\",0," + crystal.QL * 5 + "," + crystal.QL * 5 + ",0);");
                    }
                }
            }

            sw.Close();
            sw.Dispose();

            Console.WriteLine("Completed.");
            Console.ReadLine();
        }
    }
}