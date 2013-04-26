// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stat.cs" company="CellAO Team">
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
//   Set/Get clients stat
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;

    /// <summary>
    ///     Set/Get clients stat
    /// </summary>
    public static class Stat
    {
        #region Public Methods and Operators

        public static void Send(Client client, int stat, int value, bool announce)
        {
            Send(client, stat, (UInt32)value, announce);
        }

        public static void Send(Client client, int stat, uint value, bool announce)
        {
            var message = new StatMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = client.Character.Id
                                          }, 
                                  Stats =
                                      new[]
                                          {
                                              new GameTuple<CharacterStat, uint>
                                                  {
                                                      Value1 =
                                                          (CharacterStat)stat, 
                                                      Value2 = value
                                                  }
                                          }
                              };

            client.SendCompressed(0x00000C0E, client.Character.Id, message);

            /* announce to playfield? */
            if (announce)
            {
                Announce.PlayfieldOthers(client, 0x00000C0E, message);
            }
        }

        public static void SendBulk(Character ch, Dictionary<int, uint> statsToUpdate)
        {
            if (statsToUpdate.Count == 0)
            {
                return;
            }

            var toPlayfield = new List<int>();
            foreach (var keyValuePair in statsToUpdate)
            {
                if (ch.Stats.GetStatbyNumber(keyValuePair.Key).AnnounceToPlayfield)
                {
                    toPlayfield.Add(keyValuePair.Key);
                }
            }

            var stats = new List<GameTuple<CharacterStat, uint>>();
            foreach (var keyValuePair in statsToUpdate)
            {
                if (toPlayfield.Contains(keyValuePair.Key))
                {
                    stats.Add(
                        new GameTuple<CharacterStat, uint>
                            {
                                Value1 = (CharacterStat)keyValuePair.Key, 
                                Value2 = keyValuePair.Value
                            });
                }
            }

            /* announce to playfield? */
            if (toPlayfield.Any() == false)
            {
                return;
            }

            var message = new StatMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = (IdentityType)ch.Type, 
                                              Instance = ch.Id
                                          }, 
                                  Stats = stats.ToArray()
                              };

            Announce.PlayfieldOthers(ch.PlayField, 0x00000C0E, message);
        }

        public static void SendBulk(Client client, Dictionary<int, uint> statsToUpdate)
        {
            if (statsToUpdate.Count == 0)
            {
                return;
            }

            var toPlayfieldIds = new List<int>();
            foreach (var keyValuePair in statsToUpdate)
            {
                if (client.Character.Stats.GetStatbyNumber(keyValuePair.Key).AnnounceToPlayfield)
                {
                    toPlayfieldIds.Add(keyValuePair.Key);
                }
            }

            var toPlayfield = new List<GameTuple<CharacterStat, uint>>();
            var toClient = new List<GameTuple<CharacterStat, uint>>();

            foreach (var keyValuePair in statsToUpdate)
            {
                var statValue = new GameTuple<CharacterStat, uint>
                                    {
                                        Value1 = (CharacterStat)keyValuePair.Key, 
                                        Value2 = keyValuePair.Value
                                    };
                toClient.Add(statValue);

                if (toPlayfieldIds.Contains(keyValuePair.Key))
                {
                    toPlayfield.Add(statValue);
                }
            }

            var message = new StatMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = client.Character.Id
                                          }, 
                                  Stats = toClient.ToArray()
                              };

            client.SendCompressed(3086, client.Character.Id, message);

            /* announce to playfield? */
            if (toPlayfieldIds.Count > 0)
            {
                message.Stats = toPlayfield.ToArray();
                Announce.PlayfieldOthers(client, 0x00000C0E, message);
            }
        }

        /// <summary>
        /// Set own stat (no announce)
        /// </summary>
        /// <param name="client">
        /// Affected client
        /// </param>
        /// <param name="stat">
        /// Stat
        /// </param>
        /// <param name="value">
        /// Value
        /// </param>
        /// <param name="announce">
        /// Let others on same playfield know?
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint Set(Client client, int stat, uint value, bool announce)
        {
            var oldValue = (uint)client.Character.Stats.StatValueByName(stat);
            Send(client, stat, value, announce);
            return oldValue;
        }

        #endregion
    }
}