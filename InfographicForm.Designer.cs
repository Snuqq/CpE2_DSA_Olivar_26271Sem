namespace CpE2_DSA_Olivar_26271Sem
{
    partial class InfographicForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel content;
        private System.Windows.Forms.Label heading;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) components.Dispose();
                titleFont.Dispose(); bodyFont.Dispose(); emphasisFont.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.content = new System.Windows.Forms.TableLayoutPanel();
            this.heading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.ColumnCount = 1;
            this.content.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.content.RowCount = 4;
            this.content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.content.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.content.Name = "content";
            this.heading.Text = "THREE WAYS TO FIND 13\r\nSorted array • Indexes start at 0 • Gold = found value";
            this.heading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.heading.ForeColor = System.Drawing.Color.Maroon;
            this.heading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.heading.Name = "heading";
            this.content.Controls.Add(this.heading, 0, 0);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(810, 471);
            this.Controls.Add(this.content);
            this.Name = "InfographicForm";
            this.Text = "Search Algorithms Infographic";
            this.ResumeLayout(false);
        }
    }
}
