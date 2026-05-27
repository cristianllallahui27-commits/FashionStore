using AutoMapper;
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

            // Prenda
            CreateMap<Prenda, PrendaDTO>().ReverseMap();

            // Cliente
            CreateMap<Cliente, ClienteDTO>().ReverseMap();

            // Vendedor
            CreateMap<Vendedor, VendedorDTO>().ReverseMap();

            // MetodoPago
            CreateMap<MetodoPago, MetodoPagoDTO>().ReverseMap();
        }
    }
}
