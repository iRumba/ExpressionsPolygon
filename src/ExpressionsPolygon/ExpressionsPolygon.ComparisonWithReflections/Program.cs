using BenchmarkDotNet.Running;

namespace ExpressionsPolygon.ComparisonWithReflections
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ExpressionsVsReflectionOnceAssignBenchmarks>();
        }
    }
}
