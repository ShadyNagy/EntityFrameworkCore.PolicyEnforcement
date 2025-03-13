using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class PermissionPolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly string _permission;

	public PermissionPolicyDefinition(string policyName, string permission)
	{
		_policyName = policyName;
		_permission = permission;
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
		if (userContext.HasPermission(_permission))
			return e => true;
		else
			return e => false;
	}

	public bool CanEvaluateWithoutDb()
	{
		return true;
	}
}