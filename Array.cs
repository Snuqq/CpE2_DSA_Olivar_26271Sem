using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Array : Form
    {
        private Form mainForm;

        string[] names =
        {
            "Charles",
            "Joshua",
            "Maria",
            "Shawn"
        };

        public Array(Form frm)
        {
            InitializeComponent();
            mainForm = frm;
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndex.Text, out int index))
            {
                if (index >= 0 && index < names.Length)
                {
                    txtValue.Text = names[index];
                }
                else
                {
                    MessageBox.Show("Invalid index.");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid index.");
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndex.Text, out int index))
            {
                if (index >= 0 && index < names.Length)
                {
                    names[index] = txtValue.Text;
                    MessageBox.Show("Value inserted.");
                }
                else
                {
                    MessageBox.Show("Invalid index.");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid index.");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Close();
        }

        private void Array_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.Show();
        }
    }
}