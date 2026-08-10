using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Queries;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Controllers;

/// <summary>
/// Manages tasks within projects owned by the authenticated user.
/// </summary>
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

    /// <summary>
    /// Lists the tasks of a project, optionally filtered by status and/or priority.
    /// </summary>
    /// <param name="projectId">The project id.</param>
    /// <param name="status">Optional status filter (Pending, InProgress, Completed).</param>
    /// <param name="priority">Optional priority filter (Low, Medium, High).</param>
    /// <response code="200">Returns the matching tasks.</response>
    /// <response code="404">The project does not exist or does not belong to the user.</response>
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] Domain.Enums.TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId, UserId, status, priority));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new task in a project owned by the authenticated user.
    /// </summary>
    /// <param name="command">The task's title, description, priority, due date and project id.</param>
    /// <response code="200">The task was created.</response>
    /// <response code="400">Validation failed.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command)
    {
        var result = await _mediator.Send(command with { UserId = UserId });
        return Ok(result);
    }

    /// <summary>
    /// Updates the status of a task.
    /// </summary>
    /// <param name="id">The task id.</param>
    /// <param name="command">The new status.</param>
    /// <response code="204">The status was updated.</response>
    /// <response code="404">The task does not exist or does not belong to the user.</response>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusCommand command)
    {
        await _mediator.Send(command with { TaskId = id, UserId = UserId });
        return NoContent();
    }
}
