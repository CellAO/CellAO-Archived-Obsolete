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

    internal class Function_hit : FunctionPrototype
    {
        public new int FunctionNumber = 53002;

        public new string FunctionName = "hit";

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
            int Statnumber;
            int minhit;
            int maxhit;
            if (Target is Statels.Statel)
            {
                Statnumber = Int32.Parse((string)Arguments[0]);
                minhit = Int32.Parse((string)Arguments[1]);
                maxhit = Int32.Parse((string)Arguments[2]);
                if (minhit > maxhit)
                {
                    minhit = maxhit;
                    maxhit = Int32.Parse((string)Arguments[1]);
                }
            }
            else
            {
                Statnumber = (int)Arguments[0];
                minhit = (int)Arguments[1];
                maxhit = (int)Arguments[2];
                if (minhit > maxhit)
                {
                    minhit = maxhit;
                    maxhit = (int)Arguments[1];
                }
            }
            Random rnd = new Random();
            int random = rnd.Next(minhit, maxhit);
            Character ch = (Character)Self;

            // Increase only to maximum value. if max value is lower then actual value, half of the random will be subtracted
            if (Statnumber == 27)
            {
                random = Math.Min(random, ch.Stats.Life.Value - ch.Stats.Health.Value);
            }
            if (Statnumber == 132)
            {
                random = Math.Min(random, ch.Stats.MaxNanoEnergy.Value - ch.Stats.NanoEnergyPool.Value);
            }
            if (random < 0)
            {
                random /= 2;
            }

            ((Character)Self).Stats.SetStatValueByName(
                Statnumber, (uint)(((Character)Self).Stats.StatValueByName(Statnumber) + random));
            return true;
        }
    }
}