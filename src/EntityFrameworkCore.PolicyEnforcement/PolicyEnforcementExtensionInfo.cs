using EntityFrameworkCore.PolicyEnforcement.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace EntityFrameworkCore.PolicyEnforcement;

internal class PolicyEnforcementExtensionInfo : DbContextOptionsExtensionInfo
{
	private readonly PolicyEnforcementExtension _extension;
	private string? _logFragment;

	public PolicyEnforcementExtensionInfo(PolicyEnforcementExtension extension)
		: base(extension)
	{
		_extension = extension;
	}

	public override bool IsDatabaseProvider => false;

	public override string LogFragment =>
		_logFragment ??= $"PolicyEnforcement: EnableForQueries={_extension.Options.EnableForQueries} " +
		                 $"EnableForCommands={_extension.Options.EnableForCommands}";

	public override int GetServiceProviderHashCode() => 0;

	public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
	{
		debugInfo["PolicyEnforcement:EnableForQueries"] = _extension.Options.EnableForQueries.ToString();
		debugInfo["PolicyEnforcement:EnableForCommands"] = _extension.Options.EnableForCommands.ToString();
		debugInfo["PolicyEnforcement:ThrowOnViolation"] = _extension.Options.ThrowOnViolation.ToString();
	}

	public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
	{
		return other is PolicyEnforcementExtensionInfo;
	}
}