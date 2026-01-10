using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetClientTicketById;
public record GetClientTicketByIdQuery(GetClientTicketByIdQueryDto QueryDto)
    : IRequest<GetClientTicketByIdResponseDto>;