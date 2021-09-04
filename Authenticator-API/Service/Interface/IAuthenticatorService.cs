using System.Threading.Tasks;

namespace Authenticator_API.Service.Interface
{
    public interface IAuthenticatorService
    {
        Task<string> GenerateTokenAsync(string userName);
    }
}