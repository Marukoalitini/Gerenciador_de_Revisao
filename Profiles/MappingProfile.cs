using AutoMapper;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Enums;
using Motos.Models;

namespace Motos.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Request
        CreateMap<CriarClienteRequest, Cliente>();
        CreateMap<CriarConcessionariaRequest, Concessionaria>();
        CreateMap<AdicionarEnderecoRequest, Endereco>();
        CreateMap<MotoRequest, Moto>();
        CreateMap<RevisaoRequest, Revisao>();
        CreateMap<RevisaoItemRequest, RevisaoItem>();

        CreateMap<AtualizarClienteRequest, Cliente>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<AtualizarConcessionariaRequest, Concessionaria>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<AtualizarMotoRequest, Moto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Response
        CreateMap<Cliente, ClienteResponse>();
        CreateMap<Concessionaria, ConcessionariaResponse>();
        CreateMap<Endereco, EnderecoResponse>();
        CreateMap<Moto, MotoResponse>();
        CreateMap<Revisao, RevisaoResponse>();
            
        CreateMap<RevisaoItem, RevisaoItemResponse>();
        
        CreateMap<ItemTemplate, ItemTemplateResponse>();
        CreateMap<ChecklistTemplate, ChecklistTemplateResponse>()
            .ForCtorParam("Modelos", opt => opt.MapFrom(src => 
                src.Modelos.Select(m => ObterNomeModelo(m)).ToList()));
    }

    private string ObterNomeModelo(string modeloStr)
    {
        return Enum.TryParse<ModeloMoto>(modeloStr, out var modelo) 
            ? modelo.GetDisplayName() 
            : modeloStr;
    }
}
