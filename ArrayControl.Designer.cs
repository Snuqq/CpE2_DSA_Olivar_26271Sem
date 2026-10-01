namespace CpE2_DSA_Olivar_26271Sem
{
    partial class ArrayControl
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
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblIndex = new System.Windows.Forms.Label();
            this.txtIndexNo = new System.Windows.Forms.TextBox();
            this.btnDisplayIndexValue = new System.Windows.Forms.Button();
            this.btnDisplayAll = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstbArray = new System.Windows.Forms.ListBox();
            this.grpArray = new System.Windows.Forms.GroupBox();
            this.grpArray.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblValue
            // 
            this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(470, 84);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(108, 25);
            this.lblValue.TabIndex = 0;
            this.lblValue.Text = "Insert value";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(584, 86);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(145, 26);
            this.txtValue.TabIndex = 1;
            // 
            // lblIndex
            // 
            this.lblIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndex.Location = new System.Drawing.Point(488, 121);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(90, 25);
            this.lblIndex.TabIndex = 2;
            this.lblIndex.Text = "Index No.";
            // 
            // txtIndexNo
            // 
            this.txtIndexNo.Location = new System.Drawing.Point(581, 118);
            this.txtIndexNo.Name = "txtIndexNo";
            this.txtIndexNo.Size = new System.Drawing.Size(145, 26);
            this.txtIndexNo.TabIndex = 3;
            // 
            // btnDisplayIndexValue
            // 
            this.btnDisplayIndexValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplayIndexValue.Location = new System.Drawing.Point(484, 171);
            this.btnDisplayIndexValue.Name = "btnDisplayIndexValue";
            this.btnDisplayIndexValue.Size = new System.Drawing.Size(120, 38);
            this.btnDisplayIndexValue.TabIndex = 4;
            this.btnDisplayIndexValue.Text = "Display Index";
            this.btnDisplayIndexValue.Click += new System.EventHandler(this.btnDisplayIndexValue_Click);
            // 
            // btnDisplayAll
            // 
            this.btnDisplayAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplayAll.Location = new System.Drawing.Point(609, 171);
            this.btnDisplayAll.Name = "btnDisplayAll";
            this.btnDisplayAll.Size = new System.Drawing.Size(120, 38);
            this.btnDisplayAll.TabIndex = 5;
            this.btnDisplayAll.Text = "Display All";
            this.btnDisplayAll.Click += new System.EventHandler(this.btnDisplayAll_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsert.Location = new System.Drawing.Point(484, 219);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(120, 38);
            this.btnInsert.TabIndex = 6;
            this.btnInsert.Text = "Insert";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(609, 219);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 38);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(484, 260);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(245, 100);
            this.lblStatus.TabIndex = 8;
            // 
            // lstbArray
            // 
            this.lstbArray.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstbArray.HorizontalScrollbar = true;
            this.lstbArray.ItemHeight = 20;
            this.lstbArray.Location = new System.Drawing.Point(20, 31);
            this.lstbArray.Name = "lstbArray";
            this.lstbArray.Size = new System.Drawing.Size(400, 324);
            this.lstbArray.TabIndex = 9;
            // 
            // grpArray
            // 
            this.grpArray.Controls.Add(this.lblValue);
            this.grpArray.Controls.Add(this.txtValue);
            this.grpArray.Controls.Add(this.lblIndex);
            this.grpArray.Controls.Add(this.txtIndexNo);
            this.grpArray.Controls.Add(this.btnDisplayIndexValue);
            this.grpArray.Controls.Add(this.btnDisplayAll);
            this.grpArray.Controls.Add(this.btnInsert);
            this.grpArray.Controls.Add(this.btnClear);
            this.grpArray.Controls.Add(this.lblStatus);
            this.grpArray.Controls.Add(this.lstbArray);
            this.grpArray.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpArray.Location = new System.Drawing.Point(14, 16);
            this.grpArray.Name = "grpArray";
            this.grpArray.Size = new System.Drawing.Size(770, 430);
            this.grpArray.TabIndex = 10;
            this.grpArray.TabStop = false;
            this.grpArray.Text = "Array";
            // 
            // ArrayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.grpArray);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ArrayControl";
            this.Text = "Array (zero-based index)";
            this.grpArray.ResumeLayout(false);
            this.grpArray.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.TextBox txtIndexNo;
        private System.Windows.Forms.Button btnDisplayIndexValue;
        private System.Windows.Forms.Button btnDisplayAll;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstbArray;
        private System.Windows.Forms.GroupBox grpArray;
    }
}
