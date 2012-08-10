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
using AO.Core;
using LoginEngine.QueryBase;
#endregion

namespace LoginEngine.Packets
{
    public class CheckLogin
    {
        /// <summary>
        /// 
        /// </summary>
        public int i_false;

        #region Query Setup...
        /// <summary>
        /// 
        /// </summary>
        private readonly LoginName ln = new LoginName();

        /// <summary>
        /// 
        /// </summary>
        private readonly LoginFlags lf = new LoginFlags();

        /// <summary>
        /// 
        /// </summary>
        private readonly LoginPasswd lp = new LoginPasswd();

        /// <summary>
        /// 
        /// </summary>
        private SqlWrapper ms = new SqlWrapper();
        #endregion

        #region Check To See If The Player is allowed to login (Not banned/etc)...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public bool IsLoginAllowed(Client client, string accountName)
        {
            if (accountName.ToLower() != client.AccountName.ToLower())
                return false;

            ln.GetLoginName(accountName);
            lf.GetLoginFlags(accountName);

            if (ln.LoginN != null && accountName.ToLower() == ln.LoginN.ToLower() && lf.FlagsL == i_false)
            {
                return true; // Login OK
            }
            else
            {
                return false; // Login Not Permitted
            }
        }
        #endregion

        #region Check to See if The Password matches and then send Character List...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="LoginKey"></param>
        /// <returns></returns>
        public bool IsLoginCorrect(Client client, string LoginKey)
        {
            LoginEncryption le = new LoginEncryption();

            lp.GetLoginPassword(client.AccountName);

            return le.IsValidLogin(LoginKey, client.ServerSalt, client.AccountName, lp.PasswdL);
        }
        #endregion

        #region Check to See if the Character the client is trying to use is on the account or not
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public bool IsCharacterOnAccount(Client client, int characterId)
        {
            LoginEncryption le = new LoginEncryption();

            return le.IsCharacterOnAccount(client.AccountName, (UInt32) characterId);
        }
        #endregion
    }
}