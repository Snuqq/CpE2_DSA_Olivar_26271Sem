using UnityEngine;

public sealed class SearchTheatre : MonoBehaviour
{
    [Header("Camera framing")]
    [SerializeField, Min(.01f)] float cameraFollowSeconds = .2f;
    [SerializeField, Range(0, 1)] float destinationLookAhead = .25f;
    [SerializeField, Min(0)] float performerHeight = 2.9f;
    [SerializeField, Min(0)] float performerRadius = .65f;
    [SerializeField, Min(0)] float platformRadius = .88f;
    [SerializeField, Range(.01f, .3f)] float framingMargin = .08f;
    [SerializeField, Min(.01f)] float minimumFrameSize = 2.15f;
    [SerializeField, Min(0)] float cameraElevation = 1.3f;
    [SerializeField, Min(.01f)] float cameraDistance = 12f;
    [SerializeField, Range(.05f, .4f)] float preframeWidenFraction = .2f;
    [SerializeField, Range(.1f, .8f)] float preframePanFraction = .5f;
    Vector3 frameStart, frameTarget, followVelocity;
    float frameStartSize, frameWideSize, frameTravelSize, frameAspect;
    Bounds trajectoryBounds;
    bool framePrepared, frameModeConfigured, frameCommitted;
    public Camera View { get; private set; }
    public FurinaActor Actor { get; private set; }
    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public const float CameraPitch = 25;
    public Vector3 CameraFocus { get; private set; }
    public float CameraCenter { get { return View.transform.position.x; } }
    public Vector3 OverviewFocus { get { return Vector3.up * 1.1f; } }
    FontaineScenery scenery;
    public float OverviewSize
    {
        get { return Mathf.Max((Mathf.Max(16, (Columns - 1) * 2.4f + 8) + 4) / (2 * View.aspect),
            5.8f + (Rows - 1) * 1.6f * Mathf.Sin(CameraPitch * Mathf.Deg2Rad)); }
    }
    public float FrameSize(Vector3 first, Vector3 last)
    {
        return Mathf.Max(2.15f, (Mathf.Abs(last.x - first.x) + 4.8f) / (2 * View.aspect),
            (Mathf.Abs(last.z - first.z) * Mathf.Sin(CameraPitch * Mathf.Deg2Rad) + 4.3f) * .5f);
    }
    public void Initialize()
    {
        Columns = Rows = 1;
        View = Camera.main;
        if (View == null)
        {
            View = new GameObject("Theatre camera").AddComponent<Camera>();
            View.transform.SetParent(transform, false);
        }
        View.clearFlags = CameraClearFlags.SolidColor;
        View.backgroundColor = new Color(.025f, .045f, .08f);
        View.orthographic = true;
        View.rect = new Rect(0, .27f, .65f, .61f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(.25f, .32f, .43f);
        RenderSettings.ambientEquatorColor = new Color(.17f, .20f, .26f);
        RenderSettings.ambientGroundColor = new Color(.12f, .10f, .13f);
        var key = new GameObject("Warm theatre light").AddComponent<Light>();
        key.transform.SetParent(transform); key.type = LightType.Directional;
        key.intensity = 1.18f; key.color = new Color(1, .86f, .65f); key.transform.rotation = Quaternion.Euler(44, -32, 0);
        key.shadows = LightShadows.Soft; key.shadowStrength = .62f;
        key.shadowBias = .035f; key.shadowNormalBias = .12f; key.shadowCustomResolution = 1024;
        var fill = new GameObject("Hydro rim light").AddComponent<Light>();
        fill.transform.SetParent(transform); fill.type = LightType.Directional;
        fill.intensity = .42f; fill.color = new Color(.42f, .67f, 1); fill.transform.rotation = Quaternion.Euler(25, 145, 0);
        scenery = new GameObject("Fontaine opera architecture").AddComponent<FontaineScenery>();
        scenery.transform.SetParent(transform); scenery.Initialize();
        Actor = new GameObject("Furina performer").AddComponent<FurinaActor>();
        Actor.transform.SetParent(transform); Actor.Initialize(); Actor.transform.position = Position(0);
    }
    public Vector3 Position(int index)
    {
        return scenery.Position(index);
    }
    public void MoveCamera(Vector3 center, float size)
    {
        CameraFocus = center;
        View.transform.rotation = Quaternion.Euler(CameraPitch, 0, 0);
        View.transform.position = center + Vector3.up * cameraElevation + View.transform.rotation * Vector3.back * cameraDistance;
        View.orthographicSize = size;
    }
    public void BeginFrameMove(Vector3 source, Vector3 destination, float arcHeight, SearchMotionSettings motion)
    {
        float radius = Mathf.Max(platformRadius, performerRadius, motion.platformSupportRadius);
        trajectoryBounds = new Bounds(source, Vector3.zero);
        trajectoryBounds.Encapsulate(destination);
        trajectoryBounds.Expand(new Vector3(radius * 2, 0, radius * 2));
        Vector3 maximum = trajectoryBounds.max;
        maximum.y += performerHeight + Mathf.Max(0, arcHeight);
        trajectoryBounds.SetMinMax(trajectoryBounds.min, maximum);
        frameStart = CameraFocus;
        frameTarget = trajectoryBounds.center - Vector3.up * cameraElevation;
        frameStartSize = View.orthographicSize;
        frameTravelSize = SizeFor(trajectoryBounds, frameTarget);
        frameWideSize = Mathf.Max(frameStartSize, SizeFor(trajectoryBounds, frameStart), frameTravelSize);
        frameAspect = View.aspect;
        followVelocity = Vector3.zero;
        framePrepared = true;
        frameModeConfigured = false;
        frameCommitted = false;
    }
    public void UpdateFrameMove(float chooseProgress, Vector3 actor, Vector3 destination, float delta, bool wholeStage)
    {
        if (!framePrepared || delta <= 0) return;
        if (!frameModeConfigured)
        {
            if (wholeStage)
            {
                frameTarget = OverviewFocus;
                frameTravelSize = Mathf.Max(OverviewSize, SizeFor(trajectoryBounds, frameTarget));
                frameWideSize = Mathf.Max(frameStartSize, SizeFor(trajectoryBounds, frameStart), frameTravelSize);
            }
            frameModeConfigured = true;
        }
        // Resizing is allowed to widen the safe frame; ordinary travel never changes zoom.
        if (!Mathf.Approximately(frameAspect, View.aspect))
        {
            frameAspect = View.aspect;
            frameTravelSize = wholeStage ? Mathf.Max(OverviewSize, SizeFor(trajectoryBounds, frameTarget))
                : SizeFor(trajectoryBounds, frameTarget);
            frameWideSize = Mathf.Max(frameStartSize, SizeFor(trajectoryBounds, frameStart), frameTravelSize);
        }
        float progress = Mathf.Clamp01(chooseProgress);
        if (progress < 1)
        {
            float panEnd = Mathf.Min(.95f, preframeWidenFraction + preframePanFraction);
            if (progress < preframeWidenFraction)
                MoveCamera(frameStart, Mathf.Lerp(frameStartSize, frameWideSize,
                    Mathf.SmoothStep(0, 1, progress / preframeWidenFraction)));
            else if (progress < panEnd)
                MoveCamera(Vector3.Lerp(frameStart, frameTarget,
                    Mathf.SmoothStep(0, 1, (progress - preframeWidenFraction) / (panEnd - preframeWidenFraction))), frameWideSize);
            else
                MoveCamera(frameTarget, Mathf.Lerp(frameWideSize, frameTravelSize,
                    Mathf.SmoothStep(0, 1, (progress - panEnd) / (1 - panEnd))));
            followVelocity = Vector3.zero;
            return;
        }
        if (wholeStage || !frameCommitted)
        {
            MoveCamera(frameTarget, frameTravelSize);
            followVelocity = Vector3.zero;
            frameCommitted = true;
            return;
        }
        Vector3 desired = Vector3.Lerp(actor, destination, destinationLookAhead)
            + Vector3.up * (performerHeight * .5f - cameraElevation);
        Bounds visible = new Bounds(actor, Vector3.zero);
        visible.Encapsulate(actor + Vector3.up * performerHeight);
        visible.Encapsulate(destination);
        visible.Expand(new Vector3(platformRadius * 2, 0, platformRadius * 2));
        desired = ClampFocus(desired, visible, frameTravelSize);
        Vector3 current = ClampFocus(CameraFocus, visible, frameTravelSize);
        Vector3 smoothed = Vector3.SmoothDamp(current, desired, ref followVelocity, cameraFollowSeconds, Mathf.Infinity, delta);
        MoveCamera(ClampFocus(smoothed, visible, frameTravelSize), frameTravelSize);
    }
    float SizeFor(Bounds bounds, Vector3 focus)
    {
        Vector3 horizontal = Vector3.right;
        Vector3 vertical = Quaternion.Euler(CameraPitch, 0, 0) * Vector3.up;
        Vector3 relative = bounds.center - (focus + Vector3.up * cameraElevation);
        float halfWidth = Mathf.Abs(Vector3.Dot(relative, horizontal)) + bounds.extents.x;
        float halfHeight = Mathf.Abs(Vector3.Dot(relative, vertical))
            + Mathf.Abs(vertical.y) * bounds.extents.y + Mathf.Abs(vertical.z) * bounds.extents.z;
        return Mathf.Max(minimumFrameSize, halfWidth / Mathf.Max(.01f, View.aspect), halfHeight) / (1 - framingMargin);
    }
    Vector3 ClampFocus(Vector3 focus, Bounds bounds, float size)
    {
        Vector3 vertical = Quaternion.Euler(CameraPitch, 0, 0) * Vector3.up;
        Vector3 aim = focus + Vector3.up * cameraElevation;
        float safeHalfHeight = size * (1 - framingMargin);
        float halfWidth = safeHalfHeight * View.aspect;
        float verticalExtent = Mathf.Abs(vertical.y) * bounds.extents.y + Mathf.Abs(vertical.z) * bounds.extents.z;
        float centerY = Vector3.Dot(bounds.center, vertical), aimY = Vector3.Dot(aim, vertical);
        float minX = bounds.max.x - halfWidth, maxX = bounds.min.x + halfWidth;
        float minY = centerY + verticalExtent - safeHalfHeight, maxY = centerY - verticalExtent + safeHalfHeight;
        float x = minX <= maxX ? Mathf.Clamp(aim.x, minX, maxX) : bounds.center.x;
        float y = minY <= maxY ? Mathf.Clamp(aimY, minY, maxY) : centerY;
        return focus + Vector3.right * (x - aim.x) + vertical * (y - aimY);
    }
    public void ShowContact(int index, bool found, float delta, bool paused)
    {
        scenery.ShowContact(index, found, delta, paused);
    }
    public void ResetEffects()
    {
        framePrepared = false;
        followVelocity = Vector3.zero;
        scenery.ResetEffects();
    }
    public void ShowValues(int count)
    {
        int plannedRows = Mathf.CeilToInt(count / 8f);
        Columns = Mathf.CeilToInt(count / (float)plannedRows);
        Rows = Mathf.CeilToInt(count / (float)Columns);
        scenery.Layout(Columns, Rows, count);
    }
    public void Highlight(int left, int right, int probe, bool found) { scenery.Highlight(left, right, probe, found); }
}
