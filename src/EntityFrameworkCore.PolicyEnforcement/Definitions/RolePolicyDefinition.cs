using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using System;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Models;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class RolePolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly string _role;

	public RolePolicyDefinition(string policyName, string role)
	{
		_policyName = policyName;
		_role = role;
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
		if (userContext.IsInRole(_role))
			return e => true;
		else
			return e => false;
	}

	public bool CanEvaluateWithoutDb()
	{
		return true;
	}
}