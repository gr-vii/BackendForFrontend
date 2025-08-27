using MediatR;
using BackendForFrontend.Application.Contracts.Auth;

namespace BackendForFrontend.Application.Commands.Auth;

public record LoginCommand(
    string Username,
    string Password
) : IRequest<LoginResponse>;

