namespace Motos.Dto.Response;

public record LoginResponse(
    string Nome,
    string Email,
    string Token
);