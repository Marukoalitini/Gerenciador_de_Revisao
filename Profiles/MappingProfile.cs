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
        CreateMap<Cliente, ClienteResponse>()
            .PreserveReferences();
        CreateMap<Concessionaria, ConcessionariaResponse>();
        CreateMap<Endereco, EnderecoResponse>();
        CreateMap<Moto, MotoResponse>()
            .PreserveReferences();
        CreateMap<Revisao, RevisaoResponse>()
            .PreserveReferences();

        CreateMap<RevisaoItem, RevisaoItemResponse>();
    }
}
