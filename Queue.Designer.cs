namespace CpE2_DSA_Olivar_26271Sem
{
    partial class Queue
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
            this.txtQueue = new System.Windows.Forms.TextBox();
            this.btnEnqueue = new System.Windows.Forms.Button();
            this.btnDequeue = new System.Windows.Forms.Button();
            this.btnContains = new System.Windows.Forms.Button();
            this.btnCount = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnPeek = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstDisplay = new System.Windows.Forms.ListBox();
            this.grpQueue = new System.Windows.Forms.GroupBox();
            this.grpQueue.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblValue
            // 
            this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(465, 67);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(103, 25);
            this.lblValue.TabIndex = 0;
            this.lblValue.Text = "Insert value";
            // 
            // txtQueue
            // 
            this.txtQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQueue.Location = new System.Drawing.Point(469, 95);
            this.txtQueue.Name = "txtQueue";
            this.txtQueue.Size = new System.Drawing.Size(246, 26);
            this.txtQueue.TabIndex = 1;
            // 
            // btnEnqueue
            // 
            this.btnEnqueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnqueue.Location = new System.Drawing.Point(469, 127);
            this.btnEnqueue.Name = "btnEnqueue";
            this.btnEnqueue.Size = new System.Drawing.Size(120, 38);
            this.btnEnqueue.TabIndex = 2;
            this.btnEnqueue.Text = "Enqueue";
            this.btnEnqueue.Click += new System.EventHandler(this.btnEnqueue_Click);
            // 
            // btnDequeue
            // 
            this.btnDequeue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDequeue.Location = new System.Drawing.Point(469, 171);
            this.btnDequeue.Name = "btnDequeue";
            this.btnDequeue.Size = new System.Drawing.Size(120, 38);
            this.btnDequeue.TabIndex = 3;
            this.btnDequeue.Text = "Dequeue";
            this.btnDequeue.Click += new System.EventHandler(this.btnDequeue_Click);
            // 
            // btnContains
            // 
            this.btnContains.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContains.Location = new System.Drawing.Point(595, 127);
            this.btnContains.Name = "btnContains";
            this.btnContains.Size = new System.Drawing.Size(120, 38);
            this.btnContains.TabIndex = 5;
            this.btnContains.Text = "Contains";
            this.btnContains.Click += new System.EventHandler(this.btnContains_Click);
            // 
            // btnCount
            // 
            this.btnCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCount.Location = new System.Drawing.Point(469, 215);
            this.btnCount.Name = "btnCount";
            this.btnCount.Size = new System.Drawing.Size(120, 38);
            this.btnCount.TabIndex = 6;
            this.btnCount.Text = "Count";
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(595, 215);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 38);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnPeek
            // 
            this.btnPeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPeek.Location = new System.Drawing.Point(595, 171);
            this.btnPeek.Name = "btnPeek";
            this.btnPeek.Size = new System.Drawing.Size(120, 38);
            this.btnPeek.TabIndex = 8;
            this.btnPeek.Text = "Peek";
            this.btnPeek.Click += new System.EventHandler(this.btnPeek_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(470, 256);
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
            // 
            // grpQueue
            // 
            this.grpQueue.Controls.Add(this.lblValue);
            this.grpQueue.Controls.Add(this.txtQueue);
            this.grpQueue.Controls.Add(this.btnEnqueue);
            this.grpQueue.Controls.Add(this.btnDequeue);
            this.grpQueue.Controls.Add(this.btnContains);
            this.grpQueue.Controls.Add(this.btnCount);
            this.grpQueue.Controls.Add(this.btnClear);
            this.grpQueue.Controls.Add(this.btnPeek);
            this.grpQueue.Controls.Add(this.lblStatus);
            this.grpQueue.Controls.Add(this.lstDisplay);
            this.grpQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpQueue.Location = new System.Drawing.Point(14, 16);
            this.grpQueue.Name = "grpQueue";
            this.grpQueue.Size = new System.Drawing.Size(770, 430);
            this.grpQueue.TabIndex = 11;
            this.grpQueue.TabStop = false;
            this.grpQueue.Text = "Queue";
            // 
            // Queue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.grpQueue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Queue";
            this.Text = "Queue (front to back)";
            this.Load += new System.EventHandler(this.Queue_Load);
            this.grpQueue.ResumeLayout(false);
            this.grpQueue.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtQueue;
        private System.Windows.Forms.Button btnEnqueue;
        private System.Windows.Forms.Button btnDequeue;
        private System.Windows.Forms.Button btnContains;
        private System.Windows.Forms.Button btnCount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPeek;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstDisplay;
        private System.Windows.Forms.GroupBox grpQueue;
    }
}
