using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionsPolygon.ComparisonWithReflections
{
    public static class ExpressionsCreator
    {
        public static Expression<Action<object, object>> PropertySetterExpression(Type entityType, string propertyName)
        {
            var property = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            var parameterEntityExpression = Expression.Parameter(typeof(object), "e");
            var parameterValueExpression = Expression.Parameter(typeof(object), "v");

            var entityCast = Expression.Convert(parameterEntityExpression, entityType);
            var valueCast = Expression.Convert(parameterValueExpression, property?.PropertyType ??
                                                                         throw new InvalidOperationException("Something wrong"));

            var propertySelector = Expression.Property(entityCast, property);

            var assignExpression = Expression.Assign(propertySelector, valueCast);

            return Expression.Lambda<Action<object, object>>(assignExpression, parameterEntityExpression, parameterValueExpression);
        }
    }
}