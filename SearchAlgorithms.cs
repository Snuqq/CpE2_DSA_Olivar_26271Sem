using System;
using System.Collections.Generic;

namespace CpE2_DSA_Olivar_26271Sem
{
    internal class SearchStep
    {
        public int Index;
        public string Message;
        public bool Found;
        public SearchStep(int index, string message, bool found = false)
        {
            Index = index;
            Message = message;
            Found = found;
        }
    }

    internal static class SearchAlgorithms
    {
        public static List<SearchStep> CreateSteps(int[] values, int target, string algorithm)
        {
            var steps = new List<SearchStep>();
            if (values.Length == 0)
            {
                steps.Add(new SearchStep(-1, "The array is empty."));
                return steps;
            }
            int result;
            switch (algorithm)
            {
                case "Jump Search": result = Jump(values, target, steps); break;
                case "Exponential Search": result = Exponential(values, target, steps); break;
                case "Fibonacci Search": result = Fibonacci(values, target, steps); break;
                default: throw new ArgumentException("Select a search algorithm.");
            }
            steps.Add(new SearchStep(result, result < 0 ? "Target not found." : "Found at index " + result + ".", result >= 0));
            return steps;
        }

        private static void Check(int[] values, int index, List<SearchStep> steps, string operation)
        {
            steps.Add(new SearchStep(index, operation + ": index " + index + " = " + values[index]));
        }

        private static int Jump(int[] values, int target, List<SearchStep> steps)
        {
            int block = Math.Max(1, (int)Math.Sqrt(values.Length));
            int start = 0;
            int end = Math.Min(block, values.Length);
            while (true)
            {
                Check(values, end - 1, steps, "Check block end");
                if (values[end - 1] >= target) break;
                start = end;
                if (start >= values.Length) return -1;
                end = Math.Min(end + block, values.Length);
            }
            for (int i = start; i < end; i++)
            {
                Check(values, i, steps, "Search inside block");
                if (values[i] == target) return i;
                if (values[i] > target) break;
            }
            return -1;
        }

        private static int Exponential(int[] values, int target, List<SearchStep> steps)
        {
            Check(values, 0, steps, "Check first value");
            if (values[0] == target) return 0;
            int bound = 1;
            while (bound < values.Length)
            {
                Check(values, bound, steps, "Double search boundary");
                if (values[bound] >= target) break;
                bound *= 2;
            }
            int left = bound / 2;
            int right = Math.Min(bound, values.Length - 1);
            while (left <= right)
            {
                int middle = left + (right - left) / 2;
                Check(values, middle, steps, "Binary search");
                if (values[middle] == target) return middle;
                if (values[middle] < target) left = middle + 1;
                else right = middle - 1;
            }
            return -1;
        }

        private static int Fibonacci(int[] values, int target, List<SearchStep> steps)
        {
            long smaller = 0, previous = 1, fibonacci = 1;
            while (fibonacci < values.Length)
            {
                smaller = previous;
                previous = fibonacci;
                fibonacci = smaller + previous;
            }
            int offset = -1;
            while (fibonacci > 1)
            {
                int index = (int)Math.Min(offset + smaller, values.Length - 1L);
                Check(values, index, steps, "Fibonacci probe");
                if (values[index] < target)
                {
                    fibonacci = previous;
                    previous = smaller;
                    smaller = fibonacci - previous;
                    offset = index;
                }
                else if (values[index] > target)
                {
                    fibonacci = smaller;
                    previous -= smaller;
                    smaller = fibonacci - previous;
                }
                else return index;
            }
            if (previous != 0 && offset + 1 < values.Length)
            {
                Check(values, offset + 1, steps, "Check remaining value");
                if (values[offset + 1] == target) return offset + 1;
            }
            return -1;
        }
    }
}
