using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class ExponentialSearchForm : Form
    {
        bool loaded;
        int session;
        readonly Random random = new Random();

        public ExponentialSearchForm()
        {
            InitializeComponent();
            new ToolTip(components).SetToolTip(speed, "Speed changes apply to the next step.");
            txtNumbers.Multiline = true;
            txtNumbers.WordWrap = false;
            txtNumbers.TextChanged += (sender, e) =>
            {
                if (txtNumbers.Text.IndexOfAny(new[] { '\r', '\n', '\t' }) < 0) return;
                txtNumbers.Text = txtNumbers.Text.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
                txtNumbers.SelectionStart = txtNumbers.TextLength;
            };
            pnlAnimation.Algorithm = "Exponential Search";
            pnlAnimation.Message += Receive;
            btnStart.Enabled = btnPause.Enabled = btnNext.Enabled = false;
            ActiveControl = txtNumbers;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (loaded) return;
            int[] values = new int[(int)numberCount.Value];
            int value = 0;
            for (int i = 0; i < values.Length; i++)
            {
                value += random.Next(1, 11);
                values[i] = value;
            }
            txtNumbers.Text = string.Join(", ", values);
            txtTarget.Text = values[values.Length / 2].ToString();
            lblStatus.Text = "Generated " + values.Length + " sorted numbers. Change the target or press Start.";
            txtTarget.Focus();
            txtTarget.SelectAll();
        }

        private void speed_ValueChanged(object sender, EventArgs e)
        {
            if (pnlAnimation != null) ChangeSpeed();
        }

        bool Prepare()
        {
            if (!pnlAnimation.Ready)
            {
                lblStatus.Text = "Wait for the Unity theatre to load.";
                return false;
            }
            var tokens = txtNumbers.Text.Split(new[] { ',', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int[] values = new int[tokens.Length];
            int targetValue;
            if (tokens.Length > 40)
            {
                lblStatus.Text = "Use 1–40 sorted whole numbers.";
                return false;
            }
            for (int i = 0; i < tokens.Length; i++)
            {
                if (!int.TryParse(tokens[i].Trim(), out values[i]))
                {
                    lblStatus.Text = "Separate whole numbers with spaces or commas.";
                    txtNumbers.Focus();
                    return false;
                }
                if (i > 0 && values[i] < values[i-1])
                {
                    lblStatus.Text = "Sort the numbers from smallest to largest first.";
                    txtNumbers.Focus();
                    return false;
                }
            }
            if (!int.TryParse(txtTarget.Text, out targetValue))
            {
                lblStatus.Text = "Enter a whole number target.";
                txtTarget.Focus();
                return false;
            }
            session++;
            var plan = new SearchPlan { Values = values, Target = targetValue, Algorithm = pnlAnimation.Algorithm,
                Steps = CreateSteps(values, targetValue) };
            pnlAnimation.Send("LOAD|" + session + "|" + plan.Serialize());
            ChangeSpeed();
            loaded = true;
            txtNumbers.Enabled = txtTarget.Enabled = btnGenerate.Enabled = numberCount.Enabled = false;
            return true;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (loaded || Prepare()) pnlAnimation.Send("PLAY|" + session);
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (loaded) pnlAnimation.Send("PAUSE|" + session);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (loaded || Prepare()) pnlAnimation.Send("STEP|" + session);
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            session++;
            loaded = false;
            txtNumbers.Enabled = txtTarget.Enabled = btnGenerate.Enabled = numberCount.Enabled = true;
            btnStart.Text = "Start";
            btnPause.Enabled = false;
            if (pnlAnimation.Ready) pnlAnimation.Send("RESET|" + session);
            lblStatus.Text = "Enter sorted numbers and a target, then press Start.";
            txtNumbers.Focus();
        }
        public void ChangeSpeed()
        {
            if (pnlAnimation.Ready) pnlAnimation.Send("SPEED|" + session + "|" + (speed.Value / 1000m).ToString(CultureInfo.InvariantCulture));
        }
        void Receive(string text)
        {
            if (text == "READY")
            {
                btnStart.Enabled = btnNext.Enabled = true;
                btnPause.Enabled = false;
                lblStatus.Text = "Ready. Press Start or Next Step.";
                txtNumbers.Focus();
                return;
            }
            if (text.StartsWith("ERROR|"))
            {
                loaded = false;
                txtNumbers.Enabled = txtTarget.Enabled = btnGenerate.Enabled = numberCount.Enabled = true;
                btnStart.Enabled = btnPause.Enabled = btnNext.Enabled = false;
                lblStatus.Text = text.Substring(6);
                return;
            }
            var parts = text.Split('|');
            int id;
            if (parts.Length < 9 || parts[0] != "STATE" || !int.TryParse(parts[1], out id) || id != session) return;
            bool moving = parts[3] == "1";
            bool paused = parts[4] == "1";
            bool playing = parts[5] == "1";
            bool finished = parts[7] == "1";

            lblStatus.Text = parts[8];
            btnStart.Text = paused ? "Resume" : "Start";
            btnPause.Enabled = !paused && (moving || playing);
            if (finished)
            {
                loaded = false;
                txtNumbers.Enabled = txtTarget.Enabled = btnGenerate.Enabled = numberCount.Enabled = true;
                txtTarget.Focus();
                txtTarget.SelectAll();
            }
        }

        // The form computes the search; Unity displays these steps in the same order.
        public static List<SearchStep> CreateSteps(int[] values, int target)
        {
            SearchSteps.ValidateValues(values);
            var steps = new List<SearchStep>();
            int result = ExponentialSearch(values, target, steps);
            SearchSteps.Complete(values, target, result, steps);
            return steps;
        }

        static int ExponentialSearch(int[] values, int target, List<SearchStep> steps)
        {
            if (values.Length == 0) return -1;
            int comparison = SearchSteps.Probe(values, target, 0, 0, values.Length - 1, steps, "Exponential: first value", "Check index 0 before expanding the boundary.");
            if (comparison == 0) return 0;
            if (comparison > 0) return -1;
            int left = 1, right = values.Length - 1;
            long boundary = 1;
            while (boundary < values.Length)
            {
                int index = (int)boundary;
                comparison = SearchSteps.Probe(values, target, index, left, right, steps, "Exponential: double boundary",
                    "Double the boundary while values are below the target: 1, 2, 4, 8... Probe index " + boundary + ".");
                if (comparison == 0) return index;
                if (comparison > 0)
                {
                    right = index - 1;
                    break;
                }
                left = index + 1;
                boundary *= 2;
            }
            while (left <= right)
            {
                int middle = left + (right - left) / 2;
                comparison = SearchSteps.Probe(values, target, middle, left, right, steps, "Exponential: binary search",
                    "The boundary is known. Halve the remaining range.\nmid = " + left + " + floor((" + right + " - " + left + ") / 2) = " + middle + ".");
                if (comparison == 0) return middle;
                if (comparison < 0)
                    left = middle + 1;
                else
                    right = middle - 1;
            }
            return -1;
        }
    }
}
