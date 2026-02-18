using MediatR;

namespace Tawsella.Application.Features.Customers.Queries.GetCourierPublicProfile
{
    public class GetCourierPublicProfileQuery : IRequest<GetCourierPublicProfileQueryResponse>
    {
        public string CourierId { get; set; }
    }
}
