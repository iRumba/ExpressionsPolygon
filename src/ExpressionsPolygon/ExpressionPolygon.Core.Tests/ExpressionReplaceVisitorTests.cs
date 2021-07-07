using System;
using System.Linq.Expressions;
using ExpressionsPolygon.Core;
using FluentAssertions;
using NUnit.Framework;

namespace ExpressionPolygon.Core.Tests
{
    public class ExpressionReplaceVisitorTests
    {
        [Test]
        public void Visit_should_replace_parameter()
        {
            Expression<Func<TestClass, int>> expressionForReplace = x => x.Property;

            var replacedParameter = expressionForReplace.Parameters[0];

            var replaceTo = Expression.Parameter(typeof(TestClass), "x");

            var replacer = new ExpressionReplaceVisitor(replacedParameter, replaceTo);

            replaceTo.Should().NotBe(expressionForReplace.Parameters[0]);
            replaceTo.Should().NotBe((expressionForReplace.Body as MemberExpression)?.Expression ?? throw new InvalidOperationException("Something wrong!"));

            var replacedExpression = (Expression<Func<TestClass, int>>)replacer.Visit(expressionForReplace) ?? throw new InvalidOperationException("Something wrong!");

            replaceTo.Should().Be(replacedExpression.Parameters[0]);
            replaceTo.Should().Be((replacedExpression.Body as MemberExpression)?.Expression ?? throw new InvalidOperationException("Something wrong!"));
        }

        public class TestClass
        {
            public int Property { get; set; }
        }
    }
}