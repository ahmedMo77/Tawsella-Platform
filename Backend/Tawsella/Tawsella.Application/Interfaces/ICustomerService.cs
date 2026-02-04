using System.Collections.Generic;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerProfileDto> GetProfile(string CustomerId);
        Task<BaseToReturnDto> UpdateCustomerProfile(string CustomerId, UpdateProfileDto dto);
        Task<CustomerStatisticsDto> GetCustomerStatistics(string CustomerId);
        Task<CourierPublicProfileDto> GetCourierPublicProfileAsync(string courierId); // maybe move to ICourierService
    }
}