using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQuery : IRequest<GetTicketMessagesQueryResponseDto>
    {
        public GetTicketMessagesQueryDto Filters { get; set; }

        public GetTicketMessagesQuery(GetTicketMessagesQueryDto filters)
        {
            Filters = filters;
        }
    }
}
