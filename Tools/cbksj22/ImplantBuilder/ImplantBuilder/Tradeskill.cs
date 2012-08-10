using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImplantBuilder
{
    class Tradeskill
    {
        public static Dictionary<string, float> NPMult = new Dictionary<string, float>();

        public static void PopulateNanoProgrammingMultiplier()
        {
            NPMult.Add("1h Blunt", 1.8f);
            NPMult.Add("1h Edged Weapon", 1.8f);
            NPMult.Add("2h Blunt", 1.9f);
            NPMult.Add("2h Edged", 1.8f);
            NPMult.Add("% Add All Def.", 2.5f);
            NPMult.Add("% Add All Off", 2.5f);
            NPMult.Add("% Add. Chem. Dam.", 2.5f);
            NPMult.Add("% Add. Energy Dam.", 2.5f);
            NPMult.Add("% Add. Fire Dam.", 2.5f);
            NPMult.Add("% Add. Melee Dam.", 2.5f);
            NPMult.Add("% Add. Poison Dam.", 2.5f);
            NPMult.Add("% Add. Proj. Dam.", 2.5f);
            NPMult.Add("% Add.Rad. Dam.", 2.5f);
            NPMult.Add("% Add. Xp", 2.5f);
            NPMult.Add("Adventuring", 1.5f);
            NPMult.Add("Agility", 2.25f);
            NPMult.Add("Aimed Shot", 2.1f);
            NPMult.Add("Assault Rif", 2.25f);
            NPMult.Add("Bio.Metamor", 2.4f);
            NPMult.Add("Body Dev", 2f);
            NPMult.Add("Bow", 2f);
            NPMult.Add("Bow Spc Att", 2f);
            NPMult.Add("Brawling", 1.65f);
            NPMult.Add("Break & Entry", 2f);
            NPMult.Add("Burst", 2.1f);
            NPMult.Add("Chemical AC", 2f);
            NPMult.Add("Chemistry", 2f);
            NPMult.Add("Cold AC", 2f);
            NPMult.Add("Comp. Liter", 2f);
            NPMult.Add("Concealment", 1.8f);
            NPMult.Add("Dimach", 2.25f);
            NPMult.Add("Disease AC", 1.75f);
            NPMult.Add("Dodge-Rng", 2f);
            NPMult.Add("Duck-Exp", 2f);
            NPMult.Add("Elec. Engi", 2f);
            NPMult.Add("Energy AC", 2.25f);
            NPMult.Add("Evade-ClsC", 2f);
            NPMult.Add("Fast Attack", 1.9f);
            NPMult.Add("Fire AC", 2.0f);
            NPMult.Add("First Aid", 1.8f);
            NPMult.Add("Fling Shot", 1.8f);
            NPMult.Add("Full Auto", 2.25f);
            NPMult.Add("Grenade", 1.9f);
            NPMult.Add("Heal Delta", 2.5f);
            NPMult.Add("Heavy Weapons", 1f);
            NPMult.Add("Imp/Proj AC", 2.25f);
            NPMult.Add("Intelligence", 2.25f);
            NPMult.Add("Map Navig", 1.25f);
            NPMult.Add("Martial Arts", 2.5f);
            NPMult.Add("Matter Crea", 2.4f);
            NPMult.Add("Matt.Metam", 2.4f);
            NPMult.Add("Max Health", 2.5f);
            NPMult.Add("Max Nano", 2.5f);
            NPMult.Add("Max NCU", 2.5f);
            NPMult.Add("Mech. Engi", 2f);
            NPMult.Add("Melee Ener", 2f);
            NPMult.Add("Melee. Init", 2f);
            NPMult.Add("Melee/Ma AC", 2.25f);
            NPMult.Add("MG / SMG", 2f);
            NPMult.Add("Multi Ranged", 2f);
            NPMult.Add("Mult. Melee", 2.25f);
            NPMult.Add("NanoC. Init", 2f);
            NPMult.Add("Nano Formula Interrupt Modifier", 2.5f);
            NPMult.Add("Nano Point Cost Modifier", 2.5f);
            NPMult.Add("Nano Pool", 3f);
            NPMult.Add("Nano Progra", 2f);
            NPMult.Add("Nano Regeneration", 2.5f);
            NPMult.Add("Nano Resist", 2f);
            NPMult.Add("Parry", 2.1f);
            NPMult.Add("Perception", 2f);
            NPMult.Add("Pharma Tech", 2f);
            NPMult.Add("Physic. Init", 2f);
            NPMult.Add("Piercing", 1.6f);
            NPMult.Add("Pistol", 2f);
            NPMult.Add("Psychic", 2.25f);
            NPMult.Add("Psychology", 2f);
            NPMult.Add("Psycho Modi", 2.4f);
            NPMult.Add("Quantum FT", 2f);
            NPMult.Add("Radiation AC", 2f);
            NPMult.Add("Ranged Ener", 2f);
            NPMult.Add("Ranged. Init", 2f);
            NPMult.Add("RangeInc. NF", 2.5f);
            NPMult.Add("RangeInc. Weapon", 2.5f);
            NPMult.Add("Rifle", 2.25f);
            NPMult.Add("Riposte", 2.5f);
            NPMult.Add("Run Speed", 2.0f);
            NPMult.Add("Sense", 2.25f);
            NPMult.Add("Sensory Impr", 2.2f);
            NPMult.Add("Sharp Obj", 1.25f);
            NPMult.Add("Shield Chemical AC", 2.5f);
            NPMult.Add("Shield Cold AC", 2.5f);
            NPMult.Add("Shield Energy AC", 2.5f);
            NPMult.Add("Shield Fire AC", 2.5f);
            NPMult.Add("Shield Melee AC", 2.5f);
            NPMult.Add("Shield Poison AC", 2.5f);
            NPMult.Add("Shield Projectile AC", 2.5f);
            NPMult.Add("Shield Radiation AC", 2.5f);
            NPMult.Add("Shotgun", 1.7f);
            NPMult.Add("Skill Time Lock Modifier", 2.5f);
            NPMult.Add("Sneak Atck", 2.5f);
            NPMult.Add("Stamina", 2.25f);
            NPMult.Add("Strength", 2.25f);
            NPMult.Add("Swimming", 1.25f);
            NPMult.Add("Time & Space", 2.4f);
            NPMult.Add("Trap Disarm", 1.8f);
            NPMult.Add("Treatment", 2.15f);
            NPMult.Add("Tutoring", 1.3f);
            NPMult.Add("Vehicle Air", 1f);
            NPMult.Add("Vehicle Grnd", 1.5f);
            NPMult.Add("Vehicle Hydr", 1.2f);
            NPMult.Add("Weapon Smt", 2f);
        }

        public static Dictionary<string, float> Second = new Dictionary<string, float>();

        public static void PopulateSecondarySkills()
        { 
        }

        public static Dictionary<string, int> Type = new Dictionary<string, int>();

        public static void PopulateTypes()
        {
            Type.Add("Faded", 100);
            Type.Add("Bright", 150);
            Type.Add("Shining", 200);
        }

        public Tradeskill(Cluster cluster, Implant implant, Implant result)
        {
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\implants.txt", true, Encoding.GetEncoding("windows-1252"));
            sw.AutoFlush = true;

            string skills = "160";
            string percent = string.Empty;


            string bump = "";
            switch (cluster.Type)
            { 
                case "Faded":
                    if (cluster.isJobe == true)
                        bump = "200";
                    else bump = "100";
                    break;
                case "Bright":
                    if (cluster.isJobe == true)
                        bump = "300";
                    else bump = "200";
                    break;
                case "Shining":
                    if (cluster.isJobe == true)
                        bump = "400";
                    else bump = "300";
                    break;
            }

            
            int minxp = 50;
            int maxxp = 50;
            int range;

            var l1 = from m in NPMult
                         where m.Key == cluster.Skill
                         select m;
            var l2 = from m in Type
                           where m.Key == cluster.Type
                           select m;

            percent = Math.Round(l1.ElementAt(0).Value * l2.ElementAt(0).Value).ToString();

            if (cluster.MaxQL == 200)
            {
                minxp = 5;
                maxxp = 1000;
            }
            else
            {
                minxp = 1005;
                maxxp = 1500;
            }

            if (cluster.Type == "Faded")
            {
                range = 18;
            }
            else if (cluster.Type == "Bright")
            {
                range = 16;
            }
            else
            {
                range = 14;
            }

            // COLS: 
            // SrcHighID 
            // TgtHighID 
            // SrcName 
            // TgtName 
            // ResName 
            // ResLowID 
            // ResHighID 
            // SkillsIds 
            // SkillReqs 
            // SkillBumps 
            // MaxBump 
            // MinXP 
            // MaxXP
            
            switch (cluster.Skill)
            {
                case "% Add All Def.":
                case "% Add All Off":
                case "% Add. Xp":
                case "Nano Formula Interrupt Modifier":
                case "Skill Time Lock Modifier":
                    {
                        if (cluster.Type == "Faded")
                        {
                            skills += ",162";
                            percent += ",325";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",162";
                            percent += ",475";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",162";
                            percent += ",625";
                            bump += "," + bump;
                        }
                    }
                    break;
                case "% Add. Chem. Dam.":
                case "% Add. Energy Dam.":
                case "% Add. Fire Dam.":
                case "% Add. Melee Dam.":
                case "% Add. Poison Dam.":
                case "% Add. Proj. Dam.":
                case "% Add.Rad. Dam.":
                case "Shield Chemical AC":
                case "Shield Cold AC":
                case "Shield Energy AC":
                case "Shield Fire AC":
                case "Shield Melee AC":
                case "Shield Poison AC":
                case "Shield Projectile AC":
                case "Shield Radiation AC":
                case "Nano Point Cost Modifier":
                    { 
                        if (cluster.Type == "Faded")
                        {
                            skills += ",157";
                            percent += ",325";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",157";
                            percent += ",475";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",157";
                            percent += ",625";
                            bump += "," + bump;
                        }
                    }
                    break;
                case "Heal Delta":
                    { 
                        if (cluster.Type == "Faded")
                        {
                            skills += ",159";
                            percent += ",325";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",159";
                            percent += ",475";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",159";
                            percent += ",625";
                            bump += "," + bump;
                        }
                    }
                    break;
                case "Nano Regeneration":
                    {
                        if (cluster.Type == "Faded")
                        {
                            skills += ",159";
                            percent += ",275";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",159";
                            percent += ",400";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",159";
                            percent += ",525";
                            bump += "," + bump;
                        }
                    }
                    break;
                case "Max NCU":
                    { 
                        if (cluster.Type == "Faded")
                        {
                            skills += ",161";
                            percent += ",325";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",161";
                            percent += ",475";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",161";
                            percent += ",625";
                            bump += "," + bump;
                        }
                    }
                    break;
                case "RangeInc. NF":
                    { 
                        if (cluster.Type == "Faded")
                        {
                            skills = "160";
                            percent = "325";
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills = "160";
                            percent = "475";
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills = "160";
                            percent = "625";
                        }
                    }
                    break;
                case "RangeInc. Weapon":
                    { 
                        // WS
                        if (cluster.Type == "Faded")
                        {
                            skills += ",158";
                            percent += ",325";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Bright")
                        {
                            skills += ",158";
                            percent += ",475";
                            bump += "," + bump;
                        }
                        else if (cluster.Type == "Shining")
                        {
                            skills += ",158";
                            percent += ",625";
                            bump += "," + bump;
                        }
                    }
                    break;
                default:
                    break;
            }

            sw.WriteLine("INSERT INTO `tradeskill` VALUES (" + cluster.HighID + "," + implant.HighID + ",0," + "\'" + result.LowID + "," + result.HighID + "\'," + range + ",3,\"" + skills + "\",\"" + percent + "\",\"" + bump + "\",5," + minxp + "," + maxxp + ",1);");

            sw.Close();
            sw.Dispose();
        }
    }
}
