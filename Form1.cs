using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnArray_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            ArrayControl ctrl = new ArrayControl();
            ctrl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(ctrl);
        }

        private void btnLinkedList_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            LinkedListControl ctrl = new LinkedListControl();
            ctrl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(ctrl);
        }
        private void lblAuthor_Click(object sender, EventArgs e)
        {
            
        }
    }
}