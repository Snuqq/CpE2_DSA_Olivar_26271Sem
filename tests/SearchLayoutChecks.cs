using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CpE2_DSA_Olivar_26271Sem;

class SearchLayoutChecks
{
    static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Field<T>(object owner, string name) { return (T)owner.GetType().GetField(name, Fields).GetValue(owner); }
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }

    [STAThread] static int Main()
    {
        Environment.SetEnvironmentVariable("DSA_DISABLE_UNITY", "1");
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try
        {
            foreach (float scale in new[] { 1f, 1.25f })
                using (var shell = new Form1())
                {
                    shell.WindowState = FormWindowState.Normal;
                    if (scale != 1f) shell.Scale(new SizeF(scale, scale));
                    foreach (var size in new[] { new Size(1366, 704), new Size((int)(1040 * scale), (int)(620 * scale)), new Size(1600, 900), new Size(1366, 704) })
                    {
                        shell.ClientSize = size;
                        shell.PerformLayout();
                        var main = Field<SplitContainer>(shell, "splitContainer1");
                        var content = Field<SplitContainer>(shell, "splitContainer2").Panel2;
                        Check(shell.ClientRectangle.Contains(main.Bounds), "School shell extends outside client area: " + main.Bounds + " in " + shell.ClientRectangle + ", max " + main.MaximumSize);
                        Check(Math.Abs(main.Left * 2 + main.Width - shell.ClientSize.Width) <= 1, "School shell is not centered horizontally: " + main.Bounds + " in " + shell.ClientRectangle + ", scale " + scale);
                        Check(Math.Abs(main.Top * 2 + main.Height - shell.ClientSize.Height) <= 1, "School shell is not centered vertically.");
                        var navigation = Field<TableLayoutPanel>(shell, "navigation");
                        Check(navigation.Parent.ClientRectangle.Contains(navigation.Bounds), "Navigation panel is clipped: " + navigation.Bounds + " in " + navigation.Parent.ClientRectangle);
                        foreach (Control button in navigation.Controls)
                            Check(navigation.ClientRectangle.Contains(button.Bounds) && (!(button is Button) || button.Height >= 25), "Navigation button is clipped: " + button.Text);
                        foreach (var algorithm in new[] { "JumpSearch", "ExponentialSearch", "FibonacciSearch" })
                        {
                            var form = (Form)Activator.CreateInstance(typeof(Form1).Assembly.GetType("CpE2_DSA_Olivar_26271Sem." + algorithm + "Form"));
                            // Explicit scaling simulates increased system fonts without changing the desktop DPI.
                            if (scale != 1f)
                            {
                                form.Scale(new SizeF(scale, scale));
                            }
                            shell.GetType().GetMethod("formShow", Fields).Invoke(shell, new object[] { form, content });
                            form.PerformLayout();
                            Check(!form.AutoScroll && !form.HorizontalScroll.Visible && !form.VerticalScroll.Visible, algorithm + " has scrolling.");
                            var player = Field<Control>(form, "pnlAnimation");
                            Check(form.ClientRectangle.Contains(player.Bounds), algorithm + " player is clipped: " + player.Bounds + " within " + form.ClientRectangle);
                            Check(player.Height >= 250 && player.Width >= 700, algorithm + " player is too small.");
                            foreach (Control control in form.Controls)
                            {
                                Check(form.ClientRectangle.Contains(control.Bounds), algorithm + " control is clipped: " + control.Name);
                                if (control != player) Check(control.Bottom <= player.Top, algorithm + " control overlaps player: " + control.Name);
                            }
                            Console.WriteLine("PASS: {0}, scale {1}, client {2}, player {3}", algorithm, scale, shell.ClientSize, player.Size);
                        }
                    }
                }
            return 0;
        }
        catch (Exception error) { Console.WriteLine("FAIL: " + error.GetBaseException().Message); return 1; }
    }
}
