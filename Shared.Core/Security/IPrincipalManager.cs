namespace Shared.Core.Security;

public interface IPrincipalManager<out TPrincipal, TId>
{
    public TPrincipal GetCurrentUser();

    public TId GetPrincipalId();
}