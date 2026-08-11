using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class LinkedListForm : Form
    {
        Form mainForm;
        LinkedList<int> Llist = new LinkedList<int>();

        public LinkedListForm(Form frm)
        {
            InitializeComponent();
            mainForm = frm;

            cmbOperation.SelectedIndex = 0;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(txtValue.Text);
                string operation = cmbOperation.SelectedItem?.ToString();

                switch (operation)
                {
                    case "AddFirst":
                        Llist.AddFirst(value);
                        break;

                    case "AddLast":
                        Llist.AddLast(value);
                        break;

                    case "AddAfter":
                        {
                            LinkedListNode<int> node = GetSelectedNode();
                            if (node == null) return;

                            Llist.AddAfter(node, value);
                            break;
                        }

                    case "AddBefore":
                        {
                            LinkedListNode<int> node = GetSelectedNode();
                            if (node == null) return;

                            Llist.AddBefore(node, value);
                            break;
                        }

                    default:
                        MessageBox.Show("Please select an operation.");
                        return;
                }

                DisplayAll();

                txtValue.Clear();
                txtValue.Focus();
            }
            catch
            {
                MessageBox.Show("Invalid input.");
                txtValue.Focus();
            }
        }

        private LinkedListNode<int> GetSelectedNode()
        {
            if (lstbLinkedList.SelectedItem == null)
            {
                MessageBox.Show("Select a value in the list first.");
                return null;
            }

            int selectedValue = Convert.ToInt32(lstbLinkedList.SelectedItem);
            return Llist.Find(selectedValue);
        }

        private void DisplayAll()
        {
            lstbLinkedList.Items.Clear();

            foreach (int value in Llist)
            {
                lstbLinkedList.Items.Add(value);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            Close();
        }

        private void LinkedListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.Show();
        }
    }
}