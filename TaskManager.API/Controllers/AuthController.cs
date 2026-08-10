using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Auth.Commands;

namespace TaskManager.API.Controllers;

/// <summary>
/// Handles user registration and authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">The new user's name, email and password.</param>
    /// <returns>A JWT access token for the newly created user.</returns>
    /// <response code="200">User created and authenticated.</response>
    /// <response code="400">Validation failed (e.g. email already in use).</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="command">The user's credentials.</param>
    /// <returns>A JWT access token.</returns>
    /// <response code="200">Login succeeded.</response>
    /// <response code="400">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
