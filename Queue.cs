using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class Queue : Form
    {
        private readonly Queue<string> myQueue = new Queue<string>();
        public Queue()
        {
            InitializeComponent();
            ActiveControl = txtQueue;
            btnEnqueue.Click += FocusInput;
            btnDequeue.Click += FocusInput;
            btnDisplay.Click += FocusInput;
            btnContains.Click += FocusInput;
            btnCount.Click += FocusInput;
            btnClear.Click += FocusInput;
            btnPeek.Click += FocusInput;
        }

        private void FocusInput(object sender, EventArgs e)
        {
            txtQueue.Focus();
            txtQueue.SelectAll();
        }

        private void Queue_Load(object sender, EventArgs e) { DisplayAll(); }

        private void btnEnqueue_Click(object sender, EventArgs e)
        {
            if (!HasValue()) return;
            myQueue.Enqueue(txtQueue.Text);
            DisplayAll();
        }

        private void btnDequeue_Click(object sender, EventArgs e)
        {
            if (!HasItems()) return;
            myQueue.Dequeue();
            DisplayAll();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            DisplayAll();
        }

        private void btnContains_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Contains value: " + myQueue.Contains(txtQueue.Text);
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Count: " + myQueue.Count;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            myQueue.Clear();
            DisplayAll();
        }

        private void btnPeek_Click(object sender, EventArgs e)
        {
            if (!HasItems()) return;
            lblStatus.Text = "Next value: " + myQueue.Peek();
        }

        private bool HasValue()
        {
            if (!string.IsNullOrWhiteSpace(txtQueue.Text)) return true;
            lblStatus.Text = "Insert a value first.";
            txtQueue.Focus();
            return false;
        }

        private bool HasItems()
        {
            if (myQueue.Count > 0) return true;
            lblStatus.Text = "The queue is empty.";
            return false;
        }

        private void DisplayAll()
        {
            lstDisplay.Items.Clear();
            foreach (string value in myQueue) lstDisplay.Items.Add(value);
            lblStatus.Text = "Count: " + myQueue.Count;
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
