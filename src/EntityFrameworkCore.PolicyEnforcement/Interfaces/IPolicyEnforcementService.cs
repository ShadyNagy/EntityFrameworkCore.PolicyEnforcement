using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

/// <summary>
/// Service for enforcing access policies on entities.
/// </summary>
public interface IPolicyEnforcementService
{
	/// <summary>
	/// Sets the user context for policy evaluation.
	/// </summary>
	/// <param name="userContext">The user context containing user information.</param>
	void SetUserContext(IUserContext userContext);

	/// <summary>
	/// Gets the current user context.
	/// </summary>
	/// <returns>The current user context.</returns>
	IUserContext GetUserContext();

	/// <summary>
	/// Determines whether policies should be applied to the specified entity type.
	/// </summary>
	/// <param name="entityType">The entity type to check.</param>
	/// <returns>True if policies should be applied; otherwise, false.</returns>
	bool ShouldApplyPolicies(IEntityType entityType);

	/// <summary>
	/// Gets the policy expression for an entity type.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="entityType">The entity type metadata.</param>
	/// <param name="policyName">The name of the policy to retrieve. Defaults to "Default".</param>
	/// <returns>A predicate expression representing the policy.</returns>
	Expression<Func<TEntity, bool>> GetPolicyExpression<TEntity>(
		IEntityType entityType, string policyName = "Default") where TEntity : class;
}