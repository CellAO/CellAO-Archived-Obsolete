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
    public partial class SelectFunction : Form
    {
        public SelectFunction()
        {
            InitializeComponent();
            set();
        }

        public int functionnum;
        public int tickcount;
        public int tickinterval;
        public int target;

        public void set()
        {
            Functions.Items.Clear();
            foreach (int key in NamesandNumbers.Functions.Keys)
            {
                Functions.Items.Add(NamesandNumbers.Functions[key] + " (" + key + ")");
            }
            Targets.Items.Clear();
            foreach (int key in NamesandNumbers.Targets.Keys)
            {
                Targets.Items.Add(NamesandNumbers.Targets[key] + " (" + key + ")");
            }

        }

        public void getdata(StatelFunction sf)
        {
            for (int c = 0; c < Functions.Items.Count; c++)
            {
                if (MainWindow.strBefore(MainWindow.strAfter(Functions.Items[c].ToString(), "("),")") == sf.functionnum.ToString())
                {
                    Functions.SelectedIndex = c;
                    break;
                }
            }

            tickcountbox.Text = sf.tickcount.ToString();
            tickintervalbox.Text = sf.tickinterval.ToString();
            for (int c = 0; c < Targets.Items.Count; c++)
            {
                if (MainWindow.strBefore(MainWindow.strAfter(Targets.Items[c].ToString(), "("), ")") == sf.target.ToString())
                {
                    Targets.SelectedIndex = c;
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            functionnum = Int32.Parse(MainWindow.strAfter(MainWindow.strBefore(Functions.SelectedItem.ToString(),")"),"("));
            tickcount = Int32.Parse(tickcountbox.Text);
            tickinterval = Int32.Parse(tickintervalbox.Text);
            target = Int32.Parse(MainWindow.strAfter(MainWindow.strBefore(Targets.SelectedItem.ToString(), ")"), "("));
        }
    }
}
