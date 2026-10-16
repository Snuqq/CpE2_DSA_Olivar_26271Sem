using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CpE2_DSA_Olivar_26271Sem;
class AnimationChecks
{
    static BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static object Field(object panel, string name) { return panel.GetType().GetField(name, flags).GetValue(panel); }
    static void Tick(object panel) { panel.GetType().GetMethod("Animate", flags).Invoke(panel, new object[] { null, EventArgs.Empty }); }
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    [STAThread] static void Main()
    {
        var panel = new SearchAnimationPanel { Size = new Size(750, 390) };
        panel.SetValues(new[] { 1, 3, 5, 7 });
        panel.ShowStep(0, false, 100); Thread.Sleep(120); Tick(panel);
        var first = (PointF)Field(panel, "marker");
        panel.ShowStep(3, false, 200); Thread.Sleep(70); Tick(panel);
        var middle = (PointF)Field(panel, "marker");
        Check(middle.X > first.X && middle.X < 321, "Indicator must move between values");
        panel.PauseAnimation(); var paused = (PointF)Field(panel, "marker");
        Thread.Sleep(80); Tick(panel); Check(((PointF)Field(panel, "marker")).Equals(paused), "Pause must freeze motion");
        panel.ResumeAnimation(); Thread.Sleep(180); Tick(panel);
        Check((float)Field(panel, "progress") == 1, "Resume must complete movement");
        panel.ShowStep(3, true, 100); Thread.Sleep(120); Tick(panel);
        using(var bitmap = new Bitmap(750, 390)) panel.DrawToBitmap(bitmap, new Rectangle(0, 0, 750, 390));
        panel.ShowStep(-1, false, 100);
        Check(!((System.Windows.Forms.Timer)Field(panel, "animationTimer")).Enabled, "Missing result must stop motion");
        panel.SetValues(new int[0]); Check((int)Field(panel, "activeIndex") == -1, "Reset must clear highlight");
        var timer = (System.Windows.Forms.Timer)Field(panel, "animationTimer"); panel.Dispose();
        Console.WriteLine("PASS: Motion interpolation, pause/resume, rendering, not-found and reset.");
    }
}
