using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetActiveOrder
{
    public record GetActiveOrderQuery(): IRequest<GetActiveOrderQueryResponse>;

}
