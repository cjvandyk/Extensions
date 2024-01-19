/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

namespace System.Linq.Expressions
{
    /// <summary>
    /// A query builder class.
    /// </summary>
    [Serializable]
    public static partial class QueryBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return C => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return C => false;
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> currentExpression,
            Expression<Func<T, bool>> appendageExpression)
        {
            var newExpression = Expression.Invoke(
                appendageExpression,
                currentExpression.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(currentExpression.Body, newExpression),
                currentExpression.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> currentExpression,
            Expression<Func<T, bool>> appendageExpression)
        {
            var newExpression = Expression.Invoke(
                appendageExpression,
                currentExpression.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(currentExpression.Body, newExpression),
                currentExpression.Parameters);
        }
    }
}
