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
    public partial class DynamicEdit : Form
    {

        public FunctionTemplates template;
        public DynamicEdit()
        {
            InitializeComponent();
        }

        public void createDynels(FunctionTemplates templ)
        {
            string dummystring = "";
            int dummyint = 0;
            Single dummysingle = 0.0f;
            LineValue dummyline = new LineValue();

            PlayfieldValue dummypf = new PlayfieldValue();


            template = templ;
            int y = 20;
            int i = 0;
            foreach (FunctionTemplate ft in templ.Templates)
            {
                if (ft.DataType == dummystring.GetType())
                {
                    Label lab = new Label();
                    lab.Parent = this;
                    lab.Left = 20;
                    lab.Top = y+3;
                    lab.Text = ft.name + ":";
                    lab.Name = "Lab" + i;
                    lab.AutoSize = true;
                    lab.Update();

                    TextBox ed = new TextBox();
                    ed.Parent = this;
                    ed.Left = 20;
                    ed.Text = ft.baseValue;
                    ed.Top = y;
                    ed.Name = "Ed" + i;
                    ed.Height = 20;
                    ed.Width = 200;
                    ed.Enabled = !ft.ro;

                    Panel p = new Panel();
                    p.Parent = this;
                    p.Left = 0;
                    p.Width = this.Width;
                    p.Height = 1;
                    p.Top = y + 25;
                    p.BackColor = Color.DarkGray;

                    y += 30;
                    i++;
                }
                else
                    if (ft.DataType == dummypf.GetType())
                    {
                        Label lab = new Label();
                        lab.Parent = this;
                        lab.Left = 20;
                        lab.Top = y + 3;
                        lab.Text = ft.name + ":";
                        lab.Name = "Lab" + i;
                        lab.AutoSize = true;
                        lab.Update();

                        TextBox ed = new TextBox();
                        ed.Parent = this;
                        ed.Name = "Ed" + i;
                        ed.Top = y;
                        ed.Text = ft.baseValue;
                        ed.Left = 20;
                        ed.Height = 20;
                        ed.Enabled = false;

                        Button b = new Button();
                        b.Parent = this;
                        b.Text = "Choose PF";
                        b.Left = 20;
                        b.Enabled = !ft.ro;
                        b.Top = y-1;
                        b.MouseClick += pfbutton;
                        b.Name = "but" + i;

                        Panel p = new Panel();
                        p.Parent = this;
                        p.Left = 0;
                        p.Width = this.Width;
                        p.Height = 1;
                        p.Top = y + 25;
                        p.BackColor = Color.DarkGray;

                        y += 30;
                        i++;

                    }
                    else
                        if (ft.DataType == dummyint.GetType())
                        {
                            Label lab = new Label();
                            lab.Parent = this;
                            lab.Left = 20;
                            lab.Top = y+3;
                            lab.Text = ft.name + ":";
                            lab.Name = "Lab" + i;
                            lab.AutoSize = true;
                            lab.Update();

                            TextBox ed = new TextBox();
                            ed.Parent = this;
                            ed.Left = 20;
                            ed.Text = ft.baseValue;
                            ed.Top = y;
                            ed.Name = "Ed" + i;
                            ed.Enabled = !ft.ro;

                            Panel p = new Panel();
                            p.Parent = this;
                            p.Left = 0;
                            p.Width = this.Width;
                            p.Height = 1;
                            p.Top = y + 25;
                            p.BackColor = Color.DarkGray;

                            y += 30;
                            i++;
                        }
                        else
                            if (ft.DataType == dummyline.GetType())
                            {
                                Label lab = new Label();
                                lab.Parent = this;
                                lab.Left = 20;
                                lab.Top = y + 3;
                                lab.Text = ft.name + ":";
                                lab.Name = "Lab" + i;
                                lab.AutoSize = true;
                                lab.Update();

                                TextBox ed = new TextBox();
                                ed.Parent = this;
                                ed.Left = 20;
                                int num1;
                                int num2;
                                num1 = Int32.Parse(ft.baseValue);
                                num2 = num1 & 0xffff;
                                num1 = num1 >> 16;
                                ed.Text = num1.ToString();
                                ed.Enabled = !ft.ro;
                                ed.Name = "Ed" + i;
                                ed.Top = y;
                                y += 30;


                                ed = new TextBox();
                                ed.Parent = this;
                                ed.Name = "pfholder" + i;
                                ed.Text = num2.ToString();
                                ed.Left = 20;
                                ed.Top = y;
                                ed.Enabled = false;

                                Button b = new Button();
                                b.Parent = this;
                                b.Text = "Choose PF";
                                b.Left = 20;
                                b.Enabled = !ft.ro;
                                b.Top = y - 1;
                                b.MouseClick += pfbutton2;
                                b.Name = "pfbut" + i;

                                Panel p = new Panel();
                                p.Parent = this;
                                p.Left = 0;
                                p.Width = this.Width;
                                p.Height = 1;
                                p.Top = y + 25;
                                p.BackColor = Color.DarkGray;

                                i++;
                                y += 30;

                            }


            }
            int maxx = 20;
            for (int c = 0; c < i; c++)
            {
                maxx = Math.Max(this.Controls["Lab" + c].Left + this.Controls["Lab" + c].Width, maxx);
            }

            for (int c = 0; c < i; c++)
            {
                this.Controls["Ed" + c].Left = maxx;
                if (this.Controls["but" + c] != null)
                {
                    this.Controls["but" + c].Left = this.Controls["Ed" + c].Left + this.Controls["Ed" + c].Width + 10;
                }
                if (template.Templates.ElementAt(c).DataType == dummyline.GetType())
                {
                    this.Controls["pfholder" + c].Left = maxx;
                    this.Controls["pfbut" + c].Left = this.Controls["pfholder" + c].Left + this.Controls["pfholder" + c].Width + 10;
                }
            }

            this.Height= y + 80;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void pfbutton(object sender, EventArgs e)
        {
            SelectPlayfield sp = new SelectPlayfield();
            int num = Int32.Parse(MainWindow.strAfter(((Button)sender).Name, "but"));
            sp.selectpf(this.Controls["Ed" + num].Text);
            sp.ShowDialog();
            if (sp.DialogResult == DialogResult.OK)
            {
                this.Controls["Ed" + num].Text = sp.selectedplayfield.ToString();
            }
        }

        private void pfbutton2(object sender, EventArgs e)
        {
            SelectPlayfield sp = new SelectPlayfield();
            int num = Int32.Parse(MainWindow.strAfter(((Button)sender).Name, "pfbut"));
            sp.selectpf(this.Controls["pfholder" + num].Text);
            sp.ShowDialog();
            if (sp.DialogResult == DialogResult.OK)
            {
                this.Controls["pfholder" + num].Text = sp.selectedplayfield.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int c=0;
            LineValue dummyline = new LineValue();

            foreach (FunctionTemplate ft in template.Templates)
            {
                ft.baseValue = this.Controls["Ed" + c].Text;
                if (ft.DataType == dummyline.GetType())
                {
                    int num1;
                    int num2;
                    num2 = Int32.Parse(this.Controls["pfholder" + c].Text);
                    num1 = Int32.Parse(ft.baseValue);
                    ft.baseValue = ((num1 << 16) + num2).ToString();
                }
                c++;
            }
        }
    }
}
