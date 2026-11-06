using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;

// Opt-in sampling: no screenshots, per-frame strings, or profiler allocations.
public sealed class SearchPerformanceProbe
{
    const int Capacity = 16384;
    readonly double[] frameTimes = new double[Capacity];
    readonly double[] stageTimes = new double[Capacity];
    Action<string> complete;
    string label;
    int session, count, over33, over50;
    double requestedSeconds, startSeconds, previousFrameSeconds, frameSeconds, stageTotal;
    long previousAllocated, frameAllocated, frameStamp, totalAllocated, stageAllocated;
    bool active, sampleFrame, allocationCounterAvailable;
    int focusedFrames, renderedFrames, highestRenderInterval;
    double totalFrameSeconds;
    public bool LogEvidence { get; set; }

    public void Begin(int sessionId, string sampleLabel, double seconds, Action<string> onComplete, double warmupSeconds = 1)
    {
        if (active) throw new InvalidOperationException("A performance sample is already running.");
        if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 1 || seconds > 60)
            throw new ArgumentOutOfRangeException("seconds", "Sample for 1–60 seconds.");
        if (double.IsNaN(warmupSeconds) || double.IsInfinity(warmupSeconds) || warmupSeconds < 0 || warmupSeconds > 60)
            throw new ArgumentOutOfRangeException("warmupSeconds");
        if (string.IsNullOrWhiteSpace(sampleLabel) || sampleLabel.IndexOfAny(new[] { '|', '\r', '\n' }) >= 0)
            throw new ArgumentException("Provide a single-line sample label.", "sampleLabel");
        if (onComplete == null) throw new ArgumentNullException("onComplete");
        long allocationBefore = GC.GetAllocatedBytesForCurrentThread();
        var sentinel = new byte[4096];
        long allocationAfter = GC.GetAllocatedBytesForCurrentThread();
        GC.KeepAlive(sentinel);
        allocationCounterAvailable = allocationAfter - allocationBefore >= sentinel.Length;
        session = sessionId; label = sampleLabel; requestedSeconds = seconds; complete = onComplete;
        count = over33 = over50 = 0;
        focusedFrames = renderedFrames = highestRenderInterval = 0;
        totalFrameSeconds = 0;
        totalAllocated = stageAllocated = 0; stageTotal = 0;
        startSeconds = Time.realtimeSinceStartupAsDouble + warmupSeconds;
        previousFrameSeconds = 0; sampleFrame = false; active = true;
        if (LogEvidence)
            UnityEngine.Debug.Log(string.Format(CultureInfo.InvariantCulture,
                "Frame pacing BEGIN {0}: target={1}, vsync={2}, renderInterval={3}, refreshHz={4:F3}, focused={5}, background={6}, captureRate={7}, embedded={8}",
                label, Application.targetFrameRate, QualitySettings.vSyncCount,
                UnityEngine.Rendering.OnDemandRendering.renderFrameInterval,
                Screen.currentResolution.refreshRateRatio.value, Application.isFocused, Application.runInBackground,
                Time.captureFramerate, Array.IndexOf(Environment.GetCommandLineArgs(), "-parentHWND") >= 0));
    }

    public void BeginFrame()
    {
        if (!active) return;
        frameSeconds = Time.realtimeSinceStartupAsDouble;
        long allocated = GC.GetAllocatedBytesForCurrentThread();
        sampleFrame = frameSeconds >= startSeconds && previousFrameSeconds >= startSeconds;
        if (sampleFrame)
        {
            double milliseconds = (frameSeconds - previousFrameSeconds) * 1000;
            frameTimes[count] = milliseconds;
            totalFrameSeconds += milliseconds / 1000;
            if (Application.isFocused) focusedFrames++;
            if (UnityEngine.Rendering.OnDemandRendering.willCurrentFrameRender) renderedFrames++;
            highestRenderInterval = Math.Max(highestRenderInterval, UnityEngine.Rendering.OnDemandRendering.renderFrameInterval);
            totalAllocated += Math.Max(0, allocated - previousAllocated);
            if (milliseconds > 1000d / 30) over33++;
            if (milliseconds > 50) over50++;
        }
        previousFrameSeconds = frameSeconds; previousAllocated = allocated;
        frameAllocated = GC.GetAllocatedBytesForCurrentThread();
        frameStamp = Stopwatch.GetTimestamp();
    }

    public void EndFrame()
    {
        if (!active || !sampleFrame) return;
        double milliseconds = (Stopwatch.GetTimestamp() - frameStamp) * 1000d / Stopwatch.Frequency;
        stageAllocated += Math.Max(0, GC.GetAllocatedBytesForCurrentThread() - frameAllocated);
        stageTimes[count] = milliseconds; stageTotal += milliseconds; count++;
        if (frameSeconds - startSeconds >= requestedSeconds || count == Capacity) Finish();
    }

    void Finish()
    {
        active = false;
        Array.Sort(frameTimes, 0, count);
        Array.Sort(stageTimes, 0, count);
        var renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        int visibleRenderers = 0;
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i].enabled && renderers[i].gameObject.activeInHierarchy) visibleRenderers++;
        string reply = string.Join("|", new[] {
            "PERFORMED", session.ToString(CultureInfo.InvariantCulture), label,
            count.ToString(CultureInfo.InvariantCulture), Number(frameSeconds - startSeconds),
            Number(Percentile(frameTimes, count, .5)), Number(Percentile(frameTimes, count, .95)),
            Number(Percentile(frameTimes, count, .99)), Number(stageTotal / count),
            Number(Percentile(stageTimes, count, .95)), allocationCounterAvailable ? Number(totalAllocated / (double)count) : "N/A",
            allocationCounterAvailable ? Number(stageAllocated / (double)count) : "N/A", over33.ToString(CultureInfo.InvariantCulture),
            over50.ToString(CultureInfo.InvariantCulture), visibleRenderers.ToString(CultureInfo.InvariantCulture),
            SystemInfo.graphicsDeviceName.Replace('|', '/'), SystemInfo.graphicsDeviceType.ToString(),
            "focusedFrames=" + focusedFrames.ToString(CultureInfo.InvariantCulture),
            "requestedRenders=" + renderedFrames.ToString(CultureInfo.InvariantCulture),
            "renderInterval=" + highestRenderInterval.ToString(CultureInfo.InvariantCulture),
            "vsync=" + QualitySettings.vSyncCount.ToString(CultureInfo.InvariantCulture),
            "target=" + Application.targetFrameRate.ToString(CultureInfo.InvariantCulture),
            "background=" + Application.runInBackground
        });
        if (LogEvidence)
            UnityEngine.Debug.Log(string.Format(CultureInfo.InvariantCulture,
                "Frame pacing END {0}: updates={1}, requestedRenders={2}, averageFps={3:F2}, medianMs={4:F3}, p95Ms={5:F3}, p99Ms={6:F3}, focusedFrames={7}, maxRenderInterval={8}, vsync={9}, target={10}. Measure screenshot capture overhead separately.",
                label, count, renderedFrames, count / Math.Max(.000001, totalFrameSeconds),
                Percentile(frameTimes, count, .5), Percentile(frameTimes, count, .95), Percentile(frameTimes, count, .99),
                focusedFrames, highestRenderInterval, QualitySettings.vSyncCount, Application.targetFrameRate));
        var callback = complete; complete = null; callback(reply);
    }

    public static double Percentile(double[] sorted, int length, double fraction)
    {
        if (sorted == null || length < 1 || length > sorted.Length || fraction < 0 || fraction > 1 || double.IsNaN(fraction))
            throw new ArgumentOutOfRangeException("length", "Provide sorted samples and a percentile from 0 to 1.");
        double at = (length - 1) * fraction;
        int low = (int)Math.Floor(at), high = (int)Math.Ceiling(at);
        return sorted[low] + (sorted[high] - sorted[low]) * (at - low);
    }
    static string Number(double value) { return value.ToString("F4", CultureInfo.InvariantCulture); }
}
