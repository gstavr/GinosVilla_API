using GinosVilla_Web.Models.Dto;

namespace GinosVilla_Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDTO);
        TokenDTO GetToken();
        void ClearToken();
    }
}
