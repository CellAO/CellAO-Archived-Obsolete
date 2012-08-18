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

    using AO.Core;

    internal class Function_teleport : FunctionPrototype
    {
        public new int FunctionNumber = 53016;

        public new string FunctionName = "teleport";

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

        public bool FunctionExecute(Dynel self, Dynel caller, object target, object[] arguments)
        {
            Client cli = ((Character)self).Client;
            Quaternion q = new Quaternion(0, 1, 0, 0);
            AOCoord a = new AOCoord();
            Int32 pf;
            if (target is Statels.Statel)
            {
                a.x = Int32.Parse((string)arguments[0]);
                a.y = Int32.Parse((string)arguments[1]);
                a.z = Int32.Parse((string)arguments[2]);
                pf = Int32.Parse((string)arguments[3]);
            }
            else
            {
                a.x = (Int32)arguments[0];
                a.y = (Int32)arguments[1];
                a.z = (Int32)arguments[2];
                pf = (Int32)arguments[3];
            }
            cli.Teleport(a, q, pf);
            return true;
        }
    }
}