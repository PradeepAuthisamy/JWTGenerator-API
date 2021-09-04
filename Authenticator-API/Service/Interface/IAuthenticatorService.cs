namespace Authenticator_API.Service.Interface
{
    public interface IAuthenticatorService
    {
        string GenerateToken(string userName, string passWord);
    }
}