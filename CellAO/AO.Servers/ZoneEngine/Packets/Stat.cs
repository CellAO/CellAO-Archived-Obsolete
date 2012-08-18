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

namespace ZoneEngine.Packets
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;

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
            PacketWriter packetWriter = new PacketWriter();

            uint oldValue = (uint)client.Character.Stats.StatValueByName(stat);
            client.Character.Stats.SetStatValueByName(stat, value);

            packetWriter.PushBytes(new byte[] { 0xDF, 0xDF, });
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x2B333D6E);
            packetWriter.PushIdentity(50000, client.Character.ID);
            packetWriter.PushByte(1);
            packetWriter.PushInt(1);
            packetWriter.PushInt(stat);
            packetWriter.PushUInt(value);

            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);

            /* announce to playfield? */
            if (announce)
            {
                Announce.Playfield(client.Character.PlayField, packet);
            }

            return oldValue;
        }

        public static void Send(Client client, int stat, int value, bool announce)
        {
            Send(client, stat, (UInt32)value, announce);
        }

        public static void Send(Client client, int stat, uint value, bool announce)
        {
            PacketWriter writer = new PacketWriter();
            writer.PushBytes(new byte[] { 0xDF, 0xDF, });
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

            byte[] packet = writer.Finish();
            client.SendCompressed(packet);

            /* announce to playfield? */
            if (announce)
            {
                Announce.PlayfieldOthers(client, packet);
            }
        }

        public static void SendBulk(Character ch, Dictionary<int, uint> statsToUpdate)
        {
            if (statsToUpdate.Count == 0)
            {
                return;
            }
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushBytes(new byte[] { 0xDF, 0xDF, });
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(ch.ID);
            packetWriter.PushInt(0x2B333D6E);
            packetWriter.PushIdentity(ch.Type, ch.ID);
            packetWriter.PushByte(1);

            List<int> toPlayfield = new List<int>();
            foreach (KeyValuePair<int, uint> keyValuePair in statsToUpdate)
            {
                if (ch.Stats.GetStatbyNumber(keyValuePair.Key).AnnounceToPlayfield)
                {
                    toPlayfield.Add(keyValuePair.Key);
                }
            }

            packetWriter.PushInt(toPlayfield.Count);

            foreach (KeyValuePair<int, uint> keyValuePair in statsToUpdate)
            {
                if (toPlayfield.Contains(keyValuePair.Key))
                {
                    packetWriter.PushInt(keyValuePair.Key);
                    packetWriter.PushUInt(keyValuePair.Value);
                }
            }

            /* announce to playfield? */
            if (toPlayfield.Count > 0)
            {
                byte[] packet = packetWriter.Finish();
                Announce.PlayfieldOthers(ch.PlayField, packet);
            }
        }

        public static void SendBulk(Client client, Dictionary<int, uint> statsToUpdate)
        {
            if (statsToUpdate.Count == 0)
            {
                return;
            }
            PacketWriter packetWriter = new PacketWriter();
            PacketWriter toPlayfieldWriter = new PacketWriter();
            //            client.Character.Stats.SetBaseValue(stat, value);
            packetWriter.PushBytes(new byte[] { 0xDF, 0xDF, });
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x2B333D6E);
            packetWriter.PushIdentity(50000, client.Character.ID);
            packetWriter.PushByte(1);

            toPlayfieldWriter.PushBytes(new byte[] { 0xDF, 0xDF, });
            toPlayfieldWriter.PushShort(10);
            toPlayfieldWriter.PushShort(1);
            toPlayfieldWriter.PushShort(0);
            toPlayfieldWriter.PushInt(3086);
            toPlayfieldWriter.PushInt(client.Character.ID);
            toPlayfieldWriter.PushInt(0x2B333D6E);
            toPlayfieldWriter.PushIdentity(50000, client.Character.ID);
            toPlayfieldWriter.PushByte(1);

            List<int> toPlayfieldIds = new List<int>();
            foreach (KeyValuePair<int, uint> keyValuePair in statsToUpdate)
            {
                if (client.Character.Stats.GetStatbyNumber(keyValuePair.Key).AnnounceToPlayfield)
                {
                    toPlayfieldIds.Add(keyValuePair.Key);
                }
            }

            packetWriter.PushInt(statsToUpdate.Count);
            toPlayfieldWriter.PushInt(toPlayfieldIds.Count);

            foreach (KeyValuePair<int, uint> keyValuePair in statsToUpdate)
            {
                packetWriter.PushInt(keyValuePair.Key);
                packetWriter.PushUInt(keyValuePair.Value);
                if (toPlayfieldIds.Contains(keyValuePair.Key))
                {
                    toPlayfieldWriter.PushInt(keyValuePair.Key);
                    toPlayfieldWriter.PushUInt(keyValuePair.Value);
                }
            }

            byte[] reply = packetWriter.Finish();
            client.SendCompressed(reply);

            /* announce to playfield? */
            if (toPlayfieldIds.Count > 0)
            {
                byte[] replyToPlayfield = toPlayfieldWriter.Finish();
                Announce.PlayfieldOthers(client, replyToPlayfield);
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