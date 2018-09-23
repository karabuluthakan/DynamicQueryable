using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        public static IQueryable Take(this IQueryable source, int count) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Take",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count))
                );
        }

        public static IQueryable Skip(this IQueryable source, int count) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Skip",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, params object[] values) {
            return GroupBy(source, keySelector, elementSelector, resultSelector, null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);
            var elementLambda = Evaluator.ToLambda(elementSelector, new[] { source.ElementType }, variables, values);
            var enumElementType = typeof(IEnumerable<>).MakeGenericType(elementLambda.Body.Type);
            var resultLambda = Evaluator.ToLambda(resultSelector, new[] { keyLambda.Body.Type, enumElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type, resultLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda),
                    Expression.Quote(elementLambda),
                    Expression.Quote(resultLambda)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, params object[] values) {
            return GroupBy(source, keySelector, resultSelector, (IDictionary<string, object>)null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);
            var enumSourceType = typeof(IEnumerable<>).MakeGenericType(source.ElementType);
            var resultLambda = Evaluator.ToLambda(resultSelector, new[] { keyLambda.Body.Type, enumSourceType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, resultLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda),
                    Expression.Quote(resultLambda)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, params object[] values) {
            return GroupBy(source, keySelector, (IDictionary<string, object>)null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda)
                )
            );
        }
    }
}
