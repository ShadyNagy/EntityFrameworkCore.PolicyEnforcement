namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

public interface IUserContext
{
	string GetCurrentUserId();
	bool IsInRole(string role);
	bool HasPermission(string permission);
}