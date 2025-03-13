namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

public interface IDefineOwnAccessPolicy
{
	bool CanAccess(IUserContext userContext, string operation);
}