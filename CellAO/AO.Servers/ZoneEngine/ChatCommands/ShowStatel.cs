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

namespace ZoneEngine.ChatCommands
{
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Script;

    public class ChatCommandShowStatel : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            client.SendChatText("Looking up for statel in playfield " + client.Character.PlayField.ToString());
            Statels.Statel o = null;
            foreach (Statels.Statel s in Statels.Statelppf[client.Character.PlayField])
            {
                if (o == null)
                {
                    o = s;
                }
                else
                {
                    if (AOCoord.Distance2D(client.Character.Coordinates, s.Coordinates)
                        < AOCoord.Distance2D(client.Character.Coordinates, o.Coordinates))
                    {
                        o = s;
                    }
                }
            }
            if (o == null)
            {
                client.SendChatText("No statel on this playfield... Very odd, where exactly are you???");
                return;
            }
            client.SendChatText(o.Type + ":" + o.Instance);
            foreach (Statels.Statel_Event se in o.Events)
            {
                client.SendChatText(
                    "Event: " + se.EventNumber.ToString() + " # of Functions: " + se.Functions.Count.ToString());
                foreach (Statels.Statel_Function sf in se.Functions)
                {
                    string Fargs = "";
                    foreach (string arg in sf.Arguments)
                    {
                        if (Fargs.Length > 0)
                        {
                            Fargs = Fargs + ", ";
                        }
                        Fargs = Fargs + arg;
                    }
                    client.SendChatText(
                        "    Fn: " + sf.FunctionNumber.ToString() + ", # of Args: " + sf.Arguments.Count.ToString());
                    client.SendChatText("    Args: " + Fargs);
                    foreach (Statels.Statel_Function_Requirement sfr in sf.Requirements)
                    {
                        string req;
                        req = "Attr: " + sfr.AttributeNumber.ToString() + " Value: " + sfr.AttributeValue.ToString()
                              + " Target: " + sfr.Target.ToString() + " Op: " + sfr.Operator.ToString();
                        client.SendChatText("    Req: " + req);
                    }
                }
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command showstatel");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Always true, only string arguments
            return true;
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> GetCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("showstatel");
            return temp;
        }
    }
}