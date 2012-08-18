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

    public static class WeaponItemFullUpdate
    {
        public static void SendOwner(Client client, Character character)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!! (more for each weapon, see below)
            // Packet only sent when Weapons are in Posession or being equipped the first time

            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xDF); // starter
            packetWriter.PushByte(0xDF); // starter
            packetWriter.PushShort(0xA);
            packetWriter.PushShort(0x1);
            packetWriter.PushShort(0); // Length
            packetWriter.PushInt(3086); // Sender
            packetWriter.PushInt(character.ID); // Reciever
            packetWriter.PushInt(0x3B1D2268); // Packet ID
            packetWriter.PushInt(0xC74A); // Type Weapon (always instanced)
            packetWriter.PushInt(0x45AEE789);
            // Instance of Weapon, hardcoding this causes a crash if you want to equip more than one weapon
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xB);
            packetWriter.PushInt(character.Type);
            packetWriter.PushInt(character.ID); // Reciever
            packetWriter.PushInt(0xC0A);
            packetWriter.PushInt(0xF424F); // constant
            packetWriter.PushShort(0);
            packetWriter.PushInt(0x6); // Placement/Location Maybe?
            packetWriter.PushInt(0x1F88); // Another Constant

            // Stat 1
            packetWriter.PushInt(0); // constant Flags
            packetWriter.PushUInt(0x403); // Item Flag
            // Stat 2
            packetWriter.PushInt(0x17); // constant Static Instance?
            packetWriter.PushInt(0x1e6d0); // Weapon ITEM ID
            // Stat 3
            packetWriter.PushInt(0x2BD); //constant ACG Item Level
            packetWriter.PushInt(0x5); // Quality-Level
            // Stat 4
            packetWriter.PushInt(0x2BE); // constant ACGItemTemplateID
            packetWriter.PushInt(0x1e6d0); // Weapon ITEM ID (low ID)
            // Stat 5
            packetWriter.PushInt(0x2BF); // constant ACGItemTemplateID2
            packetWriter.PushInt(0x1e6d1); // High ID
            // Stat 6
            packetWriter.PushInt(0x19C); // Constant MultipleCount
            packetWriter.PushInt(0x1); // Amount
            // Stat 7
            packetWriter.PushInt(0x1A); // Constant Energy
            packetWriter.PushUInt(0x28); // Ammo
            // End
            packetWriter.PushInt(0); // Empty fill

            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);
        }

        public static void SendPlayfield(Client client, Character character)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!!
            // Packet only sent when Weapons are in Posession!

            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xDF); // starter
            packetWriter.PushByte(0xDF); // starter
            packetWriter.PushShort(0xA);
            packetWriter.PushShort(0x1);
            packetWriter.PushShort(0); // Length
            packetWriter.PushInt(3086); // Sender
            packetWriter.PushInt(character.ID); // Reciever
            packetWriter.PushInt(0x3B1D2268); // Packet ID
            packetWriter.PushInt(0xC74A); // .... Weapon maybe?
            packetWriter.PushInt(0x45AEE789); // ID
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xB);
            packetWriter.PushInt(0xC350);
            packetWriter.PushInt(character.ID); // Reciever
            packetWriter.PushInt(0xC0A);
            packetWriter.PushInt(0xF424F); // constant
            packetWriter.PushShort(0);
            packetWriter.PushInt(0x6); // Placement/Location Maybe?
            packetWriter.PushInt(0x1F88); // Another Constant

            // Stat 1
            packetWriter.PushInt(0); // constant Flags
            packetWriter.PushUInt(0x403); // Item Flag
            // Stat 2
            packetWriter.PushInt(0x17); // constant Static Instance?
            packetWriter.PushInt(0x1e6d0); // Weapon ITEM ID
            // Stat 3
            packetWriter.PushInt(0x2BD); //constant ACG Item Level
            packetWriter.PushInt(0x5); // Quality-Level
            // Stat 4
            packetWriter.PushInt(0x2BE); // constant ACGItemTemplateID
            packetWriter.PushInt(0x1e6d0); // Weapon ITEM ID (low ID)
            // Stat 5
            packetWriter.PushInt(0x2BF); // constant ACGItemTemplateID2
            packetWriter.PushInt(0x1e6d1); // High ID
            // Stat 6
            packetWriter.PushInt(0x19C); // Constant MultipleCount
            packetWriter.PushInt(0x1); // Amount
            // Stat 7
            packetWriter.PushInt(0x1A); // Constant Energy
            packetWriter.PushUInt(0x28); // Ammo
            // End
            packetWriter.PushInt(0); // Empty fill

            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);
        }
    }
}