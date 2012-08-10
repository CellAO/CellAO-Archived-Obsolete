using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CreateStatelEvents
{
    public partial class SelectPlayfield : Form
    {

        public int selectedplayfield = -1;

        public SelectPlayfield()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedplayfield = Int32.Parse(MainWindow.strBefore(PFBox.SelectedItem.ToString(), " "));
        }

        public void selectpf(string pf)
        {
            for (int c = 0; c < PFBox.Items.Count; c++)
            {
                if (MainWindow.strBefore(PFBox.Items[c].ToString(), " ") == pf)
                {
                    PFBox.SelectedIndex = c;
                    return;
                }
            }
        }
    }
}
