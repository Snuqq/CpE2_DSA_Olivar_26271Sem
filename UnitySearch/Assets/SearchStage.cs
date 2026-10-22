using System;
using System.Globalization;
using System.Collections;
using System.IO;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

public class SearchStage : MonoBehaviour
{
    [SerializeField] SearchMotionSettings motion = new SearchMotionSettings();
    [SerializeField, Min(1)] float maximumRecordingSeconds = 30f;
    [SerializeField, Min(.001f)] float recordingSampleSeconds = 1f / 30f;
    [SerializeField, Min(0)] float diagnosticWarmupSeconds = 1f;
    readonly SearchPerformanceProbe performance = new SearchPerformanceProbe();
    public SearchMotionSettings Motion { get { return motion; } }
    public SearchMotionPlan MotionPlan { get; private set; }
    public SearchMotionPhase MotionPhase { get { return MotionPlan == null ? SearchMotionPhase.Choose : MotionPlan.PhaseAt(Playback.Elapsed); } }
    public bool ComparisonRevealed { get { return Current != null && (Current.Terminal || (MotionPlan != null && Playback.Elapsed >= MotionPlan.StartOf(SearchMotionPhase.Compare))); } }
    SearchMotionPhase? loggedPhase;
    public readonly SearchPlayback Playback = new SearchPlayback();
    public SearchTheatre Theatre { get; private set; }
    public int[] Values { get; private set; }
    public int Target { get; private set; }
    public string Algorithm { get; private set; }
    public SearchPlan Plan { get; private set; }
    public SearchStep Current { get { return Playback.Current >= 0 ? Playback.Steps[Playback.Current] : null; } }
    SearchConnection connection;
    int session;
    Vector3 from, to, cameraFrom;
    float zoomFrom;
    public bool WholeStage { get; private set; }
    public void SetWholeStage(bool enabled) { WholeStage = enabled; DrawPose(); }
    bool recording;
    bool restartClock;
    double lastPlaybackTime;
    SearchMotionPlan drawnPlan;
    double drawnElapsed = -1;
    bool drawnWholeStage;
    int drawnWidth, drawnHeight;
    public string Error { get; private set; }
    void Start()
    {
        try { Initialize(); StartCoroutine(ReadyAfterFirstFrame()); }
        catch (Exception ex)
        {
            Error = ex.Message;
            if (connection != null) connection.Send("ERROR|" + ex.Message.Replace('|', '/'));
            Debug.LogException(ex); enabled = false;
        }
    }
    IEnumerator ReadyAfterFirstFrame()
    {
        yield return new WaitForEndOfFrame();
        connection.Send("LOADED");
    }
    public void Initialize()
    {
        if (Theatre != null) return;
        Application.targetFrameRate = 60; Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 4;
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowDistance = 35;
        connection = new SearchConnection();
        Theatre = new GameObject("Blue and gold theatre").AddComponent<SearchTheatre>();
        Theatre.transform.SetParent(transform); Theatre.Initialize();
        gameObject.AddComponent<SearchOverlay>().Stage = this;
        Playback.Duration = motion.defaultStepSeconds;
        Playback.DurationForStep = PrepareMotion;
        Playback.Changed += Report;
        var args = Environment.GetCommandLineArgs();
        int algorithmAt = Array.IndexOf(args, "-searchAlgorithm");
        string algorithm = algorithmAt >= 0 && algorithmAt + 1 < args.Length ? args[algorithmAt + 1] : "Fibonacci Search";
        LoadSample(algorithm, 13);
        lastPlaybackTime = Time.realtimeSinceStartupAsDouble;
    }
    public void LoadSample(string algorithm, int target)
    {
        string name = algorithm.Replace(" Search", "") + "-" + target.ToString(CultureInfo.InvariantCulture);
        var sample = Resources.Load<TextAsset>("SearchSamples/" + name);
        if (sample == null) throw new FileNotFoundException("Missing search preview: " + name);
        Load(SearchPlan.Deserialize(sample.text));
    }
    public void ResetPlayback() { Load(Plan); }
    public void Load(SearchPlan plan)
    {
        if (plan == null) throw new ArgumentNullException("plan");
        plan.Validate();
        Plan = plan; Values = plan.Values; Target = plan.Target; Algorithm = plan.Algorithm;
        MotionPlan = null; loggedPhase = null;
        drawnElapsed = -1;
        Theatre.ShowValues(Values.Length); Theatre.Actor.transform.position = Theatre.Position(0);
        Theatre.MoveCamera(Theatre.OverviewFocus, Theatre.OverviewSize);
        Theatre.Actor.transform.rotation = Quaternion.identity; Theatre.Actor.ResetPose();
        Theatre.Actor.Pose(null, 0, Theatre.Position(0), Theatre.Position(0), false, false, motion);
        Theatre.Highlight(0, Values.Length - 1, -1, false); Playback.Load(plan.Steps);
    }
    void Update()
    {
        if (connection == null) return;
        if (connection.Embedded && connection.Error != null) { Application.Quit(); return; }
        performance.BeginFrame();
        restartClock = false;
        string message;
        while (connection.Commands.TryDequeue(out message))
        {
            try { Command(message); }
            catch (Exception ex) { Error = ex.Message; connection.Send("ERROR|" + ex.Message.Replace('|', '/')); }
        }
        // A newly accepted command must not consume time spent loading the previous frame.
        double now = Time.realtimeSinceStartupAsDouble;
        Playback.Tick(restartClock ? 0 : now - lastPlaybackTime);
        lastPlaybackTime = now;
        DrawPose();
        performance.EndFrame();
    }
    void Command(string message)
    {
        var parts = message.StartsWith("LOAD|") ? message.Split(new[] { '|' }, 3) : message.Split('|');
        if (parts.Length < 2) throw new ArgumentException("Incomplete command.");
        int id = int.Parse(parts[1], CultureInfo.InvariantCulture);
        if (parts[0] == "LOAD")
        {
            if (parts.Length != 3) throw new ArgumentException("Incomplete search plan.");
            var plan = SearchPlan.Deserialize(parts[2]);
            session = id; Load(plan); return;
        }
        if (parts[0] == "RESET") { session = id; ResetPlayback(); return; }
        if (id != session) return;
        switch (parts[0])
        {
            case "PLAY": Playback.Play(); restartClock = true; break;
            case "PAUSE": Playback.Pause(); break;
            case "STEP": Playback.Step(); restartClock = true; break;
            case "SPEED": Playback.Duration = Math.Max(motion.minimumStepSeconds, Math.Min(motion.maximumStepSeconds, double.Parse(parts[2], CultureInfo.InvariantCulture))); break;
            case "CAPTURE": StartCoroutine(CaptureProof()); break;
            case "VIEW": SetWholeStage(parts[2] == "wide"); break;
            case "PERF":
                performance.Begin(session, parts[2], double.Parse(parts[3], CultureInfo.InvariantCulture),
                    connection.Send, diagnosticWarmupSeconds);
                break;
            case "RECORD":
                if (!recording) StartCoroutine(RecordProof(Mathf.Clamp(float.Parse(parts[2], CultureInfo.InvariantCulture), recordingSampleSeconds, maximumRecordingSeconds)));
                break;
            default: throw new ArgumentException("Unknown search command.");
        }
    }
    IEnumerator CaptureProof()
    {
        yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        string path = Path.Combine(Application.dataPath, "..", "SearchProof.png");
        File.WriteAllBytes(path, image.EncodeToPNG()); Destroy(image);
        connection.Send("CAPTURED|" + session + "|" + Theatre.Actor.transform.position.x.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.CameraCenter.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.View.orthographicSize.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.Actor.transform.position.z.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.CameraFocus.z.ToString("R", CultureInfo.InvariantCulture)
            + "|" + MotionPhase + "|" + Playback.Elapsed.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.Actor.MaxPlantedDrift.ToString("R", CultureInfo.InvariantCulture));
    }
    IEnumerator RecordProof(float duration)
    {
        recording = true;
        string folder = "MotionProof-" + DateTime.UtcNow.Ticks;
        string path = Path.Combine(Application.dataPath, "..", folder);
        Directory.CreateDirectory(path);
        float start = Time.realtimeSinceStartup, next = 0;
        int frame = 0;
        using (var timeline = new StreamWriter(Path.Combine(path, "times.csv")))
        {
            while (Time.realtimeSinceStartup - start < duration)
            {
                yield return new WaitForEndOfFrame();
                float elapsed = Time.realtimeSinceStartup - start;
                if (elapsed < next) continue;
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                string name = "frame-" + frame++.ToString("D4") + ".png";
                File.WriteAllBytes(Path.Combine(path, name), image.EncodeToPNG()); Destroy(image);
                timeline.WriteLine(name + "," + elapsed.ToString("R", CultureInfo.InvariantCulture));
                next = elapsed + recordingSampleSeconds;
            }
        }
        recording = false; connection.Send("RECORDED|" + session + "|" + folder);
    }
    void Report()
    {
        if (connection == null) return;
        var step = Current;
        string message = step == null ? "Ready. Press Start or Next Step." :
            "Step " + (Playback.Current + 1) + "/" + Playback.Steps.Count + " — " +
            (Playback.Moving && !step.Terminal ? step.Phase + ": checking index " + step.Index + "." : step.Message);
        if (Playback.Paused) message = "Paused — " + message;
        connection.Send("STATE|" + session + "|" + Playback.Current + "|" + Bit(Playback.Moving) + "|" +
            Bit(Playback.Paused) + "|" + Bit(Playback.Playing) + "|" + (step == null ? 0 : step.Comparisons) + "|" + Bit(Playback.Finished) + "|" + message);
    }
    static string Bit(bool value) { return value ? "1" : "0"; }
    double PrepareMotion(int index, double requestedSeconds)
    {
        var step = Playback.Steps[index];
        from = Theatre.Actor.transform.position;
        to = step.Index >= 0 ? Theatre.Position(step.Index) : from;
        cameraFrom = Theatre.CameraFocus; zoomFrom = Theatre.View.orthographicSize;
        MotionPlan = step.Terminal ? motion.Result() : motion.Probe(Vector3.Distance(from, to), requestedSeconds);
        Theatre.Actor.BeginMotion(); loggedPhase = null;
        return MotionPlan.Duration;
    }
    public void DrawPose()
    {
        if (drawnPlan == MotionPlan && drawnElapsed == Playback.Elapsed && drawnWholeStage == WholeStage
            && drawnWidth == Screen.width && drawnHeight == Screen.height) return;
        drawnPlan = MotionPlan; drawnElapsed = Playback.Elapsed; drawnWholeStage = WholeStage;
        drawnWidth = Screen.width; drawnHeight = Screen.height;
        var step = Current;
        if (step == null)
        {
            Theatre.MoveCamera(Theatre.OverviewFocus, Theatre.OverviewSize);
            Theatre.Actor.Pose(null, 0, Theatre.Position(0), Theatre.Position(0), false, false, motion); return;
        }
        double elapsed = Playback.Elapsed;
        Theatre.Highlight(ComparisonRevealed ? step.NextLeft : step.Left, ComparisonRevealed ? step.NextRight : step.Right,
            step.Index, ComparisonRevealed && step.Found);
        var actor = Theatre.Actor;
        actor.Pose(MotionPlan, elapsed, from, to, step.Terminal, step.Found, motion);
        if (step.Terminal)
        {
            float outro = WholeStage ? 1 : Mathf.SmoothStep(0, 1, MotionPlan.ProgressOf(SearchMotionPhase.Pullback, elapsed));
            Theatre.MoveCamera(Vector3.Lerp(cameraFrom, Theatre.OverviewFocus, outro), Mathf.Lerp(zoomFrom, Theatre.OverviewSize, outro));
        }
        else
        {
            float choose = Mathf.SmoothStep(0, 1, MotionPlan.ProgressOf(SearchMotionPhase.Choose, elapsed));
            Vector3 focus = (actor.transform.position + to) * .5f;
            float size = Theatre.FrameSize(actor.transform.position, to);
            Theatre.MoveCamera(WholeStage ? Theatre.OverviewFocus : Vector3.Lerp(cameraFrom, focus, choose),
                WholeStage ? Theatre.OverviewSize : Mathf.Lerp(zoomFrom, size, choose));
        }
        if (motion.logMotionPhases && loggedPhase != MotionPhase)
        {
            loggedPhase = MotionPhase;
            Debug.Log(string.Format(CultureInfo.InvariantCulture,
                "MOTION step={0} phase={1} scheduled={2:F4}s duration={3:F4}s planted={4} drift={5:F6}m max={6:F6}m supportY={7:F3} pelvisDrop={8:F3}",
                Playback.Current, MotionPhase, MotionPlan.StartOf(MotionPhase), MotionPlan.LengthOf(MotionPhase), actor.FeetPlanted,
                actor.PlantedDrift, actor.MaxPlantedDrift, actor.SupportHeight, actor.PelvisDrop));
        }
    }
    void OnDestroy() { if (connection != null) connection.Dispose(); }
}
