using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionsPolygon.Core;
using FluentAssertions;
using NUnit.Framework;

namespace ExpressionPolygon.Core.Tests
{
    public class Tests
    {
        private static object[] _orJoinCases = 
        {
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue2),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 || x.PropTrue2)).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.PropFalse1),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 || x.PropFalse1)).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 || x.MethodTrue1())).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => false),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                // ReSharper disable once RedundantLogicalConditionalExpressionOperand
                ((Expression<Func<TestClass, bool>>) (x => false || x.MethodTrue1())).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => MethodFromTestClassFalse(x)),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                ((Expression<Func<TestClass, bool>>) (x => MethodFromTestClassFalse(x) || x.MethodTrue1())).ToString()
            }
        };

        private static object[] _orJoinCasesMultipleArguments =
        {
            new object[]
            {
                (Expression<Func<TestClass, TestClass, bool>>) ((k, l) => k.PropTrue1 == l.PropTrue2),
                (Expression<Func<TestClass, TestClass, bool>>) ((k, l) => k.PropFalse1 == l.PropFalse2),
                ((Expression<Func<TestClass, TestClass, bool>>) ((k, l) => k.PropTrue1 == l.PropTrue2 || k.PropFalse1 == l.PropFalse2)).ToString()
            },
        };

        private static object[] _andJoinCases =
        {
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue2),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 && x.PropTrue2)).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.PropFalse1),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 && x.PropFalse1)).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => x.PropTrue1),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                ((Expression<Func<TestClass, bool>>) (x => x.PropTrue1 && x.MethodTrue1())).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => true),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                // ReSharper disable once RedundantLogicalConditionalExpressionOperand
                ((Expression<Func<TestClass, bool>>) (x => true && x.MethodTrue1())).ToString()
            },
            new object[]
            {
                (Expression<Func<TestClass, bool>>) (x => MethodFromTestClassTrue(x)),
                (Expression<Func<TestClass, bool>>) (x => x.MethodTrue1()),
                ((Expression<Func<TestClass, bool>>) (x => MethodFromTestClassTrue(x) && x.MethodTrue1())).ToString()
            }
        };

        private static object[] _addTailCases =
        {
            new object[]
            {
                (Expression<Func<TestClass, TestClassNested>>) (x => x.Nested),
                (Expression<Func<TestClassNested, int>>) (x => x.IntProp),
                ((Expression<Func<TestClass, int>>) (x => x.Nested.IntProp)).ToString()
            }
        };

        [TestCaseSource(nameof(_orJoinCases))]
        [Test]
        public void Or_should_join_successful(Expression<Func<TestClass, bool>> predicate1, Expression<Func<TestClass, bool>> predicate2, string expected)
        {
            var resPredicate = predicate1.Or(predicate2);

            resPredicate.ToString().Should().Be(expected);
        }

        [TestCaseSource(nameof(_orJoinCasesMultipleArguments))]
        [Test]
        public void Or_should_join_successful_with_multiple_arguments(Expression<Func<TestClass, TestClass, bool>> predicate1, Expression<Func<TestClass, TestClass, bool>> predicate2, string expected)
        {
            var resPredicate = predicate1.Or(predicate2);

            resPredicate.ToString().Should().Be(expected);
        }

        [TestCaseSource(nameof(_andJoinCases))]
        [Test]
        public void And_should_join_successful(Expression<Func<TestClass, bool>> predicate1, Expression<Func<TestClass, bool>> predicate2, string expected)
        {
            var resPredicate = predicate1.And(predicate2);

            resPredicate.ToString().Should().Be(expected);
        }

        [TestCaseSource(nameof(_addTailCases))]
        [Test]
        public void AddTail_should_join_successful(Expression<Func<TestClass, TestClassNested>> lambda1,
            Expression<Func<TestClassNested, int>> lambda2, string expected)
        {
            var resPredicate = lambda1.AddTail(lambda2);

            resPredicate.ToString().Should().Be(expected);
        }

        [Test]
        public void Any_method_should_be_join()
        {
            Expression<Func<TestClassNested, bool>> predicate = x => x.IntProp == 5;

            Expression<Func<TestClass, IEnumerable<TestClassNested>>> collectionSelector = k => k.NestedCollection;

            Expression<Func<TestClass, bool>> expected = k => k.NestedCollection.Any(x => x.IntProp == 5);

            var resultPredicate = collectionSelector.Any(predicate);

            resultPredicate.ToString().Should().Be(expected.ToString());
        }

        [Test]
        public void All_method_should_be_join()
        {
            Expression<Func<TestClassNested, bool>> predicate = x => x.IntProp == 5;

            Expression<Func<TestClass, IEnumerable<TestClassNested>>> collectionSelector = k => k.NestedCollection;

            Expression<Func<TestClass, bool>> expected = k => k.NestedCollection.All(x => x.IntProp == 5);

            var resultPredicate = collectionSelector.All(predicate);

            resultPredicate.ToString().Should().Be(expected.ToString());
        }

        [Test]
        public void Contains_method_should_be_join()
        {
            Expression<Func<TestClass, IEnumerable<int>>> collectionSelector = k => k.IntCollection;

            Expression<Func<TestClass, bool>> expected = k => k.IntCollection.Contains(5);

            var resultPredicate = collectionSelector.Contains(5);

            resultPredicate.ToString().Should().Be(expected.ToString());
        }

        public static bool MethodFromTestClassTrue(TestClass _) => true;

        public static bool MethodFromTestClassFalse(TestClass _) => false;

        public class TestClass
        {
            public bool PropTrue1 => true;

            public bool PropTrue2 => true;

            public bool PropFalse1 => false;

            public bool PropFalse2 => false;

            public TestClassNested Nested { get; } = new();

            public List<int> IntCollection { get; } = new List<int>
            {
                5
            };

            public List<TestClassNested> NestedCollection { get; } = new List<TestClassNested>
            {
                new()
            };

            public bool MethodTrue1() => true;
        }

        public class TestClassNested
        {
            public int IntProp => 5;
        }
    }
}