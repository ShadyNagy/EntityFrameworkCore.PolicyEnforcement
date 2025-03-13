using System;
using EntityFrameworkCore.PolicyEnforcement.Interceptors;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using EntityFrameworkCore.PolicyEnforcement.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

public static class PolicyEnforcementServiceCollectionExtensions
{
	public static IServiceCollection AddPolicyEnforcement(this IServiceCollection services,
		Action<PolicyEnforcementOptions>? configureOptions = null)
	{
		services.AddOptions<PolicyEnforcementOptions>()
			.Configure(options => configureOptions?.Invoke(options));

		services.AddScoped<IPolicyEnforcementService, PolicyEnforcementService>();
		services.AddScoped<IQueryFilterInterceptor, PolicyQueryInterceptor>();
		services.AddScoped<ISaveChangesInterceptor, PolicySaveChangesInterceptor>();

		return services;
	}
}