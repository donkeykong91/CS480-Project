using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManageDevices
{
    public partial class Form3 : Form
    {
        private string password;
        private string retyped;
        private bool isClicked = false;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isClicked = true;
            if (retyped != password) { label3.Visible = true; }
            else
            {
                Form1 main = new ManageDevices.Form1();
                main.Show();
                this.Hide();
            }
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            password = textBox1.Text;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) 
        {
            textBox1.UseSystemPasswordChar = checkBox1.Checked == false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
                textBox2.Enabled = true;
                retyped = textBox2.Text;
           

            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
