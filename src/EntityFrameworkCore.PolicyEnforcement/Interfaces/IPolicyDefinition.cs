using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

public interface IPolicyDefinition<TEntity> where TEntity : class
{
	IPolicyDefinition<TEntity> Or(IPolicyDefinition<TEntity> other);
	IPolicyDefinition<TEntity> And(IPolicyDefinition<TEntity> other);
	Expression<Func<TEntity, bool>> ToExpression(IUserContext userContext);
	bool CanEvaluateWithoutDb();
}