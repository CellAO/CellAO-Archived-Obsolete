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
using System.Data;
using System.Text;
using AO.Core;
using LoginEngine.QueryBase;
#endregion

namespace LoginEngine.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterListPacket
    {
        #region Query Setup
        /// <summary>
        /// 
        /// </summary>
        private readonly SqlWrapper ms = new SqlWrapper();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="accountName"></param>
        public void SendPacket(Client client, string accountName)
        {
            #region Expansions Checker
            Int32 Expansions = 0;
            Int32 AllowedCharacters = 0;
            /* This checks your expansions and
               number of characters allowed (num. of chars doesn't work)*/
            string SqlQuery = "SELECT `Expansions`,`Allowed_Characters` FROM `login` WHERE Username = '" + accountName +
                              "'";
            DataTable dt = ms.ReadDT(SqlQuery);
            if (dt.Rows.Count > 0)
            {
                Expansions = Int32.Parse((string) dt.Rows[0][0]);
                AllowedCharacters = (Int32) dt.Rows[0][1];
            }
            #endregion

            List<CharacterEntry> characters = CharacterList.LoadCharacters(accountName);
            PacketWriter pwriter = new PacketWriter();

            pwriter.PushByte(0xDF);
            pwriter.PushByte(0xDF);
            pwriter.PushShort(1); // packet type
            pwriter.PushShort(1); // ?
            pwriter.PushShort(0); // packet length (writer will take care of this)
            pwriter.PushInt(1);
            pwriter.PushInt(0x615B);
            pwriter.PushInt(0xE);
            pwriter.PushInt(characters.Count); // number of characters
            foreach (CharacterEntry character in characters)
            {
                pwriter.PushInt(4); // ?
                pwriter.PushInt(character.ID); // character ID

                // PlayfieldProxy starts
                pwriter.PushByte(0x61); // PlayfieldProxy version
                pwriter.PushIdentity(0xC79D, character.playfield);
                pwriter.PushInt(1);
                pwriter.PushInt(0);
                pwriter.PushIdentity(0, 0);
                // PlayfieldProxy ends
                // TODO: what is it?
                pwriter.PushInt(1); // ?

                // CharacterInfo starts
                pwriter.PushInt(4); // CharacterInfo version
                // if CharacterInfo version == 2
                {
                    // pwriter.PushInt(character.Breed);
                    // pwriter.PushInt(character.Gender);
                    // pwriter.PushBytes(Encoding.ASCII.GetBytes(character.Name.PadRight(0x20,char.MinValue)));
                }
                // else
                {
                    pwriter.PushInt(character.ID); // character ID
                    {
                        // if there is problem with name
                        //pwriter.PushInt(256); //(will set name to "ERROR-CHANGE-NAME")

                        // if name is ok
                        pwriter.PushInt(character.Name.Length);
                        pwriter.PushBytes(Encoding.ASCII.GetBytes(character.Name));
                    }
                    pwriter.PushInt(character.Breed);
                    pwriter.PushInt(character.Gender);
                    pwriter.PushInt(character.Profession);
                    pwriter.PushInt(character.Level);
                    {
                        // lets just leave it like that for now..
                        string areaName = "area unknown";
                        pwriter.PushInt(areaName.Length);
                        pwriter.PushBytes(Encoding.ASCII.GetBytes(areaName));
                    }
                    // TODO: What are these?
                    pwriter.PushInt(0); // ?
                    pwriter.PushInt(0); // some string (int is string length)
                    // if CharacterVersion > 3
                    {
                        // TODO: what are these 3 ints?
                        pwriter.PushInt(0); // ?
                        pwriter.PushInt(0); // ?
                        pwriter.PushInt(0); // ?
                    }
                    // CharacterInfo ends
                }
            }
            // TODO: find out what this really is
            pwriter.PushInt(AllowedCharacters); // not really allowed characters..
            pwriter.PushInt(Expansions);

            byte[] reply = pwriter.Finish();
            client.Send(ref reply);
        }
    }
}