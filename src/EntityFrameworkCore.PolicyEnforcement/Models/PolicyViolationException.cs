using System;

namespace EntityFrameworkCore.PolicyEnforcement.Models;

public class PolicyViolationException : Exception
{
	public string EntityName { get; }
	public string PolicyName { get; }

	public PolicyViolationException(string entityName, string policyName)
		: base($"Access to entity '{entityName}' was denied by policy '{policyName}'")
	{
		EntityName = entityName;
		PolicyName = policyName;
	}
}