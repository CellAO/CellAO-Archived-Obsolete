#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AO.Core;
#endregion

#region NameSpace

namespace ZoneEngine.Script
{

    #region Class AOChatCommand
    /// <summary>
    /// The Class in charge of printing information to our consoles
    /// To add a new chat command refer to the ones already inside Scripts/ChatCommands
    /// Important: Class names of chat command scripts have to be lowercase!
    /// </summary>
    public abstract class AOChatCommand
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Script Entry points
        /// <summary>
        /// Execute the chat command
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="args">command arguments</param>
        public abstract void ExecuteCommand(Client client, Identity target, string[] args);

        /// <summary>
        /// Returns the GM Level needed for this command
        /// </summary>
        /// <returns>GMLevel needed</returns>
        public abstract int GMLevelNeeded();

        /// <summary>
        /// Returns Help for this command
        /// </summary>
        /// <returns>Help text</returns>
        public abstract void CommandHelp(Client client);

        /// <summary>
        /// Returns a list of commands handled by this class
        /// </summary>
        /// <returns>List of command strings</returns>
        public abstract List<string> GetCommands();

        /// <summary>
        /// Checks the command Arguments
        /// </summary>
        /// <param name="args">True if command arguments are fine</param>
        /// <returns></returns>
        public abstract bool CheckCommandArguments(string[] args);
        #endregion

        #region Helper to check Command Arguments
        public bool CheckArgumentHelper(List<Type> typelist, string[] args)
        {
            // Return false if number of args dont match (first argument is Command, so it doesnt count)
            if (args.Length - 1 != typelist.Count)
            {
                return false;
            }

            bool argumentsok = true;
            for (int argcounter = 0; argcounter < typelist.Count; argcounter++)
            {
                if (typelist.ElementAt(argcounter) == typeof (string))
                {
                    continue;
                }

                if (typelist.ElementAt(argcounter) == typeof (int))
                {
                    int temp;
                    argumentsok &= int.TryParse(args[argcounter + 1], out temp);
                    continue;
                }

                if (typelist.ElementAt(argcounter) == typeof (Int32))
                {
                    Int32 temp;
                    argumentsok &= Int32.TryParse(args[argcounter + 1], out temp);
                    continue;
                }

                if (typelist.ElementAt(argcounter) == typeof (bool))
                {
                    bool temp;
                    argumentsok &= bool.TryParse(args[argcounter + 1], out temp);
                    continue;
                }

                if (typelist.ElementAt(argcounter) == typeof (uint))
                {
                    uint temp;
                    argumentsok &= uint.TryParse(args[argcounter + 1], out temp);
                    continue;
                }

                if (typelist.ElementAt(argcounter) == typeof (float))
                {
                    float temp;
                    argumentsok &= float.TryParse(args[argcounter + 1], NumberStyles.Any, CultureInfo.InvariantCulture,
                                                  out temp);
                }
            }
            return argumentsok;
        }
        #endregion Helper to check Command Arguments
    }
    #endregion Class AOChatCommand
}

#endregion NameSpace