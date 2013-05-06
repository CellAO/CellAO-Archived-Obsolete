#region License
/*
Copyright (c) 2005-2011, CellAO Team

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Collections;


namespace CreateStatelEvents
{
    using MS.Internal.Xml.XPath;

    public partial class MainWindow : Form
    {
        public static string strBefore(string s, string before)
        {
            return s.Substring(0, s.IndexOf(before));
        }

        public static string strAfter(string s, string after)
        {
            return s.Substring(s.IndexOf(after) + after.Length, s.Length - s.IndexOf(after) - after.Length);
        }

        public List<StatelFunction> functions = new List<StatelFunction>();
        public List<StatelEvent> events = new List<StatelEvent>();
        public List<StatelRequirement> reqs = new List<StatelRequirement>();
        public List<StatelArgument> args = new List<StatelArgument>();
        public int selectedstatel;
        public int selectedevent;
        public int selectedargument;
        public int selectedrequirement;
        public int selectedfunction;

        public StatelEvent evdrag;
        public StatelFunction funcdrag;
        public StatelArgument argdrag;
        public StatelRequirement reqdrag;

        public ListBox dragsource = null;

        public Point lastmousedown = new Point();

        public static Config.ConfigData cfg;

        public MainWindow()
        {
            InitializeComponent();
            cfg = Config.GetConfigData();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(PlayfieldBox, "Select Playfield");
            toolTip1.SetToolTip(StatelBox, "Select Statel");
            toolTip1.SetToolTip(ArgumentBox, "Double-Click argument to edit");
            toolTip1.SetToolTip(FunctionBox, "Double-Click argument to edit");

            DirectoryInfo di = new DirectoryInfo(cfg.statel_dir);
            SortedList sl = new SortedList();
            foreach (FileInfo fi in di.GetFiles())
            {
                string sho = fi.Name;
                sho = sho.Substring(0, sho.LastIndexOf('.'));
                int sel = Int32.Parse(sho);
                sl.Add(sel.ToString().PadLeft(4, ' '), sel.ToString());
            }

            for (int i = 0; i < sl.Count; i++)
            {
                PlayfieldBox.Items.Add(sl.GetByIndex(i));
            }
            List<string> temp = new List<String>();

            temp.Add("Stat Number");
            temp.Add("Min");
            temp.Add("Max");
            Argstext.Add(53002, temp);

            temp = new List<String>();
            temp.Add("X (Integer)");
            temp.Add("Y (Integer)");
            temp.Add("Z (Integer)");
            temp.Add("Playfield");
            Argstext.Add(53016, temp);

            NamesandNumbers.set();

        }

        public Dictionary<int, List<string>> Argstext = new Dictionary<int, List<string>>();


        private List<Int32> ReadOffsets(FileStream fs)
        {
            BinaryReader br = new BinaryReader(fs);
            br.ReadInt32();
            List<Int32> temp = new List<Int32>();
            int lastoffset = 0;
            int offset = 0;
            while (lastoffset >= offset)
            {
                lastoffset = offset;
                offset = br.ReadInt32();
                temp.Add(offset);
            }
            return temp;
        }

        private void PlayfieldBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string s1 = PlayfieldBox.SelectedItem.ToString().TrimStart(' ');

            FileStream fs;
            StatelBox.Items.Clear();
            EventBox.Items.Clear();
            FunctionBox.Items.Clear();
            /*if (File.Exists(cfg.ao_dir + "\\cd_image\\data\\statels\\" + s1.Trim() + ".pf"))
            {
                fs = new FileStream(cfg.ao_dir + "\\cd_image\\data\\statels\\" + s1.Trim() + ".pf", FileMode.Open);
                List<Int32> offsets = ReadOffsets(fs);
                BinaryReader b = new BinaryReader(fs);
                foreach (Int32 ofs in offsets)
                {
                    long fseek = ofs;
                    fs.Seek(ofs + 3, SeekOrigin.Begin);

                    int len = b.ReadInt32();
                    int type = b.ReadInt32();
                    uint instance = b.ReadUInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    int template = b.ReadInt32();
                    string itemname = "";
                    try
                    {
                        FileStream fs2 = new FileStream(cfg.item_dir + @"\" + template.ToString() + ".rdbdata", FileMode.Open);
                        BinaryReader r = new BinaryReader(fs2);
                        fs2.Seek(0x18, SeekOrigin.Begin);
                        while (r.ReadInt32() != 0x21)
                        {
                            r.ReadInt32();
                        }
                        int sho1 = r.ReadInt16();
                        r.ReadInt16();
                        itemname = Encoding.ASCII.GetString(r.ReadBytes(sho1));
                        r.Close();
                        fs2.Close();
                    }
                    catch
                    {
                        itemname = "";
                    }

                    fs.Seek(fseek + len + 4, SeekOrigin.Begin);
                    
                    if (itemname != "")
                    {
                        StatelBox.Items.Add(type.ToString() + ":" + instance.ToString().PadLeft(12, '0') + " " + itemname);
                    }
                }

            }
            else*/
            {
                fs = new FileStream(cfg.statel_dir + @"\" + s1 + ".rdbdata", FileMode.Open);
                BinaryReader b = new BinaryReader(fs);
                int count = b.ReadInt32();
                while (count > 0)
                {
                    long fseek = fs.Position;
                    int len = b.ReadInt32();
                    int type = b.ReadInt32();
                    uint instance = b.ReadUInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    b.ReadInt32();
                    int template = b.ReadInt32();
                    string itemname = "";
                    try
                    {
                        FileStream fs2 = new FileStream(cfg.item_dir + @"\" + template.ToString() + ".rdbdata", FileMode.Open);
                        BinaryReader r = new BinaryReader(fs2);
                        fs2.Seek(0x18, SeekOrigin.Begin);
                        while (r.ReadInt32() != 0x21)
                        {
                            r.ReadInt32();
                        }
                        int sho1 = r.ReadInt16();
                        r.ReadInt16();
                        itemname = Encoding.ASCII.GetString(r.ReadBytes(sho1));
                        r.Close();
                        fs2.Close();
                    }
                    catch
                    {
                        itemname = "";
                    }

                    fs.Seek(fseek + len + 4, SeekOrigin.Begin);
                    count--;
                    if (itemname != "")
                    {
                        StatelBox.Items.Add(type.ToString() + ":" + instance.ToString().PadLeft(12, '0') + " " + itemname);
                    }
                }
                b.Close();
            }


            fs.Close();




            this.Cursor = Cursors.Default;
        }

        public void ChangeStatelSelection()
        {
            this.Cursor = Cursors.WaitCursor;
            DataSet ds = common.readSQL("SELECT * FROM statels where instance=" + strBefore(strAfter(StatelBox.SelectedItem.ToString(), ":"), " ") + " AND type=" + strBefore(StatelBox.SelectedItem.ToString(), ":") + " ORDER BY id ASC");
            DataTable dt = ds.Tables[0];
            EventBox.Items.Clear();
            FunctionBox.Items.Clear();
            ArgumentBox.Items.Clear();
            args.Clear();
            RequirementBox.Items.Clear();
            reqs.Clear();
            if (dt.Rows.Count > 0)
            {
                selectedstatel = (Int32)dt.Rows[0]["id"];
                foreach (DataRow dr in dt.Rows)
                {
                    DataSet ds2 = common.readSQL("SELECT * from statel_events where statel_id=" + dr["id"].ToString() + " ORDER BY eventid, statel_id ASC");
                    DataTable dt2 = ds2.Tables[0];
                    events.Clear();
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        StatelEvent f = new StatelEvent();
                        f.eventid = (Int32)dr2[0];
                        f.eventnum = (Int32)dr2[2];
                        events.Add(f);
                        EventBox.Items.Add(NamesandNumbers.Events[Int32.Parse(dr2[2].ToString())] + " (" + Int32.Parse(dr2[2].ToString()) + ")");
                    }
                    if (EventBox.Items.Count > 0)
                    {
                        EventBox.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                selectedstatel = -1;
            }
            this.Cursor = Cursors.Default;
        }

        private void StatelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeStatelSelection();
        }

        private void EventBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeEventSelection();
        }

        public void ChangeEventSelection()
        {
            if (EventBox.SelectedIndex != -1)
            {
                this.Cursor = Cursors.WaitCursor;
                FunctionBox.Items.Clear();
                selectedevent = events.ElementAt(EventBox.SelectedIndex).eventid;
                DataSet ds = common.readSQL("SELECT * FROM statel_functions s WHERE event_id=" + selectedevent + " AND statel_id=" + selectedstatel + " ORDER BY functionid, event_id ASC");
                DataTable dt = ds.Tables[0];
                functions.Clear();
                args.Clear();
                ArgumentBox.Items.Clear();
                reqs.Clear();
                RequirementBox.Items.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    StatelFunction f = new StatelFunction();
                    f.functionid = (Int32)r[0];
                    f.functionnum = (Int32)r[3];
                    f.target = (Int32)r[4];
                    f.tickcount = (Int32)r[5];
                    f.tickinterval = (Int32)r[6];
                    functions.Add(f);
                    FunctionBox.Items.Add(f.ToString());
                }
                if (FunctionBox.Items.Count > 0)
                {
                    FunctionBox.SelectedIndex = 0;
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void FunctionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeFunctionSelection();
            FunctionUp.Enabled = FunctionBox.SelectedIndex > 0;
            FunctionDown.Enabled = (FunctionBox.Items.Count - FunctionBox.SelectedIndex) > 1;
        }

        public void ChangeFunctionSelection()
        {
            if (FunctionBox.SelectedIndex != -1)
            {
                this.Cursor = Cursors.WaitCursor;
                selectedfunction = functions.ElementAt(FunctionBox.SelectedIndex).functionid;
                DataSet ds = common.readSQL("SELECT * FROM statel_function_arguments where function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel + " ORDER BY attrid, function_id ASC");

                ArgumentBox.Items.Clear();
                args.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StatelArgument a = new StatelArgument();
                    a.attributeid = (Int32)dr[0];
                    a.value = (string)dr[4];
                    args.Add(a);
                    ArgumentBox.Items.Add(a.value);
                }

                RequirementBox.Items.Clear();
                reqs.Clear();
                ds = common.readSQL("SELECT * FROM statel_function_reqs where function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel + " ORDER BY reqid, function_id ASC");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StatelRequirement r = new StatelRequirement();
                    r.attribute = (Int32)dr[4];
                    r.childop = (Int32)dr[7];
                    r.op = (Int32)dr[6];
                    r.target = (Int32)dr[8];
                    r.value = (Int32)dr[5];
                    r.reqid = (Int32)dr[0];
                    reqs.Add(r);
                    RequirementBox.Items.Add(r.ToString());
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void but_RemoveEvent_Click(object sender, EventArgs e)
        {
            if (EventBox.SelectedIndex == -1)
            {
                return;
            }
            if (MessageBox.Show("Delete this event and all of its functions?", "Delete event?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StatelEvent te = events.ElementAt(EventBox.SelectedIndex);
                foreach (StatelFunction f in functions)
                {
                    common.execSQL("DELETE FROM statel_function_reqs WHERE function_id=" + f.functionid);
                    common.execSQL("DELETE FROM statel_function_arguments WHERE function_id=" + f.functionid);
                }
                common.execSQL("DELETE FROM statel_functions WHERE event_id=" + te.eventid);
                common.execSQL("DELETE FROM statel_events WHERE eventid=" + te.eventid);
                ChangeStatelSelection();
            }
        }

        private void but_RemoveFunction_Click(object sender, EventArgs e)
        {
            if (FunctionBox.SelectedIndex == -1)
            {
                return;
            }
            if (MessageBox.Show("Delete this function and all of its arguments/requirements?", "Delete function?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StatelFunction sf = functions.ElementAt(FunctionBox.SelectedIndex);
                common.execSQL("DELETE FROM statel_function_reqs WHERE function_id=" + sf.functionid);
                common.execSQL("DELETE FROM statel_function_arguments WHERE function_id=" + sf.functionid);
                common.execSQL("DELETE FROM statel_functions WHERE functionid=" + sf.functionid);
                ChangeEventSelection();
            }
        }

        private void but_RemoveArgument_Click(object sender, EventArgs e)
        {
            if (ArgumentBox.SelectedIndex == -1)
            {
                return;
            }
            if (MessageBox.Show("Delete this argument?", "Delete argument?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                common.execSQL("DELETE FROM statel_function_arguments WHERE attrid=" + args.ElementAt(ArgumentBox.SelectedIndex).attributeid);
                ChangeFunctionSelection();
            }
        }

        private void but_RemoveRequirement_Click(object sender, EventArgs e)
        {
            if (ArgumentBox.SelectedIndex == -1)
            {
                return;
            }
            if (MessageBox.Show("Delete this requirement?", "Delete requirement?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                common.execSQL("DELETE FROM statel_function_reqs WHERE reqid=" + reqs.ElementAt(RequirementBox.SelectedIndex).reqid);
                ChangeFunctionSelection();
            }
        }

        private void ArgumentBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ArgumentBox.SelectedIndex != -1)
            {
                selectedargument = args.ElementAt(ArgumentBox.SelectedIndex).attributeid;
            }
        }

        private void RequirementBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RequirementBox.SelectedIndex != -1)
            {
                selectedrequirement = reqs.ElementAt(RequirementBox.SelectedIndex).reqid;
            }
        }

        private void EventBox_DragDrop(object sender, DragEventArgs e)
        {
            if (dragsource != sender)
            {
                ChangeStatelSelection();
                return;
            }
            Point p = new Point(e.X, e.Y);
            p = ((ListBox)sender).PointToClient(p);

            int indexOfItem = EventBox.IndexFromPoint(p.X, p.Y);
            if ((indexOfItem >= 0) && (indexOfItem < EventBox.Items.Count))
            {
                StatelEvent se = events.ElementAt(indexOfItem);
                se.changeid(selectedstatel, evdrag.eventid);
                ChangeStatelSelection();
            }
            Setline(null, false, null);
        }

        private void EventBox_DragEnter(object sender, DragEventArgs e)
        {
            if ((ListBox)sender == EventBox)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void EventBox_DragOver(object sender, DragEventArgs e)
        {
            if ((ListBox)sender == EventBox)
            {
                e.Effect = DragDropEffects.Move;
            }
            Setline((ListBox)sender, true, e);
        }

        private void Setline(ListBox target, bool visible, DragEventArgs e)
        {
            panel1.Visible = visible;
            if (target == null)
            {
                return;
            }

            Point p = new Point(e.X, e.Y);
            p = (target).PointToClient(p);

            int indexOfItem = EventBox.IndexFromPoint(p.X, p.Y);
            int y = target.PointToClient(p).Y;

            int x1 = target.Left;
            int x2 = target.Width;
            int ih = target.ItemHeight;

            int y1 = (int)Math.Min(Math.Round((double)(y / ih)), target.Items.Count) * ih + target.Top;
            panel1.Left = x1;
            panel1.Top = y1;
            panel1.Width = x2;
            panel1.Height = 2;
        }

        private void PlayfieldBox_DrawItem(object sender, DrawItemEventArgs e)
        {

            e.DrawBackground();
            // Draw the current item text based on the current Font and the custom brush settings.
            if (((ComboBox)sender).Items.Count > 0)
            {
                SizeF stringSize = new SizeF();
                stringSize = e.Graphics.MeasureString(((ComboBox)sender).Items[e.Index].ToString(), e.Font);

                Brush myBrush = SystemBrushes.ControlText;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    myBrush = SystemBrushes.HighlightText;
                }
                e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), e.Font, myBrush, new PointF(e.Bounds.Left, e.Bounds.Y));
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.

            e.DrawFocusRectangle();
        }


        private void FunctionUp_Click(object sender, EventArgs e)
        {
            // Swap Functions
            common.execSQL("UPDATE statel_functions SET functionid=-1 WHERE functionid=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_functions SET functionid=" + selectedfunction + " WHERE functionid=" + (selectedfunction - 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_functions SET functionid=" + (selectedfunction - 1).ToString() + " WHERE functionid=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            // Swap Requirements
            common.execSQL("UPDATE statel_function_reqs SET function_id=-1 WHERE function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_reqs SET function_id=" + selectedfunction + " WHERE function_id=" + (selectedfunction - 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_reqs SET function_id=" + (selectedfunction - 1).ToString() + " WHERE function_id=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            // Swap Requirements
            common.execSQL("UPDATE statel_function_arguments SET function_id=-1 WHERE function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_arguments SET function_id=" + selectedfunction + " WHERE function_id=" + (selectedfunction - 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_arguments SET function_id=" + (selectedfunction - 1).ToString() + " WHERE function_id=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            ChangeStatelSelection();
        }

        private void FunctionDown_Click(object sender, EventArgs e)
        {
            // Swap Functions
            common.execSQL("UPDATE statel_functions SET functionid=-1 WHERE functionid=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_functions SET functionid=" + selectedfunction + " WHERE functionid=" + (selectedfunction - 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_functions SET functionid=" + (selectedfunction - 1).ToString() + " WHERE functionid=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            // Swap Requirements
            common.execSQL("UPDATE statel_function_reqs SET function_id=-1 WHERE function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_reqs SET function_id=" + selectedfunction + " WHERE function_id=" + (selectedfunction + 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_reqs SET function_id=" + (selectedfunction + 1).ToString() + " WHERE function_id=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            // Swap Requirements
            common.execSQL("UPDATE statel_function_arguments SET function_id=-1 WHERE function_id=" + selectedfunction + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_arguments SET function_id=" + selectedfunction + " WHERE function_id=" + (selectedfunction + 1).ToString() + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
            common.execSQL("UPDATE statel_function_arguments SET function_id=" + (selectedfunction + 1).ToString() + " WHERE function_id=-1 AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);

            ChangeStatelSelection();

        }

        private void EventBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectEvent se = new SelectEvent();
            se.ShowDialog();
            if (se.DialogResult == DialogResult.Cancel)
            {
                return;
            }
            common.execSQL("UPDATE statel_events SET eventnum=" + se.returnvalue + " WHERE eventid=" + selectedevent + " AND statel_id=" + selectedstatel);
            ChangeStatelSelection();
        }

        private void but_AddEvent_Click(object sender, EventArgs e)
        {
            SelectEvent se = new SelectEvent();
            se.ShowDialog();
            if (se.DialogResult == DialogResult.Cancel)
            {
                return;
            }

            DataSet ds = common.readSQL("SELECT count(*) from statel_events where statel_id=" + selectedstatel);
            int nexteventid = 1;
            if ((Int32)ds.Tables[0].Rows[0][0] > 0)
            {
                ds = common.readSQL("SELECT eventid from statel_events WHERE statel_id=" + selectedstatel + " ORDER BY eventid DESC");
                nexteventid = ((Int32)ds.Tables[0].Rows[0][0]) + 1;
            }
            common.execSQL("INSERT INTO statel_events VALUES (" + nexteventid + "," + selectedstatel + "," + se.returnvalue + ")");
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences p = new Preferences();
            p.set();
            p.ShowDialog();
            if (p.DialogResult == DialogResult.OK)
            {
                cfg.ao_dir = p.ao_dir;
                cfg.connectstring = p.connectstring;
                cfg.item_dir = p.item_dir;
                cfg.statel_dir = p.statel_dir;
                Config.SaveConfigData(cfg);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(cfg.ao_dir + "\\cd_image\\data\\db\\ResourceDatabase.idx"))
            {
                MessageBox.Show("Path to Anarchy Online is wrong, please check your preferences.");
                return;
            }

            Extractor extractor = new Extractor(cfg.ao_dir + "\\cd_image\\data\\db\\");

            progressBar1.Step = 1;
            panel2.Visible = true;
            panel2.Update();

            foreach (int recType in extractor.GetRecordTypes())
            {
                if ((recType == 1000026) || (recType==1000020))
                {
                    progressBar1.Maximum = extractor.GetRecordInstances(recType).Length;
                    progressBar1.Value = 0;
                    foreach (int recInstance in extractor.GetRecordInstances(recType))
                    {
                        byte[] temp = extractor.GetRecordData(recType, recInstance);
                        if (!Directory.Exists(cfg.item_dir + "\\" + recType))
                        {
                            Directory.CreateDirectory(cfg.item_dir + "\\" + recType);
                        }
                        FileStream fs =
                            new FileStream(
                                cfg.item_dir + "\\" + recType.ToString() + "\\" + recInstance.ToString() + ".rdbdata",
                                FileMode.Create,
                                FileAccess.Write);
                        fs.Write(temp, 0, temp.Length);
                        fs.Close();
                        progressBar1.PerformStep();
                    }
                }
            }
            panel2.Visible = false;
        }

        private void but_AddFunction_Click(object sender, EventArgs e)
        {
            SelectFunction sf = new SelectFunction();
            sf.set();
        }

        private void but_AddRequirement_Click(object sender, EventArgs e)
        {
        }

        private void FunctionBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FunctionBox.SelectedIndex != -1)
            {
                StatelFunction oldfunc = functions.ElementAt(FunctionBox.SelectedIndex);
                SelectFunction sf = new SelectFunction();
                sf.getdata(oldfunc);
                sf.ShowDialog();
                if (sf.DialogResult == DialogResult.OK)
                {
                    common.execSQL("REPLACE INTO statel_functions VALUES (" + oldfunc.functionid + "," + selectedevent + "," + selectedstatel + "," + sf.functionnum + "," + sf.target + "," + sf.tickcount + "," + sf.tickinterval + ")");
                    int oldindex = FunctionBox.SelectedIndex;
                    StatelBox_SelectedIndexChanged(null, null);
                    FunctionBox.SelectedIndex = oldindex;
                    if (sf.functionnum != oldfunc.functionnum)
                    {
                        common.execSQL("DELETE FROM statel_function_arguments WHERE function_id=" + oldfunc.functionid + " AND event_id=" + selectedevent + " AND statel_id=" + selectedstatel);
                        ArgumentBox_MouseDoubleClick(null, null);
                    }
                }
            }
        }

        private void ArgumentBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            int funcnum = Int32.Parse(strBefore(strAfter(FunctionBox.SelectedItem.ToString(), "("), ")"));
            if (NamesandNumbers.Functiontemplates.ContainsKey(funcnum))
            {
                DynamicEdit de = new DynamicEdit();
                FunctionTemplates ft = NamesandNumbers.Functiontemplates[funcnum];
                PlayfieldValue dummypf = new PlayfieldValue();
                LineValue dummyline = new LineValue();
                int argnum = 0;
                foreach (FunctionTemplate tmpl in ft.Templates)
                {
                    if (ArgumentBox.Items.Count > argnum)
                    {
                        tmpl.baseValue = ArgumentBox.Items[argnum++].ToString();
                    }
                }
                de.createDynels(ft);
                de.Controls["button_cancel"].Enabled = sender != null;
                de.ShowDialog();

                if (de.DialogResult == DialogResult.OK)
                {
                    ArgumentBox.Items.Clear();
                    argnum = 0;
                    common.execSQL("DELETE FROM statel_function_arguments WHERE statel_id=" + selectedstatel + " AND event_id=" + selectedevent + " AND function_id=" + selectedfunction);
                    foreach (FunctionTemplate tmpl in de.template.Templates)
                    {
                        ArgumentBox.Items.Add(tmpl.baseValue);
                        common.execSQL("INSERT INTO statel_function_arguments VALUES (" + argnum + "," + selectedfunction + "," + selectedevent + "," + selectedstatel + ",'" + tmpl.baseValue + "')");
                        argnum++;
                    }
                }
            }
            else
            {
                MessageBox.Show("Function not implemented yet");
            }
        }
    }
}
