namespace CpE2_DSA_Olivar_26271Sem
{
    partial class Stack
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.lblValue = new System.Windows.Forms.Label();
            this.txtStack = new System.Windows.Forms.TextBox();
            this.btnpush = new System.Windows.Forms.Button();
            this.btnpop = new System.Windows.Forms.Button();
            this.btnContains = new System.Windows.Forms.Button();
            this.btnCount = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnPeek = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstDisplay = new System.Windows.Forms.ListBox();
            this.grpStack = new System.Windows.Forms.GroupBox();
            this.grpStack.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblValue
            // 
            this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(468, 66);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(107, 25);
            this.lblValue.TabIndex = 0;
            this.lblValue.Text = "Insert value";
            // 
            // txtStack
            // 
            this.txtStack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStack.Location = new System.Drawing.Point(471, 94);
            this.txtStack.Name = "txtStack";
            this.txtStack.Size = new System.Drawing.Size(245, 26);
            this.txtStack.TabIndex = 1;
            // 
            // btnpush
            // 
            this.btnpush.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpush.Location = new System.Drawing.Point(472, 126);
            this.btnpush.Name = "btnpush";
            this.btnpush.Size = new System.Drawing.Size(120, 38);
            this.btnpush.TabIndex = 2;
            this.btnpush.Text = "Push";
            this.btnpush.Click += new System.EventHandler(this.btnpush_Click);
            // 
            // btnpop
            // 
            this.btnpop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpop.Location = new System.Drawing.Point(472, 170);
            this.btnpop.Name = "btnpop";
            this.btnpop.Size = new System.Drawing.Size(120, 38);
            this.btnpop.TabIndex = 3;
            this.btnpop.Text = "Pop";
            this.btnpop.Click += new System.EventHandler(this.btnpop_Click);
            // 
            // btnContains
            // 
            this.btnContains.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContains.Location = new System.Drawing.Point(598, 126);
            this.btnContains.Name = "btnContains";
            this.btnContains.Size = new System.Drawing.Size(120, 38);
            this.btnContains.TabIndex = 5;
            this.btnContains.Text = "Contains";
            this.btnContains.Click += new System.EventHandler(this.btnContains_Click);
            // 
            // btnCount
            // 
            this.btnCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCount.Location = new System.Drawing.Point(471, 214);
            this.btnCount.Name = "btnCount";
            this.btnCount.Size = new System.Drawing.Size(120, 38);
            this.btnCount.TabIndex = 6;
            this.btnCount.Text = "Count";
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(598, 214);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 38);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnPeek
            // 
            this.btnPeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPeek.Location = new System.Drawing.Point(598, 170);
            this.btnPeek.Name = "btnPeek";
            this.btnPeek.Size = new System.Drawing.Size(120, 38);
            this.btnPeek.TabIndex = 8;
            this.btnPeek.Text = "Peek";
            this.btnPeek.Click += new System.EventHandler(this.btnPeek_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(471, 255);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(245, 100);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // lstDisplay
            // 
            this.lstDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstDisplay.HorizontalScrollbar = true;
            this.lstDisplay.ItemHeight = 20;
            this.lstDisplay.Location = new System.Drawing.Point(20, 31);
            this.lstDisplay.Name = "lstDisplay";
            this.lstDisplay.Size = new System.Drawing.Size(400, 324);
            this.lstDisplay.TabIndex = 10;
            this.lstDisplay.SelectedIndexChanged += new System.EventHandler(this.lstDisplay_SelectedIndexChanged);
            // 
            // grpStack
            // 
            this.grpStack.Controls.Add(this.lblValue);
            this.grpStack.Controls.Add(this.txtStack);
            this.grpStack.Controls.Add(this.btnpush);
            this.grpStack.Controls.Add(this.btnpop);
            this.grpStack.Controls.Add(this.btnContains);
            this.grpStack.Controls.Add(this.btnCount);
            this.grpStack.Controls.Add(this.btnClear);
            this.grpStack.Controls.Add(this.btnPeek);
            this.grpStack.Controls.Add(this.lblStatus);
            this.grpStack.Controls.Add(this.lstDisplay);
            this.grpStack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStack.Location = new System.Drawing.Point(14, 16);
            this.grpStack.Name = "grpStack";
            this.grpStack.Size = new System.Drawing.Size(770, 430);
            this.grpStack.TabIndex = 11;
            this.grpStack.TabStop = false;
            this.grpStack.Text = "Stack";
            this.grpStack.Enter += new System.EventHandler(this.grpStack_Enter);
            // 
            // Stack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.grpStack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Stack";
            this.Text = "Stack (top to bottom)";
            this.Load += new System.EventHandler(this.Stack_Load);
            this.grpStack.ResumeLayout(false);
            this.grpStack.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtStack;
        private System.Windows.Forms.Button btnpush;
        private System.Windows.Forms.Button btnpop;
        private System.Windows.Forms.Button btnContains;
        private System.Windows.Forms.Button btnCount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPeek;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstDisplay;
        private System.Windows.Forms.GroupBox grpStack;
    }
}
