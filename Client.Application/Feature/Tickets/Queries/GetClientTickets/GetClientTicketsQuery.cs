using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetClientTickets
{
    public class GetClientTicketsQuery : IRequest<List<GetClientTicketsQueryResponseDto>>
    {
        public GetClientTicketsQueryDto Filters { get; set; }

        public GetClientTicketsQuery(GetClientTicketsQueryDto filters)
        {
            Filters = filters;
        }
    }
}
