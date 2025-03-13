using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

public static class PolicyDbContextExtensions
{
	public static void SetUserContext(this DbContext dbContext, IUserContext userContext)
	{
		if (dbContext is null)
			throw new ArgumentNullException(nameof(dbContext));

		dbContext.GetService<IPolicyEnforcementService>().SetUserContext(userContext);
	}
}