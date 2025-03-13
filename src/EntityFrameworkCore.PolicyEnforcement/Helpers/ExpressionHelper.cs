using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Helpers;

internal static class ExpressionHelper
{
	public static Expression<Func<T, bool>> CombineOr<T>(
		Expression<Func<T, bool>> expr1,
		Expression<Func<T, bool>> expr2)
	{
		var parameter = Expression.Parameter(typeof(T));

		var leftVisitor = new ParameterRebinder(expr1.Parameters[0], parameter);
		var left = leftVisitor.Visit(expr1.Body);

		var rightVisitor = new ParameterRebinder(expr2.Parameters[0], parameter);
		var right = rightVisitor.Visit(expr2.Body);

		return Expression.Lambda<Func<T, bool>>(
			Expression.OrElse(left, right), parameter);
	}

	public static Expression<Func<T, bool>> CombineAnd<T>(
		Expression<Func<T, bool>> expr1,
		Expression<Func<T, bool>> expr2)
	{
		var parameter = Expression.Parameter(typeof(T));

		var leftVisitor = new ParameterRebinder(expr1.Parameters[0], parameter);
		var left = leftVisitor.Visit(expr1.Body);

		var rightVisitor = new ParameterRebinder(expr2.Parameters[0], parameter);
		var right = rightVisitor.Visit(expr2.Body);

		return Expression.Lambda<Func<T, bool>>(
			Expression.AndAlso(left, right), parameter);
	}

	private class ParameterRebinder : ExpressionVisitor
	{
		private readonly ParameterExpression _oldParameter;
		private readonly ParameterExpression _newParameter;

		public ParameterRebinder(ParameterExpression oldParameter, ParameterExpression newParameter)
		{
			_oldParameter = oldParameter;
			_newParameter = newParameter;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == _oldParameter)
				return _newParameter;
			return base.VisitParameter(node);
		}
	}
}