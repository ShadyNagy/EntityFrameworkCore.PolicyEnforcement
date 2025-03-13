using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using EntityFrameworkCore.PolicyEnforcement.Models;
using System;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PolicyEnforcement.Definitions;

internal class CustomPolicyDefinition<TEntity> : IPolicyDefinition<TEntity> where TEntity : class
{
	private readonly string _policyName;
	private readonly Func<IUserContext, Expression<Func<TEntity, bool>>> _expressionBuilder;

	public CustomPolicyDefinition(string policyName, Func<IUserContext, Expression<Func<TEntity, bool>>> expressionBuilder)
	{
		_policyName = policyName;
		_expressionBuilder = expressionBuilder;
	}

	public IPolicyDefinition<TEntity> Or(IPolicyDefinition<TEntity> other)
	{
		return new CompositePolicyDefinition<TEntity>(_policyName, this, other, PolicyCompositionType.Or);
	}

	public IPolicyDefinition<TEntity> And(IPolicyDefinition<TEntity> other)
	{
		return new CompositePolicyDefinition<TEntity>(_policyName, this, other, PolicyCompositionType.And);
	}

	public Expression<Func<TEntity, bool>> ToExpression(IUserContext userContext)
	{
		return _expressionBuilder(userContext);
	}

	public bool CanEvaluateWithoutDb()
	{
		return false;
	}
}