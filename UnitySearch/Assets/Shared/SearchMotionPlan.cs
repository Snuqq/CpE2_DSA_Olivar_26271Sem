using System;

namespace CpE2_DSA_Olivar_26271Sem
{
    public enum SearchMotionPhase
    {
        Choose, Anticipate, Takeoff, Travel, Land, Settle, Compare,
        Look, Register, React, ReactionSettle, CameraHold, Pullback, ResultHold
    }

    // Presentation-only schedule. It neither owns nor modifies search steps.
    public sealed class SearchMotionPlan
    {
        readonly SearchMotionPhase[] phases;
        readonly double[] seconds;
        public double Duration { get; private set; }
        public SearchMotionPlan(SearchMotionPhase[] phases, double[] seconds)
        {
            if (phases == null || seconds == null || phases.Length == 0 || phases.Length != seconds.Length)
                throw new ArgumentException("A motion phase needs a duration.");
            this.phases = (SearchMotionPhase[])phases.Clone();
            this.seconds = (double[])seconds.Clone();
            foreach (double value in seconds)
            {
                if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
                    throw new ArgumentOutOfRangeException("seconds");
                Duration += value;
            }
            if (Duration <= 0) throw new ArgumentException("Motion duration must be positive.");
        }
        public double StartOf(SearchMotionPhase phase)
        {
            double start = 0;
            for (int i = 0; i < phases.Length; i++)
            {
                if (phases[i] == phase) return start;
                start += seconds[i];
            }
            return Duration;
        }
        public double LengthOf(SearchMotionPhase phase)
        {
            for (int i = 0; i < phases.Length; i++) if (phases[i] == phase) return seconds[i];
            return 0;
        }
        public float ProgressOf(SearchMotionPhase phase, double elapsed)
        {
            double length = LengthOf(phase), start = StartOf(phase);
            if (length == 0) return elapsed >= start ? 1 : 0;
            return (float)Math.Max(0, Math.Min(1, (elapsed - start) / length));
        }
        public SearchMotionPhase PhaseAt(double elapsed)
        {
            double end = 0;
            for (int i = 0; i < phases.Length; i++)
            {
                end += seconds[i];
                if (elapsed < end) return phases[i];
            }
            return phases[phases.Length - 1];
        }
    }
}
