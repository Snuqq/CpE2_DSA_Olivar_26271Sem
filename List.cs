using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class ListForm : Form
    {
        List<string> myList = new List<string>();

        public ListForm()
        {
            InitializeComponent();
            ActiveControl = txtValue;

            btnAdd.Click += btnAdd_Click;
            btnInsert.Click += btnInsert_Click;
            btnRemove.Click += btnRemove_Click;
            btnRemoveAt.Click += btnRemoveAt_Click;
            btnCount.Click += btnCount_Click;
            btnContains.Click += btnContains_Click;
            btnIndexOf.Click += btnIndexOf_Click;
            btnSort.Click += btnSort_Click;
            btnClear.Click += btnClear_Click;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            myList.Add(txtValue.Text);
            DisplayAll();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            int index;
            if (!HasValue() || !GetIndex(true, out index)) return;
            myList.Insert(index, txtValue.Text);
            DisplayAll();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            bool removed = myList.Remove(txtValue.Text);
            DisplayAll();
            Status(removed ? "Value removed." : "Value not found.");
        }

        private void btnRemoveAt_Click(object sender, EventArgs e)
        {
            int index;
            if (!GetIndex(false, out index)) return;
            myList.RemoveAt(index);
            DisplayAll();
            txtIndex.Focus();
            txtIndex.SelectAll();
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            Status("Count: " + myList.Count);
        }

        private void btnContains_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            Status("Contains: " + myList.Contains(txtValue.Text));
        }

        private void btnIndexOf_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            Status("Index: " + myList.IndexOf(txtValue.Text));
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            myList.Sort();
            DisplayAll();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            myList.Clear();
            txtValue.Clear();
            txtIndex.Clear();
            DisplayAll();
        }

        private void DisplayAll()
        {
            lstDisplay.Items.Clear();
            lstDisplay.Items.AddRange(myList.ToArray());
            Status("Count: " + myList.Count);
        }

        private void Status(string message)
        {
            lblStatus.Text = message;
            txtValue.Focus();
            txtValue.SelectAll();
        }

        private bool HasValue()
        {
            if (!string.IsNullOrWhiteSpace(txtValue.Text)) return true;
            Status("Insert a value first.");
            return false;
        }

        private bool GetIndex(bool inserting, out int index)
        {
            int max = inserting ? myList.Count : myList.Count - 1;

            if (int.TryParse(txtIndex.Text, out index)
                && index >= 0 && index <= max)
                return true;

            lblStatus.Text = max < 0
                ? "The list is empty."
                : "Enter an index from 0 to " + max + ".";

            txtIndex.Focus();
            txtIndex.SelectAll();
            return false;
        }
    }
}
