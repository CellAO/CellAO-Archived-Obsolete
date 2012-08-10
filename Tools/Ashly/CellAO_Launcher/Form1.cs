#region License
/*
Copyright (c) 2005-2009, CellAO Team

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.IO;
#endregion

namespace CellAO_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region This Area is for public stuff like Strings Ints ect...
        public string AssVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion

        //Todo: Add a Version Check and Update System for CellAO... with this launcher..
        #region This Area is for Checking if you have the latest CellAO and if not Updateing it...
        private void btn_Update_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature does not exist at this time");
        }
        #endregion

        // Todo: Add a md5 Check against the .exe files and if they fail bring up a window to tell the users
        // that they are using the wrong CellAO..
        #region This area is for Starting Each Engine and checking md5 hashes on them first...
        private void btn_StartLE_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "LoginEngine.exe";
            proc.Start();
            this.btn_StartLE.Enabled = false;
        }

        private void btn_StartZE_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ZoneEngine.exe";
            proc.Start();
            this.btn_StartZE.Enabled = false;
        }

        private void btn_StartCE_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ChatEngine.exe";
            proc.Start();
            this.btn_StartCE.Enabled = false;
        }
        #endregion

        #region This Area is for Form Loading Code...
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://www.aocell.info/", "R_Version.txt");
                StreamReader sr = File.OpenText("R_Version.txt");
                string Release_Version = sr.ReadLine();
                sr.Close();
                sr.Dispose();
                StreamReader sr2 = File.OpenText("Version.txt");
                string Local_Version = sr2.ReadLine();
                sr2.Close();
                sr2.Dispose();
                this.Text = "CellAO Launcher -  Local Version: " + Local_Version + " Remote Version: " + Release_Version;
                if (Local_Version != Release_Version)
                {
                    this.btn_Update.Visible = true;
                    
                }
                //this.Text = "CellAO Launcher - Version: " + AssVersion + " You Currently have CellAO version : Online Check System Does Not Work Yet";
                File.Delete("R_Version.txt");
            }
            catch (Exception)
            {
                this.Text = "CellAO Launcher - Version: " + AssVersion + " You Currently have CellAO version : WebSite is Down...";
            }
        }
        #endregion
       
    }
}
