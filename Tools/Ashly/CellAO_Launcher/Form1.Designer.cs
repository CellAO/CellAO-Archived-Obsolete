namespace CellAO_Launcher
{
    partial class Form1
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.btn_Update = new System.Windows.Forms.Button();
            this.btn_StartLE = new System.Windows.Forms.Button();
            this.btn_StartZE = new System.Windows.Forms.Button();
            this.btn_StartCE = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(949, 294);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://www.aocell.info", System.UriKind.Absolute);
            // 
            // btn_Update
            // 
            this.btn_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Update.Enabled = false;
            this.btn_Update.Location = new System.Drawing.Point(12, 305);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(217, 23);
            this.btn_Update.TabIndex = 2;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Visible = false;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // btn_StartLE
            // 
            this.btn_StartLE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_StartLE.Location = new System.Drawing.Point(327, 305);
            this.btn_StartLE.Name = "btn_StartLE";
            this.btn_StartLE.Size = new System.Drawing.Size(125, 23);
            this.btn_StartLE.TabIndex = 3;
            this.btn_StartLE.Text = "Start LoginEngine";
            this.btn_StartLE.UseVisualStyleBackColor = true;
            this.btn_StartLE.Click += new System.EventHandler(this.btn_StartLE_Click);
            // 
            // btn_StartZE
            // 
            this.btn_StartZE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_StartZE.Location = new System.Drawing.Point(458, 305);
            this.btn_StartZE.Name = "btn_StartZE";
            this.btn_StartZE.Size = new System.Drawing.Size(123, 23);
            this.btn_StartZE.TabIndex = 4;
            this.btn_StartZE.Text = "Start ZoneEngine";
            this.btn_StartZE.UseVisualStyleBackColor = true;
            this.btn_StartZE.Click += new System.EventHandler(this.btn_StartZE_Click);
            // 
            // btn_StartCE
            // 
            this.btn_StartCE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_StartCE.Location = new System.Drawing.Point(588, 305);
            this.btn_StartCE.Name = "btn_StartCE";
            this.btn_StartCE.Size = new System.Drawing.Size(115, 23);
            this.btn_StartCE.TabIndex = 5;
            this.btn_StartCE.Text = "Start ChatEngine";
            this.btn_StartCE.UseVisualStyleBackColor = true;
            this.btn_StartCE.Click += new System.EventHandler(this.btn_StartCE_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 340);
            this.Controls.Add(this.btn_StartCE);
            this.Controls.Add(this.btn_StartZE);
            this.Controls.Add(this.btn_StartLE);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.Button btn_StartLE;
        private System.Windows.Forms.Button btn_StartZE;
        private System.Windows.Forms.Button btn_StartCE;
    }
}

