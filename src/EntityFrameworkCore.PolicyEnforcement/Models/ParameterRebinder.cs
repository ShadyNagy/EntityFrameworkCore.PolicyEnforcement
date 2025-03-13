using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Models;

internal class ParameterRebinder : ExpressionVisitor
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