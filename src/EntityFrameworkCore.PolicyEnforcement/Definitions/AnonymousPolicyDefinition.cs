using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class AnonymousPolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;

	public AnonymousPolicyDefinition(string policyName)
	{
		_policyName = policyName;
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
		return e => true;
	}

	public bool CanEvaluateWithoutDb()
	{
		return true;
	}
}