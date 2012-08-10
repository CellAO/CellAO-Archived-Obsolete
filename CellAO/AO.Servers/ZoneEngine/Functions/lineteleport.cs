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
using AO.Core;
using ZoneEngine.Collision;
#endregion

namespace ZoneEngine.Functions
{
    internal class Function_lineteleport : FunctionPrototype
    {
        public new int FunctionNumber = 53059;

        public new string FunctionName = "lineteleport";

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

        /// <summary>
        /// Lineteleport
        /// </summary>
        /// <param name="Self"></param>
        /// <param name="Caller"></param>
        /// <param name="Target"></param>
        /// <param name="Arguments">UInt32 (LineType?), UInt32 (Combo of Line# and Playfield), Int32 (Playfield to teleport to)</param>
        /// <returns></returns>
        public bool FunctionExecute(Dynel Self, Dynel Caller, object Target, object[] Arguments)
        {
            uint arg2;
            int to_pf;
            Console.WriteLine(Target.GetType().ToString());
            if (Target is Statels.Statel)
            {
                arg2 = UInt32.Parse((string) Arguments[1]); // Linesegment and playfield (lower word)
                arg2 = arg2 >> 16;
                to_pf = Int32.Parse((string) Arguments[2]);
            }
            else
            {
                arg2 = (UInt32) Arguments[1];
                to_pf = (Int32) Arguments[2];
            }
            coordheading a = FindEntry(to_pf, (Int32) arg2);
            if (a.Coordinates.x != -1)
            {
                ((Character) Self).client.Teleport(a.Coordinates, a.Heading, to_pf);
                return true;
            }
            return false;
        }

        public coordheading FindEntry(int Playfield, int DestinationNumber)
        {
            coordheading ret = new coordheading();
            ret.Coordinates.x = -1;
            foreach (WallCollision.Line l in WallCollision.destinations[Playfield].playfield.lines)
            {
                if (l.ID != DestinationNumber)
                {
                    continue;
                }
                ret.Coordinates.x = (l.start.X + l.end.X)/2;
                ret.Coordinates.y = (l.start.Y + l.end.Y)/2;
                ret.Coordinates.z = (l.start.Z + l.end.Z)/2;
                // TODO: Calculate the right Quaternion for the heading...
                // - Algorithman
                Quaternion q = new Quaternion(new Vector3((l.end.X - l.start.X), 1, -(l.end.Z - l.start.Z)));
                ret.Heading.x = q.x;
                ret.Heading.y = q.y;
                ret.Heading.z = q.z;
                ret.Heading.w = q.w;
            }
            return ret;
        }
    }
}