using System;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

public static class PolicyModelBuilderExtensions
{
	public static EntityTypeBuilder<TEntity> HasAccessPolicy<TEntity>(
			this EntityTypeBuilder<TEntity> entityTypeBuilder,
			Func<PolicyBuilder<TEntity>, IPolicyDefinition<TEntity>> policyBuilderFunc,
			string policyName = "Default")
			where TEntity : class
	{
		var builder = new PolicyBuilder<TEntity>(policyName);
		var policy = policyBuilderFunc(builder);

		entityTypeBuilder.Metadata.SetAnnotation($"AccessPolicy:{policyName}", policy);

		return entityTypeBuilder;
	}
}