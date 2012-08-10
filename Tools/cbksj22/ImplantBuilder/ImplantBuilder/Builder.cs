using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImplantBuilder
{
    public static class Builder
    {
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public static void PopulateDictionary()
        {
            dictionary.Add("1h Blunt", "1 Hand Blunt Weapons");
            dictionary.Add("1h Edged Weapon", "1 Hand Edged Weapons");
            dictionary.Add("2h Blunt", "2 Handed Blunt Weapons");
            dictionary.Add("2h Edged", "2 Handed Edged Weapons");
            dictionary.Add("% Add All Def.", "Percentage Added to all Defencive Rolls");
            dictionary.Add("% Add All Off", "Percentage Added to all Offensive Rolls");
            dictionary.Add("% Add. Chem. Dam.", "Points Added to Chemical Damage");
            dictionary.Add("% Add. Energy Dam.", "Points Added to Energy Damage");
            dictionary.Add("% Add. Fire Dam.", "Points Added to Fire Damage");
            dictionary.Add("% Add. Melee Dam.", "Points Added to Melee Damage");
            dictionary.Add("% Add. Poison Dam.", "Points Added to Poison Damage");
            dictionary.Add("% Add. Proj. Dam.", "Points Added to Projectile Damage");
            dictionary.Add("% Add.Rad. Dam.", "Points Added to Radiation Damage");
            dictionary.Add("% Add. Xp", "Percentage Additional Experience");
            dictionary.Add("Adventuring", "Adventuring");
            dictionary.Add("Agility", "Agility");
            dictionary.Add("Aimed Shot", "Aimed Shot");
            dictionary.Add("Assault Rif", "Assault Rifle");
            dictionary.Add("Bio.Metamor", "Biological Metamorphoses");
            dictionary.Add("Body Dev", "Body Development");
            dictionary.Add("Bow", "Bow");
            dictionary.Add("Bow Spc Att", "Bow Special Attack");
            dictionary.Add("Brawling", "Brawling");
            dictionary.Add("Break & Entry", "Breaking and Entering");
            dictionary.Add("Burst", "Burst");
            dictionary.Add("Chemical AC", "Chemical Armor-Class");
            dictionary.Add("Chemistry", "Chemistry");
            dictionary.Add("Cold AC", "Cold Armor-Class");
            dictionary.Add("Comp. Liter", "Computer Literacy");
            dictionary.Add("Concealment", "Concealment");
            dictionary.Add("Dimach", "Dimach (Soul Attack)");
            dictionary.Add("Disease AC", "Disease and Poison Armor-Class");
            dictionary.Add("Dodge-Rng", "Dodge Ranged Attacks");
            dictionary.Add("Duck-Exp", "Duck Explosions and Thrown Objects");
            dictionary.Add("Elec. Engi", "Electrical Engineering");
            dictionary.Add("Energy AC", "Energy Attack Armor-Class");
            dictionary.Add("Evade-ClsC", "Evade Close Combat and Martial Art Attacks");
            dictionary.Add("Fast Attack", "Fast Attack");
            dictionary.Add("Fire AC", "Fire Armor-Class");
            dictionary.Add("First Aid", "First Aid");
            dictionary.Add("Fling Shot", "Fling Shot");
            dictionary.Add("Full Auto", "Full Auto");
            dictionary.Add("Grenade", "Grenade or Lumping Throwing");
            dictionary.Add("Heal Delta", "Health Regeneration Add");
            dictionary.Add("Heavy Weapons", "Operate Heavy Machinery");
            dictionary.Add("Imp/Proj AC", "Impact and Projectile Weapon Armor-Class");
            dictionary.Add("Intelligence", "Intelligence");
            dictionary.Add("Map Navig", "Map Navigation");
            dictionary.Add("Martial Arts", "Martial Arts");
            dictionary.Add("Matter Crea", "Matter Creations");
            dictionary.Add("Matt.Metam", "Matter Metamorphoses");
            dictionary.Add("Max Health", "life");
            dictionary.Add("Max Nano", "Max Nano");
            dictionary.Add("Max NCU", "NCU Count");
            dictionary.Add("Mech. Engi", "Mechanical Enginering");
            dictionary.Add("Melee Ener", "Melee Energy Weapons");
            dictionary.Add("Melee. Init", "Melee Weapons Initiative");
            dictionary.Add("Melee/Ma AC", "Melee Attacks and Martial Art Armor-Class");
            dictionary.Add("MG / SMG", "Machine Guns (MG) and Sub Machine Guns (SMG)");
            dictionary.Add("Multi Ranged", "Multiple Ranged Weapons");
            dictionary.Add("Mult. Melee", "Multiple Melee Weapons");
            dictionary.Add("NanoC. Init", "Nano Execution Init");
            dictionary.Add("Nano Formula Interrupt Modifier", "Percentage Change in chance of being interrupted while Executing Nanos");
            dictionary.Add("Nano Point Cost Modifier", "Nano Execution Cost Percentage Change");
            dictionary.Add("Nano Pool", "Nano Energy Pool");
            dictionary.Add("Nano Progra", "Nano-Bot Programming");
            dictionary.Add("Nano Regeneration", string.Empty);
            dictionary.Add("Nano Resist", "Nano Resistance");
            dictionary.Add("Parry", "Parry");
            dictionary.Add("Perception", "Perception and spotting");
            dictionary.Add("Pharma Tech", "Pharmacological Technology");
            dictionary.Add("Physic. Init", "Physical Prowess and Martial Arts Initiative");
            dictionary.Add("Piercing", "Piercing Weapons");
            dictionary.Add("Pistol", "Pistol");
            dictionary.Add("Psychic", "Psychic");
            dictionary.Add("Psychology", "Psychology");
            dictionary.Add("Psycho Modi", "Psychological Modifications");
            dictionary.Add("Quantum FT", "Quantum Force field Technology");
            dictionary.Add("Radiation AC", "Radiation Armor-Class");
            dictionary.Add("Ranged Ener", "Ranged Energy Weapons");
            dictionary.Add("Ranged. Init", "Ranged Weapons Initiative");
            dictionary.Add("RangeInc. NF", "Range Increaser Nano Formula");
            dictionary.Add("RangeInc. Weapon", "Range Increaser Weapon");
            dictionary.Add("Rifle", "Rifle and Sniper-Rifle");
            dictionary.Add("Riposte", "Riposte");
            dictionary.Add("Run Speed", "Run Speed");
            dictionary.Add("Sense", "Sense");
            dictionary.Add("Sensory Impr", "Sensory Improvement and Modification");
            dictionary.Add("Sharp Obj", "Knife or Sharp Object Throwing");
            dictionary.Add("Shield Chemical AC", "Shield Chemical AC");
            dictionary.Add("Shield Cold AC", "Shield Cold AC");
            dictionary.Add("Shield Energy AC", "Shield Energy AC");
            dictionary.Add("Shield Fire AC", "Shield Fire AC");
            dictionary.Add("Shield Melee AC", "Shield Melee AC");
            dictionary.Add("Shield Poison AC", "Shield Poison AC");
            dictionary.Add("Shield Projectile AC", "Shield Projectile AC");
            dictionary.Add("Shield Radiation AC", "Shield Radiation AC");
            dictionary.Add("Shotgun", "Shotgun");
            dictionary.Add("Skill Time Lock Modifier", "Percentage Change in Skill Timer Lock");
            dictionary.Add("Sneak Atck", "Sneak Attack");
            dictionary.Add("Stamina", "Stamina");
            dictionary.Add("Strength", "Strength");
            dictionary.Add("Swimming", "Swimming");
            dictionary.Add("Time & Space", "Time and Space Alteration");
            dictionary.Add("Trap Disarm", "Trap Disarmament");
            dictionary.Add("Treatment", "Treatment");
            dictionary.Add("Tutoring", "Tutoring");
            dictionary.Add("Vehicle Air", "Vehicle Navigation, Airborne");
            dictionary.Add("Vehicle Grnd", "Vehicle Navigation, Ground");
            dictionary.Add("Vehicle Hydr", "Vehicle Navigation, Waterbased");
            dictionary.Add("Weapon Smt", "Weapon Smithing");
        }

        public static void Disassemble(List<Implant> implants)
        {
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\implants.txt", true, Encoding.GetEncoding("windows-1252"));
            sw.AutoFlush = true;
            foreach (Implant implant in implants)
            {
                if ((implant.Faded != "Empty") || (implant.Bright != "Empty") || (implant.Shining != "Empty"))
                {
                    foreach (Implant empty in implants)
                    {
                        if (empty.Faded == "Empty" && empty.Bright == "Empty" && empty.Shining == "Empty")
                        {
                            sw.WriteLine("INSERT INTO `tradeskill` VALUES (161867," + implant.HighID + ",0," + "\'" + empty.LowID + "," + empty.HighID + "\',10,2,\"165,160\",\"425,100\",\"0,0\",0,5,1000,0);");
                        }
                    }
                }
            }
            sw.Close();
            sw.Dispose();
        }

        public static void Build(List<Cluster> clusters, List<Implant> implants)
        {
            foreach (Cluster cluster in clusters)
            {
                bool isUsed = false;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Attemping to combine {0}s ...", cluster.Location);
                Console.ResetColor();
                foreach (Implant implant in implants)
                {
                    if (cluster.Type == "Faded")
                    {
                        if (implant.Faded == "Empty")
                        {
                            if (implant.Location.ToLower() == cluster.Location.ToLower())
                            {
                                foreach (Implant result in implants)
                                {
                                    var value = dictionary.Where(m => m.Key == cluster.Skill).Select(m => m);
                                    foreach (KeyValuePair<string, string> kvp in value)
                                    {
                                        if (result.Faded == kvp.Value && implant.Bright == result.Bright && result.Shining == implant.Shining)
                                        {
                                            isUsed = true;
                                            Tradeskill ts = new Tradeskill(cluster, implant, result);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (cluster.Type == "Bright")
                    {
                        if (implant.Bright == "Empty")
                        {
                            if (implant.Location.ToLower() == cluster.Location.ToLower())
                            {
                                foreach (Implant result in implants)
                                {
                                    var value = dictionary.Where(m => m.Key == cluster.Skill).Select(m => m);
                                    foreach (KeyValuePair<string, string> kvp in value)
                                    {
                                        if (result.Faded == implant.Faded && kvp.Value == result.Bright && result.Shining == implant.Shining)
                                        {
                                            isUsed = true;
                                            Tradeskill ts = new Tradeskill(cluster, implant, result);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else if (cluster.Type == "Shining")
                    {
                        if (implant.Shining == "Empty")
                        {
                            if (implant.Location == cluster.Location)
                            {
                                foreach (Implant result in implants)
                                {
                                    var value = dictionary.Where(m => m.Key == cluster.Skill).Select(m => m);
                                    foreach (KeyValuePair<string, string> kvp in value)
                                    {
                                        if (result.Faded == implant.Faded && implant.Bright == result.Bright && result.Shining == kvp.Value)
                                        {
                                            isUsed = true;
                                            Tradeskill ts = new Tradeskill(cluster, implant, result);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!isUsed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Failed.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Success!");
                    Console.ResetColor();
                }
            }
        }
    }
}
