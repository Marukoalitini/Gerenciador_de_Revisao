namespace Motos.Dto.Response;

public record class EnderecoResponse(
    int Id,
    string Cep,
    string Rua,
    int Numero,
    string Bairro,
    string? Complemento
);