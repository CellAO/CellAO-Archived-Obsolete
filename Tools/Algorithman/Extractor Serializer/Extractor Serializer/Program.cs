// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Extractor_Serializer
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    using AO.Core;

    using ComponentAce.Compression.Libs.zlib;

    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        /// The ext.
        /// </summary>
        private static Extractor extractor;

        #endregion

        #region Public Methods and Operators

        private const int CopyStreamBufferLength = 1 * 1024 * 1024; // 8 MB

        /// <summary>
        /// The copy stream.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[CopyStreamBufferLength];
            int len;
            while ((len = input.Read(buffer, 0, CopyStreamBufferLength)) > 0)
            {
                output.Write(buffer, 0, len);
                Console.Write(
                    "\rCompressing " + Convert.ToInt32(Math.Floor((double)input.Position / input.Length * 100.0)) + "%");
            }

            output.Flush();
        }

        /// <summary>
        /// The GetData.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="recordtype">
        /// The recordtype.
        /// </param>
        public static void GetData(string path, int recordtype)
        {
            int[] items = extractor.GetRecordInstances(recordtype);
            int cou = 0;
            foreach (int item in items)
            {
                var fileStream = new FileStream(
                    path + item.ToString(CultureInfo.InvariantCulture), FileMode.Create, FileAccess.Write);

                byte[] data = extractor.GetRecordData(recordtype, item);
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
                if (cou % 10 == 0)
                {
                    Console.WriteLine(item);
                }

                cou++;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            Console.WriteLine("**********************************************************************");
            Console.WriteLine("**                                                                  **");
            Console.WriteLine("**  AO Item and Nano Extractor/Serializer v0.8beta                  **");
            Console.WriteLine("**                                                                  **");
            Console.WriteLine("**********************************************************************");

            Console.WriteLine();

            string AOPath = string.Empty;
            bool foundAO = false;
            Console.WriteLine("Enter exit to close program");
            while (!foundAO)
            {
                if (File.Exists("config.txt"))
                {
                    TextReader tr = new StreamReader("config.txt");
                    AOPath = tr.ReadLine();
                    tr.Close();
                }

                foundAO = false;
                Console.Write("Please enter your AO Install Path [" + AOPath + "]:");
                string temp = Console.ReadLine();
                if (temp != string.Empty)
                {
                    AOPath = temp;
                }

                if (temp.ToLower() == "exit")
                {
                    return;
                }

                try
                {
                    extractor = new Extractor(AOPath);
                    TextWriter tw2 = new StreamWriter("config.txt", false, Encoding.GetEncoding("windows-1252"));
                    tw2.WriteLine(AOPath);
                    tw2.Close();
                    foundAO = true;
                    Console.WriteLine("Found AO Database on " + AOPath);
                }
                catch (Exception)
                {
                    foundAO = false;
                }

                // Try to add cd_image\data\db
                if (!foundAO)
                {
                    try
                    {
                        AOPath = Path.Combine(AOPath, "cd_image\\data\\db");
                        extractor = new Extractor(AOPath);
                        TextWriter tw2 = new StreamWriter("config.txt", false, Encoding.GetEncoding("windows-1252"));
                        tw2.WriteLine(AOPath);
                        tw2.Close();
                        foundAO = true;
                        Console.WriteLine("Found AO Database on " + AOPath);
                    }
                    catch (Exception)
                    {
                        foundAO = false;
                    }
                }
            }

            TextWriter tw = new StreamWriter("itemnames.sql", false, Encoding.GetEncoding("windows-1252"));
            tw.WriteLine("DROP TABLE IF EXISTS `itemnames`;");
            tw.WriteLine("CREATE TABLE `itemnames` (");
            tw.WriteLine("  `AOID` int(10) NOT NULL,");
            tw.WriteLine("  `Name` varchar(250) NOT NULL,");
            tw.WriteLine("  PRIMARY KEY (`AOID`)");
            tw.WriteLine(") ENGINE=MyIsam DEFAULT CHARSET=latin1;");
            tw.WriteLine();
            tw.Close();

            Console.WriteLine("Number of Items to extract: " + extractor.GetRecordInstances(0xF4254).Length);
                
                // ITEM RECORD TYPE
            Console.WriteLine("Number of Nanos to extract: " + extractor.GetRecordInstances(0xFDE85).Length);
                
                // NANO RECORD TYPE

            // Console.WriteLine(extractor.GetRecordInstances(0xF4241).Length); // Playfields
            // Console.WriteLine(extractor.GetRecordInstances(0xF4266).Length); // Nano Strains
            // Console.WriteLine(extractor.GetRecordInstances(0xF4264).Length); // Perks

            // GetData(@"D:\c#\extractor serializer\data\items\",0xf4254);
            // GetData(@"D:\c#\extractor serializer\data\nanos\",0xfde85);
            // GetData(@"D:\c#\extractor serializer\data\playfields\",0xf4241);
            // GetData(@"D:\c#\extractor serializer\data\nanostrains\",0xf4266);
            // GetData(@"D:\c#\extractor serializer\data\perks\",0xf4264);
            var np = new NewParser();
            var rawItemList = new List<AOItem>();
            var rawNanoList = new List<AONanos>();
            foreach (int recnum in extractor.GetRecordInstances(0xFDE85))
            {
                rawNanoList.Add(np.ParseNano(0xFDE85, recnum, extractor.GetRecordData(0xFDE85, recnum), "temp.sql"));
            }

            File.Delete("temp.sql");
            Console.WriteLine();
            Console.WriteLine("Nanos extracted: " + rawNanoList.Count);

            List<string> ItemNamesSql = new List<string>();

            foreach (int recnum in extractor.GetRecordInstances(0xF4254))
            {
                rawItemList.Add(
                    np.ParseItem(0xF4254, recnum, extractor.GetRecordData(0xF4254, recnum), ItemNamesSql));
            }

            Console.WriteLine();
            Console.WriteLine("Compacting itemnames.sql");
            TextWriter itnsql = new StreamWriter("itemnames.sql", true, Encoding.GetEncoding("windows-1252"));
            while (ItemNamesSql.Count > 0)
            {
                int count = 0;
                string toWrite = string.Empty;
                while ((count < 20) && (ItemNamesSql.Count > 0))
                {
                    if (toWrite != "")
                    {
                        toWrite += ",";
                    }
                    toWrite += ItemNamesSql[0];
                    ItemNamesSql.RemoveAt(0);
                    count++;
                }
                if (toWrite != "")
                {
                    itnsql.WriteLine("INSERT INTO itemnames VALUES " + toWrite + ";");
                }
            }
            itnsql.Close();


            Console.WriteLine();
            Console.WriteLine("Items extracted: " + rawItemList.Count);

            Console.WriteLine();
            Console.WriteLine("Creating serialized nano data file - please wait");
            Stream sf = new FileStream("nanos.dat", FileMode.Create);

            var ds = new ZOutputStream(sf, zlibConst.Z_BEST_COMPRESSION);
            var sm = new MemoryStream();
            var bf = new BinaryFormatter();

            var nanoList2 = new List<AONanos>();

            int maxnum = 250;
            byte[] buffer = BitConverter.GetBytes(maxnum);
            sm.Write(buffer, 0, buffer.Length);

            foreach (AONanos nanos in rawNanoList)
            {
                nanoList2.Add(nanos);
                if (nanoList2.Count == 250)
                {
                    bf.Serialize(sm, nanoList2);
                    sm.Flush();
                    nanoList2.Clear();
                }
            }

            bf.Serialize(sm, nanoList2);
            sm.Seek(0, SeekOrigin.Begin);
            CopyStream(sm, ds);
            sm.Close();
            ds.Close();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Creating serialized item data file - please wait");

            sf = new FileStream("items.dat", FileMode.Create);

            ds = new ZOutputStream(sf, zlibConst.Z_BEST_COMPRESSION);
            sm = new MemoryStream();
            bf = new BinaryFormatter();

            List<AOItem> items = new List<AOItem>();

            maxnum = 250;
            buffer = BitConverter.GetBytes(maxnum);
            sm.Write(buffer, 0, buffer.Length);

            foreach (AOItem it in rawItemList)
            {
                items.Add(it);
                if (items.Count == 250)
                {
                    bf.Serialize(sm, items);
                    sm.Flush();
                    items.Clear();
                }
            }

            bf.Serialize(sm, items);
            sm.Seek(0, SeekOrigin.Begin);
            CopyStream(sm, ds);
            sm.Close();
            ds.Close();

            Console.WriteLine();
            Console.WriteLine("Checking files...");
            Console.WriteLine();

            ItemHandler.CacheAllItems("items.dat");

            Console.WriteLine("Items: " + ItemHandler.ItemList.Count + " successfully converted");

            ItemHandler.ItemList.Clear();

            NanoHandler.CacheAllNanos("nanos.dat");
            Console.WriteLine("Nanos: " + NanoHandler.NanoList.Count + " successfully converted");

            Console.WriteLine();
            Console.WriteLine("Further Instructions:");
            Console.WriteLine("- Copy items.dat and nanos.dat into your CellAO folder and overwrite.");
            Console.WriteLine("- Apply itemnames.sql to your database");
            Console.WriteLine("Press Enter to exit and have fun with CellAO");
            Console.ReadLine();
        }

        #endregion
    }
}