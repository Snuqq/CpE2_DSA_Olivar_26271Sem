using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Exercises framing without the imported model, animation graph, or capture overhead.
public static class SearchCameraChecks
{
    [MenuItem("Search Demo/Verify camera and contact effects")]
    public static void Run()
    {
        var root = new GameObject("Temporary camera verification");
        int samples = 0;
        try
        {
            var theatre = root.AddComponent<SearchTheatre>();
            var view = new GameObject("Verification camera").AddComponent<Camera>();
            view.transform.SetParent(root.transform);
            view.orthographic = true;
            typeof(SearchTheatre).GetProperty("View").SetValue(theatre, view, null);
            var motion = new SearchMotionSettings();
            Vector3[] destinations = { new Vector3(2.4f, 0, 0), new Vector3(7.2f, 0, -3.2f),
                new Vector3(-16.8f, 0, 12.8f), new Vector3(0, .3f, -6.4f) };
            foreach (float aspect in new[] { .75f, 1.33f, 2.1f })
            foreach (var destination in destinations)
            foreach (float dt in new[] { 1f / 30, 1f / 60, 1f / 120 })
            foreach (bool wholeStage in new[] { false, true })
            {
                view.aspect = aspect;
                Vector3 source = new Vector3(0, 0, 0);
                theatre.MoveCamera(source + Vector3.up * .15f, 2.8f);
                theatre.BeginFrameMove(source, destination, 1.5f, motion);
                Vector3 initial = view.transform.position;
                float initialSize = view.orthographicSize;
                theatre.UpdateFrameMove(0, source, destination, dt, wholeStage);
                Check(Vector3.Distance(initial, view.transform.position) < .00001f
                    && Mathf.Abs(view.orthographicSize - initialSize) < .00001f, "preframing snapped at start");
                // Sample exact phase boundaries: widening, pan, and tightening must not overlap.
                foreach (float p in new[] { .1f, .2f, .4f, .7f, .85f, 1f })
                {
                    Vector3 previous = theatre.CameraFocus;
                    float previousSize = view.orthographicSize;
                    theatre.UpdateFrameMove(p, source, destination, dt, wholeStage);
                    bool pan = Vector3.Distance(previous, theatre.CameraFocus) > .00001f;
                    bool zoom = Mathf.Abs(previousSize - view.orthographicSize) > .00001f;
                    Check(!(pan && zoom), "zoom and pan overlap during preframing");
                    InFrame(view, source, "source during preframing");
                    InFrame(view, source + Vector3.up * 2.7f, "head during preframing");
                }
                float lockedSize = view.orthographicSize;
                for (float t = 0; t <= 1 + dt; t += dt)
                {
                    float progress = Mathf.Min(1, t);
                    Vector3 actor = Vector3.Lerp(source, destination, progress)
                        + Vector3.up * (6 * progress * (1 - progress));
                    theatre.UpdateFrameMove(1, actor, destination, dt, wholeStage);
                    Check(Mathf.Abs(view.orthographicSize - lockedSize) < .00001f, "travel zoom changed");
                    InFrame(view, actor, "actor feet during travel");
                    InFrame(view, actor + Vector3.up * 2.7f, "actor head during travel");
                    InFrame(view, destination, "destination during travel");
                    Vector3 pausedPosition = view.transform.position;
                    Quaternion pausedRotation = view.transform.rotation;
                    theatre.UpdateFrameMove(1, actor + Vector3.right, destination, 0, wholeStage);
                    Check(view.transform.position == pausedPosition && view.transform.rotation == pausedRotation
                        && view.orthographicSize == lockedSize, "paused camera changed");
                    samples++;
                }
            }
            CheckEffects(root.transform);
            Debug.Log("Camera/contact checks passed: " + samples + " arc samples, 30/60/120 Hz, three aspect ratios, frozen zoom and pause, pooled ripple and found glow.");
        }
        finally { UnityEngine.Object.DestroyImmediate(root); }
    }
    static void CheckEffects(Transform parent)
    {
        var scenery = new GameObject("Temporary contact effects").AddComponent<FontaineScenery>();
        scenery.transform.SetParent(parent);
        scenery.Initialize();
        scenery.Layout(5, 2, 10);
        scenery.Highlight(0, 9, 0, false);
        var ripple = scenery.transform.Find("Landing ripple").GetComponent<Renderer>();
        Check(!ripple.enabled, "ripple begins visible");
        scenery.ShowContact(0, false, .1f, false);
        Check(ripple.enabled, "landing did not trigger ripple");
        Vector3 scale = ripple.transform.localScale;
        scenery.ShowContact(0, false, 1, true);
        Check(ripple.enabled && ripple.transform.localScale == scale, "paused ripple changed");
        scenery.ShowContact(0, false, 1, false);
        Check(!ripple.enabled, "ripple did not expire");
        scenery.ShowContact(0, false, .1f, false);
        Check(!ripple.enabled, "held contact retriggered ripple");
        scenery.ShowContact(-1, false, .1f, false);
        scenery.ShowContact(0, false, .1f, false);
        Check(ripple.enabled, "new landing at same platform did not retrigger");
        scenery.Highlight(0, 9, 0, true);
        scenery.ShowContact(0, true, .1f, false);
        var face = scenery.transform.Find("Index 0 enamel podium/Enamel surface").GetComponent<Renderer>();
        var tint = new MaterialPropertyBlock();
        face.GetPropertyBlock(tint);
        Check(tint.GetColor("_EmissionColor").g > .60f * .22f, "found glow did not pulse");
        scenery.ResetEffects();
        Check(!ripple.enabled, "reset left ripple active");
        scenery.ShowContact(-1, false, 1, false);
        face.GetPropertyBlock(tint);
        Check(Mathf.Abs(tint.GetColor("_EmissionColor").g - .60f * .22f) < .0001f, "reset left found pulse active");
    }
    static void InFrame(Camera view, Vector3 world, string label)
    {
        Vector3 point = view.WorldToViewportPoint(world);
        Check(point.z > 0 && point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1,
            label + " clipped: " + point);
    }
    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Camera check failed: " + message);
    }
}
