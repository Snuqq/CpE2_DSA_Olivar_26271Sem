using System;
using System.IO;
using CpE2_DSA_Olivar_26271Sem;

class ExportSearchSamples
{
    static void Main()
    {
        const string folder = "UnitySearch/Assets/Resources/SearchSamples";
        Directory.CreateDirectory(folder);
        foreach (string name in new[] { "Jump", "Exponential", "Fibonacci" })
        foreach (int target in new[] { 13, 12 })
        {
            int[] values = { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
            string algorithm = name + " Search";
            var plan = new SearchPlan { Algorithm = algorithm, Target = target, Values = values,
                Steps = SearchCases.CreateSteps(values, target, algorithm) };
            string path = Path.Combine(folder, name + "-" + target + ".xml");
            string xml = plan.Serialize();
            if (!File.Exists(path) || File.ReadAllText(path) != xml) File.WriteAllText(path, xml);
        }
        Console.WriteLine("Exported six Unity preview plans from the form algorithms.");
    }
}
