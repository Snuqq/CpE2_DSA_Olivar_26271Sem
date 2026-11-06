using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CpE2_DSA_Olivar_26271Sem;

class EmbeddedChecks
{
    static string last, error, capture;
    static string ProofRoot { get { return Environment.GetEnvironmentVariable("DSA_TEST_OUTPUT") ?? Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../obj")); } }
    static void Record(Form form, string name, string target, int seconds = 14)
    {
        last = error = null;
        string recording = null;
        using (form)
        {
            var host = Control<UnitySearchHost>(form, "pnlAnimation");
            host.Message += text => { if (text.StartsWith("STATE|")) last = text; if (text.StartsWith("ERROR|")) error = text; if (text.StartsWith("RECORDED|")) recording = text.Split('|')[2]; };
            form.Show(); Wait(() => host.Ready, "recording player ready");
            Control<NumericUpDown>(form, "speed").Value = 1500;
            Control<TextBox>(form, "txtNumbers").Text = "1,3,5,7,9,11,13,15,17,19";
            Control<TextBox>(form, "txtTarget").Text = target;
            host.Send("RECORD|0|" + seconds);
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => recording != null, "motion recording", seconds + 20);
            string output = ProofRoot;
            Directory.CreateDirectory(output);
            File.WriteAllText(Path.Combine(output, name + "-recording.txt"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer", recording));
            form.Close();
        }
        Pump(2000);
        Console.WriteLine("RECORDED: " + name);
    }
    static T Control<T>(Form form, string name) where T : Control { return (T)form.Controls.Find(name, true)[0]; }
    static void DenseLabels()
    {
        last = error = capture = null;
        using (var form = new FibonacciSearchForm())
        {
            var host = Control<UnitySearchHost>(form, "pnlAnimation");
            host.Message += text => { if (text.StartsWith("STATE|")) last = text; if (text.StartsWith("ERROR|")) error = text; if (text.StartsWith("CAPTURED|")) capture = text; };
            form.Show(); Wait(() => host.Ready, "dense-label player");
            Control<TextBox>(form, "txtNumbers").Text = string.Join(",", Enumerable.Range(0, 40).Select(i => int.MinValue + i));
            Control<TextBox>(form, "txtTarget").Text = int.MinValue.ToString();
            Control<NumericUpDown>(form, "speed").Value = 400;
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last != null && last.StartsWith("STATE|1|") && last.Split('|')[7] == "1", "dense-label result", 60);
            if (!last.Contains("Found at index 0")) throw new Exception("Dense integer array result was incorrect.");
            Capture(host, 1, "dense-integer-labels");
            var pointer = Cursor.Position;
            try
            {
                form.Activate(); host.Focus();
                Cursor.Position = host.PointToScreen(new System.Drawing.Point(50, 312));
                Pump(800);
                Capture(host, 1, "dense-integer-tooltip");
            }
            finally { Cursor.Position = pointer; }
            form.Close();
        }
        Pump(2000);
        Console.WriteLine("PASS: 40 full-width integers; inspect dense-integer-labels.png for label overflow");
    }
    static void Wait(Func<bool> condition, string why, int seconds = 45)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            Application.DoEvents(); Thread.Sleep(10);
            if (error != null) throw new Exception(error);
            if (watch.Elapsed.TotalSeconds > seconds) throw new Exception("Timeout: " + why + "; last event: " + last);
        }
    }
    static void Pump(int milliseconds)
    {
        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < milliseconds) { Application.DoEvents(); Thread.Sleep(10); }
    }
    static void Capture(UnitySearchHost host, int session, string name)
    {
        capture = null; host.Send("CAPTURE|" + session); Wait(() => capture != null, "frame capture");
        string output = Path.Combine(ProofRoot, "embedded-proof");
        Directory.CreateDirectory(output);
        File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer/SearchProof.png"), Path.Combine(output, name + ".png"), true);
        Console.WriteLine("CAPTURE: " + name);
    }
    static void Run(Form form, string name)
    {
        last = error = capture = null;
        using (form)
        {
            var host = Control<UnitySearchHost>(form, "pnlAnimation");
            host.Message += text => { if (text.StartsWith("STATE|")) last = text; if (text.StartsWith("ERROR|")) error = text; if (text.StartsWith("CAPTURED|")) capture = text; };
            form.StartPosition = FormStartPosition.CenterScreen; form.Show();
            Wait(() => host.Ready, "Unity handshake");
            IntPtr child = IntPtr.Zero;
            Wait(() => { EnumChildWindows(host.Handle, (window, data) => { child = window; return false; }, IntPtr.Zero); return child != IntPtr.Zero; }, "embedded child window");
            if (GetParent(child) != host.Handle) throw new Exception("Unity window is not parented to animation panel.");
            Pump(800); Capture(host, 0, name + "-ready");
            Control<NumericUpDown>(form, "speed").Value = 1800;
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last != null && last.StartsWith("STATE|1|0|1"), "first step");
            Pump(650); Control<Button>(form, "btnPause").PerformClick();
            Wait(() => last.Split('|')[4] == "1", "pause acknowledgement");
            Capture(host, 1, name + "-paused");
            byte[] frozen = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer/SearchProof.png"));
            string paused = last; Pump(800);
            if (last != paused) throw new Exception("Paused playback advanced.");
            Capture(host, 1, name + "-paused-later");
            if (!frozen.SequenceEqual(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer/SearchProof.png")))) throw new Exception("Paused rendered pose changed.");
            Control<Button>(form, "btnNext").PerformClick();
            Wait(() => last.Split('|')[3] == "0" && last.Split('|')[4] == "0", "single step completes", 10);
            Capture(host, 1, name + "-compare");
            Control<NumericUpDown>(form, "speed").Value = 400;
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last.Split('|')[7] == "1", "search completes", 30);
            if (!last.Contains("Found at index")) throw new Exception("Expected found result: " + last);
            Capture(host, 1, name + "-found");
            Control<Button>(form, "btnReset").PerformClick();
            Wait(() => last.StartsWith("STATE|2|-1|"), "reset");
            if (!Control<TextBox>(form, "txtNumbers").Enabled) throw new Exception("Reset left inputs disabled.");
            Control<TextBox>(form, "txtNumbers").Text = "3, 1";
            Control<Button>(form, "btnStart").PerformClick();
            if (!Control<Label>(form, "lblStatus").Text.Contains("Sort the numbers")) throw new Exception("Unsorted input accepted.");
            Control<TextBox>(form, "txtNumbers").Text = string.Join(",", Enumerable.Range(-20, 40));
            Control<TextBox>(form, "txtTarget").Text = "99";
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last.StartsWith("STATE|3|") && last.Split('|')[7] == "1", "missing target on 40 values", 60);
            if (!last.Contains("Target not found")) throw new Exception("Missing result was incorrect.");
            Capture(host, 3, name + "-40-missing");
            CheckPosition(8.4f, 0, -6.4f, 0);
            Control<TextBox>(form, "txtNumbers").Text = int.MinValue + ",0," + int.MaxValue;
            Control<TextBox>(form, "txtTarget").Text = int.MaxValue.ToString();
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last.StartsWith("STATE|4|") && last.Split('|')[7] == "1", "integer extremes", 20);
            Capture(host, 4, name + "-extremes");
            Control<TextBox>(form, "txtNumbers").Text = "7";
            Control<TextBox>(form, "txtTarget").Text = "7";
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last.StartsWith("STATE|5|") && last.Split('|')[7] == "1", "single value", 20);
            if (!last.Contains("Found at index 0")) throw new Exception("Single value did not match.");
            Capture(host, 5, name + "-single");
            CheckPosition(0, 0, 0, 0);
            Control<TextBox>(form, "txtNumbers").Text = string.Join(",", Enumerable.Range(0, 24).Select(i => i / 2 - 6));
            Control<TextBox>(form, "txtTarget").Text = "2";
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last.StartsWith("STATE|6|") && last.Split('|')[7] == "1", "24 values with duplicates", 40);
            if (!last.Contains("Found at index 16") && !last.Contains("Found at index 17")) throw new Exception("Duplicate result was incorrect.");
            Capture(host, 6, name + "-24-duplicates");
            host.Size = new System.Drawing.Size(900, 450); Pump(300);
            RECT bounds; GetClientRect(child, out bounds);
            if (bounds.Right != 900 || bounds.Bottom != 450) throw new Exception("Child failed to resize with its panel.");
            host.Size = new System.Drawing.Size(750, 390); Pump(300);
            uint pid; GetWindowThreadProcessId(child, out pid);
            Process.GetProcessById((int)pid).Kill();
            WaitForFailure(host);
            error = null;
            typeof(UnitySearchHost).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(host, new object[] { EventArgs.Empty });
            Wait(() => host.Ready, "retry after forced player exit");
            form.Close();
        }
        Pump(2000);
        Console.WriteLine("PASS: " + name + " embedding, controls, frozen pixels, 1/24/40 values, duplicates, integer extremes, resize, crash recovery, disposal");
    }
    static void WaitForFailure(UnitySearchHost host)
    {
        var watch = Stopwatch.StartNew();
        while (host.Ready && watch.Elapsed.TotalSeconds < 10) { Application.DoEvents(); Thread.Sleep(10); }
        if (host.Ready) throw new Exception("Player crash was not detected.");
    }
    static void CheckPosition(float actor, float camera, float actorZ, float cameraZ)
    {
        var parts = capture.Split('|');
        float actualActor = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
        float actualCamera = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
        float actualActorZ = float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);
        float actualCameraZ = float.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture);
        if (Math.Abs(actualActor - actor) > .01f || Math.Abs(actualCamera - camera) > .01f
            || Math.Abs(actualActorZ - actorZ) > .01f || Math.Abs(actualCameraZ - cameraZ) > .01f)
            throw new Exception("Stage position mismatch: " + capture + "; expected " + actor + ", " + camera);
    }
    static int PlayerId(UnitySearchHost host)
    {
        int result = 0;
        EnumChildWindows(host.Handle, (window, data) => { uint id; GetWindowThreadProcessId(window, out id); result = (int)id; return false; }, IntPtr.Zero);
        if (result == 0) throw new Exception("Embedded player window missing.");
        return result;
    }
    static bool Exited(int id)
    {
        try { using (var process = Process.GetProcessById(id)) return process.HasExited; }
        catch (ArgumentException) { return true; }
    }
    static void Lifecycle()
    {
        error = null;
        using (var shell = new Form1())
        {
            shell.Show();
            string[] buttons = { "btnJumpSearch", "btnExponentialSearch", "btnFibonacciSearch" };
            for (int i = 0; i < 20; i++)
            {
                Control<Button>(shell, buttons[i % buttons.Length]).PerformClick();
                var host = Control<UnitySearchHost>(shell, "pnlAnimation");
                Wait(() => host.Ready, "navigation " + i);
                int id = PlayerId(host);
                Control<Button>(shell, "btnArray").PerformClick();
                Wait(() => Exited(id), "navigation player disposal", 10);
            }
            shell.Close();
        }
        for (int i = 0; i < 5; i++)
        {
            using (var form = new JumpSearchForm()) { form.Show(); Pump(100); form.Close(); }
            Pump(2000);
        }
        if (Process.GetProcessesByName("FurinaSearch").Length != 0) throw new Exception("Closing during startup left a player.");
        string marker = Path.Combine(Path.GetTempPath(), "FurinaOwner-" + Guid.NewGuid().ToString("N") + ".txt");
        using (var owner = Process.Start(new ProcessStartInfo(Application.ExecutablePath, "crash-owner \"" + marker + "\"") { UseShellExecute = false }))
        {
            try
            {
                Wait(() => File.Exists(marker), "crash-test owner readiness");
                int player = int.Parse(File.ReadAllText(marker));
                owner.Kill(); owner.WaitForExit();
                Wait(() => Exited(player), "player exits after host crash", 10);
            }
            finally { if (!owner.HasExited) owner.Kill(); if (File.Exists(marker)) File.Delete(marker); }
        }
        Console.WriteLine("PASS: 20 actual shell navigation cycles, 5 startup closes, forced owner crash without orphan player");
    }
    static void FittedPlayer()
    {
        using (var shell = new Form1())
        {
            shell.Show();
            foreach (string algorithm in new[] { "Jump", "Exponential", "Fibonacci" })
            {
                last = error = capture = null;
                Control<Button>(shell, "btn" + algorithm + "Search").PerformClick();
                var host = Control<UnitySearchHost>(shell, "pnlAnimation");
                host.Message += text => {
                    if (text.StartsWith("STATE|")) last = text;
                    if (text.StartsWith("ERROR|")) error = text;
                    if (text.StartsWith("CAPTURED|")) capture = text;
                };
                Wait(() => host.Ready, "fitted " + algorithm + " player ready");
                Control<Button>(shell, "btnNext").PerformClick();
                Wait(() => last != null && last.StartsWith("STATE|1|0|"), "form-owned search plan accepted");
                host.Send("PAUSE|1");
                Wait(() => last.Split('|')[4] == "1", "fitted player pause");
                foreach (var state in new[] { FormWindowState.Normal, FormWindowState.Maximized })
                {
                    shell.WindowState = state;
                    if (state == FormWindowState.Normal) shell.ClientSize = new System.Drawing.Size(1040, 620);
                    Pump(500);
                    var playerBounds = host.RectangleToScreen(host.ClientRectangle);
                    for (Control parent = host.Parent; parent != null; parent = parent.Parent)
                    {
                        if (!parent.RectangleToScreen(parent.ClientRectangle).Contains(playerBounds))
                            throw new Exception(algorithm + " player clipped by " + parent.Name);
                        var scroll = parent as ScrollableControl;
                        if (scroll != null && (scroll.VerticalScroll.Visible || scroll.HorizontalScroll.Visible))
                            throw new Exception(algorithm + " still requires scrolling.");
                    }
                    IntPtr child = IntPtr.Zero;
                    EnumChildWindows(host.Handle, (window, data) => { child = window; return false; }, IntPtr.Zero);
                    RECT bounds; GetClientRect(child, out bounds);
                    if (bounds.Right != host.ClientSize.Width || bounds.Bottom != host.ClientSize.Height)
                        throw new Exception("Unity viewport does not fill the resized host.");
                    string name = "fitted-" + algorithm.ToLowerInvariant() + "-" + state.ToString().ToLowerInvariant();
                    Capture(host, 1, name + "-player");
                    shell.Activate(); Pump(150);
                    var client = shell.RectangleToScreen(shell.ClientRectangle);
                    using (var bitmap = new System.Drawing.Bitmap(client.Width, client.Height))
                    using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(client.Location, System.Drawing.Point.Empty, client.Size);
                        bitmap.Save(Path.Combine(ProofRoot, "embedded-proof", name + "-shell.png"));
                    }
                    Console.WriteLine("PASS: {0} {1}, full player {2}, no scrollbars", algorithm, state, host.ClientSize);
                }
            }
            shell.Close();
        }
        Pump(2000);
        if (Process.GetProcessesByName("FurinaSearch").Length != 0) throw new Exception("Fitted player remained after closing shell.");
    }
    sealed class PoseSample
    {
        public double X, Y, Z, Zoom, Elapsed, Drift, ResetProgress, IdleTime;
        public string Phase;
        public bool Resetting;
        public int Occupied;
        public static PoseSample Read(string message)
        {
            var fields = message.Split('|');
            if (fields.Length < 15) throw new Exception("Player is missing animation diagnostics; rebuild Unity: " + message);
            return new PoseSample { X = Number(fields[2]), Zoom = Number(fields[4]), Z = Number(fields[5]),
                Phase = fields[7], Elapsed = Number(fields[8]), Drift = Number(fields[9]), Y = Number(fields[10]),
                Resetting = fields[11] == "1", ResetProgress = Number(fields[12]), IdleTime = Number(fields[13]),
                Occupied = int.Parse(fields[14], System.Globalization.CultureInfo.InvariantCulture) };
        }
        public double Distance(PoseSample other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
        }
    }
    static double Number(string value)
    {
        double number = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        if (double.IsNaN(number) || double.IsInfinity(number)) throw new Exception("Non-finite diagnostic: " + value);
        return number;
    }
    static PoseSample Sample(UnitySearchHost host, int session, string name)
    {
        Capture(host, session, name);
        var pose = PoseSample.Read(capture);
        if (pose.Drift > .01) throw new Exception("Planted foot drift exceeded 1 cm: " + capture);
        return pose;
    }
    static byte[] ProofPixels()
    {
        return File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer/SearchProof.png"));
    }
    static bool StateIs(int session, int step, bool moving)
    {
        return last != null && last.StartsWith("STATE|" + session + "|" + step + "|" + (moving ? "1" : "0") + "|");
    }
    static void WaitReset(UnitySearchHost host, int session, string name)
    {
        var watch = Stopwatch.StartNew();
        while (Sample(host, session, name).Resetting)
        {
            if (watch.Elapsed.TotalSeconds > 8) throw new Exception("Reset return did not settle.");
            Pump(100);
        }
    }
    static void Animation()
    {
        Animation(1000);
        Animation(5000);
        Wait(() => {
            foreach (var process in Process.GetProcessesByName("FurinaSearch"))
                using (process) { if (!process.HasExited) return false; }
            return true;
        }, "all animation players exit", 10);
    }
    static void Animation(int milliseconds)
    {
        last = error = capture = null;
        string prefix = "animation-" + milliseconds;
        using (var form = new JumpSearchForm())
        {
            var host = Control<UnitySearchHost>(form, "pnlAnimation");
            host.Message += text => {
                if (text.StartsWith("STATE|")) last = text;
                if (text.StartsWith("ERROR|")) error = text;
                if (text.StartsWith("CAPTURED|")) capture = text;
            };
            form.Show(); Wait(() => host.Ready, "animation player ready");
            var idle = Sample(host, 0, prefix + "-idle-a");
            byte[] idlePixels = ProofPixels();
            Pump(700);
            var idleLater = Sample(host, 0, prefix + "-idle-b");
            if (idleLater.IdleTime <= idle.IdleTime || idlePixels.SequenceEqual(ProofPixels()))
                throw new Exception("Idle breathing and weight shift did not advance.");
            Control<NumericUpDown>(form, "speed").Value = milliseconds;
            Control<TextBox>(form, "txtNumbers").Text = "1 3 5 7 9 11 13 15 17 19";
            Control<TextBox>(form, "txtTarget").Text = "13";
            Control<Button>(form, "btnNext").PerformClick();
            Wait(() => StateIs(1, 0, true), "first animation beat");
            Pump(milliseconds == 5000 ? 900 : 350);
            host.Send("PAUSE|1");
            Wait(() => StateIs(1, 0, true) && last.Split('|')[4] == "1", "pause acknowledgement");
            var paused = Sample(host, 1, prefix + "-paused-a");
            byte[] pausedPixels = ProofPixels();
            Pump(450);
            var pausedLater = Sample(host, 1, prefix + "-paused-b");
            if (paused.Elapsed != pausedLater.Elapsed || paused.IdleTime != pausedLater.IdleTime
                || paused.Distance(pausedLater) > .00001 || !pausedPixels.SequenceEqual(ProofPixels()))
                throw new Exception("Pause changed the character, camera, idle, or effects.");
            if (milliseconds == 5000 && (paused.Phase != "Travel" || paused.Y <= idle.Y + .05))
                throw new Exception("Expected an elevated midair pose at the 5000 ms setting: " + capture);
            for (int i = 0; i < 20; i++) host.Send("STEP|1");
            Wait(() => StateIs(1, 0, false), "spam Next completes only the active beat", 12);
            Pump(350);
            if (!StateIs(1, 0, false)) throw new Exception("Next spam queued extra algorithm probes.");
            var landed = Sample(host, 1, prefix + "-landed");
            if (landed.Occupied != 2) throw new Exception("The first block probe did not land on index 2.");
            if (milliseconds == 5000 && Math.Abs(landed.Zoom - paused.Zoom) > .001)
                throw new Exception("The camera zoom changed after takeoff.");

            host.Send("STEP|1");
            Wait(() => StateIs(1, 1, true), "second animation beat");
            Pump(milliseconds == 5000 ? 900 : 350);
            host.Send("PAUSE|1");
            Wait(() => StateIs(1, 1, true) && last.Split('|')[4] == "1", "second airborne pause");
            var beforeReset = Sample(host, 1, prefix + "-before-reset");
            if (milliseconds == 5000)
            {
                host.Send("VIEW|1|wide"); Pump(150);
                var deferred = Sample(host, 1, prefix + "-deferred-view");
                if (Math.Abs(deferred.Zoom - beforeReset.Zoom) > .001 || deferred.Distance(beforeReset) > .00001)
                    throw new Exception("A view toggle changed the camera during a paused airborne pose.");
                host.Send("VIEW|1|follow");
            }
            Control<Button>(form, "btnReset").PerformClick();
            Wait(() => StateIs(2, -1, false), "reset clears search state immediately");
            var returning = Sample(host, 2, prefix + "-reset-return");
            if (!returning.Resetting || returning.ResetProgress < 0 || returning.ResetProgress >= 1)
                throw new Exception("Reset skipped its visible return: " + capture);
            double homeDistance = Math.Sqrt(Math.Pow(returning.X + 4.8, 2) + Math.Pow(returning.Z - 1.6, 2));
            if (homeDistance < .02 || returning.Distance(beforeReset) > 3)
                throw new Exception("Reset teleported to the starting platform.");
            WaitReset(host, 2, prefix + "-reset-settled");
            CheckPosition(-4.8f, 0, 1.6f, 0);
            if (!Control<TextBox>(form, "txtNumbers").Enabled) throw new Exception("Reset left input disabled.");

            Control<TextBox>(form, "txtTarget").Text = "12";
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => last != null && last.StartsWith("STATE|3|") && last.Split('|')[7] == "1", "not-found reaction and result hold", 60);
            if (!last.Contains("Target not found")) throw new Exception("Not-found result changed.");
            Sample(host, 3, prefix + "-not-found");

            Control<TextBox>(form, "txtTarget").Text = "13";
            Control<Button>(form, "btnStart").PerformClick();
            Wait(() => StateIs(4, 0, true), "reset/start race setup");
            Pump(milliseconds == 5000 ? 900 : 350);
            Control<Button>(form, "btnReset").PerformClick();
            Control<TextBox>(form, "txtNumbers").Text = "2 4 6 8 10 12 14 16 18 20";
            Control<TextBox>(form, "txtTarget").Text = "12";
            Control<Button>(form, "btnStart").PerformClick();
            host.Send("STEP|4"); host.Send("PLAY|4");
            Wait(() => last != null && last.StartsWith("STATE|6|") && last.Split('|')[7] == "1", "latest search starts after reset settles", 60);
            if (!last.Contains("Found at index 5")) throw new Exception("A stale session or pending load changed the result.");
            Sample(host, 6, prefix + "-found-after-reset");
            int player = PlayerId(host);
            form.Close(); Wait(() => Exited(player), "animation player disposal", 10);
        }
        Console.WriteLine("PASS: " + milliseconds + " ms idle, frozen Pause, Next spam, airborne reset, latest-session replay, found/not-found; review saved poses for artistic quality");
    }
    static void Motion() { Animation(); }
    static void Performance()
    {
        string output = Path.Combine(ProofRoot, "performance-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + ".csv");
        Directory.CreateDirectory(ProofRoot);
        using (var report = new StreamWriter(output))
        {
            report.WriteLine("algorithm,scenario,show_to_ready_ms,frames,sample_seconds,frame_median_ms,frame_p95_ms,frame_p99_ms,stage_mean_ms,stage_p95_ms,main_thread_alloc_bytes_per_frame,stage_alloc_bytes_per_frame,frames_over_33ms,frames_over_50ms,active_renderers,gpu,graphics_api,measured_fps,extra_diagnostics");
            Performance(new JumpSearchForm(), "jump", report);
            Performance(new ExponentialSearchForm(), "exponential", report);
            Performance(new FibonacciSearchForm(), "fibonacci", report);
        }
        Console.WriteLine("PERFORMANCE REPORT: " + output);
    }
    static void Performance(Form form, string name, StreamWriter report)
    {
        last = error = null;
        string sample = null;
        using (form)
        {
            var host = Control<UnitySearchHost>(form, "pnlAnimation");
            host.Message += text => {
                if (text.StartsWith("STATE|")) last = text;
                if (text.StartsWith("ERROR|")) error = text;
                if (text.StartsWith("PERFORMED|")) sample = text;
            };
            var startup = Stopwatch.StartNew();
            form.Show(); Wait(() => host.Ready, "performance player ready");
            startup.Stop();
            const int session = 100;
            int[] values = Enumerable.Range(0, 40).Select(i => i * 2 + 1).ToArray();
            var steps = form is JumpSearchForm ? JumpSearchForm.CreateSteps(values, 200) :
                form is ExponentialSearchForm ? ExponentialSearchForm.CreateSteps(values, 200) :
                FibonacciSearchForm.CreateSteps(values, 200);
            var plan = new SearchPlan { Values = values, Target = 200, Algorithm = host.Algorithm, Steps = steps };
            host.Send("LOAD|" + session + "|" + plan.Serialize());
            Wait(() => last != null && last.StartsWith("STATE|100|-1|"), "40-value performance setup");
            host.Send("PERF|100|idle40|5");
            Wait(() => sample != null, "idle performance sample", 20);
            WritePerformance(report, name, startup.Elapsed.TotalMilliseconds, sample);
            sample = null;
            host.Send("SPEED|100|3");
            host.Send("PERF|100|moving40|12");
            host.Send("PLAY|100");
            Wait(() => sample != null, "moving performance sample", 30);
            if (last == null || last.Split('|')[2] == "-1") throw new Exception("Performance playback never started.");
            WritePerformance(report, name, startup.Elapsed.TotalMilliseconds, sample);
            int player = PlayerId(host);
            form.Close();
            Wait(() => Exited(player), "performance player disposal", 10);
        }
        Console.WriteLine("MEASURED: " + name + " startup, idle40, moving40 without screenshot recording");
    }
    static void WritePerformance(StreamWriter report, string name, double startupMs, string sample)
    {
        string[] fields = sample.Split('|');
        if (fields.Length < 17 || fields[1] != "100") throw new Exception("Invalid performance response: " + sample);
        int frames = int.Parse(fields[3], System.Globalization.CultureInfo.InvariantCulture);
        if (frames < 1) throw new Exception("No performance samples: " + sample);
        for (int i = 4; i < 15; i++)
        {
            if ((i == 10 || i == 11) && fields[i] == "N/A") continue;
            double value = double.Parse(fields[i], System.Globalization.CultureInfo.InvariantCulture);
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0) throw new Exception("Invalid performance measurement: " + sample);
        }
        double seconds = Number(fields[4]);
        if (seconds <= 0) throw new Exception("Invalid performance sample duration: " + sample);
        double fps = frames / seconds;
        string line = Csv(name) + "," + Csv(fields[2]) + "," + startupMs.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)
            + "," + string.Join(",", fields.Skip(3).Take(14).Select(Csv))
            + "," + fps.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "," + Csv(string.Join("|", fields.Skip(17)));
        report.WriteLine(line); report.Flush(); Console.WriteLine(line);
        Console.WriteLine("MEASURED FPS: {0} {1}: {2:F2}; p95 {3} ms; frames over 50 ms: {4}", name, fields[2], fps, fields[6], fields[13]);
    }
    static string Csv(string value) { return "\"" + value.Replace("\"", "\"\"") + "\""; }
    [STAThread] static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            if (args.Length > 0 && args[0] == "crash-owner")
            {
                using (var owner = new JumpSearchForm())
                {
                    owner.Show(); var host = Control<UnitySearchHost>(owner, "pnlAnimation");
                    Wait(() => host.Ready, "owner player ready");
                    File.WriteAllText(args[1], PlayerId(host).ToString());
                    Application.Run(owner);
                }
                return 0;
            }
            if (args.Length > 0 && args[0] == "lifecycle") { Lifecycle(); return 0; }
            if (args.Length > 0 && args[0] == "fit") { FittedPlayer(); return 0; }
            if (args.Length > 0 && args[0] == "record")
            {
                Record(new JumpSearchForm(), "jump", "13");
                Record(new ExponentialSearchForm(), "exponential", "13");
                Record(new FibonacciSearchForm(), "fibonacci-missing", "12");
                return 0;
            }
            if (args.Length > 0 && args[0] == "revision-record")
            {
                Record(new JumpSearchForm(), "revision-final-jump", "13", 24);
                Record(new ExponentialSearchForm(), "revision-final-exponential", "13", 24);
                Record(new FibonacciSearchForm(), "revision-final-fibonacci-missing", "12", 24);
                return 0;
            }
            if (args.Length > 0 && args[0] == "polish-record")
            {
                string run = "polish-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                Record(new JumpSearchForm(), run + "-jump", "13", 28);
                Record(new ExponentialSearchForm(), run + "-exponential", "13", 28);
                Record(new FibonacciSearchForm(), run + "-fibonacci-missing", "12", 28);
                File.WriteAllLines(Path.Combine(ProofRoot, "latest-polish-recordings.txt"),
                    new[] { run + "-jump", run + "-exponential", run + "-fibonacci-missing" });
                return 0;
            }
            if (args.Length > 0 && args[0] == "animation") { Animation(); return 0; }
            if (args.Length > 0 && args[0] == "motion") { Motion(); return 0; }
            if (args.Length > 0 && args[0] == "performance") { Performance(); return 0; }
            if (args.Length > 0 && args[0] == "labels") { DenseLabels(); return 0; }
            Run(new FibonacciSearchForm(), "fibonacci");
            if (args.Length == 0 || args[0] != "smoke") { Run(new JumpSearchForm(), "jump"); Run(new ExponentialSearchForm(), "exponential"); }
            if (Process.GetProcessesByName("FurinaSearch").Length != 0) throw new Exception("Player remained after form disposal.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    delegate bool Callback(IntPtr window, IntPtr data);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr parent, Callback callback, IntPtr data);
    [DllImport("user32.dll")] static extern IntPtr GetParent(IntPtr window);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr window, out uint id);
    [DllImport("user32.dll")] static extern bool GetClientRect(IntPtr window, out RECT rect);
    [StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
}
