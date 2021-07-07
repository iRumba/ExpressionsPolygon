using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace ExpressionsPolygon.ComparisonWithReflections
{
    public class ExpressionsVsReflectionOnceAssignBenchmarks
    {
        private static PropertyInfo _propertyInfo = typeof(BenchClass).GetProperty(nameof(BenchClass.Property),
            BindingFlags.Instance | BindingFlags.Public);

        private static Action<object, object> _assignAction =
            ExpressionsCreator.PropertySetterExpression(typeof(BenchClass), nameof(BenchClass.Property)).Compile();

        private BenchClass _directBc = new();
        private BenchClass _exprBc = new();
        private BenchClass _exprDynamicBc = new();
        private BenchClass _reflBc = new();

        [Benchmark(Description = "Direct once")]
        public void DirectOnceAssign()
        {
            _directBc.Property = 5;
        }

        [Benchmark(Description = "Expressions once")]
        public void ExpressionOnceAssign()
        {
            _assignAction(_exprBc, 5);
        }

        [Benchmark(Description = "Expressions dynamic once")]
        public void ExpressionDynamicOnceAssign()
        {
            _assignAction.DynamicInvoke(_exprDynamicBc, 5);
        }

        [Benchmark(Description = "Reflection once")]
        public void ReflectionOnceAssign()
        {
            _propertyInfo.SetValue(_reflBc, 5);
        }

        [Benchmark(Description = "Direct loop")]
        public void DirectLoopAssign()
        {
            for (var i = 1; i < 1000; i++)
                _directBc.Property = i;
        }

        [Benchmark(Description = "Expressions loop")]
        public void ExpressionLoopAssign()
        {
            for (var i = 1; i < 1000; i++)
                _assignAction(_exprBc, i);
        }

        [Benchmark(Description = "Expressions dynamic loop")]
        public void ExpressionDynamicLoopAssign()
        {
            for (var i = 1; i < 1000; i++)
                _assignAction.DynamicInvoke(_exprDynamicBc, i);
        }

        [Benchmark(Description = "Reflection loop")]
        public void ReflectionLoopAssign()
        {
            for (var i = 1; i < 1000; i++)
                _propertyInfo.SetValue(_reflBc, i);
        }

        public class BenchClass
        {
            public int Property { get; set; }
        }
    }
}