using System;
using EntityFrameworkCore.PolicyEnforcement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

/// <summary>
/// Extension methods for DbContextOptionsBuilder to configure policy enforcement.
/// </summary>
public static class PolicyDbContextOptionsExtensions
{
	/// <summary>
	/// Enables policy enforcement for the DbContext.
	/// </summary>
	/// <param name="optionsBuilder">The options builder.</param>
	/// <param name="optionsAction">Optional action to configure policy enforcement options.</param>
	/// <returns>The options builder for chaining.</returns>
	public static DbContextOptionsBuilder UsePolicyEnforcement(
		this DbContextOptionsBuilder optionsBuilder,
		Action<PolicyEnforcementOptions>? optionsAction = null)
	{
		var options = new PolicyEnforcementOptions();
		optionsAction?.Invoke(options);

		((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
			.AddOrUpdateExtension(new PolicyEnforcementExtension(options));

		return optionsBuilder;
	}
}