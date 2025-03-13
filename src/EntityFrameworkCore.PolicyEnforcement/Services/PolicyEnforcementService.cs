using System;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Services;

internal class PolicyEnforcementService : IPolicyEnforcementService
{
	private readonly PolicyEnforcementOptions _options;
	private IUserContext _userContext;

	public PolicyEnforcementService(PolicyEnforcementOptions options)
	{
		_options = options;
		_userContext = new DefaultUserContext();
	}

	public void SetUserContext(IUserContext userContext)
	{
		_userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
	}

	public IUserContext GetUserContext() => _userContext;

	public bool ShouldApplyPolicies(IEntityType entityType)
	{
		if (_options.SkipPolicyCheckForAdmins &&
		    _userContext.IsInRole(_options.AdminRoleName))
			return false;

		return entityType.GetAnnotations()
			.Any(a => a.Name.StartsWith("AccessPolicy:"));
	}

	public Expression<Func<TEntity, bool>> GetPolicyExpression<TEntity>(
		IEntityType entityType, string policyName = "Default")
		where TEntity : class
	{
		var annotation = entityType.FindAnnotation($"AccessPolicy:{policyName}");

		if (annotation?.Value is IPolicyDefinition<TEntity> policy)
		{
			return policy.ToExpression(_userContext);
		}

		if (policyName != "Default")
		{
			var defaultAnnotation = entityType.FindAnnotation("AccessPolicy:Default");
			if (defaultAnnotation?.Value is IPolicyDefinition<TEntity> defaultPolicy)
			{
				return defaultPolicy.ToExpression(_userContext);
			}
		}

		return e => true;
	}
}
