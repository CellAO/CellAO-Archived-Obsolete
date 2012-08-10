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
using System.IO;
using AO.Core;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class ScriptAPI
    {
        /// <summary>
        /// 
        /// </summary>
        public ScriptAPI()
        {
            try
            {
                // Lua Removed L = new Lua();
            }
            catch (FileNotFoundException ex)
            {
                LogScriptAction("Could not load file ({0})...", ex.Message);
            }

            #region Lua Bindings
            //put Lua bindings after this comment
            /* Bye Bye Bindings
            L.RegisterFunction("print", this, this.GetType().GetMethod("Print"));
            L.RegisterFunction("NewTimer", this, this.GetType().GetMethod("NewTimer"));
            L.RegisterFunction("Broadcast", this, this.GetType().GetMethod("Broadcast"));
            L.RegisterFunction("SetStat", this, this.GetType().GetMethod("SetStat"));

            L.RegisterFunction("NewCoord", this, this.GetType().GetMethod("NewCoord"));
            L.RegisterFunction("NewQuat", this, this.GetType().GetMethod("NewQuat"));
            // Experimental
            L.RegisterFunction("SendVicinityChat", this, this.GetType().GetMethod("SendVicinityChat"));
             */
            #endregion
        }

        public void SendVicinityChat(Dynel target, string message)
        {
            target.SendVicinityChat(message);
        }

        public Quaternion NewQuat(float A, float B, float C, float D)
        {
            return new Quaternion(A, B, C, D);
        }

        public AOCoord NewCoord(float X, float Y, float Z)
        {
            return new AOCoord(X, Y, Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cli"></param>
        /// <param name="tosend"></param>
        public void Broadcast(Client cli, string tosend)
        {
            Announce.Broadcast(cli, tosend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cli"></param>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        /// <param name="announce"></param>
        public void SetStat(Client cli, int stat, uint value, bool announce)
        {
            cli.Character.Stats.Set(stat, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToPrint"></param>
        public void Print(string ToPrint)
        {
            PrintText("{0}", ToPrint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public void PrintText(string messageFormat, params object[] args)
        {
            LogScriptAction(messageFormat, args);
        }

        /// <summary>
        /// Executes a script with filename
        /// </summary>
        /// <param name="filename">Filename of the script.</param>
        public void ExecuteScript(string filename)
        {
            if (!File.Exists(filename))
            {
                LogScriptAction("ExecuteScript: Can't find file '{0}'", filename);
                return;
            }
            /* Lua Removed
            L.DoFile(filename);
            LuaTable a = new LuaTable(0, L);
             */
        }

        /* Lua Removed
        /// <summary>
        /// Returns handler to main Lua state.
        /// </summary>
        /// <returns>Lua.</returns>
        public Lua GetLua()
        {
            return L;
        }
        
        /// <summary>
        /// General Hook calling function. This will be the proxy for calling Lua functions from the Core code.
        /// </summary>
        /// <param name="Hook">The Hook name</param>
        /// <param name="argv">Array of values</param>
        /// <returns>Whether the hook call was successful or not.</returns>
        public bool CallHook(string Hook, params object[] argv)
        {
            //L.GetFunction(Hook).Call(argv);
            LuaFunction f = L.GetFunction(Hook);
            if (f != null)
            {
                object[] ret = null;
                try
                {
                    ret = f.Call(argv);
                }
                catch (LuaException e)
                {
                    LogScriptAction("Got exception: {0}", e.Message);
                }

                //return (bool)ret[0];
                if (ret == null) { return true; }
                if (ret[0] != null)
                {
                    return (bool)ret[0];
                }
                else
                {
                    return true;
                }
            }
            else
            {
                LogScriptAction("Can't find hook to call: '{0}'.", Hook);
                return false;
            }
        }

        /// <summary>
        /// Returns timer object to Lua.
        /// </summary>
        /// <param name="Interval">Interval until timer end in ms</param>
        /// <param name="f">Function name string</param>
        /// <param name="RemoveOnEnd">Whether to remove the timer when it ends</param>
        /// <returns></returns>
        public LuaTimer NewTimer(int Interval, string f, bool RemoveOnEnd)
        {
            return new LuaTimer(Interval, f, RemoveOnEnd);
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagefmt"></param>
        /// <param name="args"></param>
        public static void LogScriptAction(string messagefmt, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("++ ScriptAPI: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(messagefmt + "\n", args);
            Console.ResetColor();
        }

        //--- internal funcs

        //public static Lua L = null; 
    }
}