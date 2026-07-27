using MediatR;

namespace SmartDeskAI.Application.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(Guid UserId) : IRequest;