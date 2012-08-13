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

#region Usings...

#endregion

namespace ZoneEngine.Packets
{
    using AO.Core;

    internal class WeaponItemFullUpdate
    {
        public static void UpdateWeaponsCache(Character ch)
        {
            int CurrentSlot = 64;

            while (CurrentSlot < 109)
            {
                CurrentSlot++;
            }
        }

        public static void SendOwner(Client cl, Character ch)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!! (more for each weapon, see below)
            // Packet only sent when Weapons are in Posession or being equipped the first time

            PacketWriter PW = new PacketWriter();

            PW.PushByte(0xDF); // starter
            PW.PushByte(0xDF); // starter
            PW.PushShort(0xA);
            PW.PushShort(0x1);
            PW.PushShort(0); // Length
            PW.PushInt(3086); // Sender
            PW.PushInt(ch.ID); // Reciever
            PW.PushInt(0x3B1D2268); // Packet ID
            PW.PushInt(0xC74A); // Type Weapon (always instanced)
            PW.PushInt(0x45AEE789);
            // Instance of Weapon, hardcoding this causes a crash if you want to equip more than one weapon
            PW.PushByte(0);
            PW.PushInt(0xB);
            PW.PushInt(ch.Type);
            PW.PushInt(ch.ID); // Reciever
            PW.PushInt(0xC0A);
            PW.PushInt(0xF424F); // constant
            PW.PushShort(0);
            PW.PushInt(0x6); // Placement/Location Maybe?
            PW.PushInt(0x1F88); // Another Constant

            // Stat 1
            PW.PushInt(0); // constant Flags
            PW.PushUInt(0x403); // Item Flag
            // Stat 2
            PW.PushInt(0x17); // constant Static Instance?
            PW.PushInt(0x1e6d0); // Weapon ITEM ID
            // Stat 3
            PW.PushInt(0x2BD); //constant ACG Item Level
            PW.PushInt(0x5); // Quality-Level
            // Stat 4
            PW.PushInt(0x2BE); // constant ACGItemTemplateID
            PW.PushInt(0x1e6d0); // Weapon ITEM ID (low ID)
            // Stat 5
            PW.PushInt(0x2BF); // constant ACGItemTemplateID2
            PW.PushInt(0x1e6d1); // High ID
            // Stat 6
            PW.PushInt(0x19C); // Constant MultipleCount
            PW.PushInt(0x1); // Amount
            // Stat 7
            PW.PushInt(0x1A); // Constant Energy
            PW.PushUInt(0x28); // Ammo
            // End
            PW.PushInt(0); // Empty fill

            byte[] reply2 = PW.Finish();
            cl.SendCompressed(reply2);
        }

        public static void SendPlayfield(Client cl, Character ch)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!!
            // Packet only sent when Weapons are in Posession!

            PacketWriter PW = new PacketWriter();

            PW.PushByte(0xDF); // starter
            PW.PushByte(0xDF); // starter
            PW.PushShort(0xA);
            PW.PushShort(0x1);
            PW.PushShort(0); // Length
            PW.PushInt(3086); // Sender
            PW.PushInt(ch.ID); // Reciever
            PW.PushInt(0x3B1D2268); // Packet ID
            PW.PushInt(0xC74A); // .... Weapon maybe?
            PW.PushInt(0x45AEE789); // ID
            PW.PushByte(0);
            PW.PushInt(0xB);
            PW.PushInt(0xC350);
            PW.PushInt(ch.ID); // Reciever
            PW.PushInt(0xC0A);
            PW.PushInt(0xF424F); // constant
            PW.PushShort(0);
            PW.PushInt(0x6); // Placement/Location Maybe?
            PW.PushInt(0x1F88); // Another Constant

            // Stat 1
            PW.PushInt(0); // constant Flags
            PW.PushUInt(0x403); // Item Flag
            // Stat 2
            PW.PushInt(0x17); // constant Static Instance?
            PW.PushInt(0x1e6d0); // Weapon ITEM ID
            // Stat 3
            PW.PushInt(0x2BD); //constant ACG Item Level
            PW.PushInt(0x5); // Quality-Level
            // Stat 4
            PW.PushInt(0x2BE); // constant ACGItemTemplateID
            PW.PushInt(0x1e6d0); // Weapon ITEM ID (low ID)
            // Stat 5
            PW.PushInt(0x2BF); // constant ACGItemTemplateID2
            PW.PushInt(0x1e6d1); // High ID
            // Stat 6
            PW.PushInt(0x19C); // Constant MultipleCount
            PW.PushInt(0x1); // Amount
            // Stat 7
            PW.PushInt(0x1A); // Constant Energy
            PW.PushUInt(0x28); // Ammo
            // End
            PW.PushInt(0); // Empty fill

            byte[] reply = PW.Finish();
            cl.SendCompressed(reply);
        }
    }
}