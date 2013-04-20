// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extractor.cs" company="">
//   
// </copyright>
// <summary>
//   The extractor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Extractor_Serializer
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// The extractor.
    /// </summary>
    public class Extractor : IDisposable
    {
        #region Fields

        /// <summary>
        /// The da ts.
        /// </summary>
        private readonly List<bStream> DATs = new List<bStream>();

        /// <summary>
        /// The records.
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, uint>> Records = new Dictionary<int, Dictionary<int, uint>>();

        /// <summary>
        /// The block offset.
        /// </summary>
        private readonly uint blockOffset;

        /// <summary>
        /// The data file size.
        /// </summary>
        private readonly uint dataFileSize;

        /// <summary>
        /// The total records.
        /// </summary>
        private readonly uint totalRecords;

        /// <summary>
        /// The df.
        /// </summary>
        private int DF;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Extractor"/> class.
        /// </summary>
        /// <param name="rdbPath">
        /// The rdb path.
        /// </param>
        public Extractor(string rdbPath = "./")
        {
            Regex regex = new Regex("ResourceDatabase\\.dat(\\.\\d{3})?$", RegexOptions.IgnoreCase);
            SortedSet<string> sortedSet = new SortedSet<string>();
            string[] files = Directory.GetFiles(rdbPath);
            for (int i = 0; i < files.Length; i++)
            {
                string text = files[i];
                if (regex.IsMatch(text))
                {
                    sortedSet.Add(text);
                }
            }

            foreach (string text in sortedSet)
            {
                this.DATs.Add(new bStream(text));
            }

            string path = rdbPath + ((rdbPath.Substring(rdbPath.Length - 1, 1) == "\\") ? string.Empty : "\\")
                          + "ResourceDatabase.idx";
            bStream bStream = new bStream(File.ReadAllBytes(path));
            this.blockOffset = bStream.ReadUInt32_At(12u);
            this.dataFileSize = bStream.ReadUInt32_At(184u);
            if (this.dataFileSize < 0u)
            {
                this.dataFileSize = (uint)this.DATs[0].Length;
            }

            bStream.Position = bStream.ReadUInt32_At(72u);
            for (uint num = bStream.ReadUInt32(); num > 0u; num = bStream.ReadUInt32_At(num))
            {
                bStream.ReadInt32();
                short num2 = bStream.ReadInt16();
                bStream.Position += 8u;
                this.totalRecords += (uint)num2;
                while (true)
                {
                    short expr_1F6 = num2;
                    num2--;
                    if (expr_1F6 <= 0)
                    {
                        break;
                    }

                    uint value = bStream.ReadUInt32();
                    int key = bStream.ReadInt32_MSB();
                    int key2 = bStream.ReadInt32_MSB();
                    if (!this.Records.ContainsKey(key))
                    {
                        this.Records.Add(key, new Dictionary<int, uint>());
                    }

                    this.Records[key].Add(key2, value);
                }
            }

            bStream.Close();
            bStream.Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Extractor"/> class. 
        /// </summary>
        ~Extractor()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the total records.
        /// </summary>
        public uint TotalRecords
        {
            get
            {
                return this.totalRecords;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The get record data.
        /// </summary>
        /// <param name="RecordType">
        /// The record type.
        /// </param>
        /// <param name="RecordInstance">
        /// The record instance.
        /// </param>
        /// <param name="decode">
        /// The decode.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// </exception>
        public byte[] GetRecordData(int RecordType, int RecordInstance, bool decode = false)
        {
            uint position = this.Records[RecordType][RecordInstance];
            byte[] buffer = this.SegRead(34u, position);
            bStream bStream = new bStream(buffer);
            int num = bStream.ReadInt32_At(10u);
            if (RecordType != num)
            {
                throw new InvalidDataException("Invalid Record Type");
            }

            int num2 = bStream.ReadInt32();
            if (RecordInstance != num2)
            {
                throw new InvalidDataException("Invalid Record Instance");
            }

            uint count = bStream.ReadUInt32() - 12u;
            byte[] array = this.SegRead(count, 4294967295u);
            if (decode)
            {
                ulong num3 = (ulong)RecordInstance;
                int i = 0;
                while (i < array.Length)
                {
                    num3 *= 16850947uL;
                    num3 %= 21023087759uL;
                    byte[] arg_B0_0 = array;
                    int expr_AB = i++;
                    arg_B0_0[expr_AB] ^= (byte)num3;
                }
            }

            return array;
        }

        /// <summary>
        /// The get record instances.
        /// </summary>
        /// <param name="recordType">
        /// The record type.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public int[] GetRecordInstances(int recordType)
        {
            return this.Records[recordType].Keys.ToArray();
        }

        /// <summary>
        /// The get record types.
        /// </summary>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public int[] GetRecordTypes()
        {
            return this.Records.Keys.ToArray();
        }

        /// <summary>
        /// The is valid instance.
        /// </summary>
        /// <param name="RecordType">
        /// The record type.
        /// </param>
        /// <param name="RecordInstance">
        /// The record instance.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidInstance(int RecordType, int RecordInstance)
        {
            return this.Records.ContainsKey(RecordType) && this.Records[RecordType].ContainsKey(RecordInstance);
        }

        /// <summary>
        /// The is valid type.
        /// </summary>
        /// <param name="recordType">
        /// The record type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidType(int recordType)
        {
            return this.Records.ContainsKey(recordType);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (bStream current in this.DATs)
                {
                    current.Close();
                    current.Dispose();
                }
            }
        }

        /// <summary>
        /// The seg read.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// </exception>
        private byte[] SegRead(uint count, uint position = 4294967295u)
        {
            if (position != 4294967295u)
            {
                this.DF = (int)Math.Floor(1.0f * position / this.dataFileSize);
                if (this.DF > 0)
                {
                    position = (uint)(position - (this.dataFileSize - this.blockOffset) * (ulong)this.DF);
                }

                this.DATs[this.DF].Position = position;
            }

            byte[] array = new byte[count];
            int num = this.DATs[this.DF].Read(array, 0, (int)count);
            if (num == -1)
            {
                throw new EndOfStreamException();
            }

            if (num != (long)count)
            {
                this.DATs[++this.DF].Position = this.blockOffset;
                this.DATs[this.DF].Read(array, num, (int)(count - (uint)num));
            }

            return array;
        }

        #endregion
    }
}