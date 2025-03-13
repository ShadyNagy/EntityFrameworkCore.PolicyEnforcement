using System;

namespace EntityFrameworkCore.PolicyEnforcement.Models;

/// <summary>
/// Exception thrown when a policy violation occurs.
/// </summary>
public class PolicyViolationException : Exception
{
	/// <summary>
	/// Gets the name of the entity for which access was denied.
	/// </summary>
	public string EntityName { get; }

	/// <summary>
	/// Gets the name of the policy that denied access.
	/// </summary>
	public string PolicyName { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="PolicyViolationException"/> class.
	/// </summary>
	/// <param name="entityName">The name of the entity for which access was denied.</param>
	/// <param name="policyName">The name of the policy that denied access.</param>
	public PolicyViolationException(string entityName, string policyName)
		: base($"Access to entity '{entityName}' was denied by policy '{policyName}'")
	{
		EntityName = entityName;
		PolicyName = policyName;
	}
}