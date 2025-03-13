using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

public interface IPolicyEnforcementService
{
	void SetUserContext(IUserContext userContext);
	IUserContext GetUserContext();
	bool ShouldApplyPolicies(IEntityType entityType);
	Expression<Func<TEntity, bool>> GetPolicyExpression<TEntity>(
		IEntityType entityType, string policyName = "Default") where TEntity : class;
}