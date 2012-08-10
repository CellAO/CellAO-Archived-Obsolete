/*************************************************************************
 *                             Configuration.cs
 *                            ------------------
 *   begin                : Jan 24, 2007
 *   copyright            : (C) The WCell Team
 *   email                : info@wcell.org
 *************************************************************************/

/*************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *************************************************************************/

using System;
using System.Net;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Cell.Core
{
    /// <summary>
    /// This class provides a wrapper for <see cref="System.Net.IPAddress"/> that can be serialized with XML.
    /// </summary>
    /// <seealso cref="XmlConfig"/>
    /// <seealso cref="System.Xml.Serialization"/>
    /// <seealso cref="System.Net.IPAddress"/>
    [Serializable]
    public class XmlIPAddress
    {
        /// <summary>
        /// The <see cref="IPAddress"/>.
        /// </summary>
        private IPAddress m_ip = new IPAddress(0x0100007f); //127.0.0.1

        /// <summary>
        /// Gets/Sets a string representation of a <see cref="System.Net.IPAddress"/>.
        /// </summary>
        public string Address
        {
            get { return m_ip.ToString(); }
            set
            {
                IPAddress buf;
                if (IPAddress.TryParse(value, out buf))
                {
                    m_ip = buf;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the internal <see cref="System.Net.IPAddress"/>.
        /// </summary>
        [XmlIgnore]
        public IPAddress IPAddress
        {
            get { return m_ip; }
            set { m_ip = value; }
        }

        /// <summary>
        /// Initializes a new instace of the <see cref="XmlIPAddress"/> class.
        /// </summary>
        public XmlIPAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIPAddress"/> class with the address specified as a <see cref="System.Byte"/> array.
        /// </summary>
        /// <param name="address">The byte array value of the IP address.</param>
        /// <exception cref="System.ArgumentNullException">address is null.</exception>
        public XmlIPAddress(byte[] address)
        {
            m_ip = new IPAddress(address);
        }

        /// <summary>
        /// Initializes a new instace of the <see cref="XmlIPAddress"/> class with the specified address and scope.
        /// </summary>
        /// <param name="address">The byte array value of the IP address.</param>
        /// <param name="scopeid">The long value of the scope identifier.</param>
        /// <exception cref="System.ArgumentNullException">address is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// scopeid &lt; 0 or
        /// scopeid > 0x00000000FFFFFFF
        /// </exception>
        public XmlIPAddress(byte[] address, long scopeid)
        {
            m_ip = new IPAddress(address, scopeid);
        }

        /// <summary>
        /// Initializes a new instace of the <see cref="XmlIPAddress"/> class with the address specified as a <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="newAddress">The long value of the IP address. For example, the value 0x2414188f in big endian format would be the IP address "143.24.20.36".</param>
        public XmlIPAddress(long newAddress)
        {
            m_ip = new IPAddress(newAddress);
        }

        /// <summary>
        /// Initializes a new instace of the <see cref="XmlIPAddress"/> class with the address specified as a <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="newAddress">The new <see cref="IPAddress"/>.</param>
        public XmlIPAddress(IPAddress newAddress)
        {
            m_ip = newAddress;
        }

        /// <summary>
        /// Converts the <see cref="XmlIPAddress"/> into a string.
        /// </summary>
        /// <returns>A string representation of the internal <see cref="IPAddress"/>.</returns>
        public override string ToString()
        {
            return m_ip.ToString();
        }

        /// <summary>
        /// Gets a hash code for the object.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return m_ip.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XmlIPAddress))
            {
                return false;
            }

            if ((obj as XmlIPAddress).IPAddress.GetHashCode() != IPAddress.GetHashCode())
            {
                return false;
            }

            return true;
        }
    }
}