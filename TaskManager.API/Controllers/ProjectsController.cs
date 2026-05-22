using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.API.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetProjectsByUserQuery(UserId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id, UserId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
    {
        var result = await _mediator.Send(command with { UserId = UserId });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectCommand command)
    {
        await _mediator.Send(command with { Id = id, UserId = UserId });
        return NoContent();
    }
}