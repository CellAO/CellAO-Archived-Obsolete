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
    /// Interface for an execution object.
    /// </summary>
    /// <seealso cref="ThreadMgr"/>
    /// <remarks>
    /// An execution object is queued to be executed by the application thread pool. See <see cref="ThreadMgr.QueueExecObj"/> for more details.
    /// </remarks>
    public interface IExecObj
    {
        /// <summary>
        /// Called by the application thread pool to perform some action.
        /// <seealso cref="ThreadMgr"/>
        /// </summary>
        void Execute();
    }
}