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
    [SerializeField] bool logFramePacing;
    readonly SearchPerformanceProbe performance = new SearchPerformanceProbe();
    public SearchMotionSettings Motion { get { return motion; } }
    public SearchMotionPlan MotionPlan { get; private set; }
    public SearchMotionPhase MotionPhase { get { return MotionPlan == null ? SearchMotionPhase.Choose : MotionPlan.PhaseAt(Playback.Elapsed); } }
    public bool ComparisonRevealed { get { return Current != null && (Current.Terminal || (MotionPlan != null && Playback.Elapsed >= MotionPlan.StartOf(SearchMotionPhase.Compare))); } }
    public int OccupiedIndex
    {
        get
        {
            if (Current == null || MotionPlan == null) return -1;
            return Current.Terminal || Playback.Elapsed >= MotionPlan.StartOf(SearchMotionPhase.Land)
                ? Current.Index : -1;
        }
    }
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
    public void SetWholeStage(bool enabled)
    {
        pendingWholeStage = enabled;
        if (!Playback.Moving && !Playback.Paused && !resetting) BeginViewChange();
    }
    bool recording;
    bool restartClock;
    double lastPlaybackTime;
    double idleTime, resetElapsed, viewElapsed;
    float presentationDelta;
    bool resetting, viewChanging, scanMove;
    bool? pendingWholeStage;
    SearchPlan pendingPlan;
    string deferredPlayback;
    Vector3 resetDestination;
    public bool Resetting { get { return resetting; } }
    public double ResetProgress { get { return resetting ? Math.Min(1, resetElapsed / motion.resetSeconds) : 1; } }
    int displayWidth, displayHeight;
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
        // Hardware pacing only when the display can divide evenly to 60 Hz.
        double refresh = Screen.currentResolution.refreshRateRatio.value;
        int interval = (int)Math.Round(refresh / 60.0);
        QualitySettings.vSyncCount = interval >= 1 && interval <= 4 && Math.Abs(refresh / interval - 60) < .5 ? interval : 0;
        UnityEngine.Rendering.OnDemandRendering.renderFrameInterval = 1;
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
    public void ResetPlayback()
    {
        if (Plan != null) BeginReset(Plan);
    }
    void BeginReset(SearchPlan plan)
    {
        pendingPlan = plan;
        deferredPlayback = null;
        resetElapsed = 0;
        resetting = true; viewChanging = false;
        pendingWholeStage = null;
        restartClock = true;
        resetDestination = Theatre.Position(0);
        Theatre.Actor.CaptureResetPose();
        Theatre.ResetEffects();
        Theatre.Highlight(0, Values.Length - 1, -1, false);
        Theatre.BeginFrameMove(Theatre.Actor.transform.position, resetDestination, motion.minimumHopHeight, motion);
        MotionPlan = null;
        Playback.Load(plan.Steps);
    }
    void DrawReset()
    {
        float preframe = Mathf.Min(motion.chooseSeconds, motion.resetSeconds * motion.resetPreframeFraction);
        float choose = preframe > 0 ? Mathf.Clamp01((float)resetElapsed / preframe) : 1;
        float progress = Mathf.Clamp01((float)(resetElapsed - preframe) / Mathf.Max(.001f, motion.resetSeconds - preframe));
        Theatre.Actor.ConfigurePresentation(false, idleTime, presentationDelta, resetDestination);
        Theatre.Actor.PoseReset(progress, resetDestination, motion);
        Theatre.UpdateFrameMove(choose, Theatre.Actor.transform.position, resetDestination, presentationDelta, true);
        if (resetElapsed < motion.resetSeconds) return;
        SearchPlan plan = pendingPlan;
        string command = deferredPlayback;
        bool paused = Playback.Paused;
        resetting = false; pendingPlan = null; deferredPlayback = null;
        Load(plan);
        if (command == "PLAY") Playback.Play();
        else if (command == "STEP") Playback.Step();
        if (paused) Playback.Pause();
    }
    void BeginViewChange()
    {
        if (!pendingWholeStage.HasValue) return;
        WholeStage = pendingWholeStage.Value; pendingWholeStage = null;
        viewElapsed = 0; viewChanging = true;
        var position = Theatre.Actor.transform.position;
        Theatre.BeginFrameMove(position, position, 0, motion);
    }
    public void Load(SearchPlan plan)
    {
        if (plan == null) throw new ArgumentNullException("plan");
        plan.Validate();
        Plan = plan; Values = plan.Values; Target = plan.Target; Algorithm = plan.Algorithm;
        MotionPlan = null; loggedPhase = null;
        idleTime = 0;
        resetting = viewChanging = false;
        presentationDelta = 0;
        restartClock = true;
        Theatre.ResetEffects();
        Theatre.ShowValues(Values.Length); Theatre.Actor.transform.position = Theatre.Position(0);
        Theatre.MoveCamera(Theatre.OverviewFocus, Theatre.OverviewSize);
        Theatre.Actor.transform.rotation = Quaternion.identity; Theatre.Actor.ResetPose();
        Theatre.Actor.ConfigurePresentation(false, idleTime, 0, Theatre.Position(0));
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
        double delta = restartClock ? 0 : Math.Max(0, now - lastPlaybackTime);
        lastPlaybackTime = now;
        AdvancePresentation(delta);
        performance.EndFrame();
    }
    public void AdvancePresentation(double delta)
    {
        if (double.IsNaN(delta) || double.IsInfinity(delta) || delta < 0) return;
        presentationDelta = Playback.Paused ? 0 : (float)delta;
        idleTime += presentationDelta;
        if (resetting)
        {
            resetElapsed += presentationDelta;
            DrawReset();
        }
        else
        {
            Playback.Tick(delta);
            DrawPose();
        }
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
            if (id < session) return;
            session = id;
            if (resetting)
            {
                pendingPlan = plan; deferredPlayback = null; Playback.Load(plan.Steps);
            }
            else if (Plan != null && (Playback.Current >= 0 || Vector3.Distance(Theatre.Actor.transform.position, Theatre.Position(0)) > .001f))
                BeginReset(plan);
            else Load(plan);
            return;
        }
        if (parts[0] == "RESET") { if (id < session) return; session = id; ResetPlayback(); return; }
        if (id != session) return;
        switch (parts[0])
        {
            case "PLAY":
                bool resumePlay = resetting || Playback.Paused || !Playback.Moving;
                if (resetting) { deferredPlayback = "PLAY"; Playback.Load(Playback.Steps); }
                else Playback.Play();
                restartClock |= resumePlay; break;
            case "PAUSE": Playback.Pause(); break;
            case "STEP":
                bool resumeStep = resetting || Playback.Paused || !Playback.Moving;
                if (resetting) { deferredPlayback = "STEP"; Playback.Load(Playback.Steps); }
                else Playback.Step();
                restartClock |= resumeStep; break;
            case "SPEED": Playback.Duration = Math.Max(motion.minimumStepSeconds, Math.Min(motion.maximumStepSeconds, double.Parse(parts[2], CultureInfo.InvariantCulture))); break;
            case "CAPTURE": StartCoroutine(CaptureProof()); break;
            case "VIEW": SetWholeStage(parts[2] == "wide"); break;
            case "PERF":
                performance.LogEvidence = logFramePacing;
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
            + "|" + Theatre.Actor.MaxPlantedDrift.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Theatre.Actor.transform.position.y.ToString("R", CultureInfo.InvariantCulture)
            + "|" + Bit(resetting) + "|" + ResetProgress.ToString("R", CultureInfo.InvariantCulture)
            + "|" + idleTime.ToString("R", CultureInfo.InvariantCulture) + "|" + OccupiedIndex);
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
        if (resetting) message = "Resetting — returning to the starting platform.";
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
        if (pendingWholeStage.HasValue) { WholeStage = pendingWholeStage.Value; pendingWholeStage = null; }
        viewChanging = false;
        scanMove = step.Phase != null && step.Phase.IndexOf("linear scan", StringComparison.OrdinalIgnoreCase) >= 0;
        MotionPlan = step.Terminal ? motion.Result() : motion.Probe(Vector3.Distance(from, to), requestedSeconds, scanMove);
        if (!step.Terminal) Theatre.BeginFrameMove(from, to, motion.maximumHopHeight, motion);
        Theatre.Actor.BeginMotion(); loggedPhase = null;
        return MotionPlan.Duration;
    }
    public void DrawPose()
    {
        if (resetting) { DrawReset(); return; }
        if (pendingWholeStage.HasValue && !Playback.Paused && (!Playback.Moving || MotionPhase == SearchMotionPhase.Compare)) BeginViewChange();
        if (displayWidth != Screen.width || displayHeight != Screen.height)
        {
            displayWidth = Screen.width; displayHeight = Screen.height;
            if (!Playback.Moving) Theatre.BeginFrameMove(Theatre.Actor.transform.position, Theatre.Actor.transform.position, 0, motion);
        }
        var step = Current;
        if (step == null)
        {
            Theatre.Actor.ConfigurePresentation(false, idleTime, presentationDelta, Theatre.Position(0));
            Theatre.Actor.Pose(null, 0, Theatre.Position(0), Theatre.Position(0), false, false, motion);
            if (viewChanging) UpdateViewChange(Theatre.Actor.transform.position);
            return;
        }
        double elapsed = Playback.Elapsed;
        Theatre.Highlight(ComparisonRevealed ? step.NextLeft : step.Left, ComparisonRevealed ? step.NextRight : step.Right,
            OccupiedIndex, ComparisonRevealed && step.Found);
        var actor = Theatre.Actor;
        bool overshoot = !step.Terminal && !scanMove && ComparisonRevealed && step.Index >= 0
            && Values[step.Index] >= Target;
        actor.ConfigurePresentation(scanMove, idleTime, presentationDelta, to, overshoot);
        actor.Pose(MotionPlan, elapsed, from, to, step.Terminal, step.Found, motion);
        Theatre.ShowContact(OccupiedIndex, step.Found && ComparisonRevealed, presentationDelta, Playback.Paused);
        if (viewChanging) UpdateViewChange(actor.transform.position);
        else if (step.Terminal)
        {
            float outro = Mathf.SmoothStep(0, 1, MotionPlan.ProgressOf(SearchMotionPhase.Pullback, elapsed));
            // Center first, then zoom; the character has already settled.
            float pan = Mathf.SmoothStep(0, 1, Mathf.Clamp01(outro * 2));
            float zoom = Mathf.SmoothStep(0, 1, Mathf.Clamp01(outro * 2 - 1));
            Theatre.MoveCamera(Vector3.Lerp(cameraFrom, Theatre.OverviewFocus, pan), Mathf.Lerp(zoomFrom, Theatre.OverviewSize, zoom));
        }
        else
        {
            float choose = MotionPlan.ProgressOf(SearchMotionPhase.Choose, elapsed);
            Theatre.UpdateFrameMove(choose, actor.transform.position, to, presentationDelta, WholeStage);
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
    void UpdateViewChange(Vector3 position)
    {
        viewElapsed += presentationDelta;
        float choose = Mathf.Clamp01((float)viewElapsed / Math.Max(.001f, motion.chooseSeconds));
        Theatre.UpdateFrameMove(choose, position, position, presentationDelta, WholeStage);
        if (choose >= 1) viewChanging = false;
    }
    void OnDestroy() { if (connection != null) connection.Dispose(); }
}
