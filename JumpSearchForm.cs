using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public partial class JumpSearchForm : Form
    {
        private int[] values = new int[0];
        private List<SearchStep> steps = new List<SearchStep>();
        private int nextStep;
        private SearchStep current;

        public JumpSearchForm()
        {
            InitializeComponent();
            ActiveControl = txtNumbers;
        }

        private bool Prepare()
        {
            string[] inputs = txtNumbers.Text.Split(',');
            if (inputs.Length > 40)
            {
                lblStatus.Text = "Use up to 40 numbers so every value stays readable.";
                return false;
            }
            int[] parsed = new int[inputs.Length];
            int target;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (!int.TryParse(inputs[i].Trim(), out parsed[i]))
                {
                    lblStatus.Text = "Enter whole numbers separated by commas.";
                    txtNumbers.Focus();
                    return false;
                }
                if (i > 0 && parsed[i] < parsed[i - 1])
                {
                    lblStatus.Text = "Numbers must be sorted from smallest to largest.";
                    txtNumbers.Focus();
                    return false;
                }
            }
            if (!int.TryParse(txtTarget.Text, out target))
            {
                lblStatus.Text = "Enter a whole number as the target.";
                txtTarget.Focus();
                return false;
            }
            values = parsed;
            pnlAnimation.SetValues(values);
            steps = SearchAlgorithms.CreateSteps(values, target, "Jump Search");
            nextStep = 0;
            current = null;
            SetInputs(false);
            return true;
        }

        private void SetInputs(bool enabled)
        {
            txtNumbers.Enabled = enabled;
            txtTarget.Enabled = enabled;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (nextStep >= steps.Count && !Prepare()) return;
            pnlAnimation.ResumeAnimation();
            tmrSearch.Start();
            if (current == null) ShowNextStep();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            tmrSearch.Stop();
            pnlAnimation.PauseAnimation();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            tmrSearch.Stop();
            if (nextStep >= steps.Count && !Prepare()) return;
            ShowNextStep();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tmrSearch.Stop();
            values = new int[0];
            pnlAnimation.SetValues(values);
            steps.Clear();
            nextStep = 0;
            current = null;
            SetInputs(true);
            lblStatus.Text = "Enter sorted numbers and a target, then press Start.";
            pnlAnimation.Invalidate();
            txtNumbers.Focus();
        }

        private void tmrSearch_Tick(object sender, EventArgs e) { ShowNextStep(); }

        private void ShowNextStep()
        {
            if (nextStep >= steps.Count) return;
            current = steps[nextStep++];
            pnlAnimation.ShowStep(current.Index, current.Found, (int)(tmrSearch.Interval * 0.8));
            lblStatus.Text = "Step " + nextStep + "/" + steps.Count + " - " + current.Message;
            pnlAnimation.Invalidate();
            if (nextStep == steps.Count)
            {
                tmrSearch.Stop();
                SetInputs(true);
            }
        }

        private void speed_ValueChanged(object sender, EventArgs e)
        {
            tmrSearch.Interval = (int)speed.Value;
        }

    }
}
