using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.DTOs.CustomerDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.AutoMapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponseDto>().ReverseMap(); 

            CreateMap<Order, OrderListDTO>().ForMember(dest => dest.OrderId, src => src
            .MapFrom(o => o.Id)).ForMember(dest => dest.TotalPrice, src => src
            .MapFrom(o => o.FinalPrice))
             .ForMember(dest => dest.Status, src => src
            .MapFrom(o => o.Status)).ReverseMap();

            CreateMap<CreateOrderDto, Order>()
          .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending))
          .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => PaymentStatus.Pending))
          .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
          .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
          .ForMember(dest => dest.UserId, opt => opt.Ignore())
          .ForMember(dest => dest.CourierEarnings, opt => opt.Ignore()) 
          .ForMember(dest => dest.PlatformCommission, opt => opt.Ignore())
          .ForMember(dest => dest.EstimatedPrice, opt => opt.Ignore()) 
          .ForAllMembers(opt => opt.MapFrom(src => src));

        }
    }
}
