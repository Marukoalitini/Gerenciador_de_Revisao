namespace Motos.Dto.Response;

public record EnderecoResponse(
    int Id,
    string Rua,
    int Numero,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep,
    string? Complemento
);