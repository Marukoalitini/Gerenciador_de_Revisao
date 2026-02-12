using Motos.Enums;

namespace Motos.Dto.Response;

public record MotoResponse(
    int Id, 
    ModeloMoto ModeloMoto,
    string Cor,
    string NumeroChassi,
    string Placa,
    DateOnly DataDeVenda,
    string NotaFiscal,
    int Serie,
    string ImgDecalqueChassi
    );