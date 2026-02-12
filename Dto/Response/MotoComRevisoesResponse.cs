using System.Text.Json.Serialization;
using Motos.Enums;

namespace Motos.Dto.Response;

public record class MotoComRevisoesResponse(
    int Id, 
    ModeloMoto ModeloMoto,
    string Cor,
    string NumeroChassi,
    string Placa,
    DateOnly DataDeVenda,
    string NotaFiscal,
    int Serie,
    string ImgDecalqueChassi,
    List<RevisaoSemClienteEMotoResponse> Revisoes
    )
{
    public string NomeModelo => ModeloMoto.GetDisplayName();
}