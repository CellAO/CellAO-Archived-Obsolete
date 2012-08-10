#region Usings...
using System;
using AO.Core;
#endregion

namespace ZoneEngine.Functions
{
    internal class Function_KnuBotAction : FunctionPrototype
    {
        public new int FunctionNumber = 2;

        public new string FunctionName = "KnuBotAction";

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

        public AOFunctions CreateKnuBotFunction(int KnuBotaction)
        {
            AOFunctions aof = new AOFunctions();
            aof.Arguments.Add(KnuBotaction);
            aof.TickCount = 1;
            aof.TickInterval = 0;
            aof.FunctionType = FunctionNumber;
            return aof;
        }

        public void KnuBotNextAction(Character Self, int ActionNumber, uint delay)
        {
            Self.AddTimer(20000, DateTime.Now + TimeSpan.FromMilliseconds(delay), CreateKnuBotFunction(ActionNumber),
                          false);
        }


        public bool FunctionExecute(Dynel Self, Dynel Caller, object Target, object[] Arguments)
        {
            int actionnumber = (Int32) Arguments[0];
            NonPC _self = (NonPC) Self;
            _self.KnuBot.Action((Int32) Arguments[0]);
            return true;
        }
    }
}