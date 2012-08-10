using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace PatternBuilder
{
    public class Program
    {
        public class Item
        {
            public string FullName;
            public string MobName;
            public string Description;
            public int AOID;
            public int QL;
        }

        public static List<Item> A = new List<Item>();
        public static List<Item> B = new List<Item>();
        public static List<Item> C = new List<Item>();
        public static List<Item> D = new List<Item>();
        public static List<Item> AB = new List<Item>();
        public static List<Item> ABC = new List<Item>();
        public static List<Item> Complete = new List<Item>();
        public static List<Item> Notum = new List<Item>();
        public static List<Item> Novictalized = new List<Item>();

        public static List<Item> Crystal = new List<Item>();
        public static List<Item> Novictum= new List<Item>();

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

                            if (Regex.IsMatch(item.FullName, "^(A.+an Pattern).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    A.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(B.+ar Pattern).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    B.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Chi Pattern).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    C.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Dom Pattern).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    D.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(A.+an-Bh.+ar) Assembly.+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    AB.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(A.+an-B.+ar-Chi) Assembly.+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    ABC.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Complete Blueprint Pattern).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    Complete.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Notum Crystal with etched).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    Notum.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Novictalized Notum Crystal with).+", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Description, "'.+'", RegexOptions.IgnoreCase))
                                {
                                    item.MobName = Regex.Match(item.Description, "'.+'").Value;
                                    Novictalized.Add(item);
                                }
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Subdued Flow of Novictum)$", RegexOptions.IgnoreCase))
                            {
                                Novictum.Add(item);
                            }
                            else if (Regex.IsMatch(item.FullName, "^(Crystal Filled by the Source)$", RegexOptions.IgnoreCase))
                            {
                                Crystal.Add(item);
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

            Console.WriteLine("A: {0}", A.Count);
            Console.WriteLine("B: {0}", B.Count);
            Console.WriteLine("C: {0}", C.Count);
            Console.WriteLine("D: {0}", D.Count);
            Console.WriteLine("AB: {0}", AB.Count);
            Console.WriteLine("ABC: {0}", ABC.Count);
            Console.WriteLine("Complete: {0}", Complete.Count);
            Console.WriteLine("Notum: {0}", Notum.Count);
            Console.WriteLine("Novictalized: {0}", Novictalized.Count);
            Console.WriteLine("Crystal: {0}", Crystal.Count);
            Console.WriteLine("Novictum: {0}", Novictum.Count);

            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\dump.txt", true, Encoding.GetEncoding("windows-1252"));
            sw.AutoFlush = true;

            foreach (Item ta in A)
            {
                foreach (Item so in B)
                {
                    if (ta.MobName == so.MobName && ta.QL == so.QL)
                    {
                        foreach (Item re in AB)
                        {
                            if (re.MobName == so.MobName && re.QL == so.QL)
                            {
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + so.AOID + "," + ta.AOID + ",0," + "\'" + re.AOID + "," + re.AOID + "\',0,3,\"54\",\"35\",\"0\",0," + re.QL * 5 + "," + re.QL * 5 + ",0);");
                            }
                        }
                    }
                }
            }

            foreach (Item ta in AB)
            {
                foreach (Item so in C)
                {
                    if (ta.MobName == so.MobName && ta.QL == so.QL)
                    {
                        foreach (Item re in ABC)
                        {
                            if (re.MobName == so.MobName && re.QL == so.QL)
                            {
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + so.AOID + "," + ta.AOID + ",0," + "\'" + re.AOID + "," + re.AOID + "\',0,3,\"54\",\"35\",\"0\",0," + re.QL * 5 + "," + re.QL * 5 + ",0);");
                            }
                        }
                    }
                }
            }

            foreach (Item ta in ABC)
            {
                foreach (Item so in D)
                {
                    if (ta.MobName == so.MobName && ta.QL == so.QL)
                    {
                        foreach (Item re in Complete)
                        {
                            if (re.MobName == so.MobName && re.QL == so.QL)
                            {
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + so.AOID + "," + ta.AOID + ",0," + "\'" + re.AOID + "," + re.AOID + "\',0,3,\"54\",\"35\",\"0\",0," + re.QL * 5 + "," + re.QL * 5 + ",0);");
                            }
                        }
                    }
                }
            }

            foreach (Item ta in Complete)
            {
                foreach (Item so in Crystal)
                {
                    if (so.QL > 1)
                    {
                        foreach (Item re in Notum)
                        {
                            if (ta.MobName == re.MobName && ta.QL == re.QL)
                            {
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + so.AOID + "," + ta.AOID + ",0," + "\'" + re.AOID + "," + re.AOID + "\',1,3,\"160\",\"600\",\"0\",0," + re.QL * 5 + "," + re.QL * 5 + ",0);");
                            }
                        }
                    }
                }
            }

            foreach (Item ta in Notum)
            {
                foreach (Item so in Novictum)
                {
                    if (so.QL > 1)
                    {
                        foreach (Item re in Novictalized)
                        {
                            if (ta.MobName == re.MobName && ta.QL == re.QL)
                            {
                                sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + so.AOID + "," + ta.AOID + ",0," + "\'" + re.AOID + "," + re.AOID + "\',1,3,\"157\",\"450\",\"0\",0," + re.QL * 5 + "," + re.QL * 5 + ",0);");
                            }
                        }
                    }
                }
            }

            sw.Close();
            sw.Dispose();

            Console.ReadLine();
        }
    }
}