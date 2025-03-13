using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class OwnershipPolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly Expression<Func<TEntity, string>> _ownerIdSelector;

	public OwnershipPolicyDefinition(string policyName, Expression<Func<TEntity, string>> ownerIdSelector)
	{
		_policyName = policyName;
		_ownerIdSelector = ownerIdSelector;
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
		var currentUserId = userContext.GetCurrentUserId();
		if (string.IsNullOrEmpty(currentUserId))
			return e => false;

		var parameter = Expression.Parameter(typeof(TEntity), "e");

		var visitor = new ParameterRebinder(_ownerIdSelector.Parameters[0], parameter);
		var ownerSelector = visitor.Visit(_ownerIdSelector.Body);

		var userIdConstant = Expression.Constant(currentUserId, typeof(string));
		var equalityCheck = Expression.Equal(ownerSelector, userIdConstant);

		return Expression.Lambda<Func<TEntity, bool>>(equalityCheck, parameter);
	}

	public bool CanEvaluateWithoutDb()
	{
		return false;
	}
}