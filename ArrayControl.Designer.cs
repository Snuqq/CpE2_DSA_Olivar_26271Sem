namespace CpE2_DSA_Olivar_26271Sem
{
    partial class ArrayControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArrayControl));
            this.lstbArray = new System.Windows.Forms.ListBox();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnDisplayAll = new System.Windows.Forms.Button();
            this.btnDisplay = new System.Windows.Forms.Button();
            this.txtIndex = new System.Windows.Forms.TextBox();
            this.lblIndex = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstbArray
            // 
            this.lstbArray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstbArray.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstbArray.FormattingEnabled = true;
            this.lstbArray.ItemHeight = 24;
            this.lstbArray.Location = new System.Drawing.Point(0, 0);
            this.lstbArray.Name = "lstbArray";
            this.lstbArray.Size = new System.Drawing.Size(610, 352);
            this.lstbArray.TabIndex = 0;
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnClearAll);
            this.pnlControls.Controls.Add(this.btnInsert);
            this.pnlControls.Controls.Add(this.txtValue);
            this.pnlControls.Controls.Add(this.lblValue);
            this.pnlControls.Controls.Add(this.btnDisplayAll);
            this.pnlControls.Controls.Add(this.btnDisplay);
            this.pnlControls.Controls.Add(this.txtIndex);
            this.pnlControls.Controls.Add(this.lblIndex);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlControls.Location = new System.Drawing.Point(330, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(280, 352);
            this.pnlControls.TabIndex = 1;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearAll.Location = new System.Drawing.Point(20, 285);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(230, 40);
            this.btnClearAll.TabIndex = 8;
            this.btnClearAll.Text = "Clear";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsert.Location = new System.Drawing.Point(20, 225);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(230, 40);
            this.btnInsert.TabIndex = 7;
            this.btnInsert.Text = "Insert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // txtValue
            // 
            this.txtValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValue.Location = new System.Drawing.Point(140, 176);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(110, 29);
            this.txtValue.TabIndex = 6;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(20, 180);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(109, 24);
            this.lblValue.TabIndex = 5;
            this.lblValue.Text = "Insert Value";
            // 
            // btnDisplayAll
            // 
            this.btnDisplayAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplayAll.Location = new System.Drawing.Point(20, 120);
            this.btnDisplayAll.Name = "btnDisplayAll";
            this.btnDisplayAll.Size = new System.Drawing.Size(230, 40);
            this.btnDisplayAll.TabIndex = 4;
            this.btnDisplayAll.Text = "Display All";
            this.btnDisplayAll.UseVisualStyleBackColor = true;
            this.btnDisplayAll.Click += new System.EventHandler(this.btnDisplayAll_Click);
            // 
            // btnDisplay
            // 
            this.btnDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisplay.Location = new System.Drawing.Point(20, 70);
            this.btnDisplay.Name = "btnDisplay";
            this.btnDisplay.Size = new System.Drawing.Size(230, 40);
            this.btnDisplay.TabIndex = 3;
            this.btnDisplay.Text = "Display Index Value";
            this.btnDisplay.UseVisualStyleBackColor = true;
            this.btnDisplay.Click += new System.EventHandler(this.btnDisplay_Click);
            // 
            // txtIndex
            // 
            this.txtIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIndex.Location = new System.Drawing.Point(140, 20);
            this.txtIndex.Name = "txtIndex";
            this.txtIndex.Size = new System.Drawing.Size(110, 29);
            this.txtIndex.TabIndex = 2;
            // 
            // lblIndex
            // 
            this.lblIndex.AutoSize = true;
            this.lblIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndex.Location = new System.Drawing.Point(20, 25);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(92, 24);
            this.lblIndex.TabIndex = 1;
            this.lblIndex.Text = "Index No.";
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // ArrayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.lstbArray);
            this.Name = "ArrayControl";
            this.Size = new System.Drawing.Size(610, 352);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstbArray;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.TextBox txtIndex;
        private System.Windows.Forms.Button btnDisplay;
        private System.Windows.Forms.Button btnDisplayAll;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnClearAll;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
    }
}