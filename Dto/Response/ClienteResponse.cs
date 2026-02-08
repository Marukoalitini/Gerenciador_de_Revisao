namespace Motos.Dto.Response;

public record class ClienteResponse(
    int Id,
    string Nome,
    string Email,
    string Telefone,
    string Celular
);