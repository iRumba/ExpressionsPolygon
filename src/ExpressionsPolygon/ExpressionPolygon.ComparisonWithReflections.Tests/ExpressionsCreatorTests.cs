using ExpressionsPolygon.ComparisonWithReflections;
using FluentAssertions;
using NUnit.Framework;

namespace ExpressionPolygon.ComparisonWithReflections.Tests
{
    public class ExpressionsCreatorTests
    {

        [Test]
        public void PropertySetterExpression_should_create_assign_expression()
        {
            var expr = ExpressionsCreator.PropertySetterExpression(typeof(TestClass), nameof(TestClass.Property));
            var assAct = expr.Compile();
            var e = new TestClass();
            const int v = 5;

            assAct(e, v);

            e.Property.Should().Be(5);
        }

        public class TestClass
        {
            public int Property { get; set; }
        }
    }
}