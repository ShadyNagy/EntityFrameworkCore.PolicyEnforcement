using EntityFrameworkCore.PolicyEnforcement.Interceptors;
using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using EntityFrameworkCore.PolicyEnforcement.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.PolicyEnforcement.Extensions;

internal class PolicyEnforcementExtension : IDbContextOptionsExtension
{
	private readonly PolicyEnforcementOptions _options;
	private DbContextOptionsExtensionInfo _info;
	public PolicyEnforcementOptions Options => _options;

	public PolicyEnforcementExtension(PolicyEnforcementOptions options)
	{
		_options = options;
	}

	public DbContextOptionsExtensionInfo Info =>
		_info ??= new PolicyEnforcementExtensionInfo(this);

	public void ApplyServices(IServiceCollection services)
	{
		services.AddSingleton(_options);
		services.AddScoped<IPolicyEnforcementService, PolicyEnforcementService>();
		services.AddScoped<IQueryFilterInterceptor, PolicyQueryInterceptor>();
		services.AddScoped<ISaveChangesInterceptor, PolicySaveChangesInterceptor>();
		services.AddScoped<IModelCustomizer, PolicyModelCustomizer>();
	}

	public void Validate(IDbContextOptions options) { }
}