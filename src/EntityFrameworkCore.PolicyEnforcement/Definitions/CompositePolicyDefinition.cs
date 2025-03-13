using System;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Helpers;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class CompositePolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly IPolicyDefinition<TEntity> _left;
	private readonly IPolicyDefinition<TEntity> _right;
	private readonly PolicyCompositionType _compositionType;

	public CompositePolicyDefinition(
			string policyName,
			IPolicyDefinition<TEntity> left,
			IPolicyDefinition<TEntity> right,
			PolicyCompositionType compositionType)
	{
		_policyName = policyName;
		_left = left;
		_right = right;
		_compositionType = compositionType;
	}

	public IPolicyDefinition<TEntity> Or(IPolicyDefinition<TEntity> other)
	{
		return new CompositePolicyDefinition<TEntity>(_policyName, this, other, PolicyCompositionType.Or);
	}

	public IPolicyDefinition<TEntity> And(IPolicyDefinition<TEntity> other)
	{
		return new CompositePolicyDefinition<TEntity>(_policyName, this, other, PolicyCompositionType.And);
	}

	public Expression<Func<TEntity, bool>> ToExpression(IUserContext userContext)
	{
		var leftExpr = _left.ToExpression(userContext);
		var rightExpr = _right.ToExpression(userContext);

		return _compositionType == PolicyCompositionType.And
				? ExpressionHelper.CombineAnd(leftExpr, rightExpr)
				: ExpressionHelper.CombineOr(leftExpr, rightExpr);
	}

	public bool CanEvaluateWithoutDb()
	{
		return _left.CanEvaluateWithoutDb() && _right.CanEvaluateWithoutDb();
	}
}