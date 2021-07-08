using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionsPolygon.Core
{
    /// <summary>
    /// Методы расширения для деревьев выражений. Прямого отношения к EF не имеет. Возможно, есть смысл вынести в отдельный проект.
    /// </summary>
    public static class ExpressionExtensions
    {
        private static MethodInfo _anyMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(x => x.Name == nameof(Enumerable.Any) && x.GetParameters().Length == 2);

        private static MethodInfo _allMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(x => x.Name == nameof(Enumerable.All) && x.GetParameters().Length == 2);

        private static MethodInfo _containsMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2);

        /// <summary>
        ///     Логическая операция ИЛИ.
        /// </summary>
        /// <typeparam name="TEntity"> Тип параметра выражений. </typeparam>
        /// <param name="leftPredicate"> Левый предикат. </param>
        /// <param name="rightPredicate"> Правый предикат. </param>
        /// <returns> Итоговый предикат. </returns>
        public static Expression<Func<TEntity, bool>> Or<TEntity>(
            this Expression<Func<TEntity, bool>> leftPredicate,
            Expression<Func<TEntity, bool>> rightPredicate)
        {
            return (Expression<Func<TEntity, bool>>) leftPredicate.Or((LambdaExpression) rightPredicate);
        }

        /// <summary>
        ///     Логическая операция ИЛИ.
        /// </summary>
        /// <typeparam name="TEntity1"> Тип параметра выражений. </typeparam>
        /// <typeparam name="TEntity2"> Тип параметра выражений. </typeparam>
        /// <param name="leftPredicate"> Левый предикат. </param>
        /// <param name="rightPredicate"> Правый предикат. </param>
        /// <returns> Итоговый предикат. </returns>
        public static Expression<Func<TEntity1, TEntity2, bool>> Or<TEntity1, TEntity2>(
            this Expression<Func<TEntity1, TEntity2, bool>> leftPredicate,
            Expression<Func<TEntity1, TEntity2, bool>> rightPredicate)
        {
            return (Expression<Func<TEntity1, TEntity2, bool>>)leftPredicate.Or((LambdaExpression)rightPredicate);
        }

        /// <summary>
        ///     Логическая операция ИЛИ.
        /// </summary>
        /// <typeparam name="TEntity"> Тип параметра выражений. </typeparam>
        /// <param name="leftPredicate"> Левый предикат. </param>
        /// <param name="rightPredicate"> Правый предикат. </param>
        /// <returns> Итоговый предикат. </returns>
        public static Expression<Func<TEntity, bool>> And<TEntity>(
            this Expression<Func<TEntity, bool>> leftPredicate,
            Expression<Func<TEntity, bool>> rightPredicate)
        {
            return (Expression<Func<TEntity, bool>>) leftPredicate.And((LambdaExpression) rightPredicate);
        }

        /// <summary>
        ///     Применение метода Any к выражению коллекции.
        /// </summary>
        /// <typeparam name="TEntity"> Тип сущности источника. </typeparam>
        /// <typeparam name="TProperty"> Тип элемента коллекции. </typeparam>
        /// <param name="collectionSelector"> Селектор коллекции от сущности. </param>
        /// <param name="propertyPredicate"> Предикат к элементу коллекции. </param>
        /// <returns> Итоговое выражение. </returns>
        public static Expression<Func<TEntity, bool>> Any<TEntity, TProperty>(
            this Expression<Func<TEntity, IEnumerable<TProperty>>> collectionSelector,
            Expression<Func<TProperty, bool>> propertyPredicate)
        {
            return collectionSelector.MakeCollectionPredicateMethodCall(
                _anyMethod.MakeGenericMethod(typeof(TProperty)), propertyPredicate);
        }

        /// <summary>
        ///     Применение метода All к выражению коллекции.
        /// </summary>
        /// <typeparam name="TEntity"> Тип сущности источника. </typeparam>
        /// <typeparam name="TProperty"> Тип элемента коллекции. </typeparam>
        /// <param name="collectionSelector"> Селектор коллекции от сущности. </param>
        /// <param name="propertyPredicate"> Предикат к элементу коллекции. </param>
        /// <returns> Итоговое выражение. </returns>
        public static Expression<Func<TEntity, bool>> All<TEntity, TProperty>(
            this Expression<Func<TEntity, IEnumerable<TProperty>>> collectionSelector,
            Expression<Func<TProperty, bool>> propertyPredicate)
        {
            return collectionSelector.MakeCollectionPredicateMethodCall(
                _allMethod.MakeGenericMethod(typeof(TProperty)), propertyPredicate);
        }

        /// <summary>
        ///     Применение метода Contains к выражению коллекции.
        /// </summary>
        /// <typeparam name="TEntity"> Тип сущности источника. </typeparam>
        /// <typeparam name="TProperty"> Тип элемента коллекции. </typeparam>
        /// <param name="collectionSelector"> Селектор коллекции от сущности. </param>
        /// <param name="value"> Значение, проверяемое на наличие. </param>
        /// <returns> Итоговое выражение. </returns>
        public static Expression<Func<TEntity, bool>> Contains<TEntity, TProperty>(
            this Expression<Func<TEntity, IEnumerable<TProperty>>> collectionSelector,
            TProperty value)
        {
            var constantExpression = Expression.Constant(value);
            return collectionSelector.MakeCollectionPredicateMethodCall(
                _containsMethod.MakeGenericMethod(typeof(TProperty)), constantExpression);
        }

        /// <summary>
        ///     Применить к коллекции метод для предиката коллекции.
        /// </summary>
        /// <typeparam name="TEntity"> Тип сущности источника. </typeparam>
        /// <typeparam name="TProperty"> Тип элемента коллекции. </typeparam>
        /// <param name="collectionSelector"> Селектор коллекции от сущности. </param>
        /// <param name="arguments"> Аргументы кроме аргемента коллекции. </param>
        /// <param name="collectionPredicateMethod"></param>
        /// <returns></returns>
        private static Expression<Func<TEntity, bool>> MakeCollectionPredicateMethodCall<TEntity, TProperty>(
            this Expression<Func<TEntity, IEnumerable<TProperty>>> collectionSelector,
            MethodInfo collectionPredicateMethod, params Expression[] arguments)
        {
            var call = Expression.Call(null, collectionPredicateMethod, new []{collectionSelector.Body}.Concat(arguments));

            return Expression.Lambda<Func<TEntity, bool>>(call, collectionSelector.Parameters);
        }

        /// <summary>
        ///     Логическая операция И.
        /// </summary>
        /// <param name="leftPredicate"> Левый предикат. </param>
        /// <param name="rightPredicate"> Правый предикат. </param>
        /// <returns> Итоговый предикат. </returns>
        private static LambdaExpression And(this LambdaExpression leftPredicate, LambdaExpression rightPredicate)
        {
            return CreateLogicalExpression(LogicalOperation.And, leftPredicate, rightPredicate);
        }

        /// <summary>
        ///     Логическая операция ИЛИ.
        /// </summary>
        /// <param name="leftPredicate"> Левый предикат. </param>
        /// <param name="rightPredicate"> Правый предикат. </param>
        /// <returns> Итоговый предикат. </returns>
        private static LambdaExpression Or(this LambdaExpression leftPredicate, LambdaExpression rightPredicate)
        {
            return CreateLogicalExpression(LogicalOperation.Or, leftPredicate, rightPredicate);
        }

        /// <summary>
        ///     Создание предиката на основе логической операции.
        /// </summary>
        /// <param name="logicalOperation">Тип логической операции. </param>
        /// <param name="leftPredicate">Левый предикат. </param>
        /// <param name="rightPredicate">правый предикат. </param>
        /// <returns>итоговый предикат. </returns>
        private static LambdaExpression CreateLogicalExpression(
            LogicalOperation logicalOperation,
            LambdaExpression leftPredicate,
            LambdaExpression rightPredicate
        )
        {
            if (leftPredicate is null)
                return rightPredicate;

            if (rightPredicate is null)
                return leftPredicate;

            if (!leftPredicate.Parameters.Select(x => x.Type)
                .SequenceEqual(rightPredicate.Parameters.Select(x => x.Type)))
                throw new InvalidOperationException(
                    "Параметры левого предиката должны совпадать с параметрами правого");

            ExpressionType expressionType;

            switch (logicalOperation)
            {
                case LogicalOperation.And:
                    expressionType = ExpressionType.AndAlso;
                    break;
                case LogicalOperation.Or:
                    expressionType = ExpressionType.OrElse;
                    break;
                default:
                    throw new NotSupportedException();
            }

            var newParameters = leftPredicate.Parameters.Select(x => Expression.Parameter(x.Type, x.Name)).ToArray();

            var res = leftPredicate.Parameters.Zip(rightPredicate.Parameters.Zip(newParameters, (r, n) => (r, n)),
                (l, rn) => (new ExpressionReplaceVisitor(l, rn.n), new ExpressionReplaceVisitor(rn.r, rn.n)));


            var leftPredicateBody = leftPredicate.Body;
            var rightPredicateBody = rightPredicate.Body;

            foreach (var (leftPredicateReplacer, rightPredicateReplacer) in res)
            {
                leftPredicateBody = leftPredicateReplacer.Visit(leftPredicateBody) ??
                                    throw new InvalidOperationException();
                rightPredicateBody = rightPredicateReplacer.Visit(rightPredicateBody) ??
                                     throw new InvalidOperationException();
            }

            var binary = Expression.MakeBinary(expressionType, leftPredicateBody, rightPredicateBody);

            return Expression.Lambda(binary, newParameters);
        }

        /// <summary>
        ///     Добавить хвост к выражению (склеивание двух коротких выражения в одно длинное).
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности с которой начинается выражение. </typeparam>
        /// <typeparam name="TProperty">Тип свойства сущности. </typeparam>
        /// <typeparam name="TResult">Тип выражения, выполняемого над свойством сущности. </typeparam>
        /// <param name="propertySelector">Доступ к свойству. </param>
        /// <param name="resultSelector">Доступ к результату. </param>
        /// <returns>Итоговое выражение. </returns>
        public static Expression<Func<TEntity, TResult>> AddTail<TEntity, TProperty, TResult>(
            this Expression<Func<TEntity, TProperty>> propertySelector,
            Expression<Func<TProperty, TResult>> resultSelector)
        {
            return (Expression<Func<TEntity, TResult>>)propertySelector.AddTail((LambdaExpression)resultSelector);
        }

        /// <summary>
        ///     Продолжить лямбда выражение.
        /// </summary>
        /// <param name="leftLambda"> Исходное выражение. </param>
        /// <param name="rightLambda"> Добавляемое выражение. </param>
        /// <returns> Итоговое выражение. </returns>
        public static LambdaExpression AddTail(this LambdaExpression leftLambda, LambdaExpression rightLambda)
        {
            return MergeLambdas(leftLambda, rightLambda);
        }

        /// <summary>
        ///     Склеивание лямбда выражений.
        /// </summary>
        /// <param name="leftLambda"> Левое выражение. </param>
        /// <param name="rightLambda"> Правое выражение. </param>
        /// <returns> Итоговое выражение. </returns>
        private static LambdaExpression MergeLambdas(LambdaExpression leftLambda, LambdaExpression rightLambda)
        {
            if (leftLambda is null)
                throw new ArgumentNullException(nameof(leftLambda));

            if (rightLambda is null)
                throw new ArgumentNullException(nameof(rightLambda));

            if (rightLambda.Parameters.Count != 1 ||
                !rightLambda.Parameters[0].Type.IsAssignableFrom(leftLambda.Body.Type))
                throw new InvalidOperationException(
                    "Правое выражение должно иметь один параметр, совпадающий по типу с возвращаемым значением левого выражения");

            var bodyReplacer = new ExpressionReplaceVisitor(rightLambda.Parameters[0], leftLambda.Body);

            var newBody = bodyReplacer.Visit(rightLambda.Body);

            if (newBody is null)
                throw new InvalidOperationException("Some thing wrong!");
            
            var lambda = Expression.Lambda(newBody, leftLambda.Parameters);

            return lambda;
        }
    }
}