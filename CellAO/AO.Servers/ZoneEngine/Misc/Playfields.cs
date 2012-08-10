#region License
/*
Copyright (c) 2005-2012, CellAO Team

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

#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#endregion

namespace ZoneEngine.Misc
{

    #region DistrictInfo Class
    public class DistrictInfo
    {
        #region Constructor
        #endregion

        #region XML
        // Generally this shouldn't be used outside of the static constructor
        public static List<DistrictInfo> LoadXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (List<DistrictInfo>), new XmlRootAttribute("Districts"));
            TextReader reader = new StreamReader(fileName);
            List<DistrictInfo> data = (List<DistrictInfo>) serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        // This really should only be used for development. Included for completeness.
        public static void DumpXML(string fileName, PlayfieldInfo pfInfo)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (List<DistrictInfo>), new XmlRootAttribute("Districts"));
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(String.Empty, String.Empty);
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, pfInfo.districts, xsn);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Encoding.ASCII.GetString(stream.GetBuffer()));
            stream.Dispose();
            xmlDoc.DocumentElement.SetAttribute("Playfield", pfInfo.id.ToString());
            xmlDoc.Save(fileName);
        }

        public static List<DistrictInfo> LoadDistricts(int pf)
        {
            string fileName = Path.Combine("XML Data", "Districts");
            fileName = Path.Combine(fileName, pf + ".xml");
            if (File.Exists(fileName))
            {
                return LoadXML(fileName);
            }
            else
            {
                return new List<DistrictInfo>();
            }
        }
        #endregion

        [XmlElement("Name")] public string districtName = "Nameless District";

        [XmlAttribute("MinLevel")] public int minLevel;

        [XmlAttribute("MaxLevel")] public int maxLevel;

        [XmlAttribute("SuppressionGas")] public int suppressionGas = 100;
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

        #region Constructor
        #endregion

        /// <summary>
        /// Playfield ID number
        /// </summary>
        [XmlAttribute("id")]
        public int id
        {
            get { return _id; }
            set
            {
                _id = value;

                districts = DistrictInfo.LoadDistricts(_id);
            }
        }

        private int _id;

        /// <summary>
        /// Name of playfield
        /// </summary>
        [XmlElement("Name")] public string name = string.Empty;

        /// <summary>
        /// What expansion(s) are required to be in this Playfield.
        /// Bits have the same meaning as the Expansions stat. More than one can be set.
        /// </summary>
        [XmlAttribute("expansion")] public int expansion;

        /// <summary>
        /// If the Playfield is disabled or not
        /// </summary>
        [XmlAttribute("disabled")] public bool disabled;

        /// <summary>
        /// Playfield X coordinate
        /// </summary>
        [XmlAttribute("x")] public int x = 100000;

        /// <summary>
        /// Scale X
        /// </summary>
        [XmlAttribute("xscale")] public Single xscale = 1.0f;

        /// <summary>
        /// Playfield Z coordinate
        /// </summary>
        [XmlAttribute("z")] public int z = 100000;

        /// <summary>
        /// Scale Z
        /// </summary>
        [XmlAttribute("zscale")] public Single zscale = 1.0f;

        /// <summary>
        /// DistrictInfo
        /// </summary>
        //[XmlElement("District")]
        [XmlIgnore] public List<DistrictInfo> districts;
    }
    #endregion

    #region Playfields Class
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Playfields")]
    public class Playfields
    {
        [XmlIgnore] public static readonly Playfields Instance;

        #region Constructors
        private Playfields()
        {
        }

        static Playfields()
        {
            Instance = LoadXML(Path.Combine("XML Data", "Playfields.xml"));
        }
        #endregion

        #region XML
        // Generally this shouldn't be used outside of the static constructor
        public static Playfields LoadXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (Playfields));
            TextReader reader = new StreamReader(fileName);
            Playfields data = (Playfields) serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        // This really should only be used for development. Included for completeness.
        public static void DumpXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (Playfields));
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
        [XmlElement("Playfield")] public List<PlayfieldInfo> playfields;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playfieldName"></param>
        /// <returns></returns>
        public static int PlayfieldNameToPlayfieldId(string playfieldName)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.name == playfieldName)
                    return pfInfo.id;
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
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.id == playfieldId)
                    return pfInfo.name;
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
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.id == playfieldId)
                {
                    return !pfInfo.disabled;
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
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.name == playfieldName)
                {
                    return !pfInfo.disabled;
                }
            }

            return false;
        }

        #region GetPlayfield Coords (needed for playfieldanarchyf packet)
        public static int GetPlayfieldX(int playfieldNumber)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.id == playfieldNumber)
                    return pfInfo.x;
            }

            return 100000;
        }

        public static int GetPlayfieldZ(int playfieldNumber)
        {
            foreach (PlayfieldInfo pfInfo in Instance.playfields)
            {
                if (pfInfo.id == playfieldNumber)
                    return pfInfo.z;
            }

            return 100000;
        }
        #endregion
    }
    #endregion
}