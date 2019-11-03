using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateAccessToken(string id, string userName);
    }
}
