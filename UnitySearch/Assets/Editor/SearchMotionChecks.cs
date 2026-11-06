using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

// Exercises the imported rig and the real presentation driver, without saving assets.
public static class SearchMotionChecks
{
    [MenuItem("Search Demo/Play-mode checks/Jump found")]
    static void JumpFound() { Play("Jump Search", 13); }
    [MenuItem("Search Demo/Play-mode checks/Jump missing")]
    static void JumpMissing() { Play("Jump Search", 12); }
    [MenuItem("Search Demo/Play-mode checks/Exponential found")]
    static void ExponentialFound() { Play("Exponential Search", 13); }
    [MenuItem("Search Demo/Play-mode checks/Exponential missing")]
    static void ExponentialMissing() { Play("Exponential Search", 12); }
    [MenuItem("Search Demo/Play-mode checks/Fibonacci found")]
    static void FibonacciFound() { Play("Fibonacci Search", 13); }
    [MenuItem("Search Demo/Play-mode checks/Fibonacci missing")]
    static void FibonacciMissing() { Play("Fibonacci Search", 12); }
    static void Play(string algorithm, int target)
    {
        if (!Application.isPlaying) { Debug.LogError("Enter Play mode before choosing a verification run."); return; }
        var stage = UnityEngine.Object.FindFirstObjectByType<SearchStage>();
        if (stage == null) { Debug.LogError("Open Assets/Search.unity first."); return; }
        stage.Motion.logMotionPhases = true;
        stage.SetWholeStage(false);
        stage.LoadSample(algorithm, target);
        stage.Playback.Duration = stage.Motion.defaultStepSeconds;
        stage.Playback.Play();
        Selection.activeGameObject = stage.Theatre.Actor.gameObject;
    }
    static int samples;
    static float maximumDrift;
    static float maximumLegLengthError, minimumWalkingKneeBend;
    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Motion check failed: " + message);
    }
    static void Near(double actual, double expected, string message)
    {
        Check(Math.Abs(actual - expected) < .00001, message + ": " + actual + " != " + expected);
    }
    public static void Run()
    {
        samples = 0; maximumDrift = 0;
        maximumLegLengthError = 0; minimumWalkingKneeBend = 180;
        var root = new GameObject("Temporary motion verification");
        var stage = root.AddComponent<SearchStage>();
        Camera camera = null;
        try
        {
            stage.Initialize(); camera = stage.Theatre.View;
            var actor = stage.Theatre.Actor;
            var motion = stage.Motion;
            var allBones = actor.GetComponentsInChildren<Transform>();
            File.WriteAllLines("../obj/motion-rig-bones.txt", allBones.Select(b => b.name).ToArray());
            var hip = allBones.Single(b => b.name == "Hips_02");
            var ankleL = allBones.Single(b => b.name == "Left ankle_0122");
            var ankleR = allBones.Single(b => b.name == "Right ankle_0126");
            var shoulder = allBones.Single(b => b.name.StartsWith("Left shoulder"));
            var destinations = new[] { new Vector3(2.4f, 0, 0), new Vector3(-4.8f, 0, -3.2f),
                new Vector3(0, 0, 3.2f), new Vector3(7.2f, .3f, 0), new Vector3(0, -.365f, -3.2f) };
            foreach (var destination in destinations)
            foreach (double dt in new[] { 1.0 / 30, 1.0 / 60, .037 })
            {
                Vector3 source = new Vector3(0, .2f, 0);
                actor.ResetPose(); actor.Pose(null, 0, source, source, false, false, motion);
                float restHipOffset = hip.position.y - source.y;
                Vector3 footL = ankleL.position, footR = ankleR.position;
                actor.BeginMotion();
                var plan = motion.Probe(Vector3.Distance(source, destination), 1.5);
                for (double t = 0; t <= plan.Duration + dt; t += dt)
                {
                    double at = Math.Min(t, plan.Duration);
                    actor.Pose(plan, at, source, destination, false, false, motion);
                    maximumDrift = Mathf.Max(maximumDrift, actor.PlantedDrift); samples++;
                    Check(actor.PlantedDrift < motion.plantedDriftTolerance, "planted foot drift at " + at);
                    Check(!float.IsNaN(hip.position.y), "non-finite pose");
                    if (actor.FeetPlanted)
                    {
                        bool landed = at >= plan.StartOf(SearchMotionPhase.Land);
                        Vector3 support = landed ? destination : source;
                        Near(actor.transform.position.y, support.y, "support surface");
                        Near(hip.position.y, support.y + restHipOffset - actor.PelvisDrop, "pelvis support tracking");
                        Vector3 translation = landed ? destination - source : Vector3.zero;
                        Check(Vector3.Distance(ankleL.position, footL + translation) < .01f, "left anchor moved");
                        Check(Vector3.Distance(ankleR.position, footR + translation) < .01f, "right anchor moved");
                    }
                    Vector3 before = ankleL.position;
                    Quaternion arm = shoulder.rotation;
                    actor.Pose(plan, at, source, destination, false, false, motion);
                    Check(Vector3.Distance(before, ankleL.position) < .00001f && Quaternion.Angle(arm, shoulder.rotation) < .02f, "paused pose changed");
                }
                actor.Pose(plan, plan.StartOf(SearchMotionPhase.Takeoff), source, destination, false, false, motion);
                Near(actor.PelvisDrop, motion.compressionDepth, "anticipation compression");
                actor.Pose(plan, plan.StartOf(SearchMotionPhase.Settle), source, destination, false, false, motion);
                Near(actor.PelvisDrop, motion.compressionDepth, "landing compression");
                actor.Pose(plan, plan.StartOf(SearchMotionPhase.Compare), source, destination, false, false, motion);
                Near(actor.PelvisDrop, 0, "settled pelvis");
                Near(plan.LengthOf(SearchMotionPhase.Anticipate), motion.anticipationSeconds, "anticipation duration");
                Near(plan.LengthOf(SearchMotionPhase.Land), motion.landingSeconds, "landing duration");
                Near(plan.LengthOf(SearchMotionPhase.Settle), motion.settleSeconds, "settle duration");
            }
            double shortTravel = motion.Probe(2.4f, 1.5).LengthOf(SearchMotionPhase.Travel);
            double longTravel = motion.Probe(8, 1.5).LengthOf(SearchMotionPhase.Travel);
            Check(longTravel > shortTravel && longTravel <= motion.maximumTravelSeconds, "distance scaling/clamp");
            foreach (double requested in new[] { (double)motion.minimumStepSeconds, .7, 1.5, 3, 10 })
            {
                double previous = 0;
                foreach (float distance in new[] { 2.4f, 3.2f, 4.8f, 8f })
                {
                    double travel = motion.Probe(distance, requested).LengthOf(SearchMotionPhase.Travel);
                    Check(travel >= previous && travel <= motion.maximumTravelSeconds,
                        "walk/hop timing reverses at " + distance + " for " + requested);
                    previous = travel;
                }
            }
            motion.scaleDurationWithDistance = false;
            Near(motion.Probe(2.4f, 1.5).LengthOf(SearchMotionPhase.Travel), motion.Probe(8, 1.5).LengthOf(SearchMotionPhase.Travel), "constant timing");
            motion.scaleDurationWithDistance = true;
            CheckWalkAndContinuity(actor, motion, allBones, ankleL, ankleR);

            foreach (string algorithm in new[] { "Jump Search", "Exponential Search", "Fibonacci Search" })
            foreach (int target in new[] { 13, 12 })
            {
                stage.LoadSample(algorithm, target);
                var expected = stage.Plan.Steps;
                string trace = string.Join(";", expected.Select(s => s.Index + "," + s.Found + "," + s.Terminal + "," + s.Comparisons));
                stage.Playback.Duration = 1.5;
                int visited = 0;
                while (!stage.Playback.Finished)
                {
                    stage.Playback.Step(); stage.AdvancePresentation(0);
                    Check(stage.Playback.Current == visited++, "step order");
                    if (stage.Current.Terminal)
                    {
                        Vector3 focus = stage.Theatre.CameraFocus;
                        float size = camera.orthographicSize;
                        var plan = stage.MotionPlan;
                        double pullback = plan.StartOf(SearchMotionPhase.Pullback);
                        Near(plan.LengthOf(SearchMotionPhase.ResultHold), motion.resultHoldSeconds, "overview hold");
                        Check(pullback >= plan.StartOf(SearchMotionPhase.ReactionSettle) + motion.reactionSettleSeconds, "camera overlaps reaction");
                        while (stage.Playback.Elapsed + 1.0 / 60 < pullback)
                        {
                            stage.AdvancePresentation(1.0 / 60);
                            Check(Vector3.Distance(focus, stage.Theatre.CameraFocus) < .00001f, "camera moved during reaction");
                            Near(camera.orthographicSize, size, "camera zoom during reaction");
                            Check(actor.PlantedDrift < .01f, "reaction contact");
                            maximumDrift = Mathf.Max(maximumDrift, actor.PlantedDrift); samples++;
                        }
                        stage.AdvancePresentation(plan.StartOf(SearchMotionPhase.ResultHold) - stage.Playback.Elapsed);
                        Near(camera.orthographicSize, stage.Theatre.OverviewSize, "settled overview");
                        Check(!stage.Playback.Finished, "overview hold skipped");
                    }
                    stage.AdvancePresentation(stage.Playback.BeatDuration);
                }
                string actual = string.Join(";", stage.Playback.Steps.Select(s => s.Index + "," + s.Found + "," + s.Terminal + "," + s.Comparisons));
                Check(actual == trace && visited == expected.Count, "algorithm trace changed");
                Check(stage.Current.Found == (target == 13), "search result changed");
            }
            string result = "PASS: " + samples + " actual-rig samples; maximum planted drift=" + maximumDrift.ToString("R")
                + "m; maximum leg segment length change=" + maximumLegLengthError.ToString("R")
                + "m; minimum mid-walk planted knee bend=" + minimumWalkingKneeBend.ToString("F2") + " degrees"
                + "; support height, compression, pause, distance toggle, six algorithm traces, camera reaction hold and seven-second overview hold.";
            File.WriteAllText("../obj/motion-checks.txt", result); Debug.Log(result);
        }
        finally
        {
            if (stage.Theatre != null) stage.Theatre.Actor.CharacterAnimator.Dispose();
            UnityEngine.Object.DestroyImmediate(root);
            if (camera != null) UnityEngine.Object.DestroyImmediate(camera.gameObject);
        }
    }
    static void CheckWalkAndContinuity(FurinaActor actor, SearchMotionSettings motion, Transform[] bones, Transform left, Transform right)
    {
        var upper = new[] { bones.Single(b => b.name == "Left leg_0120"), bones.Single(b => b.name == "Right leg_0124") };
        var knees = new[] { bones.Single(b => b.name == "Left knee_0121"), bones.Single(b => b.name == "Right knee_0125") };
        var feet = new[] { left, right };
        var thighs = upper.Select((b, i) => Vector3.Distance(b.position, knees[i].position)).ToArray();
        var shins = knees.Select((b, i) => Vector3.Distance(b.position, feet[i].position)).ToArray();
        foreach (float distance in new[] { 2.4f, -2.4f, 7.2f })
        {
            Vector3 source = Vector3.zero, destination = Vector3.right * distance;
            actor.ResetPose(); actor.Pose(null, 0, source, source, false, false, motion); actor.BeginMotion();
            var plan = motion.Probe(Math.Abs(distance), 3);
            float leftHeight = left.position.y, rightHeight = right.position.y;
            Vector3 previousLeft = left.position, previousRight = right.position;
            bool plantedLeft = true, plantedRight = true, sawFloor = false, sawWalk = false;
            for (int i = 0; i <= 600; i++)
            {
                double t = plan.Duration * i / 600;
                actor.Pose(plan, t, source, destination, false, false, motion); samples++;
                Check(actor.PlantedDrift < motion.plantedDriftTolerance, "locomotion foot drift " + distance + " at " + t + ": " + actor.PlantedDrift);
                maximumDrift = Mathf.Max(maximumDrift, actor.PlantedDrift);
                if (actor.Walking)
                {
                    sawWalk = true;
                    float travel = plan.ProgressOf(SearchMotionPhase.Travel, t);
                    for (int leg = 0; leg < 2; leg++)
                    {
                        float error = Mathf.Max(Mathf.Abs(Vector3.Distance(upper[leg].position, knees[leg].position) - thighs[leg]),
                            Mathf.Abs(Vector3.Distance(knees[leg].position, feet[leg].position) - shins[leg]));
                        maximumLegLengthError = Mathf.Max(maximumLegLengthError, error);
                        Check(error < .0001f, "walking stretches a leg segment");
                        if (travel > .15f && travel < .85f && (leg == 0 ? actor.LeftPlanted : actor.RightPlanted))
                        {
                            float bend = 180 - Vector3.Angle(upper[leg].position - knees[leg].position, feet[leg].position - knees[leg].position);
                            minimumWalkingKneeBend = Mathf.Min(minimumWalkingKneeBend, bend);
                        }
                    }
                    if (actor.SupportHeight < -.1f) sawFloor = true;
                    if (plantedLeft && actor.LeftPlanted) Check(Vector3.Distance(previousLeft, left.position) < .01f, "left walking support slides");
                    if (plantedRight && actor.RightPlanted) Check(Vector3.Distance(previousRight, right.position) < .01f, "right walking support slides");
                    Check(left.position.y >= leftHeight - motion.floorDepth - .01f, "left foot below floor");
                    Check(right.position.y >= rightHeight - motion.floorDepth - .01f, "right foot below floor");
                }
                previousLeft = left.position; previousRight = right.position;
                plantedLeft = actor.LeftPlanted; plantedRight = actor.RightPlanted;
            }
            if (Math.Abs(distance) <= motion.maximumWalkDistance) Check(sawWalk && sawFloor, "walking never reached the floor");
            foreach (SearchMotionPhase phase in new[] { SearchMotionPhase.Anticipate, SearchMotionPhase.Takeoff,
                SearchMotionPhase.Travel, SearchMotionPhase.Land, SearchMotionPhase.Settle, SearchMotionPhase.Compare })
            {
                double boundary = plan.StartOf(phase);
                actor.Pose(plan, Math.Max(0, boundary - .00001), source, destination, false, false, motion);
                Vector3[] positions = bones.Select(b => b.position).ToArray();
                Quaternion[] rotations = bones.Select(b => b.rotation).ToArray();
                actor.Pose(plan, boundary + .00001, source, destination, false, false, motion);
                for (int i = 0; i < bones.Length; i++)
                {
                    Check(Vector3.Distance(positions[i], bones[i].position) < .002f, "position pop: " + bones[i].name + " at " + phase);
                    Check(Quaternion.Angle(rotations[i], bones[i].rotation) < 1f, "rotation pop: " + bones[i].name + " at " + phase);
                }
            }
        }
    }
}
