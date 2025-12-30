using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetTicketById
{
    public class GetTicketByIdQuery : IRequest<GetTicketByIdQueryResponseDto>
    {
        public GetTicketByIdQueryDto Filters { get; set; }

        public GetTicketByIdQuery(GetTicketByIdQueryDto filters)
        {
            Filters = filters;
        }
    }
}
