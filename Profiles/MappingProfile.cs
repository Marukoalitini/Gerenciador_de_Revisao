using AutoMapper;
using Motos.Dto.Request;
using Motos.Dto.Response;
using Motos.Models;

namespace Motos.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Request
        CreateMap<ClienteRequest, Cliente>();
        CreateMap<ConcessionariaRequest, Concessionaria>();
        CreateMap<EnderecoRequest, Endereco>();
        CreateMap<MotoRequest, Moto>();
        CreateMap<RevisaoRequest, Revisao>();
        CreateMap<RevisaoItemRequest, RevisaoItem>();
        
        // Response
        CreateMap<Cliente, ClienteResponse>();
        CreateMap<Concessionaria, ConcessionariaResponse>();
        CreateMap<Endereco, EnderecoResponse>();
        CreateMap<Moto, MotoResponse>();
    }
}