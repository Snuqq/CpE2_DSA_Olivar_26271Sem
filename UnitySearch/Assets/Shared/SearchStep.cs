using System;
using System.Collections.Generic;

namespace CpE2_DSA_Olivar_26271Sem
{
    // A probe is one comparison; the final result is a separate, non-comparison beat.
    [Serializable]
    public sealed class SearchStep
    {
        public int Index, Left, Right, NextLeft, NextRight, Comparisons;
        public bool Found, Terminal;
        public string Phase, Reason, Comparison, Message;
    }

    // Records teaching labels and comparisons; each form owns its search decisions.
    public static class SearchSteps
    {
        public static void ValidateValues(int[] values)
        {
            if (values == null) throw new ArgumentNullException("values");
            for (int i = 1; i < values.Length; i++)
                if (values[i] < values[i - 1]) throw new ArgumentException("Values must be sorted.");
        }

        public static void Complete(int[] values, int target, int result, List<SearchStep> steps)
        {
            steps.Add(new SearchStep {
                Index = result, Found = result >= 0, Terminal = true,
                Left = result >= 0 ? result : 0, Right = result,
                NextLeft = result >= 0 ? result : 0, NextRight = result,
                Comparisons = steps.Count, Phase = result >= 0 ? "Found" : "Not found",
                Reason = result >= 0 ? "The value equals the target. Search complete." : "No candidate positions remain.",
                Comparison = result >= 0 ? values[result] + " = " + target : "Target " + target + " is absent.",
                Message = result >= 0 ? "Found at index " + result + "." : "Target not found."
            });
        }

        public static int Probe(int[] a, int target, int i, int left, int right, List<SearchStep> steps,
            string phase, string reason)
        {
            int cmp = a[i].CompareTo(target);
            int lo = cmp < 0 ? Math.Max(left, i + 1) : left;
            int hi = cmp > 0 ? Math.Min(right, i - 1) : right;
            if (cmp == 0) lo = hi = i;
            string comparison = a[i] + (cmp < 0 ? " < " : cmp > 0 ? " > " : " = ") + target;
            string outcome = cmp == 0 ? "Match." : cmp < 0
                ? "Sorted order rules out this index and everything before it."
                : "Sorted order rules out this index and everything after it.";
            steps.Add(new SearchStep { Index = i, Left = left, Right = right, NextLeft = lo,
                NextRight = hi, Comparisons = steps.Count + 1, Phase = phase, Reason = reason,
                Comparison = comparison, Message = "Index " + i + ": " + comparison + ". " + outcome });
            return cmp;
        }

    }
}
