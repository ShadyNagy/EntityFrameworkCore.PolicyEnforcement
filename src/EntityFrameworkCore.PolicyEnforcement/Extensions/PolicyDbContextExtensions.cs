using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

/// <summary>
/// Extension methods for DbContext to support policy enforcement.
/// </summary>
public static class PolicyDbContextExtensions
{
	/// <summary>
	/// Sets the user context for the current database context instance.
	/// </summary>
	/// <param name="dbContext">The database context.</param>
	/// <param name="userContext">The user context containing user information for policy evaluation.</param>
	/// <exception cref="ArgumentNullException">Thrown when dbContext is null.</exception>
	public static void SetUserContext(this DbContext dbContext, IUserContext userContext)
	{
		if (dbContext is null)
			throw new ArgumentNullException(nameof(dbContext));

		dbContext.GetService<IPolicyEnforcementService>().SetUserContext(userContext);
	}
}