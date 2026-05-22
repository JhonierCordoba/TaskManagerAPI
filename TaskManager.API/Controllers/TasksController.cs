using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Queries;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] Domain.Enums.TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId, UserId, status, priority));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command)
    {
        var result = await _mediator.Send(command with { UserId = UserId });
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusCommand command)
    {
        await _mediator.Send(command with { TaskId = id, UserId = UserId });
        return NoContent();
    }
}