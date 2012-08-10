namespace CreateStatelEvents
{
    partial class SelectFunction
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Functions = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tickcountbox = new System.Windows.Forms.TextBox();
            this.tickintervalbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Targets = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(337, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(256, 350);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Functions
            // 
            this.Functions.FormattingEnabled = true;
            this.Functions.Location = new System.Drawing.Point(12, 12);
            this.Functions.Name = "Functions";
            this.Functions.Size = new System.Drawing.Size(246, 316);
            this.Functions.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "TickCount";
            // 
            // tickcountbox
            // 
            this.tickcountbox.Location = new System.Drawing.Point(267, 28);
            this.tickcountbox.Name = "tickcountbox";
            this.tickcountbox.Size = new System.Drawing.Size(145, 20);
            this.tickcountbox.TabIndex = 4;
            // 
            // tickintervalbox
            // 
            this.tickintervalbox.Location = new System.Drawing.Point(267, 77);
            this.tickintervalbox.Name = "tickintervalbox";
            this.tickintervalbox.Size = new System.Drawing.Size(145, 20);
            this.tickintervalbox.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "TickInterval";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Target";
            // 
            // Targets
            // 
            this.Targets.FormattingEnabled = true;
            this.Targets.Location = new System.Drawing.Point(267, 129);
            this.Targets.Name = "Targets";
            this.Targets.Size = new System.Drawing.Size(145, 199);
            this.Targets.TabIndex = 8;
            // 
            // SelectFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 385);
            this.Controls.Add(this.Targets);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tickintervalbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tickcountbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Functions);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "SelectFunction";
            this.Text = "SelectFunction";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox Functions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tickcountbox;
        private System.Windows.Forms.TextBox tickintervalbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox Targets;
    }
}