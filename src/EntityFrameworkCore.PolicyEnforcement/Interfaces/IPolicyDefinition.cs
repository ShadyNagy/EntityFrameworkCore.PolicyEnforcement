using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

/// <summary>
/// Represents a policy definition that can be composed with other policies.
/// </summary>
/// <typeparam name="TEntity">The entity type this policy applies to.</typeparam>
public interface IPolicyDefinition<TEntity> where TEntity : class
{
	/// <summary>
	/// Combines this policy with another using a logical OR operation.
	/// </summary>
	/// <param name="other">The other policy to combine with.</param>
	/// <returns>A new composite policy representing the OR combination.</returns>
	IPolicyDefinition<TEntity> Or(IPolicyDefinition<TEntity> other);

	/// <summary>
	/// Combines this policy with another using a logical AND operation.
	/// </summary>
	/// <param name="other">The other policy to combine with.</param>
	/// <returns>A new composite policy representing the AND combination.</returns>
	IPolicyDefinition<TEntity> And(IPolicyDefinition<TEntity> other);

	/// <summary>
	/// Converts the policy definition to a LINQ expression based on the current user context.
	/// </summary>
	/// <param name="userContext">The user context containing user information.</param>
	/// <returns>A predicate expression that can be used in LINQ queries.</returns>
	Expression<Func<TEntity, bool>> ToExpression(IUserContext userContext);

	/// <summary>
	/// Determines whether this policy can be evaluated without database access.
	/// </summary>
	/// <returns>True if the policy can be evaluated without database access; otherwise, false.</returns>
	bool CanEvaluateWithoutDb();
}