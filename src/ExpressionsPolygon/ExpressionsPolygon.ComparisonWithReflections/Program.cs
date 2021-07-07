using BenchmarkDotNet.Running;

namespace ExpressionsPolygon.ComparisonWithReflections
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ExpressionsVsReflectionOnceAssignBenchmarks>();
        }
    }
}
