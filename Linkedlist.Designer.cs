namespace CpE2_DSA_Olivar_26271Sem
{
    partial class Linkedlist
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstbLinkedList = new System.Windows.Forms.ListBox();
            this.Llist = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bttndisplay = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstbLinkedList);
            this.groupBox1.Controls.Add(this.Llist);
            this.groupBox1.Controls.Add(this.txtValue);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bttndisplay);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(772, 429);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LinkedList";
            // 
            // lstbLinkedList
            // 
            this.lstbLinkedList.FormattingEnabled = true;
            this.lstbLinkedList.ItemHeight = 20;
            this.lstbLinkedList.Location = new System.Drawing.Point(16, 41);
            this.lstbLinkedList.Name = "lstbLinkedList";
            this.lstbLinkedList.Size = new System.Drawing.Size(340, 304);
            this.lstbLinkedList.TabIndex = 6;
            // 
            // Llist
            // 
            this.Llist.FormattingEnabled = true;
            this.Llist.Items.AddRange(new object[] {
            "AddFirst",
            "AddLast"});
            this.Llist.Location = new System.Drawing.Point(463, 192);
            this.Llist.Name = "Llist";
            this.Llist.Size = new System.Drawing.Size(241, 28);
            this.Llist.TabIndex = 5;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(463, 226);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(242, 26);
            this.txtValue.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(362, 229);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Insert Value";
            // 
            // bttndisplay
            // 
            this.bttndisplay.Location = new System.Drawing.Point(588, 269);
            this.bttndisplay.Name = "bttndisplay";
            this.bttndisplay.Size = new System.Drawing.Size(119, 25);
            this.bttndisplay.TabIndex = 2;
            this.bttndisplay.Text = "Display All";
            this.bttndisplay.UseVisualStyleBackColor = true;
            this.bttndisplay.Click += new System.EventHandler(this.bttndisplay_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(463, 269);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 25);
            this.button1.TabIndex = 1;
            this.button1.Text = "Insert";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Linkedlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "Linkedlist";
            this.Size = new System.Drawing.Size(800, 450);
            this.Load += new System.EventHandler(this.Linkedlist_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bttndisplay;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Llist;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ListBox lstbLinkedList;
    }
}