namespace CpE2_DSA_Olivar_26271Sem
{
    partial class Form1
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
            this.btnArray = new System.Windows.Forms.Button();
            this.btnLinkedList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnArray
            // 
            this.btnArray.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArray.Location = new System.Drawing.Point(12, 12);
            this.btnArray.Name = "btnArray";
            this.btnArray.Size = new System.Drawing.Size(99, 32);
            this.btnArray.TabIndex = 0;
            this.btnArray.Text = "Array";
            this.btnArray.UseVisualStyleBackColor = true;
            this.btnArray.Click += new System.EventHandler(this.btnArray_Click);
            // 
            // btnLinkedList
            // 
            this.btnLinkedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLinkedList.Location = new System.Drawing.Point(12, 50);
            this.btnLinkedList.Name = "btnLinkedList";
            this.btnLinkedList.Size = new System.Drawing.Size(126, 32);
            this.btnLinkedList.TabIndex = 1;
            this.btnLinkedList.Text = "LinkedList";
            this.btnLinkedList.UseVisualStyleBackColor = true;
            this.btnLinkedList.Click += new System.EventHandler(this.btnLinkedList_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 110);
            this.Controls.Add(this.btnLinkedList);
            this.Controls.Add(this.btnArray);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnArray;
        private System.Windows.Forms.Button btnLinkedList;
    }
}