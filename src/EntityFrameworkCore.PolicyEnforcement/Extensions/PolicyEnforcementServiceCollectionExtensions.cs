using System;
using EntityFrameworkCore.PolicyEnforcement.Interceptors;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using EntityFrameworkCore.PolicyEnforcement.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register policy enforcement services.
/// </summary>
public static class PolicyEnforcementServiceCollectionExtensions
{
  /// <summary>
  /// Adds policy enforcement services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configureOptions">Optional action to configure policy enforcement options.</param>
  /// <returns>The service collection for chaining.</returns>
  public static IServiceCollection AddPolicyEnforcement(this IServiceCollection services,
    Action<PolicyEnforcementOptions>? configureOptions = null)
  {
    var options = new PolicyEnforcementOptions();
    configureOptions?.Invoke(options);

    services.AddSingleton(options);
    services.AddScoped<IPolicyEnforcementService, PolicyEnforcementService>();
    services.AddScoped<IQueryFilterInterceptor, PolicyQueryInterceptor>();
    services.AddScoped<ISaveChangesInterceptor, PolicySaveChangesInterceptor>();

    return services;
  }
}
