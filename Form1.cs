using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            splitContainer2.Panel2.Resize += Panel2_Resize;
        }

        private void btnArray_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            ArrayControl ctrl = new ArrayControl();
            splitContainer2.Panel2.Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
        }

        private void btnLinkedList_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            Linkedlist ctrl = new Linkedlist();
            splitContainer2.Panel2.Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
        }

        private void CenterInPanel2(Control ctrl)
        {
            ctrl.Location = new Point(
                (splitContainer2.Panel2.ClientSize.Width - ctrl.Width) / 2,
                (splitContainer2.Panel2.ClientSize.Height - ctrl.Height) / 2
            );
        }

        private void Panel2_Resize(object sender, EventArgs e)
        {
            if (splitContainer2.Panel2.Controls.Count > 0)
            {
                CenterInPanel2(splitContainer2.Panel2.Controls[0]);
            }
        }

        private void CenterSplitContainer()
        {
            splitContainer1.Location = new Point(
                (ClientSize.Width - splitContainer1.Width) / 2,
                (ClientSize.Height - splitContainer1.Height) / 2
            );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterSplitContainer();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterSplitContainer();
            Invalidate();
        }
    }
}
