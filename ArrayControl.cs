using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class ArrayControl : Form
    {
        private string[] studentName = { "Jerome", "Maria", "Juan", "John" };
        public ArrayControl()
        {
            InitializeComponent();
            ActiveControl = txtValue;
            btnInsert.Click += FocusValue;
            btnClear.Click += FocusValue;
            btnDisplayAll.Click += FocusValue;
            btnDisplayIndexValue.Click += delegate
            {
                txtIndexNo.Focus();
                txtIndexNo.SelectAll();
            };
        }

        private void FocusValue(object sender, EventArgs e)
        {
            txtValue.Focus();
            txtValue.SelectAll();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstbArray.Items.Clear();
            txtValue.Clear();
            txtIndexNo.Clear();
            lblStatus.Text = "Display cleared. Display All restores the array values.";
        }

        private void btnDisplayIndexValue_Click(object sender, EventArgs e)
        {
            int index;
            if (!int.TryParse(txtIndexNo.Text, out index) || index < 0 || index >= studentName.Length)
            {
                lblStatus.Text = "Enter an index from 0 to " + (studentName.Length - 1) + ".";
                txtIndexNo.SelectAll();
                txtIndexNo.Focus();
                return;
            }
            lstbArray.Items.Clear();
            lstbArray.Items.Add(studentName[index]);
            lblStatus.Text = "Index " + index;
        }

        private void btnDisplayAll_Click(object sender, EventArgs e) { DisplayAll(); }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtValue.Text))
            {
                lblStatus.Text = "Insert a value first.";
                txtValue.Focus();
                return;
            }
            int length = studentName.Length + 1;
            Array.Resize(ref studentName, length);
            studentName[length - 1] = txtValue.Text;
            DisplayAll();
            lblStatus.Text = "New value inserted!";
        }

        private void DisplayAll()
        {
            lstbArray.Items.Clear();
            foreach (string studName in studentName) lstbArray.Items.Add(studName);
            lblStatus.Text = "Count: " + studentName.Length;
        }
    }
}
