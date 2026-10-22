using System;
using CpE2_DSA_Olivar_26271Sem;
class SearchChecks
{
    static int Main()
    {
        var random = new Random(42);
        foreach (string algorithm in new[] { "Jump Search", "Exponential Search", "Fibonacci Search" })
        {
            for (int size = 0; size <= 100; size++)
            {
                int[] values = new int[size];
                for (int i = 0; i < size; i++) values[i] = random.Next(-30, 31);
                Array.Sort(values);
                for (int target = -35; target <= 35; target++)
                {
                    var steps = SearchCases.CreateSteps(values, target, algorithm);
                    var result = steps[steps.Count - 1];
                    if (size > 0 && size <= 40)
                    {
                        var plan = new SearchPlan { Values = values, Target = target, Algorithm = algorithm, Steps = steps };
                        string xml = plan.Serialize();
                        if (SearchPlan.Deserialize(xml).Serialize() != xml) throw new Exception("Plan changed during transport");
                    }
                    bool exists = Array.IndexOf(values, target) >= 0;
                    if (result.Found != exists || (result.Found && values[result.Index] != target))
                        throw new Exception(algorithm + " failed for length " + size + " target " + target);
                    int count = 0;
                    foreach (var step in steps)
                    {
                        if (step.Index < -1 || step.Index >= size) throw new Exception("Invalid highlighted index");
                        if (step.Terminal) continue;
                        count++;
                        if (step.Comparisons != count || string.IsNullOrEmpty(step.Reason)) throw new Exception("Missing teaching metadata");
                        if (step.Index < step.Left || step.Index > step.Right) throw new Exception(algorithm + " probed outside its candidate range");
                        if (step.NextLeft < step.Left || step.NextRight > step.Right) throw new Exception("Range expanded");
                        bool retained = !exists;
                        for (int i = Math.Max(0, step.NextLeft); i <= Math.Min(size - 1, step.NextRight); i++)
                            if (values[i] == target) retained = true;
                        if (!retained) throw new Exception(algorithm + " eliminated the target");
                    }
                    if (!result.Terminal || result.Comparisons != count) throw new Exception("Invalid terminal beat");
                }
            }
            Console.WriteLine("PASS: " + algorithm + " (7,171 searches)");
            foreach (var special in new[] { new[] { int.MinValue, -1, 0, int.MaxValue }, new[] { 5 }, new[] { 7, 7, 7 } })
                foreach (int target in new[] { int.MinValue, -1, 0, 5, 7, 8, int.MaxValue })
                {
                    var trace = SearchCases.CreateSteps(special, target, algorithm);
                    if (trace[trace.Count - 1].Found != (Array.IndexOf(special, target) >= 0)) throw new Exception("Extreme-value search failed");
                }
        }
        try { SearchCases.CreateSteps(new[] { 2, 1 }, 1, "Jump Search"); throw new Exception("Unsorted input accepted"); }
        catch (ArgumentException) { }
        CheckTrace("Jump Search", new[] { 2, 5, 8, 6, 6 });
        CheckTrace("Exponential Search", new[] { 0, 1, 2, 4, 8, 6, 6 });
        CheckTrace("Fibonacci Search", new[] { 4, 7, 5, 6, 6 });
        Console.WriteLine("PASS: form-owned probe order and 8,520 playable plan roundtrips");
        return 0;
    }
    static void CheckTrace(string algorithm, int[] expected)
    {
        var steps = SearchCases.CreateSteps(new[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 }, 13, algorithm);
        if (steps.Count != expected.Length) throw new Exception("Probe count changed: " + algorithm);
        for (int i = 0; i < expected.Length; i++)
            if (steps[i].Index != expected[i]) throw new Exception("Probe order changed: " + algorithm);
    }
}
