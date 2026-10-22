using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class JumpSearchForm : Form
    {
        bool loaded;
        int session;

        public JumpSearchForm()
        {
            InitializeComponent();
            pnlAnimation.Algorithm = "Jump Search";
            pnlAnimation.Message += Receive;
            btnStart.Enabled = btnPause.Enabled = btnNext.Enabled = false;
            ActiveControl = txtNumbers;
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
            var tokens = txtNumbers.Text.Split(',');
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
                    lblStatus.Text = "Enter whole numbers separated by commas.";
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
            txtNumbers.Enabled = txtTarget.Enabled = false;
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
            txtNumbers.Enabled = txtTarget.Enabled = true;
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
                txtNumbers.Enabled = txtTarget.Enabled = true;
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
                txtNumbers.Enabled = txtTarget.Enabled = true;
                txtTarget.Focus();
                txtTarget.SelectAll();
            }
        }

        // The form computes the search; Unity displays these steps in the same order.
        public static List<SearchStep> CreateSteps(int[] values, int target)
        {
            SearchSteps.ValidateValues(values);
            var steps = new List<SearchStep>();
            int result = JumpSearch(values, target, steps);
            SearchSteps.Complete(values, target, result, steps);
            return steps;
        }

        static int JumpSearch(int[] values, int target, List<SearchStep> steps)
        {
            if (values.Length == 0) return -1;
            int blockSize = Math.Max(1, (int)Math.Sqrt(values.Length));
            int left = 0, right = values.Length - 1;
            int blockEnd = Math.Min(blockSize - 1, right);
            while (left <= right)
            {
                int comparison = SearchSteps.Probe(values, target, blockEnd, left, right, steps, "Jump: block boundary",
                    "Block size = floor(sqrt(" + values.Length + ")) = " + blockSize + ". Check block end " + blockEnd + ".");
                if (comparison == 0) return blockEnd;
                if (comparison > 0)
                {
                    right = blockEnd - 1;
                    break;
                }
                left = blockEnd + 1;
                blockEnd = Math.Min(blockEnd + blockSize, right);
            }
            for (int index = left; index <= right; index++)
            {
                int comparison = SearchSteps.Probe(values, target, index, index, right, steps, "Jump: linear scan",
                    "The block boundary passed the target. Scan this block from left to right; check index " + index + ".");
                if (comparison == 0) return index;
                if (comparison > 0) break;
            }
            return -1;
        }
    }
}
