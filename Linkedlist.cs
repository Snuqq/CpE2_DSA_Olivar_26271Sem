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

        private void button1_Click(object sender, EventArgs e)
        {
            switch(Llist.Text)
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
    }
}
