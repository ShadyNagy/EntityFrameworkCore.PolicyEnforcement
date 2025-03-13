using EntityFrameworkCore.PolicyEnforcement.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.PolicyEnforcement.Models;
using System.Linq.Expressions;
using System;
using System.Collections.Concurrent;

namespace EntityFrameworkCore.PolicyEnforcement.Interceptors;

internal class PolicySaveChangesInterceptor : SaveChangesInterceptor
{
	private readonly IPolicyEnforcementService _policyService;
	private readonly PolicyEnforcementOptions _options;
	private static readonly ConcurrentDictionary<string, Delegate> _compiledExpressionCache = new();

	public PolicySaveChangesInterceptor(
							IPolicyEnforcementService policyService,
							PolicyEnforcementOptions options)
	{
		_policyService = policyService;
		_options = options;
	}

	public override InterceptionResult<int> SavingChanges(
							DbContextEventData eventData,
							InterceptionResult<int> result)
	{
		if (_options.EnableForCommands)
		{
			EnforcePoliciesOnChanges(eventData.Context);
		}
		return base.SavingChanges(eventData, result);
	}

	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
							DbContextEventData eventData,
							InterceptionResult<int> result,
							CancellationToken cancellationToken = default)
	{
		if (_options.EnableForCommands)
		{
			EnforcePoliciesOnChanges(eventData.Context);
		}
		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void EnforcePoliciesOnChanges(DbContext? context)
	{
		if (context == null) return;

		var entries = context.ChangeTracker.Entries()
								.Where(e => e.State == EntityState.Added ||
																		e.State == EntityState.Modified ||
																		e.State == EntityState.Deleted);

		foreach (var entry in entries)
		{
			var entityType = context.Model.FindEntityType(entry.Entity.GetType());
			if (entityType != null && _policyService.ShouldApplyPolicies(entityType))
			{
				if (!CanModifyEntity(entry, entityType))
				{
					if (_options.ThrowOnViolation)
					{
						throw new PolicyViolationException(
												entityType.Name,
												GetOperationName(entry.State));
					}
					else
					{
						entry.State = EntityState.Unchanged;
					}
				}
			}
		}
	}

	private bool CanModifyEntity(EntityEntry entry, IEntityType entityType)
	{
		if (entry.Entity is IDefineOwnAccessPolicy selfChecking)
		{
			return selfChecking.CanAccess(
									_policyService.GetUserContext(),
									GetOperationName(entry.State));
		}

		var policyName = GetPolicyNameForOperation(entry.State);

		try
		{
			var expression = GetPolicyExpression(entityType.ClrType, policyName);
			if (expression == null)
				return true;

			return EvaluatePolicyForEntity(expression, entry.Entity);
		}
		catch (Exception ex)
		{
			//TODO: use ILogger
			// System.Diagnostics.Debug.WriteLine($"Error evaluating policy: {ex.Message}");
			return false;
		}
	}

	private object? GetPolicyExpression(Type entityType, string policyName)
	{
		try
		{
			var method = typeof(IPolicyEnforcementService)
									.GetMethod(nameof(IPolicyEnforcementService.GetPolicyExpression))
									?.MakeGenericMethod(entityType);

			if (method == null) return null;

			return method.Invoke(
									_policyService,
									new object[] { entityType, policyName });
		}
		catch (Exception ex)
		{
			//TODO: use ILogger
			//System.Diagnostics.Debug.WriteLine($"Error getting policy expression: {ex.Message}");
			return null;
		}
	}

	private string GetOperationName(EntityState state)
	{
		return state switch
		{
			EntityState.Added => "Create",
			EntityState.Modified => "Update",
			EntityState.Deleted => "Delete",
			_ => "Read"
		};
	}

	private string GetPolicyNameForOperation(EntityState state)
	{
		return state switch
		{
			EntityState.Added => "Create",
			EntityState.Modified => "Update",
			EntityState.Deleted => "Delete",
			_ => "Default"
		};
	}

	private bool EvaluatePolicyForEntity(object policyExpression, object entity)
	{
		if (policyExpression == null)
			return true;

		var entityType = entity.GetType();

		var cacheKey = $"{entityType.FullName}_{policyExpression.GetHashCode()}";

		var compiledFunc = _compiledExpressionCache.GetOrAdd(cacheKey, _ =>
		{
			var funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
			var expressionType = typeof(Expression<>).MakeGenericType(funcType);

			if (!expressionType.IsInstanceOfType(policyExpression))
			{
				var parameter = Expression.Parameter(entityType, "e");
				var trueConstant = Expression.Constant(true, typeof(bool));
				var lambda = Expression.Lambda(funcType, trueConstant, parameter);
				return lambda.Compile();
			}

			try
			{
				var compileMethod = expressionType.GetMethod("Compile", Type.EmptyTypes);
				if (compileMethod == null)
				{
					var parameter = Expression.Parameter(entityType, "e");
					var trueConstant = Expression.Constant(true, typeof(bool));
					var lambda = Expression.Lambda(funcType, trueConstant, parameter);
					return lambda.Compile();
				}

				return (Delegate)compileMethod.Invoke(policyExpression, null)!;
			}
			catch (Exception ex)
			{
				//TODO: use ILogger
				//System.Diagnostics.Debug.WriteLine($"Error compiling expression: {ex.Message}");

				var parameter = Expression.Parameter(entityType, "e");
				var falseConstant = Expression.Constant(false, typeof(bool));
				var lambda = Expression.Lambda(funcType, falseConstant, parameter);
				return lambda.Compile();
			}
		});

		try
		{
			var funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
			var invokeMethod = funcType.GetMethod("Invoke", new[] { entityType });
			if (invokeMethod == null)
				return true;

			return (bool)invokeMethod.Invoke(compiledFunc, new[] { entity });
		}
		catch (Exception ex)
		{
			//TODO: use ILogger
			//System.Diagnostics.Debug.WriteLine($"Error evaluating policy: {ex.Message}");
			return false;
		}
	}
}