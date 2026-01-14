using Client.Application.Feature.Tickets.Queries.GetTicketMessages;
using MediatR;

public class GetTicketMessagesQuery : IRequest<GetTicketMessagesQueryResponseDto>
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; } 
    public bool IsTechSupport { get; set; } 

    public GetTicketMessagesQuery(Guid ticketId, Guid userId, bool isTechSupport)
    {
        TicketId = ticketId;
        UserId = userId;
        IsTechSupport = isTechSupport;
    }
}
