// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ItemNanoSerializer
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text.RegularExpressions;

    using AO.Core;

    using ZoneEngine;

    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add slashes.
        /// </summary>
        /// <param name="InputTxt">
        /// The input txt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AddSlashes(string InputTxt)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent
            string Result = InputTxt;

            try
            {
                Result = Regex.Replace(InputTxt, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }

        /// <summary>
        /// The create items dat.
        /// </summary>
        public static void CreateItemsDat()
        {
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable("SELECT AOID from items order by AOID asc");
            int c = 0;
            string fullline = string.Empty;
            List<AOItem> lll = new List<AOItem>();
            foreach (DataRow row in dt.Rows)
            {
                MemoryStream m = new MemoryStream();

                ItemHandlerold.Item it = new ItemHandlerold.Item((int)row[0]);
                if ((c % 200) == 0)
                {
                    Console.Write("\rWriting AOId:Number written  -  " + it.AOID.ToString() + ":" + c.ToString());
                }

                c++;
                AOItem it2 = new AOItem();
                it2.Flags = (int)it.getItemAttribute(30);
                it2.LowID = it.AOID;
                it2.HighID = it.AOID;
                it2.Instance = 0;
                it2.Type = 0;
                it2.Quality = it.QL;
                it2.Nothing = 0;
                it2.MultipleCount = 1;
                it2.ItemType = it.itemtype;

                foreach (AOItemAttribute aoit in it.attack)
                {
                    it2.Attack.Add(aoit);
                }

                foreach (AOItemAttribute aoit in it.defend)
                {
                    it2.Defend.Add(aoit);
                }

                foreach (AOItemAttribute aoit in it.ItemAttributes)
                {
                    it2.Stats.Add(aoit);
                }

                foreach (AOEvents ev in it.ItemEvents)
                {
                    it2.Events.Add(ev);
                }

                lll.Add(it2);
            }

            BinaryFormatter bf = new BinaryFormatter();
            Stream file = new FileStream("items.dat", FileMode.Create);
            MemoryStream mz = new MemoryStream();
            DeflateStream gz = new DeflateStream(file, CompressionMode.Compress);

            List<AOItem> temp = new List<AOItem>();
            int i = 0;
            while (i < lll.Count)
            {
                temp.Add(lll.ElementAt(i).ShallowCopy());
                i++;
                if (i % 500 == 0)
                {
                    bf.Serialize(mz, temp);
                    temp.Clear();
                }
            }

            bf.Serialize(mz, temp);

            mz.Seek(0, SeekOrigin.Begin);
            mz.CopyTo(gz);

            /*
            foreach (AOItem d in lll)
            {
                bf.Serialize(file, d);
            }
             */
            gz.Flush();
            gz.Close();
            file.Close();
        }


        public static void CreateNanosDat()
        {
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable("SELECT AOID from nanos order by AOID asc");
            int c = 0;
            string fullline = string.Empty;
            List<AOItem> lll = new List<AOItem>();
            foreach (DataRow row in dt.Rows)
            {
                MemoryStream m = new MemoryStream();

                AONano it = new AONano();

                it.AOID=(int)row[0];
                if ((c % 200) == 0)
                {
                    Console.Write("\rWriting AOId:Number written  -  " + it.AOID.ToString() + ":" + c.ToString());
                }

                c++;
                AOItem it2 = new AOItem();
                it2.Flags = (int)it.getItemAttribute(30);
                it2.LowID = it.AOID;
                it2.HighID = it.AOID;
                it2.Instance = 0;
                it2.Type = 0;
                it2.Quality = it.QL;
                it2.Nothing = 0;
                it2.MultipleCount = 1;
                it2.ItemType = it.itemtype;

                foreach (AOItemAttribute aoit in it.attack)
                {
                    it2.Attack.Add(aoit);
                }

                foreach (AOItemAttribute aoit in it.defend)
                {
                    it2.Defend.Add(aoit);
                }

                foreach (AOItemAttribute aoit in it.ItemAttributes)
                {
                    it2.Stats.Add(aoit);
                }

                foreach (AOEvents ev in it.ItemEvents)
                {
                    it2.Events.Add(ev);
                }

                lll.Add(it2);
            }

            BinaryFormatter bf = new BinaryFormatter();
            Stream file = new FileStream("nanos.dat", FileMode.Create);
            MemoryStream mz = new MemoryStream();
            DeflateStream gz = new DeflateStream(file, CompressionMode.Compress);

            List<AOItem> temp = new List<AOItem>();
            int i = 0;
            while (i < lll.Count)
            {
                temp.Add(lll.ElementAt(i).ShallowCopy());
                i++;
                if (i % 500 == 0)
                {
                    bf.Serialize(mz, temp);
                    temp.Clear();
                }
            }

            bf.Serialize(mz, temp);

            mz.Seek(0, SeekOrigin.Begin);
            mz.CopyTo(gz);

            /*
            foreach (AOItem d in lll)
            {
                bf.Serialize(file, d);
            }
             */
            gz.Flush();
            gz.Close();
            file.Close();
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
        }

        #endregion
    }
}