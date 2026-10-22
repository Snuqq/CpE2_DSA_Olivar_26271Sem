using UnityEngine;

public sealed class SearchTheatre : MonoBehaviour
{
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
        View.transform.position = center + Vector3.up * 1.3f + View.transform.rotation * Vector3.back * 12;
        View.orthographicSize = size;
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
