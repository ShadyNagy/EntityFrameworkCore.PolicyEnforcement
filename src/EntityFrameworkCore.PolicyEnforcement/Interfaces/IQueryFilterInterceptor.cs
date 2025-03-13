using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

/// <summary>
/// Interceptor for applying query filters based on access policies.
/// </summary>
public interface IQueryFilterInterceptor
{
	/// <summary>
	/// Gets the filter expression for an entity type.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="entityType">Optional entity type metadata. If null, entity type will be inferred.</param>
	/// <returns>A predicate expression to be used as a query filter.</returns>
	Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(IEntityType? entityType = null)
		where TEntity : class;
}
