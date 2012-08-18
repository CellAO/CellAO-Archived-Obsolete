#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...
#endregion

namespace ZoneEngine
{
    using System;
    using System.IO;

    using AO.Core;

    using ZoneEngine.Misc;

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
        /// <param name="client"> </param>
        /// <param name="toSend"></param>
        public void Broadcast(Client client, string toSend)
        {
            Announce.Broadcast(client, toSend);
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
            cli.Character.Stats.SetStatValueByName(stat, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToPrint"></param>
        public void Print(string ToPrint)
        {
            this.PrintText("{0}", ToPrint);
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
        /// <param name="fileName">Filename of the script.</param>
        public void ExecuteScript(string fileName)
        {
            if (!File.Exists(fileName))
            {
                LogScriptAction("ExecuteScript: Can't find file '{0}'", fileName);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="arguments"></param>
        public static void LogScriptAction(string messageFormat, params object[] arguments)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("++ ScriptAPI: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(messageFormat + "\n", arguments);
            Console.ResetColor();
        }
    }
}