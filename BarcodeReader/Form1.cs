using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarcodeReader
{
    public partial class Form1 : Form
    {
        private string datafile;
        private System.IO.StreamReader sr;
        private System.IO.StreamWriter sw;
        public Form1()
        {
            InitializeComponent();
            datafile = System.Environment.CurrentDirectory;
            sr = new System.IO.StreamReader(datafile + "/data.txt");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                listBox1.Items.Add(textBox1.Text);
                textBox1.Text = string.Empty;
                try { 
                
                }
                catch { }
            }
        }
    }
}
