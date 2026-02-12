namespace Motos.Dto.Response;

public record class ConcessionariaResponse(
    int Id,
    string NomeConcessionaria,
    string Telefone,
    string Cnpj,
    List<EnderecoResponse> Enderecos
);