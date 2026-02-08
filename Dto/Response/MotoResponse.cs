namespace Motos.Dto.Response;

public record class MotoResponse(
    int Id,
    string Cor,
    string NumeroChassi,
    string Placa,
    DateTime DataDeVenda,
    string NotaFiscal,
    int Serie,
    string ImgDecalqueChassi
    );