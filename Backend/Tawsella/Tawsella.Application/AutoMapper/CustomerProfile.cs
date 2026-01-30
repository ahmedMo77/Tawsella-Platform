using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.AutoMapper
{
    public class CustomerMappingProfile  : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerProfileDto>()
          .ForMember(dest => dest.FullName,
              opt => opt.MapFrom(src => src.User.FullName))
          .ForMember(dest => dest.Email,
              opt => opt.MapFrom(src => src.User.Email))
          .ForMember(dest => dest.PhoneNumber,
              opt => opt.MapFrom(src => src.User.PhoneNumber)).ReverseMap();

            CreateMap<UpdateProfileDto, Customer>().ReverseMap();
              


        }
    }
}
