using System;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

/// <summary>
/// Extension methods for EntityTypeBuilder to configure access policies.
/// </summary>
public static class PolicyModelBuilderExtensions
{
	/// <summary>
	/// Defines an access policy for an entity type.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="entityTypeBuilder">The entity type builder.</param>
	/// <param name="policyBuilderFunc">Function to build and compose the policy.</param>
	/// <param name="policyName">Optional name for the policy. Defaults to "Default".</param>
	/// <returns>The entity type builder for chaining.</returns>
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