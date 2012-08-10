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

namespace ZoneEngine.Functions
{
    internal class Function_npcapplynanoformula : FunctionPrototype
    {
        public new int FunctionNumber = 53102;

        public new string FunctionName = "npcapplynanoformula";

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
            return false;
        }
    }

    internal class Function_npccallforhelp : FunctionPrototype
    {
        public new int FunctionNumber = 53120;

        public new string FunctionName = "npccallforhelp";

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
            return false;
        }
    }

    internal class Function_npccastnanoifpossible : FunctionPrototype
    {
        public new int FunctionNumber = 53215;

        public new string FunctionName = "npccastnanoifpossible";

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
            return false;
        }
    }

    internal class Function_npccastnanoifpossibleonfighttarget : FunctionPrototype
    {
        public new int FunctionNumber = 53216;

        public new string FunctionName = "npccastnanoifpossibleonfighttarget";

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
            return false;
        }
    }

    internal class Function_npcenabledieofboredom : FunctionPrototype
    {
        public new int FunctionNumber = 53146;

        public new string FunctionName = "npcenabledieofboredom";

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
            return false;
        }
    }

    internal class Function_npcdisablemovement : FunctionPrototype
    {
        public new int FunctionNumber = 53148;

        public new string FunctionName = "npcdisablemovement";

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
            return false;
        }
    }

    internal class Function_npccreatepet : FunctionPrototype
    {
        public new int FunctionNumber = 53129;

        public new string FunctionName = "npccreatepet";

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
            return false;
        }
    }

    internal class Function_npcclonetarget : FunctionPrototype
    {
        public new int FunctionNumber = 53161;

        public new string FunctionName = "npcclonetarget";

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
            return false;
        }
    }

    internal class Function_npcclearsignal : FunctionPrototype
    {
        public new int FunctionNumber = 53119;

        public new string FunctionName = "npcclearsignal";

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
            return false;
        }
    }

    internal class Function_npcfollowselected : FunctionPrototype
    {
        public new int FunctionNumber = 53095;

        public new string FunctionName = "npcfollowselected";

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
            return false;
        }
    }

    internal class Function_npcfightselected : FunctionPrototype
    {
        public new int FunctionNumber = 53077;

        public new string FunctionName = "npcfightselected";

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
            return false;
        }
    }

    internal class Function_npcfakeattackontarget : FunctionPrototype
    {
        public new int FunctionNumber = 53145;

        public new string FunctionName = "npcfakeattackontarget";

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
            return false;
        }
    }

    internal class Function_npcenablepvprules : FunctionPrototype
    {
        public new int FunctionNumber = 53172;

        public new string FunctionName = "npcenablepvprules";

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
            return false;
        }
    }

    internal class Function_npcenablegroundtoaircombat : FunctionPrototype
    {
        public new int FunctionNumber = 53170;

        public new string FunctionName = "npcenablegroundtoaircombat";

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
            return false;
        }
    }

    internal class Function_npckilltarget : FunctionPrototype
    {
        public new int FunctionNumber = 53131;

        public new string FunctionName = "npckilltarget";

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
            return false;
        }
    }

    internal class Function_npchatelisttargetaggroers : FunctionPrototype
    {
        public new int FunctionNumber = 53147;

        public new string FunctionName = "npchatelisttargetaggroers";

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
            return false;
        }
    }

    internal class Function_npchatelisttarget : FunctionPrototype
    {
        public new int FunctionNumber = 53081;

        public new string FunctionName = "npchatelisttarget";

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
            return false;
        }
    }

    internal class Function_npcgettargethatelist : FunctionPrototype
    {
        public new int FunctionNumber = 53090;

        public new string FunctionName = "npcgettargethatelist";

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
            return false;
        }
    }

    internal class Function_npcfreezehatelist : FunctionPrototype
    {
        public new int FunctionNumber = 53195;

        public new string FunctionName = "npcfreezehatelist";

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
            return false;
        }
    }

    internal class Function_npcsayrobotspeech : FunctionPrototype
    {
        public new int FunctionNumber = 53104;

        public new string FunctionName = "npcsayrobotspeech";

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
            return false;
        }
    }

    internal class Function_npcpushscript : FunctionPrototype
    {
        public new int FunctionNumber = 53107;

        public new string FunctionName = "npcpushscript";

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
            return false;
        }
    }

    internal class Function_npcpopscript : FunctionPrototype
    {
        public new int FunctionNumber = 53108;

        public new string FunctionName = "npcpopscript";

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
            return false;
        }
    }

    internal class Function_npcmovementaction : FunctionPrototype
    {
        public new int FunctionNumber = 53191;

        public new string FunctionName = "npcmovementaction";

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
            return false;
        }
    }

    internal class Function_npcmoveforward : FunctionPrototype
    {
        public new int FunctionNumber = 53096;

        public new string FunctionName = "npcmoveforward";

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
            return false;
        }
    }

    internal class Function_npcsetconfigstats : FunctionPrototype
    {
        public new int FunctionNumber = 53197;

        public new string FunctionName = "npcsetconfigstats";

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
            return false;
        }
    }

    internal class Function_npcsendplaysync : FunctionPrototype
    {
        public new int FunctionNumber = 53097;

        public new string FunctionName = "npcsendplaysync";

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
            return false;
        }
    }

    internal class Function_npcsendpetstatus : FunctionPrototype
    {
        public new int FunctionNumber = 53214;

        public new string FunctionName = "npcsendpetstatus";

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
            return false;
        }
    }

    internal class Function_npcsendcommand : FunctionPrototype
    {
        public new int FunctionNumber = 53103;

        public new string FunctionName = "npcsendcommand";

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
            return false;
        }
    }

    internal class Function_npcselecttarget : FunctionPrototype
    {
        public new int FunctionNumber = 53062;

        public new string FunctionName = "npcselecttarget";

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
            return false;
        }
    }

    internal class Function_npcsetwanderingmode : FunctionPrototype
    {
        public new int FunctionNumber = 53199;

        public new string FunctionName = "npcsetwanderingmode";

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
            return false;
        }
    }

    internal class Function_npcsetstuckdetectscheme : FunctionPrototype
    {
        public new int FunctionNumber = 53171;

        public new string FunctionName = "npcsetstuckdetectscheme";

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
            return false;
        }
    }

    internal class Function_npcsetsneakmode : FunctionPrototype
    {
        public new int FunctionNumber = 53190;

        public new string FunctionName = "npcsetsneakmode";

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
            return false;
        }
    }

    internal class Function_npcsetmovetotarget : FunctionPrototype
    {
        public new int FunctionNumber = 53198;

        public new string FunctionName = "npcsetmovetotarget";

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
            return false;
        }
    }

    internal class Function_npcsetmaster : FunctionPrototype
    {
        public new int FunctionNumber = 53091;

        public new string FunctionName = "npcsetmaster";

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
            return false;
        }
    }

    internal class Function_npcstopsurrender : FunctionPrototype
    {
        public new int FunctionNumber = 53114;

        public new string FunctionName = "npcstopsurrender";

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
            return false;
        }
    }

    internal class Function_npcstoppetduel : FunctionPrototype
    {
        public new int FunctionNumber = 53219;

        public new string FunctionName = "npcstoppetduel";

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
            return false;
        }
    }

    internal class Function_npcstopmoving : FunctionPrototype
    {
        public new int FunctionNumber = 53116;

        public new string FunctionName = "npcstopmoving";

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
            return false;
        }
    }

    internal class Function_npcstartsurrender : FunctionPrototype
    {
        public new int FunctionNumber = 53113;

        public new string FunctionName = "npcstartsurrender";

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
            return false;
        }
    }

    internal class Function_npcsocialanim : FunctionPrototype
    {
        public new int FunctionNumber = 53078;

        public new string FunctionName = "npcsocialanim";

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
            return false;
        }
    }

    internal class Function_npctogglefov : FunctionPrototype
    {
        public new int FunctionNumber = 53183;

        public new string FunctionName = "npctogglefov";

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
            return false;
        }
    }

    internal class Function_npctogglefightmoderegenrate : FunctionPrototype
    {
        public new int FunctionNumber = 53179;

        public new string FunctionName = "npctogglefightmoderegenrate";

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
            return false;
        }
    }

    internal class Function_npcteleporttospawnpoint : FunctionPrototype
    {
        public new int FunctionNumber = 53143;

        public new string FunctionName = "npcteleporttospawnpoint";

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
            return false;
        }
    }

    internal class Function_npctargethasitem : FunctionPrototype
    {
        public new int FunctionNumber = 53217;

        public new string FunctionName = "npctargethasitem";

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
            return false;
        }
    }

    internal class Function_npcsummonenemy : FunctionPrototype
    {
        public new int FunctionNumber = 53163;

        public new string FunctionName = "npcsummonenemy";

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
            return false;
        }
    }

    internal class Function_npcwipehatelist : FunctionPrototype
    {
        public new int FunctionNumber = 53126;

        public new string FunctionName = "npcwipehatelist";

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
            return false;
        }
    }

    internal class Function_npcusespecialattackitem : FunctionPrototype
    {
        public new int FunctionNumber = 53194;

        public new string FunctionName = "npcusespecialattackitem";

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
            return false;
        }
    }

    internal class Function_npcuniqueplayersinhatelist : FunctionPrototype
    {
        public new int FunctionNumber = 53203;

        public new string FunctionName = "npcuniqueplayersinhatelist";

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
            return false;
        }
    }

    internal class Function_npcturntotarget : FunctionPrototype
    {
        public new int FunctionNumber = 53080;

        public new string FunctionName = "npcturntotarget";

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
            return false;
        }
    }

    internal class Function_npctrygroupform : FunctionPrototype
    {
        public new int FunctionNumber = 53098;

        public new string FunctionName = "npctrygroupform";

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
            return false;
        }
    }
}