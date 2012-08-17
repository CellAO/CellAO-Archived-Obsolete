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

namespace ZoneEngine.Functions
{
    using System;
    using System.Data;

    using AO.Core;

    internal class Function_teleportproxy : FunctionPrototype
    {
        public new int FunctionNumber = 53082;

        public new string FunctionName = "teleportproxy";

        public override int ReturnNumber()
        {
            return this.FunctionNumber;
        }

        public override bool Execute(Dynel self, Dynel caller, object target, object[] arguments)
        {
            lock (self)
            {
                lock (caller)
                {
                    lock (target)
                    {
                        return this.FunctionExecute(self, caller, target, arguments);
                    }
                }
            }
        }

        public override string ReturnName()
        {
            return this.FunctionName;
        }

        public bool FunctionExecute(Dynel Self, Dynel Caller, object Target, object[] Arguments)
        {
            Identity pfinstance = new Identity();
            Identity id2 = new Identity();
            Identity id3 = new Identity();
            Client cli = ((Character)Self).Client;
            if (Target is Statels.Statel)
            {
                pfinstance.Type = Int32.Parse((string)Arguments[0]);
                pfinstance.Instance = Int32.Parse((string)Arguments[1]);
                id2.Type = Int32.Parse((string)Arguments[2]);
                id2.Instance = Int32.Parse((string)Arguments[3]);
                id3.Type = Int32.Parse((string)Arguments[4]);
                id3.Instance = Int32.Parse((string)Arguments[5]);
            }
            else
            {
                // Shouldnt happen ever, as far as i know only Statels do ProxyTeleports (perhaps GM's can too tho)
                pfinstance.Type = (Int32)Arguments[0];
                pfinstance.Instance = (Int32)Arguments[1];
                id2.Type = (Int32)Arguments[2];
                id2.Instance = (Int32)Arguments[3];
                id3.Type = (Int32)Arguments[4];
                id3.Instance = (Int32)Arguments[5];
            }
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable("SELECT * from proxydestinations WHERE playfield=" + pfinstance.Instance);
            if (dt.Rows.Count == 0)
            {
#if DEBUG
                cli.SendChatText("No Destination found for playfield " + pfinstance.Instance);
                foreach (string arg in Arguments)
                {
                    cli.SendChatText("Argument: " + arg);
                }
#endif
            }
            else
            {
                AOCoord a = new AOCoord();
                a.x = (Single)dt.Rows[0][1];
                a.y = (Single)dt.Rows[0][2];
                a.z = (Single)dt.Rows[0][3];
                Quaternion q = new Quaternion(0, 0, 0, 0);
                q.x = (Single)dt.Rows[0][4];
                q.y = (Single)dt.Rows[0][5];
                q.z = (Single)dt.Rows[0][6];
                q.w = (Single)dt.Rows[0][7];
                int instance = (Int32)((Statels.Statel)Target).Instance;
                // TODO: 1=GS need to change that later to a crowdbalancing system
                cli.TeleportProxy(a, q, pfinstance.Instance, pfinstance, 1, instance, id2, id3);
            }
            return true;
        }
    }
}