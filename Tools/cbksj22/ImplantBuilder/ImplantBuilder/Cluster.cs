using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImplantBuilder
{
    public class Cluster
    {
        public string Name;
        public string Location;
        public string Type;
        public string Skill;
        public int LowID;
        public int HighID;
        public int MaxQL;
        public bool isJobe;

        public void BuildClusters(List<Program.Item> items)
        {
            List<Cluster>[] Clusters = new List<Cluster>[13]
                {
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>()
                };

            List<Cluster>[] RefinedClusters = new List<Cluster>[13] 
            {
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>(),
                    new List<Cluster>()
            };

            bool isRefined = false;

            foreach (Program.Item item in items)
            {
                Cluster cluster = new Cluster();

                if (Regex.IsMatch(item.Name, "Faded", RegexOptions.IgnoreCase))
                {
                    cluster.Type = "Faded";
                }
                else if (Regex.IsMatch(item.Name, "Bright", RegexOptions.IgnoreCase))
                {
                    cluster.Type = "Bright";
                }
                else if (Regex.IsMatch(item.Name, "Shiny", RegexOptions.IgnoreCase))
                {
                    cluster.Type = "Shining";
                }

                if (item.QL == 300)
                {
                    isRefined = true;
                    cluster.MaxQL = 300;
                    cluster.Name = item.Name;
                    cluster.HighID = item.AOID;
                    cluster.LowID = item.AOID - 1;
                    cluster.Location = Regex.Match(item.Name, @"\(.*\)", RegexOptions.IgnoreCase).Value.Trim(new char[] { '(', ')' });
                    string skill = Regex.Match(item.Name, @".*Refined", RegexOptions.IgnoreCase).Value;
                    cluster.Skill = skill.Remove(skill.Count() - 8).Trim();

                    if (Regex.IsMatch(cluster.Name, "NanoCluster of Nano Regeneration", RegexOptions.IgnoreCase))
                    {
                        if (Regex.IsMatch(cluster.Name, "Faded", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Feet";
                            cluster.Type = "Faded";
                        }
                        else if (Regex.IsMatch(cluster.Name, "Bright", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Right-Arm";
                            cluster.Type = "Bright";
                        }
                        else if (Regex.IsMatch(cluster.Name, "Shining", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Right-Wrist";
                            cluster.Type = "Shining";
                        }
                        cluster.Skill = "Nano Regeneration";
                    }

                    #region Switch
                    switch (cluster.Location.ToLower())
                    {
                        case "eye":
                            RefinedClusters[0].Add(cluster);
                            break;
                        case "head":
                            RefinedClusters[1].Add(cluster);
                            break;
                        case "ear":
                            RefinedClusters[2].Add(cluster);
                            break;
                        case "right-arm":
                            RefinedClusters[3].Add(cluster);
                            break;
                        case "chest":
                            RefinedClusters[4].Add(cluster);
                            break;
                        case "left-arm":
                            RefinedClusters[5].Add(cluster);
                            break;
                        case "right-wrist":
                            RefinedClusters[6].Add(cluster);
                            break;
                        case "waist":
                            RefinedClusters[7].Add(cluster);
                            break;
                        case "left-wrist":
                            RefinedClusters[8].Add(cluster);
                            break;
                        case "right-hand":
                            RefinedClusters[9].Add(cluster);
                            break;
                        case "leg":
                            RefinedClusters[10].Add(cluster);
                            break;
                        case "left-hand":
                            RefinedClusters[11].Add(cluster);
                            break;
                        case "feet":
                            RefinedClusters[12].Add(cluster);
                            break;
                    }
                    #endregion
                }

                else if (item.QL == 200)
                {
                    isRefined = false;
                    cluster.MaxQL = 200;
                    cluster.Name = item.Name;
                    cluster.HighID = item.AOID;
                    cluster.LowID = item.AOID - 1;
                    cluster.Location = Regex.Match(item.Name, @"\(.*\)", RegexOptions.IgnoreCase).Value.Trim(new char[] { '(', ')' });

                    if (!Regex.IsMatch(item.Name, "Jobe", RegexOptions.IgnoreCase))
                    {
                        string skill = Regex.Match(item.Name, @".*Cluster", RegexOptions.IgnoreCase).Value;
                        cluster.Skill = skill.Remove(skill.Count() - 7).Trim();
                    }
                    else
                    {
                        cluster.isJobe = true;
                        string skill = Regex.Match(item.Name, @".*Jobe Cluster", RegexOptions.IgnoreCase).Value;
                        cluster.Skill = skill.Remove(skill.Count() - 12).Trim();
                    }

                    if (Regex.IsMatch(cluster.Name, "NanoCluster of Nano Regeneration", RegexOptions.IgnoreCase))
                    {
                        if (Regex.IsMatch(cluster.Name, "Faded", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Feet";
                            cluster.Type = "Faded";
                        }
                        else if (Regex.IsMatch(cluster.Name, "Bright", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Right-Arm";
                            cluster.Type = "Bright";
                        }
                        else if (Regex.IsMatch(cluster.Name, "Shining", RegexOptions.IgnoreCase))
                        {
                            cluster.Location = "Right-Wrist";
                            cluster.Type = "Shining";
                        }
                        cluster.Skill = "Nano Regeneration";
                    }

                    #region Switch
                    switch (cluster.Location.ToLower())
                    {
                        case "eye":
                            Clusters[0].Add(cluster);
                            break;
                        case "head":
                            Clusters[1].Add(cluster);
                            break;
                        case "ear":
                            Clusters[2].Add(cluster);
                            break;
                        case "right-arm":
                            Clusters[3].Add(cluster);
                            break;
                        case "chest":
                            Clusters[4].Add(cluster);
                            break;
                        case "left-arm":
                            Clusters[5].Add(cluster);
                            break;
                        case "right-wrist":
                            Clusters[6].Add(cluster);
                            break;
                        case "waist":
                            Clusters[7].Add(cluster);
                            break;
                        case "left-wrist":
                            Clusters[8].Add(cluster);
                            break;
                        case "right-hand":
                            Clusters[9].Add(cluster);
                            break;
                        case "leg":
                            Clusters[10].Add(cluster);
                            break;
                        case "left-hand":
                            Clusters[11].Add(cluster);
                            break;
                        case "feet":
                            Clusters[12].Add(cluster);
                            break;
                    }
                    #endregion
                }
            }

            if (!isRefined)
            {
                Program.EyeClusters = Clusters[0];
                Program.HeadClusters = Clusters[1];
                Program.EarClusters = Clusters[2];
                Program.RightArmClusters = Clusters[3];
                Program.ChestClusters = Clusters[4];
                Program.LeftArmClusters = Clusters[5];
                Program.RightWristClusters = Clusters[6];
                Program.WaistClusters = Clusters[7];
                Program.LeftWristClusters = Clusters[8];
                Program.RightHandClusters = Clusters[9];
                Program.LegClusters = Clusters[10];
                Program.LeftHandClusters = Clusters[11];
                Program.FeetClusters = Clusters[12];
            }
            if (isRefined)
            {
                Program.RefinedEyeClusters = RefinedClusters[0];
                Program.RefinedHeadClusters = RefinedClusters[1];
                Program.RefinedEarClusters = RefinedClusters[2];
                Program.RefinedRightArmClusters = RefinedClusters[3];
                Program.RefinedChestClusters = RefinedClusters[4];
                Program.RefinedLeftArmClusters = RefinedClusters[5];
                Program.RefinedRightWristClusters = RefinedClusters[6];
                Program.RefinedWaistClusters = RefinedClusters[7];
                Program.RefinedLeftWristClusters = RefinedClusters[8];
                Program.RefinedRightHandClusters = RefinedClusters[9];
                Program.RefinedLegClusters = RefinedClusters[10];
                Program.RefinedLeftHandClusters = RefinedClusters[11];
                Program.RefinedFeetClusters = RefinedClusters[12];
            }
        }
    }
}