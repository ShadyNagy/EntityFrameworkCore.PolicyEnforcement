using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using System;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Models;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class PropertyPolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly Expression<Func<TEntity, bool>> _predicate;

	public PropertyPolicyDefinition(string policyName, Expression<Func<TEntity, bool>> predicate)
	{
		_policyName = policyName;
		_predicate = predicate;
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
		return _predicate;
	}

	public bool CanEvaluateWithoutDb()
	{
		return false;
	}
}
