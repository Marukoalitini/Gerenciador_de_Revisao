using System.Collections.Generic;

namespace Motos.Dto.Response;

public record class ChecklistTemplateResponse(
    int Id,
    int NumeroRevisao,
    List<string> Modelos,
    List<ItemTemplateResponse> Itens
);
