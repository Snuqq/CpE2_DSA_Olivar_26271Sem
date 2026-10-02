using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            CenterMainPanel();
        }

        private void CenterMainPanel()
        {
            splitContainer1.Location = new System.Drawing.Point(
                (ClientSize.Width - splitContainer1.Width) / 2,
                (ClientSize.Height - splitContainer1.Height) / 2);
        }

        private void Form1_Load(object sender, EventArgs e) { CenterMainPanel(); }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterMainPanel();
            Invalidate();
        }

        private void btnArray_Click(object sender, EventArgs e)
        {
            formShow(new ArrayControl(), splitContainer2.Panel2);
        }

        private void btnLinkedList_Click(object sender, EventArgs e)
        {
            Form linkedListForm = new Form();
            linkedListForm.Controls.Add(new Linkedlist { Dock = DockStyle.Fill });
            formShow(linkedListForm, splitContainer2.Panel2);
        }

        private void btnStack_Click(object sender, EventArgs e)
        {
            formShow(new Stack(), splitContainer2.Panel2);
        }

        private void btnQueue_Click(object sender, EventArgs e)
        {
            formShow(new Queue(), splitContainer2.Panel2);
        }

        private void formShow(Form formToShow, Panel formToShowParent)
        {
            while (formToShowParent.Controls.Count > 0)
                formToShowParent.Controls[0].Dispose();
            formToShow.TopLevel = false;
            formToShow.FormBorderStyle = FormBorderStyle.None;
            formToShow.Dock = DockStyle.Fill;
            formToShowParent.Controls.Add(formToShow);
            formToShow.Show();
            formToShow.Select();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            formShow(new ListForm(), splitContainer2.Panel2);
        }
    }
}
