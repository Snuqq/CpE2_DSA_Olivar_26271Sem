using System;
using System.Collections.Generic;

namespace CpE2_DSA_Olivar_26271Sem
{
    // The single playback clock is used by Unity and tested without a graphics runtime.
    public sealed class SearchPlayback
    {
        public List<SearchStep> Steps { get; private set; }
        public int Current { get; private set; }
        public double Progress { get; private set; }
        public bool Moving { get; private set; }
        public bool Paused { get; private set; }
        public bool Playing { get; private set; }
        public bool Finished { get { return Steps != null && Current == Steps.Count - 1 && !Moving; } }
        double duration;
        public double Duration
        {
            get { return duration; }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0) throw new ArgumentOutOfRangeException("value");
                duration = value;
            }
        }
        double beatDuration;
        public double Elapsed { get { return Progress * beatDuration; } }
        public double BeatDuration { get { return beatDuration; } }
        public Func<int, double, double> DurationForStep;
        public event Action Changed;

        public void Load(List<SearchStep> steps)
        {
            Steps = steps; Current = -1; Progress = 0; Moving = Playing = Paused = false;
            Notify();
        }
        public void Play() { if (Finished) return; Playing = true; Paused = false; if (!Moving) Begin(); else Notify(); }
        public void Pause() { Playing = false; Paused = true; Notify(); }
        public void Step()
        {
            if (Finished) return;
            Playing = false; Paused = false;
            if (!Moving) Begin(); else Notify();
        }
        void Begin()
        {
            if (Steps == null || Current + 1 >= Steps.Count) return;
            Current++; Progress = 0; Moving = true;
            // Snapshot duration: changing speed cannot rewind or snap an in-flight pose.
            beatDuration = DurationForStep == null ? Duration : DurationForStep(Current, Duration);
            if (double.IsNaN(beatDuration) || double.IsInfinity(beatDuration) || beatDuration <= 0)
                throw new InvalidOperationException("Configure a positive duration before playback.");
            Notify();
        }
        public void Tick(double delta)
        {
            if (Paused || delta <= 0 || double.IsNaN(delta) || double.IsInfinity(delta)) return;
            if (!Moving) { if (Playing && !Finished) Begin(); return; }
            Progress = Math.Min(1, Progress + delta / beatDuration);
            if (Progress >= 1) { Moving = false; if (Finished) Playing = false; Notify(); }
        }
        void Notify() { if (Changed != null) Changed(); }
    }
}
