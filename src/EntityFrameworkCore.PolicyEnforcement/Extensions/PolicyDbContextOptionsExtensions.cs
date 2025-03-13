using System;
using EntityFrameworkCore.PolicyEnforcement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

public static class PolicyDbContextOptionsExtensions
{
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