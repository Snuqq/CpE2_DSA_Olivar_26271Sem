using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class InfographicForm : Form
    {
        private static readonly int[] ExampleValues = { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
        private const int Target = 13;
        private static readonly Color Ink = Color.FromArgb(48, 40, 38);
        private static readonly Color Accent = Color.FromArgb(122, 28, 40);
        private static readonly Color Paper = Color.FromArgb(250, 247, 240);
        private static readonly Color Checking = Color.FromArgb(255, 190, 108);
        private static readonly Color Found = Color.FromArgb(245, 207, 88);
        private static readonly Color RuledOut = Color.FromArgb(226, 225, 222);
        public InfographicForm()
        {
            InitializeComponent();
            var tabs = (TabControl)Controls.Find("infographicTabs", true)[0];
            WireLesson(tabs.TabPages[1], "Jump", JumpSearchForm.CreateSteps(ExampleValues, Target));
            WireLesson(tabs.TabPages[2], "Exponential", ExponentialSearchForm.CreateSteps(ExampleValues, Target));
            WireLesson(tabs.TabPages[3], "Fibonacci", FibonacciSearchForm.CreateSteps(ExampleValues, Target));
            int index = 1;
            foreach (Control control in Descendants(tabs.TabPages[0]))
            {
                var link = control as LinkLabel;
                if (link == null) continue;
                int destination = index++;
                link.LinkClicked += delegate { tabs.SelectedIndex = destination; };
            }
        }

        private static IEnumerable<Control> Descendants(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (Control nested in Descendants(child)) yield return nested;
            }
        }

        private static T Find<T>(Control parent, string name) where T : Control
        {
            return (T)parent.Controls.Find(name, true)[0];
        }

        private static Button ButtonWithText(Control parent, string text)
        {
            foreach (Control control in Descendants(parent))
                if (control is Button && control.Text == text) return (Button)control;
            throw new InvalidOperationException("Missing button: " + text);
        }

        private void WireLesson(TabPage page, string title, List<SearchStep> steps)
        {
            steps.RemoveAll(step => step.Terminal);
            var layout = Find<TableLayoutPanel>(page, "walkthrough");
            var about = Find<Panel>(page, "aboutAlgorithm");
            var basics = Find<TableLayoutPanel>(page, "basicLesson");
            var history = Find<TableLayoutPanel>(page, "historyLesson");
            var diagram = Find<ProbeDiagram>(page, "diagram");
            diagram.SetSteps(steps);
            var strategy = Find<Label>(layout, "strategy");
            var comparison = Find<Label>(layout, "comparison");
            var explanation = Find<Label>(layout, "decision");
            var detail = Find<Label>(layout, "detail");
            var previous = ButtonWithText(layout, "Previous");
            var next = ButtonWithText(layout, "Next");
            Label counter = null;
            foreach (Control control in Descendants(layout))
                if (control is Label && control.Text.StartsWith("Step ")) counter = (Label)control;
            Action refresh = delegate {
                int position = diagram.Position;
                SearchStep step = steps[position];
                int value = ExampleValues[step.Index];
                bool found = value == Target;
                if (title == "Fibonacci")
                {
                    int offset = -1;
                    for (int i = 0; i < position; i++)
                        if (ExampleValues[steps[i].Index] < Target) offset = steps[i].Index;
                    strategy.Text = (position == 0 ? "10 items → use Fibonacci size 13 (8 + 5). " : "Use the next smaller split. ") +
                        "Check index " + offset + " + " + (step.Index - offset) + " = " + step.Index + ".";
                }
                comparison.Text = step.Comparison + "   •   " + (found ? "Found at index " + step.Index : value < Target ? "Too small" : "Too large");
                comparison.ForeColor = Accent;
                explanation.Text = DecisionText(title, steps, position);
                detail.Text = DetailText(title, steps, position);
                previous.Enabled = position > 0;
                next.Enabled = position < steps.Count - 1;
                counter.Text = "Step " + (position + 1) + " of " + steps.Count + "   •   " + (found ? "Search complete" : "Checking index " + step.Index);
                diagram.AccessibleDescription = counter.Text + ". " + comparison.Text + ". " + explanation.Text + ". " + detail.Text;
                diagram.Invalidate();
            };
            previous.Click += delegate { if (diagram.Position > 0) diagram.Position--; refresh(); };
            next.Click += delegate { if (diagram.Position < steps.Count - 1) diagram.Position++; refresh(); };
            Action showExample = delegate { about.Visible = false; layout.Visible = true; layout.BringToFront(); previous.Select(); };
            Find<Button>(basics, "btnTryExample").Click += delegate { showExample(); };
            Find<Button>(history, "btnHistoryExample").Click += delegate { showExample(); };
            Find<Button>(basics, "btnHistory").Click += delegate { basics.Visible = false; history.Visible = true; history.BringToFront(); };
            Find<Button>(history, "btnBasics").Click += delegate { history.Visible = false; basics.Visible = true; basics.BringToFront(); };
            Find<LinkLabel>(layout, "lnkAbout").LinkClicked += delegate {
                layout.Visible = false; about.Visible = true; about.BringToFront();
                if (basics.Visible) Find<Button>(basics, "btnTryExample").Select();
                else Find<Button>(history, "btnHistoryExample").Select();
            };
            foreach (Control control in Descendants(about))
            {
                var link = control as LinkLabel;
                if (link == null || string.IsNullOrEmpty(link.AccessibleDescription)) continue;
                string url = link.AccessibleDescription;
                link.LinkClicked += delegate {
                    try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); }
                    catch (System.ComponentModel.Win32Exception) { MessageBox.Show("Open this source in your browser:\n" + url); }
                };
            }
            layout.Visible = false;
            history.Visible = false;
            basics.Visible = true;
            about.BringToFront();
            refresh();
        }

        private static string DecisionText(string title, List<SearchStep> steps, int position)
        {
            SearchStep current = steps[position];
            int value = ExampleValues[current.Index];
            if (value == Target) return "The value matches the target. Stop searching.";
            SearchStep next = steps[position + 1];
            if (title == "Jump")
                return value < Target ? "Skip the smaller values. Jump to index " + next.Index + " next."
                    : "We passed 13. Scan indexes " + next.Left + "–" + next.Right + ", starting at " + next.Index + ".";
            if (title == "Exponential")
                return position == 0 ? "The first value is too small. Start expanding at index 1." : value < Target ? "The target is farther right. Double the boundary to index " + next.Index + "."
                    : "The boundary passed 13. Binary-search indexes " + next.Left + "–" + next.Right + "; check " + next.Index + ".";
            return value < Target ? "Discard indexes 0–" + current.Index + ". Use a smaller Fibonacci size to move right."
                : "Discard index " + current.Index + " and everything after it. Search the smaller left range.";
        }

        private static string DetailText(string title, List<SearchStep> steps, int position)
        {
            SearchStep current = steps[position];
            if (title == "Fibonacci")
            {
                int offset = -1;
                for (int i = 0; i < position; i++)
                    if (ExampleValues[steps[i].Index] < Target) offset = steps[i].Index;
                int after = ExampleValues[current.Index] < Target ? current.Index : offset;
                return "Offset " + offset + " → " + after + "  |  Last index ruled out as too small; -1 means none.";
            }
            if (ExampleValues[current.Index] == Target)
                return "Found value " + Target + " at index " + current.Index + " after " + steps.Count + " comparisons. Indexes start at 0.";
            if (title == "Jump") return "Block size: floor(√10) = 3. Check indexes 2, 5, 8, then scan the chosen block.";
            return "Expand through indexes 0, 1, 2, 4, 8. Then check the middle of the remaining range.";
        }

        public sealed class ProbeDiagram : Control
        {
            private List<SearchStep> steps;
            public int Position;
            public ProbeDiagram() : this(JumpSearchForm.CreateSteps(ExampleValues, Target)) { }

            internal void SetSteps(List<SearchStep> value) { steps = value; Position = 0; Invalidate(); }

            public ProbeDiagram(List<SearchStep> steps)
            {
                this.steps = steps;
                DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float scale = g.DpiX / 96F;
                var state = g.Save();
                g.ScaleTransform(scale, scale);
                float availableWidth = ClientSize.Width / scale;
                float availableHeight = ClientSize.Height / scale;
                float top = Math.Max(0, (availableHeight - 148) / 2);
                float cellWidth = (availableWidth - 54) / ExampleValues.Length;
                float badgeY = top + 51, tileY = top + 80;
                SearchStep current = steps[Position];
                bool found = ExampleValues[current.Index] == Target;
                int left = found ? current.Index : steps[Position + 1].Left;
                int right = found ? current.Index : steps[Position + 1].Right;
                using (var small = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var number = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var badgeFont = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var pen = new Pen(Accent, 2))
                using (var arrow = new AdjustableArrowCap(4, 4))
                using (var centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var inkBrush = new SolidBrush(Ink))
                using (var accentBrush = new SolidBrush(Accent))
                {
                    Legend(g, small, 4, top, Checking, "Checking");
                    Legend(g, small, 116, top, Found, "Found");
                    Legend(g, small, 208, top, Paper, "Still possible");
                    Legend(g, small, 346, top, RuledOut, "Ruled out");
                    if (Position > 0)
                    {
                        // Only the latest move is drawn; badges retain the earlier probe order.
                        float from = 52 + (steps[Position - 1].Index + .5F) * cellWidth - 3;
                        float to = 52 + (current.Index + .5F) * cellWidth - 3;
                        pen.CustomEndCap = arrow;
                        g.DrawBezier(pen, from, badgeY - 6, from, top + 17, to, top + 17, to, badgeY - 6);
                    }
                    for (int i = 0; i < ExampleValues.Length; i++)
                    {
                        float x = 52 + i * cellWidth;
                        var rect = new RectangleF(x, tileY, cellWidth - 6, 42);
                        Color color = i < left || i > right ? RuledOut : Paper;
                        if (i == current.Index) color = found ? Found : Checking;
                        using (var brush = new SolidBrush(color)) g.FillRectangle(brush, rect);
                        g.DrawString(ExampleValues[i].ToString(), number, inkBrush, rect, centered);
                        g.DrawString(i.ToString(), small, inkBrush, new RectangleF(x, tileY + 44, rect.Width, 20), centered);
                        for (int p = 0; p <= Position; p++)
                            if (steps[p].Index == i)
                            {
                                var badge = new RectangleF(x + rect.Width / 2 - 11, badgeY, 22, 22);
                                g.FillEllipse(p == Position ? accentBrush : Brushes.White, badge);
                                using (var outline = new Pen(Accent)) g.DrawEllipse(outline, badge);
                                g.DrawString((p + 1).ToString(), badgeFont, p == Position ? Brushes.White : accentBrush, badge, centered);
                            }
                    }
                    g.DrawString("Value", small, inkBrush, new PointF(0, tileY + 13));
                    g.DrawString("Index", small, inkBrush, new PointF(0, tileY + 47));
                }
                g.Restore(state);
            }

            private static void Legend(Graphics g, Font font, float x, float y, Color color, string text)
            {
                using (var brush = new SolidBrush(color)) g.FillRectangle(brush, x, y + 3, 11, 11);
                using (var brush = new SolidBrush(Ink)) g.DrawString(text, font, brush, x + 17, y);
            }
        }
    }
}
