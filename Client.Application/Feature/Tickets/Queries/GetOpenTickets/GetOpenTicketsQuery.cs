using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetOpenTickets
{
    public class GetOpenTicketsQuery : IRequest<List<GetOpenTicketsQueryResponseDto>>
    {
        public GetOpenTicketsQueryDto Filters { get; set; }

        public GetOpenTicketsQuery(GetOpenTicketsQueryDto filters)
        {
            Filters = filters;
        }
    }
}
