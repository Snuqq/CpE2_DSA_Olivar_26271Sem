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
                lblArray.Text = names[index];

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

        private void lblValue_Click(object sender, EventArgs e)
        {

        }
    }
}