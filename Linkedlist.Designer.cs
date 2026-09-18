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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCurrent = new System.Windows.Forms.TextBox();
            this.cmbAddBeforeAfter = new System.Windows.Forms.ComboBox();
            this.txtValue2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInsertBeforeAfter = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCount = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.cmbRemove = new System.Windows.Forms.GroupBox();
            this.cmdRemove = new System.Windows.Forms.ComboBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.cmbAdd = new System.Windows.Forms.GroupBox();
            this.Llist = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bttnInsert = new System.Windows.Forms.Button();
            this.lstbLinkedList = new System.Windows.Forms.ListBox();
            this.bttndisplay = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.cmbRemove.SuspendLayout();
            this.cmbAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnCount);
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.cmbRemove);
            this.groupBox1.Controls.Add(this.cmbAdd);
            this.groupBox1.Controls.Add(this.lstbLinkedList);
            this.groupBox1.Controls.Add(this.bttndisplay);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(826, 567);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LinkedList";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnFind);
            this.groupBox3.Controls.Add(this.txtFind);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(486, 246);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(257, 179);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Find Item";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(152, 65);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(90, 30);
            this.btnFind.TabIndex = 12;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(107, 33);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(135, 26);
            this.txtFind.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Find Value";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtCurrent);
            this.groupBox2.Controls.Add(this.cmbAddBeforeAfter);
            this.groupBox2.Controls.Add(this.txtValue2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnInsertBeforeAfter);
            this.groupBox2.Location = new System.Drawing.Point(223, 246);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 179);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Add Item Before / After";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Current Index";
            // 
            // txtCurrent
            // 
            this.txtCurrent.Location = new System.Drawing.Point(116, 59);
            this.txtCurrent.Name = "txtCurrent";
            this.txtCurrent.Size = new System.Drawing.Size(135, 26);
            this.txtCurrent.TabIndex = 6;
            // 
            // cmbAddBeforeAfter
            // 
            this.cmbAddBeforeAfter.FormattingEnabled = true;
            this.cmbAddBeforeAfter.Items.AddRange(new object[] {
            "AddBefore",
            "AddAfter"});
            this.cmbAddBeforeAfter.Location = new System.Drawing.Point(116, 25);
            this.cmbAddBeforeAfter.Name = "cmbAddBeforeAfter";
            this.cmbAddBeforeAfter.Size = new System.Drawing.Size(135, 28);
            this.cmbAddBeforeAfter.TabIndex = 5;
            // 
            // txtValue2
            // 
            this.txtValue2.Location = new System.Drawing.Point(116, 91);
            this.txtValue2.Name = "txtValue2";
            this.txtValue2.Size = new System.Drawing.Size(135, 26);
            this.txtValue2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Insert Value";
            // 
            // btnInsertBeforeAfter
            // 
            this.btnInsertBeforeAfter.Location = new System.Drawing.Point(161, 123);
            this.btnInsertBeforeAfter.Name = "btnInsertBeforeAfter";
            this.btnInsertBeforeAfter.Size = new System.Drawing.Size(90, 30);
            this.btnInsertBeforeAfter.TabIndex = 1;
            this.btnInsertBeforeAfter.Text = "Insert";
            this.btnInsertBeforeAfter.UseVisualStyleBackColor = true;
            this.btnInsertBeforeAfter.Click += new System.EventHandler(this.btnInsertBeforeAfter_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(415, 41);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 30);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCount
            // 
            this.btnCount.Location = new System.Drawing.Point(319, 41);
            this.btnCount.Name = "btnCount";
            this.btnCount.Size = new System.Drawing.Size(90, 30);
            this.btnCount.TabIndex = 10;
            this.btnCount.Text = "Count";
            this.btnCount.UseVisualStyleBackColor = true;
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(511, 41);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(146, 30);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset Initial Value";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // cmbRemove
            // 
            this.cmbRemove.Controls.Add(this.cmdRemove);
            this.cmbRemove.Controls.Add(this.btnRemove);
            this.cmbRemove.Location = new System.Drawing.Point(486, 80);
            this.cmbRemove.Name = "cmbRemove";
            this.cmbRemove.Size = new System.Drawing.Size(257, 160);
            this.cmbRemove.TabIndex = 8;
            this.cmbRemove.TabStop = false;
            this.cmbRemove.Text = "Remove Item";
            // 
            // cmdRemove
            // 
            this.cmdRemove.FormattingEnabled = true;
            this.cmdRemove.Items.AddRange(new object[] {
            "RemoveFirst",
            "RemoveLast"});
            this.cmdRemove.Location = new System.Drawing.Point(46, 63);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(180, 28);
            this.cmdRemove.TabIndex = 5;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(136, 97);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 30);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // cmbAdd
            // 
            this.cmbAdd.Controls.Add(this.Llist);
            this.cmbAdd.Controls.Add(this.txtValue);
            this.cmbAdd.Controls.Add(this.label1);
            this.cmbAdd.Controls.Add(this.bttnInsert);
            this.cmbAdd.Location = new System.Drawing.Point(223, 80);
            this.cmbAdd.Name = "cmbAdd";
            this.cmbAdd.Size = new System.Drawing.Size(257, 160);
            this.cmbAdd.TabIndex = 7;
            this.cmbAdd.TabStop = false;
            this.cmbAdd.Text = "Add New Item";
            // 
            // Llist
            // 
            this.Llist.FormattingEnabled = true;
            this.Llist.Items.AddRange(new object[] {
            "AddFirst",
            "AddLast"});
            this.Llist.Location = new System.Drawing.Point(107, 25);
            this.Llist.Name = "Llist";
            this.Llist.Size = new System.Drawing.Size(135, 28);
            this.Llist.TabIndex = 5;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(107, 65);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(135, 26);
            this.txtValue.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Insert Value";
            // 
            // bttnInsert
            // 
            this.bttnInsert.Location = new System.Drawing.Point(152, 97);
            this.bttnInsert.Name = "bttnInsert";
            this.bttnInsert.Size = new System.Drawing.Size(90, 30);
            this.bttnInsert.TabIndex = 1;
            this.bttnInsert.Text = "Insert";
            this.bttnInsert.UseVisualStyleBackColor = true;
            this.bttnInsert.Click += new System.EventHandler(this.bttnInsert_Click);
            // 
            // lstbLinkedList
            // 
            this.lstbLinkedList.FormattingEnabled = true;
            this.lstbLinkedList.ItemHeight = 20;
            this.lstbLinkedList.Location = new System.Drawing.Point(16, 41);
            this.lstbLinkedList.Name = "lstbLinkedList";
            this.lstbLinkedList.Size = new System.Drawing.Size(191, 384);
            this.lstbLinkedList.TabIndex = 6;
            // 
            // bttndisplay
            // 
            this.bttndisplay.Location = new System.Drawing.Point(223, 41);
            this.bttndisplay.Name = "bttndisplay";
            this.bttndisplay.Size = new System.Drawing.Size(90, 30);
            this.bttndisplay.TabIndex = 2;
            this.bttndisplay.Text = "Display All";
            this.bttndisplay.UseVisualStyleBackColor = true;
            this.bttndisplay.Click += new System.EventHandler(this.bttndisplay_Click);
            // 
            // Linkedlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "Linkedlist";
            this.Size = new System.Drawing.Size(859, 600);
            this.Load += new System.EventHandler(this.Linkedlist_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.cmbRemove.ResumeLayout(false);
            this.cmbAdd.ResumeLayout(false);
            this.cmbAdd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bttndisplay;
        private System.Windows.Forms.Button bttnInsert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Llist;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ListBox lstbLinkedList;
        private System.Windows.Forms.GroupBox cmbAdd;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox cmbRemove;
        private System.Windows.Forms.ComboBox cmdRemove;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbAddBeforeAfter;
        private System.Windows.Forms.TextBox txtValue2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInsertBeforeAfter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCurrent;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label label5;
    }
}