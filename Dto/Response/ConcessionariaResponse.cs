namespace Motos.Dto.Response;

public record class ConcessionariaResponse(
    int Id,
    string Nome,
    string Telefone,
    string Cnpj,
    EnderecoResponse[] Enderecos
);