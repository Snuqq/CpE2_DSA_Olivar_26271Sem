namespace CpE2_DSA_Olivar_26271Sem
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.navigation = new System.Windows.Forms.TableLayoutPanel();
            this.lblStructures = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnInfographic = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnFibonacciSearch = new System.Windows.Forms.Button();
            this.btnExponentialSearch = new System.Windows.Forms.Button();
            this.btnJumpSearch = new System.Windows.Forms.Button();
            this.btnArray = new System.Windows.Forms.Button();
            this.btnLinkList = new System.Windows.Forms.Button();
            this.btnStack = new System.Windows.Forms.Button();
            this.btnQueue = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.navigation.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Maroon;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.MaximumSize = new System.Drawing.Size(1013, 602);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Maroon;
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1013, 602);
            this.splitContainer1.SplitterDistance = 130;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Orange;
            this.label3.Location = new System.Drawing.Point(9, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(306, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Christian Josh D. Olivar (CpE-2)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Orange;
            this.label2.Location = new System.Drawing.Point(9, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(509, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data Structures and Algorithms (1st Sem 2026-2027)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Impact", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gold;
            this.label1.Location = new System.Drawing.Point(7, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(761, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "University of Perpetual Help DALTA - Calamba Campus";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.Gray;
            this.splitContainer2.Panel1.Controls.Add(this.navigation);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Size = new System.Drawing.Size(1013, 471);
            this.splitContainer2.SplitterDistance = 199;
            this.splitContainer2.TabIndex = 0;
            //
            // navigation
            //
            this.navigation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.navigation.ColumnCount = 1;
            this.navigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.navigation.Controls.Add(this.lblStructures, 0, 0);
            this.navigation.Controls.Add(this.btnArray, 0, 1);
            this.navigation.Controls.Add(this.btnLinkList, 0, 2);
            this.navigation.Controls.Add(this.btnStack, 0, 3);
            this.navigation.Controls.Add(this.btnQueue, 0, 4);
            this.navigation.Controls.Add(this.button1, 0, 5);
            this.navigation.Controls.Add(this.lblSearch, 0, 6);
            this.navigation.Controls.Add(this.btnJumpSearch, 0, 7);
            this.navigation.Controls.Add(this.btnExponentialSearch, 0, 8);
            this.navigation.Controls.Add(this.btnFibonacciSearch, 0, 9);
            this.navigation.Controls.Add(this.btnInfographic, 0, 10);
            this.navigation.Location = new System.Drawing.Point(15, 12);
            this.navigation.Name = "navigation";
            this.navigation.RowCount = 11;
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.navigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.lblStructures.Text = "DATA STRUCTURES";
            this.lblStructures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStructures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStructures.ForeColor = System.Drawing.Color.White;
            this.lblStructures.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);

            this.lblSearch.Text = "SEARCH ALGORITHMS";
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSearch.ForeColor = System.Drawing.Color.White;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.navigation.Size = new System.Drawing.Size(165, 448);
            this.navigation.TabIndex = 0;
            // 
            // button1
            // 
            this.btnInfographic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfographic.FlatAppearance.BorderSize = 0;
            this.btnInfographic.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnInfographic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInfographic.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnInfographic.Name = "btnInfographic";
            this.btnInfographic.Text = "Infographic";
            this.btnInfographic.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnInfographic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInfographic.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnInfographic.TabIndex = 8;
            this.btnInfographic.UseVisualStyleBackColor = false;
            this.btnInfographic.Click += new System.EventHandler(this.btnInfographic_Click);
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.button1.Location = new System.Drawing.Point(15, 236);
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 46);
            this.button1.TabIndex = 4;
            this.button1.Text = "List";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnList_Click);
            // 
            // btnFibonacciSearch
            // 
            this.btnFibonacciSearch.BackColor = System.Drawing.Color.White;
            this.btnFibonacciSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFibonacciSearch.Location = new System.Drawing.Point(15, 404);
            this.btnFibonacciSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFibonacciSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnFibonacciSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFibonacciSearch.FlatAppearance.BorderSize = 0;
            this.btnFibonacciSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnFibonacciSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFibonacciSearch.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnFibonacciSearch.Name = "btnFibonacciSearch";
            this.btnFibonacciSearch.Size = new System.Drawing.Size(165, 46);
            this.btnFibonacciSearch.TabIndex = 7;
            this.btnFibonacciSearch.Text = "Fibonacci Search";
            this.btnFibonacciSearch.UseVisualStyleBackColor = false;
            this.btnFibonacciSearch.Click += new System.EventHandler(this.btnFibonacciSearch_Click);
            // 
            // btnExponentialSearch
            // 
            this.btnExponentialSearch.BackColor = System.Drawing.Color.White;
            this.btnExponentialSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExponentialSearch.Location = new System.Drawing.Point(15, 348);
            this.btnExponentialSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExponentialSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnExponentialSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExponentialSearch.FlatAppearance.BorderSize = 0;
            this.btnExponentialSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnExponentialSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExponentialSearch.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnExponentialSearch.Name = "btnExponentialSearch";
            this.btnExponentialSearch.Size = new System.Drawing.Size(165, 46);
            this.btnExponentialSearch.TabIndex = 6;
            this.btnExponentialSearch.Text = "Exponential Search";
            this.btnExponentialSearch.UseVisualStyleBackColor = false;
            this.btnExponentialSearch.Click += new System.EventHandler(this.btnExponentialSearch_Click);
            // 
            // btnJumpSearch
            // 
            this.btnJumpSearch.BackColor = System.Drawing.Color.White;
            this.btnJumpSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJumpSearch.Location = new System.Drawing.Point(15, 292);
            this.btnJumpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnJumpSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnJumpSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJumpSearch.FlatAppearance.BorderSize = 0;
            this.btnJumpSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnJumpSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJumpSearch.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnJumpSearch.Name = "btnJumpSearch";
            this.btnJumpSearch.Size = new System.Drawing.Size(165, 46);
            this.btnJumpSearch.TabIndex = 5;
            this.btnJumpSearch.Text = "Jump Search";
            this.btnJumpSearch.UseVisualStyleBackColor = false;
            this.btnJumpSearch.Click += new System.EventHandler(this.btnJumpSearch_Click);
            // 
            // btnArray
            // 
            this.btnArray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArray.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnArray.Location = new System.Drawing.Point(15, 12);
            this.btnArray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnArray.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArray.FlatAppearance.BorderSize = 0;
            this.btnArray.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnArray.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnArray.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnArray.Name = "btnArray";
            this.btnArray.Size = new System.Drawing.Size(165, 46);
            this.btnArray.TabIndex = 0;
            this.btnArray.Text = "Array";
            this.btnArray.UseVisualStyleBackColor = false;
            this.btnArray.Click += new System.EventHandler(this.btnArray_Click);
            // 
            // btnLinkList
            // 
            this.btnLinkList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkList.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnLinkList.Location = new System.Drawing.Point(15, 68);
            this.btnLinkList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLinkList.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnLinkList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLinkList.FlatAppearance.BorderSize = 0;
            this.btnLinkList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnLinkList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLinkList.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnLinkList.Name = "btnLinkList";
            this.btnLinkList.Size = new System.Drawing.Size(165, 46);
            this.btnLinkList.TabIndex = 1;
            this.btnLinkList.Text = "LinkedList";
            this.btnLinkList.UseVisualStyleBackColor = false;
            this.btnLinkList.Click += new System.EventHandler(this.btnLinkedList_Click);
            // 
            // btnStack
            // 
            this.btnStack.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnStack.Location = new System.Drawing.Point(15, 124);
            this.btnStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStack.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnStack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStack.FlatAppearance.BorderSize = 0;
            this.btnStack.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStack.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnStack.Name = "btnStack";
            this.btnStack.Size = new System.Drawing.Size(165, 46);
            this.btnStack.TabIndex = 2;
            this.btnStack.Text = "Stack";
            this.btnStack.UseVisualStyleBackColor = false;
            this.btnStack.Click += new System.EventHandler(this.btnStack_Click);
            // 
            // btnQueue
            // 
            this.btnQueue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnQueue.Location = new System.Drawing.Point(15, 180);
            this.btnQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQueue.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnQueue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQueue.FlatAppearance.BorderSize = 0;
            this.btnQueue.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnQueue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQueue.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnQueue.Name = "btnQueue";
            this.btnQueue.Size = new System.Drawing.Size(165, 46);
            this.btnQueue.TabIndex = 3;
            this.btnQueue.Text = "Queue";
            this.btnQueue.UseVisualStyleBackColor = false;
            this.btnQueue.Click += new System.EventHandler(this.btnQueue_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1284, 941);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MinimumSize = new System.Drawing.Size(980, 570);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.navigation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel navigation;
        private System.Windows.Forms.Button btnLinkList;
        private System.Windows.Forms.Button btnArray;
        private System.Windows.Forms.Button btnStack;
        private System.Windows.Forms.Button btnQueue;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnFibonacciSearch;
        private System.Windows.Forms.Button btnExponentialSearch;
        private System.Windows.Forms.Button btnJumpSearch;
        private System.Windows.Forms.Button btnInfographic;
        private System.Windows.Forms.Label lblStructures;
        private System.Windows.Forms.Label lblSearch;
    }
}
