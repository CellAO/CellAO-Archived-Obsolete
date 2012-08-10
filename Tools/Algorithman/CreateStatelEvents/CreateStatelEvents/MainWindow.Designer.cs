namespace CreateStatelEvents
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.EventBox = new System.Windows.Forms.ListBox();
            this.FunctionBox = new System.Windows.Forms.ListBox();
            this.l_Functions = new System.Windows.Forms.Label();
            this.ArgumentBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RequirementBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.but_AddArgument = new System.Windows.Forms.Button();
            this.but_RemoveArgument = new System.Windows.Forms.Button();
            this.but_RemoveRequirement = new System.Windows.Forms.Button();
            this.but_AddRequirement = new System.Windows.Forms.Button();
            this.but_RemoveFunction = new System.Windows.Forms.Button();
            this.but_AddFunction = new System.Windows.Forms.Button();
            this.but_RemoveEvent = new System.Windows.Forms.Button();
            this.but_AddEvent = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FunctionDown = new System.Windows.Forms.Button();
            this.FunctionUp = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.PlayfieldBox = new System.Windows.Forms.ListBox();
            this.StatelBox = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Playfield";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Statel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 206);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Events";
            // 
            // EventBox
            // 
            this.EventBox.AllowDrop = true;
            this.EventBox.FormattingEnabled = true;
            this.EventBox.Location = new System.Drawing.Point(12, 222);
            this.EventBox.Name = "EventBox";
            this.EventBox.Size = new System.Drawing.Size(120, 121);
            this.EventBox.TabIndex = 13;
            this.EventBox.SelectedIndexChanged += new System.EventHandler(this.EventBox_SelectedIndexChanged);
            this.EventBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.EventBox_MouseDoubleClick);
            // 
            // FunctionBox
            // 
            this.FunctionBox.FormattingEnabled = true;
            this.FunctionBox.Location = new System.Drawing.Point(173, 222);
            this.FunctionBox.Name = "FunctionBox";
            this.FunctionBox.Size = new System.Drawing.Size(599, 121);
            this.FunctionBox.TabIndex = 14;
            this.FunctionBox.SelectedIndexChanged += new System.EventHandler(this.FunctionBox_SelectedIndexChanged);
            this.FunctionBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FunctionBox_MouseDoubleClick);
            // 
            // l_Functions
            // 
            this.l_Functions.AutoSize = true;
            this.l_Functions.Location = new System.Drawing.Point(170, 206);
            this.l_Functions.Name = "l_Functions";
            this.l_Functions.Size = new System.Drawing.Size(53, 13);
            this.l_Functions.TabIndex = 15;
            this.l_Functions.Text = "Functions";
            // 
            // ArgumentBox
            // 
            this.ArgumentBox.FormattingEnabled = true;
            this.ArgumentBox.Location = new System.Drawing.Point(12, 378);
            this.ArgumentBox.Name = "ArgumentBox";
            this.ArgumentBox.Size = new System.Drawing.Size(120, 134);
            this.ArgumentBox.TabIndex = 16;
            this.ArgumentBox.SelectedIndexChanged += new System.EventHandler(this.ArgumentBox_SelectedIndexChanged);
            this.ArgumentBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ArgumentBox_MouseDoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 362);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Arguments (DblClick)";
            // 
            // RequirementBox
            // 
            this.RequirementBox.FormattingEnabled = true;
            this.RequirementBox.Location = new System.Drawing.Point(173, 378);
            this.RequirementBox.Name = "RequirementBox";
            this.RequirementBox.Size = new System.Drawing.Size(599, 134);
            this.RequirementBox.TabIndex = 18;
            this.RequirementBox.SelectedIndexChanged += new System.EventHandler(this.RequirementBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(170, 362);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Requirements";
            // 
            // but_AddArgument
            // 
            this.but_AddArgument.Enabled = false;
            this.but_AddArgument.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_AddArgument.Location = new System.Drawing.Point(138, 378);
            this.but_AddArgument.Name = "but_AddArgument";
            this.but_AddArgument.Size = new System.Drawing.Size(20, 24);
            this.but_AddArgument.TabIndex = 20;
            this.but_AddArgument.Text = "+";
            this.but_AddArgument.UseVisualStyleBackColor = true;
            // 
            // but_RemoveArgument
            // 
            this.but_RemoveArgument.Enabled = false;
            this.but_RemoveArgument.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_RemoveArgument.Location = new System.Drawing.Point(138, 408);
            this.but_RemoveArgument.Name = "but_RemoveArgument";
            this.but_RemoveArgument.Size = new System.Drawing.Size(20, 24);
            this.but_RemoveArgument.TabIndex = 21;
            this.but_RemoveArgument.Text = "-";
            this.but_RemoveArgument.UseVisualStyleBackColor = true;
            this.but_RemoveArgument.Click += new System.EventHandler(this.but_RemoveArgument_Click);
            // 
            // but_RemoveRequirement
            // 
            this.but_RemoveRequirement.Enabled = false;
            this.but_RemoveRequirement.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_RemoveRequirement.Location = new System.Drawing.Point(778, 408);
            this.but_RemoveRequirement.Name = "but_RemoveRequirement";
            this.but_RemoveRequirement.Size = new System.Drawing.Size(20, 24);
            this.but_RemoveRequirement.TabIndex = 23;
            this.but_RemoveRequirement.Text = "-";
            this.but_RemoveRequirement.UseVisualStyleBackColor = true;
            this.but_RemoveRequirement.Click += new System.EventHandler(this.but_RemoveRequirement_Click);
            // 
            // but_AddRequirement
            // 
            this.but_AddRequirement.Enabled = false;
            this.but_AddRequirement.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_AddRequirement.Location = new System.Drawing.Point(778, 378);
            this.but_AddRequirement.Name = "but_AddRequirement";
            this.but_AddRequirement.Size = new System.Drawing.Size(20, 24);
            this.but_AddRequirement.TabIndex = 22;
            this.but_AddRequirement.Text = "+";
            this.but_AddRequirement.UseVisualStyleBackColor = true;
            this.but_AddRequirement.Click += new System.EventHandler(this.but_AddRequirement_Click);
            // 
            // but_RemoveFunction
            // 
            this.but_RemoveFunction.Enabled = false;
            this.but_RemoveFunction.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_RemoveFunction.Location = new System.Drawing.Point(778, 252);
            this.but_RemoveFunction.Name = "but_RemoveFunction";
            this.but_RemoveFunction.Size = new System.Drawing.Size(20, 24);
            this.but_RemoveFunction.TabIndex = 25;
            this.but_RemoveFunction.Text = "-";
            this.but_RemoveFunction.UseVisualStyleBackColor = true;
            this.but_RemoveFunction.Click += new System.EventHandler(this.but_RemoveFunction_Click);
            // 
            // but_AddFunction
            // 
            this.but_AddFunction.Enabled = false;
            this.but_AddFunction.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_AddFunction.Location = new System.Drawing.Point(778, 222);
            this.but_AddFunction.Name = "but_AddFunction";
            this.but_AddFunction.Size = new System.Drawing.Size(20, 24);
            this.but_AddFunction.TabIndex = 24;
            this.but_AddFunction.Text = "+";
            this.but_AddFunction.UseVisualStyleBackColor = true;
            this.but_AddFunction.Click += new System.EventHandler(this.but_AddFunction_Click);
            // 
            // but_RemoveEvent
            // 
            this.but_RemoveEvent.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_RemoveEvent.Location = new System.Drawing.Point(138, 252);
            this.but_RemoveEvent.Name = "but_RemoveEvent";
            this.but_RemoveEvent.Size = new System.Drawing.Size(20, 24);
            this.but_RemoveEvent.TabIndex = 27;
            this.but_RemoveEvent.Text = "-";
            this.but_RemoveEvent.UseVisualStyleBackColor = true;
            this.but_RemoveEvent.Click += new System.EventHandler(this.but_RemoveEvent_Click);
            // 
            // but_AddEvent
            // 
            this.but_AddEvent.BackColor = System.Drawing.SystemColors.Control;
            this.but_AddEvent.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_AddEvent.Location = new System.Drawing.Point(138, 222);
            this.but_AddEvent.Name = "but_AddEvent";
            this.but_AddEvent.Size = new System.Drawing.Size(20, 24);
            this.but_AddEvent.TabIndex = 26;
            this.but_AddEvent.Text = "+";
            this.but_AddEvent.UseVisualStyleBackColor = false;
            this.but_AddEvent.Click += new System.EventHandler(this.but_AddEvent_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.panel1.BackColor = System.Drawing.SystemColors.WindowText;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(762, 190);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 10);
            this.panel1.TabIndex = 28;
            this.panel1.Visible = false;
            // 
            // FunctionDown
            // 
            this.FunctionDown.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FunctionDown.Enabled = false;
            this.FunctionDown.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FunctionDown.Location = new System.Drawing.Point(778, 312);
            this.FunctionDown.Name = "FunctionDown";
            this.FunctionDown.Size = new System.Drawing.Size(20, 24);
            this.FunctionDown.TabIndex = 32;
            this.FunctionDown.Text = "↓";
            this.FunctionDown.UseVisualStyleBackColor = true;
            this.FunctionDown.Click += new System.EventHandler(this.FunctionDown_Click);
            // 
            // FunctionUp
            // 
            this.FunctionUp.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FunctionUp.Enabled = false;
            this.FunctionUp.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FunctionUp.Location = new System.Drawing.Point(778, 282);
            this.FunctionUp.Name = "FunctionUp";
            this.FunctionUp.Size = new System.Drawing.Size(20, 24);
            this.FunctionUp.TabIndex = 31;
            this.FunctionUp.Text = "↑";
            this.FunctionUp.UseVisualStyleBackColor = true;
            this.FunctionUp.Click += new System.EventHandler(this.FunctionUp_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(809, 24);
            this.menuStrip1.TabIndex = 33;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(185, 22);
            this.toolStripMenuItem1.Text = "Extract Data from AO";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(190, 295);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(435, 64);
            this.panel2.TabIndex = 34;
            this.panel2.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 26);
            this.progressBar1.Maximum = 120000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(427, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 2;
            this.progressBar1.UseWaitCursor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(145, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Extracting Data... please wait";
            // 
            // PlayfieldBox
            // 
            this.PlayfieldBox.FormattingEnabled = true;
            this.PlayfieldBox.Location = new System.Drawing.Point(12, 48);
            this.PlayfieldBox.Name = "PlayfieldBox";
            this.PlayfieldBox.Size = new System.Drawing.Size(120, 147);
            this.PlayfieldBox.TabIndex = 35;
            this.PlayfieldBox.SelectedIndexChanged += new System.EventHandler(this.PlayfieldBox_SelectedIndexChanged);
            // 
            // StatelBox
            // 
            this.StatelBox.FormattingEnabled = true;
            this.StatelBox.Location = new System.Drawing.Point(173, 48);
            this.StatelBox.Name = "StatelBox";
            this.StatelBox.Size = new System.Drawing.Size(599, 147);
            this.StatelBox.TabIndex = 36;
            this.StatelBox.SelectedIndexChanged += new System.EventHandler(this.StatelBox_SelectedIndexChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 535);
            this.Controls.Add(this.StatelBox);
            this.Controls.Add(this.PlayfieldBox);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.FunctionDown);
            this.Controls.Add(this.FunctionUp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.but_RemoveEvent);
            this.Controls.Add(this.but_AddEvent);
            this.Controls.Add(this.but_RemoveFunction);
            this.Controls.Add(this.but_AddFunction);
            this.Controls.Add(this.but_RemoveRequirement);
            this.Controls.Add(this.but_AddRequirement);
            this.Controls.Add(this.but_RemoveArgument);
            this.Controls.Add(this.but_AddArgument);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.RequirementBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ArgumentBox);
            this.Controls.Add(this.l_Functions);
            this.Controls.Add(this.FunctionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "CellAO Statel Event Creator/Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox EventBox;
        private System.Windows.Forms.ListBox FunctionBox;
        private System.Windows.Forms.Label l_Functions;
        private System.Windows.Forms.ListBox ArgumentBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox RequirementBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button but_AddArgument;
        private System.Windows.Forms.Button but_RemoveArgument;
        private System.Windows.Forms.Button but_RemoveRequirement;
        private System.Windows.Forms.Button but_AddRequirement;
        private System.Windows.Forms.Button but_RemoveFunction;
        private System.Windows.Forms.Button but_AddFunction;
        private System.Windows.Forms.Button but_RemoveEvent;
        private System.Windows.Forms.Button but_AddEvent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button FunctionDown;
        private System.Windows.Forms.Button FunctionUp;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListBox PlayfieldBox;
        private System.Windows.Forms.ListBox StatelBox;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

