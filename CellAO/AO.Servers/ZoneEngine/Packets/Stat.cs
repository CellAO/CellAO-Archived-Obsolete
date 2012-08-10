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
using System.Collections.Generic;
using AO.Core;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine.Packets
{
    /// <summary>
    /// Set/Get clients stat
    /// </summary>
    public static class Stat
    {
        /// <summary>
        /// Set own stat (no announce)
        /// </summary>
        /// <param name="client">Affected client</param>
        /// <param name="stat">Stat</param>
        /// <param name="value">Value</param>
        /// <param name="announce">Let others on same playfield know?</param>
        public static uint Set(Client client, int stat, uint value, bool announce)
        {
            PacketWriter writer = new PacketWriter();

            uint oldValue = (uint) client.Character.Stats.Get(stat);
            client.Character.Stats.Set(stat, value);

            writer.PushBytes(new byte[] {0xDF, 0xDF,});
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(client.Character.ID);
            writer.PushInt(0x2B333D6E);
            writer.PushIdentity(50000, client.Character.ID);
            writer.PushByte(1);
            writer.PushInt(1);
            writer.PushInt(stat);
            writer.PushUInt(value);

            byte[] reply = writer.Finish();
            client.SendCompressed(reply);

            /* announce to playfield? */
            if (announce)
            {
                Announce.Playfield(client.Character.PlayField, ref reply);
            }

            return oldValue;
        }

        public static void Send(Client client, int stat, int value, bool announce)
        {
            Send(client, stat, (UInt32) value, announce);
        }

        public static void Send(Client client, int stat, uint value, bool announce)
        {
            PacketWriter writer = new PacketWriter();
            writer.PushBytes(new byte[] {0xDF, 0xDF,});
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(client.Character.ID);
            writer.PushInt(0x2B333D6E);
            writer.PushIdentity(50000, client.Character.ID);
            writer.PushByte(1);
            writer.PushInt(1);
            writer.PushInt(stat);
            writer.PushUInt(value);

            byte[] reply = writer.Finish();
            client.SendCompressed(reply);

            /* announce to playfield? */
            if (announce)
            {
                Announce.PlayfieldOthers(client, ref reply);
            }
        }

        public static void SendBulk(Character ch, Dictionary<int, uint> StatsToUpdate)
        {
            if (StatsToUpdate.Count == 0)
                return;
            PacketWriter toplayfield = new PacketWriter();

            toplayfield.PushBytes(new byte[] {0xDF, 0xDF,});
            toplayfield.PushShort(10);
            toplayfield.PushShort(1);
            toplayfield.PushShort(0);
            toplayfield.PushInt(3086);
            toplayfield.PushInt(ch.ID);
            toplayfield.PushInt(0x2B333D6E);
            toplayfield.PushIdentity(ch.Type, ch.ID);
            toplayfield.PushByte(1);

            List<int> topf = new List<int>();
            foreach (KeyValuePair<int, uint> kv in StatsToUpdate)
            {
                if (ch.Stats.GetStatbyNumber(kv.Key).AnnounceToPlayfield)
                {
                    topf.Add(kv.Key);
                }
            }

            toplayfield.PushInt(topf.Count);

            foreach (KeyValuePair<int, uint> kv in StatsToUpdate)
            {
                if (topf.Contains(kv.Key))
                {
                    toplayfield.PushInt(kv.Key);
                    toplayfield.PushUInt(kv.Value);
                }
            }

            /* announce to playfield? */
            if (topf.Count > 0)
            {
                byte[] replytopf = toplayfield.Finish();
                Announce.PlayfieldOthers(ch.PlayField, ref replytopf);
            }
        }


        public static void SendBulk(Client client, Dictionary<int, uint> StatsToUpdate)
        {
            if (StatsToUpdate.Count == 0)
                return;
            PacketWriter writer = new PacketWriter();
            PacketWriter toplayfield = new PacketWriter();
            //            client.Character.Stats.SetBaseValue(stat, value);
            writer.PushBytes(new byte[] {0xDF, 0xDF,});
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(client.Character.ID);
            writer.PushInt(0x2B333D6E);
            writer.PushIdentity(50000, client.Character.ID);
            writer.PushByte(1);

            toplayfield.PushBytes(new byte[] {0xDF, 0xDF,});
            toplayfield.PushShort(10);
            toplayfield.PushShort(1);
            toplayfield.PushShort(0);
            toplayfield.PushInt(3086);
            toplayfield.PushInt(client.Character.ID);
            toplayfield.PushInt(0x2B333D6E);
            toplayfield.PushIdentity(50000, client.Character.ID);
            toplayfield.PushByte(1);

            List<int> topf = new List<int>();
            foreach (KeyValuePair<int, uint> kv in StatsToUpdate)
            {
                if (client.Character.Stats.GetStatbyNumber(kv.Key).AnnounceToPlayfield)
                {
                    topf.Add(kv.Key);
                }
            }

            writer.PushInt(StatsToUpdate.Count);
            toplayfield.PushInt(topf.Count);

            foreach (KeyValuePair<int, uint> kv in StatsToUpdate)
            {
                writer.PushInt(kv.Key);
                writer.PushUInt(kv.Value);
                if (topf.Contains(kv.Key))
                {
                    toplayfield.PushInt(kv.Key);
                    toplayfield.PushUInt(kv.Value);
                }
            }

            byte[] reply = writer.Finish();
            client.SendCompressed(reply);

            /* announce to playfield? */
            if (topf.Count > 0)
            {
                byte[] replytopf = toplayfield.Finish();
                Announce.PlayfieldOthers(client, ref replytopf);
            }
        }

        /*
        /// <summary>
        /// Set stat of target (announce)
        /// </summary>
        /// <param name="client">Client that used /set</param>
        /// <param name="stat">Stat</param>
        /// <param name="value">Value</param>
        /// <param name="targettype">Target type</param>
        /// <param name="targetinstance">Target instance</param>
        public static int Set(int stat, uint value, int targettype, int targetinstance)
        {
            PacketWriter writer = new PacketWriter();

            uint oldValue = 1234567890;
            Dynel dyn = null;

            if (targettype == 50000)
            {

                dyn = Misc.FindDynel.FindDynelByID(50000, targetinstance);
                if (dyn == null)
                {
                    // TODO: ErrorHandling
                    return -1;
                }

                // Is dyn a Character or subclass?
                if (dyn is Character)
                {
                    Character ch = (Character)dyn;
                    oldValue = ch.Stats.Cash.StatValue;
                    ch.Stats.Cash.Set(value);
                }
            }

            writer.PushBytes(new byte[] { 0xDF, 0xDF, });
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(0);  // Announcer fills this
            writer.PushInt(0x2B333D6E);
            writer.PushIdentity(targettype, targetinstance);
            writer.PushByte(1);
            writer.PushInt(1);
            writer.PushInt(stat);
            writer.PushUInt(value);

            byte[] reply = writer.Finish();
            if (dyn != null)
            {
                Misc.Announce.Playfield(dyn.PlayField, ref reply);
            }

            return oldValue;
        }
        */
    }
}

/*
            byte[] tmp_array = new byte[]
            {
            };
            client.CmpStream.Write(tmp_array, 0, tmp_array.Length);
            client.CmpStream.Flush();
*/