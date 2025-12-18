using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Interfaces;

public class GetTicketMessagesQuery : IRequest<List<TicketMessageDto>>
{
    public int TicketId { get; set; }

    public GetTicketMessagesQuery(int ticketId)
    {
        TicketId = ticketId;
    }
}

public class GetTicketMessagesQueryHandler : IRequestHandler<GetTicketMessagesQuery, List<TicketMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketMessagesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketMessageDto>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception($"Ticket {request.TicketId} not found");

        return ticket.Messages.Select(m => new TicketMessageDto
        {
            Id = m.Id,
            TicketId = m.TicketId,
            AuthorId = m.AuthorId,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList();
    }
}