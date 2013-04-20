// --------------------------------------------------------------------------------------------------------------------
// <copyright file="bStream.cs" company="">
//   
// </copyright>
// <summary>
//   The b stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Extractor_Serializer
{
    #region Usings ...

    using System;
    using System.IO;

    #endregion

    /// <summary>
    /// The b stream.
    /// </summary>
    internal class bStream : BinaryReader
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="bStream"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public bStream(string fileName)
            : base(
                new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess)
                )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="bStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public bStream(byte[] buffer)
            : base(new MemoryStream(buffer))
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return (int)this.BaseStream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public uint Position
        {
            get
            {
                return (uint)this.BaseStream.Position;
            }

            set
            {
                this.BaseStream.Position = (long)value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The read int 32_ at.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReadInt32_At(uint position)
        {
            this.Position = position;
            return this.ReadInt32();
        }

        /// <summary>
        /// The read int 32_ msb.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReadInt32_MSB()
        {
            byte[] bytes = BitConverter.GetBytes(this.ReadInt32());
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// The read u int 32_ at.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint ReadUInt32_At(uint position)
        {
            this.Position = position;
            return this.ReadUInt32();
        }

        #endregion
    }
}