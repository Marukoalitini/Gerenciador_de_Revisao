namespace Motos.Dto.Response;

public record class ConcessionariaResponse(
    string Nome,
    string Telefone,
    string Cnpj,
    EnderecoResponse[] Enderecos
);