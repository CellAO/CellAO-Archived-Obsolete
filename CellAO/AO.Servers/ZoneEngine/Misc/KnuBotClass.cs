﻿#region License
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

namespace ZoneEngine.Misc
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Packets;

    public class KnuBotClass
    {
        protected Character _talkingto;

        public Character TalkingTo
        {
            get
            {
                return this._talkingto;
            }
            set
            {
                this._talkingto = value;
            }
        }

        public NonPlayerCharacterClass parent;

        public List<AOItem> TradedItems = new List<AOItem>();

        public bool InTrade;

        public bool WantsTrade;

        public KnuBotClass(Character target, NonPlayerCharacterClass _parent)
        {
            this.TalkingTo = target;
            this.parent = _parent;
        }

        public KnuBotClass(NonPlayerCharacterClass _parent)
        {
            this.TalkingTo = null;
            this.parent = _parent;

            this.CallKnuBotAcceptTrade += this.OnKnuBotAcceptTrade;
            this.CallKnuBotAnswer += this.OnKnuBotAnswer;
            this.CallKnuBotCloseChatWindow += this.OnKnuBotCloseChatWindow;
            this.CallKnuBotDeclineTrade += this.OnKnuBotDeclineTrade;
            this.CallKnuBotOpenChatWindow += this.OnKnuBotOpenChatWindow;
            this.CallKnuBotStartTrade += this.OnKnuBotStartTrade;
            this.CallKnuBotTrade += this.OnKnuBotTrade;
        }

        public virtual void OnKnuBotAnswer(object sender, KnuBotAnswerEventArgs e)
        {
        }

        public virtual void OnKnuBotCloseChatWindow(object sender, KnuBotEventArgs e)
        {
            if (this.TradedItems.Count > 0)
            {
                KnuBotRejectedItems.Send(this.TalkingTo.client, this.parent, this.TradedItems.ToArray());
                this.TradedItems.Clear();
            }
        }

        public virtual void OnKnuBotOpenChatWindow(object sender, KnuBotEventArgs e)
        {
        }

        public virtual void OnKnuBotAcceptTrade(object sender, KnuBotEventArgs e)
        {
            this.InTrade = false;
        }

        public virtual void OnKnuBotDeclineTrade(object sender, KnuBotEventArgs e)
        {
            KnuBotRejectedItems.Send(this.TalkingTo.client, this.parent, this.TradedItems.ToArray());
            foreach (AOItem item in this.TradedItems)
            {
                this.TalkingTo.AddItemToInventory(item);
            }
            this.TradedItems.Clear();
            this.InTrade = false;
        }

        public virtual void OnKnuBotStartTrade(object sender, KnuBotEventArgs e)
        {
            if (this.WantsTrade && !this.InTrade)
            {
                this.TradedItems.Clear();
                this.InTrade = true;
            }
        }

        public virtual void OnKnuBotTrade(object sender, KnuBotTradeEventArgs e)
        {
            this.TradedItems.Add(e._item);
        }

        public virtual void Action(Int32 actionnumber)
        {
        }

        public void KnuBotNextAction(int ActionNumber, uint delay)
        {
            this.parent.AddTimer(
                20000, DateTime.Now + TimeSpan.FromMilliseconds(delay), this.CreateKnuBotFunction(ActionNumber), false);
        }

        public void AppendText(string message)
        {
            KnuBotAppendText.Send(
                this.TalkingTo.client,
                this.parent,
                message.Replace("%name", this.TalkingTo.Name).Replace("%myname", this.parent.Name) + "\n");
        }

        public void SendChoices(string[] choices)
        {
            KnuBotAnswerList.Send(this.TalkingTo.client, this.parent, choices);
        }

        public void OpenTrade(string message, int numberofitemslots)
        {
            PacketHandlers.KnuBotStartTrade.Send(this.TalkingTo.client, this.parent, message, numberofitemslots);
        }

        public void OpenChat()
        {
            Packets.KnuBotOpenChatWindow.Send(this.TalkingTo.client, this.parent);
        }

        public void CloseChat()
        {
            PacketHandlers.KnuBotCloseChatWindow.Send(this.TalkingTo, this.parent);
        }

        public void SpawnItem(int lowid, int highid, int ql)
        {
            // TODO: Add check for full inventory!
            InventoryEntries mi = new InventoryEntries();
            AOItem it = ItemHandler.interpolate(lowid, highid, ql);
            mi.Item = it;
            mi.Container = 104;
            mi.Placement = this.TalkingTo.GetNextFreeInventory(104);
            this.TalkingTo.Inventory.Add(mi);
            AddTemplate.Send(this.TalkingTo.client, mi);
        }

        public AOFunctions CreateKnuBotFunction(int KnuBotaction)
        {
            AOFunctions aof = new AOFunctions();
            aof.Arguments.Add(KnuBotaction);
            aof.TickCount = 1;
            aof.TickInterval = 0;
            aof.FunctionType = 2; // KnuBotActionTimer
            aof.Target = this.parent.ID;
            return aof;
        }

        public void Teleport(int x, int z, int y, int pf, uint delay)
        {
            AOFunctions aof = new AOFunctions();
            aof.Target = this.TalkingTo.ID;
            aof.TickCount = 0;
            aof.TickInterval = 1;
            aof.FunctionType = Constants.functiontype_teleport;
            aof.Arguments.Add(x);
            aof.Arguments.Add(y);
            aof.Arguments.Add(z);
            aof.Arguments.Add(pf);
            this.TalkingTo.AddTimer(20001, DateTime.Now + TimeSpan.FromMilliseconds(delay), aof, false);
        }

        #region KnuBot Events
        public class KnuBotAnswerEventArgs : EventArgs
        {
            public Character _sender;

            public int _answer;

            public KnuBotAnswerEventArgs(Character sender, int answer)
            {
                this._sender = sender;
                this._answer = answer;
            }
        }

        public event EventHandler<KnuBotAnswerEventArgs> CallKnuBotAnswer;

        protected virtual void OnKnuBotAnswerEvent(KnuBotAnswerEventArgs e)
        {
            EventHandler<KnuBotAnswerEventArgs> handler = this.CallKnuBotAnswer;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotAnswer(Character ch, int number)
        {
            this.OnKnuBotAnswerEvent(new KnuBotAnswerEventArgs(ch, number));
        }

        public class KnuBotEventArgs : EventArgs
        {
            public Character _sender;

            public KnuBotEventArgs(Character sender)
            {
                this._sender = sender;
            }
        }

        public event EventHandler<KnuBotEventArgs> CallKnuBotCloseChatWindow;

        protected virtual void OnKnuBotCloseChatWindowEvent(KnuBotEventArgs e)
        {
            EventHandler<KnuBotEventArgs> handler = this.CallKnuBotCloseChatWindow;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotCloseChatWindow(Character ch)
        {
            this.OnKnuBotCloseChatWindowEvent(new KnuBotEventArgs(ch));
            // TODO: Give back Items in Trade window
        }

        public event EventHandler<KnuBotEventArgs> CallKnuBotOpenChatWindow;

        protected virtual void OnKnuBotOpenChatWindowEvent(KnuBotEventArgs e)
        {
            EventHandler<KnuBotEventArgs> handler = this.CallKnuBotOpenChatWindow;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotOpenChatWindow(Character ch)
        {
            this.OnKnuBotOpenChatWindowEvent(new KnuBotEventArgs(ch));
        }

        public event EventHandler<KnuBotEventArgs> CallKnuBotDeclineTrade;

        protected virtual void OnKnuBotDeclineTradeEvent(KnuBotEventArgs e)
        {
            EventHandler<KnuBotEventArgs> handler = this.CallKnuBotDeclineTrade;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotDeclineTrade(Character ch)
        {
            this.OnKnuBotDeclineTradeEvent(new KnuBotEventArgs(ch));
        }

        public event EventHandler<KnuBotEventArgs> CallKnuBotAcceptTrade;

        protected virtual void OnKnuBotAcceptTradeEvent(KnuBotEventArgs e)
        {
            EventHandler<KnuBotEventArgs> handler = this.CallKnuBotAcceptTrade;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotAcceptTrade(Character character)
        {
            this.OnKnuBotAcceptTradeEvent(new KnuBotEventArgs(character));
        }

        public event EventHandler<KnuBotEventArgs> CallKnuBotStartTrade;

        protected virtual void OnKnuBotStartTradeEvent(KnuBotEventArgs e)
        {
            EventHandler<KnuBotEventArgs> handler = this.CallKnuBotStartTrade;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotStartTrade(Character character)
        {
            this.OnKnuBotStartTradeEvent(new KnuBotEventArgs(character));
        }

        public class KnuBotTradeEventArgs : EventArgs
        {
            public Character _sender;

            public AOItem _item;

            public KnuBotTradeEventArgs(Character sender, AOItem item)
            {
                this._sender = sender;
                this._item = item;
            }
        }

        public event EventHandler<KnuBotTradeEventArgs> CallKnuBotTrade;

        protected virtual void OnKnuBotTradeEvent(KnuBotTradeEventArgs e)
        {
            EventHandler<KnuBotTradeEventArgs> handler = this.CallKnuBotTrade;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void KnuBotTrade(Character character, AOItem item)
        {
            this.OnKnuBotTradeEvent(new KnuBotTradeEventArgs(character, item));
        }
        #endregion
    }
}