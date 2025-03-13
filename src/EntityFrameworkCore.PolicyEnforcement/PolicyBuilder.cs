using System;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Definitions;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;

namespace EntityFrameworkCore.PolicyEnforcement;

public class PolicyBuilder<TEntity> where TEntity : class
{
	private readonly string _policyName;

	internal PolicyBuilder(string policyName = "Default")
	{
		_policyName = policyName;
	}

	public IPolicyDefinition<TEntity> RequireProperty(Expression<Func<TEntity, bool>> predicate)
	{
		return new PropertyPolicyDefinition<TEntity>(_policyName, predicate);
	}

	public IPolicyDefinition<TEntity> RequireRole(string role)
	{
		return new RolePolicyDefinition<TEntity>(_policyName, role);
	}

	public IPolicyDefinition<TEntity> RequirePermission(string permission)
	{
		return new PermissionPolicyDefinition<TEntity>(_policyName, permission);
	}

	public IPolicyDefinition<TEntity> RequireOwnership(Expression<Func<TEntity, string>> ownerIdSelector)
	{
		return new OwnershipPolicyDefinition<TEntity>(_policyName, ownerIdSelector);
	}

	public IPolicyDefinition<TEntity> AllowAnonymous()
	{
		return new AnonymousPolicyDefinition<TEntity>(_policyName);
	}

	public IPolicyDefinition<TEntity> Custom(Func<IUserContext, Expression<Func<TEntity, bool>>> expressionBuilder)
	{
		return new CustomPolicyDefinition<TEntity>(_policyName, expressionBuilder);
	}
}