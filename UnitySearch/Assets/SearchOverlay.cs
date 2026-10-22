using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

public sealed class SearchOverlay : MonoBehaviour
{
    public SearchStage Stage;
    GUIStyle title, body, small, value, accent, comparison, cell, viewButton, plaqueValue, plaqueIndex;
    Font sans, serif;
    readonly Color gold = new Color(.91f, .76f, .43f);
    readonly Color ink = new Color(.035f, .06f, .12f);
    readonly GUIContent heading = new GUIContent("Furina's Search Theatre");
    readonly GUIContent wholeStageButton = new GUIContent("Whole stage");
    readonly GUIContent autoZoomButton = new GUIContent("Auto zoom");
    readonly GUIContent attribution = new GUIContent("Furina: X9_YT / CC BY 4.0 • MMD model: miHoYo    |    Speed changes apply to the next step.");
    readonly GUIContent ellipsis = new GUIContent("…");
    readonly GUIContent algorithmText = new GUIContent();
    readonly GUIContent targetText = new GUIContent();
    readonly GUIContent phaseText = new GUIContent();
    readonly GUIContent reasonText = new GUIContent();
    readonly GUIContent comparisonText = new GUIContent();
    readonly GUIContent messageText = new GUIContent();
    readonly GUIContent stageText = new GUIContent();
    readonly GUIContent rangeText = new GUIContent();
    readonly GUIContent tooltipText = new GUIContent();
    readonly ValueLabel[] labels = new ValueLabel[40];
    int labelCount, rangeLeft, rangeRight, comparisonFontSize;
    SearchStep cachedStep;
    SearchMotionPhase cachedPhase;
    string cachedAlgorithm;
    int cachedTarget;
    bool cachedRevealed, cachedPaused, cachedFinished, cachedWholeStage, stateCached;

    // Widths are measured only when data changes; camera zoom then selects a cached size.
    sealed class ValueLabel
    {
        public int Number;
        public readonly GUIContent Value = new GUIContent();
        public readonly GUIContent Index = new GUIContent();
        public readonly GUIContent StageIndex = new GUIContent();
        public readonly GUIContent Cell = new GUIContent();
        public readonly float[] PlaqueWidths = new float[9];
        public readonly float[] CellWidths = new float[5];
        public int CellFontSize;
    }

    string Introduction()
    {
        switch (Stage.Algorithm)
        {
            case "Jump Search": return "Jump by square-root-sized blocks, then scan the chosen block from left to right.";
            case "Exponential Search": return "Double the boundary to find a range, then halve it using binary search.";
            default: return "Split the range using consecutive Fibonacci numbers. Offset marks the last index known to be smaller than the target.";
        }
    }
    void Styles()
    {
        if (body != null) return;
        sans = Font.CreateDynamicFontFromOSFont("Segoe UI", 16);
        serif = Font.CreateDynamicFontFromOSFont("Georgia", 24);
        body = new GUIStyle(GUI.skin.label) { font = sans, fontSize = 14, wordWrap = true };
        body.normal.textColor = new Color(.88f, .92f, 1);
        small = new GUIStyle(body) { fontSize = 11 };
        value = new GUIStyle(body) { fontSize = 16, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        title = new GUIStyle(body) { font = serif, fontSize = 22 }; title.normal.textColor = gold;
        accent = new GUIStyle(body) { fontSize = 16, fontStyle = FontStyle.Bold }; accent.normal.textColor = gold;
        comparison = new GUIStyle(accent) { wordWrap = false };
        cell = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, fontSize = 14, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        plaqueValue = new GUIStyle(value) { wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        plaqueIndex = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter, wordWrap = false, padding = new RectOffset(0, 0, 0, 0) };
        plaqueIndex.normal.textColor = gold;
        viewButton = new GUIStyle(body) { alignment = TextAnchor.MiddleCenter, wordWrap = false, fontSize = 13 };
        viewButton.normal.textColor = gold;
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = new ValueLabel();
            labels[i].Index.text = i.ToString();
            labels[i].StageIndex.text = "INDEX " + i;
        }
    }
    static int FittingSize(float[] widths, int maximum, float available)
    {
        int size = maximum;
        while (size > 10 && widths[size - 10] > available) size--;
        return size;
    }
    bool CacheValues()
    {
        bool countChanged = labelCount != Stage.Values.Length;
        bool changed = countChanged;
        float available = 860f / Mathf.Min(10, Stage.Values.Length) - 30;
        for (int i = 0; i < Stage.Values.Length; i++)
        {
            var label = labels[i];
            bool newValue = i >= labelCount || label.Number != Stage.Values[i];
            if (newValue)
            {
                changed = true;
                label.Number = Stage.Values[i];
                label.Value.text = label.Number.ToString();
                label.Cell.tooltip = "Index " + i + " = " + label.Value.text;
                for (int size = 10; size <= 18; size++)
                {
                    plaqueValue.fontSize = size;
                    label.PlaqueWidths[size - 10] = plaqueValue.CalcSize(label.Value).x;
                    if (size > 14) continue;
                    cell.fontSize = size;
                    label.CellWidths[size - 10] = cell.CalcSize(label.Value).x;
                }
            }
            if (!newValue && !countChanged) continue;
            label.CellFontSize = FittingSize(label.CellWidths, 14, available);
            label.Cell.text = label.CellWidths[label.CellFontSize - 10] > available ? ellipsis.text : label.Value.text;
        }
        labelCount = Stage.Values.Length;
        return changed;
    }
    void CacheState(bool valuesChanged)
    {
        var step = Stage.Current;
        var phase = Stage.MotionPhase;
        bool revealed = Stage.ComparisonRevealed;
        bool paused = Stage.Playback.Paused, finished = Stage.Playback.Finished;
        if (stateCached && !valuesChanged && ReferenceEquals(step, cachedStep) && phase == cachedPhase
            && revealed == cachedRevealed && paused == cachedPaused && finished == cachedFinished
            && Stage.WholeStage == cachedWholeStage && Stage.Target == cachedTarget && Stage.Algorithm == cachedAlgorithm) return;

        cachedStep = step; cachedPhase = phase; cachedRevealed = revealed; cachedPaused = paused;
        cachedFinished = finished; cachedWholeStage = Stage.WholeStage;
        cachedTarget = Stage.Target; cachedAlgorithm = Stage.Algorithm; stateCached = true;
        int comparisons = step == null ? 0 : step.Comparisons - (revealed ? 0 : 1);
        algorithmText.text = Stage.Algorithm;
        targetText.text = "TARGET  " + Stage.Target + "     •     " + comparisons + " comparisons";
        phaseText.text = step == null ? "The stage is yours" : step.Phase;
        reasonText.text = step == null ? Introduction() : step.Reason;
        comparisonText.text = step == null ? "Sorted • zero-based indexes" : revealed ? step.Comparison : "Checking index " + step.Index + "...";
        comparison.fontSize = 16;
        while (comparison.fontSize > 10 && comparison.CalcSize(comparisonText).x > 268) comparison.fontSize--;
        comparisonFontSize = comparison.fontSize;
        messageText.text = step == null ? "Gold: checking    Green: found\nDimmed: ruled out"
            : revealed ? step.Message : "Watch the chosen position before revealing the comparison.";
        string beat = paused ? "PAUSED" : finished ? "COMPLETE" : step == null ? "READY" : phase.ToString().ToUpperInvariant();
        string stageRange = Stage.WholeStage || step == null || finished
            ? "ALL " + labelCount + " POSITIONS · VALUES BELOW"
            : step.Terminal ? "SEARCH RESULT" : "FOCUSING INDEX " + step.Index;
        stageText.text = beat + "     /     " + stageRange;
        rangeLeft = step == null ? 0 : revealed ? step.NextLeft : step.Left;
        rangeRight = step == null ? labelCount - 1 : revealed ? step.NextRight : step.Right;
        string range = rangeLeft > rangeRight ? "No candidates remain" : rangeLeft == rangeRight
            ? "Candidate index " + rangeLeft : "Candidate indexes " + rangeLeft + "–" + rangeRight;
        rangeText.text = "ARRAY  ·  index | value     /     " + (step == null ? "Ready to search" : range);
    }
    void Fill(Rect rect, Color color) { var old = GUI.color; GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = old; }
    void Frame(Rect rect, Color background, Color border)
    {
        Fill(rect, border); Fill(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), background);
    }
    Rect PerformerRect(Camera camera, float sx, float sy)
    {
        var bounds = Stage.Theatre.Actor.WorldBounds;
        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        for (int corner = 0; corner < 8; corner++)
        {
            var point = bounds.center + Vector3.Scale(bounds.extents,
                new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, (corner & 4) == 0 ? -1 : 1));
            var screen = camera.WorldToScreenPoint(point);
            var projected = new Vector2(screen.x / sx, (Screen.height - screen.y) / sy - 58);
            min = Vector2.Min(min, projected); max = Vector2.Max(max, projected);
        }
        return Rect.MinMaxRect(min.x - 3, min.y - 3, max.x + 3, max.y + 3);
    }
    void OnGUI()
    {
        if (Stage == null || Stage.Values == null || Stage.Values.Length == 0) return;
        Styles();
        float sx = Screen.width / 900f, sy = Screen.height / 480f;
        var previousMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(sx, sy, 1));
        bool repaint = Event.current.type == EventType.Repaint;
        var viewRect = new Rect(424, 12, 148, 29);
        if (repaint)
        {
            Fill(new Rect(0, 0, 900, 58), ink);
            Frame(viewRect, viewRect.Contains(Event.current.mousePosition) ? new Color(.12f, .23f, .30f) : new Color(.055f, .12f, .18f), gold * .65f);
        }
        // Keep this control in every event so keyboard focus, clicks and hover still work.
        if (GUI.Button(viewRect, Stage.WholeStage ? autoZoomButton : wholeStageButton, viewButton)) Stage.SetWholeStage(!Stage.WholeStage);
        if (repaint)
        {
            CacheState(CacheValues());
            DrawOverlay(sx, sy);
        }
        GUI.matrix = previousMatrix;
    }
    void DrawOverlay(float sx, float sy)
    {
        var step = cachedStep;
        GUI.Label(new Rect(20, 9, 540, 31), heading, title);
        GUI.Label(new Rect(595, 10, 290, 25), algorithmText, accent);
        GUI.Label(new Rect(595, 35, 280, 22), targetText, small);
        Fill(new Rect(18, 54, 864, 1), gold * .7f);
        Fill(new Rect(585, 61, 315, 289), ink);
        Fill(new Rect(586, 69, 1, 263), gold * .3f);
        Fill(new Rect(600, 68, 22, 1), gold * .6f);
        GUI.Label(new Rect(606, 74, 270, 25), phaseText, accent);
        GUI.Label(new Rect(606, 108, 268, 90), reasonText, body);
        comparison.fontSize = comparisonFontSize;
        GUI.Label(new Rect(606, 207, 268, 28), comparisonText, comparison);
        GUI.Label(new Rect(606, 242, 268, 68), messageText, body);
        Fill(new Rect(15, 63, 554, 22), new Color(.025f, .045f, .08f, .86f));
        GUI.Label(new Rect(20, 65, 540, 23), stageText, small);
        if (step != null)
        {
            Fill(new Rect(606, 326, 268, 3), new Color(.15f, .2f, .3f));
            Fill(new Rect(606, 326, 268 * (float)Stage.Playback.Progress, 3), gold);
        }
        DrawStageLabels(sx, sy);
        Fill(new Rect(0, 350, 900, 130), ink); Fill(new Rect(18, 350, 864, 1), gold * .5f);
        GUI.Label(new Rect(20, 354, 860, 19), rangeText, small);
        int columns = Mathf.Min(10, labelCount), rows = (labelCount + columns - 1) / columns;
        float width = 860f / columns, height = Mathf.Min(37, 76f / rows);
        string hoveredTooltip = string.Empty;
        Vector2 mouse = Event.current.mousePosition;
        plaqueIndex.fontSize = 10;
        for (int i = 0; i < labelCount; i++)
        {
            var label = labels[i];
            var rect = new Rect(20 + i % columns * width, 377 + i / columns * height, width - 4, height - 2);
            Color color = i < rangeLeft || i > rangeRight ? new Color(.08f, .10f, .15f) : new Color(.09f, .19f, .31f);
            if (step != null && i == step.Index) color = step.Found && cachedRevealed ? new Color(.1f, .45f, .31f) : new Color(.45f, .30f, .10f);
            Frame(rect, color, new Color(.19f, .25f, .29f));
            GUI.Label(new Rect(rect.x + 2, rect.y, 17, rect.height), label.Index, plaqueIndex);
            Fill(new Rect(rect.x + 20, rect.y + 4, 1, Mathf.Max(1, rect.height - 8)), new Color(.28f, .34f, .37f));
            cell.fontSize = label.CellFontSize;
            var valueRect = new Rect(rect.x + 23, rect.y, rect.width - 26, rect.height);
            GUI.Label(valueRect, label.Cell, cell);
            if (valueRect.Contains(mouse)) hoveredTooltip = label.Cell.tooltip;
        }
        if (tooltipText.text != hoveredTooltip) tooltipText.text = hoveredTooltip;
        if (!string.IsNullOrEmpty(tooltipText.text))
        {
            Fill(new Rect(570, 352, 310, 23), ink);
            GUI.Label(new Rect(580, 354, 300, 22), tooltipText, body);
        }
        GUI.Label(new Rect(20, 457, 860, 20), attribution, small);
    }
    void DrawStageLabels(float sx, float sy)
    {
        var camera = Stage.Theatre.View;
        var origin = camera.WorldToScreenPoint(Vector3.zero);
        float spacing = (camera.WorldToScreenPoint(Vector3.right * 2.4f).x - origin.x) / sx;
        if (spacing < 30) return;
        float rowSpacing = Mathf.Abs(camera.WorldToScreenPoint(Vector3.forward * 3.2f).y - origin.y) / sy;
        bool compact = (Stage.Theatre.Rows > 1 && rowSpacing < 50) || spacing < 65;
        var performer = PerformerRect(camera, sx, sy);
        float performerDepth = camera.WorldToScreenPoint(Stage.Theatre.Actor.transform.position).z;
        float labelWidth = Mathf.Min(104, spacing - 4);
        float available = compact ? labelWidth - 18 : labelWidth - 8;
        GUI.BeginGroup(new Rect(0, 58, 585, 292));
        for (int i = 0; i < labelCount; i++)
        {
            var label = labels[i];
            var screen = camera.WorldToScreenPoint(Stage.Theatre.Position(i) + new Vector3(0, .12f, -.9f));
            var rect = new Rect(screen.x / sx - labelWidth * .5f, (Screen.height - screen.y) / sy - 58, labelWidth, compact ? 18 : 39);
            // The full array stays below; stage plaques must not obscure the performer.
            if (screen.z > performerDepth && rect.Overlaps(performer)) continue;
            bool selected = cachedStep != null && cachedStep.Index == i;
            Color border = selected ? (cachedStep.Found ? new Color(.36f, .96f, .72f) : gold) : new Color(.38f, .33f, .24f);
            Frame(rect, new Color(.035f, .075f, .11f, .96f), border);
            plaqueValue.normal.textColor = selected ? Color.white : new Color(.86f, .91f, .94f);
            plaqueValue.fontSize = FittingSize(label.PlaqueWidths, compact ? 12 : 18, available);
            if (compact)
            {
                plaqueIndex.fontSize = 9;
                GUI.Label(new Rect(rect.x + 1, rect.y, 13, 18), label.Index, plaqueIndex);
                Fill(new Rect(rect.x + 14, rect.y + 4, 1, 10), border);
                GUI.Label(new Rect(rect.x + 16, rect.y, available, 18),
                    label.PlaqueWidths[plaqueValue.fontSize - 10] > available ? ellipsis : label.Value, plaqueValue);
                continue;
            }
            GUI.Label(new Rect(rect.x + 4, rect.y, available, 24), label.Value, plaqueValue);
            plaqueIndex.fontSize = 10;
            GUI.Label(new Rect(rect.x, rect.y + 23, rect.width, 14), label.StageIndex, plaqueIndex);
        }
        GUI.EndGroup();
    }
    void OnDestroy()
    {
        if (sans != null) { if (Application.isPlaying) Destroy(sans); else DestroyImmediate(sans); }
        if (serif != null) { if (Application.isPlaying) Destroy(serif); else DestroyImmediate(serif); }
    }
}
