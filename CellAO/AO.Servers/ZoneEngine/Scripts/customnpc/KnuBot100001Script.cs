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

#region NameSpace

namespace ZonEngine.Script.customnpcs
{
    using System;

    using AO.Core;

    using ZoneEngine;
    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    #region Class GenericNPC
    /// <summary>
    /// A Basic NPC from a script
    /// Has to be public or when in assembly form .net wont see it
    /// </summary>
    public class KnuBot100001Script : IAOScript
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Script Entry Point
        /// <summary>
        /// Script Entry Point
        /// </summary>
        /// <param name="args"></param>
        public void Main(string[] args)
        {
        }
        #endregion

        #region Script Initialization
        // This will be called by ZoneServer at startup after loading all Spawns/Vendors etc.
        // The Character parameter is not used here, but has to be defined (yet)
        public void Init(Character ch)
        {
            NonPlayerCharacterClass target = (NonPlayerCharacterClass)FindDynel.FindDynelById(50000, 100001);
            if (target != null)
            {
                target.KnuBot = new KnuBotNPC100001(target);
                Console.WriteLine("Registered KnuBot100001 Script with NPC");
            }
        }
        #endregion
    }
    #endregion Class GenericNPC

    #region Our KnuBot implementation
    public class KnuBotNPC100001 : KnuBotClass
    {
        private int lastaction;

        public KnuBotNPC100001(Character target, NonPlayerCharacterClass _parent)
            : base(target, _parent)
        {
        }

        // Adding our event handlers
        public KnuBotNPC100001(NonPlayerCharacterClass _parent)
            : base(_parent)
        {
            this.CallKnuBotCloseChatWindow += this.MyCloseChatWindow;
            this.CallKnuBotAnswer += this.MyAnswer;
            this.CallKnuBotOpenChatWindow += this.MyOpenChatWindow;
            this.CallKnuBotAcceptTrade += this.MyAcceptTrade;
            this.CallKnuBotDeclineTrade += this.MyDeclineTrade;
            this.CallKnuBotStartTrade += this.MyStartTrade;
        }

        public override void Action(Int32 actionnumber)
        {
            if (this.TalkingTo == null)
            {
                return;
            }
            switch (actionnumber)
            {
                case 0: // Start
                    this.OpenChat();
                    this.KnuBotNextAction(1, 400);
                    break;
                case 1:
                    this.AppendText("Hello %name.");
                    this.AppendText("I'm the mighty %myname and this is a test of the KnuBot functions:\n");
                    this.KnuBotNextAction(2, 400);
                    break;
                case 2:
                    this.AppendText("Implemented now:");
                    this.AppendText("- Teleportation");
                    this.AppendText("- Item Spawn");
                    this.AppendText("- Stat change");
                    this.KnuBotNextAction(3, 400);
                    break;
                case 3:
                    this.SendChoices(
                        new[] { "Boost my level", "Boost my AI level", "Teleportation", "Gimme a Yalmaha", "Good bye" });
                    break;
                case 5:
                    this.CloseChat();
                    break;
                case 30:
                    this.SendChoices(new[] { "15", "25", "60", "100", "150", "175", "200", "220", "Something else" });
                    break;
                case 31:
                    this.SendChoices(new[] { "10", "20", "30", "Something else" });
                    break;
                case 32:
                    this.SendChoices(new[] { "Parnassos", "Parnassos Ark HQ", "Borealis", "Something else" });
                    break;
                case 33:
                    this.SendChoices(new[] { "Yalmaha XL Gold Flash", "Back" });
                    break;
            }
            this.lastaction = actionnumber;
        }

        public void MyAnswer(object sender, KnuBotAnswerEventArgs e)
        {
            switch (this.lastaction)
            {
                case 3:
                    this.TalkingTo.Client.SendChatText("Chosen number " + e.Answer.ToString());
                    switch (e.Answer)
                    {
                        case 0:
                            this.KnuBotNextAction(30, 100);
                            break;
                        case 1:
                            this.KnuBotNextAction(31, 100);
                            break;
                        case 2:
                            this.KnuBotNextAction(32, 100);
                            break;
                        case 3:
                            this.KnuBotNextAction(33, 100);
                            break;
                        case 4:
                            this.CloseChat();
                            break;
                            /* Other way to do this:
                if (e._answer<4)
                {
                    KnuBotNextAction(lastaction * 10 + e._answer, 100);
                    break;
                }
                CloseChat(); break;
                         */
                    }
                    break;
                case 30:
                    switch (e.Answer)
                    {
                            // { "15", "25", "60", "100", "150", "175", "200", "220", "Something else" }
                        case 0:
                            this.TalkingTo.Stats.Level.Value = 15;
                            break;
                        case 1:
                            this.TalkingTo.Stats.Level.Value = 25;
                            break;
                        case 2:
                            this.TalkingTo.Stats.Level.Value = 60;
                            break;
                        case 3:
                            this.TalkingTo.Stats.Level.Value = 100;
                            break;
                        case 4:
                            this.TalkingTo.Stats.Level.Value = 150;
                            break;
                        case 5:
                            this.TalkingTo.Stats.Level.Value = 175;
                            break;
                        case 6:
                            this.TalkingTo.Stats.Level.Value = 200;
                            break;
                        case 7:
                            this.TalkingTo.Stats.Level.Value = 220;
                            break;
                    }
                    this.KnuBotNextAction(1, 100);
                    break;
                case 31:
                    switch (e.Answer)
                    {
                            // { "10", "20", "30", "Something else" }
                        case 0:
                            this.TalkingTo.Stats.AlienLevel.Value = 10;
                            break;
                        case 1:
                            this.TalkingTo.Stats.AlienLevel.Value = 20;
                            break;
                        case 2:
                            this.TalkingTo.Stats.AlienLevel.Value = 30;
                            break;
                    }
                    this.KnuBotNextAction(1, 100);
                    break;
                case 32:
                    switch (e.Answer)
                    {
                        case 0:
                            this.Teleport(458, 317, 37, 500, 500);
                            this.CloseChat();
                            break;
                        case 1:
                            this.Teleport(702, 749, 40, 501, 500);
                            this.CloseChat();
                            break;
                        case 2:
                            this.Teleport(670, 535, 73, 800, 500);
                            this.CloseChat();
                            break;
                        case 3:
                            this.KnuBotNextAction(1, 100);
                            break;
                    }

                    break;
                case 33:
                    switch (e.Answer)
                    {
                        case 0:
                            this.AppendText("Oh, you like the gold one? Nice choice.");
                            this.SpawnItem(203292, 203293, 30);
                            break;
                    }
                    this.KnuBotNextAction(1, 100);
                    break;
            }
        }

        public void MyCloseChatWindow(object sender, KnuBotEventArgs e)
        {
            this.Parent.PurgeTimer(20000);
        }

        public void MyOpenChatWindow(object sender, KnuBotEventArgs e)
        {
            this.lastaction = 0;
            if (this.TalkingTo != null)
            {
                this.TalkingTo = e.Sender;
            }
            this.Action(0);
        }

        public void MyDeclineTrade(object sender, KnuBotEventArgs e)
        {
            this.KnuBotNextAction(1, 1000);
        }

        public void MyAcceptTrade(object sender, KnuBotEventArgs e)
        {
            foreach (AOItem item in this.TradedItems)
            {
                // Spawn back our items with max QL
                this.SpawnItem(item.LowID, item.HighID, 500);
                this.AppendText("Spawned back with max ql: " + item.LowID + "/" + item.HighID);
            }
            this.TradedItems.Clear();
            this.CloseChat();
        }

        public void MyStartTrade(object sender, KnuBotEventArgs e)
        {
            ZoneEngine.PacketHandlers.KnuBotStartTrade.Send(
                this.TalkingTo.Client, this.Parent, "Wanna trade? Go ahead.", 4);
        }
    }
    #endregion
}

#endregion NameSpace