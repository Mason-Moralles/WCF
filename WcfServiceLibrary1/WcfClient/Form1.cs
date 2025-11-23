using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WcfClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var client = new ServiceReference1.Service1Client();

            var users = client.GetUsers();

            listBoxUsers.Items.Clear();

            foreach (var u in users)
            {
                listBoxUsers.Items.Add($"{u.Email} | {u.Password}");
            }

            client.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
