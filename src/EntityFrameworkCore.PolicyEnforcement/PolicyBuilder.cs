using System;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Definitions;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;

namespace EntityFrameworkCore.PolicyEnforcement;

/// <summary>
/// Builder class for creating and composing access policies for entities.
/// </summary>
/// <typeparam name="TEntity">The entity type for which the policy is being defined.</typeparam>
public class PolicyBuilder<TEntity> where TEntity : class
{
	private readonly string _policyName;

	internal PolicyBuilder(string policyName = "Default")
	{
		_policyName = policyName;
	}

	/// <summary>
	/// Creates a policy that requires entities to satisfy a specific property-based condition.
	/// </summary>
	/// <param name="predicate">The expression that defines the property conditions to be met.</param>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> RequireProperty(Expression<Func<TEntity, bool>> predicate)
	{
		return new PropertyPolicyDefinition<TEntity>(_policyName, predicate);
	}

	/// <summary>
	/// Creates a policy that requires the current user to be in a specific role.
	/// </summary>
	/// <param name="role">The role name that the user must have.</param>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> RequireRole(string role)
	{
		return new RolePolicyDefinition<TEntity>(_policyName, role);
	}

	/// <summary>
	/// Creates a policy that requires the current user to have a specific permission.
	/// </summary>
	/// <param name="permission">The permission that the user must have.</param>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> RequirePermission(string permission)
	{
		return new PermissionPolicyDefinition<TEntity>(_policyName, permission);
	}

	/// <summary>
	/// Creates a policy that requires the current user to be the owner of the entity.
	/// </summary>
	/// <param name="ownerIdSelector">Expression to select the owner ID property from the entity.</param>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> RequireOwnership(Expression<Func<TEntity, string>> ownerIdSelector)
	{
		return new OwnershipPolicyDefinition<TEntity>(_policyName, ownerIdSelector);
	}

	/// <summary>
	/// Creates a policy that allows anonymous access to the entity.
	/// </summary>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> AllowAnonymous()
	{
		return new AnonymousPolicyDefinition<TEntity>(_policyName);
	}

	/// <summary>
	/// Creates a custom policy using a provided expression builder function.
	/// </summary>
	/// <param name="expressionBuilder">Function that builds a filter expression based on the user context.</param>
	/// <returns>A policy definition that can be further composed with other policies.</returns>
	public IPolicyDefinition<TEntity> Custom(Func<IUserContext, Expression<Func<TEntity, bool>>> expressionBuilder)
	{
		return new CustomPolicyDefinition<TEntity>(_policyName, expressionBuilder);
	}
}