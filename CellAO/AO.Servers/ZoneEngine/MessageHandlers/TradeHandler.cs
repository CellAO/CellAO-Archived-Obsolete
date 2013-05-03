// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TradeHandler.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the TradeHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.MessageHandlers
{
    using System;
    using System.ComponentModel.Composition;

    using AO.Core;
    using AO.Core.Components;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;

    [Export(typeof(IHandleMessage))]
    public class TradeHandler : IHandleMessage<TradeMessage>
    {
        #region Public Methods and Operators

        public void Handle(object sender, Message message)
        {
            var client = (Client)sender;
            var tradeMessage = (TradeMessage)message.Body;

            var character =
                (Character)FindDynel.FindDynelById(tradeMessage.Target);
            var chaffected =
                (Character)
                FindDynel.FindDynelById(tradeMessage.Identity);

            // If target is a NPC, call its Action 0
            if ((character is NonPlayerCharacterClass) && (tradeMessage.Action == TradeAction.None))
            {
                if (((NonPlayerCharacterClass)character).KnuBot != null)
                {
                    character.KnuBotTarget = character;
                    ((NonPlayerCharacterClass)character).KnuBot.TalkingTo = chaffected;
                    ((NonPlayerCharacterClass)character).KnuBot.Action(0);
                }

                return;
            }

            var cashDeduct = 0;
            int inventoryCounter;
            InventoryEntries inventoryEntry;

            switch (tradeMessage.Action)
            {
                case TradeAction.End: // end trade
                    inventoryCounter = client.Character.Inventory.Count - 1;
                    while (inventoryCounter >= 0)
                    {
                        inventoryEntry = client.Character.Inventory[inventoryCounter];
                        AOItem aoItem;
                        if (inventoryEntry.Container == -1)
                        {
                            var nextFree = client.Character.GetNextFreeInventory(104);
                            aoItem = ItemHandler.GetItemTemplate(inventoryEntry.Item.LowID);
                            var price = aoItem.getItemAttribute(74);
                            var mult = aoItem.getItemAttribute(212); // original multiplecount
                            if (mult == 0)
                            {
                                mult = 1;
                                inventoryEntry.Item.MultipleCount = 1;
                            }

                            // Deduct Cash (ie.item.multiplecount) div mult * price
                            cashDeduct +=
                                Convert.ToInt32(
                                    mult * price
                                    * (100
                                       - Math.Floor(
                                           Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0)) / 2500);

                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            inventoryEntry.Placement = nextFree;
                            inventoryEntry.Container = 104;
                            if (!aoItem.isStackable())
                            {
                                var multiplicator = inventoryEntry.Item.MultipleCount;
                                inventoryEntry.Item.MultipleCount = 0;
                                while (multiplicator > 0)
                                {
                                    AddTemplate.Send(client, inventoryEntry);
                                    multiplicator--;
                                }
                            }
                            else
                            {
                                AddTemplate.Send(client, inventoryEntry);
                            }
                        }

                        if (inventoryEntry.Container == -2)
                        {
                            aoItem = ItemHandler.interpolate(
                                inventoryEntry.Item.LowID, inventoryEntry.Item.HighID, inventoryEntry.Item.Quality);
                            double multipleCount = aoItem.getItemAttribute(212); // original multiplecount
                            var price = aoItem.getItemAttribute(74);
                            if (multipleCount == 0.0)
                            {
                                multipleCount = 1.0;
                            }
                            else
                            {
                                multipleCount = inventoryEntry.Item.MultipleCount / multipleCount;
                            }

                            cashDeduct -=
                                Convert.ToInt32(
                                    multipleCount * price
                                    * (100
                                       + Math.Floor(
                                           Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0)) / 2500);

                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            client.Character.Inventory.Remove(inventoryEntry);
                        }

                        inventoryCounter--;
                    }

                    client.Character.Stats.Cash.Set((uint)(client.Character.Stats.Cash.Value - cashDeduct));

                    // Packets.Stat.Set(client, 61, client.Character.Stats.Cash.StatValue - cashdeduct, false);
                    var lastTrade = new Identity
                                        {
                                            Type = client.Character.LastTrade.Type, 
                                            Instance = client.Character.LastTrade.Instance
                                        };

                    var endReply = new TradeMessage
                                       {
                                           Unknown = 0x00, 
                                           Unknown1 = 0x00000001, 
                                           Action = TradeAction.Unknown, 
                                           Target = lastTrade, 
                                           Container = lastTrade
                                       };
                    client.Character.LastTrade = Identity.None;

                    client.SendCompressed(endReply);
                    break;
                case TradeAction.Decline:

                    // Decline trade
                    inventoryCounter = client.Character.Inventory.Count - 1;
                    while (inventoryCounter >= 0)
                    {
                        inventoryEntry = client.Character.Inventory[inventoryCounter];
                        if (inventoryEntry.Container == -1)
                        {
                            client.Character.Inventory.Remove(inventoryEntry);
                        }
                        else
                        {
                            if (inventoryEntry.Container == -2)
                            {
                                inventoryEntry.Placement = client.Character.GetNextFreeInventory(104);
                                inventoryEntry.Container = 104;
                            }
                        }

                        inventoryCounter--;
                    }

                    var declineReply = new TradeMessage
                                           {
                                               Unknown = tradeMessage.Unknown, 
                                               Unknown1 = tradeMessage.Unknown1, 
                                               Action = tradeMessage.Action, 
                                               Target = tradeMessage.Target, 
                                               Container = tradeMessage.Container
                                           };

                    client.SendCompressed(declineReply);
                    break;
                case TradeAction.AddItem: // add item to trade window
                case TradeAction.RemoveItem: // remove item from trade window
                    if (character.Inventory.Count == 0)
                    {
                        ((VendingMachine)character).LoadTemplate(((VendingMachine)character).TemplateId);
                    }

                    var reply = new TradeMessage
                                    {
                                        Unknown = tradeMessage.Unknown, 
                                        Unknown1 = tradeMessage.Unknown1, 
                                        Action = tradeMessage.Action, 
                                        Target = tradeMessage.Target, 
                                        Container = tradeMessage.Container
                                    };

                    client.SendCompressed(reply);

                    if (client.Character == character)
                    {
                        if (tradeMessage.Action == TradeAction.AddItem)
                        {
                            inventoryEntry = character.GetInventoryAt(tradeMessage.Container.Instance);
                            inventoryEntry.Placement = character.GetNextFreeInventory(-2);
                            inventoryEntry.Container = -2;
                        }

                        if (tradeMessage.Action == TradeAction.RemoveItem)
                        {
                            inventoryEntry = character.GetInventoryAt(tradeMessage.Container.Instance, -2);
                            inventoryEntry.Placement = character.GetNextFreeInventory(104);
                            inventoryEntry.Container = 104;
                        }
                    }
                    else
                    {
                        var inew = new InventoryEntries
                                       {
                                           Container = -1, 
                                           Placement = character.GetNextFreeInventory(-1)
                                       };
                        var oldPlacement = tradeMessage.Container.Instance;
                        var totrade = character.GetInventoryAt(oldPlacement);
                        inew.Item.LowID = totrade.Item.LowID;
                        inew.Item.HighID = totrade.Item.HighID;
                        inew.Item.MultipleCount = totrade.Item.MultipleCount;
                        if (tradeMessage.Action == TradeAction.RemoveItem)
                        {
                            // Remove item from trade window
                            inew.Item.MultipleCount = -inew.Item.MultipleCount;
                        }

                        inew.Item.Quality = totrade.Item.Quality;
                        chaffected.InventoryReplaceAdd(inew);
                    }

                    break;
            }
        }

        #endregion
    }
}