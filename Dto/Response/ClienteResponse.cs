namespace Motos.Dto.Response;

public record class ClienteResponse(
    int Id,
    string NomeCliente,
    string Email,
    string Telefone,
    string Celular,
    EnderecoResponse? Endereco,
    List<MotoResponse>? Motos
);