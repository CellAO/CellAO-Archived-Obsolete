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

namespace Cell.Core
{
    /// <summary>
    /// Global constants for the Cell framework.
    /// </summary>
    public static class CellDef
    {
        /// <summary>
        /// File name for the Cell framework error file.
        /// </summary>
        public const string CORE_LOG_FNAME = "CellCore";

        /// <summary>
        /// Internal version string.
        /// </summary>
        public const string SVER = "Cell v1.0 ALPHA";

        /// <summary>
        /// Internal version number.
        /// </summary>
        public const float VER = 1.0f;

        /// <summary>
        /// Maximum size of a packet buffer.
        /// </summary>
        public const int MAX_PBUF = 4096; //4kb
    }
}