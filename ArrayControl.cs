using System;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class ArrayControl : UserControl
    {
        string[] names = { "Charles", "Joshua", "Maria", "Shawn" };

        public ArrayControl()
        {
            InitializeComponent();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                lstbArray.Items.Clear();
                lstbArray.Items.Add(names[Convert.ToInt32(txtIndex.Text)]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtIndex.Clear();
                txtIndex.Focus();
            }
        }

        private void btnDisplayAll_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            lstbArray.Items.Clear();
            txtValue.Clear();
            txtIndex.Clear();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                int newLength = names.Length + 1;
                Array.Resize(ref names, newLength);
                names[newLength - 1] = txtValue.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtValue.Focus();
                return;
            }

            MessageBox.Show("New value inserted");
            DisplayAll();
            txtValue.Clear();
            txtIndex.Clear();
        }

        private void DisplayAll()
        {
            lstbArray.Items.Clear();
            foreach (string name in names)
            {
                lstbArray.Items.Add(name);
            }
        }
    }
}
