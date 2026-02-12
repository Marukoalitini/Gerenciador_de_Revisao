using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Motos.Services;

public class TokenService
{
    private readonly IConfiguration _configuracao;

    public TokenService(IConfiguration configuracao)
    {
        _configuracao = configuracao;
    }

    public string GerarToken(int id, string nome, string email, string perfil)
    {
        var chave = Encoding.ASCII.GetBytes(_configuracao["Jwt:Key"] ?? "chave_super_secreta_padrao_para_desenvolvimento_123");
        var gerenciadorToken = new JwtSecurityTokenHandler();
        
        var parametrosToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, nome),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, perfil)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = gerenciadorToken.CreateToken(parametrosToken);
        return gerenciadorToken.WriteToken(token);
    }
}