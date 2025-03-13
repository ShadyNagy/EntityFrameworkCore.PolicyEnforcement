using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Interceptors;

internal class PolicyQueryInterceptor : IInterceptor, IQueryFilterInterceptor
{
	private readonly IPolicyEnforcementService _policyService;

	public PolicyQueryInterceptor(IPolicyEnforcementService policyService)
	{
		_policyService = policyService;
	}

	public Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(IEntityType? entityType = null)
		where TEntity : class
	{
		var userContext = _policyService.GetUserContext();
		if (userContext == null)
			return e => true;

		try
		{
			if (entityType != null && _policyService.ShouldApplyPolicies(entityType))
				return _policyService.GetPolicyExpression<TEntity>(entityType);

			return e => true;
		}
		catch (Exception ex)
		{
			return e => true;
		}
	}
}