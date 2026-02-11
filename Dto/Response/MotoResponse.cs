using System.Text.Json.Serialization;
using Motos.Enums;

namespace Motos.Dto.Response;

public record class MotoResponse(
    int Id, 
    ModeloMoto ModeloMoto,
    string Cor,
    string NumeroChassi,
    string Placa,
    DateTime DataDeVenda,
    string NotaFiscal,
    int Serie,
    string ImgDecalqueChassi,
    List<RevisaoResponse> Revisoes
    )
{
    public string NomeModelo => ModeloMoto.GetDisplayName();
}