using System;
using System.Collections;
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
    public partial class Linkedlist : UserControl
    {
        int[] myInt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        LinkedList<int> myLinkedList = new LinkedList<int>();
        LinkedListNode<int> current;
        public Linkedlist()
        {
            InitializeComponent();
        }



        private void Linkedlist_Load(object sender, EventArgs e)
        {
            LoadArrayToLinkedList();
        }

        private void LoadArrayToLinkedList()
        {
            myLinkedList.Clear();
            foreach (int i in myInt)
            {
                myLinkedList.AddLast(i);
            }
        }

        private void bttndisplay_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        private void DisplayAll()
        {
            lstbLinkedList.Items.Clear();
            foreach (int value in myLinkedList)
            {
                lstbLinkedList.Items.Add(value);
            }
        }

        private void bttnInsert_Click(object sender, EventArgs e)
        {
            try
            {

                switch (Llist.Text)
                {
                    case "AddFirst":
                        myLinkedList.AddFirst(Convert.ToInt32(txtValue.Text));
                        break;
                    case "AddLast":
                        myLinkedList.AddLast(Convert.ToInt32(txtValue.Text));
                        break;
                    default:
                        MessageBox.Show("Please select an option from the dropdown.");
                        break;
                }
                DisplayAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                txtValue.Focus();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                switch (cmdRemove.Text)
                {
                    case "RemoveFirst":
                        myLinkedList.RemoveFirst();
                        break;
                    case "RemoveLast":
                        myLinkedList.RemoveLast();
                        break;
                    default:
                        MessageBox.Show("Please select an option to remove.");
                        break;
                }
                DisplayAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnInsertBeforeAfter_Click(object sender, EventArgs e)
        {
            current = myLinkedList.Find(Convert.ToInt32(txtCurrent.Text));
            try
            {
                switch (cmbAddBeforeAfter.Text)
                {
                    case "AddBefore":
                        myLinkedList.AddBefore(current, Convert.ToInt32(txtValue2.Text));
                        break;
                    case "AddAfter":
                        myLinkedList.AddAfter(current, Convert.ToInt32(txtValue2.Text));
                        break;
                    default:
                        MessageBox.Show("Please select an option to insert.");
                        break;
                }
                DisplayAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Count: " + myLinkedList.Count.ToString());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            myLinkedList.Clear();
            DisplayAll();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                LinkedListNode<int> found = myLinkedList.Find(Convert.ToInt32(txtFind.Text));
                if (found != null)
                {
                    MessageBox.Show("Found: " + found.Value.ToString());
                }
                else
                {
                    MessageBox.Show("Value not found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadArrayToLinkedList();
            DisplayAll();
        }

    }
}
