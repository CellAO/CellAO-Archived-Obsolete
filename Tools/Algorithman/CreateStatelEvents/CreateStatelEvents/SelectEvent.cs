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
    public partial class SelectEvent : Form
    {
        public SelectEvent()
        {
            InitializeComponent();
        }

        public int returnvalue = -1;

        private void SelectEvent_Load(object sender, EventArgs e)
        {
            EventsBox.Items.Clear();
            foreach (int key in NamesandNumbers.Events.Keys)
            {
                EventsBox.Items.Add(key.ToString() + " " + NamesandNumbers.Events[key]);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            returnvalue = Int32.Parse(MainWindow.strBefore((string)EventsBox.SelectedItem, " "));
        }
    }
}
