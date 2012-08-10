#region Usings...
using System.Collections.Generic;
using ZoneEngine.Misc;
using ZoneEngine.Packets;
#endregion

namespace ZoneEngine.Functions
{
    internal class Function_StatTimerFunction : FunctionPrototype
    {
        public new int FunctionNumber = 1;

        public new string FunctionName = "StatTimerFunction";

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
            Dictionary<int, uint> statstoupdate = new Dictionary<int, uint>();
            foreach (Class_Stat cs in ch.Stats.all)
            {
                if (cs.changed)
                {
                    if (cs.SendBaseValue)
                    {
                        statstoupdate.Add(cs.StatNumber, cs.StatBaseValue);
                    }
                    else
                    {
                        statstoupdate.Add(cs.StatNumber, (uint) cs.Value);
                    }
                    cs.changed = false;
                }
            }
            if (ch.client == null)
            {
                Stat.SendBulk(ch, statstoupdate);
            }
            else
            {
                Stat.SendBulk(ch.client, statstoupdate);
            }
            return true;
        }
    }
}