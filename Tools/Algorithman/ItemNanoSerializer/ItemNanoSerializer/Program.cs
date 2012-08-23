using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AO.Core;
using ZoneEngine;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace ItemNanoSerializer
{
    public class Program
    {
        static void Main(string[] args)
        {


            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable("SELECT AOID from items order by AOID asc");
            TextWriter output = new StreamWriter(@"items_ser.sql", false);
            int c = 0;
            output.WriteLine("CREATE  TABLE `items_ser` (`AOID` INT NOT NULL ,`Data` BLOB NULL ,PRIMARY KEY (`AOID`) );");
            string fullline = "";
            List<AOItem> lll = new List<AOItem>();
            foreach (DataRow row in dt.Rows)
            {
                MemoryStream m = new MemoryStream();

                ItemHandlerold.Item it = new ItemHandlerold.Item((int)row[0]);
                if ((c % 200) == 0)
                    Console.Write("\r" + it.AOID.ToString() + ":" + c.ToString());
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
            //output.WriteLine("INSERT INTO items_ser VALUES " + fullline.Substring(0, fullline.Length - 1) + ";");
            output.Close();

        }
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
                Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }
    }
}
