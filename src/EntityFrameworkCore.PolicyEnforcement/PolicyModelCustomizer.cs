using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement;

internal class PolicyModelCustomizer : ModelCustomizer
{
	private readonly IQueryFilterInterceptor _queryFilterInterceptor;

	public PolicyModelCustomizer(ModelCustomizerDependencies dependencies,
		IQueryFilterInterceptor queryFilterInterceptor)
		: base(dependencies)
	{
		_queryFilterInterceptor = queryFilterInterceptor;
	}

	public override void Customize(ModelBuilder modelBuilder, DbContext context)
	{
		base.Customize(modelBuilder, context);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			if (!entityType.GetAnnotations().Any(a => a.Name.StartsWith("AccessPolicy:")))
				continue;

			var entityClrType = entityType.ClrType;

			var filterMethod = typeof(PolicyModelCustomizer)
				.GetMethod(nameof(ApplyQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(entityClrType);

			filterMethod.Invoke(this, new object[] { modelBuilder });
		}
	}

	private void ApplyQueryFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class
	{
		var entityType = modelBuilder.Model.FindEntityType(typeof(TEntity));
		var filterExpression = _queryFilterInterceptor.GetFilterExpression<TEntity>(entityType as IEntityType);
		modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
	}
}