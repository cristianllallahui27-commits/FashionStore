using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;

namespace FashionStore.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Categoria
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();

            // Prenda — mapea Categoria.Nombre a CategoriaNombre
            CreateMap<Prenda, PrendaDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                           opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : null))
                .ReverseMap()
                .ForMember(dest => dest.Categoria, opt => opt.Ignore());

            // Cliente
            CreateMap<Cliente, ClienteDTO>().ReverseMap();

            // Vendedor
            CreateMap<Vendedor, VendedorDTO>().ReverseMap();

            // MetodoPago
            CreateMap<MetodoPago, MetodoPagoDTO>().ReverseMap();
        }
    }
}