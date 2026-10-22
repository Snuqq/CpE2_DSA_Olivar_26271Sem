using System;
using System.IO;
using CpE2_DSA_Olivar_26271Sem;
class PlaybackChecks
{
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    static int Main()
    {
        var clock = new SearchPlayback { Duration = 2 };
        var steps = SearchCases.CreateSteps(new[] { 1, 3, 5, 7, 9 }, 7, "Fibonacci Search");
        clock.Load(steps); clock.Play(); clock.Tick(.5);
        Check(clock.Current == 0 && clock.Progress == .25, "Initial playback");
        clock.Pause(); clock.Tick(50); Check(clock.Progress == .25, "Pause moved");
        clock.Duration = .4; clock.Play(); clock.Tick(.5); Check(clock.Progress == .5, "Speed rewound in-flight step");
        clock.Step(); clock.Step(); clock.Tick(1); clock.Tick(1);
        Check(clock.Current == 0 && !clock.Moving && !clock.Playing, "Rapid step skipped a comparison");
        clock.Step(); Check(clock.Current == 1 && clock.Progress == 0, "Single step did not advance");
        clock.Tick(.4); Check(!clock.Moving, "New speed was not applied");
        clock.Play(); for (int i = 0; i < 100; i++) clock.Tick(.5);
        Check(clock.Finished && !clock.Playing, "Playback never completed");
        clock.Load(steps); Check(clock.Current == -1 && !clock.Moving, "Reset did not clear playback");
        try { clock.Duration = double.NaN; throw new Exception("Non-finite speed accepted"); }
        catch (ArgumentOutOfRangeException) { }
        var plan = new SearchMotionPlan(new[] { SearchMotionPhase.Look, SearchMotionPhase.React, SearchMotionPhase.Pullback }, new[] { .2, .6, 1.05 });
        Check(Math.Abs(plan.StartOf(SearchMotionPhase.Pullback) - .8) < .00001, "Reaction/pullback overlap");
        Check(plan.PhaseAt(.2) == SearchMotionPhase.React && plan.ProgressOf(SearchMotionPhase.Pullback, .79) == 0, "Phase boundary");
        Check(plan.ProgressOf(SearchMotionPhase.Pullback, 100) == 1, "Phase progress clamp");
        int planned = 0;
        clock.DurationForStep = (index, requested) => { planned++; return plan.Duration + requested; };
        clock.Duration = 2; clock.Load(steps); clock.Play(); clock.Tick(.5);
        double elapsed = clock.Elapsed, progress = clock.Progress;
        clock.Duration = .4; clock.Pause(); clock.Tick(10);
        Check(planned == 1 && clock.Elapsed == elapsed && clock.Progress == progress, "Paused motion replanned");
        clock.Play(); clock.Tick(.5);
        Check(planned == 1 && Math.Abs(clock.Elapsed - 1) < .00001 && Math.Abs(clock.BeatDuration - 3.85) < .00001, "In-flight schedule changed");
        using (var stream = new MemoryStream())
        {
            var sent = new SearchPlan { Values = new[] { 1, 3, 5, 7, 9 }, Target = 7,
                Algorithm = "Fibonacci Search", Steps = steps };
            steps[0].Reason = "Reason | with <tags>, & punctuation\nand Unicode: é.";
            string xml = sent.Serialize();
            SearchWire.Write(stream, "LOAD|1|" + xml); stream.Position = 0;
            var fields = SearchWire.Read(stream).Split(new[] { '|' }, 3);
            Check(fields[0] == "LOAD" && fields[1] == "1", "Load command roundtrip");
            var received = SearchPlan.Deserialize(fields[2]);
            Check(received.Serialize() == xml && received.Steps[0].Reason == steps[0].Reason,
                "Wire transport changed the supplied plan");
            Check(SearchWire.Version == 2, "Old player protocol was not rejected");
            received.Steps[0].Index = 99;
            try { received.Validate(); throw new Exception("Invalid probe accepted"); }
            catch (ArgumentException) { }
        }
        try { SearchWire.Read(new MemoryStream(BitConverter.GetBytes(100000))); throw new Exception("Unbounded message accepted"); }
        catch (InvalidDataException) { }
        try { SearchPlan.Deserialize(new string('x', 64001)); throw new Exception("Unbounded XML accepted"); }
        catch (InvalidDataException) { }
        bool rejectedDtd = false;
        try { SearchPlan.Deserialize("<!DOCTYPE SearchPlan [<!ENTITY value 'expanded'>]><SearchPlan>&value;</SearchPlan>"); }
        catch (InvalidOperationException) { rejectedDtd = true; }
        Check(rejectedDtd, "DTD accepted in search plan");
        Console.WriteLine("PASS: playback, pause/resume, rapid step, speed, reset, terminal state, plan transport, bounded framing");
        return 0;
    }
}
