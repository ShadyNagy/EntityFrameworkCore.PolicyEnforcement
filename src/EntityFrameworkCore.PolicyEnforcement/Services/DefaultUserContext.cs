using EntityFrameworkCore.PolicyEnforcement.Interfaces;

namespace EntityFrameworkCore.PolicyEnforcement.Services;

internal class DefaultUserContext : IUserContext
{
	public string GetCurrentUserId() => null;
	public bool IsInRole(string role) => false;
	public bool HasPermission(string permission) => false;
}