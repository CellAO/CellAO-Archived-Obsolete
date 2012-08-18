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

#region Using
#endregion

namespace ZoneEngine.Misc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    #region DistrictInfo Class
    public class DistrictInfo
    {
        #region Constructor
        #endregion

        #region XML
        // Generally this shouldn't be used outside of the static constructor
        public static List<DistrictInfo> LoadXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DistrictInfo>), new XmlRootAttribute("Districts"));
            TextReader reader = new StreamReader(fileName);
            List<DistrictInfo> data = (List<DistrictInfo>)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        // This really should only be used for development. Included for completeness.
        public static void DumpXml(string fileName, PlayfieldInfo pfInfo)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DistrictInfo>), new XmlRootAttribute("Districts"));
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(String.Empty, String.Empty);
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, pfInfo.Districts, xsn);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Encoding.ASCII.GetString(stream.GetBuffer()));
            stream.Dispose();
            xmlDoc.DocumentElement.SetAttribute("Playfield", pfInfo.PlayfieldId.ToString());
            xmlDoc.Save(fileName);
        }

        public static List<DistrictInfo> LoadDistricts(int pf)
        {
            string fileName = Path.Combine("XML Data", "Districts");
            fileName = Path.Combine(fileName, pf + ".xml");
            if (File.Exists(fileName))
            {
                return LoadXml(fileName);
            }
            else
            {
                return new List<DistrictInfo>();
            }
        }
        #endregion

        [XmlElement("Name")]
        private string districtName = "Nameless District";

        [XmlAttribute("MinLevel")]
        private int minLevel;

        [XmlAttribute("MaxLevel")]
        private int maxLevel;

        [XmlAttribute("SuppressionGas")]
        private int suppressionGas = 100;

        public string DistrictName
        {
            get
            {
                return this.districtName;
            }
            set
            {
                this.districtName = value;
            }
        }

        public int MinLevel
        {
            get
            {
                return this.minLevel;
            }
            set
            {
                this.minLevel = value;
            }
        }

        public int MaxLevel
        {
            get
            {
                return this.maxLevel;
            }
            set
            {
                this.maxLevel = value;
            }
        }

        public int SuppressionGas
        {
            get
            {
                return this.suppressionGas;
            }
            set
            {
                this.suppressionGas = value;
            }
        }
    }
    #endregion

    #region PlayfieldInfo Class
    /// <summary>
    /// Class to hold information about Playfields
    /// </summary>
    public class PlayfieldInfo
    {
        /*
         * At some point, this class will contain zone boarders/etc for zoning and any
         * other pf-specific related info such as a handle to the spawns list for the
         * playfield, currently spawned monsters and their locations, etc
         * 
         */

        /// <summary>
        /// Playfield ID number
        /// </summary>
        [XmlAttribute("id")]
        public int PlayfieldId
        {
            get
            {
                return this.playfieldId;
            }
            set
            {
                this.playfieldId = value;

                this.districts = DistrictInfo.LoadDistricts(this.playfieldId);
            }
        }

        private int playfieldId;

        /// <summary>
        /// Name of playfield
        /// </summary>
        [XmlElement("Name")]
        private string playfieldName = string.Empty;

        /// <summary>
        /// What expansion(s) are required to be in this Playfield.
        /// Bits have the same meaning as the Expansions stat. More than one can be set.
        /// </summary>
        [XmlAttribute("expansion")]
        public int Expansion { get; set; }

        /// <summary>
        /// If the Playfield is disabled or not
        /// </summary>
        [XmlAttribute("disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// Playfield X coordinate
        /// </summary>
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Scale X
        /// </summary>
        public float XScale
        {
            get
            {
                return this.xScale;
            }
            set
            {
                this.xScale = value;
            }
        }

        /// <summary>
        /// Playfield Z coordinate
        /// </summary>
        public int Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }

        /// <summary>
        /// Scale Z
        /// </summary>
        public float ZScale
        {
            get
            {
                return this.zScale;
            }
            set
            {
                this.zScale = value;
            }
        }

        /// <summary>
        /// DistrictInfo
        /// </summary>
        //[XmlElement("District")]
        public List<DistrictInfo> Districts
        {
            get
            {
                return this.districts;
            }
        }

        /// <summary>
        /// Name of playfield
        /// </summary>
        public string PlayfieldName
        {
            get
            {
                return this.playfieldName;
            }
            set
            {
                this.playfieldName = value;
            }
        }

        /// <summary>
        /// Playfield X coordinate
        /// </summary>
        [XmlAttribute("x")]
        private int x = 100000;

        /// <summary>
        /// Scale X
        /// </summary>
        [XmlAttribute("xscale")]
        private Single xScale = 1.0f;

        /// <summary>
        /// Playfield Z coordinate
        /// </summary>
        [XmlAttribute("z")]
        private int z = 100000;

        /// <summary>
        /// Scale Z
        /// </summary>
        [XmlAttribute("zscale")]
        private Single zScale = 1.0f;

        /// <summary>
        /// DistrictInfo
        /// </summary>
        //[XmlElement("District")]
        [XmlIgnore]
        private List<DistrictInfo> districts;
    }
    #endregion

    #region Playfields Class
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Playfields")]
    public class Playfields
    {
        [XmlIgnore]
        public static readonly Playfields Instance;

        #region Constructors
        private Playfields()
        {
        }

        static Playfields()
        {
            Instance = LoadXml(Path.Combine("XML Data", "Playfields.xml"));
        }

        /// <summary>
        /// 
        /// </summary>
        public List<PlayfieldInfo> PlayfieldInfos
        {
            get
            {
                return this.playfieldInfos;
            }
        }
        #endregion

        #region XML
        // Generally this shouldn't be used outside of the static constructor
        public static Playfields LoadXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Playfields));
            TextReader reader = new StreamReader(fileName);
            Playfields data = (Playfields)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        // This really should only be used for development. Included for completeness.
        public static void DumpXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Playfields));
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(String.Empty, String.Empty);
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, Instance, xsn);
            writer.Close();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Playfield")]
        private List<PlayfieldInfo> playfieldInfos;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playfieldName"></param>
        /// <returns></returns>
        public static int PlayfieldNameToPlayfieldId(string playfieldName)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldName == playfieldName)
                {
                    return pfInfo.PlayfieldId;
                }
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playfieldId"></param>
        /// <returns></returns>
        public static string PlayfieldIdToPlayfieldName(int playfieldId)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldId == playfieldId)
                {
                    return pfInfo.PlayfieldName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playfieldId"></param>
        /// <returns></returns>
        public static bool ValidPlayfield(int playfieldId)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldId == playfieldId)
                {
                    return !pfInfo.Disabled;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playfieldName"></param>
        /// <returns></returns>
        public static bool ValidPlayfield(string playfieldName)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldName == playfieldName)
                {
                    return !pfInfo.Disabled;
                }
            }

            return false;
        }

        #region GetPlayfield Coords (needed for playfieldanarchyf packet)
        public static int GetPlayfieldX(int playfieldNumber)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldId == playfieldNumber)
                {
                    return pfInfo.X;
                }
            }

            return 100000;
        }

        public static int GetPlayfieldZ(int playfieldNumber)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfieldInfos)
            {
                if (pfInfo.PlayfieldId == playfieldNumber)
                {
                    return pfInfo.Z;
                }
            }

            return 100000;
        }
        #endregion
    }
    #endregion
}