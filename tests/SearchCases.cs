using System;
using System.Collections.Generic;
using CpE2_DSA_Olivar_26271Sem;

// Test dispatch only. Production dispatch is the delegate assigned by each form.
static class SearchCases
{
    public static List<SearchStep> CreateSteps(int[] values, int target, string algorithm)
    {
        switch (algorithm)
        {
            case "Jump Search": return JumpSearchForm.CreateSteps(values, target);
            case "Exponential Search": return ExponentialSearchForm.CreateSteps(values, target);
            case "Fibonacci Search": return FibonacciSearchForm.CreateSteps(values, target);
            default: throw new ArgumentException("Unknown test algorithm.");
        }
    }
}
