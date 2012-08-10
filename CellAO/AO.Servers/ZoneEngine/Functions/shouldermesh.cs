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
#endregion

namespace ZoneEngine.Functions
{
    internal class Function_shouldermesh : FunctionPrototype
    {
        public new int FunctionNumber = 53038;

        public new string FunctionName = "shouldermesh";

        public override int ReturnNumber()
        {
            return FunctionNumber;
        }

        public override bool Execute(Dynel Self, Dynel Caller, object Target, object[] Arguments)
        {
            lock (Self)
            {
                lock (Caller)
                {
                    lock (Target)
                    {
                        return FunctionExecute(Self, Caller, Target, Arguments);
                    }
                }
            }
        }

        public override string ReturnName()
        {
            return FunctionName;
        }

        public bool FunctionExecute(Dynel Self, Dynel Caller, object Target, object[] Arguments)
        {
            Character ch = (Character) Self;
            if (Arguments.Length == 2)
            {
                ch.Stats.ShoulderMeshLeft.StatModifier =
                    (Int32) ((Int32) Arguments[1] - ch.Stats.ShoulderMeshLeft.StatBaseValue);
                ch.Stats.ShoulderMeshRight.StatModifier =
                    (Int32) ((Int32) Arguments[1] - ch.Stats.ShoulderMeshRight.StatBaseValue);
                ch.MeshLayer.AddMesh(3, (Int32) Arguments[1], (Int32) Arguments[0], 4);
                ch.MeshLayer.AddMesh(4, (Int32) Arguments[1], (Int32) Arguments[0], 4);
            }
            else
            {
                Int32 placement = (Int32) Arguments[Arguments.Length - 1];
                if ((placement == 52) || (placement == 54)) // Social pads
                {
                    if (placement == 52)
                    {
                        ch.SocialMeshLayer.AddMesh(3, (Int32) Arguments[1], (Int32) Arguments[0], 4);
                    }
                    else
                    {
                        ch.SocialMeshLayer.AddMesh(4, (Int32) Arguments[1], (Int32) Arguments[0], 4);
                    }
                }
                else
                {
                    if (placement == 20)
                    {
                        ch.Stats.ShoulderMeshRight.StatModifier =
                            (Int32) ((Int32) Arguments[1] - ch.Stats.ShoulderMeshRight.StatBaseValue);
                        ch.MeshLayer.AddMesh(3, (Int32) Arguments[1], (Int32) Arguments[0], 4);
                    }
                    else
                    {
                        ch.Stats.ShoulderMeshLeft.StatModifier =
                            (Int32) ((Int32) Arguments[1] - ch.Stats.ShoulderMeshLeft.StatBaseValue);
                        ch.MeshLayer.AddMesh(4, (Int32) Arguments[1], (Int32) Arguments[0], 4);
                    }
                }
            }
            return true;
        }
    }
}