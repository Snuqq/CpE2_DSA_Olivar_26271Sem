using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public sealed class UnitySearchHost : Panel
    {
        Process player;
        NamedPipeServerStream pipe;
        BlockingCollection<string> outgoing;
        IntPtr child;
        readonly Timer startup = new Timer { Interval = 100 };
        DateTime launched;
        string status = "Furina's search theatre loads when you run the application.";
        bool stopped, ready, failed, sceneLoaded;
        int generation;
        public event Action<string> Message;
        public string Algorithm { get; set; }
        public bool Ready { get { return ready; } }
        public UnitySearchHost()
        {
            Algorithm = "Fibonacci Search";
            BackColor = Color.FromArgb(12, 20, 43); ForeColor = Color.Gold;
            DoubleBuffered = true;
            startup.Tick += CheckWindow;
            AccessibleName = "Unity search animation";
        }
        bool IsDesigner { get { return DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            Process.GetCurrentProcess().ProcessName.IndexOf("Design", StringComparison.OrdinalIgnoreCase) >= 0 ||
            Environment.GetEnvironmentVariable("DSA_DISABLE_UNITY") == "1"; } }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && IsHandleCreated && !IsDesigner && player == null && !failed) Launch();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Visible && !IsDesigner && player == null && !failed) Launch();
        }
        void Launch()
        {
            string exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnityPlayer", "FurinaSearch.exe");
            if (!File.Exists(exe)) { Fail("Unity player is missing. Run scripts/BuildSearchApp.ps1, then reopen this form."); return; }
            try
            {
                stopped = false; failed = false; ready = false; sceneLoaded = false;
                string name = "FurinaSearch-" + Guid.NewGuid().ToString("N");
                var security = new PipeSecurity();
                security.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));
                pipe = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 4096, 4096, security);
                outgoing = new BlockingCollection<string>(128);
                player = Process.Start(new ProcessStartInfo(exe, "-parentHWND " + Handle.ToInt64() +
                    " delayed -screen-fullscreen 0 -screen-width " + Math.Max(1, Width) + " -screen-height " + Math.Max(1, Height) +
                    " -searchPipe " + name + " -searchParent " + Process.GetCurrentProcess().Id +
                    " -searchAlgorithm \"" + Algorithm + "\"" +
                    " -logFile \"" + Path.Combine(Path.GetTempPath(), name + ".log") + "\"") {
                    UseShellExecute = false, WorkingDirectory = Path.GetDirectoryName(exe) });
                var connectedPipe = pipe;
                var queue = outgoing;
                int version = ++generation;
                Task.Run(() => ReadLoop(connectedPipe, queue, version));
                launched = DateTime.UtcNow;
                status = "Opening Furina's theatre..."; Invalidate(); startup.Start();
            }
            catch (Exception ex) { Fail("Could not start Unity: " + ex.Message); StopPlayer(); }
        }
        void ReadLoop(NamedPipeServerStream connection, BlockingCollection<string> queue, int version)
        {
            try
            {
                connection.WaitForConnection();
                if (SearchWire.Read(connection) != "HELLO|" + SearchWire.Version) throw new IOException("Unity player version mismatch. Rebuild the player.");
                Task.Run(() => {
                    try { foreach (string command in queue.GetConsumingEnumerable()) SearchWire.Write(connection, command); }
                    catch (Exception ex) { Post(() => Fail("Unity connection closed: " + ex.Message), version); }
                });
                while (!stopped)
                {
                    string response = SearchWire.Read(connection);
                    Post(() => {
                        if (response == "LOADED") { sceneLoaded = true; MarkReady(); }
                        else if (response.StartsWith("ERROR|")) Fail(response.Substring(6));
                        else if (Message != null) Message(response);
                    }, version);
                }
            }
            catch (Exception ex) { Post(() => Fail("Unity connection closed: " + ex.Message), version); }
        }
        public void Send(string command)
        {
            if (outgoing == null || outgoing.IsAddingCompleted || !outgoing.TryAdd(command))
                Fail("Unity is unavailable. Click the animation area to retry.");
        }
        void Post(Action action, int version)
        {
            if (stopped || IsDisposed || !IsHandleCreated) return;
            try { BeginInvoke(new Action(() => { if (!stopped && !IsDisposed && generation == version) action(); })); }
            catch (InvalidOperationException) { }
        }
        void Fail(string message)
        {
            StopPlayer();
            failed = true; ready = false; status = message; Invalidate();
            if (Message != null) Message("ERROR|" + message);
        }
        void CheckWindow(object sender, EventArgs e)
        {
            if (player == null) return;
            if (player.HasExited) { Fail("Unity stopped. Click here to retry."); StopPlayer(); return; }
            if ((!ready || child == IntPtr.Zero) && (DateTime.UtcNow - launched).TotalSeconds > 45)
            { Fail("Unity did not become ready. Click here to retry."); return; }
            if (child == IntPtr.Zero)
            {
                EnumWindows((window, data) => {
                    uint id; GetWindowThreadProcessId(window, out id);
                    if (id == player.Id && GetWindowLongPtr(window, -21).ToInt64() % 2 != 0) child = window;
                    return true;
                }, IntPtr.Zero);
                // With -parentHWND Unity may already have created a child window.
                if (child == IntPtr.Zero) EnumChildWindows(Handle, (window, data) => {
                    uint id; GetWindowThreadProcessId(window, out id);
                    if (id == player.Id && (GetWindowLongPtr(window, -21).ToInt64() & 1) != 0) child = window;
                    return true;
                }, IntPtr.Zero);
                if (child != IntPtr.Zero)
                {
                    long style = GetWindowLongPtr(child, -16).ToInt64();
                    SetWindowLongPtr(child, -16, new IntPtr((style & ~0x80000000L & ~0x00C00000L) | 0x40000000L));
                    SetParent(child, Handle); ResizePlayer(); ShowWindow(child, 5);
                    MarkReady();
                }
            }
        }
        void MarkReady()
        {
            if (ready || !sceneLoaded || child == IntPtr.Zero) return;
            ready = true; status = "Ready";
            if (Message != null) Message("READY");
        }
        protected override void OnResize(EventArgs e) { base.OnResize(e); ResizePlayer(); }
        void ResizePlayer() { if (child != IntPtr.Zero) MoveWindow(child, 0, 0, Math.Max(1, ClientSize.Width), Math.Max(1, ClientSize.Height), true); }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            TextRenderer.DrawText(e.Graphics, status, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }
        protected override void OnClick(EventArgs e) { base.OnClick(e); if (failed && !IsDesigner) { StopPlayer(); Launch(); } }
        void StopPlayer()
        {
            stopped = true; generation++; ready = false; startup.Stop(); child = IntPtr.Zero;
            if (outgoing != null) outgoing.CompleteAdding();
            if (pipe != null) { pipe.Dispose(); pipe = null; }
            var owned = player; player = null;
            if (owned != null) Task.Run(() => {
                try { if (!owned.WaitForExit(1500)) owned.Kill(); }
                catch (InvalidOperationException) { }
                finally { owned.Dispose(); }
            });
        }
        protected override void Dispose(bool disposing) { if (disposing) { StopPlayer(); startup.Dispose(); } base.Dispose(disposing); }
        delegate bool WindowCallback(IntPtr window, IntPtr data);
        [DllImport("user32.dll")] static extern bool EnumWindows(WindowCallback callback, IntPtr data);
        [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr parent, WindowCallback callback, IntPtr data);
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr window, out uint id);
        [DllImport("user32.dll", EntryPoint="GetWindowLongPtrW")] static extern IntPtr GetLong64(IntPtr window, int index);
        [DllImport("user32.dll", EntryPoint="GetWindowLongW")] static extern int GetLong32(IntPtr window, int index);
        static IntPtr GetWindowLongPtr(IntPtr window, int index) { return IntPtr.Size == 8 ? GetLong64(window, index) : new IntPtr(GetLong32(window, index)); }
        [DllImport("user32.dll", EntryPoint="SetWindowLongPtrW")] static extern IntPtr SetLong64(IntPtr window, int index, IntPtr value);
        [DllImport("user32.dll", EntryPoint="SetWindowLongW")] static extern int SetLong32(IntPtr window, int index, int value);
        static void SetWindowLongPtr(IntPtr window, int index, IntPtr value) { if (IntPtr.Size == 8) SetLong64(window, index, value); else SetLong32(window, index, unchecked((int)value.ToInt64())); }
        [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr window, IntPtr parent);
        [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr window, int x, int y, int w, int h, bool repaint);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr window, int command);
    }
}
