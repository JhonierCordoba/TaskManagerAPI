using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.API.Controllers;

/// <summary>
/// Manages projects owned by the authenticated user.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Lists all projects belonging to the authenticated user.
    /// </summary>
    /// <response code="200">Returns the user's projects.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetProjectsByUserQuery(UserId));
        return Ok(result);
    }

    /// <summary>
    /// Gets a single project by id.
    /// </summary>
    /// <param name="id">The project id.</param>
    /// <response code="200">Returns the project.</response>
    /// <response code="404">The project does not exist or does not belong to the user.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id, UserId));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new project for the authenticated user.
    /// </summary>
    /// <param name="command">The project's name and optional description.</param>
    /// <response code="201">The project was created.</response>
    /// <response code="400">Validation failed.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
    {
        var result = await _mediator.Send(command with { UserId = UserId });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing project owned by the authenticated user.
    /// </summary>
    /// <param name="id">The project id.</param>
    /// <param name="command">The updated name and description.</param>
    /// <response code="204">The project was updated.</response>
    /// <response code="404">The project does not exist or does not belong to the user.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectCommand command)
    {
        await _mediator.Send(command with { Id = id, UserId = UserId });
        return NoContent();
    }
}
