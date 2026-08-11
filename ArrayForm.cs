using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class ArrayForm : Form
    {
        Form mainForm;

        string[] names = { "Charles", "Joshua", "Maria", "Shawn" };

        public ArrayForm(Form frm)
        {
            InitializeComponent();
            mainForm = frm;
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(txtIndex.Text);
                MessageBox.Show(names[index], "Index " + index);

                txtIndex.Focus();
                txtIndex.SelectAll();
            }
            catch
            {
                MessageBox.Show("Invalid input.");
                txtIndex.Focus();
                txtIndex.SelectAll();
            }
        }

        private void btnDisplayAll_Click(object sender, EventArgs e)
        {
            lstbArray.Items.Clear();

            foreach (string name in names)
            {
                lstbArray.Items.Add(name);
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            lstbArray.Items.Clear();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                string[] temp = new string[names.Length + 1];

                for (int i = 0; i < names.Length; i++)
                {
                    temp[i] = names[i];
                }

                temp[names.Length] = txtValue.Text;

                names = temp;

                txtValue.Clear();
                txtIndex.Clear();

                txtValue.Focus();
            }
            catch
            {
                MessageBox.Show("Error.");
                txtValue.Focus();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            Close();
        }

        private void ArrayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.Show();
        }
    }
}