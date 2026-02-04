using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.NotificationDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.DTOs.ReviewDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. Order Mappings
            CreateMap<Order, OrderResponseDto>()
                // الوصول لبيانات المندوب من خلال علاقة Courier -> User
                .ForMember(dest => dest.CourierName, opt => opt.MapFrom(src => src.Courier != null ? src.Courier.User.FullName : null))
                .ForMember(dest => dest.CourierPhone, opt => opt.MapFrom(src => src.Courier != null ? src.Courier.User.PhoneNumber : null));

            // من Entity لـ ProfileDto (مع سحب بيانات من الـ User والـ Wallet)
            CreateMap<Courier, CourierProfileDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(d => d.WalletBalance, o => o.MapFrom(s => s.Wallet != null ? s.Wallet.Balance : 0));

            // من Entity لـ PublicProfile (اللي بيشوفه العميل)
            CreateMap<Courier, CourierPublicProfileDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.CompletedDeliveries, o => o.MapFrom(s => s.Orders.Count(o => o.Status == OrderStatus.Delivered)))
                .ForMember(d => d.Reviews, o => o.MapFrom(s => s.ReviewsReceived));

            CreateMap<Review, CourierReviewItemDto>();

            // 3. Order Application Mappings
            CreateMap<OrderApplication, OrderApplicationWithCourierDto>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AppliedAt, opt => opt.MapFrom(src => src.CreatedAt))
                // هنا بنعمل Map للـ Courier Entity كاملة لـ CourierPublicProfileDto
                .ForMember(dest => dest.Courier, opt => opt.MapFrom(src => src.Courier));

            // 4. Notification Mappings
            CreateMap<Notification, NotificationDto>();

            // 5. Review Mappings
            CreateMap<Review, CourierReviewItemDto>();

            // 6. Customer Profile
            CreateMap<Customer, CustomerProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));
        }
    }
}
