using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImplantBuilder
{
    public class Implant
    {
        public string Name;
        public string Location;
        public int LowID;
        public int HighID;
        public string Faded;
        public string Bright;
        public string Shining;
        public bool isHigh;

        public void BuildImplants(List<Program.Item> items)
        {
            List<Implant>[] Implants = new List<Implant>[13]
                {
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>()
                };

            List<Implant>[] RefinedImplants = new List<Implant>[13] 
            {
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>(),
                    new List<Implant>()
            };

            bool isRefined = false;

            foreach (Program.Item item in items)
            {
                if (Regex.IsMatch(item.Name, "^(A Shining)", RegexOptions.IgnoreCase) || Regex.IsMatch(item.Name, "^A Bright", RegexOptions.IgnoreCase) || Regex.IsMatch(item.Name, "^A Faded", RegexOptions.IgnoreCase))
                {
                    continue;
                }

                Implant implant = new Implant();

                if (Regex.IsMatch(item.Name, "Basic.*Implant", RegexOptions.IgnoreCase))
                {
                    string loc = Regex.Match(item.Name, "Basic.*Implant", RegexOptions.IgnoreCase).Value;
                    loc = loc.Substring(6);
                    loc = loc.Remove(loc.Count() - 8);
                    implant.Location = loc;

                    implant.Faded = "Empty";
                    implant.Bright = "Empty";
                    implant.Shining = "Empty";
                }
                else if (Regex.IsMatch(item.Name, "Chest Implant: Refined Empty", RegexOptions.IgnoreCase))
                {
                    implant.Location = "Chest";

                    implant.Faded = "Empty";
                    implant.Bright = "Empty";
                    implant.Shining = "Empty";
                }
                else if (Regex.IsMatch(item.Name, ".*Implant", RegexOptions.IgnoreCase))
                {
                    string loc = Regex.Match(item.Name, ".*Implant", RegexOptions.IgnoreCase).Value;
                    loc = loc.Remove(loc.Count() - 8);
                    implant.Location = loc.Trim();

                    string faded = Regex.Match(item.Description, "Faded NanoCluster: .*?\n", RegexOptions.IgnoreCase).Value;
                    if (faded.Count() > 19)
                    {
                        faded = faded.Substring(19).Trim();
                        implant.Faded = faded;
                    }
                    else
                        implant.Faded = "";

                    string bright = Regex.Match(item.Description, "Bright NanoCluster: .*?\n", RegexOptions.IgnoreCase).Value;
                    if (bright.Count() > 20)
                    {
                        bright = bright.Substring(20).Trim();
                        implant.Bright = bright;
                    }
                    else
                        implant.Bright = "";

                    string shining = Regex.Match(item.Description, "Shining NanoCluster: .*?\n", RegexOptions.IgnoreCase).Value;
                    if (shining.Count() > 21)
                    {
                        shining = shining.Substring(21).Trim();
                        implant.Shining = shining;
                    }
                    else
                        implant.Shining = "";
                }

                implant.Name = item.Name;
                implant.HighID = item.AOID;
                implant.LowID = item.AOID - 1;

                if (item.QL == 300)
                {
                    isRefined = true;

                    #region Switch
                    switch (implant.Location.ToLower())
                    {
                        case "eye":
                            RefinedImplants[0].Add(implant);
                            break;
                        case "head":
                            RefinedImplants[1].Add(implant);
                            break;
                        case "ear":
                            RefinedImplants[2].Add(implant);
                            break;
                        case "right-arm":
                            RefinedImplants[3].Add(implant);
                            break;
                        case "chest":
                            RefinedImplants[4].Add(implant);
                            break;
                        case "left-arm":
                            RefinedImplants[5].Add(implant);
                            break;
                        case "right-wrist":
                            RefinedImplants[6].Add(implant);
                            break;
                        case "waist":
                            RefinedImplants[7].Add(implant);
                            break;
                        case "left-wrist":
                            RefinedImplants[8].Add(implant);
                            break;
                        case "right-hand":
                            RefinedImplants[9].Add(implant);
                            break;
                        case "leg":
                            RefinedImplants[10].Add(implant);
                            break;
                        case "left-hand":
                            RefinedImplants[11].Add(implant);
                            break;
                        case "feet":
                            RefinedImplants[12].Add(implant);
                            break;
                    }
                    #endregion
                }

                else if (item.QL == 200)
                {
                    isHigh = false;

                    #region Switch
                    switch (implant.Location.ToLower())
                    {
                        case "eye":
                            Implants[0].Add(implant);
                            break;
                        case "head":
                            Implants[1].Add(implant);
                            break;
                        case "ear":
                            Implants[2].Add(implant);
                            break;
                        case "right-arm":
                            Implants[3].Add(implant);
                            break;
                        case "chest":
                            Implants[4].Add(implant);
                            break;
                        case "left-arm":
                            Implants[5].Add(implant);
                            break;
                        case "right-wrist":
                            Implants[6].Add(implant);
                            break;
                        case "waist":
                            Implants[7].Add(implant);
                            break;
                        case "left-wrist":
                            Implants[8].Add(implant);
                            break;
                        case "right-hand":
                            Implants[9].Add(implant);
                            break;
                        case "leg":
                            Implants[10].Add(implant);
                            break;
                        case "left-hand":
                            Implants[11].Add(implant);
                            break;
                        case "feet":
                            Implants[12].Add(implant);
                            break;
                    }
                    #endregion
                }
            }

            if (!isRefined)
            {
                Program.EyeImplants = Implants[0];
                Program.HeadImplants = Implants[1];
                Program.EarImplants = Implants[2];
                Program.RightArmImplants = Implants[3];
                Program.ChestImplants = Implants[4];
                Program.LeftArmImplants = Implants[5];
                Program.RightWristImplants = Implants[6];
                Program.WaistImplants = Implants[7];
                Program.LeftWristImplants = Implants[8];
                Program.RightHandImplants = Implants[9];
                Program.LegImplants = Implants[10];
                Program.LeftHandImplants = Implants[11];
                Program.FeetImplants = Implants[12];
            }
            if (isRefined)
            {
                Program.RefinedEyeImplants = RefinedImplants[0];
                Program.RefinedHeadImplants = RefinedImplants[1];
                Program.RefinedEarImplants = RefinedImplants[2];
                Program.RefinedRightArmImplants = RefinedImplants[3];
                Program.RefinedChestImplants = RefinedImplants[4];
                Program.RefinedLeftArmImplants = RefinedImplants[5];
                Program.RefinedRightWristImplants = RefinedImplants[6];
                Program.RefinedWaistImplants = RefinedImplants[7];
                Program.RefinedLeftWristImplants = RefinedImplants[8];
                Program.RefinedRightHandImplants = RefinedImplants[9];
                Program.RefinedLegImplants = RefinedImplants[10];
                Program.RefinedLeftHandImplants = RefinedImplants[11];
                Program.RefinedFeetImplants = RefinedImplants[12];
            }
        }
    }
}
