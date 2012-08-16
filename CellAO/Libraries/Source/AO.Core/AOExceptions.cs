// -----------------------------------------------------------------------
// <copyright file="AOExceptions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace AO.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Character does not exist Exception
    /// </summary>
    [Serializable]
    public class CharacterDoesNotExistException : ApplicationException
    {
        /// <summary>
        /// Exception handler
        /// </summary>
        /// <param name="message">Message to log</param>
        public CharacterDoesNotExistException(string message)
            : base(message)
        {
        }
    }


    /// <summary>
    /// Wrong packet type passed to Vicinity Handler
    /// </summary>
    [Serializable]
    public class WrongPacketTypeException : ApplicationException
    {
        /// <summary>
        /// Exception handler
        /// </summary>
        /// <param name="message">Message to log</param>
        public WrongPacketTypeException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Stat does not exist
    /// </summary>
    [Serializable]
    public class StatDoesNotExistException : ApplicationException
    {
        /// <summary>
        /// Exception handler
        /// </summary>
        /// <param name="message">Message to log</param>
        public StatDoesNotExistException(string message)
            : base(message)
        {
        }
    }

}
