using System.Linq.Expressions;

namespace ExpressionsPolygon.Core
{
    /// <summary>
    ///     Визитор для замены одного выражения на другое.
    /// </summary>
    public class ExpressionReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _replaceFrom;
        private readonly Expression _replaceTo;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="replaceFrom"> Выражение которое надо заменить. </param>
        /// <param name="replaceTo"> Выражение, на которое меняем. </param>
        public ExpressionReplaceVisitor(Expression replaceFrom, Expression replaceTo)
        {
            _replaceFrom = replaceFrom;
            _replaceTo = replaceTo;
        }

        /// <inheritdoc />
        public override Expression Visit(Expression node)
        {
            if (node == _replaceFrom)
                return _replaceTo;

            return base.Visit(node);
        }
    }
}
