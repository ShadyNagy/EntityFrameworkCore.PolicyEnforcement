﻿using EntityFrameworkCore.PolicyEnforcement.Interfaces;
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

namespace EntityFrameworkCore.PolicyEnforcement.Interceptors;

internal class PolicySaveChangesInterceptor : SaveChangesInterceptor
{
	private readonly IPolicyEnforcementService _policyService;
	private readonly PolicyEnforcementOptions _options;

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

	private void EnforcePoliciesOnChanges(DbContext context)
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
			var method = typeof(IPolicyEnforcementService)
							.GetMethod(nameof(IPolicyEnforcementService.GetPolicyExpression))
							.MakeGenericMethod(entityType.ClrType);

			var policyExpression = method.Invoke(
							_policyService,
							new object[] { entityType, policyName });

			return EvaluatePolicyForEntity(policyExpression, entry.Entity);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error evaluating policy: {ex.Message}");

			return false;
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

		var funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
		var expressionType = typeof(Expression<>).MakeGenericType(funcType);

		if (!expressionType.IsInstanceOfType(policyExpression))
			return true;

		try
		{
			var compileMethod = expressionType.GetMethod("Compile", Type.EmptyTypes);
			if (compileMethod == null)
				return true;

			var func = compileMethod.Invoke(policyExpression, null);

			var invokeMethod = funcType.GetMethod("Invoke", new[] { entityType });
			if (invokeMethod == null)
				return true;

			return (bool)invokeMethod.Invoke(func, new[] { entity });
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error evaluating policy: {ex.Message}");
			return false;
		}
	}
}