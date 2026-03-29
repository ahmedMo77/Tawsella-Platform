using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetCourierProfile
{
    public record GetCourierProfileQuery() : IRequest<GetCourierProfileQueryResponse>;
}
