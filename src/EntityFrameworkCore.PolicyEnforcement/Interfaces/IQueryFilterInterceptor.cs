using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

public interface IQueryFilterInterceptor
{
	Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(IEntityType? entityType = null)
		where TEntity : class;
}
