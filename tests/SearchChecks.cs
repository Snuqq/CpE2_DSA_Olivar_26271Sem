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
                    var steps = SearchAlgorithms.CreateSteps(values, target, algorithm);
                    var result = steps[steps.Count - 1];
                    bool exists = Array.IndexOf(values, target) >= 0;
                    if (result.Found != exists || (result.Found && values[result.Index] != target))
                        throw new Exception(algorithm + " failed for length " + size + " target " + target);
                    foreach (var step in steps)
                        if (step.Index < -1 || step.Index >= size) throw new Exception("Invalid highlighted index");
                }
            }
            Console.WriteLine("PASS: " + algorithm + " (7,171 searches)");
        }
        return 0;
    }
}
