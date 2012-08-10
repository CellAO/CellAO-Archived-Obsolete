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
    /// Interface for an object pool.
    /// </summary>
    /// <seealso cref="ObjectPoolMgr"/>
    /// <remarks>
    /// An object pool holds reusable objects. See <see cref="ObjectPoolMgr"/> for more details.
    /// </remarks>
    public interface IObjectPool
    {
        /// <summary>
        /// Enqueues an object in the pool to be reused.
        /// </summary>
        /// <param name="obj">The object to be put back in the pool.</param>
        void Enqueue(object obj);

        /// <summary>
        /// Grabs an object from the pool.
        /// </summary>
        /// <returns>An object from the pool.</returns>
        object DequeueObj();
    }
}