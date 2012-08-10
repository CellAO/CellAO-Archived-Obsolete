using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace ImplantBuilder
{
    public class Program
    {
        public class Item
        {
            public string Name;
            public string Description;
            public string Type;
            public int AOID;
            public int QL;
        }

        public static List<Implant> EyeImplants = new List<Implant>();
        public static List<Cluster> EyeClusters = new List<Cluster>();
        public static List<Implant> RefinedEyeImplants = new List<Implant>();
        public static List<Cluster> RefinedEyeClusters = new List<Cluster>();

        public static List<Implant> HeadImplants = new List<Implant>();
        public static List<Cluster> HeadClusters = new List<Cluster>();
        public static List<Implant> RefinedHeadImplants = new List<Implant>();
        public static List<Cluster> RefinedHeadClusters = new List<Cluster>();

        public static List<Implant> EarImplants = new List<Implant>();
        public static List<Cluster> EarClusters = new List<Cluster>();
        public static List<Implant> RefinedEarImplants = new List<Implant>();
        public static List<Cluster> RefinedEarClusters = new List<Cluster>();
        
        public static List<Implant> RightArmImplants = new List<Implant>();
        public static List<Cluster> RightArmClusters = new List<Cluster>();
        public static List<Implant> RefinedRightArmImplants = new List<Implant>();
        public static List<Cluster> RefinedRightArmClusters = new List<Cluster>();

        public static List<Implant> ChestImplants = new List<Implant>();
        public static List<Cluster> ChestClusters = new List<Cluster>();
        public static List<Implant> RefinedChestImplants = new List<Implant>();
        public static List<Cluster> RefinedChestClusters = new List<Cluster>();

        public static List<Implant> LeftArmImplants = new List<Implant>();
        public static List<Cluster> LeftArmClusters = new List<Cluster>();
        public static List<Implant> RefinedLeftArmImplants = new List<Implant>();
        public static List<Cluster> RefinedLeftArmClusters = new List<Cluster>();

        public static List<Implant> RightWristImplants = new List<Implant>();
        public static List<Cluster> RightWristClusters = new List<Cluster>();
        public static List<Implant> RefinedRightWristImplants = new List<Implant>();
        public static List<Cluster> RefinedRightWristClusters = new List<Cluster>();

        public static List<Implant> WaistImplants = new List<Implant>();
        public static List<Cluster> WaistClusters = new List<Cluster>();
        public static List<Implant> RefinedWaistImplants = new List<Implant>();
        public static List<Cluster> RefinedWaistClusters = new List<Cluster>();

        public static List<Implant> LeftWristImplants = new List<Implant>();
        public static List<Cluster> LeftWristClusters = new List<Cluster>();
        public static List<Implant> RefinedLeftWristImplants = new List<Implant>();
        public static List<Cluster> RefinedLeftWristClusters = new List<Cluster>();

        public static List<Implant> RightHandImplants = new List<Implant>();
        public static List<Cluster> RightHandClusters = new List<Cluster>();
        public static List<Implant> RefinedRightHandImplants = new List<Implant>();
        public static List<Cluster> RefinedRightHandClusters = new List<Cluster>();

        public static List<Implant> LegImplants = new List<Implant>();
        public static List<Cluster> LegClusters = new List<Cluster>();
        public static List<Implant> RefinedLegImplants = new List<Implant>();
        public static List<Cluster> RefinedLegClusters = new List<Cluster>();

        public static List<Implant> LeftHandImplants = new List<Implant>();
        public static List<Cluster> LeftHandClusters = new List<Cluster>();
        public static List<Implant> RefinedLeftHandImplants = new List<Implant>();
        public static List<Cluster> RefinedLeftHandClusters = new List<Cluster>();

        public static List<Implant> FeetImplants = new List<Implant>();
        public static List<Cluster> FeetClusters = new List<Cluster>();
        public static List<Implant> RefinedFeetImplants = new List<Implant>();
        public static List<Cluster> RefinedFeetClusters = new List<Cluster>();

        public static List<Item> Implants = new List<Item>();
        public static List<Item> Clusters = new List<Item>();
        public static List<Item> RefinedImplants = new List<Item>();
        public static List<Item> RefinedClusters = new List<Item>();
        
        public static List<string> Tradeskills = new List<string>();

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
                    item.Name = reader.Value;

                    reader.MoveToAttribute("QL");
                    item.QL = Convert.ToInt32(reader.Value);

                    reader.MoveToAttribute("ItemType");
                    item.Type = reader.Value;

                    while (reader.Read())
                    {
                        if (reader.Name == "Description")
                        {
                            item.Description = reader.ReadElementString();

                            if (item.Type == "Implant")
                            { 
                                if (Regex.IsMatch(item.Description, "Shining NanoCluster", RegexOptions.IgnoreCase))
                                {
                                    if (Regex.IsMatch(item.Name, "Refined", RegexOptions.IgnoreCase))
                                    {
                                        RefinedImplants.Add(item);
                                    }
                                    else
                                    Implants.Add(item);                                  
                                }
                            }
                            else if (item.Type == "Misc" && Regex.IsMatch(item.Description, "can be used with any.*?implant", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(item.Name, "Refined", RegexOptions.IgnoreCase))
                                {
                                    RefinedClusters.Add(item);
                                }
                                else
                                Clusters.Add(item);
                            }

                            break;
                        }
                    }
                    while (reader.Name != "Item")
                    {
                        reader.Read();
                    }
                }
            }

            Cluster c = new Cluster();
            c.BuildClusters(Clusters);
            c.BuildClusters(RefinedClusters);

            Implant i = new Implant();
            i.BuildImplants(Implants);
            i.BuildImplants(RefinedImplants);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============CLUSTERS==============");
            Console.WriteLine();
            Console.WriteLine("Eye Clusters:\t\t\t{0}", EyeClusters.Count);
            Console.WriteLine("Refined Eye Clusters:\t\t{0}", RefinedEyeClusters.Count);
            Console.WriteLine("Head Clusters:\t\t\t{0}", HeadClusters.Count);
            Console.WriteLine("Refined Head Clusters:\t\t{0}", RefinedHeadClusters.Count);
            Console.WriteLine("Ear Clusters:\t\t\t{0}", EarClusters.Count);
            Console.WriteLine("Refined Ear Clusters:\t\t{0}", RefinedEarClusters.Count);
            Console.WriteLine("Right-Arm Clusters:\t\t{0}", RightArmClusters.Count);
            Console.WriteLine("Refined Right-Arm Clusters:\t{0}", RefinedRightArmClusters.Count);
            Console.WriteLine("Chest Clusters:\t\t\t{0}", ChestClusters.Count);
            Console.WriteLine("Refined Chest Clusters:\t\t{0}", RefinedChestClusters.Count);
            Console.WriteLine("Left-Arm Clusters:\t\t{0}", LeftArmClusters.Count);
            Console.WriteLine("Refined Left-Arm Clusters:\t{0}", RefinedLeftArmClusters.Count);
            Console.WriteLine("Right-Wrist Clusters:\t\t{0}", RightWristClusters.Count);
            Console.WriteLine("Refined Right-Wrist Clusters:\t{0}", RefinedRightWristClusters.Count);
            Console.WriteLine("Waist Clusters:\t\t\t{0}", WaistClusters.Count);
            Console.WriteLine("Refined Waist Clusters:\t\t{0}", RefinedWaistClusters.Count);
            Console.WriteLine("Left-Wrist Clusters:\t\t{0}", LeftWristClusters.Count);
            Console.WriteLine("Refined Left-Wrist Clusters:\t{0}", RefinedLeftWristClusters.Count);
            Console.WriteLine("Right-Hand Clusters:\t\t{0}", RightHandClusters.Count);
            Console.WriteLine("Refined Right-Hand Clusters:\t{0}", RefinedRightHandClusters.Count);
            Console.WriteLine("Leg Clusters:\t\t\t{0}", LegClusters.Count);
            Console.WriteLine("Refined Leg Clusters:\t\t{0}", RefinedLegClusters.Count);
            Console.WriteLine("Left-Hand Clusters:\t\t{0}", LeftHandClusters.Count);
            Console.WriteLine("Refined Left-Hand Clusters:\t{0}", RefinedLeftHandClusters.Count);
            Console.WriteLine("Feet Clusters:\t\t\t{0}", FeetClusters.Count);
            Console.WriteLine("Refined Feet Clusters:\t\t{0}", RefinedFeetClusters.Count);
            Console.WriteLine();
            Console.WriteLine("==============CLUSTERS==============");

            Console.WriteLine();

            Console.WriteLine("==============IMPLANTS==============");
            Console.WriteLine();
            Console.WriteLine("Eye Implants:\t\t\t{0}", EyeImplants.Count);
            Console.WriteLine("Refined Eye Implants:\t\t{0}", RefinedEyeImplants.Count);
            Console.WriteLine("Head Implants:\t\t\t{0}", HeadImplants.Count);
            Console.WriteLine("Refined Head Implants:\t\t{0}", RefinedHeadImplants.Count);
            Console.WriteLine("Ear Implants:\t\t\t{0}", EarImplants.Count);
            Console.WriteLine("Refined Ear Implants:\t\t{0}", RefinedEarImplants.Count);
            Console.WriteLine("Right-Arm Implants:\t\t{0}", RightArmImplants.Count);
            Console.WriteLine("Refined Right-Arm Implants:\t{0}", RefinedRightArmImplants.Count);
            Console.WriteLine("Chest Implants:\t\t\t{0}", ChestImplants.Count);
            Console.WriteLine("Refined Chest Implants:\t\t{0}", RefinedChestImplants.Count);
            Console.WriteLine("Left-Arm Implants:\t\t{0}", LeftArmImplants.Count);
            Console.WriteLine("Refined Left-Arm Implants:\t{0}", RefinedLeftArmImplants.Count);
            Console.WriteLine("Right-Wrist Implants:\t\t{0}", RightWristImplants.Count);
            Console.WriteLine("Refined Right-Wrist Implants:\t{0}", RefinedRightWristImplants.Count);
            Console.WriteLine("Waist Implants:\t\t\t{0}", WaistImplants.Count);
            Console.WriteLine("Refined Waist Implants:\t\t{0}", RefinedWaistImplants.Count);
            Console.WriteLine("Left-Wrist Implants:\t\t{0}", LeftWristImplants.Count);
            Console.WriteLine("Refined Left-Wrist Implants:\t{0}", RefinedLeftWristImplants.Count);
            Console.WriteLine("Right-Hand Implants:\t\t{0}", RightHandImplants.Count);
            Console.WriteLine("Refined Right-Hand Implants:\t{0}", RefinedRightHandImplants.Count);
            Console.WriteLine("Leg Implants:\t\t\t{0}", LegImplants.Count);
            Console.WriteLine("Refined Leg Implants:\t\t{0}", RefinedLegImplants.Count);
            Console.WriteLine("Left-Hand Implants:\t\t{0}", LeftHandImplants.Count);
            Console.WriteLine("Refined Left-Hand Implants:\t{0}", RefinedLeftHandImplants.Count);
            Console.WriteLine("Feet Implants:\t\t\t{0}", FeetImplants.Count);
            Console.WriteLine("Refined Feet Implants:\t\t{0}", RefinedFeetImplants.Count);
            Console.WriteLine();
            Console.WriteLine("==============IMPLANTS==============");
            Console.ResetColor();

            Console.WriteLine();

            Builder.Disassemble(EyeImplants);
            Builder.Disassemble(HeadImplants);
            Builder.Disassemble(EarImplants);
            Builder.Disassemble(RightArmImplants);
            Builder.Disassemble(ChestImplants);
            Builder.Disassemble(LeftArmImplants);
            Builder.Disassemble(RightWristImplants);
            Builder.Disassemble(WaistImplants);
            Builder.Disassemble(LeftWristImplants);
            Builder.Disassemble(RightHandImplants);
            Builder.Disassemble(LegImplants);
            Builder.Disassemble(LeftHandImplants);
            Builder.Disassemble(FeetImplants);

            Builder.PopulateDictionary();
            Tradeskill.PopulateTypes();
            Tradeskill.PopulateNanoProgrammingMultiplier();

            //Builder.Build(EyeClusters, EyeImplants);
            //Builder.Build(RefinedEyeClusters, RefinedEyeImplants);
            //Builder.Build(HeadClusters, HeadImplants);
            //Builder.Build(RefinedHeadClusters, RefinedHeadImplants);
            //Builder.Build(EarClusters, EarImplants);
            //Builder.Build(RefinedEarClusters, RefinedEarImplants);
            //Builder.Build(RightArmClusters, RightArmImplants);
            //Builder.Build(RefinedRightArmClusters, RefinedRightArmImplants);
            //Builder.Build(ChestClusters, ChestImplants);
            //Builder.Build(RefinedChestClusters, RefinedChestImplants);
            //Builder.Build(LeftArmClusters, LeftArmImplants);
            //Builder.Build(RefinedLeftArmClusters, RefinedLeftArmImplants);
            //Builder.Build(RightWristClusters, RightWristImplants);
            //Builder.Build(RefinedRightWristClusters, RefinedRightWristImplants);
            //Builder.Build(WaistClusters, WaistImplants);
            //Builder.Build(RefinedWaistClusters, RefinedWaistImplants);
            //Builder.Build(LeftWristClusters, LeftWristImplants);
            //Builder.Build(RefinedLeftWristClusters, RefinedLeftWristImplants);
            //Builder.Build(RightHandClusters, RightHandImplants);
            //Builder.Build(RefinedRightHandClusters, RefinedRightHandImplants);
            //Builder.Build(LegClusters, LegImplants);
            //Builder.Build(RefinedLegClusters, RefinedLegImplants);
            //Builder.Build(LeftHandClusters, LeftHandImplants);
            //Builder.Build(RefinedLeftHandClusters, RefinedLeftHandImplants);
            //Builder.Build(FeetClusters, FeetImplants);
            //Builder.Build(RefinedFeetClusters, RefinedFeetImplants);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nComplete...");
            Console.ResetColor();

            reader.Close();
            Console.ReadLine();
        }
    }
}
