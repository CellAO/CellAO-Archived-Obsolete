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

    internal class Function_texture : FunctionPrototype
    {
        public new int FunctionNumber = 53039;

        public new string FunctionName = "texture";

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
            if ((Target is Character) || (Target is NonPlayerCharacterClass))
            {
                Character t = (Character)Target;
                bool found = false;
                Int32 placement;
                if (Arguments.Length == 2)
                {
                    placement = 0;
                }
                else
                {
                    placement = (Int32)Arguments[Arguments.Length - 1];
                }
                if (placement >= 49)
                {
                    if (t.SocialTab.ContainsKey((Int32)Arguments[1]))
                    {
                        t.SocialTab[(Int32)Arguments[1]] = (Int32)Arguments[0];
                    }
                    else
                    {
                        t.SocialTab.Add((Int32)Arguments[1], (Int32)Arguments[0]);
                    }
                }
                else
                {
                    foreach (AOTextures aot in t.Textures)
                    {
                        if (aot.place == (Int32)Arguments[1])
                        {
                            found = true;
                            aot.Texture = (Int32)Arguments[0];
                        }
                    }
                    if (!found)
                    {
                        t.Textures.Add(new AOTextures((Int32)Arguments[1], (Int32)Arguments[0]));
                    }
                }
            }
            return true;
        }
    }
}