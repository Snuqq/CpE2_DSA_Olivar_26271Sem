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
        private readonly Font titleFont = new Font("Segoe UI", 17F, FontStyle.Bold);
        private readonly Font bodyFont = new Font("Segoe UI", 10F);
        private readonly Font emphasisFont = new Font("Segoe UI", 12F, FontStyle.Bold);

        public InfographicForm()
        {
            InitializeComponent();
            var tabs = new TabControl { Dock = DockStyle.Fill, Name = "infographicTabs", Font = bodyFont, Padding = new Point(16, 6) };
            var overview = new TabPage("Compare") { BackColor = Color.White };
            Controls.Remove(content);
            overview.Controls.Add(content);
            tabs.TabPages.Add(overview);
            Controls.Add(tabs);
            heading.Font = emphasisFont;
            heading.Text = "One target. Three search strategies.\nFind 13 in the sorted array: 1, 3, 5, 7, 9, 11, 13, 15, 17, 19";
            content.RowStyles[0].Height = 64;
            AddOverview(tabs, 1, "Jump", "Jump by blocks, then scan.", JumpSearchForm.CreateSteps(ExampleValues, Target));
            AddOverview(tabs, 2, "Exponential", "Double the boundary, then halve the range.", ExponentialSearchForm.CreateSteps(ExampleValues, Target));
            AddOverview(tabs, 3, "Fibonacci", "Narrow the range using Fibonacci sizes.", FibonacciSearchForm.CreateSteps(ExampleValues, Target));
        }

        private Label TextLabel(string text, Font font)
        {
            return new Label { Text = text, Dock = DockStyle.Fill, Margin = Padding.Empty,
                ForeColor = Ink, Font = font, TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false };
        }

        private void AddOverview(TabControl tabs, int row, string title, string description, List<SearchStep> steps)
        {
            steps.RemoveAll(step => step.Terminal);
            var section = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2,
                Padding = new Padding(12, 4, 12, 4), Margin = new Padding(0, 4, 0, 4), BackColor = Paper };
            section.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 156));
            section.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            section.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            section.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            var name = TextLabel(title, emphasisFont);
            name.ForeColor = Accent;
            section.Controls.Add(name, 0, 0);
            section.Controls.Add(TextLabel(description, bodyFont), 1, 0);
            var explore = new LinkLabel { Text = "Learn & try →", Dock = DockStyle.Fill, Font = bodyFont,
                TextAlign = ContentAlignment.MiddleLeft, LinkColor = Accent, ActiveLinkColor = Color.Maroon, TabIndex = row };
            explore.LinkClicked += delegate { tabs.SelectedIndex = row; };
            section.Controls.Add(explore, 0, 1);
            string path = "";
            foreach (SearchStep step in steps)
                path += (path.Length == 0 ? "Values:  " : "  →  ") + ExampleValues[step.Index];
            section.Controls.Add(TextLabel(path + "   •   " + steps.Count + " checks", bodyFont), 1, 1);
            content.Controls.Add(section, 0, row);
            AddWalkthrough(tabs, title, description, steps);
        }

        private void AddWalkthrough(TabControl tabs, string title, string description, List<SearchStep> steps)
        {
            var page = new TabPage(title) { BackColor = Color.White, Padding = new Padding(12, 8, 12, 4) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Margin = Padding.Empty };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 31));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            header.Controls.Add(TextLabel(title + " search", titleFont), 0, 0);
            var strategy = TextLabel(description, bodyFont); strategy.Name = "strategy";
            header.Controls.Add(strategy, 0, 1);
            var target = TextLabel("Target  " + Target, emphasisFont);
            target.BackColor = Paper;
            target.TextAlign = ContentAlignment.MiddleCenter;
            header.Controls.Add(target, 1, 0);
            header.SetRowSpan(target, 2);
            var diagram = new ProbeDiagram(steps) { Dock = DockStyle.Fill, Margin = Padding.Empty,
                AccessibleName = title + " search diagram", Name = "diagram" };
            var decision = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Paper, Padding = new Padding(12, 4, 12, 4), Margin = new Padding(0, 4, 0, 2) };
            decision.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            decision.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            decision.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            decision.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            var comparison = TextLabel("", emphasisFont); comparison.Name = "comparison";
            var explanation = TextLabel("", bodyFont); explanation.Name = "decision";
            var detail = TextLabel("", bodyFont); detail.Name = "detail";
            decision.Controls.Add(comparison, 0, 0);
            decision.Controls.Add(explanation, 0, 1);
            decision.Controls.Add(detail, 0, 2);
            var controls = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0, 6, 0, 0) };
            controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
            controls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
            var previous = new Button { Text = "Previous", Dock = DockStyle.Fill, Font = bodyFont, Margin = Padding.Empty };
            var next = new Button { Text = "Next", Dock = DockStyle.Fill, Font = bodyFont, Margin = Padding.Empty };
            var counter = TextLabel("", bodyFont); counter.TextAlign = ContentAlignment.MiddleCenter;
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
            controls.Controls.Add(previous, 0, 0); controls.Controls.Add(counter, 1, 0); controls.Controls.Add(next, 2, 0);
            layout.Controls.Add(header, 0, 0); layout.Controls.Add(diagram, 0, 1);
            layout.Controls.Add(decision, 0, 2); layout.Controls.Add(controls, 0, 3);
            Control about = null;
            Action showExample = delegate { about.Visible = false; layout.Visible = true; layout.BringToFront(); previous.Select(); };
            about = CreateAbout(title, showExample);
            var aboutLink = new LinkLabel { Name = "lnkAbout", Text = "About", Font = bodyFont, Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft, LinkColor = Accent };
            aboutLink.LinkClicked += delegate { layout.Visible = false; about.Visible = true; about.BringToFront(); about.Controls.Find("btnTryExample", true)[0].Select(); };
            controls.ColumnCount = 4;
            controls.ColumnStyles.Insert(0, new ColumnStyle(SizeType.Absolute, 70));
            controls.SetColumn(previous, 1); controls.SetColumn(counter, 2); controls.SetColumn(next, 3);
            controls.Controls.Add(aboutLink, 0, 0);
            layout.Name = "walkthrough";
            layout.Visible = false;
            page.Controls.Add(layout); page.Controls.Add(about); tabs.TabPages.Add(page);
            refresh();
        }

        private Control CreateAbout(string title, Action showExample)
        {
            string description, credit, method, uses, cost, sourceLabel, sourceUrl;
            if (title == "Jump")
            {
                description = "Jump search finds a value in sorted data by skipping equal-sized blocks.\nWhen it reaches a block that could contain the target, it checks that block one value at a time.";
                credit = "Historical contribution: Ben Shneiderman • 1978";
                method = "1. Choose a block size near √n.\n2. Check the end of each block.\n3. If a value passes the target, scan that block.\n4. Return the index, or report not found.";
                uses = "Useful for block-based or sequential access.\nIt skips groups instead of checking every value.\nOn random-access arrays, binary search usually\nneeds fewer comparisons.";
                cost = "O(√n): the work grows roughly with the square root of the array size.";
                sourceLabel = "Read Shneiderman's 1978 paper ↗";
                sourceUrl = "https://www.cs.umd.edu/~ben/papers/Shneiderman1978Jump.pdf";
            }
            else if (title == "Exponential")
            {
                description = "Exponential search finds a value in sorted data by doubling the search boundary.\nOnce it brackets the target, it uses binary search to halve the remaining range repeatedly.";
                credit = "Credited to Jon Bentley & Andrew Chi-Chih Yao • 1976";
                method = "1. Check the first value.\n2. Expand through indexes 1, 2, 4, 8, ...\n3. Return immediately on a match.\n4. Once past the target, binary-search that range.";
                uses = "Useful when the upper bound is unknown\nor the target is near the beginning.\nIt also works on a regular sorted array.\nFinding the range adds some overhead.";
                cost = "O(log n): the work grows slowly as the array gets larger.";
                sourceLabel = "Read Bentley & Yao's 1976 paper ↗";
                sourceUrl = "https://doi.org/10.1016/0020-0190(76)90071-5";
            }
            else
            {
                description = "Fibonacci search finds a value in sorted data using Fibonacci numbers to split the range.\nAfter each comparison, it keeps the possible side. The offset marks the last discarded index on the left.";
                credit = "Inventor: David E. Ferguson • 1960";
                method = "1. Find a Fibonacci size covering the array.\n2. Split using smaller Fibonacci numbers.\n3. Compare, then keep only the possible range.\n4. Repeat until found or no candidates remain.";
                uses = "An alternative to binary search on sorted arrays.\nIts search positions use addition and subtraction.\nThe splits are unequal, unlike binary search.\nIt is not automatically faster than binary search.";
                cost = "O(log n): smaller Fibonacci sizes quickly reduce the search range.";
                sourceLabel = "Read NIST's entry & Ferguson reference ↗";
                sourceUrl = "https://xlinux.nist.gov/dads/HTML/fibonaccianSearch.html";
            }

            var about = new TableLayoutPanel { Name = "aboutAlgorithm", Dock = DockStyle.Fill,
                ColumnCount = 1, RowCount = 6, Margin = Padding.Empty };
            about.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            about.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            about.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            about.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            about.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            about.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            about.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            about.Controls.Add(TextLabel("About " + title.ToLowerInvariant() + " search", titleFont), 0, 0);
            var summary = TextLabel(description, bodyFont);
            summary.Name = "algorithmDescription";
            summary.TextAlign = ContentAlignment.TopLeft;
            about.Controls.Add(summary, 0, 1);
            var author = TextLabel(credit, emphasisFont); author.ForeColor = Accent;
            author.BackColor = Paper;
            author.Padding = new Padding(10, 0, 10, 0);
            about.Controls.Add(author, 0, 2);
            var lesson = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2,
                Margin = new Padding(0, 4, 0, 4) };
            lesson.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            lesson.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            lesson.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            lesson.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lesson.Controls.Add(TextLabel("How it works", emphasisFont), 0, 0);
            lesson.Controls.Add(TextLabel("Uses & limitations", emphasisFont), 1, 0);
            var methodLabel = TextLabel(method, bodyFont); methodLabel.TextAlign = ContentAlignment.TopLeft;
            var usesLabel = TextLabel(uses, bodyFont); usesLabel.TextAlign = ContentAlignment.TopLeft;
            lesson.Controls.Add(methodLabel, 0, 1); lesson.Controls.Add(usesLabel, 1, 1);
            about.Controls.Add(lesson, 0, 3);
            var facts = TextLabel("Requires sorted data • n = number of values\nWorst-case search time: " + cost, bodyFont);
            facts.BackColor = Paper;
            facts.Padding = new Padding(10, 0, 10, 0);
            about.Controls.Add(facts, 0, 4);
            var actions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                Margin = new Padding(0, 5, 0, 0) };
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            var source = new LinkLabel { Text = sourceLabel, Dock = DockStyle.Fill, Font = bodyFont,
                LinkColor = Accent, TextAlign = ContentAlignment.MiddleLeft, AccessibleDescription = sourceUrl };
            source.LinkClicked += delegate {
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sourceUrl) { UseShellExecute = true }); }
                catch (System.ComponentModel.Win32Exception) { System.Windows.Forms.MessageBox.Show("Open this source in your browser:\n" + sourceUrl); }
            };
            var tryExample = new Button { Name = "btnTryExample", Text = "Try the example →", Dock = DockStyle.Fill,
                Font = bodyFont, Margin = Padding.Empty };
            tryExample.Click += delegate { showExample(); };
            actions.Controls.Add(source, 0, 0); actions.Controls.Add(tryExample, 1, 0);
            about.Controls.Add(actions, 0, 5);
            var host = new Panel { Name = "aboutAlgorithm", Dock = DockStyle.Fill };
            about.Name = "basicLesson";
            Control historyLesson = null;
            Action showBasics = delegate { historyLesson.Visible = false; about.Visible = true; about.BringToFront(); };
            historyLesson = CreateHistoryLesson(title, credit, sourceUrl, showBasics, showExample);
            historyLesson.Visible = false;
            var historyButton = new Button { Name = "btnHistory", Text = "History & real use", Dock = DockStyle.Fill,
                Font = bodyFont, Margin = new Padding(0, 0, 6, 0) };
            historyButton.Click += delegate { about.Visible = false; historyLesson.Visible = true; historyLesson.BringToFront(); };
            actions.ColumnCount = 3;
            actions.ColumnStyles.Insert(1, new ColumnStyle(SizeType.Absolute, 160));
            actions.SetColumn(tryExample, 2);
            actions.Controls.Add(historyButton, 1, 0);
            host.Controls.Add(about); host.Controls.Add(historyLesson);
            return host;
        }

        private Control CreateHistoryLesson(string title, string credit, string sourceUrl, Action showBasics, Action showExample)
        {
            string problem, idea, example, benefit, note;
            if (title == "Jump")
            {
                problem = "Shneiderman studied sequential files where\nbinary search was awkward or unavailable.\nReading records one by one required too\nmany comparisons.";
                idea = "Skip blocks, then scan just one block.\nSmall blocks mean many jumps; large blocks\nmean a long final scan. Balancing these costs\ngives a jump size near √n for equal-cost checks.";
                example = "Documented use: IBM VSAM indexes.\nShneiderman described compressed keys of\nvarying lengths. Jump pointers let the search\nskip groups, then scan the selected group.";
                benefit = "Skip irrelevant keys instead of testing all of them.\nIllustration: 100 sorted records, blocks of 10;\nabout 10 boundary checks plus at most 9 scans.\nFast block access is needed for this saving.";
                note = "Shneiderman analyzed an existing technique. His paper does not establish a single original inventor.";
            }
            else if (title == "Exponential")
            {
                problem = "Bentley and Yao studied ordered tables with\nno known upper bound. Ordinary binary search\nneeds a finite search range before it can\nchoose a middle position.";
                idea = "Grow a boundary: 1, 2, 4, 8, 16, ...\nOnce it passes the target, the last two bounds\ncontain the answer. Binary search can finish\nwithout first measuring the entire table.";
                example = "Illustration: a sorted, timestamped log that\nallows indexed reads, but has no known count.\nProbe farther ahead until the timestamp passes\nthe one wanted, then binary-search that interval.";
                benefit = "Find nearby records without scanning the log\nor knowing its total length. The work depends\non how far away the target is. A forward-only\nstream cannot provide those indexed reads.";
                note = "1975 technical report; journal publication in 1976. The log scenario is illustrative.";
                sourceUrl = "https://www.slac.stanford.edu/pubs/slacpubs/1500/slac-pub-1679.pdf";
            }
            else
            {
                problem = "Ferguson's 1960 paper offered another way\nto search ordered data. The design question:\nhow can a search shrink its range while using\nsimple arithmetic to calculate positions?";
                idea = "A Fibonacci size is the sum of the two before it:\n13 = 8 + 5. After a comparison, keep one part\nand reuse smaller sizes. Search positions can\nbe updated with addition and subtraction.";
                example = "Illustration: a device with a sorted table of\nsensor codes. Fibonacci search can locate a\ncode without checking every entry, using\naddition and subtraction for its next position.";
                benefit = "Fewer checks than a full linear scan as the\ntable grows. Simple arithmetic may help on\nsuitable hardware, but binary search also has\ncheap operations. Measure before choosing.";
                note = "This explains the published method, not a personal discovery story. The device example is illustrative.";
            }
            var lesson = new TableLayoutPanel { Name = "historyLesson", Dock = DockStyle.Fill,
                ColumnCount = 1, RowCount = 5, Margin = Padding.Empty };
            lesson.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            lesson.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            lesson.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            lesson.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            lesson.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            lesson.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            lesson.Controls.Add(TextLabel(title + ": history & real use", titleFont), 0, 0);
            var attribution = TextLabel(credit, emphasisFont); attribution.ForeColor = Accent;
            lesson.Controls.Add(attribution, 0, 1);
            var columns = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Margin = Padding.Empty };
            columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            columns.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            columns.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            columns.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            columns.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            columns.Controls.Add(TextLabel("The problem", emphasisFont), 0, 0);
            columns.Controls.Add(TextLabel("Real-world example", emphasisFont), 1, 0);
            columns.Controls.Add(TextLabel("Why that idea works", emphasisFont), 0, 2);
            columns.Controls.Add(TextLabel("Benefit & limits", emphasisFont), 1, 2);
            string[] paragraphs = { problem, example, idea, benefit };
            for (int i = 0; i < paragraphs.Length; i++)
            {
                var paragraph = TextLabel(paragraphs[i], bodyFont);
                paragraph.TextAlign = ContentAlignment.TopLeft;
                columns.Controls.Add(paragraph, i % 2, i < 2 ? 1 : 3);
            }
            lesson.Controls.Add(columns, 0, 2);
            var context = TextLabel(note, bodyFont); context.BackColor = Paper;
            lesson.Controls.Add(context, 0, 3);
            var actions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0, 5, 0, 0) };
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            var source = new LinkLabel { Text = "Read the source ↗", Dock = DockStyle.Fill, Font = bodyFont,
                LinkColor = Accent, TextAlign = ContentAlignment.MiddleLeft, AccessibleDescription = sourceUrl };
            source.LinkClicked += delegate {
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sourceUrl) { UseShellExecute = true }); }
                catch (System.ComponentModel.Win32Exception) { System.Windows.Forms.MessageBox.Show("Open this source in your browser:\n" + sourceUrl); }
            };
            var basics = new Button { Name = "btnBasics", Text = "Back to basics", Dock = DockStyle.Fill, Font = bodyFont, Margin = new Padding(0, 0, 6, 0) };
            basics.Click += delegate { showBasics(); };
            var exampleButton = new Button { Name = "btnHistoryExample", Text = "Try the example →", Dock = DockStyle.Fill, Font = bodyFont, Margin = Padding.Empty };
            exampleButton.Click += delegate { showExample(); };
            actions.Controls.Add(source, 0, 0); actions.Controls.Add(basics, 1, 0); actions.Controls.Add(exampleButton, 2, 0);
            lesson.Controls.Add(actions, 0, 4);
            return lesson;
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

        private sealed class ProbeDiagram : Control
        {
            private readonly List<SearchStep> steps;
            public int Position;
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
