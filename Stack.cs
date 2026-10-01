using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Stack : Form
    {
        private readonly Stack<string> myStack = new Stack<string>();
        public Stack()
        {
            InitializeComponent();
            ActiveControl = txtStack;
            btnpush.Click += FocusInput;
            btnpop.Click += FocusInput;
            btnDisplay.Click += FocusInput;
            btnContains.Click += FocusInput;
            btnCount.Click += FocusInput;
            btnClear.Click += FocusInput;
            btnPeek.Click += FocusInput;
        }

        private void FocusInput(object sender, EventArgs e)
        {
            txtStack.Focus();
            txtStack.SelectAll();
        }

        private void Stack_Load(object sender, EventArgs e) { DisplayAll(); }

        private void btnpush_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            myStack.Push(txtStack.Text);
            DisplayAll();
        }

        private void btnpop_Click(object sender, EventArgs e)
        {
            if (!HasItems()) return;
            myStack.Pop();
            DisplayAll();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        private void btnContains_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Contains value: " + myStack.Contains(txtStack.Text);
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Count: " + myStack.Count;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            myStack.Clear();
            DisplayAll();
        }

        private void btnPeek_Click(object sender, EventArgs e)
        {
            if (!HasItems()) return;
            lblStatus.Text = "Next value: " + myStack.Peek();
        }

        private bool HasValue()
        {
            if (!string.IsNullOrWhiteSpace(txtStack.Text)) return true;
            lblStatus.Text = "Insert a value first.";
            txtStack.Focus();
            return false;
        }

        private bool HasItems()
        {
            if (myStack.Count > 0) return true;
            lblStatus.Text = "The stack is empty.";
            return false;
        }

        private void DisplayAll()
        {
            lstDisplay.Items.Clear();
            foreach (string value in myStack) lstDisplay.Items.Add(value);
            lblStatus.Text = "Count: " + myStack.Count;
        }

        private void grpStack_Enter(object sender, EventArgs e)
        {

        }

        private void lstDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
