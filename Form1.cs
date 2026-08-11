using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
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

        private void btnArray_Click(object sender, EventArgs e)
        {
            ArrayForm frm = new ArrayForm(this);
            frm.Show();
            this.Hide();
        }

        private void btnLinkedList_Click(object sender, EventArgs e)
        {
            LinkedListForm frm = new LinkedListForm(this);
            frm.Show();
            this.Hide();
        }
    }
}