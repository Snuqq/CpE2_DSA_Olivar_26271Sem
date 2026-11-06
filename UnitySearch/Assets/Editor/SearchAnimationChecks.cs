using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

// Runs the actual stage clock and imported character without changing scene assets.
public static class SearchAnimationChecks
{
    static readonly MethodInfo ReceiveCommand = typeof(SearchStage).GetMethod("Command", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly HashSet<string> SeenStates = new HashSet<string>();
    static int commandSession, samples;
    static float maximumDrift;

    sealed class Snapshot
    {
        readonly Transform[] bones;
        readonly Vector3[] positions;
        readonly Quaternion[] rotations;
        readonly Vector3 cameraPosition;
        readonly Quaternion cameraRotation;
        readonly float cameraSize;

        public Snapshot(SearchStage stage)
        {
            bones = stage.Theatre.Actor.GetComponentsInChildren<Transform>();
            positions = bones.Select(b => b.position).ToArray();
            rotations = bones.Select(b => b.rotation).ToArray();
            cameraPosition = stage.Theatre.View.transform.position;
            cameraRotation = stage.Theatre.View.transform.rotation;
            cameraSize = stage.Theatre.View.orthographicSize;
        }

        public void Unchanged(SearchStage stage, string context)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                Check(Vector3.Distance(bones[i].position, positions[i]) < .00002f,
                    context + " moved " + bones[i].name);
                // Quaternion.Angle can report a small nonzero result for identical float quaternions.
                Check(Quaternion.Angle(bones[i].rotation, rotations[i]) < .1f,
                    context + " rotated " + bones[i].name);
            }
            Check(Vector3.Distance(stage.Theatre.View.transform.position, cameraPosition) < .00002f,
                context + " moved the camera");
            Check(Quaternion.Angle(stage.Theatre.View.transform.rotation, cameraRotation) < .1f,
                context + " rotated the camera");
            Near(stage.Theatre.View.orthographicSize, cameraSize, .00001, context + " changed zoom");
        }
    }

    [MenuItem("Search Demo/Verify animation integration")]
    public static void Run()
    {
        samples = 0; commandSession = 0; maximumDrift = 0; SeenStates.Clear();
        var root = new GameObject("Temporary animation integration checks");
        Camera previousCamera = Camera.main;
        Camera camera = null;
        try
        {
            var stage = root.AddComponent<SearchStage>();
            stage.Initialize(); camera = stage.Theatre.View;
            Check(stage.Theatre.Actor.CharacterAnimator.HasController, "generated Animator Controller is missing");
            foreach (int rate in new[] { 30, 60, 120 })
            {
                double delta = 1.0 / rate;
                CheckIdle(stage, delta);
                foreach (double duration in new[] { 1.0, 5.0 })
                    CheckJumpTrace(stage, delta, duration);
                CheckReset(stage, delta);
            }
            CheckIdleLayerTransitions(stage, 13);
            CheckIdleLayerTransitions(stage, 12);
            CheckPendingViewReset(stage);
            CheckResetStartRace(stage);
            CheckNotFound(stage);
            foreach (SearchCharacterState state in Enum.GetValues(typeof(SearchCharacterState)))
                Check(SeenStates.Contains(state.ToString()), "state was never presented: " + state);
            string result = string.Format(CultureInfo.InvariantCulture,
                "PASS: {0} stage presentation samples at 30/60/120 Hz; maximum planted drift={1:R}m. " +
                "1s/5s beats, idle and paused poses, zero delta, block/scan arcs, fixed travel zoom, " +
                "Next spam, continuous reset, additive pose continuity, pending-view reset, " +
                "reset/start session race, seven Animator states, and not-found outcome.",
                samples, maximumDrift);
            Directory.CreateDirectory("../obj");
            File.WriteAllText("../obj/search-animation-checks.txt", result);
            Debug.Log(result);
        }
        finally
        {
            var stage = root.GetComponent<SearchStage>();
            if (stage != null && stage.Theatre != null) stage.Theatre.Actor.CharacterAnimator.Dispose();
            UnityEngine.Object.DestroyImmediate(root);
            if (camera != null && camera != previousCamera) UnityEngine.Object.DestroyImmediate(camera.gameObject);
        }
    }

    static void CheckIdle(SearchStage stage, double delta)
    {
        stage.LoadSample("Jump Search", 13);
        Transform chest = stage.Theatre.Actor.GetComponentsInChildren<Transform>().Single(t => t.name == "Chest_04");
        Quaternion before = chest.rotation;
        for (double t = 0; t < 1; t += delta) Advance(stage, delta);
        Check(Quaternion.Angle(before, chest.rotation) > .15f, "idle has no visible breathing motion");
        Check(stage.Playback.Current == -1, "idle advanced the algorithm");
        AssertFrozen(stage, delta, "idle Pause");
    }

    static void CheckJumpTrace(SearchStage stage, double delta, double duration)
    {
        stage.LoadSample("Jump Search", 13);
        stage.Playback.Duration = duration;
        string original = stage.Plan.Serialize();
        float firstBlockHeight = 0, scanHeight = 0;
        int expected = 0;
        while (!stage.Playback.Finished)
        {
            Send(stage, "STEP");
            Check(stage.Playback.Current == expected++, "Next skipped a recorded comparison");
            bool terminal = stage.Current.Terminal;
            if (!terminal) Near(stage.Playback.BeatDuration, duration, .00001, "requested step budget");
            float surface = stage.Theatre.Position(stage.Current.Index).y;
            float height = 0, travelZoom = -1;
            double running = 0;
            bool checkedPause = false;
            while (stage.Playback.Moving)
            {
                // Repeated real command dispatch must finish this beat without queuing another probe.
                if (!terminal) Send(stage, "STEP");
                Advance(stage, delta); running += delta;
                Check(stage.Playback.Current == expected - 1, "Next spam queued a probe");
                Check(running <= stage.Playback.BeatDuration + 2 * delta, "beat did not finish on schedule");
                height = Mathf.Max(height, stage.Theatre.Actor.transform.position.y - surface);
                if (!terminal && stage.MotionPhase == SearchMotionPhase.Travel)
                {
                    if (travelZoom < 0) travelZoom = stage.Theatre.View.orthographicSize;
                    Near(stage.Theatre.View.orthographicSize, travelZoom, .00001, "zoom changed during travel");
                    if (!checkedPause)
                    {
                        AssertFrozen(stage, delta, "mid-jump Pause");
                        Send(stage, "STEP"); checkedPause = true;
                    }
                }
            }
            Near(running, stage.Playback.BeatDuration, delta + .00001, "elapsed beat budget");
            if (expected == 1) firstBlockHeight = height;
            if (!terminal && stage.Current.Phase.IndexOf("linear scan", StringComparison.OrdinalIgnoreCase) >= 0)
                scanHeight = Mathf.Max(scanHeight, height);
        }
        Check(firstBlockHeight > scanHeight + .1f && scanHeight > .1f,
            "block jump and linear scan do not have visibly different arcs");
        Check(stage.Current.Found && stage.Current.Index == 6, "Jump Search result changed");
        Check(stage.Plan.Serialize() == original, "animation mutated the supplied search trace");
    }

    static void CheckReset(SearchStage stage, double delta)
    {
        stage.LoadSample("Jump Search", 13); stage.Playback.Duration = 5;
        Send(stage, "STEP");
        double halfway = stage.MotionPlan.StartOf(SearchMotionPhase.Travel)
            + stage.MotionPlan.LengthOf(SearchMotionPhase.Travel) * .5;
        while (stage.Playback.Elapsed < halfway) Advance(stage, delta);
        Vector3 previous = stage.Theatre.Actor.transform.position;
        var beforeReset = new Snapshot(stage);
        stage.ResetPlayback();
        beforeReset.Unchanged(stage, "Reset command");
        Advance(stage, 0);
        beforeReset.Unchanged(stage, "zero-time reset");
        Vector3 home = stage.Theatre.Position(0);
        float returnDistance = Vector3.Distance(previous, home);
        float maximumPerFrame = (float)(returnDistance * 4 * delta / stage.Motion.resetSeconds) + .03f;
        double elapsed = 0;
        while (stage.Resetting)
        {
            Advance(stage, delta); elapsed += delta;
            Vector3 position = stage.Theatre.Actor.transform.position;
            Check(Vector3.Distance(previous, position) < maximumPerFrame, "reset teleported the character");
            previous = position;
            Check(elapsed <= stage.Motion.resetSeconds + 2 * delta, "reset never completed");
        }
        Check(Vector3.Distance(previous, home) < .001f, "reset did not return to the starting platform");
        Check(stage.Playback.Current == -1 && !stage.Playback.Moving, "reset retained an active algorithm step");
    }

    static void CheckIdleLayerTransitions(SearchStage stage, int target)
    {
        stage.LoadSample("Jump Search", target); stage.Playback.Duration = 5;
        Advance(stage, .7);
        Send(stage, "STEP");
        CheckPoseBoundary(stage, SearchMotionPhase.Compare, "Settle to Compare");
        // Other probes are already exercised at all three rates by CheckJumpTrace.
        Advance(stage, stage.Playback.BeatDuration - stage.Playback.Elapsed + .001);
        int remaining = stage.Playback.Steps.Count;
        while (!stage.Current.Terminal && remaining-- > 0)
        {
            Send(stage, "STEP");
            if (!stage.Current.Terminal) Advance(stage, stage.Playback.BeatDuration);
        }
        Check(stage.Current.Terminal, "continuity check did not reach the result");
        CheckPoseBoundary(stage, SearchMotionPhase.ResultHold,
            "Pullback to ResultHold, target " + target);
    }

    static void CheckPoseBoundary(SearchStage stage, SearchMotionPhase phase, string context)
    {
        const double halfInterval = .00025;
        double boundary = stage.MotionPlan.StartOf(phase);
        double before = boundary - halfInterval;
        while (stage.Playback.Elapsed < before - .000000001)
            Advance(stage, Math.Min(1.0 / 60, before - stage.Playback.Elapsed));
        var bones = stage.Theatre.Actor.GetComponentsInChildren<Transform>();
        Transform head = bones.Single(t => t.name == "Head_06");
        Transform hips = bones.Single(t => t.name == "Hips_02");
        Quaternion headRotation = head.rotation, hipRotation = hips.rotation;
        Vector3 headPosition = head.position, hipPosition = hips.position;
        // A half-millisecond crossing isolates an instantaneous layer toggle from normal motion.
        Advance(stage, halfInterval * 2);
        Check(stage.MotionPhase == phase, context + " did not cross the phase boundary");
        Check(Quaternion.Angle(headRotation, head.rotation) < .5f,
            context + " snapped head rotation");
        Check(Quaternion.Angle(hipRotation, hips.rotation) < .25f,
            context + " snapped hip rotation");
        Check(Vector3.Distance(headPosition, head.position) < .001f,
            context + " snapped head position");
        Check(Vector3.Distance(hipPosition, hips.position) < .0005f,
            context + " snapped hip position");
    }

    static void CheckPendingViewReset(SearchStage stage)
    {
        const double delta = 1.0 / 60;
        stage.LoadSample("Jump Search", 13); stage.Playback.Duration = 5;
        Send(stage, "STEP");
        double middle = stage.MotionPlan.StartOf(SearchMotionPhase.Travel)
            + stage.MotionPlan.LengthOf(SearchMotionPhase.Travel) * .5;
        while (stage.Playback.Elapsed < middle) Advance(stage, delta);
        SendRaw(stage, "VIEW|" + commandSession + "|follow");
        stage.ResetPlayback();
        for (int i = 0; stage.Resetting && i < 180; i++) Advance(stage, delta);
        Check(!stage.Resetting, "pending-view reset did not finish");
        Vector3 overview = stage.Theatre.OverviewFocus;
        float zoom = stage.Theatre.OverviewSize;
        // A stale queued view used to start only after the reset completed.
        for (int i = 0; i < 60; i++)
        {
            Advance(stage, delta);
            Check(Vector3.Distance(stage.Theatre.CameraFocus, overview) < .0001f,
                "queued view moved the settled reset overview");
            Near(stage.Theatre.View.orthographicSize, zoom, .0001,
                "queued view zoomed the settled reset overview");
        }
    }

    static void CheckResetStartRace(SearchStage stage)
    {
        const double delta = 1.0 / 60;
        stage.LoadSample("Jump Search", 13); stage.Playback.Duration = 1;
        Send(stage, "STEP");
        for (int i = 0; i < 30; i++) Advance(stage, delta);
        int stale = commandSession;
        commandSession++; Send(stage, "RESET");
        Advance(stage, delta);
        AssertFrozen(stage, delta, "reset Pause");
        var missing = Resources.Load<TextAsset>("SearchSamples/Jump-12");
        Check(missing != null, "missing not-found sample");
        commandSession++;
        SendRaw(stage, "LOAD|" + commandSession + "|" + missing.text);
        Send(stage, "PLAY");
        SendRaw(stage, "RESET|" + stale);
        SendRaw(stage, "PLAY|" + stale);
        for (int i = 0; stage.Resetting && i < 180; i++) Advance(stage, delta);
        Check(!stage.Resetting && stage.Target == 12, "new plan did not survive reset/start race");
        Check(stage.Playback.Current == 0 && stage.Playback.Playing, "deferred Start was lost or replayed twice");
        Check(stage.Plan.Serialize() == SearchPlan.Deserialize(missing.text).Serialize(), "stale session replaced the plan");
    }

    static void CheckNotFound(SearchStage stage)
    {
        stage.LoadSample("Jump Search", 12); stage.Playback.Duration = 1;
        Send(stage, "PLAY");
        int remaining = 3600;
        while (!stage.Playback.Finished && remaining-- > 0) Advance(stage, 1.0 / 60);
        Check(stage.Playback.Finished && !stage.Current.Found && stage.Current.Index == -1,
            "not-found presentation failed to finish");
    }

    static void AssertFrozen(SearchStage stage, double delta, string context)
    {
        Send(stage, "PAUSE");
        var snapshot = new Snapshot(stage);
        double elapsed = stage.Playback.Elapsed, resetProgress = stage.ResetProgress;
        for (int i = 0; i < 6; i++)
        {
            Advance(stage, i % 2 == 0 ? delta : 0);
            snapshot.Unchanged(stage, context);
        }
        Near(stage.Playback.Elapsed, elapsed, .0000001, context + " advanced playback");
        Near(stage.ResetProgress, resetProgress, .0000001, context + " advanced reset");
    }

    static void Advance(SearchStage stage, double delta)
    {
        stage.AdvancePresentation(delta); samples++;
        var actor = stage.Theatre.Actor;
        SeenStates.Add(actor.CharacterAnimator.CurrentState);
        maximumDrift = Mathf.Max(maximumDrift, actor.PlantedDrift);
        Check(actor.PlantedDrift < .01f, "planted foot drift exceeds 1 cm");
        Vector3 p = actor.transform.position;
        Check(!float.IsNaN(p.x) && !float.IsNaN(p.y) && !float.IsNaN(p.z)
            && !float.IsInfinity(p.x) && !float.IsInfinity(p.y) && !float.IsInfinity(p.z), "non-finite character position");
    }

    static void Send(SearchStage stage, string command) { SendRaw(stage, command + "|" + commandSession); }
    static void SendRaw(SearchStage stage, string command)
    {
        Check(ReceiveCommand != null, "stage command receiver is missing");
        try { ReceiveCommand.Invoke(stage, new object[] { command }); }
        catch (TargetInvocationException error) { throw new InvalidOperationException("Command failed: " + command, error.InnerException); }
    }
    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Animation integration check failed: " + message);
    }
    static void Near(double actual, double expected, double tolerance, string message)
    {
        Check(Math.Abs(actual - expected) <= tolerance, message + ": " + actual + " != " + expected);
    }
}
