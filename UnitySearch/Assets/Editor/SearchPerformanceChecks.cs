using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;

// Isolates CPU posing cost. Player frame pacing is measured separately without screenshots.
public static class SearchPerformanceChecks
{
    public static void Measure(SearchStage stage)
    {
        var actor = stage.Theatre.Actor;
        var motion = stage.Motion;
        Vector3 source = stage.Theatre.Position(0), destination = stage.Theatre.Position(7);
        var plan = motion.Probe(Vector3.Distance(source, destination), 3);
        actor.ResetPose();
        actor.Pose(null, 0, source, source, false, false, motion);
        actor.BeginMotion();
        for (int i = 0; i < 100; i++) actor.Pose(plan, plan.Duration * i / 100, source, destination, false, false, motion);
        var timer = new Stopwatch();
        long counterBefore = GC.GetAllocatedBytesForCurrentThread();
        var sentinel = new byte[4096];
        bool allocationCounterAvailable = GC.GetAllocatedBytesForCurrentThread() - counterBefore >= sentinel.Length;
        GC.KeepAlive(sentinel);
        long before = GC.GetAllocatedBytesForCurrentThread();
        timer.Start();
        const int samples = 2000;
        for (int i = 0; i < samples; i++)
            actor.Pose(plan, plan.Duration * (i % 100) / 100, source, destination, false, false, motion);
        timer.Stop();
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        string result = string.Format(CultureInfo.InvariantCulture,
            "Editor CPU pose benchmark: {0} samples, {1:F4} ms/pose, {2} managed bytes/pose. Scene renderers: {3}. GPU and IMGUI excluded.",
            samples, timer.Elapsed.TotalMilliseconds / samples, allocationCounterAvailable ? (allocated / (double)samples).ToString("F1") : "N/A (counter failed known-allocation check)",
            stage.GetComponentsInChildren<Renderer>(true).Length);
        File.WriteAllText("../obj/performance-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".txt", result);
        UnityEngine.Debug.Log(result);
        stage.ResetPlayback();
        stage.AdvancePresentation(motion.resetSeconds);
    }
}
