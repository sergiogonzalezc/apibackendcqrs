using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackEndProducts.Application.Model;
using BackEndProducts.Infraestructure.Repository;

namespace BackEndProducts.Infraestructure.Repository
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductEF, Product>().ReverseMap();
        }       
    }
}
